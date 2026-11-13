#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    public class SingleInstanceManager : IDisposable
    {
        public event Action<string[]> ArgumentsReceived;

        public string MutexName { get; private set; }
        public string PipeName { get; private set; }
        public bool IsSingleInstance { get; private set; }
        public bool IsFirstInstance { get; private set; }

        private const int MaxArgumentsLength = 100;
        private const int FastConnectTimeout = 150;
        private const int ConnectTimeout = 2000;

        private readonly MutexManager mutex;
        private CancellationTokenSource cts;

        public SingleInstanceManager(string mutexName, string pipeName, string[] args) : this(mutexName, pipeName, true, args)
        {
        }

        public SingleInstanceManager(string mutexName, string pipeName, bool isSingleInstance, string[] args)
        {
            MutexName = mutexName;
            PipeName = pipeName;
            IsSingleInstance = isSingleInstance;

            if (!OperatingSystem.IsWindows())
            {
                string currentTmp = Environment.GetEnvironmentVariable("TMPDIR");
                if (string.IsNullOrEmpty(currentTmp) || currentTmp != "/tmp")
                {
                    Environment.SetEnvironmentVariable("TMPDIR", "/tmp");
                }
            }

            if (!IsSingleInstance)
            {
                IsFirstInstance = true;
                return;
            }

            // Fast IPC check: if an existing instance is already running and responds on the pipe,
            // forward arguments immediately, await confirmation (ACK), and exit without opening any window or UI.
            if (TryRedirectArguments(args, FastConnectTimeout))
            {
                IsFirstInstance = false;
                Environment.Exit(0);
                return;
            }

            mutex = new MutexManager(MutexName, 0);
            IsFirstInstance = mutex.HasHandle;

            if (IsFirstInstance)
            {
                cts = new CancellationTokenSource();

                Task.Run(ListenForConnectionsAsync, cts.Token);
            }
            else
            {
                if (RedirectArgumentsToFirstInstance(args))
                {
                    Environment.Exit(0);
                }
            }
        }

        protected virtual void OnArgumentsReceived(string[] arguments)
        {
            if (ArgumentsReceived != null)
            {
                Task.Run(() => ArgumentsReceived?.Invoke(arguments));
            }
        }

        private async Task ListenForConnectionsAsync()
        {
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    NamedPipeServerStream namedPipeServer;

                    if (OperatingSystem.IsWindows())
                    {
                        PipeSecurity pipeSecurity = new PipeSecurity();

                        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                        {
                            pipeSecurity.AddAccessRule(new PipeAccessRule(identity.User, PipeAccessRights.ReadWrite, AccessControlType.Allow));
                        }

                        namedPipeServer = NamedPipeServerStreamAcl.Create(PipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, 0, pipeSecurity);
                    }
                    else
                    {
                        namedPipeServer = new NamedPipeServerStream(PipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                    }

                    using (namedPipeServer)
                    {
                        await namedPipeServer.WaitForConnectionAsync(cts.Token).ConfigureAwait(false);

                        using (BinaryReader reader = new BinaryReader(namedPipeServer, Encoding.UTF8, leaveOpen: true))
                        {
                            int length = reader.ReadInt32();

                            if (length < 0 || length > MaxArgumentsLength)
                            {
                                throw new Exception("Invalid length: " + length);
                            }

                            string[] args = new string[length];

                            for (int i = 0; i < length; i++)
                            {
                                args[i] = reader.ReadString();
                            }

                            try
                            {
                                using (BinaryWriter writer = new BinaryWriter(namedPipeServer, Encoding.UTF8, leaveOpen: true))
                                {
                                    writer.Write(true);
                                    writer.Flush();
                                }
                            }
                            catch (Exception ex)
                            {
                                DebugHelper.WriteException(ex);
                            }

                            OnArgumentsReceived(args);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);

                    try
                    {
                        await Task.Delay(300, cts.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
        }

        private bool TryRedirectArguments(string[] args, int timeoutMs)
        {
            try
            {
                using (NamedPipeClientStream namedPipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut))
                {
                    namedPipeClient.Connect(timeoutMs);

                    AllowFirstInstanceToSetForeground(namedPipeClient);

                    using (BinaryWriter writer = new BinaryWriter(namedPipeClient, Encoding.UTF8, leaveOpen: true))
                    {
                        writer.Write(args.Length);

                        foreach (string argument in args)
                        {
                            writer.Write(argument);
                        }

                        writer.Flush();
                    }

                    try
                    {
                        using (BinaryReader reader = new BinaryReader(namedPipeClient, Encoding.UTF8, leaveOpen: true))
                        {
                            bool ack = reader.ReadBoolean();
                            if (ack)
                            {
                                DebugHelper.WriteLine("Arguments successfully sent to existing instance (ACK received).");
                                return true;
                            }
                        }
                    }
                    catch (EndOfStreamException)
                    {
                        DebugHelper.WriteLine("Arguments sent to existing instance (stream closed without explicit ACK).");
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteLine($"IPC redirection probe ({timeoutMs}ms) failed: {e.Message}");
            }

            return false;
        }

        private bool RedirectArgumentsToFirstInstance(string[] args)
        {
            const int maxRetries = 5;
            for (int i = 0; i < maxRetries; i++)
            {
                if (TryRedirectArguments(args, 300))
                {
                    return true;
                }

                Thread.Sleep(50);
            }

            return false;
        }

        // Only the foreground process may bring a window to front; others just flash in the taskbar.
        // This instance was usually started by the foreground process (for example Explorer), so it passes that right on to the first instance.
        private static void AllowFirstInstanceToSetForeground(NamedPipeClientStream namedPipeClient)
        {
            if (!OperatingSystem.IsWindows()) return;

            try
            {
                if (NativeMethods.GetNamedPipeServerProcessId(namedPipeClient.SafePipeHandle, out uint processId))
                {
                    NativeMethods.AllowSetForegroundWindow(processId);
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }

        public void Dispose()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            mutex?.Dispose();
        }
    }
}
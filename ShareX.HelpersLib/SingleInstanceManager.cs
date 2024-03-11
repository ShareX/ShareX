#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2024 ShareX Team

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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    public class SingleInstanceManager : IDisposable
    {
        public event EventHandler<ArgumentsReceivedEventArgs> ArgumentsReceived;

        public bool IsSingleInstance { get; private set; }
        public bool IsFirstInstance { get; private set; }

        private const string MutexName = "82E6AC09-0FEF-4390-AD9F-0DD3F5561EFC";
        private const string AppName = "ShareX";
        private static readonly string PipeName = $"{Environment.MachineName}-{Environment.UserName}-{AppName}";
        private const int MaxArgumentsLength = 100;

        private readonly Mutex mutex;
        private CancellationTokenSource cts;

        public SingleInstanceManager(bool isSingleInstance, string[] args)
        {
            IsSingleInstance = isSingleInstance;

            mutex = new Mutex(false, MutexName);

            if (IsSingleInstance)
            {
                try
                {
                    IsFirstInstance = mutex.WaitOne(100, false);

                    if (IsFirstInstance)
                    {
                        cts = new CancellationTokenSource();

                        Task.Run(ListenForConnectionsAsync, cts.Token);
                    }
                    else
                    {
                        RedirectArgumentsToFirstInstance(args);

                        DebugHelper.Logger.ProcessMessageQueue();
                        Environment.Exit(0);
                    }
                }
                catch (AbandonedMutexException)
                {
                    DebugHelper.WriteLine("Single instance mutex found abandoned from another process.");

                    IsFirstInstance = true;
                }
            }
            else
            {
                IsFirstInstance = true;
            }
        }

        protected virtual void OnArgumentsReceived(string[] arguments)
        {
            ArgumentsReceived?.Invoke(this, new ArgumentsReceivedEventArgs(arguments));
        }

        private async Task ListenForConnectionsAsync()
        {
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    using (NamedPipeServerStream serverPipe = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                    {
                        await serverPipe.WaitForConnectionAsync(cts.Token);

                        using (BinaryReader reader = new BinaryReader(serverPipe, Encoding.UTF8))
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

                            OnArgumentsReceived(args);
                        }
                    }
                }
                catch when (cts.IsCancellationRequested)
                {
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }
        }

        private void RedirectArgumentsToFirstInstance(string[] args)
        {
            try
            {
                using (NamedPipeClientStream clientPipe = new NamedPipeClientStream(".", PipeName, PipeDirection.Out))
                {
                    clientPipe.Connect();

                    using (BinaryWriter writer = new BinaryWriter(clientPipe, Encoding.UTF8))
                    {
                        writer.Write(args.Length);

                        foreach (string argument in args)
                        {
                            writer.Write(argument);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }

        public void Dispose()
        {
            cts?.Cancel();
            cts?.Dispose();
            mutex?.ReleaseMutex();
        }
    }

    public class ArgumentsReceivedEventArgs : EventArgs
    {
        public string[] Arguments { get; private set; }

        public ArgumentsReceivedEventArgs(string[] arguments)
        {
            Arguments = arguments;
        }
    }
}
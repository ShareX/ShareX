#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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

using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace ShareX.HelpersLib
{
    public class ApplicationInstanceManager : IDisposable
    {
        private static readonly string MutexName = "82E6AC09-0FEF-4390-AD9F-0DD3F5561EFC";
        private static readonly string AppName = "ShareX";
        private static readonly string PipeName = $"{Environment.MachineName}-{Environment.UserName}-{AppName}";
        private static readonly string SemaphoreName = PipeName + "Semaphore";

        public bool IsSingleInstance { get; private set; }
        public bool IsFirstInstance { get; private set; }

        private Mutex mutex;
        private Semaphore semaphore;
        private NamedPipeServerStream pipeServer;

        public ApplicationInstanceManager(bool isSingleInstance, string[] args, EventHandler<InstanceCallbackEventArgs> callback)
        {
            IsSingleInstance = isSingleInstance;

            mutex = new Mutex(false, MutexName);

            try
            {
                IsFirstInstance = mutex.WaitOne(100, false);

                if (IsSingleInstance && !IsFirstInstance)
                {
                    CreateMultipleInstance(args);
                }
            }
            catch (AbandonedMutexException)
            {
                // Log the mutex was abandoned in another process, it will still get acquired
                DebugHelper.WriteLine("Single instance mutex found abandoned from another process.");
                IsFirstInstance = true;
            }

            CreateFirstInstance(callback);
        }

        public void Dispose()
        {
            if (IsFirstInstance)
            {
                mutex.ReleaseMutex();
            }

            mutex.Dispose();
            semaphore?.Dispose();
            pipeServer?.Dispose();
        }

        private void CreateFirstInstance(EventHandler<InstanceCallbackEventArgs> callback)
        {
            try
            {
                semaphore = new Semaphore(1, 1, SemaphoreName, out var createdNew);
                // Mixing single instance and multi instance (via command line parameter) copies of the program can
                //  result in CreateFirstInstance being called if it isn't really the first one. Make sure this is
                //  really first instance by detecting if the semaphore was created
                if (!createdNew)
                {
                    return;
                }

                CreateServer(callback);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }

        private void CreateMultipleInstance(string[] args)
        {
            try
            {
                semaphore = Semaphore.OpenExisting(SemaphoreName);

                // Wait until the server is ready to accept data
                semaphore.WaitOne();
                SendDataToServer(args);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            Environment.Exit(0);
        }

        private void SendDataToServer(string[] args)
        {
            using (var pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.Out))
            {
                pipeClient.Connect();

                var pipeData = new InstanceCallbackEventArgs
                {
                    CommandLineArgs = args
                };

                var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pipeData));
                pipeClient.Write(bytes, 0, bytes.Length);
                pipeClient.Flush();
            }
        }

        private void CreateServer(EventHandler<InstanceCallbackEventArgs> callback)
        {
            pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            pipeServer.BeginWaitForConnection(ConnectionCallback, callback);
        }

        private void ConnectionCallback(IAsyncResult ar)
        {
            try
            {
                pipeServer.EndWaitForConnection(ar);
            }
            catch (ObjectDisposedException)
            {
                // Operation got aborted as part of program exit.
                return;
            }

            var callback = ar.AsyncState as EventHandler<InstanceCallbackEventArgs>;
            var sr = new StreamReader(pipeServer, Encoding.UTF8);
            try
            {
                if (callback != null)
                {
                    var data = sr.ReadToEnd();
                    callback(this, JsonConvert.DeserializeObject<InstanceCallbackEventArgs>(data));
                }
            }
            finally
            {
                // Close the existing server
                sr.Dispose();

                // Create a new server
                CreateServer(callback);

                // Signal that we are ready to accept a new connection
                semaphore.Release();
            }
        }
    }

    public class InstanceCallbackEventArgs : EventArgs
    {
        [JsonProperty]
        public string[] CommandLineArgs { get; internal set; }
    }
}
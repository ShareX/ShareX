#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Security.Permissions;
using System.Threading;

namespace ShareX.HelpersLib
{
    public class ApplicationInstanceManager : IDisposable
    {
        private static readonly string MutexName = "82E6AC09-0FEF-4390-AD9F-0DD3F5561EFC";
        private static readonly string AppName = "ShareX";
        private static readonly string EventName = string.Format("{0}-{1}-{2}", Environment.MachineName, Environment.UserName, AppName);
        private static readonly string SemaphoreName = string.Format("{0}{1}", EventName, "Semaphore");

        public bool IsSingleInstance { get; private set; }
        public bool IsFirstInstance { get; private set; }

        private Mutex mutex;
        private Semaphore semaphore;
        private IpcServerChannel serverChannel;

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
                DebugHelper.WriteLine("Single instance mutex found abandoned from another process");
                IsFirstInstance = true;
            }

            CreateFirstInstance(callback);
        }

        public void Dispose()
        {
            if (IsFirstInstance)
            {
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                }

                if (serverChannel != null)
                {
                    ChannelServices.UnregisterChannel(serverChannel);
                }

                if (semaphore != null)
                {
                    semaphore.Close();
                }
            }
        }

        private void CreateFirstInstance(EventHandler<InstanceCallbackEventArgs> callback)
        {
            try
            {
                bool createdNew;

                using (EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, EventName, out createdNew))
                {
                    // Mixing single instance and multi instance (via command line parameter) copies of the program can
                    //  result in CreateFirstInstance being called if it isn't really the first one. Make sure this is
                    //  really first instance by detecting if EventWaitHandle was created
                    if (!createdNew)
                    {
                        return;
                    }

                    semaphore = new Semaphore(1, 1, SemaphoreName);
                    ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, WaitOrTimerCallback, callback, Timeout.Infinite, false);

                    RegisterRemoteType(AppName);
                }
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
                InstanceProxy.CommandLineArgs = args;

                using (EventWaitHandle eventWaitHandle = EventWaitHandle.OpenExisting(EventName))
                {
                    semaphore = Semaphore.OpenExisting(SemaphoreName);
                    semaphore.WaitOne();
                    UpdateRemoteObject(AppName);

                    if (eventWaitHandle != null)
                    {
                        eventWaitHandle.Set();
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            Environment.Exit(0);
        }

        private void UpdateRemoteObject(string uri)
        {
            IpcClientChannel clientChannel = new IpcClientChannel();
            ChannelServices.RegisterChannel(clientChannel, true);

            InstanceProxy proxy = Activator.GetObject(typeof(InstanceProxy), string.Format("ipc://{0}{1}{2}/{2}", Environment.MachineName, Environment.UserName, uri)) as InstanceProxy;

            if (proxy != null)
            {
                proxy.SetCommandLineArgs(InstanceProxy.CommandLineArgs);
            }

            ChannelServices.UnregisterChannel(clientChannel);
        }

        private void RegisterRemoteType(string uri)
        {
            serverChannel = new IpcServerChannel(Environment.MachineName + Environment.UserName + uri);
            ChannelServices.RegisterChannel(serverChannel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(InstanceProxy), uri, WellKnownObjectMode.Singleton);
        }

        private void WaitOrTimerCallback(object state, bool timedOut)
        {
            EventHandler<InstanceCallbackEventArgs> callback = state as EventHandler<InstanceCallbackEventArgs>;

            if (callback != null)
            {
                try
                {
                    callback(state, new InstanceCallbackEventArgs(InstanceProxy.CommandLineArgs));
                }
                finally
                {
                    if (semaphore != null)
                    {
                        semaphore.Release();
                    }
                }
            }
        }
    }

    [Serializable]
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    internal class InstanceProxy : MarshalByRefObject
    {
        public static string[] CommandLineArgs { get; internal set; }

        public void SetCommandLineArgs(string[] commandLineArgs)
        {
            CommandLineArgs = commandLineArgs;
        }
    }

    public class InstanceCallbackEventArgs : EventArgs
    {
        public string[] CommandLineArgs { get; private set; }

        internal InstanceCallbackEventArgs(string[] commandLineArgs)
        {
            CommandLineArgs = commandLineArgs;
        }
    }
}
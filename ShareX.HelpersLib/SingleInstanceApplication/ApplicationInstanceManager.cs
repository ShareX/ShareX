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
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;

namespace SingleInstanceApplication
{
    public static class ApplicationInstanceManager
    {
        private static Semaphore semaphore;
        private static string appName = "ShareX";
        private static string eventName = string.Format("{0}-{1}", Environment.MachineName, appName);
        private static string semaphoreName = string.Format("{0}{1}", eventName, "Semaphore");

        public static void CreateFirstInstance(EventHandler<InstanceCallbackEventArgs> callback)
        {
            bool createdNew;

            using (EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, eventName, out createdNew))
            {
                // Mixing single instance and multi instance (via command line parameter) copies of the program can
                //  result in CreateFirstInstance being called if it isn't really the first one. Make sure this is
                //  really first instance by detecting if EventWaitHandle was created
                if (createdNew != true)
                {
                    return;
                }

                semaphore = new Semaphore(1, 1, semaphoreName);
                ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, WaitOrTimerCallback, callback, Timeout.Infinite, false);

                RegisterRemoteType(appName);
            }
        }

        public static void CreateMultipleInstance(EventHandler<InstanceCallbackEventArgs> callback, string[] args)
        {
            InstanceProxy.CommandLineArgs = args;

            using (EventWaitHandle eventWaitHandle = EventWaitHandle.OpenExisting(eventName))
            {
                semaphore = Semaphore.OpenExisting(semaphoreName);
                semaphore.WaitOne();
                UpdateRemoteObject(appName);

                if (eventWaitHandle != null) eventWaitHandle.Set();
            }

            Environment.Exit(0);
        }

        private static void UpdateRemoteObject(string uri)
        {
            IpcClientChannel clientChannel = new IpcClientChannel();
            ChannelServices.RegisterChannel(clientChannel, true);

            InstanceProxy proxy = Activator.GetObject(typeof(InstanceProxy), string.Format("ipc://{0}{1}/{1}", Environment.MachineName, uri)) as InstanceProxy;

            if (proxy != null)
            {
                proxy.SetCommandLineArgs(InstanceProxy.CommandLineArgs);
            }

            ChannelServices.UnregisterChannel(clientChannel);
        }

        private static void RegisterRemoteType(string uri)
        {
            IpcServerChannel serverChannel = new IpcServerChannel(Environment.MachineName + uri);
            ChannelServices.RegisterChannel(serverChannel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(InstanceProxy), uri, WellKnownObjectMode.Singleton);

            Process process = Process.GetCurrentProcess();
            process.Exited += delegate
            {
                ChannelServices.UnregisterChannel(serverChannel);
                semaphore.Close();
            };
        }

        private static void WaitOrTimerCallback(object state, bool timedOut)
        {
            EventHandler<InstanceCallbackEventArgs> callback = state as EventHandler<InstanceCallbackEventArgs>;
            if (callback == null) return;

            callback(state, new InstanceCallbackEventArgs(InstanceProxy.CommandLineArgs));

            semaphore.Release();
        }
    }
}
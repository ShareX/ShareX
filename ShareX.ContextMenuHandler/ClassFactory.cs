using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ShareX.ContextMenuHandler
{
    [ComVisible(true)]
    public class ContextMenuClassFactory : IClassFactory
    {
        public void CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject)
        {
            if (pUnkOuter != IntPtr.Zero)
            {
                const int CLASS_E_NOAGGREGATION = unchecked((int)0x80040110);
                Marshal.ThrowExceptionForHR(CLASS_E_NOAGGREGATION);
            }

            var obj = new ShareXContextMenuHandler();
            var pUnk = Marshal.GetIUnknownForObject(obj);

            var ex = Marshal.GetExceptionForHR(Marshal.QueryInterface(pUnk, ref riid, out ppvObject));
            Marshal.Release(pUnk);
            if (ex != null)
            {
                throw ex;
            }    
        }

        public void LockServer(bool fLock)
        {
            if (fLock)
            {
                NativeMethods.CoAddRefServerProcess();
            }
            else
            {
                ReleaseServerProcess();
            }
        }

        private static Thread runThread;

        public static void ReleaseServerProcess()
        {
            var count = NativeMethods.CoReleaseServerProcess();

            // There are no remaining outstanding references, so we can shut down the server.
            if (count == 0)
            {
                runThread.Interrupt();
            }
        }

        public static void Run()
        {
            var timer = new Timer(_ => GC.Collect(), null, TimeSpan.Zero, TimeSpan.FromSeconds(6));
            runThread = Thread.CurrentThread;
            var factory = new ContextMenuClassFactory();
            NativeMethods.CoRegisterClassObject(
                typeof(ShareXContextMenuHandler).GUID, factory, RegistrationClassContext.LocalServer, RegistrationConnectionType.MultipleUse, out var cookie);

            try
            {
                Thread.Sleep(Timeout.Infinite);
            }
            catch (ThreadInterruptedException)
            {
                NativeMethods.CoRevokeClassObject(cookie);
            }

            GC.KeepAlive(timer);
        }
    }
}

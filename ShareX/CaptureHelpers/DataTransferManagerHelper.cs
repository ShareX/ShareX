#if WindowsStore || WindowsStoreDebug
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;

namespace ShareX
{
    public class DataTransferManagerHelper
    {
        const string DataTransferManagerId = "a5caee9b-8708-49d1-8d36-67d25a8da00c";
        IDataTransferManagerInterOp interop = null;
        IntPtr windowHandle;

        public DataTransferManager DataTransferManager
        {
            get;
            private set;
        } = null;

        public DataTransferManagerHelper(IntPtr handle)
        {
            //TODO: Add a check for failure here. This will fail for versions of Windows
            //below Windows 10
            IActivationFactory factory = WindowsRuntimeMarshal.GetActivationFactory(typeof(DataTransferManager));

            interop = (IDataTransferManagerInterOp)factory;

            windowHandle = handle;
            DataTransferManager dtm = null;
            Guid riid = new Guid(DataTransferManagerId);
            interop.GetForWindow(windowHandle, riid, out dtm);

            this.DataTransferManager = dtm;
        }

        public void ShowShareUI()
        {
            interop.ShowShareUIForWindow(windowHandle);
        }
    }

    /// <summary>
    /// The IDataTransferManagerInterOp is documented here: https://msdn.microsoft.com/en-us/library/windows/desktop/jj542488(v=vs.85).aspx.
    /// This interface allows an app to tie the share context to a specific
    /// window using a window handle. Useful for Win32 apps.
    /// </summary>
    [ComImport, Guid("3A3DCD6C-3EAB-43DC-BCDE-45671CE800C8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDataTransferManagerInterOp
    {
        /// <summary>
        /// Get an instance of Windows.ApplicationModel.DataTransfer.DataTransferManager
        /// for the window identified by a window handle
        /// </summary>
        /// <param name="appWindow">The window handle</param>
        /// <param name="riid">ID of the DataTransferManager interface</param>
        /// <param name="pDataTransferManager">The DataTransferManager instance for this window handle</param>
        /// <returns>HRESULT</returns>
        [PreserveSig]
        uint GetForWindow([In] IntPtr appWindow, [In] ref Guid riid, [Out] out DataTransferManager pDataTransferManager);

        /// <summary>
        /// Show the share flyout for the window identified by a window handle
        /// </summary>
        /// <param name="appWindow">The window handle</param>
        /// <returns>HRESULT</returns>
        [PreserveSig]
        uint ShowShareUIForWindow(IntPtr appWindow);
    }
}
#endif
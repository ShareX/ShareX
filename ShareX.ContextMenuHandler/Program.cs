using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ShareX.ContextMenuHandler
{
    internal class Program
    {   
        static void Main(string[] args)
        {
            ContextMenuClassFactory.Run();
        }
    }

    [ComVisible(true)]
    [Guid("4B905A2A-D71C-48D1-AE87-33B15375E13D")]
    [ClassInterface(ClassInterfaceType.None)]
    public class ShareXContextMenuHandler : IExplorerCommand
    {
        public ShareXContextMenuHandler()
        {
            NativeMethods.CoAddRefServerProcess();
        }

        ~ShareXContextMenuHandler()
        {
            ContextMenuClassFactory.ReleaseServerProcess();
        }

        public void GetTitle(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName)
        {
            ppszName = "Upload with ShareX";
        }

        public void GetIcon(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszIcon)
        {
            ppszIcon = GetShareXExePath() + ",-32512";
        }

        public void GetToolTip(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszInfotip)
        {
            throw new NotImplementedException();
        }

        public void GetCanonicalName(out Guid pguidCommandName)
        {
            pguidCommandName = default;
        }

        public void GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState)
        {
            psiItemArray.GetCount(out var count);
            // For now, we only support selecting one item at a time.
            // Implementing support for multi-selection may need better UI in ShareX.
            if (count != 1)
            {
                pCmdState = EXPCMDSTATE.ECS_HIDDEN;
                return;
            }

            // If none of the items have a filesystem path, we shouldn't show the menu item.
            psiItemArray.GetAttributes(SIATTRIBFLAGS.SIATTRIBFLAGS_OR, SFGAO.FILESYSTEM, out var attribs);
            if (attribs == 0)
            {
                pCmdState = EXPCMDSTATE.ECS_HIDDEN;
                return;
            }

            pCmdState = EXPCMDSTATE.ECS_ENABLED;
        }

        public void Invoke(IShellItemArray psiItemArray, IBindCtx pbc)
        {
            psiItemArray.GetItemAt(0, out var shellItem);
            shellItem.GetDisplayName(SIGDN.FILESYSPATH, out var name);
            Process.Start(GetShareXExePath(), $"\"{name}\"");
        }

        public void GetFlags(out EXPCMDFLAGS pFlags)
        {
            pFlags = EXPCMDFLAGS.ECF_DEFAULT;
        }

        public void EnumSubCommands(out object ppenum)
        {
            throw new NotImplementedException();
        }

        private static string GetShareXExePath()
        {
            var thisLocation = typeof(ShareXContextMenuHandler).Assembly.Location;
            var dir = Path.GetDirectoryName(thisLocation);
            return Path.Combine(dir, "ShareX.exe");
        }
    }
}
using ShareX.HelpersLib; // Don't remove this, it's required for the Steam build
using System.Windows.Forms;

namespace ShareX.StartupManagers
{
    class StartupManagerFactory
    {
        static public IStartupManager GetStartupManager()
        {
#if WindowsStore
            return new CentennialStartupManager()
            {
                StartupTargetIndex = 0
            };
#elif STEAM
            return new DesktopStartupManager()
            {
                StartupTargetPath = Helpers.GetAbsolutePath("../ShareX_Launcher.exe")
            };
#else
            return new DesktopStartupManager()
            {
                StartupTargetPath = Application.ExecutablePath
            };
#endif
        }
    }
}

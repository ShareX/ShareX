using System.Windows.Forms;

namespace ShareX.StartupManagers
{
    public class DesktopStartupManager : GenericStartupManager
    {
        public override string StartupTargetPath() => Application.ExecutablePath;
    }
}

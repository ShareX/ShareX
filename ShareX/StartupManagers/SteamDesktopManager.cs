using ShareX.HelpersLib;

namespace ShareX.StartupManagers
{
    public class SteamStartupManager : GenericStartupManager
    {
        public override string StartupTargetPath() => Helpers.GetAbsolutePath("../ShareX_Launcher.exe");
    }
}

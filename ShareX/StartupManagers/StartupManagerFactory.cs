namespace ShareX.StartupManagers
{
    public class StartupManagerFactory
    {
        public static IStartupManager GetStartupManager()
        {
#if WindowsStore
            return new CentennialStartupManager();
#elif STEAM
            return new SteamStartupManager();
#else
            return new DesktopStartupManager();
#endif
        }
    }
}

#if WindowsStore
namespace ShareX.StartupManagers
{
    class CentennialStartupManager : IStartupManager
    {
        public int StartupTargetIndex;
        public StartupTaskState State
        {
            get => IntegrationHelpers.CheckStartupWindowsStore();
            set => IntegrationHelpers.ConfigureStartupWindowsStore(value == StartupTaskState.Enabled);
        }
    }
}
#endif
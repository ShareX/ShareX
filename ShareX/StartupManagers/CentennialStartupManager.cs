#if WindowsStore
using System;
using Windows.ApplicationModel;

namespace ShareX.StartupManagers
{
    public class CentennialStartupManager : IStartupManager
    {
        public int StartupTargetIndex;
        public StartupTaskState State
        {
            get => (StartupTaskState)StartupTask.GetForCurrentPackageAsync().GetResults()[StartupTargetIndex].State;
            set
            {
                if (value == StartupTaskState.Enabled)
                {
                    StartupTask.GetForCurrentPackageAsync().GetResults()[StartupTargetIndex].RequestEnableAsync().GetResults();
                }
                else if (value == StartupTaskState.Disabled)
                {
                    StartupTask.GetForCurrentPackageAsync().GetResults()[StartupTargetIndex].Disable();
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}
#endif
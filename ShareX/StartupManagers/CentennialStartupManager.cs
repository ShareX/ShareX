using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

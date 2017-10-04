using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareX.StartupManagers
{
    class CentennialStartupManager : IStartupManager
    {
        public StartupTaskState State
        {
            get => IntegrationHelpers.CheckStartupWindowsStore();
            set
            {
                bool enable;
                if (value == StartupTaskState.Enabled)
                {
                    enable = true;
                }
                else
                {
                    enable = false;
                }

                IntegrationHelpers.ConfigureStartupWindowsStore(enable);
            }
        }
    }
}

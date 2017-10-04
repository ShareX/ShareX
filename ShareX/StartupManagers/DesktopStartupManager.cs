using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareX.StartupManagers
{
    class DesktopStartupManager : IStartupManager
    {
        public StartupTaskState State
        {
            get => IntegrationHelpers.CheckStartupShortcut() ? StartupTaskState.Enabled : StartupTaskState.Disabled;
            set
            {
                if (value == StartupTaskState.Disabled)
                    IntegrationHelpers.CreateStartupShortcut(false);
                else if (value == StartupTaskState.Enabled)
                    IntegrationHelpers.CreateStartupShortcut(true);
            }
        }
    }
}

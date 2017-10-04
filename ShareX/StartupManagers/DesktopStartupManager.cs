#if !WindowsStore
using ShareX.HelpersLib;
using System;

namespace ShareX.StartupManagers
{
    public class DesktopStartupManager : IStartupManager
    {
        public string StartupTargetPath;
        public StartupTaskState State
        {
            get => ShortcutHelpers.CheckShortcut(Environment.SpecialFolder.Startup, StartupTargetPath) ? StartupTaskState.Enabled : StartupTaskState.Disabled;
            set
            {
                if (value == StartupTaskState.Enabled || value == StartupTaskState.Disabled)
                {
                    ShortcutHelpers.SetShortcut(value == StartupTaskState.Enabled, Environment.SpecialFolder.Startup, StartupTargetPath, "-silent");
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
using ShareX.HelpersLib;
using System;

namespace ShareX.StartupManagers
{
    class DesktopStartupManager : IStartupManager
    {
        public string StartupTargetPath;
        public StartupTaskState State
        {
            get => ShortcutHelpers.CheckShortcut(Environment.SpecialFolder.Startup, StartupTargetPath) ? StartupTaskState.Enabled : StartupTaskState.Disabled;
            set => ShortcutHelpers.SetShortcut(value == StartupTaskState.Enabled, Environment.SpecialFolder.Startup, StartupTargetPath, "-silent");
        }
    }
}

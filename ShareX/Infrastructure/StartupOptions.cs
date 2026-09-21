#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

namespace ShareX;

internal static class StartupOptions
{
    internal static bool MultiInstance { get; set; }
    internal static bool Portable { get; set; }
    internal static bool SilentRun { get; set; }
    internal static bool Sandbox { get; set; }
    internal static bool IsAdmin { get; set; }
    internal static bool IgnoreHotkeyWarning { get; set; }
    internal static bool AutoClose { get; set; }
}

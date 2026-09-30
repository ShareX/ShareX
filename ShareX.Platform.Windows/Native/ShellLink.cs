using System.Runtime.InteropServices;

namespace ShareX.Platform.Windows.Native;

// Windows Script Host shell link, the same COM objects ShareX.HelpersLib uses to create Startup folder shortcuts.

[ComImport, Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")]
internal class WshShell
{
}

[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), Guid("F935DC21-1CF0-11D0-ADB9-00C04FD58A0B")]
internal interface IWshShell
{
    [DispId(0x3ea)]
    IWshShortcut CreateShortcut(string pathLink);
}

[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), Guid("F935DC23-1CF0-11D0-ADB9-00C04FD58A0B")]
internal interface IWshShortcut
{
    [DispId(0)]
    string FullName { get; }

    [DispId(0x3e8)]
    string Arguments { get; set; }

    [DispId(0x3e9)]
    string Description { get; set; }

    [DispId(0x3ea)]
    string Hotkey { get; set; }

    [DispId(0x3eb)]
    string IconLocation { get; set; }

    [DispId(0x3ec)]
    string RelativePath { set; }

    [DispId(0x3ed)]
    string TargetPath { get; set; }

    [DispId(0x3ee)]
    int WindowStyle { get; set; }

    [DispId(0x3ef)]
    string WorkingDirectory { get; set; }

    [DispId(0x7d0)]
    void Load([In] string pathLink);

    [DispId(0x7d1)]
    void Save();
}

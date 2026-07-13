#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

namespace ShareX.AvaloniaUI.Windows;

public class ScreenColorPickerOptions
{
    public string Format { get; set; } = "$HEX";

    public string FormatCtrl { get; set; } = "$r255, $g255, $b255";

    public string InfoText { get; set; } = "#$HEX";

    public bool ShowMagnifier { get; set; } = true;
}

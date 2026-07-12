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

using System.Drawing;

namespace ShareX.Tools;

public sealed class PinToScreenOptions
{
    public int InitialScale { get; set; } = 100;
    public int ScaleStep { get; set; } = 10;
    public bool HighQualityScale { get; set; } = true;
    public int InitialOpacity { get; set; } = 100;
    public int OpacityStep { get; set; } = 10;
    public ContentAlignment Placement { get; set; } = ContentAlignment.BottomRight;
    public int PlacementOffset { get; set; } = 10;
    public bool TopMost { get; set; } = true;
    public bool KeepCenterLocation { get; set; } = true;
    public Color BackgroundColor { get; set; } = Color.White;
    public bool Shadow { get; set; } = true;
    public bool Border { get; set; } = true;
    public int BorderSize { get; set; } = 2;
    public Color BorderColor { get; set; } = Color.CornflowerBlue;
    public Size MinimizeSize { get; set; } = new(100, 100);
}

public sealed record PinToScreenSource(byte[] ImageData, Point? Location = null);

public sealed class PinToScreenServices
{
    public required Func<Task<PinToScreenSource?>> CaptureRegionAsync { get; init; }
    public required Func<Task<PinToScreenSource?>> GetClipboardImageAsync { get; init; }
    public required Func<Task<PinToScreenSource?>> SelectImageFileAsync { get; init; }
    public required Action<byte[]> CopyImage { get; init; }
    public Action? ImagePinned { get; init; }
}

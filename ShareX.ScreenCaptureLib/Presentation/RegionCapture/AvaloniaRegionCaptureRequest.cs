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

#nullable enable

using ShareX.ImageEditor.Integration;
using SkiaSharp;
using System.Drawing;

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

public sealed class AvaloniaRegionCaptureRequest
{
    /// <summary>Frozen screenshot. Ownership transfers to the capture window.</summary>
    public required SKBitmap Screenshot { get; init; }

    /// <summary>Physical-pixel desktop bounds represented by <see cref="Screenshot"/>.</summary>
    public required Rectangle ScreenBounds { get; init; }

    public required RegionCaptureOptions RegionCaptureOptions { get; init; }

    public required ImageEditorOptions ImageEditorOptions { get; init; }

    public bool EnableAnnotations { get; init; } = true;

    /// <summary>Optional captured cursor bitmap. Ownership transfers to the capture window.</summary>
    public SKBitmap? CursorBitmap { get; init; }

    /// <summary>Cursor draw position in screenshot pixel coordinates.</summary>
    public Point CursorPosition { get; init; }
}

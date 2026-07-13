#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using System.Drawing;

namespace ShareX.ImageEffectsLib;

public enum ImageEffectsWindowMode
{
    Presets,
    Tool,
    Editor
}

public sealed record ImageEffectsSource(Bitmap Image, string? FilePath = null);

public sealed class ImageEffectsCallbacks
{
    public Func<ImageEffectsSource?>? LoadImageFromFile { get; init; }
    public Func<ImageEffectsSource?>? LoadImageFromClipboard { get; init; }
    public Func<Bitmap, string?, string?>? SaveImage { get; init; }
    public Action<Bitmap>? UploadImage { get; init; }
    public Action? OpenImageEffectsPage { get; init; }
}

public sealed record ImageEffectsDialogResult(bool Accepted, int SelectedPresetIndex);

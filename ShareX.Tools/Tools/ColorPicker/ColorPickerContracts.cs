#region License Information (GPL v3)

/* ShareX - Copyright (c) 2007-2026 ShareX Team - GPL v3 */

#endregion License Information (GPL v3)

namespace ShareX.Tools;

public sealed record ColorPickerSample(int Argb, int X, int Y);

public sealed class ColorPickerServices
{
    public required Func<ColorPickerSample?> PickScreenColor { get; init; }
}

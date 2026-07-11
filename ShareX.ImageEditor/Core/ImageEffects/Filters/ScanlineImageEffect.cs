#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class ScanlineImageEffect : ImageEffectBase
{
    public override string Id => "scanline";
    public override string Name => "Scanlines";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.scan_line;
    public override string Description => "Adds retro CRT-style horizontal scanlines.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<ScanlineImageEffect>("spacing", "Line spacing", 2, 20, 4, (e, v) => e.Spacing = v),
        EffectParameters.FloatSlider<ScanlineImageEffect>("opacity", "Opacity", 0, 100, 50, (e, v) => e.Opacity = v)
    ];

    public int Spacing { get; set; } = 4;
    public float Opacity { get; set; } = 50f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        float alpha = Math.Clamp(Opacity / 100f, 0f, 1f);
        if (alpha <= 0f) return source.Copy();

        SKBitmap result = source.Copy();
        using SKCanvas canvas = new(result);
        using SKPaint paint = new()
        {
            Color = new SKColor(0, 0, 0, (byte)(255 * alpha)),
            StrokeWidth = 1,
            IsAntialias = false
        };

        int step = Math.Max(2, Spacing);
        for (int y = 0; y < source.Height; y += step)
        {
            canvas.DrawLine(0, y, source.Width, y, paint);
        }

        return result;
    }
}
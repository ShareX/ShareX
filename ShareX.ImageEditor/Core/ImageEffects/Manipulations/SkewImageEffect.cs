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

using ShareX.AvaloniaUI.Theming;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Manipulations;

public sealed class SkewImageEffect : ImageEffectBase
{
    public override string Id => "skew";
    public override string Name => "Skew";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.move_diagonal;
    public override string Description => "Skews the image horizontally and vertically.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<SkewImageEffect>("horizontally", "Horizontally", -89f, 89f, 0f, (e, v) => e.Horizontally = v),
        EffectParameters.FloatSlider<SkewImageEffect>("vertically", "Vertically", -89f, 89f, 0f, (e, v) => e.Vertically = v),
        EffectParameters.Bool<SkewImageEffect>("auto_resize", "Auto resize", true, (e, v) => e.AutoResize = v)
    ];

    public float Horizontally { get; set; } = 0;
    public float Vertically { get; set; } = 0;

    /// <summary>
    /// When true, the output canvas expands to fit the full skewed image.
    /// When false, the output keeps the original dimensions.
    /// </summary>
    public bool AutoResize { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (Horizontally == 0 && Vertically == 0) return source.Copy();

        float w = source.Width;
        float h = source.Height;

        // Create skew matrix
        SKMatrix matrix = SKMatrix.CreateSkew(
            (float)Math.Tan(Horizontally * Math.PI / 180),
            (float)Math.Tan(Vertically * Math.PI / 180)
        );

        // Map corners to find actual bounds
        SKPoint[] corners = new SKPoint[]
        {
            new SKPoint(0, 0),
            new SKPoint(w, 0),
            new SKPoint(w, h),
            new SKPoint(0, h)
        };

        SKPoint[] mapped = matrix.MapPoints(corners);

        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;
        foreach (var pt in mapped)
        {
            minX = Math.Min(minX, pt.X);
            minY = Math.Min(minY, pt.Y);
            maxX = Math.Max(maxX, pt.X);
            maxY = Math.Max(maxY, pt.Y);
        }

        int newWidth, newHeight;
        float offsetX, offsetY;

        if (AutoResize)
        {
            newWidth = Math.Max(1, (int)Math.Ceiling(maxX - minX));
            newHeight = Math.Max(1, (int)Math.Ceiling(maxY - minY));
            offsetX = -minX;
            offsetY = -minY;
        }
        else
        {
            newWidth = (int)w;
            newHeight = (int)h;
            offsetX = 0;
            offsetY = 0;
        }

        SKBitmap result = new SKBitmap(newWidth, newHeight, source.ColorType, source.AlphaType);
        using (SKCanvas canvas = new SKCanvas(result))
        {
            canvas.Clear(SKColors.Transparent);
            canvas.Translate(offsetX, offsetY);
            canvas.SetMatrix(canvas.TotalMatrix.PreConcat(matrix));
            canvas.DrawBitmap(source, 0, 0);
        }

        return result;
    }
}
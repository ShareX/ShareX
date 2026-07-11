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

/* TODO: SkiaSharp bug
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Manipulations;

public sealed class Rotate3DBoxImageEffect : ImageEffectBase
{
    public override string Id => "rotate_3d_box";
    public override string Name => "3D Box / Extrude";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.box;
    public override string Description => "Creates a 3D box extrusion effect with rotation.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<Rotate3DBoxImageEffect>("depth", "Depth", 0f, 500f, 50f, (e, v) => e.Depth = v),
        EffectParameters.FloatSlider<Rotate3DBoxImageEffect>("rotate_x", "Rotate X", -180f, 180f, 0f, (e, v) => e.RotateX = v),
        EffectParameters.FloatSlider<Rotate3DBoxImageEffect>("rotate_y", "Rotate Y", -180f, 180f, 0f, (e, v) => e.RotateY = v),
        EffectParameters.FloatSlider<Rotate3DBoxImageEffect>("rotate_z", "Rotate Z", -180f, 180f, 0f, (e, v) => e.RotateZ = v),
        EffectParameters.Bool<Rotate3DBoxImageEffect>("auto_resize", "Auto resize", true, (e, v) => e.AutoResize = v)
    ];

    /// <summary>
    /// Depth of the 3D box or extrusion in pixels.
    /// </summary>
    public float Depth { get; set; } = 50;

    /// <summary>
    /// Rotation around the X-axis in degrees (-180 to 180).
    /// </summary>
    public float RotateX { get; set; } = 0;

    /// <summary>
    /// Rotation around the Y-axis in degrees (-180 to 180).
    /// </summary>
    public float RotateY { get; set; } = 0;

    /// <summary>
    /// Rotation around the Z-axis in degrees (-180 to 180).
    /// </summary>
    public float RotateZ { get; set; } = 0;

    /// <summary>
    /// Whether to resize the output bitmap to fit the transformed image bounds.
    /// </summary>
    public bool AutoResize { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (RotateX == 0 && RotateY == 0 && RotateZ == 0 && Depth == 0) return source.Copy();

        float width = source.Width;
        float height = source.Height;
        float centerX = width / 2f;
        float centerY = height / 2f;

        float depthFactor = Math.Max(width, height) * 2f;

        SKMatrix44 CreateTransform(float localZ)
        {
            SKMatrix44 tzMat = SKMatrix44.CreateIdentity();
            tzMat.PostConcat(SKMatrix44.CreateTranslation(-centerX, -centerY, localZ));
            if (RotateX != 0) tzMat.PostConcat(SKMatrix44.CreateRotationDegrees(1, 0, 0, RotateX));
            if (RotateY != 0) tzMat.PostConcat(SKMatrix44.CreateRotationDegrees(0, 1, 0, RotateY));
            if (RotateZ != 0) tzMat.PostConcat(SKMatrix44.CreateRotationDegrees(0, 0, 1, RotateZ));

            SKMatrix44 perspective = SKMatrix44.CreateIdentity();
            perspective[3, 2] = -1f / depthFactor;
            tzMat.PostConcat(perspective);
            tzMat.PostConcat(SKMatrix44.CreateTranslation(centerX, centerY, 0));
            return tzMat;
        }

        // --- Solid Box Mode ---
        // Draw a solid 3D box using geometric planes and edge-sampled colors

        var frontMat44 = CreateTransform(0);
        var backMat44 = CreateTransform(-Depth);

        var frontMat = frontMat44.Matrix;
        var backMat = backMat44.Matrix;

        SKPoint[] cornersP = { new SKPoint(0, 0), new SKPoint(width, 0), new SKPoint(width, height), new SKPoint(0, height) };
        SKPoint[] frontPoints = new SKPoint[4];
        SKPoint[] backPoints = new SKPoint[4];

        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        for (int i = 0; i < 4; i++)
        {
            frontPoints[i] = frontMat.MapPoint(cornersP[i]);
            backPoints[i] = backMat.MapPoint(cornersP[i]);

            minX = Math.Min(minX, Math.Min(frontPoints[i].X, backPoints[i].X));
            minY = Math.Min(minY, Math.Min(frontPoints[i].Y, backPoints[i].Y));
            maxX = Math.Max(maxX, Math.Max(frontPoints[i].X, backPoints[i].X));
            maxY = Math.Max(maxY, Math.Max(frontPoints[i].Y, backPoints[i].Y));
        }

        int newWidth = AutoResize ? (int)Math.Ceiling(maxX - minX) : (int)width;
        int newHeight = AutoResize ? (int)Math.Ceiling(maxY - minY) : (int)height;
        newWidth = Math.Max(1, newWidth);
        newHeight = Math.Max(1, newHeight);

        SKBitmap result = new SKBitmap(newWidth, newHeight, source.ColorType, source.AlphaType);
        using (SKCanvas canvas = new SKCanvas(result))
        {
            canvas.Clear(SKColors.Transparent);

            if (AutoResize) canvas.Translate(-minX, -minY);

            // Sample edge average colors
            SKColor topColor = source.GetPixel((int)centerX, 0);
            SKColor bottomColor = source.GetPixel((int)centerX, (int)(height - 1));
            SKColor leftColor = source.GetPixel(0, (int)centerY);
            SKColor rightColor = source.GetPixel((int)(width - 1), (int)centerY);

            void DrawFace(SKPoint f1, SKPoint f2, SKPoint b1, SKPoint b2, SKColor color, float shade)
            {
                using SKPath path = new SKPath();
                path.MoveTo(f1);
                path.LineTo(f2);
                path.LineTo(b2);
                path.LineTo(b1);
                path.Close();

                byte r = (byte)(color.Red * shade);
                byte g = (byte)(color.Green * shade);
                byte b = (byte)(color.Blue * shade);
                using SKPaint p = new SKPaint { Color = new SKColor(r, g, b, 255), IsAntialias = true };

                // Minor stroke to prevent subpixel bleeding
                p.Style = SKPaintStyle.Fill;
                canvas.DrawPath(path, p);
                p.Style = SKPaintStyle.Stroke;
                p.StrokeWidth = 1f;
                canvas.DrawPath(path, p);
            }

            bool IsVisible(SKPoint f1, SKPoint f2, SKPoint b1)
            {
                float vx1 = f2.X - f1.X;
                float vy1 = f2.Y - f1.Y;
                float vx2 = b1.X - f1.X;
                float vy2 = b1.Y - f1.Y;
                return (vx1 * vy2 - vy1 * vx2) > 0;
            }

            // Top / Bottom Face
            if (!IsVisible(frontPoints[0], frontPoints[1], backPoints[0]))
                DrawFace(frontPoints[0], frontPoints[1], backPoints[0], backPoints[1], topColor, 0.85f);
            else
                DrawFace(frontPoints[3], frontPoints[2], backPoints[3], backPoints[2], bottomColor, 0.85f);

            // Left / Right Face
            if (IsVisible(frontPoints[0], frontPoints[3], backPoints[0]))
                DrawFace(frontPoints[0], frontPoints[3], backPoints[0], backPoints[3], leftColor, 0.7f);
            else
                DrawFace(frontPoints[1], frontPoints[2], backPoints[1], backPoints[2], rightColor, 0.7f);

            // Draw Front face
            canvas.ResetMatrix();
            if (AutoResize) canvas.Translate(-minX, -minY);
            canvas.Concat(in frontMat);

            using SKPaint frontPaint = new SKPaint { IsAntialias = true };
            using SKImage sourceImage = SKImage.FromBitmap(source);
            canvas.DrawImage(sourceImage, 0, 0, new SKSamplingOptions(SKCubicResampler.CatmullRom), frontPaint);
        }
        return result;
    }
}
*/
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

namespace ShareX.ImageEditor.Core.ImageEffects.Drawings;

public sealed class DrawParticlesEffect : ImageEffectBase
{
    private static readonly string[] ImageExtensions = [".png", ".jpg", ".jpeg"];

    private int _imageCount = 1;

    public override string Id => "draw_particles";
    public override string Name => "Particles";
    public override ImageEffectCategory Category => ImageEffectCategory.Drawings;
    public override string IconKey => LucideIcons.sparkles;
    public override string Description => "Draws particle images on the source image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FilePath<DrawParticlesEffect>("image_folder", "Image folder", string.Empty, (e, v) => e.ImageFolder = v),
        EffectParameters.IntNumeric<DrawParticlesEffect>("image_count", "Image count", 1, 1000, 1, (e, v) => e.ImageCount = v),
        EffectParameters.Bool<DrawParticlesEffect>("background", "Background", false, (e, v) => e.Background = v),
        EffectParameters.Bool<DrawParticlesEffect>("random_size", "Random size", false, (e, v) => e.RandomSize = v),
        EffectParameters.IntNumeric<DrawParticlesEffect>("random_size_min", "Random size min", 1, 10000, 64, (e, v) => e.RandomSizeMin = v),
        EffectParameters.IntNumeric<DrawParticlesEffect>("random_size_max", "Random size max", 1, 10000, 128, (e, v) => e.RandomSizeMax = v),
        EffectParameters.Bool<DrawParticlesEffect>("random_angle", "Random angle", false, (e, v) => e.RandomAngle = v),
        EffectParameters.IntNumeric<DrawParticlesEffect>("random_angle_min", "Random angle min", 0, 360, 0, (e, v) => e.RandomAngleMin = v),
        EffectParameters.IntNumeric<DrawParticlesEffect>("random_angle_max", "Random angle max", 0, 360, 360, (e, v) => e.RandomAngleMax = v),
        EffectParameters.Bool<DrawParticlesEffect>("random_opacity", "Random opacity", false, (e, v) => e.RandomOpacity = v),
        EffectParameters.IntNumeric<DrawParticlesEffect>("random_opacity_min", "Random opacity min", 0, 100, 0, (e, v) => e.RandomOpacityMin = v),
        EffectParameters.IntNumeric<DrawParticlesEffect>("random_opacity_max", "Random opacity max", 0, 100, 100, (e, v) => e.RandomOpacityMax = v),
        EffectParameters.Bool<DrawParticlesEffect>("no_overlap", "No overlap", false, (e, v) => e.NoOverlap = v),
        EffectParameters.IntNumeric<DrawParticlesEffect>("no_overlap_offset", "No overlap offset", 0, 1000, 0, (e, v) => e.NoOverlapOffset = v),
        EffectParameters.Bool<DrawParticlesEffect>("edge_overlap", "Edge overlap", false, (e, v) => e.EdgeOverlap = v)
    ];

    public string ImageFolder { get; set; } = string.Empty;

    public int ImageCount
    {
        get => _imageCount;
        set => _imageCount = Math.Clamp(value, 1, 1000);
    }

    public bool Background { get; set; }

    public bool RandomSize { get; set; }

    public int RandomSizeMin { get; set; } = 64;

    public int RandomSizeMax { get; set; } = 128;

    public bool RandomAngle { get; set; }

    public int RandomAngleMin { get; set; }

    public int RandomAngleMax { get; set; } = 360;

    public bool RandomOpacity { get; set; }

    public int RandomOpacityMin { get; set; }

    public int RandomOpacityMax { get; set; } = 100;

    public bool NoOverlap { get; set; }

    public int NoOverlapOffset { get; set; }

    public bool EdgeOverlap { get; set; }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        string folderPath = DrawingEffectHelpers.ExpandVariables(ImageFolder);
        if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
        {
            return source.Copy();
        }

        string[] files = Directory.EnumerateFiles(folderPath)
            .Where(x => ImageExtensions.Contains(Path.GetExtension(x), StringComparer.OrdinalIgnoreCase))
            .ToArray();

        if (files.Length == 0)
        {
            return source.Copy();
        }

        SKBitmap result = Background
            ? new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType)
            : source.Copy();

        using SKCanvas canvas = new SKCanvas(result);
        if (Background)
        {
            canvas.Clear(SKColors.Transparent);
        }

        List<SKRectI> imageRectangles = [];
        Dictionary<string, SKBitmap> imageCache = new(StringComparer.OrdinalIgnoreCase);

        try
        {
            for (int i = 0; i < ImageCount; i++)
            {
                string file = files[Random.Shared.Next(0, files.Length)];
                if (!imageCache.TryGetValue(file, out SKBitmap? particleBitmap))
                {
                    particleBitmap = SKBitmap.Decode(file);
                    if (particleBitmap != null)
                    {
                        imageCache[file] = particleBitmap;
                    }
                }

                if (particleBitmap is null || particleBitmap.Width <= 0 || particleBitmap.Height <= 0)
                {
                    continue;
                }

                DrawParticle(result, canvas, particleBitmap, imageRectangles);
            }

            if (Background)
            {
                canvas.DrawBitmap(source, 0, 0);
            }
        }
        finally
        {
            foreach (SKBitmap cachedBitmap in imageCache.Values)
            {
                cachedBitmap.Dispose();
            }
        }

        return result;
    }

    private void DrawParticle(SKBitmap target, SKCanvas canvas, SKBitmap particle, List<SKRectI> imageRectangles)
    {
        int width;
        int height;

        if (RandomSize)
        {
            int size = NextInt(Math.Min(RandomSizeMin, RandomSizeMax), Math.Max(RandomSizeMin, RandomSizeMax));
            width = size;
            height = size;

            if (particle.Width > particle.Height)
            {
                height = (int)Math.Round(size * ((double)particle.Height / particle.Width));
            }
            else if (particle.Width < particle.Height)
            {
                width = (int)Math.Round(size * ((double)particle.Width / particle.Height));
            }
        }
        else
        {
            width = particle.Width;
            height = particle.Height;
        }

        if (width < 1 || height < 1)
        {
            return;
        }

        int minOffsetX = EdgeOverlap ? -width + 1 : 0;
        int minOffsetY = EdgeOverlap ? -height + 1 : 0;
        int maxOffsetX = target.Width - (EdgeOverlap ? 0 : width) - 1;
        int maxOffsetY = target.Height - (EdgeOverlap ? 0 : height) - 1;

        SKRectI rect = default;
        int attemptCount = 0;

        do
        {
            attemptCount++;

            if (attemptCount > 1000)
            {
                return;
            }

            int x = NextInt(Math.Min(minOffsetX, maxOffsetX), Math.Max(minOffsetX, maxOffsetX));
            int y = NextInt(Math.Min(minOffsetY, maxOffsetY), Math.Max(minOffsetY, maxOffsetY));
            rect = new SKRectI(x, y, x + width, y + height);
        } while (NoOverlap && imageRectangles.Any(x => DrawingEffectHelpers.Intersects(x, DrawingEffectHelpers.Inflate(rect, NoOverlapOffset))));

        imageRectangles.Add(rect);

        int alpha = 255;
        if (RandomOpacity)
        {
            int opacity = NextInt(Math.Min(RandomOpacityMin, RandomOpacityMax), Math.Max(RandomOpacityMin, RandomOpacityMax));
            opacity = Math.Clamp(opacity, 0, 100);
            alpha = (int)Math.Round(opacity / 100f * 255);
        }

        canvas.Save();

        if (RandomAngle)
        {
            float moveX = rect.Left + (rect.Width / 2f);
            float moveY = rect.Top + (rect.Height / 2f);
            int rotate = NextInt(Math.Min(RandomAngleMin, RandomAngleMax), Math.Max(RandomAngleMin, RandomAngleMax));

            canvas.Translate(moveX, moveY);
            canvas.RotateDegrees(rotate);
            canvas.Translate(-moveX, -moveY);
        }

        using SKPaint paint = new SKPaint
        {
            IsAntialias = true
        };

        if (alpha < 255)
        {
            paint.ColorFilter = SKColorFilter.CreateBlendMode(new SKColor(255, 255, 255, (byte)alpha), SKBlendMode.Modulate);
        }

        using SKImage particleImage = SKImage.FromBitmap(particle);
        canvas.DrawImage(particleImage, new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom), new SKSamplingOptions(SKCubicResampler.CatmullRom), paint);
        canvas.Restore();
    }

    private static int NextInt(int min, int max)
    {
        if (min == max)
        {
            return min;
        }

        if (min > max)
        {
            (min, max) = (max, min);
        }

        return Random.Shared.Next(min, max);
    }
}
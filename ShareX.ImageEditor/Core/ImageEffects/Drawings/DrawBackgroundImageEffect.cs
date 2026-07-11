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

namespace ShareX.ImageEditor.Core.ImageEffects.Drawings;

public sealed class DrawBackgroundImageEffect : ImageEffectBase
{
    public override string Id => "draw_background_image";
    public override string Name => "Background image";
    public override ImageEffectCategory Category => ImageEffectCategory.Drawings;
    public override string IconKey => LucideIcons.image;
    public override string Description => "Draws a background image behind the source image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FilePath<DrawBackgroundImageEffect>("image_file_path", "Image file path", string.Empty, (e, v) => e.ImageFilePath = v),
        EffectParameters.Bool<DrawBackgroundImageEffect>("center", "Center", true, (e, v) => e.Center = v),
        EffectParameters.Bool<DrawBackgroundImageEffect>("tile", "Tile", false, (e, v) => e.Tile = v)
    ];

    public string ImageFilePath { get; set; } = string.Empty;

    public bool Center { get; set; } = true;

    public bool Tile { get; set; }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        string imagePath = DrawingEffectHelpers.ExpandVariables(ImageFilePath);
        if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
        {
            return source.Copy();
        }

        using SKBitmap? backgroundImage = SKBitmap.Decode(imagePath);
        if (backgroundImage is null || backgroundImage.Width <= 0 || backgroundImage.Height <= 0)
        {
            return source.Copy();
        }

        SKBitmap result = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new SKCanvas(result);
        canvas.Clear(SKColors.Transparent);

        if (Tile)
        {
            int tileX = 0;
            int tileY = 0;

            if (Center)
            {
                tileX = (result.Width - backgroundImage.Width) / 2 % backgroundImage.Width;
                tileY = (result.Height - backgroundImage.Height) / 2 % backgroundImage.Height;
            }

            using SKImage backgroundSKImage = SKImage.FromBitmap(backgroundImage);
            using SKShader shader = SKShader.CreateImage(
                backgroundSKImage,
                SKShaderTileMode.Repeat,
                SKShaderTileMode.Repeat,
                new SKSamplingOptions(SKCubicResampler.CatmullRom));
            using SKPaint paint = new SKPaint { Shader = shader, IsAntialias = true };

            if (Center)
            {
                canvas.Save();
                canvas.Translate(tileX, tileY);
                canvas.DrawRect(-tileX, -tileY, result.Width, result.Height, paint);
                canvas.Restore();
            }
            else
            {
                canvas.DrawRect(0, 0, result.Width, result.Height, paint);
            }
        }
        else
        {
            float aspectRatio = (float)backgroundImage.Width / backgroundImage.Height;
            int width = result.Width;
            int height = (int)(width / aspectRatio);

            if (height < result.Height)
            {
                height = result.Height;
                width = (int)(height * aspectRatio);
            }

            int x = Center ? (result.Width - width) / 2 : 0;
            int y = Center ? (result.Height - height) / 2 : 0;

            using SKPaint paint = new SKPaint { IsAntialias = true };
            using SKImage backgroundSKImage2 = SKImage.FromBitmap(backgroundImage);
            canvas.DrawImage(backgroundSKImage2, new SKRect(x, y, x + width, y + height), new SKSamplingOptions(SKCubicResampler.CatmullRom), paint);
        }

        canvas.DrawBitmap(source, 0, 0);
        return result;
    }
}
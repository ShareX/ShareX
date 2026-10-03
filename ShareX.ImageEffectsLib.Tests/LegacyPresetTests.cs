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

using ShareX.HelpersLib;
using SkiaSharp;
using System;
using System.Drawing;
using System.Linq;
using Xunit;

namespace ShareX.ImageEffectsLib.Tests
{
    /// <summary>
    /// Presets written by the GDI+ version (WinForms Padding, GDI+ enums and Font) must still load into the portable types.
    /// The JSON below uses the exact strings those types' converters wrote.
    /// </summary>
    public class LegacyPresetTests
    {
        private const string LegacyPreset = @"{
  ""Name"": ""Legacy"",
  ""Effects"": [
    { ""$type"": ""Canvas"", ""Margin"": ""1, 2, 3, 4"", ""MarginMode"": ""AbsoluteSize"", ""Color"": ""Red"", ""Enabled"": true },
    { ""$type"": ""TornEdge"", ""Depth"": 15, ""Range"": 20, ""Sides"": ""Top, Bottom"", ""CurvedEdges"": true, ""Enabled"": true },
    { ""$type"": ""DrawBorder"", ""Type"": ""Inside"", ""Size"": 3, ""DashStyle"": ""Dash"", ""Color"": ""Blue"", ""UseGradient"": true,
      ""Gradient"": { ""Type"": ""ForwardDiagonal"", ""Colors"": [ { ""Color"": ""Red"", ""Location"": 0.0 }, { ""Color"": ""Blue"", ""Location"": 100.0 } ] }, ""Enabled"": true },
    { ""$type"": ""DrawText"", ""Text"": ""Hello"", ""Placement"": ""BottomRight"", ""Offset"": ""5, 5"",
      ""TextFont"": ""Arial, 11.25pt, style=Bold"", ""TextRenderingMode"": ""AntiAlias"", ""Padding"": ""5, 6, 7, 8"", ""Enabled"": true },
    { ""$type"": ""DrawImage"", ""Placement"": ""MiddleCenter"", ""CompositingMode"": ""SourceCopy"", ""Enabled"": false }
  ]
}";

        private static ImageEffectPreset Load() => JsonHelpers.DeserializeFromString<ImageEffectPreset>(LegacyPreset, new ImageEffectsSerializationBinder());

        [Fact]
        public void LegacyPreset_LoadsEveryEffectWithItsSettings()
        {
            ImageEffectPreset preset = Load();

            Assert.Equal(5, preset.Effects.Count);

            Canvas canvas = Assert.IsType<Canvas>(preset.Effects[0]);
            Assert.Equal(new ImageMargins(1, 2, 3, 4), canvas.Margin);
            Assert.Equal(Color.Red.ToArgb(), canvas.Color.ToArgb());

            TornEdge torn = Assert.IsType<TornEdge>(preset.Effects[1]);
            Assert.Equal(ImageSides.Top | ImageSides.Bottom, torn.Sides);

            DrawBorder border = Assert.IsType<DrawBorder>(preset.Effects[2]);
            Assert.Equal(ImageDashStyle.Dash, border.DashStyle);
            Assert.Equal(ImageGradientMode.ForwardDiagonal, border.Gradient.Type);
            Assert.Equal(2, border.Gradient.Colors.Count);

            DrawText text = Assert.IsType<DrawText>(preset.Effects[3]);
            Assert.Equal(ImageContentAlignment.BottomRight, text.Placement);
            Assert.Equal("Arial", text.TextFont.Name);
            Assert.Equal(11.25f, text.TextFont.SizeInPoints);
            Assert.True(text.TextFont.Style.HasFlag(ImageFontStyle.Bold));
            Assert.Equal(ImageTextRenderingMode.AntiAlias, text.TextRenderingMode);
            Assert.Equal(new ImageMargins(5, 6, 7, 8), text.Padding);
            Assert.Equal(new Point(5, 5), text.Offset);

            DrawImage image = Assert.IsType<DrawImage>(preset.Effects[4]);
            Assert.Equal(ImageContentAlignment.MiddleCenter, image.Placement);
            Assert.Equal(ImageCompositingMode.SourceCopy, image.CompositingMode);
            Assert.False(image.Enabled);
        }

        [Fact]
        public void Preset_RoundTripsInTheSameFormat()
        {
            string json = JsonHelpers.SerializeToString(Load(), serializationBinder: new ImageEffectsSerializationBinder());

            Assert.Contains("\"Margin\": \"1, 2, 3, 4\"", json);
            Assert.Contains("\"Sides\": \"Top, Bottom\"", json);
            Assert.Contains("\"DashStyle\": \"Dash\"", json);
            Assert.Contains("\"TextFont\": \"Arial, 11.25pt, style=Bold\"", json);
            Assert.Contains("\"Placement\": \"BottomRight\"", json);

            ImageEffectPreset again = JsonHelpers.DeserializeFromString<ImageEffectPreset>(json, new ImageEffectsSerializationBinder());
            Assert.Equal(new ImageMargins(1, 2, 3, 4), ((Canvas)again.Effects[0]).Margin);
        }

        [Theory]
        [InlineData("Arial, 36pt", "Arial", 36f, false, false)]
        [InlineData("Segoe UI, 9.75pt, style=Bold, Italic", "Segoe UI", 9.75f, true, true)]
        [InlineData("Consolas, 16px", "Consolas", 12f, false, false)]
        public void ImageFont_ReadsGdiFontConverterStrings(string text, string family, float points, bool bold, bool italic)
        {
            ImageFont font = (ImageFont)new ImageFontConverter().ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, text);

            Assert.Equal(family, font.Name);
            Assert.Equal(points, font.SizeInPoints, 3);
            Assert.Equal(bold, font.Style.HasFlag(ImageFontStyle.Bold));
            Assert.Equal(italic, font.Style.HasFlag(ImageFontStyle.Italic));
        }
    }

    public class EffectTests
    {
        private static SKBitmap Sample(int width = 64, int height = 48)
        {
            SKBitmap bitmap = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
            using SKCanvas canvas = new SKCanvas(bitmap);
            canvas.Clear(new SKColor(100, 150, 200));
            using SKPaint paint = new SKPaint { Color = SKColors.Red };
            canvas.DrawRect(10, 10, 20, 20, paint);
            return bitmap;
        }

        public static TheoryData<Type> AllEffectTypes()
        {
            TheoryData<Type> data = new TheoryData<Type>();

            foreach (Type type in typeof(ImageEffect).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ImageEffect)) && !t.IsAbstract))
            {
                data.Add(type);
            }

            return data;
        }

        [Theory]
        [MemberData(nameof(AllEffectTypes))]
        public void EveryEffect_RunsWithItsDefaultsOnEveryOs(Type type)
        {
            ImageEffect effect = (ImageEffect)Activator.CreateInstance(type, nonPublic: true);
            using SKBitmap result = effect.Apply(Sample());

            Assert.NotNull(result);
            Assert.True(result.Width > 0 && result.Height > 0);
        }

        [Fact]
        public void Brightness_AddsToEveryChannel()
        {
            using SKBitmap result = new Brightness { Value = 0.2f }.Apply(Sample());
            SKColor pixel = result.GetPixel(0, 0);

            Assert.InRange(pixel.Red, 100 + 50, 100 + 52);
            Assert.InRange(pixel.Blue, 200 + 50, 200 + 52);
        }

        [Fact]
        public void Inverse_InvertsColoursAndKeepsAlpha()
        {
            using SKBitmap result = new Inverse().Apply(Sample());

            Assert.Equal(new SKColor(155, 105, 55, 255), result.GetPixel(0, 0));
        }

        [Fact]
        public void Canvas_AddsTheMargin()
        {
            using SKBitmap result = new Canvas { Margin = new ImageMargins(1, 2, 3, 4), Color = Color.Red }.Apply(Sample());

            Assert.Equal(64 + 4, result.Width);
            Assert.Equal(48 + 6, result.Height);
            Assert.Equal(SKColors.Red, result.GetPixel(0, 0));
            Assert.Equal(new SKColor(100, 150, 200), result.GetPixel(1, 2));
        }

        [Fact]
        public void Crop_RemovesTheMargin()
        {
            using SKBitmap result = new Crop { Margin = new ImageMargins(10, 10, 0, 0) }.Apply(Sample());

            Assert.Equal(54, result.Width);
            Assert.Equal(38, result.Height);
            Assert.Equal(SKColors.Red, result.GetPixel(0, 0));
        }

        [Fact]
        public void Rotate_90_WithUpsize_SwapsTheSize()
        {
            using SKBitmap result = new Rotate { Angle = 90, Upsize = true }.Apply(Sample());

            Assert.Equal(48, result.Width);
            Assert.Equal(64, result.Height);
        }

        [Fact]
        public void Resize_KeepsTheAspectRatioWhenOneSideIsZero()
        {
            using SKBitmap result = new Resize(32, 0).Apply(Sample());

            Assert.Equal(32, result.Width);
            Assert.Equal(24, result.Height);
        }

        [Fact]
        public void AutoCrop_TrimsTheBackgroundColour()
        {
            using SKBitmap result = new AutoCrop().Apply(Sample());

            Assert.Equal(20, result.Width);
            Assert.Equal(20, result.Height);
        }

        [Fact]
        public void ColorDepth_OneBit_LeavesOnlyFullOrEmptyChannels()
        {
            using SKBitmap result = new ColorDepth { BitsPerChannel = 1 }.Apply(Sample());
            SKColor pixel = result.GetPixel(0, 0);

            Assert.Equal(new SKColor(0, 255, 255), pixel);
        }

        [Fact]
        public void Preset_LeavesItsInputUntouched()
        {
            using SKBitmap source = Sample();
            ImageEffectPreset preset = new ImageEffectPreset();
            preset.Effects.Add(new Inverse());

            using SKBitmap result = preset.ApplyEffects(source);

            Assert.Equal(new SKColor(100, 150, 200), source.GetPixel(0, 0));
            Assert.NotEqual(source.GetPixel(0, 0), result.GetPixel(0, 0));
        }
    }
}

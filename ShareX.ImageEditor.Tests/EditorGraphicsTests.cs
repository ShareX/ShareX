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

// Portable graphics checks and a contact sheet made only from bundled cursors and generated text.
using ShareX.AvaloniaUI.Imaging;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Presentation.Emoji;
using ShareX.ImageEditor.Presentation.Rendering;
using SkiaSharp;
using SkiaSharp.HarfBuzz;
using Xunit;

namespace ShareX.ImageEditor.Tests;

[Collection("Editor graphics")]
public sealed class EditorGraphicsTests
{
    private static readonly string[] Sequences =
        ["1f600", "2764-fe0f", "1f44d-1f3fd", "1f469-200d-1f4bb", "1f468-200d-1f469-200d-1f467", "0031-fe0f-20e3"];
    private static readonly CursorType[] BundledCursors =
        [CursorType.HSplit, CursorType.VSplit, CursorType.NoMove2D, CursorType.NoMoveHoriz, CursorType.NoMoveVert,
         CursorType.PanEast, CursorType.PanNE, CursorType.PanNorth, CursorType.PanNW, CursorType.PanSE,
         CursorType.PanSouth, CursorType.PanSW, CursorType.PanWest];

    [Fact] public void SavedFontNamesResolveToInstalledPortableFamilies() => VerifyFonts();
    [SystemColorEmojiFact] public void ColorEmojiPreviewsShapingAndCacheOwnership() => RunWithGraphicsApartment(VerifyEmoji);
    [Fact] public void AllBundledCursorImagesDecodeWithTransparency() => VerifyCursors();
    [Fact]
    public void UnsupportedGlyphsNeverUseLastResortOrUnassignedCodePoints()
    {
        foreach (string name in new[] { ".LastResort", "LastResort", "Last Resort", ".Last Resort", "lastresort" })
            Require(SkiaEmojiBitmapRenderer.IsLastResort(name), $"{name} should be recognised as Apple's placeholder font");
        foreach (string? name in new[] { null, "", "Apple Color Emoji", "Noto Color Emoji", "LastResortSans" })
            Require(!SkiaEmojiBitmapRenderer.IsLastResort(name), $"{name} is not the placeholder font");
        Require(RenderSkia("10ffff", 160) == null, "A noncharacter should not render with any installed font");
    }
    [EmojiFixtureFact]
    public void SuppliedColorEmojiFontRendersLargeAndSmallGlyphs()
    {
        string directory = Path.Combine(Path.GetTempPath(), "ShareX-emoji-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try { VerifyEmojiFont(Environment.GetEnvironmentVariable("SHAREX_TEST_EMOJI_FONT")!, directory); }
        finally { Directory.Delete(directory, true); }
    }
    [GraphicsArtifactFact]
    public void RenderGraphicsContactSheet()
    {
        string directory = Environment.GetEnvironmentVariable("SHAREX_TEST_GRAPHICS_OUTPUT")!;
        Directory.CreateDirectory(directory);
        RunWithGraphicsApartment(() => RenderContactSheet(Path.Combine(directory, "editor-graphics.png")));
        string? font = Environment.GetEnvironmentVariable("SHAREX_TEST_EMOJI_FONT");
        if (!string.IsNullOrEmpty(font)) VerifyEmojiFont(font, directory);
    }

    private static void RunWithGraphicsApartment(Action action)
    {
        if (!OperatingSystem.IsWindows()) { action(); return; }
        Exception? failure = null;
        Thread thread = new(() => { try { action(); } catch (Exception exception) { failure = exception; } }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(30)), "Graphics verification did not complete.");
        if (failure != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
    }
    private static void VerifyFonts()
    {
        string Resolve(string? family, params string[] installed) => FontFamilyResolver.Resolve(family,
            new HashSet<string>(installed, StringComparer.OrdinalIgnoreCase), "System default");

        Equal("Segoe UI", Resolve("segoe ui", "Segoe UI", "Noto Sans"));
        Equal("Noto Sans", Resolve("Segoe UI", "DejaVu Sans", "Noto Sans"));
        Equal("Liberation Sans", Resolve("Arial", "Noto Sans", "Liberation Sans"));
        Equal("Helvetica", Resolve("Arial", "Helvetica", "Helvetica Neue"));
        Equal("Helvetica Neue", Resolve(null, "Helvetica Neue"));
        Equal("DejaVu Sans Mono", Resolve("Consolas", "Noto Sans", "DejaVu Sans Mono"));
        Equal("Custom Font", Resolve("Custom Font", "Custom Font", "Noto Sans"));
        Equal("System default", Resolve("Missing font"));

        foreach (string family in new[] { "Segoe UI", "Arial" })
        {
            string resolved = FontFamilyResolver.Resolve(family);
            using SKTypeface typeface = SKTypeface.FromFamilyName(resolved);
            using var font = new SKFont(typeface, 24);
            Require(font.MeasureText("ShareX: mixed Case 0123") > 0, $"Missing text for {family} -> {resolved}");
            Console.WriteLine($"Font: {family} -> {resolved}");
        }
        Console.WriteLine("PASS: Saved font names, installed families and portable fallbacks");
    }

    private static SKBitmap? RenderSkia(string sequence, int size) =>
        SkiaEmojiBitmapRenderer.Render(EmojiCatalogService.ToGlyph(sequence), size);

    private static void VerifyEmoji()
    {
        Require(RenderSkia("", 160) == null, "Empty emoji should not render");
        Require(RenderSkia("1f600", 0) == null, "Zero-size emoji should not render");
        Require(RenderSkia("10ffff", 160) == null, "Unsupported glyph should not become a tofu sticker");
        foreach (string sequence in Sequences)
        {
            foreach (int size in new[] { 28, 160 })
            {
                using SKBitmap? bitmap = RenderSkia(sequence, size);
                Require(bitmap != null, $"No Skia emoji for {sequence}, size {size}");
                VerifyPixels(bitmap!, $"Skia emoji {sequence}", expectColor: HasColoredArtwork(sequence));
                Equal(size, bitmap!.Width);
                Equal(size, bitmap.Height);
            }

            using SKBitmap? sticker = EmojiBitmapRenderer.RenderStickerBitmap(sequence);
            Require(sticker != null, $"No primary sticker for {sequence}");
            VerifyPixels(sticker!, $"Primary sticker {sequence}", expectColor: HasColoredArtwork(sequence));
            sticker!.Erase(SKColors.Transparent);
            using SKBitmap? second = EmojiBitmapRenderer.RenderStickerBitmap(sequence);
            VerifyPixels(second!, "Cached sticker copy", expectColor: HasColoredArtwork(sequence));
        }

        string? emojiFamily = new[] { "Noto Color Emoji", "Apple Color Emoji", "Segoe UI Emoji" }
            .FirstOrDefault(family => SKFontManager.Default.FontFamilies.Contains(family));
        Require(emojiFamily != null, "Install a system color emoji font before the graphics verification");
        using SKTypeface emojiTypeface = SKTypeface.FromFamilyName(emojiFamily);
        using var emojiFont = new SKFont(emojiTypeface, 64);
        using var shaper = new SKShaper(emojiTypeface);
        SKShaper.Result shaped = shaper.Shape(EmojiCatalogService.ToGlyph("1f469-200d-1f4bb"), emojiFont);
        Equal(1, shaped.Codepoints.Length);
        Console.WriteLine($"PASS: Color emoji, previews, ZWJ shaping and cache ownership ({emojiFamily})");
    }

    private static void VerifyEmojiFont(string fontPath, string outputDirectory)
    {
        using SKTypeface typeface = SKTypeface.FromFile(fontPath);
        using var sheet = new SKBitmap(1260, 235);
        using var canvas = new SKCanvas(sheet);
        using var labelFont = new SKFont(SKTypeface.Default, 16);
        using var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
        canvas.Clear(new SKColor(235, 235, 235));
        canvas.DrawText($"Skia + {typeface.FamilyName} from verification fixture (160 px and 28 px)", 12, 24, labelFont, paint);
        for (int i = 0; i < Sequences.Length; i++)
        {
            string glyph = EmojiCatalogService.ToGlyph(Sequences[i]);
            foreach (int size in new[] { 28, 160 })
            {
                using SKBitmap? bitmap = SkiaEmojiBitmapRenderer.Render(glyph, size, typeface);
                Require(bitmap != null, $"No {typeface.FamilyName} emoji for {Sequences[i]}");
                using SKImage sampleImage = SKImage.FromBitmap(bitmap!);
                using SKData sampleData = sampleImage.Encode(SKEncodedImageFormat.Png, 100);
                using (FileStream sampleOutput = File.Create(Path.Combine(outputDirectory, $"font-{Sequences[i]}-{size}.png")))
                    sampleData.SaveTo(sampleOutput);
                VerifyPixels(bitmap!, $"{typeface.FamilyName} {Sequences[i]} ({size}px)", expectColor: HasColoredArtwork(Sequences[i]));
                canvas.DrawBitmap(bitmap!, i * 210 + (size == 28 ? 174 : 0), size == 28 ? 173 : 35);
            }
            canvas.DrawText(Sequences[i], i * 210 + 2, 208, labelFont, paint);
        }
        using SKImage image = SKImage.FromBitmap(sheet);
        using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
        using FileStream output = File.Create(Path.Combine(outputDirectory, "emoji-font-fixture.png"));
        data.SaveTo(output);
        Console.WriteLine($"PASS: Additional color font fixture ({typeface.FamilyName})");
    }

    private static SKBitmap? RenderCursor(CursorType cursor) => CursorBitmapRenderer.CreateAnnotationBitmap(cursor);

    private static void VerifyCursors()
    {
        foreach (CursorType cursor in BundledCursors)
        {
            using SKBitmap? bitmap = RenderCursor(cursor);
            Require(bitmap != null, $"No bundled cursor for {cursor}");
            VerifyPixels(bitmap!, $"Cursor {cursor}");
        }
        Console.WriteLine("PASS: All 13 bundled cursor images decode with visible pixels and transparency");
    }

    private static void VerifyPixels(SKBitmap bitmap, string name, bool expectColor = false)
    {
        int visible = 0, transparent = 0, colored = 0;
        foreach (SKColor pixel in bitmap.Pixels)
        {
            if (pixel.Alpha == 0) transparent++;
            else
            {
                visible++;
                if (pixel.Red != pixel.Green || pixel.Green != pixel.Blue) colored++;
            }
        }
        Require(visible > 0, $"Blank {name}");
        Require(transparent > 0, $"Missing transparency in {name}");
        if (expectColor) Require(colored > 0, $"Monochrome {name}");
    }

    // Recent Noto fonts intentionally draw the family pictogram with grayscale silhouettes.
    private static bool HasColoredArtwork(string sequence) => sequence != "1f468-200d-1f469-200d-1f467";

    private static void RenderContactSheet(string path)
    {
        using var sheet = new SKBitmap(1260, 650);
        using var canvas = new SKCanvas(sheet);
        using var font = new SKFont(SKTypeface.Default, 16);
        using var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
        canvas.Clear(new SKColor(235, 235, 235));
        canvas.DrawText("Skia + system color emoji font (160 px, with 28 px preview)", 12, 24, font, paint);
        canvas.DrawText("Primary renderer (Direct2D on Windows, Skia elsewhere)", 12, 239, font, paint);
        for (int i = 0; i < Sequences.Length; i++)
        {
            int x = i * 210;
            using SKBitmap? skia = RenderSkia(Sequences[i], 160);
            using SKBitmap? preview = RenderSkia(Sequences[i], 28);
            using SKBitmap? primary = EmojiBitmapRenderer.RenderStickerBitmap(Sequences[i]);
            canvas.DrawBitmap(skia!, x, 35);
            canvas.DrawBitmap(preview!, x + 174, 173);
            canvas.DrawText(Sequences[i], x + 2, 208, font, paint);
            canvas.DrawBitmap(primary!, x, 250);
        }
        canvas.DrawText("Bundled cursor annotations (original size, then 3x for inspection)", 12, 448, font, paint);
        for (int i = 0; i < BundledCursors.Length; i++)
        {
            int x = i * 97;
            using SKBitmap? cursor = RenderCursor(BundledCursors[i]);
            canvas.DrawBitmap(cursor!, x + 12, 464);
            canvas.DrawBitmap(cursor!, new SKRect(x, 514, x + 72, 586));
            using var labelFont = new SKFont(SKTypeface.Default, 12);
            canvas.DrawText(BundledCursors[i].ToString(), x + 2, 611, labelFont, paint);
        }
        using SKImage image = SKImage.FromBitmap(sheet);
        using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
        using FileStream output = File.Create(path);
        data.SaveTo(output);
    }

    private static void Equal<T>(T expected, T actual) => Assert.Equal(expected, actual);
    private static void Require(bool condition, string message)
    {
        Assert.True(condition, message);
    }
}

public sealed class SystemColorEmojiFactAttribute : FactAttribute
{
    public SystemColorEmojiFactAttribute()
    {
        if (!new[] { "Noto Color Emoji", "Apple Color Emoji", "Segoe UI Emoji" }.Any(SKFontManager.Default.FontFamilies.Contains))
            Skip = "Requires an installed system color emoji font.";
    }
}
public sealed class EmojiFixtureFactAttribute : FactAttribute
{
    public EmojiFixtureFactAttribute()
    {
        string? path = Environment.GetEnvironmentVariable("SHAREX_TEST_EMOJI_FONT");
        if (string.IsNullOrEmpty(path)) Skip = "Set SHAREX_TEST_EMOJI_FONT to verify an additional color font fixture.";
    }
}
public sealed class GraphicsArtifactFactAttribute : FactAttribute
{
    public GraphicsArtifactFactAttribute()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SHAREX_TEST_GRAPHICS_OUTPUT")))
            Skip = "Set SHAREX_TEST_GRAPHICS_OUTPUT to generate contact sheets for visual inspection.";
    }
}
[CollectionDefinition("Editor graphics", DisableParallelization = true)]
public sealed class EditorGraphicsCollection { }

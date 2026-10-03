// Portable graphics checks and a contact sheet made only from bundled cursors and generated text.
using ShareX.AvaloniaUI.Imaging;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Presentation.Emoji;
using SkiaSharp;
using SkiaSharp.HarfBuzz;
using System.Reflection;

internal static class Program
{
    private static readonly string[] Sequences =
        ["1f600", "2764-fe0f", "1f44d-1f3fd", "1f469-200d-1f4bb", "1f468-200d-1f469-200d-1f467", "0031-fe0f-20e3"];
    private static readonly CursorType[] BundledCursors =
        [CursorType.HSplit, CursorType.VSplit, CursorType.NoMove2D, CursorType.NoMoveHoriz, CursorType.NoMoveVert,
         CursorType.PanEast, CursorType.PanNE, CursorType.PanNorth, CursorType.PanNW, CursorType.PanSE,
         CursorType.PanSouth, CursorType.PanSW, CursorType.PanWest];

    private static int Main(string[] args)
    {
        try
        {
            VerifyFonts();
            VerifyEmoji();
            VerifyCursors();
            if (args.Length > 1) VerifyEmojiFont(args[1], args[0]);
            string sheet = Path.Combine(args[0], "editor-graphics.png");
            RenderContactSheet(sheet);
            Console.WriteLine($"PASS: Editor graphics; contact sheet: {sheet}");
            return 0;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"FAIL: {exception}");
            return 1;
        }
    }

    private static void VerifyFonts()
    {
        MethodInfo resolve = typeof(FontFamilyResolver).GetMethod("Resolve", BindingFlags.Static | BindingFlags.NonPublic)!;
        string Resolve(string? family, params string[] installed) => (string)resolve.Invoke(null,
            [family, new HashSet<string>(installed, StringComparer.OrdinalIgnoreCase), "System default"])!;

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

    private static SKBitmap? RenderSkia(string sequence, int size) => (SKBitmap?)typeof(WindowsEmojiBitmapRenderer).Assembly
        .GetType("ShareX.ImageEditor.Presentation.Emoji.SkiaEmojiBitmapRenderer")!
        .GetMethod("Render", BindingFlags.Static | BindingFlags.Public, [typeof(string), typeof(int)])!
        .Invoke(null, [EmojiCatalogService.ToGlyph(sequence), size]);

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

            using SKBitmap? sticker = WindowsEmojiBitmapRenderer.RenderStickerBitmap(sequence);
            Require(sticker != null, $"No primary sticker for {sequence}");
            VerifyPixels(sticker!, $"Primary sticker {sequence}", expectColor: HasColoredArtwork(sequence));
            sticker!.Erase(SKColors.Transparent);
            using SKBitmap? second = WindowsEmojiBitmapRenderer.RenderStickerBitmap(sequence);
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
        MethodInfo render = typeof(WindowsEmojiBitmapRenderer).Assembly
            .GetType("ShareX.ImageEditor.Presentation.Emoji.SkiaEmojiBitmapRenderer")!
            .GetMethod("Render", BindingFlags.Static | BindingFlags.NonPublic)!;
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
                using SKBitmap? bitmap = (SKBitmap?)render.Invoke(null, [glyph, size, typeface]);
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

    private static SKBitmap? RenderCursor(CursorType cursor) => (SKBitmap?)typeof(WindowsEmojiBitmapRenderer).Assembly
        .GetType("ShareX.ImageEditor.Presentation.Rendering.WindowsCursorBitmapRenderer")!
        .GetMethod("CreateAnnotationBitmap")!.Invoke(null, [cursor]);

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
            using SKBitmap? primary = WindowsEmojiBitmapRenderer.RenderStickerBitmap(Sequences[i]);
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

    private static void Equal<T>(T expected, T actual) => Require(EqualityComparer<T>.Default.Equals(expected, actual),
        $"Expected '{expected}', received '{actual}'");
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}

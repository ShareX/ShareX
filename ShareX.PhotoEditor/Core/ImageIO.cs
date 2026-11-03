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

namespace ShareX.PhotoEditor.Core;

public sealed record FileFormatInfo(string Name, string[] Extensions, bool CanSave);

/// <summary>Opens and saves flat image formats and the layered project format.</summary>
public static class ImageIO
{
    public const string ProjectExtension = ".sxpe";

    public static IReadOnlyList<FileFormatInfo> Formats { get; } =
    [
        new("ShareX Photo Editor Project", [ProjectExtension], true),
        new("PNG", [".png"], true),
        new("JPEG", [".jpg", ".jpeg", ".jpe", ".jfif"], true),
        new("WebP", [".webp"], true),
        new("BMP", [".bmp"], true),
        new("GIF", [".gif"], true),
        new("TIFF", [".tif", ".tiff"], true),
        new("ICO", [".ico"], false),
        new("HEIF", [".heic", ".heif", ".avif"], false)
    ];

    public static bool IsProject(string path) => string.Equals(Path.GetExtension(path), ProjectExtension, StringComparison.OrdinalIgnoreCase);

    public static bool IsSupported(string path)
    {
        string extension = Path.GetExtension(path);
        return Formats.Any(f => f.Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>Opens a document from a file. Throws <see cref="InvalidDataException"/> with a friendly message when it fails.</summary>
    public static PhotoDocument Open(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("The file does not exist.", path);
        if (IsProject(path))
        {
            PhotoDocument project;
            try
            {
                project = ProjectSerializer.Load(path);
            }
            catch (InvalidDataException ex) when (ex.Message.Contains("newer version", StringComparison.Ordinal))
            {
                throw;
            }
            catch (Exception ex) when (ex is InvalidDataException or System.Text.Json.JsonException or KeyNotFoundException or NotSupportedException or FormatException)
            {
                throw new InvalidDataException($"Could not open \"{Path.GetFileName(path)}\" because it is not a valid ShareX Photo Editor project or the file is damaged.");
            }
            project.FilePath = path;
            return project;
        }
        SKBitmap bitmap = DecodeImage(path) ?? throw new InvalidDataException($"Could not open \"{Path.GetFileName(path)}\" because it is not a supported image or the file is damaged.");
        using (bitmap)
        {
            PhotoDocument doc = FromBitmap(bitmap, Path.GetFileNameWithoutExtension(path));
            doc.FilePath = path;
            doc.Title = Path.GetFileName(path);
            return doc;
        }
    }

    public static SKBitmap? DecodeImage(string path)
    {
        string extension = Path.GetExtension(path).ToLowerInvariant();
        if (extension is ".tif" or ".tiff")
        {
            SKBitmap? tiff = DecodeWithSystemDrawing(path);
            if (tiff != null) return tiff;
        }
        try
        {
            using FileStream stream = File.OpenRead(path);
            return SkiaImageHelpers.Decode(stream, true);
        }
        catch (Exception)
        {
            return DecodeWithSystemDrawing(path);
        }
    }

    /// <summary>Fallback decoder (GDI+) that also handles TIFF.</summary>
    private static SKBitmap? DecodeWithSystemDrawing(string path)
    {
        if (!OperatingSystem.IsWindows()) return null;
        try
        {
            using System.Drawing.Image image = System.Drawing.Image.FromFile(path);
            using MemoryStream png = new();
            image.Save(png, System.Drawing.Imaging.ImageFormat.Png);
            png.Position = 0;
            return SKBitmap.Decode(png);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>Creates a single-layer document from a bitmap. Opaque images get a locked Background layer.</summary>
    public static PhotoDocument FromBitmap(SKBitmap bitmap, string? title = null)
    {
        PhotoDocument doc = new(bitmap.Width, bitmap.Height, 72, createBackground: false);
        PixelSurface surface = PixelSurface.FromBitmap(bitmap);
        bool opaque = IsOpaque(surface.Bitmap);
        RasterLayer layer = new(opaque ? "Background" : "Layer 0", surface)
        {
            IsBackground = opaque,
            LockPosition = opaque,
            LockTransparency = opaque
        };
        doc.Root.Add(layer);
        doc.ActiveLayer = layer;
        if (title != null) doc.Title = title;
        return doc;
    }

    private static unsafe bool IsOpaque(SKBitmap bitmap)
    {
        uint* p = (uint*)bitmap.GetPixels();
        int stride = bitmap.RowBytes / 4;
        for (int y = 0; y < bitmap.Height; y++)
            for (int x = 0; x < bitmap.Width; x++)
                if ((p[y * stride + x] >> 24) != 255) return false;
        return true;
    }

    /// <summary>Saves the document. Flat formats receive the flattened image; the project format keeps layers.</summary>
    public static void Save(PhotoDocument doc, string path, int quality = 90)
    {
        if (IsProject(path))
        {
            ProjectSerializer.Save(doc, path);
            return;
        }
        using SKBitmap flat = Compositor.Flatten(doc);
        SaveBitmap(flat, path, quality);
    }

    public static void SaveBitmap(SKBitmap bitmap, string path, int quality = 90)
    {
        string extension = Path.GetExtension(path).ToLowerInvariant();
        string temp = path + ".tmp";
        try
        {
            using (FileStream stream = new(temp, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                switch (extension)
                {
                    case ".jpg" or ".jpeg" or ".jpe" or ".jfif":
                        // JPEG has no alpha: composite onto white first.
                        using (SKBitmap opaque = FlattenOnto(bitmap, SKColors.White)) opaque.Save(stream, SKEncodedImageFormat.Jpeg, quality);
                        break;
                    case ".webp":
                        bitmap.Save(stream, SKEncodedImageFormat.Webp, quality);
                        break;
                    case ".bmp":
                        SkiaImageHelpers.SaveBMP(bitmap, stream, BMPBitDepth.Automatic);
                        break;
                    case ".gif":
                        bitmap.Save(stream, SKEncodedImageFormat.Gif);
                        break;
                    case ".tif" or ".tiff":
                        SaveTiff(bitmap, stream);
                        break;
                    default:
                        bitmap.Save(stream, SKEncodedImageFormat.Png);
                        break;
                }
            }
            File.Move(temp, path, true);
        }
        finally
        {
            if (File.Exists(temp)) File.Delete(temp);
        }
    }

    private static void SaveTiff(SKBitmap bitmap, Stream stream)
    {
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("TIFF export requires Windows.");
        using MemoryStream png = new();
        bitmap.Save(png, SKEncodedImageFormat.Png);
        png.Position = 0;
        using System.Drawing.Image image = System.Drawing.Image.FromStream(png);
        image.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
    }

    public static SKBitmap FlattenOnto(SKBitmap bitmap, SKColor background)
    {
        SKBitmap result = ImageOps.CreateBitmap(bitmap.Width, bitmap.Height);
        using SKCanvas canvas = new(result);
        canvas.Clear(background);
        canvas.DrawBitmap(bitmap, 0, 0);
        return result;
    }

    public static string BuildOpenFilter() =>
        "All supported images|" + string.Join(";", Formats.SelectMany(f => f.Extensions).Select(e => "*" + e));
}

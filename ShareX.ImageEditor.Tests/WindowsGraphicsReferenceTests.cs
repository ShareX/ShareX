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

using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Presentation.Emoji;
using ShareX.ImageEditor.Presentation.Rendering;
using SkiaSharp;
using ShareX.Platform;
using ShareX.Platform.Windows;
using Xunit;

namespace ShareX.ImageEditor.Tests;

[Collection("Windows graphics reference")]
public sealed class WindowsGraphicsReferenceTests
{
    [GraphicsReferenceFact]
    public void EditorGraphicsMatchTheLocalWindowsReference()
    {
        if (!OperatingSystem.IsWindows()) return;
        PlatformServices.Initialize(new WindowsPlatformServices());
        try
        {
            string directory = Environment.GetEnvironmentVariable("SHAREX_TEST_WINDOWS_GRAPHICS_REFERENCE")!;
            bool record = Environment.GetEnvironmentVariable("SHAREX_RECORD_WINDOWS_GRAPHICS_REFERENCE") == "1";
            Directory.CreateDirectory(directory);

            foreach (CursorType cursor in Enum.GetValues<CursorType>())
            {
                using SKBitmap? actual = CursorBitmapRenderer.CreateAnnotationBitmap(cursor);
                Assert.NotNull(actual);
                CompareOrRecord(Path.Combine(directory, $"cursor-{cursor}.png"), actual!, record);
            }

            string[] sequences = ["1F600", "1F44D-1F3FD", "2764-FE0F", "1F469-200D-1F4BB", "1F468-200D-1F469-200D-1F467", "1F1F9-1F1F7"];
            foreach (string sequence in sequences)
            {
                foreach (int size in new[] { 28, 160 })
                {
                    using SKBitmap? actual = EmojiBitmapRenderer.RenderStickerBitmap(sequence, size);
                    Assert.NotNull(actual);
                    CompareOrRecord(Path.Combine(directory, $"emoji-{sequence}-{size}.png"), actual!, record);
                }
            }
        }
        finally
        {
            PlatformServices.Shutdown();
        }
    }

    private static void CompareOrRecord(string path, SKBitmap actual, bool record)
    {
        if (record)
        {
            using SKData png = actual.Encode(SKEncodedImageFormat.Png, 100);
            using FileStream file = File.Create(path);
            png.SaveTo(file);
            return;
        }

        Assert.True(File.Exists(path), $"Missing Windows reference: {path}");
        using SKBitmap expected = SKBitmap.Decode(path);
        Assert.Equal(expected.Width, actual.Width);
        Assert.Equal(expected.Height, actual.Height);
        Assert.Equal(expected.Pixels, actual.Pixels);
    }
}

public sealed class GraphicsReferenceFactAttribute : FactAttribute
{
    public GraphicsReferenceFactAttribute()
    {
        if (!OperatingSystem.IsWindows()) Skip = "Requires native Windows graphics.";
        else if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SHAREX_TEST_WINDOWS_GRAPHICS_REFERENCE")))
            Skip = "Set SHAREX_TEST_WINDOWS_GRAPHICS_REFERENCE to compare local Windows reference images.";
    }
}

[CollectionDefinition("Windows graphics reference", DisableParallelization = true)]
public sealed class WindowsGraphicsReferenceCollection { }

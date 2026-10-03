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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using ShareX.ImageEditor.Presentation.Emoji;

namespace ShareX.ImageEditor.Presentation.Controls;

public sealed class EmojiPreviewImage : Image
{
    public static readonly StyledProperty<string> UnicodeSequenceProperty =
        AvaloniaProperty.Register<EmojiPreviewImage, string>(nameof(UnicodeSequence), string.Empty);

    public static readonly StyledProperty<int> PreviewSizeProperty =
        AvaloniaProperty.Register<EmojiPreviewImage, int>(nameof(PreviewSize), 48);

    private static readonly SemaphoreSlim PreviewRenderThrottle = new(4, 4);
    private int _updateVersion;

    static EmojiPreviewImage()
    {
        UnicodeSequenceProperty.Changed.AddClassHandler<EmojiPreviewImage>((image, _) => image.QueuePreviewUpdate());
        PreviewSizeProperty.Changed.AddClassHandler<EmojiPreviewImage>((image, _) => image.QueuePreviewUpdate());
    }

    public string UnicodeSequence
    {
        get => GetValue(UnicodeSequenceProperty);
        set => SetValue(UnicodeSequenceProperty, value);
    }

    public int PreviewSize
    {
        get => GetValue(PreviewSizeProperty);
        set => SetValue(PreviewSizeProperty, value);
    }

    private void QueuePreviewUpdate()
    {
        _ = UpdatePreviewAsync();
    }

    private async Task UpdatePreviewAsync()
    {
        string unicodeSequence = UnicodeSequence;
        int previewSize = PreviewSize;
        int version = Interlocked.Increment(ref _updateVersion);

        if (string.IsNullOrWhiteSpace(unicodeSequence) || previewSize <= 0)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (version == _updateVersion)
                {
                    Source = null;
                }
            });
            return;
        }

        await PreviewRenderThrottle.WaitAsync();
        try
        {
            var bitmap = await Task.Run(() => EmojiBitmapRenderer.RenderPreviewBitmap(unicodeSequence, previewSize));

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (version == _updateVersion)
                {
                    Source = bitmap;
                }
            });
        }
        finally
        {
            PreviewRenderThrottle.Release();
        }
    }
}
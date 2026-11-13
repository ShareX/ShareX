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

#nullable enable

using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using System;
using System.Threading.Tasks;
using Bitmap = SkiaSharp.SKBitmap;

namespace ShareX.ScreenCaptureLib;

public static class ManualScrollingCaptureIntegration
{
    private static ManualScrollingCaptureWindow? _window;
    private static bool _selecting;

    /// <summary>Starts a manual scrolling capture, or finishes the one in progress.</summary>
    public static Task StartFinishAsync(Action<Bitmap>? captured = null, Action? playNotificationSound = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        return Dispatcher.UIThread.InvokeAsync(() => StartFinishOnUIThreadAsync(captured, playNotificationSound));
    }

    private static async Task StartFinishOnUIThreadAsync(Action<Bitmap>? captured, Action? playNotificationSound)
    {
        if (_window != null)
        {
            await _window.FinishAsync();
            return;
        }

        if (_selecting) return;

        (System.Drawing.Rectangle Rectangle, HelpersLib.WindowInfo? WindowInfo)? selection;
        _selecting = true;
        try
        {
            selection = await RegionCaptureTasks.GetRectangleRegionAsync(new RegionCaptureOptions());
        }
        finally
        {
            _selecting = false;
        }

        if (selection == null || selection.Value.Rectangle.Width <= 0 || selection.Value.Rectangle.Height <= 0) return;

        ManualScrollingCaptureWindow window = new(selection.Value.Rectangle);
        _window = window;
        Bitmap? image;
        try
        {
            window.Show();
            image = await window.Completion;
        }
        finally
        {
            _window = null;
        }

        if (image == null) return;

        playNotificationSound?.Invoke();
        if (captured != null) captured(image);
        else image.Dispose();
    }
}

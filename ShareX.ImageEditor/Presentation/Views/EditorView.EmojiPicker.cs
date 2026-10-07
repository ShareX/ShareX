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
using Avalonia.Media;
using Avalonia.Platform;
using ShareX.ImageEditor.Presentation.ViewModels;

namespace ShareX.ImageEditor.Presentation.Views
{
    public partial class EditorView
    {
        private Screen? _emojiPickerScreen;

        private void PositionEmojiPickerOnActiveMonitor()
        {
            ResetModalContentPosition();

            // Pick the monitor once, so browsing emojis does not make the panel chase the cursor.
            if (OperatingSystem.IsWindows() && GetCursorPos(out NativePoint cursorPosition))
            {
                _emojiPickerScreen = TopLevel.GetTopLevel(this)?.Screens?.ScreenFromPoint(
                    new PixelPoint(cursorPosition.X, cursorPosition.Y));
            }

            UpdateEmojiPickerPosition();
        }

        private void OnEditorWindowPositionChanged(object? sender, PixelPointEventArgs e)
        {
            UpdateEmojiPickerPosition();
        }

        private void OnEditorScreensChanged(object? sender, EventArgs e)
        {
            UpdateEmojiPickerPosition();
        }

        private void UpdateEmojiPickerPosition()
        {
            if (_workspaceDisposed || DataContext is not MainViewModel { ModalContent: EmojiPickerDialogViewModel } ||
                Bounds.Width <= 0 || Bounds.Height <= 0 ||
                TopLevel.GetTopLevel(this) is not TopLevel topLevel ||
                this.FindControl<ContentControl>("ModalContentHost") is not ContentControl modalHost)
            {
                return;
            }

            Rect? targetArea = null;
            Screens? screens = topLevel.Screens;
            if (_emojiPickerScreen != null && screens != null)
            {
                _emojiPickerScreen = screens.All.FirstOrDefault(screen => ReferenceEquals(screen, _emojiPickerScreen)) ??
                    screens.All.FirstOrDefault(screen => screen.Equals(_emojiPickerScreen));
                if (_emojiPickerScreen != null)
                {
                    targetArea = GetEditorAreaOnScreen(topLevel, _emojiPickerScreen);
                }
            }

            // The cursor may be on another monitor, or the editor may have moved entirely off
            // the original one. Fall back to the monitor containing the largest editor area.
            if (!targetArea.HasValue)
            {
                _emojiPickerScreen = screens?.ScreenFromVisual(this);
                if (_emojiPickerScreen != null)
                {
                    targetArea = GetEditorAreaOnScreen(topLevel, _emojiPickerScreen);
                }
            }

            Point editorCenter = new(Bounds.Width / 2, Bounds.Height / 2);
            Rect availableArea = targetArea ?? new Rect(Bounds.Size);
            // The picker must also fit a narrow visible portion of a spanning editor.
            if (Math.Abs(modalHost.MaxWidth - availableArea.Width) >= 0.01)
            {
                modalHost.MaxWidth = availableArea.Width;
            }
            if (Math.Abs(modalHost.MaxHeight - availableArea.Height) >= 0.01)
            {
                modalHost.MaxHeight = availableArea.Height;
            }

            Vector offset = availableArea.Center - editorCenter;
            if (Math.Abs(offset.X) < 0.01 && Math.Abs(offset.Y) < 0.01)
            {
                if (modalHost.RenderTransform != null)
                {
                    modalHost.RenderTransform = null;
                }
            }
            else if (modalHost.RenderTransform is not TranslateTransform transform ||
                Math.Abs(transform.X - offset.X) >= 0.01 || Math.Abs(transform.Y - offset.Y) >= 0.01)
            {
                modalHost.RenderTransform = new TranslateTransform(offset.X, offset.Y);
            }
        }

        private Rect? GetEditorAreaOnScreen(TopLevel topLevel, Screen screen)
        {
            // Screen bounds are physical desktop pixels; translate both corners into the
            // editor's logical coordinates before intersecting, including at mixed display DPI.
            Point? topLeft = topLevel.TranslatePoint(topLevel.PointToClient(screen.Bounds.Position), this);
            Point? bottomRight = topLevel.TranslatePoint(topLevel.PointToClient(
                new PixelPoint(screen.Bounds.Right, screen.Bounds.Bottom)), this);

            return topLeft.HasValue && bottomRight.HasValue
                ? GetVisibleEditorArea(new Rect(Bounds.Size), new Rect(topLeft.Value, bottomRight.Value))
                : null;
        }

        private static Rect? GetVisibleEditorArea(Rect editorClientArea, Rect screenArea)
        {
            Rect visibleClientArea = editorClientArea.Intersect(screenArea);
            return visibleClientArea.Width > 0 && visibleClientArea.Height > 0
                ? visibleClientArea
                : null;
        }
    }
}

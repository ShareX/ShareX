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

using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ShareX.AvaloniaUI.Theming;
using ShareX.ImageEditor.Localization;
using System.Globalization;

namespace ShareX.ImageEditor.Presentation.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private static readonly TimeSpan DefaultNotificationDuration = TimeSpan.FromSeconds(2.4);
        private static readonly TimeSpan NotificationHideDuration = TimeSpan.FromMilliseconds(220);
        private static readonly TimeSpan NotificationHoverPollInterval = TimeSpan.FromMilliseconds(50);

        private int _notificationVersion;
        private bool _isNotificationHovered;

        [ObservableProperty]
        private bool _isNotificationVisible;

        [ObservableProperty]
        private bool _isNotificationOpen;

        [ObservableProperty]
        private string _notificationMessage = string.Empty;

        [ObservableProperty]
        private string _notificationIcon = string.Empty;

        public bool HasNotificationIcon => !string.IsNullOrWhiteSpace(NotificationIcon);

        partial void OnNotificationIconChanged(string value)
        {
            OnPropertyChanged(nameof(HasNotificationIcon));
        }

        public void ShowNotification(string message, string? icon = null, TimeSpan? duration = null)
        {
            if (!Options.ShowNotifications || string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            _ = ShowNotificationAsync(message, icon, duration ?? DefaultNotificationDuration);
        }

        private void HideNotification()
        {
            Interlocked.Increment(ref _notificationVersion);
            Volatile.Write(ref _isNotificationHovered, false);
            NotificationMessage = string.Empty;
            NotificationIcon = string.Empty;
            IsNotificationOpen = false;
            IsNotificationVisible = false;
        }

        public void DismissNotification()
        {
            HideNotification();
        }

        internal void SetNotificationHoverState(bool isHovered)
        {
            Volatile.Write(ref _isNotificationHovered, isHovered);
        }

        private void ShowTaskActionNotification(string message, string icon)
        {
            if (Options.AutoCloseEditorOnTask)
            {
                return;
            }

            ShowNotification(message, icon);
        }

        public void ShowOpenImageNotification(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            ShowNotification(BuildFilePathNotification(Strings.MainViewModel_ImageOpened, filePath), EditorIcons.FileOpen);
        }

        public void ShowNewImageNotification(int width = 0, int height = 0)
        {
            ShowNotification(BuildImageSizeNotification(Strings.MainViewModel_NewImageCreated, width, height), EditorIcons.FileNew);
        }

        public void ShowImageCroppedNotification(int width = 0, int height = 0)
        {
            ShowNotification(BuildImageSizeNotification(Strings.MainViewModel_ImageCropped, width, height), EditorIcons.ToolCrop);
        }

        public void ShowImageCutOutNotification(int width = 0, int height = 0)
        {
            ShowNotification(BuildImageSizeNotification(Strings.MainViewModel_ImageCutOut, width, height), EditorIcons.ToolCutOut);
        }

        public void ShowImageInsertedNotification()
        {
            ShowNotification(Strings.MainViewModel_ImageInserted, EditorIcons.ToolImage);
        }

        public void ShowImageAutoCroppedNotification()
        {
            ShowNotification(Strings.MainViewModel_ImageAutoCropped, EditorIcons.ToolCrop);
        }

        public void ShowImageResizedNotification()
        {
            ShowNotification(Strings.MainViewModel_ImageResized, EditorIcons.ToolImage);
        }

        public void ShowCanvasResizedNotification()
        {
            ShowNotification(Strings.MainViewModel_CanvasResized, EditorIcons.PanelBackground);
        }

        public void ShowImageRotatedClockwiseNotification()
        {
            ShowNotification(Strings.MainViewModel_ImageRotated90Clockwise, EditorIcons.ActionRotateRight);
        }

        public void ShowImageRotatedCounterClockwiseNotification()
        {
            ShowNotification(Strings.MainViewModel_ImageRotated90CounterClockwise, EditorIcons.ActionRotateLeft);
        }

        public void ShowImageRotated180Notification()
        {
            ShowNotification(Strings.MainViewModel_ImageRotated180, EditorIcons.ActionRotateRight);
        }

        public void ShowImageRotatedCustomAngleNotification(float angle)
        {
            string formattedAngle = Math.Round(angle, 2).ToString("0.##", CultureInfo.InvariantCulture);
            ShowNotification(string.Format(Strings.MainViewModel_ImageRotatedByDegrees, formattedAngle), EditorIcons.ActionRotateRight);
        }

        public void ShowImageFlippedHorizontallyNotification()
        {
            ShowNotification(Strings.MainViewModel_ImageFlippedHorizontally, EditorIcons.PanelEffects);
        }

        public void ShowImageFlippedVerticallyNotification()
        {
            ShowNotification(Strings.MainViewModel_ImageFlippedVertically, EditorIcons.PanelEffects);
        }

        public void ShowEffectAppliedNotification(string? statusMessage)
        {
            if (string.IsNullOrWhiteSpace(statusMessage))
            {
                ShowNotification(Strings.MainViewModel_ImageEffectApplied, EditorIcons.PanelEffects);
                return;
            }

            string message = statusMessage.EndsWith('.') || statusMessage.EndsWith('!') || statusMessage.EndsWith('?')
                ? statusMessage
                : $"{statusMessage}.";

            ShowNotification(message, EditorIcons.PanelEffects);
        }

        private void ShowSaveNotification(string? savedPath, string icon)
        {
            if (string.IsNullOrWhiteSpace(savedPath))
            {
                return;
            }

            ShowTaskActionNotification(BuildFilePathNotification(Strings.MainViewModel_ImageSavedToFile, savedPath), icon);
        }

        private static string BuildFilePathNotification(string headline, string filePath)
        {
            return $"{headline}\n{string.Format(Strings.MainViewModel_FilePathFormat, filePath)}";
        }

        private static string BuildImageSizeNotification(string headline, int width, int height)
        {
            return width > 0 && height > 0
                ? $"{headline}\n{string.Format(Strings.MainViewModel_SizeFormat, width, height)}"
                : headline;
        }

        private static async Task InvokeRequestedHandlersAsync(Func<Task>? handlers)
        {
            if (handlers == null)
            {
                return;
            }

            Delegate[] invocationList = handlers.GetInvocationList();

            for (int i = 0; i < invocationList.Length; i++)
            {
                Func<Task> handler = (Func<Task>)invocationList[i];
                await handler();
            }
        }

        private static async Task<string?> InvokeRequestedHandlersAsync(Func<Task<string?>>? handlers)
        {
            if (handlers == null)
            {
                return null;
            }

            string? result = null;
            Delegate[] invocationList = handlers.GetInvocationList();

            for (int i = 0; i < invocationList.Length; i++)
            {
                Func<Task<string?>> handler = (Func<Task<string?>>)invocationList[i];
                string? currentResult = await handler();

                if (!string.IsNullOrWhiteSpace(currentResult))
                {
                    result = currentResult;
                }
            }

            return result;
        }

        private async Task ShowNotificationAsync(string message, string? icon, TimeSpan duration)
        {
            int notificationVersion = Interlocked.Increment(ref _notificationVersion);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                NotificationMessage = message;
                NotificationIcon = icon ?? string.Empty;
                IsNotificationVisible = true;
                IsNotificationOpen = true;
            });

            await Task.Delay(duration);

            await WaitForNotificationHoverExitAsync(notificationVersion);

            if (notificationVersion != Volatile.Read(ref _notificationVersion))
            {
                return;
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (notificationVersion == Volatile.Read(ref _notificationVersion))
                {
                    IsNotificationOpen = false;
                }
            });

            await Task.Delay(NotificationHideDuration);

            if (notificationVersion != Volatile.Read(ref _notificationVersion))
            {
                return;
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (notificationVersion == Volatile.Read(ref _notificationVersion))
                {
                    NotificationMessage = string.Empty;
                    NotificationIcon = string.Empty;
                    IsNotificationVisible = false;
                    Volatile.Write(ref _isNotificationHovered, false);
                }
            });
        }

        private async Task WaitForNotificationHoverExitAsync(int notificationVersion)
        {
            while (notificationVersion == Volatile.Read(ref _notificationVersion) && Volatile.Read(ref _isNotificationHovered))
            {
                await Task.Delay(NotificationHoverPollInterval);
            }
        }
    }
}

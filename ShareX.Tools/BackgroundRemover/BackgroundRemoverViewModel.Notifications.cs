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

namespace ShareX.Tools.BackgroundRemover;

public sealed partial class BackgroundRemoverViewModel
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

    private void ShowNotification(string message, string? icon = null)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            _ = ShowNotificationAsync(message, icon, DefaultNotificationDuration);
        }
    }

    public void DismissNotification()
    {
        Interlocked.Increment(ref _notificationVersion);
        Volatile.Write(ref _isNotificationHovered, false);
        NotificationMessage = string.Empty;
        NotificationIcon = string.Empty;
        IsNotificationOpen = false;
        IsNotificationVisible = false;
    }

    internal void SetNotificationHoverState(bool isHovered)
    {
        Volatile.Write(ref _isNotificationHovered, isHovered);
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

        while (notificationVersion == Volatile.Read(ref _notificationVersion) && Volatile.Read(ref _isNotificationHovered))
        {
            await Task.Delay(NotificationHoverPollInterval);
        }

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
}

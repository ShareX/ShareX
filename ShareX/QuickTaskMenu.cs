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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Localization;
using System;

namespace ShareX;

public sealed class QuickTaskMenu
{
    public delegate void TaskInfoSelectedEventHandler(QuickTaskInfo? taskInfo);
    public event TaskInfoSelectedEventHandler? TaskInfoSelected;

    private ContextMenu? _menu;
    private Window? _placementWindow;

    public void ShowMenu()
    {
        Dispatcher.UIThread.Post(ShowMenuCore);
    }

    private void ShowMenuCore()
    {
        CloseMenu();

        ContextMenu menu = new();
        MenuItem continueItem = CreateActionItem(menu, Strings.QuickTaskMenu_ShowMenu_Continue,
            LucideIcons.circle_play, () => OnTaskInfoSelected(null));
        menu.Items.Add(continueItem);
        menu.Items.Add(new Separator());

        if (Program.Settings?.QuickTaskPresets is { Count: > 0 } presets)
        {
            foreach (QuickTaskInfo taskInfo in presets)
            {
                if (taskInfo.IsValid)
                {
                    MenuItem item = CreateActionItem(menu, taskInfo.ToString(), null,
                        () => OnTaskInfoSelected(taskInfo));
                    menu.Items.Add(item);
                }
                else
                {
                    menu.Items.Add(new Separator());
                }
            }

            menu.Items.Add(new Separator());
        }

        menu.Items.Add(CreateActionItem(menu, Strings.QuickTaskMenu_ShowMenu_Edit_this_menu___,
            LucideIcons.pencil, QuickTaskMenuEditorIntegration.Show));
        menu.Items.Add(new Separator());
        menu.Items.Add(CreateActionItem(menu, Strings.QuickTaskMenu_ShowMenu_Cancel,
            LucideIcons.x, null));

        System.Drawing.Point cursorPosition = CaptureHelpers.GetCursorPosition();
        PixelPoint placementPosition = new(cursorPosition.X - 10, cursorPosition.Y - 10);
        Window placementWindow = new()
        {
            Width = 1,
            Height = 1,
            MinWidth = 1,
            MinHeight = 1,
            CanResize = false,
            ShowInTaskbar = false,
            WindowDecorations = WindowDecorations.None,
            Background = Brushes.Transparent,
            Opacity = 0,
            Topmost = true,
            WindowStartupLocation = WindowStartupLocation.Manual,
            Position = placementPosition,
            RequestedThemeVariant = ThemeManager.GetCurrentTheme()
        };

        _menu = menu;
        _placementWindow = placementWindow;

        menu.Placement = PlacementMode.BottomEdgeAlignedLeft;
        menu.PlacementTarget = placementWindow;
        menu.Closed += (_, _) => ClosePlacementWindow(menu, placementWindow);
        placementWindow.Closed += (_, _) => CloseContextMenu(menu, placementWindow);
        placementWindow.Show();
        placementWindow.Position = placementPosition;
        placementWindow.Activate();
        menu.Open(placementWindow);
        Dispatcher.UIThread.Post(() => continueItem.Focus(), DispatcherPriority.Input);
    }

    private static MenuItem CreateActionItem(ContextMenu menu, string header, string? icon, Action? action)
    {
        MenuItem item = new() { Header = header };

        if (!string.IsNullOrEmpty(icon))
        {
            TextBlock iconText = new()
            {
                Text = icon,
                FontSize = 16,
                FontWeight = FontWeight.Normal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            iconText.Classes.Add("icon");
            iconText.Classes.Add("accent-menu-icon");
            item.Icon = iconText;
        }

        item.Click += (_, _) =>
        {
            menu.Close();
            if (action != null)
            {
                Dispatcher.UIThread.Post(action, DispatcherPriority.Background);
            }
        };

        return item;
    }

    private void CloseMenu()
    {
        ContextMenu? menu = _menu;
        Window? placementWindow = _placementWindow;
        _menu = null;
        _placementWindow = null;
        menu?.Close();
        placementWindow?.Close();
    }

    private void ClosePlacementWindow(ContextMenu menu, Window placementWindow)
    {
        if (ReferenceEquals(_menu, menu)) _menu = null;
        if (ReferenceEquals(_placementWindow, placementWindow)) _placementWindow = null;
        if (placementWindow.IsVisible) placementWindow.Close();
    }

    private void CloseContextMenu(ContextMenu menu, Window placementWindow)
    {
        if (ReferenceEquals(_placementWindow, placementWindow)) _placementWindow = null;
        if (ReferenceEquals(_menu, menu))
        {
            _menu = null;
            menu.Close();
        }
    }

    private void OnTaskInfoSelected(QuickTaskInfo? taskInfo) => TaskInfoSelected?.Invoke(taskInfo);
}

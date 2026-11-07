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

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ShareX.ImageEditor.Presentation.ViewModels;

namespace ShareX.ImageEditor.Presentation.Views
{
    public partial class ToolbarCustomizationDialogView : UserControl
    {
        public ToolbarCustomizationDialogView()
        {
            AvaloniaXamlLoader.Load(this);
            AddHandler(InputElement.KeyDownEvent, OnHotkeyKeyDown, RoutingStrategies.Tunnel);
        }

        private void OnMoveUpClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is ToolbarCustomizationDialogViewModel vm && sender is Control { DataContext: ToolbarCustomizationItemViewModel item })
            {
                vm.MoveUp(item);
                e.Handled = true;
            }
        }

        private void OnMoveDownClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is ToolbarCustomizationDialogViewModel vm && sender is Control { DataContext: ToolbarCustomizationItemViewModel item })
            {
                vm.MoveDown(item);
                e.Handled = true;
            }
        }

        private void OnHotkeyKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Source is not TextBox { DataContext: ToolbarCustomizationItemViewModel { IsHotkeyEditable: true } item } textBox)
            {
                return;
            }

            bool isSecondary = textBox.Tag is SecondaryHotkeyTag;

            if (e.Key is Key.Back or Key.Delete || e.Key.ToString().Equals("Backspace", StringComparison.OrdinalIgnoreCase))
            {
                SetHotkey(item, isSecondary, "");
                e.Handled = true;
                return;
            }

            if (IsModifierKey(e.Key))
            {
                e.Handled = true;
                return;
            }

            SetHotkey(item, isSecondary, ToolbarHotkeyHelper.Format(e.Key, e.KeyModifiers));
            e.Handled = true;
        }

        private const string SecondaryHotkeyTag = "Secondary";

        private static void SetHotkey(ToolbarCustomizationItemViewModel item, bool isSecondary, string hotkey)
        {
            if (isSecondary)
            {
                item.SecondaryHotkey = hotkey;
            }
            else
            {
                item.Hotkey = hotkey;
            }
        }

        private static bool IsModifierKey(Key key)
        {
            return key is Key.LeftShift or Key.RightShift
                or Key.LeftCtrl or Key.RightCtrl
                or Key.LeftAlt or Key.RightAlt
                or Key.LWin or Key.RWin;
        }
    }
}

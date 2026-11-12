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
using CommunityToolkit.Mvvm.Input;
using ShareX.ImageEditor.Localization;
using ShareX.ImageEditor.Presentation.ViewModels;

namespace ShareX.ImageEditor.Presentation.Views
{
    public partial class ConfirmationDialogView : UserControl
    {
        private TopLevel? _topLevel;
        private bool _answered;

        public ConfirmationDialogView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            this.FindControl<Button>("YesButton")?.Focus();

            // Listen on the window so Y / N work wherever focus ended up while the dialog is open.
            _topLevel = TopLevel.GetTopLevel(this);
            _topLevel?.AddHandler(KeyDownEvent, OnTopLevelKeyDown, RoutingStrategies.Tunnel);
        }

        private void OnUnloaded(object? sender, RoutedEventArgs e)
        {
            _topLevel?.RemoveHandler(KeyDownEvent, OnTopLevelKeyDown);
            _topLevel = null;
        }

        private void OnTopLevelKeyDown(object? sender, KeyEventArgs e)
        {
            if (_answered || !IsEffectivelyVisible || DataContext is not ConfirmationDialogViewModel vm ||
                (e.KeyModifiers & ~KeyModifiers.Shift) != KeyModifiers.None)
            {
                return;
            }

            if (IsAnswerKey(e, Key.Y, Strings.ConfirmationDialogView_Yes, Strings.ConfirmationDialogView_No))
            {
                Answer(vm.YesCommand, e);
            }
            else if (IsAnswerKey(e, Key.N, Strings.ConfirmationDialogView_No, Strings.ConfirmationDialogView_Yes))
            {
                Answer(vm.NoCommand, e);
            }
        }

        private void Answer(IRelayCommand command, KeyEventArgs e)
        {
            e.Handled = true;

            if (command.CanExecute(null))
            {
                // Held keys repeat; answer once so Yes cannot start a second save.
                _answered = true;
                command.Execute(null);
            }
        }

        // Y and N always work, like a Windows message box. The first letter of the
        // translated button also works (J for "Ja", O for "Oui") unless both buttons share it.
        private static bool IsAnswerKey(KeyEventArgs e, Key key, string label, string otherLabel)
        {
            if (e.Key == key)
            {
                return true;
            }

            string? symbol = e.KeySymbol;
            if (string.IsNullOrEmpty(symbol) || string.IsNullOrEmpty(label))
            {
                return false;
            }

            string first = label.Substring(0, 1);
            if (!string.IsNullOrEmpty(otherLabel) &&
                string.Equals(first, otherLabel.Substring(0, 1), StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }

            return string.Equals(symbol, first, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}

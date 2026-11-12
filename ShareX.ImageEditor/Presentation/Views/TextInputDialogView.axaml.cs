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
using Avalonia.Threading;
using ShareX.ImageEditor.Presentation.Helpers;
using ShareX.ImageEditor.Presentation.ViewModels;

namespace ShareX.ImageEditor.Presentation.Views;

public partial class TextInputDialogView : UserControl
{
    private Key? _pressedKey;

    public TextInputDialogView()
    {
        InitializeComponent();
        Loaded += OnLoaded;

        // Enter and Shift+Enter add a new line in the text box, Ctrl+Enter is OK and Escape cancels.
        // Ctrl+Enter is caught before the text box, which would otherwise add a new line for it too.
        AddHandler(KeyDownEvent, OnPreviewKeyDown, RoutingStrategies.Tunnel);
        AddHandler(KeyUpEvent, OnPreviewKeyUp, RoutingStrategies.Tunnel);
    }

    private void OnPreviewKeyDown(object? sender, KeyEventArgs e)
    {
        _pressedKey = null;

        bool isOk = e.Key == Key.Enter && e.KeyModifiers == KeyModifiers.Control;
        bool isCancel = e.Key == Key.Escape && e.KeyModifiers == KeyModifiers.None;
        if ((isOk || isCancel) && !IsComposing())
        {
            // Act on release, so an IME that takes the press does not close the dialog.
            _pressedKey = e.Key;
            e.Handled = true;
        }
    }

    private void OnPreviewKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key is not (Key.Enter or Key.Escape) || _pressedKey != e.Key)
        {
            return;
        }

        _pressedKey = null;
        e.Handled = true;

        if (IsComposing() || DataContext is not TextInputDialogViewModel viewModel)
        {
            return;
        }

        if (e.Key == Key.Enter)
        {
            viewModel.OkCommand.Execute(null);
        }
        else
        {
            viewModel.CancelCommand.Execute(null);
        }
    }

    private bool IsComposing()
    {
        return this.FindControl<TextBox>("InputBox") is TextBox inputBox && AnnotationTextEditorHelper.IsComposing(inputBox);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (this.FindControl<TextBox>("InputBox") is TextBox inputBox)
            {
                inputBox.Focus();
                inputBox.SelectAll();
            }
        }, DispatcherPriority.Background);
    }
}

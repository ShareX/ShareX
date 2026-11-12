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
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ShareX.ImageEditor.Presentation.ViewModels;

namespace ShareX.ImageEditor.Presentation.Views;

public partial class TextInputDialogView : UserControl
{
    private bool _isOkKeyPressed;

    public TextInputDialogView()
    {
        InitializeComponent();
        Loaded += OnLoaded;

        // Enter and Shift+Enter add a new line (the text box accepts returns), Ctrl+Enter is OK, Escape cancels.
        // Tunnel so Ctrl+Enter is handled before the text box adds a new line for it.
        AddHandler(KeyDownEvent, OnPreviewKeyDown, RoutingStrategies.Tunnel);
        AddHandler(KeyUpEvent, OnPreviewKeyUp, RoutingStrategies.Tunnel);
        AddHandler(KeyUpEvent, OnEscapeKeyUp, RoutingStrategies.Bubble);
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

    private void OnPreviewKeyDown(object? sender, KeyEventArgs e)
    {
        _isOkKeyPressed = false;

        if (e.Key == Key.Enter && e.KeyModifiers.HasFlag(KeyModifiers.Control) && !IsComposing())
        {
            _isOkKeyPressed = true;
            e.Handled = true;
        }
    }

    private void OnPreviewKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter || !_isOkKeyPressed)
        {
            return;
        }

        // Finish on release, so the Enter release does not reach the editor after the dialog closes.
        _isOkKeyPressed = false;
        e.Handled = true;

        if (DataContext is TextInputDialogViewModel viewModel)
        {
            viewModel.OkCommand.Execute(null);
        }
    }

    // An IME can use Enter to confirm its composition.
    private bool IsComposing()
    {
        return this.FindControl<TextBox>("InputBox") is TextBox inputBox &&
            inputBox.GetVisualDescendants().OfType<TextPresenter>().Any(presenter => !string.IsNullOrEmpty(presenter.PreeditText));
    }

    private void OnEscapeKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && e.KeyModifiers == KeyModifiers.None && DataContext is TextInputDialogViewModel viewModel)
        {
            viewModel.CancelCommand.Execute(null);
            e.Handled = true;
        }
    }
}

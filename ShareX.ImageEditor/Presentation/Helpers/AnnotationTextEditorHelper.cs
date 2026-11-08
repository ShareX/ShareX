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
using Avalonia.VisualTree;

namespace ShareX.ImageEditor.Presentation.Helpers;

internal static class AnnotationTextEditorHelper
{
    public static void AttachKeyHandlers(TextBox textBox, Action completeEditing)
    {
        Key? completionKey = null;

        bool IsComposing() => textBox.GetVisualDescendants()
            .OfType<TextPresenter>()
            .Any(presenter => !string.IsNullOrEmpty(presenter.PreeditText));

        void OnKeyDown(object? sender, KeyEventArgs args)
        {
            completionKey = null;

            if (args.Handled || args.Key is not (Key.Enter or Key.Escape) || IsComposing())
            {
                return;
            }

            args.Handled = true;

            // Shift+Enter (common in chat apps and websites) or Ctrl+Enter inserts a new line; plain Enter finishes editing.
            if (args.Key == Key.Enter && (args.KeyModifiers.HasFlag(KeyModifiers.Shift) || args.KeyModifiers.HasFlag(KeyModifiers.Control)))
            {
                string text = textBox.Text ?? string.Empty;
                int selectionStart = Math.Min(textBox.SelectionStart, textBox.SelectionEnd);
                int selectionLength = Math.Abs(textBox.SelectionEnd - textBox.SelectionStart);
                int insertIndex = selectionLength > 0 ? selectionStart : textBox.CaretIndex;
                insertIndex = Math.Clamp(insertIndex, 0, text.Length);
                selectionLength = Math.Min(selectionLength, text.Length - insertIndex);

                textBox.Text = text.Remove(insertIndex, selectionLength).Insert(insertIndex, "\n");
                textBox.ClearSelection();
                textBox.CaretIndex = insertIndex + 1;
            }
            else
            {
                completionKey = args.Key;
            }
        }

        void OnKeyUp(object? sender, KeyEventArgs args)
        {
            if (args.Key is not (Key.Enter or Key.Escape))
            {
                return;
            }

            // An IME can consume the key press and clear its preedit text before releasing Enter.
            // Only finish editing if the matching press was received outside composition.
            bool shouldComplete = completionKey == args.Key && !IsComposing();
            completionKey = null;
            args.Handled = true;

            if (shouldComplete)
            {
                completeEditing();
            }
        }

        void OnLostFocus(object? sender, FocusChangedEventArgs args)
        {
            textBox.KeyDown -= OnKeyDown;
            textBox.KeyUp -= OnKeyUp;
            textBox.LostFocus -= OnLostFocus;
        }

        textBox.KeyDown += OnKeyDown;
        textBox.KeyUp += OnKeyUp;
        textBox.LostFocus += OnLostFocus;
    }
}

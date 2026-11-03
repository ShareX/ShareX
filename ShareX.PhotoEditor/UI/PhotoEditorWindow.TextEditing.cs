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
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using ShareX.PhotoEditor.Integration;
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Dialogs;
using ShareX.PhotoEditor.UI.Panels;

namespace ShareX.PhotoEditor.UI;

public sealed partial class PhotoEditorWindow
{
    public void BeginTextEditing(TextLayer layer, bool isNew)
    {
        if (IsTextEditing) EndTextEditing(true);
        _editingText = layer;
        _editingIsNew = isNew;
        _editingOriginal = (TextLayer)layer.CloneShared();
        bool inline = !layer.Vertical && layer.Transform.IsIdentity;
        _editingWasVisible = layer.Visible;
        if (inline) layer.Visible = false;
        _textEditor = new TextBox
        {
            Text = layer.Text,
            AcceptsReturn = true,
            TextWrapping = layer.IsParagraph ? TextWrapping.Wrap : TextWrapping.NoWrap,
            Background = inline ? Brushes.Transparent : new SolidColorBrush(Color.FromArgb(220, 30, 30, 30)),
            BorderBrush = OverlayContext.AccentBrush,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(0),
            MinWidth = 20,
            CaretBrush = OverlayContext.AccentBrush
        };
        _textEditor.TextChanged += (_, _) =>
        {
            if (_editingText == null) return;
            _editingText.Text = _textEditor.Text ?? string.Empty;
            _editingText.Invalidate();
            InvalidateCanvas();
        };
        _overlayLayer.Children.Add(_textEditor);
        PositionTextEditor();
        _textEditor.AttachedToVisualTree += (_, _) => _textEditor?.Focus();
        RefreshOptions();
        InvalidateCanvas();
        SetStatus("Type the text. Ctrl+Enter commits, Esc cancels.");
    }

    private void PositionTextEditor()
    {
        if (_textEditor == null || _editingText == null || View == null) return;
        TextLayer layer = _editingText;
        bool inline = !layer.Vertical && layer.Transform.IsIdentity;
        double zoom = View.Zoom;
        SKRect block = layer.GetBlockBounds();
        Point p = View.DocToScreen(new SKPoint(block.Left, block.Top));
        if (inline)
        {
            _textEditor.FontFamily = new FontFamily(layer.FontFamily);
            _textEditor.FontSize = Math.Max(4, layer.FontSize * zoom);
            _textEditor.FontWeight = layer.Bold || layer.FauxBold ? FontWeight.Bold : FontWeight.Normal;
            _textEditor.FontStyle = layer.Italic || layer.FauxItalic ? FontStyle.Italic : FontStyle.Normal;
            _textEditor.Foreground = new SolidColorBrush(Color.FromArgb(layer.Color.Alpha, layer.Color.Red, layer.Color.Green, layer.Color.Blue));
            _textEditor.TextAlignment = layer.Alignment switch { Core.TextAlignment.Center => Avalonia.Media.TextAlignment.Center, Core.TextAlignment.Right => Avalonia.Media.TextAlignment.Right, _ => Avalonia.Media.TextAlignment.Left };
            _textEditor.LineHeight = layer.FontSize * layer.Leading * zoom;
            if (layer.IsParagraph)
            {
                _textEditor.Width = layer.BoxSize.Width * zoom;
                _textEditor.MinHeight = layer.BoxSize.Height * zoom;
            }
            Avalonia.Controls.Canvas.SetLeft(_textEditor, p.X - 1);
            Avalonia.Controls.Canvas.SetTop(_textEditor, p.Y - 1);
        }
        else
        {
            _textEditor.FontSize = 14;
            _textEditor.Foreground = Brushes.White;
            _textEditor.Width = 260;
            _textEditor.MinHeight = 80;
            Point below = View.DocToScreen(new SKPoint(block.Left, block.Bottom));
            Avalonia.Controls.Canvas.SetLeft(_textEditor, below.X);
            Avalonia.Controls.Canvas.SetTop(_textEditor, below.Y + 8);
        }
    }

    public void EndTextEditing(bool commit)
    {
        TextLayer? layer = _editingText;
        if (layer == null) return;
        _editingText = null;
        if (_textEditor != null) _overlayLayer.Children.Remove(_textEditor);
        _textEditor = null;
        layer.Visible = _editingWasVisible;
        PhotoDocument? doc = Document;
        if (doc != null)
        {
            if (Tools.Active is TypeTool { IsMask: true } maskTool && maskTool.FinishMaskEditing(layer, commit))
            {
                // Type mask tools convert the text to a selection.
            }
            else if (!commit)
            {
                if (_editingIsNew) doc.RemoveLayer(layer);
                else if (_editingOriginal != null) _editingOriginal.CopyTextPropertiesTo(layer);
                layer.Invalidate();
            }
            else if (string.IsNullOrWhiteSpace(layer.Text))
            {
                if (_editingIsNew) doc.RemoveLayer(layer);
                else doc.History.Push("Edit Type Layer");
            }
            else
            {
                if (_editingIsNew || layer.Name == "Layer") layer.Name = layer.Text.Split('\n')[0].Trim() is { Length: > 0 } first ? first[..Math.Min(first.Length, 32)] : "Text";
                doc.History.Push(_editingIsNew ? "Type Tool" : "Edit Type Layer");
            }
        }
        _editingOriginal = null;
        InvalidateCanvas();
        RefreshPanels();
        RefreshOptions();
        SetStatus(null);
        Canvas.Focus();
    }
}

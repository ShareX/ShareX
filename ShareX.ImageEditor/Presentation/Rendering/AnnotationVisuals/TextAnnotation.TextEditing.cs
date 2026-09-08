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
using ShareX.ImageEditor.Presentation.Helpers;
using ShareX.ImageEditor.Presentation.Controls;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.Annotations;

public partial class TextAnnotation
{
    internal TextBox CreateCreationTextEditor(SolidColorBrush brush)
    {
        var textBrush = Avalonia.Media.Color.TryParse(TextColor, out var c) ? new SolidColorBrush(c) : brush;
        var textBox = new TextBox
        {
            Foreground = textBrush,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.White,
            FontSize = FontSize,
            FontFamily = new Avalonia.Media.FontFamily(FontFamily),
            FontWeight = IsBold ? Avalonia.Media.FontWeight.Bold : Avalonia.Media.FontWeight.Normal,
            FontStyle = IsItalic ? Avalonia.Media.FontStyle.Italic : Avalonia.Media.FontStyle.Normal,
            TextAlignment = TextHorizontalAlignmentHelper.ToAvaloniaTextAlignment(HorizontalAlignment),
            HorizontalContentAlignment = TextHorizontalAlignmentHelper.ToHorizontalContentAlignment(HorizontalAlignment),
            VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Text = string.Empty,
            Padding = new Thickness(4),
            AcceptsReturn = false,
            Tag = this,
            MinWidth = 0
        };

        SetTransparentEditorBackground(textBox);

        if (ShadowEnabled)
        {
            textBox.Effect = ShareX.ImageEditor.Presentation.Helpers.ShadowEffectHelper.CreateDropShadow(this);
        }

        return textBox;
    }

    internal void SetCreatedText(TextBox textBox)
    {
        Text = textBox.Text ?? string.Empty;
        Size naturalSize = OutlinedTextControl.MeasureNaturalSize(this);
        double width = naturalSize.Width > 0 ? naturalSize.Width : textBox.Bounds.Width;
        double height = naturalSize.Height > 0 ? naturalSize.Height : textBox.Bounds.Height;
        EndPoint = new SKPoint(
            (float)(Canvas.GetLeft(textBox) + width),
            (float)(Canvas.GetTop(textBox) + height));
    }

    private static void SetTransparentEditorBackground(TextBox textBox)
    {
        // Force Avalonia's internal text box states to be transparent
        textBox.Resources["TextControlBackground"] = Brushes.Transparent;
        textBox.Resources["TextControlBackgroundFocused"] = Brushes.Transparent;
        textBox.Resources["TextControlBackgroundPointerOver"] = Brushes.Transparent;
    }

    internal TextBox CreateTextEditor()
    {
        string textColor = TextColor;
        if (string.IsNullOrEmpty(textColor) || textColor == "#00000000")
        {
            textColor = string.IsNullOrEmpty(StrokeColor) || StrokeColor == "#00000000"
                ? "#FF000000"
                : StrokeColor;
        }

        var textBox = new TextBox
        {
            Text = Text,
            Foreground = new SolidColorBrush(Color.Parse(textColor)),
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.Gray,
            FontSize = FontSize,
            FontFamily = new Avalonia.Media.FontFamily(string.IsNullOrWhiteSpace(FontFamily) ? "Segoe UI" : FontFamily),
            FontWeight = IsBold ? FontWeight.Bold : FontWeight.Normal,
            FontStyle = IsItalic ? FontStyle.Italic : FontStyle.Normal,
            Padding = new Thickness(4),
            AcceptsReturn = false,
            TextAlignment = TextHorizontalAlignmentHelper.ToAvaloniaTextAlignment(HorizontalAlignment),
            HorizontalContentAlignment = TextHorizontalAlignmentHelper.ToHorizontalContentAlignment(HorizontalAlignment),
            VerticalContentAlignment = global::Avalonia.Layout.VerticalAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
            MinWidth = 20,
            Tag = this
        };

        SetTransparentEditorBackground(textBox);

        // Apply rotation to make editing match display
        if (RotationAngle != 0)
        {
            textBox.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
            textBox.RenderTransform = new RotateTransform(RotationAngle);
        }

        return textBox;
    }
}

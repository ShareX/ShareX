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

namespace ShareX.ImageEditor.Core.Annotations;

public partial class SpeechBalloonAnnotation
{
    internal TextBox CreateTextEditor()
    {
        var (foregroundBrush, editorBackground) = GetTextEditorColors(TextColor);

        var textBox = new TextBox
        {
            Text = Text,
            Background = editorBackground,
            BorderThickness = new Thickness(0),
            CornerRadius = new CornerRadius(0),
            Foreground = foregroundBrush,
            FontSize = FontSize,
            FontFamily = new Avalonia.Media.FontFamily(string.IsNullOrWhiteSpace(FontFamily) ? "Segoe UI" : FontFamily),
            FontWeight = IsBold ? FontWeight.Bold : FontWeight.Normal,
            FontStyle = IsItalic ? FontStyle.Italic : FontStyle.Normal,
            Padding = new Thickness(12),
            TextAlignment = TextHorizontalAlignmentHelper.ToAvaloniaTextAlignment(HorizontalAlignment),
            HorizontalContentAlignment = TextHorizontalAlignmentHelper.ToHorizontalContentAlignment(HorizontalAlignment),
            VerticalContentAlignment = global::Avalonia.Layout.VerticalAlignment.Center,
            AcceptsReturn = false,
            TextWrapping = TextWrapping.Wrap,
            Tag = this
        };

        ApplyTextEditorColors(textBox, foregroundBrush, editorBackground);
        return textBox;
    }

    internal void UpdateTextEditorAppearance(TextBox textBox)
    {
        textBox.FontSize = FontSize;
        textBox.FontFamily = new Avalonia.Media.FontFamily(string.IsNullOrWhiteSpace(FontFamily) ? "Segoe UI" : FontFamily);
        textBox.FontWeight = IsBold ? FontWeight.Bold : FontWeight.Normal;
        textBox.FontStyle = IsItalic ? FontStyle.Italic : FontStyle.Normal;
        textBox.TextAlignment = TextHorizontalAlignmentHelper.ToAvaloniaTextAlignment(HorizontalAlignment);
        textBox.HorizontalContentAlignment = TextHorizontalAlignmentHelper.ToHorizontalContentAlignment(HorizontalAlignment);

        var (foregroundBrush, editorBackground) = GetTextEditorColors(StrokeColor);
        ApplyTextEditorColors(textBox, foregroundBrush, editorBackground);
    }

    private (IBrush Foreground, IBrush Background) GetTextEditorColors(string foregroundColor)
    {
        IBrush foregroundBrush = new SolidColorBrush(Color.Parse(foregroundColor));
        try
        {
            Color foreColor = Color.Parse(foregroundColor);
            Color backColor = Color.Parse(FillColor);
            double foreLum = (0.299 * foreColor.R + 0.587 * foreColor.G + 0.114 * foreColor.B) / 255.0;
            double backLum = (0.299 * backColor.R + 0.587 * backColor.G + 0.114 * backColor.B) / 255.0;
            double backAlpha = backColor.A / 255.0;

            if (backAlpha > 0.1 && Math.Abs(foreLum - backLum) < 0.3)
            {
                foregroundBrush = backLum > 0.5 ? Brushes.Black : Brushes.White;
            }
        }
        catch
        {
            // Keep the requested foreground when contrast cannot be calculated.
        }

        Color fillColor = Color.Parse(FillColor);
        IBrush editorBackground;
        if (fillColor.A < 20)
        {
            Color foreground = (foregroundBrush as SolidColorBrush)?.Color ?? Colors.Black;
            editorBackground = foreground.R > 127
                ? new SolidColorBrush(Color.Parse("#AA000000"))
                : new SolidColorBrush(Color.Parse("#AAFFFFFF"));
        }
        else
        {
            editorBackground = new SolidColorBrush(fillColor);
        }

        return (foregroundBrush, editorBackground);
    }

    private static void ApplyTextEditorColors(TextBox textBox, IBrush foregroundBrush, IBrush editorBackground)
    {
        textBox.Foreground = foregroundBrush;
        textBox.Background = editorBackground;

        textBox.Resources["TextControlBackground"] = editorBackground;
        textBox.Resources["TextControlBackgroundFocused"] = editorBackground;
        textBox.Resources["TextControlBackgroundPointerOver"] = editorBackground;

        textBox.Resources["TextControlBorderThemeThickness"] = new Thickness(0);
        textBox.Resources["TextControlBorderThemeThicknessFocused"] = new Thickness(0);
        textBox.Resources["TextControlBorderThemeThicknessPointerOver"] = new Thickness(0);
        textBox.Resources["TextControlBorderBrush"] = Brushes.Transparent;
        textBox.Resources["TextControlBorderBrushFocused"] = Brushes.Transparent;
        textBox.Resources["TextControlBorderBrushPointerOver"] = Brushes.Transparent;
    }
}

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
using System.Globalization;

namespace ShareX.ImageEditor.Presentation.Controls
{
    public class EffectSlider : Slider
    {
        public static readonly StyledProperty<string> LabelProperty =
            AvaloniaProperty.Register<EffectSlider, string>(
                nameof(Label),
                defaultValue: string.Empty);

        public static readonly StyledProperty<string> ValueStringFormatProperty =
            AvaloniaProperty.Register<EffectSlider, string>(
                nameof(ValueStringFormat),
                defaultValue: "{}{0:0.##}");

        public static readonly StyledProperty<IBrush?> TrackBackgroundProperty =
            AvaloniaProperty.Register<EffectSlider, IBrush?>(
                nameof(TrackBackground));

        public static readonly DirectProperty<EffectSlider, string> ValueTextProperty =
            AvaloniaProperty.RegisterDirect<EffectSlider, string>(
                nameof(ValueText),
                o => o.ValueText);

        private string _valueText = "0";

        public string Label
        {
            get => GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public string ValueStringFormat
        {
            get => GetValue(ValueStringFormatProperty);
            set => SetValue(ValueStringFormatProperty, value);
        }

        public IBrush? TrackBackground
        {
            get => GetValue(TrackBackgroundProperty);
            set => SetValue(TrackBackgroundProperty, value);
        }

        public string ValueText
        {
            get => _valueText;
            private set => SetAndRaise(ValueTextProperty, ref _valueText, value);
        }

        static EffectSlider()
        {
            ValueProperty.Changed.AddClassHandler<EffectSlider>((s, e) => s.UpdateValueText());
            ValueStringFormatProperty.Changed.AddClassHandler<EffectSlider>((s, e) => s.UpdateValueText());
        }

        public EffectSlider()
        {
            UpdateValueText();
        }

        private void UpdateValueText()
        {
            string format = NormalizeFormat(ValueStringFormat);

            try
            {
                ValueText = string.Format(CultureInfo.CurrentCulture, format, Value);
            }
            catch (FormatException)
            {
                ValueText = Value.ToString("0.##", CultureInfo.CurrentCulture);
            }
        }

        private static string NormalizeFormat(string? format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                return "{0:0.##}";
            }

            if (format.StartsWith("{}", StringComparison.Ordinal))
            {
                return format[2..];
            }

            return format;
        }
    }
}

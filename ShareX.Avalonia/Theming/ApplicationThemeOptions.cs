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

using Avalonia.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShareX.AvaloniaUI.Theming;

public sealed class ApplicationThemeOptions : INotifyPropertyChanged
{
    public const string DefaultTheme = "Dark";
    public const string DefaultAccentColorHex = "#09AAFF";

    private string _theme = DefaultTheme;
    private bool _useSystemTheme = false;
    private string _accentColorHex = DefaultAccentColorHex;
    private bool _useSystemAccentColor = false;

    public string Theme
    {
        get => _theme;
        set => SetField(ref _theme, value);
    }

    public bool UseSystemTheme
    {
        get => _useSystemTheme;
        set => SetField(ref _useSystemTheme, value);
    }

    public string AccentColorHex
    {
        get => _accentColorHex;
        set => SetField(ref _accentColorHex, value);
    }

    public Color AccentColor
    {
        get => Color.TryParse(AccentColorHex, out Color color) && color.A > 0
            ? color
            : Color.Parse(DefaultAccentColorHex);
        set => AccentColorHex = $"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}";
    }

    // Newtonsoft.Json convention used by ApplicationConfig without coupling this project to Newtonsoft.Json.
    public bool ShouldSerializeAccentColor() => false;

    public bool UseSystemAccentColor
    {
        get => _useSystemAccentColor;
        set => SetField(ref _useSystemAccentColor, value);
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == nameof(AccentColorHex))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AccentColor)));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

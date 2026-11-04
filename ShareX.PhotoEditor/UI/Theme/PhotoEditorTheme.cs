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
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Styling;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.PhotoEditor.UI.Theme;

/// <summary>Applies the Photo editor's dark, compact theme to its windows.</summary>
public static class PhotoEditorTheme
{
    private static readonly Uri BaseUri = new("avares://ShareX.PhotoEditor/");
    private static readonly Uri ThemeUri = new("avares://ShareX.PhotoEditor/UI/Theme/PhotoEditorTheme.axaml");

    /// <summary>The configured ShareX accent color for code that draws without resources; follows accent changes.</summary>
    public static SolidColorBrush AccentBrush { get; } = CreateAccentBrush();

    private static SolidColorBrush CreateAccentBrush()
    {
        object? resource = null;
        Application.Current?.TryFindResource("ShareX.Color.Accent.Start", ThemeManager.GetCurrentTheme(), out resource);
        SolidColorBrush brush = new(resource is Color color ? color : Colors.White);
        ThemeManager.AccentColorChanged += (_, accent) => brush.Color = accent;
        return brush;
    }

    public static void Apply(Window window)
    {
        window.RequestedThemeVariant = ThemeVariant.Dark;
        window.Styles.Add(new StyleInclude(BaseUri) { Source = ThemeUri });
        window.Bind(Window.BackgroundProperty, window.GetResourceObservable("PE.Brush.Chrome"));
        window.FontFamily = new FontFamily("Segoe UI");
        window.FontSize = 12;
    }
}

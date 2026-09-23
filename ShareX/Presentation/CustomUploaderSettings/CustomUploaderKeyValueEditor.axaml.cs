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

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ShareX;

public partial class CustomUploaderKeyValueEditor : UserControl
{
    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<CustomUploaderKeyValueEditor, string>(nameof(Header), string.Empty);

    public static readonly StyledProperty<CustomUploaderKeyValueCollection?> RowsProperty =
        AvaloniaProperty.Register<CustomUploaderKeyValueEditor, CustomUploaderKeyValueCollection?>(nameof(Rows));

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public CustomUploaderKeyValueCollection? Rows
    {
        get => GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }

    public CustomUploaderKeyValueEditor() => InitializeComponent();

    private void OnAddClick(object? sender, RoutedEventArgs e) => Rows?.AddNew();

    private void OnRemoveClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: CustomUploaderKeyValueRow row }) Rows?.Remove(row);
    }
}

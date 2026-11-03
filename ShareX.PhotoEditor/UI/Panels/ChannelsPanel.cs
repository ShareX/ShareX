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
using Avalonia.Layout;
using Avalonia.Media;
using ShareX.PhotoEditor.UI.Commands;
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PhotoEditor.UI.Panels;

public sealed class ChannelsPanel : EditorPanel
{
    private static readonly IBrush SelectedBrush = new SolidColorBrush(Color.FromRgb(0x4B, 0x4B, 0x4B));
    private readonly StackPanel _list = new();

    public ChannelsPanel(PhotoEditorWindow editor) : base(editor)
    {
        StackPanel footer = new() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Spacing = 2, Margin = new Thickness(4) };
        footer.Children.Add(IconButton("selection", "Load channel as selection", () => EditorCommands.LoadChannelSelection(Editor)));
        footer.Children.Add(IconButton("mask", "Save selection as channel", () => EditorCommands.SaveSelectionToChannel(Editor)));
        footer.Children.Add(IconButton("new-layer", "Create new channel", () => EditorCommands.NewChannel(Editor)));
        footer.Children.Add(IconButton("trash", "Delete current channel", () => EditorCommands.DeleteChannel(Editor)));
        DockPanel root = new();
        DockPanel.SetDock(footer, Dock.Bottom);
        root.Children.Add(footer);
        root.Children.Add(new ScrollViewer { Content = _list });
        Content = root;
    }

    /// <summary>Grayscale thumbnail of one color channel (or the composite).</summary>
    private static unsafe SKBitmap ChannelThumbnail(SKBitmap source, ChannelView view)
    {
        SKBitmap result = ImageOps.CreateBitmap(source.Width, source.Height);
        uint* s = (uint*)source.GetPixels(), d = (uint*)result.GetPixels();
        int stride = source.RowBytes / 4;
        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                uint p = s[y * stride + x];
                if (view == ChannelView.Composite)
                {
                    d[y * stride + x] = p | 0xFF000000u;
                    continue;
                }
                uint v = view switch { ChannelView.Red => (p >> 16) & 0xFF, ChannelView.Green => (p >> 8) & 0xFF, _ => p & 0xFF };
                d[y * stride + x] = 0xFF000000u | (v << 16) | (v << 8) | v;
            }
        }
        return result;
    }

    public override string Id => "channels";

    public override string Title => "Channels";

    public override void Refresh()
    {
        _list.Children.Clear();
        PhotoDocument? doc = Doc;
        if (doc == null) return;
        (ChannelView View, string Name, string Key)[] colorChannels = [(ChannelView.Composite, "RGB", "Ctrl+2"), (ChannelView.Red, "Red", "Ctrl+3"), (ChannelView.Green, "Green", "Ctrl+4"), (ChannelView.Blue, "Blue", "Ctrl+5")];
        Editor.View?.Update(true);
        SKBitmap? composite = Editor.View?.Composite;
        using SKBitmap? small = composite != null ? ImageOps.Resize(composite, Math.Max(1, (int)(30f * composite.Width / Math.Max(composite.Width, composite.Height))), Math.Max(1, (int)(30f * composite.Height / Math.Max(composite.Width, composite.Height))), ResampleMethod.Bilinear) : null;
        foreach (var (view, name, key) in colorChannels)
        {
            // In composite view every color channel is active, like the reference panel.
            bool selected = doc.ActiveChannel == null && (doc.ChannelView == view || doc.ChannelView == ChannelView.Composite);
            Grid content = new() { ColumnDefinitions = new ColumnDefinitions("28,Auto,*,Auto") };
            Border eye = new() { Width = 28, BorderBrush = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24)), BorderThickness = new Thickness(0, 0, 1, 0), Child = doc.ChannelView == view || doc.ChannelView == ChannelView.Composite ? ToolIcons.Create("eye", 15) : null };
            content.Children.Add(eye);
            Control thumb = small != null ? new Border { Width = 32, Height = 32, BorderBrush = new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x1A)), BorderThickness = new Thickness(1), Margin = new Thickness(6, 0, 0, 0), Child = new Image { Source = BitmapHelper.ToAvalonia(ChannelThumbnail(small, view)) } } : new Border();
            Grid.SetColumn(thumb, 1);
            content.Children.Add(thumb);
            TextBlock label = Text(name);
            label.Margin = new Thickness(8, 0, 0, 0);
            Grid.SetColumn(label, 2);
            content.Children.Add(label);
            TextBlock shortcut = new() { Text = key, Classes = { "dim" }, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 10, 0) };
            Grid.SetColumn(shortcut, 3);
            content.Children.Add(shortcut);
            Border row = new() { Height = 38, Background = selected ? SelectedBrush : Brushes.Transparent, BorderBrush = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24)), BorderThickness = new Thickness(0, 0, 0, 1), Child = content };
            row.PointerPressed += (_, _) =>
            {
                doc.ActiveChannel = null;
                EditorCommands.SetChannelView(Editor, view);
            };
            _list.Children.Add(row);
        }
        if (doc.QuickMask) _list.Children.Add(new Border { Padding = new Thickness(6, 4), Background = SelectedBrush, Child = Text("Quick Mask", 12, true) });
        foreach (AlphaChannel channel in doc.Channels)
        {
            CheckBox visible = new() { IsChecked = channel.Visible };
            ToolTip.SetTip(visible, "Show the channel as an overlay");
            visible.IsCheckedChanged += (_, _) =>
            {
                channel.Visible = visible.IsChecked == true;
                Editor.InvalidateCanvas();
            };
            Border row = new() { Padding = new Thickness(6, 4), Background = doc.ActiveChannel == channel ? SelectedBrush : Brushes.Transparent, Child = Row(visible, Text(channel.Name)) };
            row.PointerPressed += async (_, e) =>
            {
                if (e.Source is CheckBox) return;
                if (e.ClickCount >= 2)
                {
                    string? name = await DialogBuilder.PromptAsync(Editor, "Channel Options", "Name:", channel.Name);
                    if (!string.IsNullOrWhiteSpace(name)) { channel.Name = name; Editor.Commit("Channel Options"); }
                    return;
                }
                if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
                {
                    doc.SetSelection(channel.Mask.Clone());
                    Editor.Commit("Load Selection");
                    return;
                }
                doc.ActiveChannel = doc.ActiveChannel == channel ? null : channel;
                if (doc.ActiveChannel != null) channel.Visible = true;
                Editor.RefreshPanels();
                Editor.InvalidateCanvas();
            };
            _list.Children.Add(row);
        }
    }
}

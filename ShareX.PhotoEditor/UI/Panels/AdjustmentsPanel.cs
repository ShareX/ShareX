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
using Avalonia.Layout;
using ShareX.PhotoEditor.UI.Commands;

namespace ShareX.PhotoEditor.UI.Panels;

public sealed class AdjustmentsPanel : EditorPanel
{
    public AdjustmentsPanel(PhotoEditorWindow editor) : base(editor)
    {
        // Icon rows match the groups of the New Adjustment Layer menu.
        TextBlock hint = new() { Text = Strings.AdjustmentsPanel_AddAdjustment, Margin = new Thickness(10, 8, 10, 4), Classes = { "dim" } };
        StackPanel root = new() { Spacing = 2 };
        root.Children.Add(hint);
        foreach (AdjustmentKind[] row in EditorCommands.AdjustmentLayerGroups)
        {
            WrapPanel line = new() { Orientation = Orientation.Horizontal, ItemSpacing = 2, LineSpacing = 2, Margin = new Thickness(8, 0) };
            foreach (AdjustmentKind kind in row)
            {
                Button button = new()
                {
                    Classes = { "icon" },
                    Content = new IconView(IconMap.ForAdjustment(kind), 16),
                    Width = 30,
                    Height = 30,
                    Padding = new Thickness(0),
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };
                string name = AdjustmentSettings.GetName(kind);
                button.PointerEntered += (_, _) => hint.Text = name;
                button.PointerExited += (_, _) => hint.Text = Strings.AdjustmentsPanel_AddAdjustment;
                ToolTip.SetTip(button, string.Format(Strings.AdjustmentsPanel_CreateAdjustmentLayerTip, name));
                AdjustmentKind captured = kind;
                button.Click += (_, _) => EditorCommands.NewAdjustmentLayer(Editor, captured);
                line.Children.Add(button);
            }
            root.Children.Add(line);
        }
        Content = new ScrollViewer { Content = root, HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled };
    }

    public override string Id => "adjustments";

    public override string Title => Strings.EditorCommands_Adjustments;

    public override void Refresh()
    {
    }
}

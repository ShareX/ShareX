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
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.Tools;

public partial class InspectWindowPickerOverlay : Window
{
    public event EventHandler<PixelPoint>? TargetPicked;
    public event EventHandler? PickingCanceled;

    public InspectWindowPickerOverlay()
    {
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
    }

    public InspectWindowPickerOverlay(Screen screen, bool selectTopLevelWindow) : this()
    {
        Position = screen.Bounds.Position;
        Width = screen.Bounds.Width / screen.Scaling;
        Height = screen.Bounds.Height / screen.Scaling;
        Cursor = new Cursor(StandardCursorType.Cross);

        TextBlock? instruction = this.FindControl<TextBlock>("InstructionText");
        if (instruction != null)
        {
            instruction.Text = selectTopLevelWindow
                ? Localization.Strings.InspectWindowPickerOverlay_Click_window
                : Localization.Strings.InspectWindowPickerOverlay_Click_control;
        }

        PointerReleased += OnPointerReleased;
        KeyDown += OnKeyDown;
        Opened += (_, _) => Focus();
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Right)
        {
            PickingCanceled?.Invoke(this, EventArgs.Empty);
            e.Handled = true;
            return;
        }

        if (e.InitialPressMouseButton != MouseButton.Left)
        {
            return;
        }

        Point position = e.GetPosition(this);
        PixelPoint screenPoint = new(
            Position.X + (int)Math.Round(position.X * RenderScaling),
            Position.Y + (int)Math.Round(position.Y * RenderScaling));
        TargetPicked?.Invoke(this, screenPoint);
        e.Handled = true;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            PickingCanceled?.Invoke(this, EventArgs.Empty);
            e.Handled = true;
        }
    }
}

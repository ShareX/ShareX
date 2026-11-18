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
using Avalonia.Media;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Presentation.Controls;
using ShareX.ImageEditor.Presentation.ViewModels;

namespace ShareX.ImageEditor.Presentation.Views;

public partial class EditorBuiltInToolbars : UserControl
{
    private static readonly CornerRadius DockedTopToolbarCornerRadius = new(0, 0, 4, 4);
    private static readonly CornerRadius FloatingTopToolbarCornerRadius = new(4);

    private readonly Border _topToolbarHost;
    private readonly TranslateTransform _topToolbarTransform = new();

    // The offset the user chose; the drawn offset is clamped so the toolbar stays inside the view.
    private Point _topToolbarOffset;
    private bool _draggingTopToolbar;
    private Point _topToolbarDragStartPointer;
    private Point _topToolbarDragStartOffset;

    public AnnotationToolbar AnnotationToolbar { get; private set; } = null!;

    public event EventHandler<double>? ZoomChanged;
    public event EventHandler? ZoomToFitRequested;

    public EditorBuiltInToolbars()
    {
        AvaloniaXamlLoader.Load(this);
        AnnotationToolbar = this.FindControl<AnnotationToolbar>("AnnotationToolbarControl")!;
        _topToolbarHost = this.FindControl<Border>("TopToolbarHost")!;

        AnnotationToolbar.RenderTransform = _topToolbarTransform;
        _topToolbarHost.PropertyChanged += (_, e) =>
        {
            if (e.Property == BoundsProperty)
            {
                ApplyTopToolbarOffset();
            }
        };
    }

    public void OpenFileMenu() => AnnotationToolbar.OpenFileMenu();

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        _draggingTopToolbar = false;
        _topToolbarOffset = GetOptions() is { } options
            ? new Point(SanitizeOffset(options.TopToolbarOffsetX), SanitizeOffset(options.TopToolbarOffsetY))
            : default;
        ApplyTopToolbarOffset();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == BoundsProperty)
        {
            ApplyTopToolbarOffset();
        }
    }

    private void OnZoomChanged(object? sender, double zoom) => ZoomChanged?.Invoke(this, zoom);

    private void OnZoomToFitRequested(object? sender, EventArgs e) => ZoomToFitRequested?.Invoke(this, EventArgs.Empty);

    private void OnTopToolbarDragHandlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control handle || !e.GetCurrentPoint(handle).Properties.IsLeftButtonPressed)
        {
            return;
        }

        e.Handled = true;

        if (e.ClickCount >= 2)
        {
            _draggingTopToolbar = false;
            _topToolbarOffset = default;
            ApplyTopToolbarOffset();
            SaveTopToolbarOffset();
            return;
        }

        // Start from where the toolbar is drawn, which may be clamped inside the view.
        _topToolbarOffset = new Point(_topToolbarTransform.X, _topToolbarTransform.Y);
        _topToolbarDragStartOffset = _topToolbarOffset;
        _topToolbarDragStartPointer = e.GetPosition(this);
        _draggingTopToolbar = true;
        e.Pointer.Capture(handle);
    }

    private void OnTopToolbarDragHandlePointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_draggingTopToolbar)
        {
            return;
        }

        _topToolbarOffset = _topToolbarDragStartOffset + (e.GetPosition(this) - _topToolbarDragStartPointer);
        ApplyTopToolbarOffset();
        e.Handled = true;
    }

    private void OnTopToolbarDragHandlePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_draggingTopToolbar)
        {
            return;
        }

        e.Pointer.Capture(null);
        EndTopToolbarDrag();
        e.Handled = true;
    }

    private void OnTopToolbarDragHandlePointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        EndTopToolbarDrag();
    }

    private void EndTopToolbarDrag()
    {
        if (!_draggingTopToolbar)
        {
            return;
        }

        _draggingTopToolbar = false;

        // Save where the toolbar ended up after clamping, not where the pointer was released.
        _topToolbarOffset = new Point(_topToolbarTransform.X, _topToolbarTransform.Y);
        SaveTopToolbarOffset();
    }

    private void ApplyTopToolbarOffset()
    {
        Rect toolbarBounds = AnnotationToolbar.Bounds;
        Rect hostBounds = _topToolbarHost.Bounds;
        Size viewSize = Bounds.Size;
        double x = 0, y = 0;

        if (toolbarBounds.Width > 0 && viewSize.Width > 0)
        {
            // Bounds ignore render transforms, so this is where the toolbar sits before the offset.
            double left = hostBounds.X + toolbarBounds.X;
            double top = hostBounds.Y + toolbarBounds.Y;
            x = Math.Clamp(_topToolbarOffset.X, Math.Min(0, -left), Math.Max(0, viewSize.Width - left - toolbarBounds.Width));
            y = Math.Clamp(_topToolbarOffset.Y, Math.Min(0, -top), Math.Max(0, viewSize.Height - top - toolbarBounds.Height));
        }

        _topToolbarTransform.X = x;
        _topToolbarTransform.Y = y;

        // Docked to the top edge the toolbar keeps its flat top; anywhere else it is a floating panel.
        AnnotationToolbar.MainToolbarCornerRadius = y > 0.5 ? FloatingTopToolbarCornerRadius : DockedTopToolbarCornerRadius;
    }

    private void SaveTopToolbarOffset()
    {
        if (GetOptions() is { } options)
        {
            options.TopToolbarOffsetX = Math.Round(_topToolbarOffset.X);
            options.TopToolbarOffsetY = Math.Round(_topToolbarOffset.Y);
        }
    }

    private ImageEditorOptions? GetOptions() => (DataContext as MainViewModel)?.Options;

    private static double SanitizeOffset(double value) => double.IsFinite(value) ? value : 0;
}

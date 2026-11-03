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
using Avalonia.Media;

namespace ShareX.PhotoEditor.UI.Panels;

public sealed class NavigatorPanel : EditorPanel
{
    private readonly Image _image = new() { Stretch = Stretch.Uniform };
    private readonly Canvas _overlay = new();
    private readonly Border _viewRect = new() { BorderBrush = Brushes.Red, BorderThickness = new Thickness(2), IsHitTestVisible = false };
    private readonly Slider _zoom = new() { Minimum = -6.6, Maximum = 5, Width = 160 };
    private readonly TextBlock _zoomText = new() { Width = 56, VerticalAlignment = VerticalAlignment.Center };
    private readonly Grid _host = new() { Height = 170 };
    private bool _updating;

    public NavigatorPanel(PhotoEditorWindow editor) : base(editor)
    {
        _overlay.Children.Add(_viewRect);
        _host.Children.Add(_image);
        _host.Children.Add(_overlay);
        _host.PointerPressed += (_, e) => PanTo(e.GetPosition(_image));
        _host.PointerMoved += (_, e) => { if (e.GetCurrentPoint(_host).Properties.IsLeftButtonPressed) PanTo(e.GetPosition(_image)); };
        _image.SizeChanged += (_, _) => UpdateRect();
        _zoom.ValueChanged += (_, e) =>
        {
            if (_updating || Editor.View == null) return;
            Editor.View.ZoomAt(Math.Pow(2, e.NewValue), new Point(Editor.View.Viewport.Width / 2, Editor.View.Viewport.Height / 2));
            Editor.InvalidateOverlay();
            UpdateRect();
        };
        StackPanel root = new() { Margin = new Thickness(6), Spacing = 6 };
        root.Children.Add(_host);
        root.Children.Add(Row(_zoomText, _zoom));
        Content = root;
    }

    public override string Id => "navigator";

    public override string Title => "Navigator";

    private void PanTo(Point p)
    {
        DocumentView? view = Editor.View;
        if (view == null || _image.Bounds.Width <= 0) return;
        double scale = Math.Min(_image.Bounds.Width / view.Document.Width, _image.Bounds.Height / view.Document.Height);
        double ox = (_image.Bounds.Width - view.Document.Width * scale) / 2, oy = (_image.Bounds.Height - view.Document.Height * scale) / 2;
        SKPoint target = new((float)((p.X - ox) / scale), (float)((p.Y - oy) / scale));
        Point screen = view.DocToScreen(target);
        view.Pan += new Point(view.Viewport.Width / 2, view.Viewport.Height / 2) - screen;
        Editor.InvalidateOverlay();
        UpdateRect();
    }

    public override void Refresh()
    {
        DocumentView? view = Editor.View;
        if (view?.Composite == null)
        {
            _image.Source = null;
            return;
        }
        float scale = Math.Min(240f / view.Document.Width, 170f / view.Document.Height);
        using SKBitmap thumb = ImageOps.Resize(view.Composite, Math.Max(1, (int)(view.Document.Width * scale)), Math.Max(1, (int)(view.Document.Height * scale)), ResampleMethod.Bilinear);
        _image.Source = BitmapHelper.ToAvalonia(thumb);
        UpdateRect();
    }

    public void UpdateRect()
    {
        DocumentView? view = Editor.View;
        if (view == null || _image.Bounds.Width <= 0) return;
        _updating = true;
        _zoom.Value = Math.Log2(view.Zoom);
        _zoomText.Text = $"{view.Zoom * 100:0.##}%";
        _updating = false;
        double scale = Math.Min(_image.Bounds.Width / view.Document.Width, _image.Bounds.Height / view.Document.Height);
        double ox = _image.Bounds.X + (_image.Bounds.Width - view.Document.Width * scale) / 2, oy = _image.Bounds.Y + (_image.Bounds.Height - view.Document.Height * scale) / 2;
        SKPoint a = view.ScreenToDoc(new Point(0, 0)), b = view.ScreenToDoc(new Point(view.Viewport.Width, view.Viewport.Height));
        Canvas.SetLeft(_viewRect, ox + Math.Min(a.X, b.X) * scale);
        Canvas.SetTop(_viewRect, oy + Math.Min(a.Y, b.Y) * scale);
        _viewRect.Width = Math.Abs(b.X - a.X) * scale;
        _viewRect.Height = Math.Abs(b.Y - a.Y) * scale;
    }
}

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

namespace ShareX.ImageEditor.Integration;

/*
 * ============================================================================
 * PLATFORM HOST EXAMPLES
 * ============================================================================
 *
 * This file documents how to integrate EditorCore into platform-specific UIs.
 *
 * ----------------------------------------------------------------------------
 * WINFORMS EXAMPLE (using SkiaSharp.Views.WindowsForms)
 * ----------------------------------------------------------------------------
 *
 * 1. Install NuGet package: SkiaSharp.Views.WindowsForms
 *
 * 2. Create a form with SKGLControl:
 *
 *    public class EditorForm : Form
 *    {
 *        private readonly EditorCore _editor = new();
 *        private readonly SKGLControl _canvas;
 *
 *        public EditorForm()
 *        {
 *            _canvas = new SKGLControl { Dock = DockStyle.Fill };
 *            Controls.Add(_canvas);
 *
 *            // Connect events
 *            _canvas.PaintSurface += OnPaintSurface;
 *            _canvas.MouseDown += OnMouseDown;
 *            _canvas.MouseMove += OnMouseMove;
 *            _canvas.MouseUp += OnMouseUp;
 *
 *            _editor.InvalidateRequested += () => _canvas.Invalidate();
 *        }
 *
 *        public void LoadImage(string path) => _editor.LoadImage(path);
 *
 *        private void OnPaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
 *        {
 *            _editor.Render(e.Surface.Canvas);
 *        }
 *
 *        private void OnMouseDown(object sender, MouseEventArgs e)
 *        {
 *            _editor.OnPointerPressed(new SKPoint(e.X, e.Y), e.Button == MouseButtons.Right);
 *        }
 *
 *        private void OnMouseMove(object sender, MouseEventArgs e)
 *        {
 *            if (e.Button != MouseButtons.None)
 *                _editor.OnPointerMoved(new SKPoint(e.X, e.Y));
 *        }
 *
 *        private void OnMouseUp(object sender, MouseEventArgs e)
 *        {
 *            _editor.OnPointerReleased(new SKPoint(e.X, e.Y));
 *        }
 *    }
 *
 * ----------------------------------------------------------------------------
 * AVALONIA EXAMPLE (using Avalonia.Skia)
 * ----------------------------------------------------------------------------
 *
 * 1. Create a custom control that renders via SkiaSharp:
 *
 *    public class EditorCanvas : SKCanvasView
 *    {
 *        private readonly EditorCore _editor = new();
 *
 *        public EditorCore Editor => _editor;
 *
 *        public EditorCanvas()
 *        {
 *            _editor.InvalidateRequested += InvalidateSurface;
 *        }
 *
 *        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
 *        {
 *            _editor.Render(e.Surface.Canvas);
 *        }
 *
 *        protected override void OnPointerPressed(PointerPressedEventArgs e)
 *        {
 *            var point = e.GetPosition(this);
 *            var props = e.GetCurrentPoint(this).Properties;
 *            _editor.OnPointerPressed(new SKPoint((float)point.X, (float)point.Y), props.IsRightButtonPressed);
 *        }
 *
 *        protected override void OnPointerMoved(PointerEventArgs e)
 *        {
 *            var point = e.GetPosition(this);
 *            _editor.OnPointerMoved(new SKPoint((float)point.X, (float)point.Y));
 *        }
 *
 *        protected override void OnPointerReleased(PointerReleasedEventArgs e)
 *        {
 *            var point = e.GetPosition(this);
 *            _editor.OnPointerReleased(new SKPoint((float)point.X, (float)point.Y));
 *        }
 *    }
 *
 * ----------------------------------------------------------------------------
 * TOOL SELECTION
 * ----------------------------------------------------------------------------
 *
 *    // Set active tool
 *    _editor.ActiveTool = EditorTool.Rectangle;
 *    _editor.ActiveTool = EditorTool.Arrow;
 *    _editor.ActiveTool = EditorTool.Blur;
 *
 *    // Set drawing properties
 *    _editor.StrokeColor = "#3b82f6";  // Blue
 *    _editor.StrokeWidth = 5;
 *
 * ----------------------------------------------------------------------------
 * KEYBOARD SHORTCUTS
 * ----------------------------------------------------------------------------
 *
 *    private void OnKeyDown(KeyEventArgs e)
 *    {
 *        if (e.Control && e.Key == Key.Z) _editor.Undo();
 *        if (e.Control && e.Key == Key.Y) _editor.Redo();
 *        if (e.Key == Key.Delete) _editor.DeleteSelected();
 *        if (e.Key == Key.Escape) _editor.Deselect();
 *    }
 *
 * ----------------------------------------------------------------------------
 * SAVE OUTPUT
 * ----------------------------------------------------------------------------
 *
 *    using var snapshot = _editor.GetSnapshot();
 *    if (snapshot != null)
 *    {
 *        using var data = snapshot.Encode(SKEncodedImageFormat.Png, 100);
 *        using var stream = File.OpenWrite("output.png");
 *        data.SaveTo(stream);
 *    }
 *
 */
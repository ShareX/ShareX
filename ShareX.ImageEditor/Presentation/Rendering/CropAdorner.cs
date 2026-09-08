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
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace ShareX.ImageEditor.Presentation.Rendering;

internal sealed class CropAdorner
{
    private readonly List<Rectangle> _cropShadeRects = new();
    private readonly List<Line> _cropGuideLines = new();

    private const int CropShadeZIndex = 5000;
    private const int CropGuideZIndex = 6500;
    private const double MinCropGuideSize = 24;

    private static readonly Color CropShadeFill = Color.FromArgb(140, 0, 0, 0);
    private static readonly Color CropGuideStroke = Color.FromArgb(210, 255, 255, 255);

    public void Attach(Canvas overlay)
    {
        if (_cropShadeRects.Count == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                var shade = new Rectangle
                {
                    Fill = new SolidColorBrush(CropShadeFill),
                    Stroke = null,
                    IsHitTestVisible = false,
                    IsVisible = false
                };
                shade.SetValue(Panel.ZIndexProperty, CropShadeZIndex);
                _cropShadeRects.Add(shade);
                overlay.Children.Add(shade);
            }
        }
        else
        {
            foreach (var shade in _cropShadeRects)
            {
                if (shade.Parent != overlay)
                {
                    (shade.Parent as Panel)?.Children.Remove(shade);
                    overlay.Children.Add(shade);
                }
            }
        }

        if (_cropGuideLines.Count == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                var guide = new Line
                {
                    Stroke = new SolidColorBrush(CropGuideStroke),
                    StrokeThickness = 1,
                    StrokeDashArray = new global::Avalonia.Collections.AvaloniaList<double> { 3, 3 },
                    IsHitTestVisible = false,
                    IsVisible = false
                };
                guide.SetValue(Panel.ZIndexProperty, CropGuideZIndex);
                _cropGuideLines.Add(guide);
                overlay.Children.Add(guide);
            }
        }
        else
        {
            foreach (var guide in _cropGuideLines)
            {
                if (guide.Parent != overlay)
                {
                    (guide.Parent as Panel)?.Children.Remove(guide);
                    overlay.Children.Add(guide);
                }
            }
        }
    }

    public void Hide()
    {
        foreach (var shade in _cropShadeRects)
        {
            shade.IsVisible = false;
        }

        foreach (var guide in _cropGuideLines)
        {
            guide.IsVisible = false;
        }
    }

    public void Update(Canvas overlay, Rect cropRect, Size canvasSize, Point imageOrigin)
    {
        Attach(overlay);

        double canvasWidth = canvasSize.Width;
        double canvasHeight = canvasSize.Height;

        if (canvasWidth <= 0 || canvasHeight <= 0)
        {
            Hide();
            return;
        }

        double left = Math.Clamp(cropRect.Left, 0, canvasWidth);
        double top = Math.Clamp(cropRect.Top, 0, canvasHeight);
        double right = Math.Clamp(cropRect.Right, 0, canvasWidth);
        double bottom = Math.Clamp(cropRect.Bottom, 0, canvasHeight);
        double width = Math.Max(0, right - left);
        double height = Math.Max(0, bottom - top);

        double overlayImageLeft = imageOrigin.X;
        double overlayImageTop = imageOrigin.Y;
        double overlayLeft = left + imageOrigin.X;
        double overlayTop = top + imageOrigin.Y;
        double overlayRight = right + imageOrigin.X;
        double overlayBottom = bottom + imageOrigin.Y;

        SetCropAdornerRect(_cropShadeRects[0], overlayImageLeft, overlayImageTop, canvasWidth, Math.Max(0, overlayTop - overlayImageTop));
        SetCropAdornerRect(_cropShadeRects[1], overlayImageLeft, overlayTop, Math.Max(0, overlayLeft - overlayImageLeft), height);
        SetCropAdornerRect(_cropShadeRects[2], overlayRight, overlayTop, Math.Max(0, (overlayImageLeft + canvasWidth) - overlayRight), height);
        SetCropAdornerRect(_cropShadeRects[3], overlayImageLeft, overlayBottom, canvasWidth, Math.Max(0, (overlayImageTop + canvasHeight) - overlayBottom));

        bool showGuides = width >= MinCropGuideSize && height >= MinCropGuideSize;
        if (!showGuides)
        {
            foreach (var guide in _cropGuideLines)
            {
                guide.IsVisible = false;
            }
            return;
        }

        double v1 = left + width / 3.0;
        double v2 = left + (2.0 * width / 3.0);
        double h1 = top + height / 3.0;
        double h2 = top + (2.0 * height / 3.0);

        _cropGuideLines[0].StartPoint = new Point(v1 + imageOrigin.X, top + imageOrigin.Y);
        _cropGuideLines[0].EndPoint = new Point(v1 + imageOrigin.X, bottom + imageOrigin.Y);
        _cropGuideLines[1].StartPoint = new Point(v2 + imageOrigin.X, top + imageOrigin.Y);
        _cropGuideLines[1].EndPoint = new Point(v2 + imageOrigin.X, bottom + imageOrigin.Y);
        _cropGuideLines[2].StartPoint = new Point(left + imageOrigin.X, h1 + imageOrigin.Y);
        _cropGuideLines[2].EndPoint = new Point(right + imageOrigin.X, h1 + imageOrigin.Y);
        _cropGuideLines[3].StartPoint = new Point(left + imageOrigin.X, h2 + imageOrigin.Y);
        _cropGuideLines[3].EndPoint = new Point(right + imageOrigin.X, h2 + imageOrigin.Y);

        foreach (var guide in _cropGuideLines)
        {
            guide.IsVisible = true;
        }
    }

    private static void SetCropAdornerRect(Rectangle rect, double left, double top, double width, double height)
    {
        Canvas.SetLeft(rect, left);
        Canvas.SetTop(rect, top);
        rect.Width = Math.Max(0, width);
        rect.Height = Math.Max(0, height);
        rect.IsVisible = rect.Width > 0 && rect.Height > 0;
    }

}

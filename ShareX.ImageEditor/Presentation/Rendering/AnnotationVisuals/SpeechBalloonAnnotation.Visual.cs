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
using ShareX.ImageEditor.Presentation.Controls;
using SkiaSharp;
using static ShareX.ImageEditor.Presentation.Rendering.AnnotationCoordinateHelper;
using static ShareX.ImageEditor.Presentation.Rendering.AnnotationVisualHelpers;

namespace ShareX.ImageEditor.Core.Annotations;

public partial class SpeechBalloonAnnotation
{
    /// <summary>
    /// Creates the Avalonia visual for this annotation (SpeechBalloonControl)
    /// </summary>
    public Control CreateVisual()
    {
        var control = new SpeechBalloonControl
        {
            Annotation = this,
            IsHitTestVisible = true,
            Tag = this
        };

        if (ShadowEnabled)
        {
            control.Effect = ShareX.ImageEditor.Presentation.Helpers.ShadowEffectHelper.CreateDropShadow(this);
        }

        return control;
    }

    internal void UpdateVisual(SpeechBalloonControl balloonControl, bool ensureMinimumSize)
    {
        var balloonBounds = GetInteractionBounds();
        balloonControl.Annotation = this;
        ApplyBoundsControl(balloonControl, balloonBounds, ensureMinimumSize);
        ApplyRotationTransform(balloonControl, RotationAngle, GetRelativeCenterOrigin(GetBounds(), balloonBounds));
        balloonControl.InvalidateVisual();
    }

    internal Control CreatePreviewVisual()
    {
        return new Rectangle
        {
            Stroke = new SolidColorBrush(Color.Parse(StrokeColor)),
            StrokeThickness = StrokeWidth,
            Fill = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)),
            RadiusX = Math.Max(0, CornerRadius),
            RadiusY = Math.Max(0, CornerRadius),
            Tag = this
        };
    }

    internal void UpdatePreviewVisual(Rectangle preview)
    {
        ApplyBoundsControl(preview, GetBounds(), ensureMinimumSize: true);
        preview.RadiusX = Math.Max(0, CornerRadius);
        preview.RadiusY = Math.Max(0, CornerRadius);
    }

    private static RelativePoint GetRelativeCenterOrigin(SKRect innerBounds, SKRect outerBounds)
    {
        if (outerBounds.Width <= 0 || outerBounds.Height <= 0)
        {
            return new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
        }

        return new RelativePoint(
            (innerBounds.MidX - outerBounds.Left) / outerBounds.Width,
            (innerBounds.MidY - outerBounds.Top) / outerBounds.Height,
            RelativeUnit.Relative);
    }

    internal Point GetTailHandlePoint()
    {
        var tailPoint = GetEffectiveTailPoint();
        Point handlePoint = new(tailPoint.X, tailPoint.Y);

        if (RotationAngle == 0)
        {
            return handlePoint;
        }

        var bounds = GetBounds();
        Point center = new(bounds.MidX, bounds.MidY);
        return RotatePoint(handlePoint, center, RotationAngle);
    }

    internal SKPoint GetTailPointFromVisual(Point visualPoint)
    {
        Point unrotatedPoint = visualPoint;

        if (RotationAngle != 0)
        {
            var bounds = GetBounds();
            Point center = new(bounds.MidX, bounds.MidY);
            unrotatedPoint = UnrotatePoint(visualPoint, center, RotationAngle);
        }

        return new SKPoint((float)unrotatedPoint.X, (float)unrotatedPoint.Y);
    }
}

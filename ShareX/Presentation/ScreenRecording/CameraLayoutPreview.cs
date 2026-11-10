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
using Avalonia.Media;
using ShareX.Platform;
using ShareX.AvaloniaUI.Theming;
using System;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX;

/// <summary>A layout illustration only; no camera stream or captured GPU texture is read back.</summary>
internal sealed class CameraLayoutPreview(TaskSettingsCapture settings, Func<DrawingRectangle> region) : Control
{
    // The brushes are looked up while rendering, so a theme or accent change only needs a redraw.
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ThemeManager.AccentColorChanged += OnAccentColorChanged;
        ActualThemeVariantChanged += OnActualThemeVariantChanged;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ThemeManager.AccentColorChanged -= OnAccentColorChanged;
        ActualThemeVariantChanged -= OnActualThemeVariantChanged;
        base.OnDetachedFromVisualTree(e);
    }

    private void OnAccentColorChanged(object? sender, Color color) => InvalidateVisual();

    private void OnActualThemeVariantChanged(object? sender, EventArgs e) => InvalidateVisual();

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        DrawingRectangle target = region();
        if (target.Width <= 0 || target.Height <= 0 || Bounds.Width <= 0 || Bounds.Height <= 0) return;
        double scale = Math.Min(Bounds.Width / target.Width, Bounds.Height / target.Height);
        Rect screen = new((Bounds.Width - target.Width * scale) / 2, (Bounds.Height - target.Height * scale) / 2,
            target.Width * scale, target.Height * scale);
        IBrush accent = FindBrush("ShareX.Brush.Accent");
        // Drawn on the settings panel, so the outlines use the indicator color that keeps 3:1 against it.
        Pen outline = new(FindBrush("ShareX.Brush.Accent.Border"), 1);
        context.DrawRectangle(new SolidColorBrush(Color.FromArgb(30, 128, 128, 128)), outline, screen);
        double aspect = settings.ScreenRecordCameraResolution == CameraCaptureResolution.Size640x480 ? 4d / 3 : 16d / 9;
        int marginPixels = Math.Min(settings.ScreenRecordCameraMargin, Math.Max(0, Math.Min(target.Width, target.Height) / 2 - 1));
        int availableWidth = target.Width - marginPixels * 2, availableHeight = target.Height - marginPixels * 2;
        int widthPixels = Math.Clamp(target.Width * settings.ScreenRecordCameraWidthPercent / 100, 2, availableWidth);
        bool circle = settings.ScreenRecordCameraShape == CameraOverlayShape.Circle;
        int heightPixels = circle ? widthPixels : Math.Max(2, (int)(widthPixels / aspect));
        if (heightPixels > availableHeight)
        {
            heightPixels = availableHeight;
            widthPixels = circle ? heightPixels : Math.Clamp((int)(heightPixels * aspect), 2, availableWidth);
        }
        double width = widthPixels * scale, height = heightPixels * scale, margin = marginPixels * scale;
        bool left = settings.ScreenRecordCameraPosition is CameraOverlayPosition.TopLeft or CameraOverlayPosition.BottomLeft;
        bool top = settings.ScreenRecordCameraPosition is CameraOverlayPosition.TopLeft or CameraOverlayPosition.TopRight;
        Rect camera = new(left ? screen.Left + margin : screen.Right - margin - width,
            top ? screen.Top + margin : screen.Bottom - margin - height, width, height);
        if (settings.ScreenRecordCameraShape == CameraOverlayShape.Circle) context.DrawEllipse(accent, outline, camera);
        else context.DrawRectangle(accent, outline, camera, 2, 2);
    }

    private IBrush FindBrush(string key) =>
        this.TryFindResource(key, ActualThemeVariant, out object? value) && value is IBrush brush ? brush : Brushes.DodgerBlue;
}

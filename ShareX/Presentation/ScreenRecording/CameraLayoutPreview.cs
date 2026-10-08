// SPDX-License-Identifier: GPL-3.0-or-later
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ShareX.ScreenRecordingLib;
using System;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX;

/// <summary>A layout illustration only; no camera stream or captured GPU texture is read back.</summary>
internal sealed class CameraLayoutPreview(TaskSettingsCapture settings, Func<DrawingRectangle> region) : Control
{
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        DrawingRectangle target = region();
        if (target.Width <= 0 || target.Height <= 0 || Bounds.Width <= 0 || Bounds.Height <= 0) return;
        double scale = Math.Min(Bounds.Width / target.Width, Bounds.Height / target.Height);
        Rect screen = new((Bounds.Width - target.Width * scale) / 2, (Bounds.Height - target.Height * scale) / 2,
            target.Width * scale, target.Height * scale);
        IBrush accent = this.FindResource("ShareX.Brush.Accent") as IBrush ?? Brushes.DodgerBlue;
        context.DrawRectangle(new SolidColorBrush(Color.FromArgb(30, 128, 128, 128)), new Pen(accent, 1), screen);
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
        if (settings.ScreenRecordCameraShape == CameraOverlayShape.Circle) context.DrawEllipse(accent, null, camera);
        else context.DrawRectangle(accent, null, camera, 2, 2);
    }
}

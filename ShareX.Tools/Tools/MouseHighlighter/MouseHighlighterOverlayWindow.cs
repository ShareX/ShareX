#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using Avalonia;
using ShareX.HelpersLib;
using SkiaSharp;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ShareX.Tools;

// A native layered window gives both per-pixel alpha and reliable click-through
// behavior without changing Avalonia's renderer or the application's DPI mode.
internal sealed class MouseHighlighterOverlayWindow : NativeWindow, IDisposable
{
    private readonly MouseHighlighterService _service;
    private readonly Rectangle _screenBounds;
    private SKSurface? _surface;
    private IntPtr _dc;
    private IntPtr _bitmap;
    private IntPtr _previousBitmap;
    private int _bufferWidth;
    private int _bufferHeight;
    private bool _visible;

    public MouseHighlighterOverlayWindow(MouseHighlighterService service, PixelRect bounds)
    {
        _service = service;
        _screenBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
        CreateHandle(new CreateParams
        {
            Caption = "ShareX - Mouse highlighter overlay",
            Style = unchecked((int)WindowStyles.WS_POPUP),
            ExStyle = (int)(WindowStyles.WS_EX_LAYERED | WindowStyles.WS_EX_TRANSPARENT |
                WindowStyles.WS_EX_TOOLWINDOW | WindowStyles.WS_EX_NOACTIVATE),
            X = bounds.X, Y = bounds.Y, Width = 1, Height = 1
        });
    }

    public void Refresh()
    {
        MouseHighlighterOptions options = _service.Options;
        Rectangle bounds = GetEffectBounds(options);
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            if (_visible) NativeMethods.ShowWindow(Handle, 0); // SW_HIDE
            _visible = false;
            return;
        }

        EnsureBuffer(bounds.Width, bounds.Height);
        SKCanvas canvas = _surface!.Canvas;
        canvas.Clear(SKColors.Transparent);
        canvas.Save();
        canvas.Translate(-bounds.X, -bounds.Y);
        Draw(canvas, options);
        canvas.Restore();
        canvas.Flush();

        POINT destination = new(bounds.X, bounds.Y);
        POINT source = new(0, 0);
        SIZE size = new(bounds.Width, bounds.Height);
        BLENDFUNCTION blend = new() { SourceConstantAlpha = 255, AlphaFormat = NativeConstants.AC_SRC_ALPHA };
        if (!UpdateLayeredWindow(Handle, IntPtr.Zero, ref destination, ref size, _dc, ref source, 0, ref blend, NativeConstants.ULW_ALPHA))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        if (!_visible)
        {
            NativeMethods.SetWindowPos(Handle, new IntPtr(NativeConstants.HWND_TOPMOST), bounds.X, bounds.Y, bounds.Width, bounds.Height,
                SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_SHOWWINDOW);
            _visible = true;
        }
    }

    private Rectangle GetEffectBounds(MouseHighlighterOptions options)
    {
        if (options.Mode == MouseHighlightMode.Spotlight)
        {
            return options.AlwaysColor.A == 0 ? Rectangle.Empty : _screenBounds;
        }

        Rectangle bounds = Rectangle.Empty;
        if (options.Mode == MouseHighlightMode.Circle && options.AlwaysColor.A > 0)
        {
            bounds = Rectangle.Intersect(Around(_service.CursorPosition, options.Radius + 2), _screenBounds);
        }
        foreach (MouseHighlight highlight in _service.Highlights)
        {
            Color color = highlight.Secondary ? options.SecondaryColor : options.PrimaryColor;
            if (color.A == 0) continue;
            int radius = options.Mode == MouseHighlightMode.Ripple ? options.RippleSize / 2 + 12 : options.Radius + 2;
            Rectangle effect = Rectangle.Intersect(Around(highlight.Position, radius), _screenBounds);
            if (effect.Width > 0 && effect.Height > 0)
            {
                bounds = bounds.Width <= 0 || bounds.Height <= 0 ? effect : Rectangle.Union(bounds, effect);
            }
        }
        return Rectangle.Intersect(bounds, _screenBounds);
    }

    private static Rectangle Around(System.Drawing.Point center, int radius) =>
        new(center.X - radius, center.Y - radius, radius * 2 + 1, radius * 2 + 1);

    private void Draw(SKCanvas canvas, MouseHighlighterOptions options)
    {
        double time = _service.Time;
        SKPoint cursor = new(_service.CursorPosition.X, _service.CursorPosition.Y);
        if (options.Mode == MouseHighlightMode.Spotlight)
        {
            bool held = _service.Highlights.Any(highlight => !highlight.Released.HasValue);
            MouseHighlight? released = _service.Highlights.LastOrDefault(highlight => highlight.Released.HasValue);
            double press = held ? 1 : released == null ? 0 : FadeOpacity(released, options, time);
            float radius = (float)(options.Radius * (1 - 0.2 * press));
            using SKPaint backdrop = Paint(options.AlwaysColor);
            canvas.DrawRect(_screenBounds.X, _screenBounds.Y, _screenBounds.Width, _screenBounds.Height, backdrop);
            // Punch a feathered hole in the backdrop using premultiplied alpha.
            float feather = Math.Min(8, radius * 0.15f);
            using SKShader mask = SKShader.CreateRadialGradient(cursor, radius + feather,
                [SKColors.Black, SKColors.Black, SKColors.Transparent], [0, (radius - feather) / (radius + feather), 1], SKShaderTileMode.Clamp);
            using SKPaint hole = new() { IsAntialias = true, Shader = mask, BlendMode = SKBlendMode.DstOut };
            canvas.DrawCircle(cursor, radius + feather, hole);
            return;
        }

        if (options.Mode == MouseHighlightMode.Circle)
        {
            using SKPaint always = Paint(options.AlwaysColor);
            canvas.DrawCircle(cursor, options.Radius, always);
        }
        foreach (MouseHighlight highlight in _service.Highlights)
        {
            Color color = highlight.Secondary ? options.SecondaryColor : options.PrimaryColor;
            SKPoint center = new(highlight.Position.X, highlight.Position.Y);
            if (options.Mode == MouseHighlightMode.Circle)
            {
                using SKPaint fill = Paint(color, FadeOpacity(highlight, options, time));
                canvas.DrawCircle(center, options.Radius, fill);
            }
            else
            {
                double progress = Math.Clamp((time - highlight.Started) / options.RippleDuration, 0, 1);
                double fade = highlight.Released.HasValue
                    ? 1 - Math.Clamp((time - highlight.Released.Value) / options.RippleDuration, 0, 1) : 1;
                float radius = (float)(options.RippleSize / 2d * (0.35 + 0.65 * progress));
                using SKPaint ring = Paint(color, fade * options.RippleIntensity);
                ring.Style = SKPaintStyle.Stroke;
                ring.StrokeWidth = (float)(3 * options.RippleIntensity);
                if (highlight.Crosshairs)
                {
                    float gap = radius * 0.4f;
                    canvas.DrawLine(center.X - radius, center.Y, center.X - gap, center.Y, ring);
                    canvas.DrawLine(center.X + gap, center.Y, center.X + radius, center.Y, ring);
                    canvas.DrawLine(center.X, center.Y - radius, center.X, center.Y - gap, ring);
                    canvas.DrawLine(center.X, center.Y + gap, center.X, center.Y + radius, ring);
                }
                else
                {
                    using SKPaint glow = Paint(color, fade * options.RippleIntensity * 0.3);
                    using SKMaskFilter blur = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 3);
                    glow.MaskFilter = blur;
                    canvas.DrawCircle(center, radius, glow);
                    canvas.DrawCircle(center, radius, ring);
                }
            }
        }
    }

    private static double FadeOpacity(MouseHighlight highlight, MouseHighlighterOptions options, double time)
    {
        if (!highlight.Released.HasValue) return 1;
        double age = time - highlight.Released.Value - options.FadeDelay;
        if (age < 0) return 1;
        return options.FadeDuration == 0 ? 0 : Math.Clamp(1 - age / options.FadeDuration, 0, 1);
    }

    private static SKPaint Paint(Color color, double opacity = 1) => new()
    {
        IsAntialias = true,
        Color = new SKColor(color.R, color.G, color.B, (byte)Math.Clamp(Math.Round(color.A * opacity), 0, 255))
    };

    private void EnsureBuffer(int width, int height)
    {
        if (_surface != null && width <= _bufferWidth && height <= _bufferHeight &&
            width >= _bufferWidth / 4 && height >= _bufferHeight / 4) return;
        ReleaseBuffer();
        // Reuse capacity as a ripple grows instead of allocating every frame.
        _bufferWidth = (width + 63) / 64 * 64;
        _bufferHeight = (height + 63) / 64 * 64;
        _dc = NativeMethods.CreateCompatibleDC(IntPtr.Zero);
        BITMAPINFOHEADER info = new(_bufferWidth, _bufferHeight, 32) { biHeight = -_bufferHeight };
        _bitmap = NativeMethods.CreateDIBSection(_dc, ref info, 0, out IntPtr pixels, IntPtr.Zero, 0);
        if (_dc == IntPtr.Zero || _bitmap == IntPtr.Zero || pixels == IntPtr.Zero)
        {
            ReleaseBuffer();
            throw new InvalidOperationException("Could not allocate the mouse highlighter bitmap.");
        }
        _previousBitmap = NativeMethods.SelectObject(_dc, _bitmap);
        _surface = SKSurface.Create(new SKImageInfo(_bufferWidth, _bufferHeight, SKColorType.Bgra8888, SKAlphaType.Premul), pixels, _bufferWidth * 4)
            ?? throw new InvalidOperationException("Could not create the mouse highlighter surface.");
    }

    private void ReleaseBuffer()
    {
        _surface?.Dispose();
        _surface = null;
        if (_previousBitmap != IntPtr.Zero) NativeMethods.SelectObject(_dc, _previousBitmap);
        if (_bitmap != IntPtr.Zero) NativeMethods.DeleteObject(_bitmap);
        if (_dc != IntPtr.Zero) NativeMethods.DeleteDC(_dc);
        _previousBitmap = _bitmap = _dc = IntPtr.Zero;
    }

    protected override void WndProc(ref Message message)
    {
        if (message.Msg == 0x0084) { message.Result = new IntPtr(-1); return; } // HTTRANSPARENT
        if (message.Msg == 0x0021) { message.Result = new IntPtr(3); return; } // MA_NOACTIVATE
        base.WndProc(ref message);
    }

    public void Dispose()
    {
        ReleaseBuffer();
        if (Handle != IntPtr.Zero) DestroyHandle();
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UpdateLayeredWindow(IntPtr window, IntPtr destinationDc, ref POINT destination,
        ref SIZE size, IntPtr sourceDc, ref POINT source, uint colorKey, ref BLENDFUNCTION blend, uint flags);
}

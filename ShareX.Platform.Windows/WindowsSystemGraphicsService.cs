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

using ShareX.Platform.Imaging;
using System;
using Vortice.Direct2D1;
using Vortice.DirectWrite;
using Vortice.Mathematics;
using Vortice.WIC;
using static Vortice.Direct2D1.D2D1;
using static Vortice.DirectWrite.DWrite;

namespace ShareX.Platform.Windows;

/// <summary>Windows colour emoji and standard cursors, preserving the editor's DirectWrite and GDI rendering.</summary>
public sealed partial class WindowsSystemGraphicsService : ISystemGraphicsService, IDisposable
{
    private readonly object syncRoot = new();
    private ID2D1Factory7? d2dFactory;
    private IDWriteFactory? dwriteFactory;
    private IWICImagingFactory? wicFactory;
    private bool initialized;
    private bool disposed;

    public FeatureSupport EmojiSupport => disposed ? FeatureSupport.NotSupported("Windows graphics services have shut down.") : FeatureSupport.Supported;
    public FeatureSupport CursorSupport => disposed ? FeatureSupport.NotSupported("Windows graphics services have shut down.") : FeatureSupport.Supported;

    public PixelBuffer? RenderEmoji(string text, int canvasSize, float fontSize)
    {
        if (string.IsNullOrWhiteSpace(text) || canvasSize <= 0 || !float.IsFinite(fontSize) || fontSize <= 0) return null;
        lock (syncRoot)
        {
            if (disposed || !InitializeEmojiFactories()) return null;
            using IWICBitmap bitmap = wicFactory!.CreateBitmap((uint)canvasSize, (uint)canvasSize,
                PixelFormat.Format32bppPBGRA, BitmapCreateCacheOption.CacheOnLoad);
            using ID2D1RenderTarget target = d2dFactory!.CreateWicBitmapRenderTarget(bitmap, new RenderTargetProperties());
            using ID2D1SolidColorBrush brush = target.CreateSolidColorBrush(new Color4(1f, 1f, 1f, 1f));
            using IDWriteTextFormat format = dwriteFactory!.CreateTextFormat("Segoe UI Emoji", FontWeight.Normal,
                FontStyle.Normal, FontStretch.Normal, fontSize);
            using IDWriteTextLayout layout = dwriteFactory.CreateTextLayout(text, format, canvasSize, canvasSize);
            format.TextAlignment = TextAlignment.Center;
            format.ParagraphAlignment = ParagraphAlignment.Center;
            target.TextAntialiasMode = Vortice.Direct2D1.TextAntialiasMode.Grayscale;
            target.BeginDraw();
            target.Clear(new Color4(0f, 0f, 0f, 0f));
            target.DrawTextLayout(new System.Numerics.Vector2(0f, 0f), layout, brush, DrawTextOptions.EnableColorFont);
            target.EndDraw();
            byte[] pixels = new byte[checked(canvasSize * canvasSize * 4)];
            bitmap.CopyPixels((uint)(canvasSize * 4), pixels);
            return FromPremultipliedBgra(canvasSize, canvasSize, pixels);
        }
    }

    private bool InitializeEmojiFactories()
    {
        if (!initialized)
        {
            initialized = true;
            try
            {
                D2D1CreateFactory(out d2dFactory);
                DWriteCreateFactory(out dwriteFactory);
                wicFactory = new IWICImagingFactory();
            }
            catch
            {
                DisposeFactories();
            }
        }
        return d2dFactory != null && dwriteFactory != null && wicFactory != null;
    }

    internal static PixelBuffer FromPremultipliedBgra(int width, int height, byte[] pixels)
    {
        for (int i = 0; i < checked(width * height * 4); i += 4)
        {
            int alpha = pixels[i + 3];
            for (int channel = 0; channel < 3; channel++)
                pixels[i + channel] = alpha == 0 ? (byte)0 : (byte)Math.Min(255, (pixels[i + channel] * 255 + alpha / 2) / alpha);
        }
        return new PixelBuffer(width, height, pixels);
    }

    private void DisposeFactories()
    {
        wicFactory?.Dispose();
        wicFactory = null;
        dwriteFactory?.Dispose();
        dwriteFactory = null;
        d2dFactory?.Dispose();
        d2dFactory = null;
    }

    public void Dispose()
    {
        lock (syncRoot)
        {
            disposed = true;
            DisposeFactories();
        }
    }
}

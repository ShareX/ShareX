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

#nullable enable

using ShareX.Platform;
using SkiaSharp;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace ShareX.ScreenCaptureLib;

public sealed class AutoCaptureWindowViewModel
{
    private readonly Func<FeatureSupport> _getSupport;
    private int _captureGeneration;

    public bool IsRunning { get; private set; }
    public bool IsClosed { get; private set; }
    public bool IsSelectingRegion { get; private set; }
    public bool IsCapturing { get; private set; }
    public bool IsBusy => IsSelectingRegion || IsCapturing;
    public bool CanEdit => !IsClosed && !IsBusy && Support.IsSupported;
    public FeatureSupport Support => Normalize(_getSupport());
    public static FeatureSupport CurrentCaptureSupport => Normalize(PlatformServices.IsInitialized
        ? PlatformServices.Current.ScreenCapture.Support : Unavailable);

    public AutoCaptureWindowViewModel(Func<FeatureSupport>? getSupport = null)
    {
        _getSupport = getSupport ?? (() => CurrentCaptureSupport);
    }

    public bool CanStart(Rectangle region) => !IsRunning && CanEdit && IsValidRegion(region);

    public bool TryStart(Rectangle region)
    {
        if (!CanStart(region)) return false;
        _captureGeneration++;
        IsRunning = true;
        return true;
    }

    public void Stop()
    {
        IsRunning = false;
        _captureGeneration++;
    }

    public void Close()
    {
        IsClosed = true;
        Stop();
    }

    public bool TryChange(Action change)
    {
        if (!CanEdit) return false;
        change();
        return true;
    }

    public async Task<bool> TrySelectRegionAsync(Func<Task<Rectangle?>> select, Action<Rectangle> apply)
    {
        if (!CanEdit) return false;
        IsSelectingRegion = true;
        try
        {
            Rectangle? region = await select();
            if (IsClosed || !Support.IsSupported || region is not Rectangle selected || !IsValidRegion(selected)) return false;
            apply(selected);
            return true;
        }
        finally
        {
            IsSelectingRegion = false;
        }
    }

    public async Task<bool> TryCaptureAsync(Rectangle region, Func<Rectangle, Task<SKBitmap?>> capture, Action<SKBitmap> publish)
    {
        if (IsClosed || !IsRunning || IsBusy || !IsValidRegion(region)) return false;
        if (!Support.IsSupported) { Stop(); return false; }
        int generation = _captureGeneration;
        IsCapturing = true;
        SKBitmap? bitmap = null;
        try
        {
            bitmap = await capture(region);
            if (IsClosed || !IsRunning || generation != _captureGeneration) return false;
            if (!Support.IsSupported) Stop();
            if (bitmap == null || IsClosed || !IsRunning || generation != _captureGeneration) return false;
            publish(bitmap);
            bitmap = null; // Ownership passes to the caller only after successful publication.
            return true;
        }
        finally
        {
            bitmap?.Dispose();
            IsCapturing = false;
        }
    }

    public static decimal ClampRepeatSeconds(decimal seconds) => Math.Clamp(seconds, 1m, 86400m);
    public static int GetRepeatDelayMilliseconds(decimal seconds) => (int)(ClampRepeatSeconds(seconds) * 1000m);
    private static bool IsValidRegion(Rectangle region) => region.Width > 0 && region.Height > 0;
    private static FeatureSupport Normalize(FeatureSupport support) => support.IsSupported ? FeatureSupport.Supported : Unavailable;
    private static FeatureSupport Unavailable => FeatureSupport.NotSupported(Localization.Strings.AutoCaptureWindow_CaptureUnavailable);
}

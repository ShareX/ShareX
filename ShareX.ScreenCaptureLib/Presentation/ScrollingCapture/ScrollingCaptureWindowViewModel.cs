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
using System;
using System.Threading.Tasks;

namespace ShareX.ScreenCaptureLib;

public sealed class ScrollingCaptureWindowViewModel
{
    public enum StartResult { Completed, Unavailable, Cancelled, Busy, Closed }

    private readonly Func<ScrollMethod, FeatureSupport> _getMethodSupport;
    private readonly Func<FeatureSupport> _getAutoScrollTopSupport;

    public bool IsBusy { get; private set; }
    public bool IsClosed { get; private set; }

    public ScrollingCaptureWindowViewModel(Func<ScrollMethod, FeatureSupport>? getMethodSupport = null,
        Func<FeatureSupport>? getAutoScrollTopSupport = null)
    {
        _getMethodSupport = getMethodSupport ?? GetCurrentMethodSupport;
        _getAutoScrollTopSupport = getAutoScrollTopSupport ?? GetCurrentAutoScrollTopSupport;
    }

    public static FeatureSupport CurrentStartSupport
    {
        get
        {
            foreach (ScrollMethod method in Enum.GetValues<ScrollMethod>())
            {
                if (GetCurrentMethodSupport(method).IsSupported) return FeatureSupport.Supported;
            }
            return Unavailable;
        }
    }

    public FeatureSupport AutoScrollTopSupport => Normalize(_getAutoScrollTopSupport());

    public FeatureSupport GetSupport(ScrollMethod method, bool autoScrollTop = false)
    {
        if (!Enum.IsDefined(method)) return Unavailable;
        FeatureSupport support = Normalize(_getMethodSupport(method));
        return support.IsSupported && autoScrollTop ? AutoScrollTopSupport : support;
    }

    public bool TryChange(ScrollMethod method, bool autoScrollTop, Action change)
    {
        if (IsClosed || IsBusy || !GetSupport(method, autoScrollTop).IsSupported) return false;
        change();
        return true;
    }

    public void Close() => IsClosed = true;

    public async Task<StartResult> TryCaptureAsync(ScrollingCaptureOptions options, Action hide,
        Func<Task> hideDelay, Func<Task<bool>> select, Func<Task> capture)
    {
        if (IsClosed) return StartResult.Closed;
        if (IsBusy) return StartResult.Busy;
        if (!GetSupport(options.ScrollMethod, options.AutoScrollTop).IsSupported) return StartResult.Unavailable;

        IsBusy = true;
        try
        {
            hide();
            await hideDelay();
            if (IsClosed) return StartResult.Closed;
            if (!GetSupport(options.ScrollMethod, options.AutoScrollTop).IsSupported) return StartResult.Unavailable;

            bool selected = await select();
            if (IsClosed) return StartResult.Closed;
            if (!selected) return StartResult.Cancelled;
            if (!GetSupport(options.ScrollMethod, options.AutoScrollTop).IsSupported) return StartResult.Unavailable;

            // The caller clears its previous preview only when capture actually starts.
            await capture();
            return IsClosed ? StartResult.Closed : StartResult.Completed;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static FeatureSupport GetCurrentMethodSupport(ScrollMethod method)
    {
        if (!PlatformServices.IsInitialized) return Unavailable;
        IPlatformServices platform = PlatformServices.Current;
        if (!platform.ScreenCapture.Support.IsSupported || !platform.Windows.Support.IsSupported) return Unavailable;
        return method switch
        {
            ScrollMethod.MouseWheel => platform.Input.MouseWheelSupport,
            ScrollMethod.ScrollMessage => platform.Input.WindowScrollSupport,
            ScrollMethod.DownArrow or ScrollMethod.PageDown => platform.Input.KeyboardSupport,
            _ => Unavailable
        };
    }

    private static FeatureSupport GetCurrentAutoScrollTopSupport()
    {
        if (!PlatformServices.IsInitialized) return Unavailable;
        IInputService input = PlatformServices.Current.Input;
        // The existing manager sends both Home and a window-scroll message for auto-top.
        return input.KeyboardSupport.IsSupported && input.WindowScrollSupport.IsSupported
            ? FeatureSupport.Supported : Unavailable;
    }

    private static FeatureSupport Normalize(FeatureSupport support) => support.IsSupported ? FeatureSupport.Supported : Unavailable;
    private static FeatureSupport Unavailable => FeatureSupport.NotSupported(Localization.Strings.ScrollingCaptureWindow_Unavailable);
}

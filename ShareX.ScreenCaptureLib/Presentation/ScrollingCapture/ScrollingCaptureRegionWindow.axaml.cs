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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ShareX.HelpersLib;
using ShareX.Platform;
using System;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX.ScreenCaptureLib;

public partial class ScrollingCaptureRegionWindow : Window
{
    private const int BorderPixels = 1;

    private readonly int _frameWidth;
    private readonly int _frameHeight;
    private double _windowScaling = 1;
    private bool _closed;

    public ScrollingCaptureRegionWindow()
        : this(new DrawingRectangle(0, 0, 640, 420))
    {
    }

    public ScrollingCaptureRegionWindow(DrawingRectangle regionRectangle)
    {
        _frameWidth = regionRectangle.Width + BorderPixels * 2;
        _frameHeight = regionRectangle.Height + BorderPixels * 2;

        InitializeComponent();
        Position = new PixelPoint(
            regionRectangle.X - BorderPixels,
            regionRectangle.Y - BorderPixels);
        ConfigureGeometry(1);

        Opened += OnOpened;
        ScalingChanged += OnScalingChanged;
        Closed += (_, _) => _closed = true;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        if (_closed) return;
        ConfigureGeometry(RenderScaling);
        ApplyClickThroughToolWindowStyle();
        ApplyNativeFrameRegion();
        Dispatcher.UIThread.Post(ApplyNativeFrameRegion, DispatcherPriority.Loaded);
    }

    private void ConfigureGeometry(double scaling)
    {
        if (_closed) return;
        _windowScaling = FrameWindowShape.NormalizeScaling(scaling);
        var size = FrameWindowShape.GetLogicalSize(_frameWidth, _frameHeight, _windowScaling);
        Width = size.Width;
        Height = size.Height;
    }

    private void OnScalingChanged(object? sender, EventArgs e)
    {
        if (_closed) return;
        ConfigureGeometry(RenderScaling);
        Dispatcher.UIThread.Post(ApplyNativeFrameRegion, DispatcherPriority.Loaded);
    }

    private void ApplyClickThroughToolWindowStyle()
    {
        FrameWindowShape.SetOverlayStyle(this, clickThrough: true);
    }

    private void ApplyNativeFrameRegion()
    {
        if (_closed) return;
        FrameWindowShape.Apply(this, FrameWindowShape.Create(new PlatformRectangle(0, 0, _frameWidth, _frameHeight), BorderPixels));
    }
}

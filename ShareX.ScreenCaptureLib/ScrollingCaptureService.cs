#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

using System;
using System.Drawing;
using System.Threading.Tasks;

namespace ShareX.ScreenCaptureLib;

public sealed class ScrollingCaptureService : IDisposable
{
    private readonly ScrollingCaptureManager _manager;

    public ScrollingCaptureOptions Options { get; }
    public Bitmap Result => _manager.Result;
    public bool IsCapturing => _manager.IsCapturing;

    public ScrollingCaptureService(ScrollingCaptureOptions options)
    {
        Options = options;
        _manager = new ScrollingCaptureManager(options);
    }

    public Task<bool> SelectWindowAsync() => _manager.SelectWindowAsync();

    public Task<ScrollingCaptureStatus> StartCaptureAsync() => _manager.StartCapture();

    public void StopCapture() => _manager.StopCapture();

    public void Dispose() => _manager.Dispose();
}

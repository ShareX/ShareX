using ShareX.ScreenRecordingLib.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Vortice.Direct3D11;

namespace ShareX.ScreenRecordingLib.Video;

internal sealed unsafe class GraphicsCapture : IDisposable
{
    internal sealed record Target(object? Item, Rectangle Source, Point Destination, nint Monitor, nint WindowHandle = 0);

    public int Width { get; }
    public int Height { get; }
    public bool HasFrame => false;
    public long LatestTimestamp { get; private set; }
    public bool HasEnded => true;

    public static List<Target> GetTargets(RecordingOptions options, out int width, out int height)
    {
        throw new PlatformNotSupportedException("Hardware screen recording requires Windows Graphics Capture.");
    }

    public GraphicsCapture(GraphicsDevice graphics, GpuVideoProcessor processor, List<Target> targets, bool cursor, int fps, bool hdrToneMapping = false)
    {
        throw new PlatformNotSupportedException("Hardware screen recording requires Windows Graphics Capture.");
    }

    public void Retarget(List<Target> targets) { }
    public void Start() { }
    public void Update() { }
    public void Dispose() { }
}

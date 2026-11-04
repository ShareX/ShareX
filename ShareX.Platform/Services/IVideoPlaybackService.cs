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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform;

public sealed record VideoPlaybackInfo(double Duration, int Width, int Height);

/// <summary>A frame of a video for a filmstrip, at <see cref="Position"/> seconds.</summary>
public sealed record VideoThumbnail(double Position, PixelBuffer Image);

/// <summary>
/// A video player drawing into a native child view of one of ShareX's windows. The system decodes and synchronizes video and
/// audio. Create, use and dispose it on the UI thread; every event is raised there.
/// </summary>
public interface IVideoPlayback : IDisposable
{
    /// <summary>The child view to embed (an HWND on Windows), described by <see cref="HandleDescriptor"/>.</summary>
    long Handle { get; }

    string HandleDescriptor { get; }

    /// <summary>Width divided by height of the loaded video, or 0 before one is loaded.</summary>
    double AspectRatio { get; }

    /// <summary>True while a play request waits for a pending seek or frame step.</summary>
    bool IsPlaybackRequested { get; }

    double Volume { get; set; }

    bool IsMuted { get; set; }

    event Action<double>? PositionChanged;
    event Action<bool>? IsPlayingChanged;
    event Action<Exception>? PlaybackFailed;
    /// <summary>The video area was clicked (to play or pause).</summary>
    event Action? VideoClicked;
    event Action? AspectRatioChanged;

    /// <summary>Opens a file. Fails with <see cref="System.IO.InvalidDataException"/> when it has no playable video.</summary>
    Task<VideoPlaybackInfo> LoadAsync(string path, CancellationToken cancellationToken = default);

    void Unload();

    void Seek(double seconds);

    void Play();

    void Pause();

    void StepFrame(bool forward);
}

/// <summary>Video playback for the video player and trimmer: Media Foundation on Windows.</summary>
public interface IVideoPlaybackService
{
    FeatureSupport Support { get; }

    /// <summary>Creates a player whose view is a child of <paramref name="parentWindowHandle"/> (that window's native handle).</summary>
    IVideoPlayback CreatePlayback(long parentWindowHandle);

    /// <summary>Small frames spread over a video for a filmstrip, without starting another process.</summary>
    Task<IReadOnlyList<VideoThumbnail>> CreateThumbnailsAsync(string path, double duration, int count, CancellationToken cancellationToken = default);
}

public sealed class UnsupportedVideoPlaybackService(string reason) : IVideoPlaybackService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public IVideoPlayback CreatePlayback(long parentWindowHandle) => throw new PlatformNotSupportedException(Support.Reason);

    public Task<IReadOnlyList<VideoThumbnail>> CreateThumbnailsAsync(string path, double duration, int count, CancellationToken cancellationToken = default) =>
        Task.FromException<IReadOnlyList<VideoThumbnail>>(new PlatformNotSupportedException(Support.Reason));
}

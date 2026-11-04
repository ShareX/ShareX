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

#if WINDOWS10_0_17763_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Windows.Media;

/// <summary>Media Foundation playback in a child HWND, and filmstrip frames from its source reader.</summary>
public sealed class WindowsVideoPlaybackService : IVideoPlaybackService
{
    public FeatureSupport Support => FeatureSupport.Supported;

    public IVideoPlayback CreatePlayback(long parentWindowHandle) => new WindowsVideoPlayback((nint)parentWindowHandle,
        SynchronizationContext.Current ?? throw new InvalidOperationException("Create the video player on the UI thread."));

    public Task<IReadOnlyList<VideoThumbnail>> CreateThumbnailsAsync(string path, double duration, int count, CancellationToken cancellationToken = default) =>
        NativeVideoThumbnails.CreateAsync(path, duration, count, cancellationToken);
}
#endif

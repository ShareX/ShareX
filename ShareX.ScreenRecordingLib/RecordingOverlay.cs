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

namespace ShareX.ScreenRecordingLib;

/// <summary>An image drawn into the recorded video, never on the screen. Called on the recording thread once per frame.</summary>
public interface IRecordingOverlay
{
    /// <summary>The image to draw on a frame of the given size in physical pixels, or null to draw nothing.</summary>
    RecordingOverlayImage? GetImage(int frameWidth, int frameHeight);
}

/// <summary>
/// Premultiplied BGRA pixels, top-down, <c>Width * 4</c> bytes per row, placed at <c>X</c>, <c>Y</c> in the frame.
/// <c>Version</c> changes whenever the pixels change, so unchanged images are not uploaded again.
/// </summary>
public sealed record RecordingOverlayImage(byte[] Pixels, int Width, int Height, int X, int Y, int Version);

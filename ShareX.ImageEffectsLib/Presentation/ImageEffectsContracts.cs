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

using System.Drawing;

namespace ShareX.ImageEffectsLib;

public enum ImageEffectsWindowMode
{
    Presets,
    Tool,
    Editor
}

public sealed record ImageEffectsSource(Bitmap Image, string? FilePath = null);

public sealed class ImageEffectsCallbacks
{
    public Func<ImageEffectsSource?>? LoadImageFromFile { get; init; }
    public Func<ImageEffectsSource?>? LoadImageFromClipboard { get; init; }
    public Func<Bitmap, string?, string?>? SaveImage { get; init; }
    public Action<Bitmap>? UploadImage { get; init; }
    public Action? OpenImageEffectsPage { get; init; }
}

public sealed record ImageEffectsDialogResult(bool Accepted, int SelectedPresetIndex);

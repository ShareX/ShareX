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

using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Controllers;

/// <summary>Owns a decoded image until the editor core or an annotation explicitly takes it.</summary>
internal sealed class EditorImportedImage : IDisposable
{
    private SKBitmap? bitmap;
    public SKBitmap Bitmap => bitmap ?? throw new ObjectDisposedException(nameof(EditorImportedImage));
    public string? SourceFilePath { get; }

    public EditorImportedImage(SKBitmap bitmap, string? sourceFilePath = null)
    {
        this.bitmap = bitmap;
        SourceFilePath = sourceFilePath;
    }

    public SKBitmap TakeBitmap()
    {
        SKBitmap result = Bitmap;
        bitmap = null;
        return result;
    }

    public void Dispose()
    {
        bitmap?.Dispose();
        bitmap = null;
    }
}
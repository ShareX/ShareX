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

using Avalonia;
using Avalonia.Media;

namespace ShareX.Icons;

/// <summary>
/// An SVG file used as an <see cref="IImage"/>, for example as the Source of an Image control. It is drawn as
/// vectors, so it stays sharp at every size and display scale. Its size is the SVG viewBox size. Only the SVG
/// subset that <see cref="SvgIconDrawing"/> reads is supported.
/// </summary>
public sealed class SvgImage : IImage
{
    private readonly SvgIconDrawing _drawing;

    private SvgImage(SvgIconDrawing drawing)
    {
        _drawing = drawing;
        Size = new Size(drawing.Width, drawing.Height);
    }

    public Size Size { get; }

    public static SvgImage Load(Stream stream) => new(SvgIconDrawing.Load(stream));

    public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
    {
        if (sourceRect.Width <= 0 || sourceRect.Height <= 0 || destRect.Width <= 0 || destRect.Height <= 0)
        {
            return;
        }

        Matrix transform = Matrix.CreateTranslation(-sourceRect.X, -sourceRect.Y) *
            Matrix.CreateScale(destRect.Width / sourceRect.Width, destRect.Height / sourceRect.Height) *
            Matrix.CreateTranslation(destRect.X, destRect.Y);

        using (context.PushClip(destRect))
        using (context.PushTransform(transform))
        {
            _drawing.Draw(context, new Rect(Size), Brushes.Black);
        }
    }
}

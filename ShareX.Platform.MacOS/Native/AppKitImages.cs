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

using System;
using System.Collections.Generic;

namespace ShareX.Platform.MacOS.Native;

/// <summary>Reads the pixels of an NSImage: its TIFF form holds every representation (1x, 2x), each re-encoded by NSBitmapImageRep.</summary>
internal static class AppKitImages
{
    // NSBitmapImageFileTypePNG
    private const nint PngFileType = 4;

    /// <summary>A bitmap representation of the image: the one nearest <paramref name="pixelHeight"/>, or the largest when it is null.</summary>
    /// <returns>The representation (autoreleased) and its scale from points to pixels, or zero when the image has no bitmap.</returns>
    public static (IntPtr Representation, double Scale) GetRepresentation(IntPtr image, int? pixelHeight)
    {
        IntPtr tiff = image != IntPtr.Zero ? ObjC.Send(image, "TIFFRepresentation") : IntPtr.Zero;
        IntPtr representations = tiff != IntPtr.Zero ? ObjC.Send(ObjC.GetClass("NSBitmapImageRep"), "imageRepsWithData:", tiff) : IntPtr.Zero;
        nint count = representations != IntPtr.Zero ? CoreFoundation.CFArrayGetCount(representations) : 0;

        if (count == 0)
        {
            return (IntPtr.Zero, 0);
        }

        List<int> heights = new List<int>((int)count);

        for (nint i = 0; i < count; i++)
        {
            heights.Add((int)ObjC.SendNInt(CoreFoundation.CFArrayGetValueAtIndex(representations, i), "pixelsHigh"));
        }

        IntPtr representation = CoreFoundation.CFArrayGetValueAtIndex(representations, SelectRepresentation(heights, pixelHeight));
        CoreGraphics.CGPoint size = ObjC.SendPoint(image, "size");
        double scale = size.Y > 0 ? ObjC.SendNInt(representation, "pixelsHigh") / size.Y : 1;
        return (representation, scale);
    }

    /// <summary>The index of the representation to use: the nearest to the requested height, else the largest. Ties go to the larger.</summary>
    internal static int SelectRepresentation(IReadOnlyList<int> pixelHeights, int? pixelHeight)
    {
        int best = 0;

        for (int i = 1; i < pixelHeights.Count; i++)
        {
            int current = pixelHeights[i], chosen = pixelHeights[best];
            bool better = pixelHeight is int target
                ? Math.Abs(current - target) < Math.Abs(chosen - target) || (Math.Abs(current - target) == Math.Abs(chosen - target) && current > chosen)
                : current > chosen;

            if (better)
            {
                best = i;
            }
        }

        return best;
    }

    /// <summary>A bitmap representation encoded as PNG.</summary>
    public static byte[]? ToPng(IntPtr representation)
    {
        if (representation == IntPtr.Zero)
        {
            return null;
        }

        IntPtr properties = ObjC.Send(ObjC.GetClass("NSDictionary"), "dictionary");
        IntPtr png = ObjC.Send(representation, "representationUsingType:properties:", PngFileType, properties);
        return png != IntPtr.Zero ? CoreFoundation.ToArray(png) : null;
    }
}

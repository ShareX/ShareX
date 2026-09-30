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

using ShareX.Platform.MacOS.Native;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.MacOS;

/// <summary>NSPasteboard through the Objective-C runtime.</summary>
public sealed class MacClipboardService : IClipboardService
{
    private const string StringType = "public.utf8-plain-text";
    private const string PngType = "public.png";
    private const string TiffType = "public.tiff";
    private const nint NSBitmapImageFileTypePNG = 4;

    public FeatureSupport Support => FeatureSupport.Supported;

    public Task<bool> SetTextAsync(string text, CancellationToken cancellationToken = default) => Task.FromResult(Run(pasteboard =>
    {
        ObjC.Send(pasteboard, "clearContents");
        IntPtr value = CoreFoundation.CreateString(text);
        IntPtr type = CoreFoundation.CreateString(StringType);

        try
        {
            return ObjC.SendBool(pasteboard, "setString:forType:", value, type);
        }
        finally
        {
            CoreFoundation.CFRelease(value);
            CoreFoundation.CFRelease(type);
        }
    }));

    public Task<string?> GetTextAsync(CancellationToken cancellationToken = default) => Task.FromResult(Run(pasteboard =>
    {
        IntPtr type = CoreFoundation.CreateString(StringType);

        try
        {
            return CoreFoundation.ToManagedString(ObjC.Send(pasteboard, "stringForType:", type));
        }
        finally
        {
            CoreFoundation.CFRelease(type);
        }
    }));

    public Task<bool> SetImageAsync(byte[] png, CancellationToken cancellationToken = default) => Task.FromResult(Run(pasteboard =>
    {
        ObjC.Send(pasteboard, "clearContents");
        IntPtr data = CoreFoundation.CreateData(png);
        IntPtr type = CoreFoundation.CreateString(PngType);

        try
        {
            return ObjC.SendBool(pasteboard, "setData:forType:", data, type);
        }
        finally
        {
            CoreFoundation.CFRelease(data);
            CoreFoundation.CFRelease(type);
        }
    }));

    public Task<byte[]?> GetImageAsync(CancellationToken cancellationToken = default) => Task.FromResult(Run<byte[]?>(pasteboard =>
    {
        IntPtr data = GetData(pasteboard, PngType);

        if (data != IntPtr.Zero)
        {
            return CoreFoundation.ToArray(data);
        }

        // Screenshots copied by macOS itself and most apps are TIFF. Convert with NSBitmapImageRep.
        data = GetData(pasteboard, TiffType);

        if (data == IntPtr.Zero)
        {
            return null;
        }

        IntPtr imageRep = ObjC.Send(ObjC.GetClass("NSBitmapImageRep"), "imageRepWithData:", data);

        if (imageRep == IntPtr.Zero)
        {
            return null;
        }

        IntPtr png = ObjC.Send(imageRep, "representationUsingType:properties:", NSBitmapImageFileTypePNG, IntPtr.Zero);
        return png != IntPtr.Zero ? CoreFoundation.ToArray(png) : null;
    }));

    public Task<bool> SetFilesAsync(IReadOnlyList<string> paths, CancellationToken cancellationToken = default) => Task.FromResult(Run(pasteboard =>
    {
        ObjC.Send(pasteboard, "clearContents");
        IntPtr[] urls = new IntPtr[paths.Count];

        try
        {
            for (int i = 0; i < paths.Count; i++)
            {
                IntPtr path = CoreFoundation.CreateString(paths[i]);
                urls[i] = CoreFoundation.CFURLCreateWithFileSystemPath(IntPtr.Zero, path, CoreFoundation.kCFURLPOSIXPathStyle, System.IO.Directory.Exists(paths[i]));
                CoreFoundation.CFRelease(path);
            }

            IntPtr array = CoreFoundation.CreateArray(urls);

            try
            {
                return ObjC.SendBool(pasteboard, "writeObjects:", array);
            }
            finally
            {
                CoreFoundation.CFRelease(array);
            }
        }
        finally
        {
            foreach (IntPtr url in urls)
            {
                if (url != IntPtr.Zero) CoreFoundation.CFRelease(url);
            }
        }
    }));

    public Task<IReadOnlyList<string>> GetFilesAsync(CancellationToken cancellationToken = default) => Task.FromResult(Run<IReadOnlyList<string>>(pasteboard =>
    {
        IntPtr urlClass = ObjC.GetClass("NSURL");
        IntPtr classes = CoreFoundation.CreateArray([urlClass]);

        try
        {
            IntPtr urls = ObjC.Send(pasteboard, "readObjectsForClasses:options:", classes, IntPtr.Zero);

            if (urls == IntPtr.Zero)
            {
                return Array.Empty<string>();
            }

            List<string> paths = new List<string>();

            for (nint i = 0; i < CoreFoundation.CFArrayGetCount(urls); i++)
            {
                IntPtr url = CoreFoundation.CFArrayGetValueAtIndex(urls, i);

                if (!ObjC.SendBool(url, "isFileURL"))
                {
                    continue;
                }

                IntPtr path = CoreFoundation.CFURLCopyFileSystemPath(url, CoreFoundation.kCFURLPOSIXPathStyle);

                if (path != IntPtr.Zero)
                {
                    paths.Add(CoreFoundation.ToManagedString(path)!);
                    CoreFoundation.CFRelease(path);
                }
            }

            return paths;
        }
        finally
        {
            CoreFoundation.CFRelease(classes);
        }
    }));

    public Task<bool> ClearAsync(CancellationToken cancellationToken = default) => Task.FromResult(Run(pasteboard =>
    {
        ObjC.Send(pasteboard, "clearContents");
        return true;
    }));

    private static IntPtr GetData(IntPtr pasteboard, string typeName)
    {
        IntPtr type = CoreFoundation.CreateString(typeName);

        try
        {
            return ObjC.Send(pasteboard, "dataForType:", type);
        }
        finally
        {
            CoreFoundation.CFRelease(type);
        }
    }

    private static T Run<T>(Func<IntPtr, T> action) =>
        ObjC.WithAutoreleasePool(() => action(ObjC.Send(ObjC.GetClass("NSPasteboard"), "generalPasteboard")));
}

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
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.MacOS;

/// <summary>
/// Text recognition with Apple's Vision framework (VNRecognizeTextRequest, macOS 10.15+), which ships with macOS: no download.
/// The PNG is decoded with ImageIO into a CGImage; results are read line by line in Vision's reading order.
/// </summary>
[SupportedOSPlatform("macos")]
public sealed unsafe partial class VisionOcrService : IOcrService
{
    private const string VisionFramework = "/System/Library/Frameworks/Vision.framework/Vision";
    private const string ImageIO = "/System/Library/Frameworks/ImageIO.framework/ImageIO";
    private const nint RecognitionLevelAccurate = 0;

    private readonly Lazy<bool> visionLoaded = new Lazy<bool>(() => NativeLibrary.TryLoad(VisionFramework, out _) && NativeLibrary.TryLoad(ImageIO, out _));
    private IReadOnlyList<OcrLanguage>? languages;

    public FeatureSupport Support => visionLoaded.Value
        ? FeatureSupport.Supported
        : FeatureSupport.NotSupported("Text recognition needs Apple's Vision framework (macOS 10.15 or later).");

    public IReadOnlyList<OcrLanguage> GetLanguages()
    {
        if (!Support.IsSupported)
        {
            return [];
        }

        return languages ??= ObjC.WithAutoreleasePool(() =>
        {
            IntPtr request = ObjC.Send(ObjC.Send(ObjC.GetClass("VNRecognizeTextRequest"), "alloc"), "init");

            try
            {
                ObjC.Send(request, "setRecognitionLevel:", RecognitionLevelAccurate);

                // The instance method exists from macOS 12; an unknown selector would abort the process.
                if (!ObjC.SendBool(request, "respondsToSelector:", ObjC.Selector("supportedRecognitionLanguagesAndReturnError:")))
                {
                    return (IReadOnlyList<OcrLanguage>)[new OcrLanguage("en-US", DisplayName("en-US"))];
                }

                IntPtr error = IntPtr.Zero;
                IntPtr tags = ObjC.Send(request, "supportedRecognitionLanguagesAndReturnError:", (IntPtr)(&error));
                return (IReadOnlyList<OcrLanguage>)ReadStrings(tags).Select(tag => new OcrLanguage(tag, DisplayName(tag))).ToList();
            }
            finally
            {
                ObjC.Send(request, "release");
            }
        });
    }

    public Task<string> RecognizeAsync(byte[] png, string languageTag, bool singleLine, CancellationToken cancellationToken = default)
    {
        if (!Support.IsSupported)
        {
            throw new PlatformNotSupportedException(Support.Reason);
        }

        return Task.Run(() => ObjC.WithAutoreleasePool(() => Recognize(png, MatchLanguage(languageTag), singleLine)), cancellationToken);
    }

    /// <summary>Vision uses tags such as "en-US" and "zh-Hans"; an unknown tag falls back to one of the same language, then to Vision's default.</summary>
    internal static string? MatchLanguage(string? tag, IReadOnlyList<string> supported)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return null;
        }

        string? exact = supported.FirstOrDefault(x => string.Equals(x, tag, StringComparison.OrdinalIgnoreCase));
        if (exact != null)
        {
            return exact;
        }

        string language = tag.Split('-', '_')[0];
        return supported.FirstOrDefault(x => string.Equals(x.Split('-', '_')[0], language, StringComparison.OrdinalIgnoreCase));
    }

    private string? MatchLanguage(string languageTag) => MatchLanguage(languageTag, GetLanguages().Select(l => l.Tag).ToList());

    private static string Recognize(byte[] png, string? language, bool singleLine)
    {
        IntPtr data = CoreFoundation.CreateData(png);
        IntPtr source = IntPtr.Zero, image = IntPtr.Zero, handler = IntPtr.Zero, request = IntPtr.Zero;

        try
        {
            source = CGImageSourceCreateWithData(data, IntPtr.Zero);
            image = source != IntPtr.Zero ? CGImageSourceCreateImageAtIndex(source, 0, IntPtr.Zero) : IntPtr.Zero;

            if (image == IntPtr.Zero)
            {
                throw new ArgumentException("The image could not be decoded.", nameof(png));
            }

            IntPtr options = ObjC.Send(ObjC.GetClass("NSDictionary"), "dictionary");
            handler = ObjC.Send(ObjC.Send(ObjC.GetClass("VNImageRequestHandler"), "alloc"), "initWithCGImage:options:", image, options);
            request = ObjC.Send(ObjC.Send(ObjC.GetClass("VNRecognizeTextRequest"), "alloc"), "init");
            ObjC.Send(request, "setRecognitionLevel:", RecognitionLevelAccurate);
            ObjC.Send(request, "setUsesLanguageCorrection:", (IntPtr)1);

            if (language != null)
            {
                IntPtr tag = CoreFoundation.CreateString(language);
                IntPtr list = CoreFoundation.CreateArray([tag]);
                ObjC.Send(request, "setRecognitionLanguages:", list);
                CoreFoundation.CFRelease(list);
                CoreFoundation.CFRelease(tag);
            }

            IntPtr requests = CoreFoundation.CreateArray([request]);
            IntPtr error = IntPtr.Zero;
            bool performed = ObjC.SendBool(handler, "performRequests:error:", requests, (IntPtr)(&error));
            CoreFoundation.CFRelease(requests);

            if (!performed)
            {
                string reason = error != IntPtr.Zero ? CoreFoundation.ToManagedString(ObjC.Send(error, "localizedDescription")) ?? "unknown error" : "unknown error";
                throw new InvalidOperationException("Text recognition failed: " + reason);
            }

            List<string> lines = new List<string>();
            IntPtr results = ObjC.Send(request, "results");
            nint count = results != IntPtr.Zero ? CoreFoundation.CFArrayGetCount(results) : 0;

            for (nint i = 0; i < count; i++)
            {
                IntPtr observation = CoreFoundation.CFArrayGetValueAtIndex(results, i);
                IntPtr candidates = ObjC.Send(observation, "topCandidates:", (IntPtr)1);
                IntPtr best = candidates != IntPtr.Zero && CoreFoundation.CFArrayGetCount(candidates) > 0 ? CoreFoundation.CFArrayGetValueAtIndex(candidates, 0) : IntPtr.Zero;
                string? text = best != IntPtr.Zero ? CoreFoundation.ToManagedString(ObjC.Send(best, "string")) : null;

                if (!string.IsNullOrEmpty(text))
                {
                    lines.Add(text);
                }
            }

            return string.Join(singleLine ? " " : "\n", lines);
        }
        finally
        {
            if (request != IntPtr.Zero) ObjC.Send(request, "release");
            if (handler != IntPtr.Zero) ObjC.Send(handler, "release");
            if (image != IntPtr.Zero) CoreFoundation.CFRelease(image);
            if (source != IntPtr.Zero) CoreFoundation.CFRelease(source);
            CoreFoundation.CFRelease(data);
        }
    }

    private static IReadOnlyList<string> ReadStrings(IntPtr array)
    {
        List<string> values = new List<string>();
        nint count = array != IntPtr.Zero ? CoreFoundation.CFArrayGetCount(array) : 0;

        for (nint i = 0; i < count; i++)
        {
            if (CoreFoundation.ToManagedString(CoreFoundation.CFArrayGetValueAtIndex(array, i)) is { Length: > 0 } value)
            {
                values.Add(value);
            }
        }

        return values;
    }

    private static string DisplayName(string tag)
    {
        try
        {
            return CultureInfo.GetCultureInfo(tag).DisplayName;
        }
        catch (CultureNotFoundException)
        {
            return tag;
        }
    }

    [LibraryImport(ImageIO)]
    private static partial IntPtr CGImageSourceCreateWithData(IntPtr data, IntPtr options);

    [LibraryImport(ImageIO)]
    private static partial IntPtr CGImageSourceCreateImageAtIndex(IntPtr source, nint index, IntPtr options);
}

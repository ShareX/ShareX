#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

using ShareX.HelpersLib;
using System.Drawing;
using System.Drawing.Imaging;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;

namespace ShareX.Tools;

public static class OCRHelper
{
    private const string SupportedVersion = "10.0.18362.0";

    public static bool IsSupported => Helpers.OSVersion >= new Version(SupportedVersion);

    public static OCRLanguageOption[] AvailableLanguages
    {
        get
        {
            ThrowIfNotSupported();
            return OcrEngine.AvailableRecognizerLanguages
                .Select(x => new OCRLanguageOption(x.DisplayName, x.LanguageTag))
                .ToArray();
        }
    }

    public static void ThrowIfNotSupported()
    {
        if (!IsSupported)
        {
            throw new InvalidOperationException(
                $"Optical character recognition is only available with Windows version {SupportedVersion} or newer.");
        }
    }

    public static async Task<string> OCR(Bitmap bitmap, string languageTag = "en", float scaleFactor = 1f,
        bool singleLine = false)
    {
        ThrowIfNotSupported();
        scaleFactor = Math.Max(scaleFactor, 1f);

        return await Task.Run(async () =>
        {
            using Bitmap scaledBitmap = ImageHelpers.ScaleImageFast(bitmap, scaleFactor);
            return await OCRInternal(scaledBitmap, languageTag, singleLine);
        });
    }

    private static async Task<string> OCRInternal(Bitmap bitmap, string languageTag, bool singleLine)
    {
        Language language = new(languageTag);
        if (!OcrEngine.IsLanguageSupported(language))
        {
            throw new InvalidOperationException($"{language.DisplayName} language is not available in this system for OCR.");
        }

        OcrEngine engine = OcrEngine.TryCreateFromLanguage(language);
        using InMemoryRandomAccessStream stream = new();
        bitmap.Save(stream.AsStream(), ImageFormat.Bmp);
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
        using SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
        OcrResult result = await engine.RecognizeAsync(softwareBitmap);

        IEnumerable<string> lines;
        if (language.LanguageTag.StartsWith("zh", StringComparison.OrdinalIgnoreCase) ||
            language.LanguageTag.StartsWith("ja", StringComparison.OrdinalIgnoreCase))
        {
            lines = result.Lines.Select(line => string.Concat(line.Words.Select(word => word.Text)));
        }
        else if (language.LayoutDirection == LanguageLayoutDirection.Rtl)
        {
            lines = result.Lines.Select(line => string.Join(" ", line.Words.Reverse().Select(word => word.Text)));
        }
        else
        {
            lines = result.Lines.Select(line => line.Text);
        }

        return string.Join(singleLine ? " " : Environment.NewLine, lines);
    }
}

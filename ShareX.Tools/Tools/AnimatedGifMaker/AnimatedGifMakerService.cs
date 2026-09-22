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

namespace ShareX.Tools;

public static class AnimatedGifMakerService
{
    private const int MaxPreviewDimension = 1200;

    public static IReadOnlyList<byte[]> CreatePreviewFrames(IReadOnlyList<string> imageFiles)
    {
        ValidateImageFiles(imageFiles);

        using Bitmap firstImage = LoadImage(imageFiles[0]);
        Size canvasSize = GetPreviewSize(firstImage.Size);
        List<byte[]> frames = new(imageFiles.Count);

        foreach (string imageFile in imageFiles)
        {
            using Bitmap source = LoadImage(imageFile);
            using Bitmap frame = ImageResizerService.Resize(source, canvasSize.Width, canvasSize.Height,
                ImageResizeMode.Fit);
            using MemoryStream stream = new();
            frame.Save(stream, ImageFormat.Png);
            frames.Add(stream.ToArray());
        }

        return frames;
    }

    public static void Create(string outputFilePath, IReadOnlyList<string> imageFiles, int delay,
        bool loop, int repeatCount)
    {
        ValidateImageFiles(imageFiles);
        ArgumentOutOfRangeException.ThrowIfLessThan(delay, 10);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(delay, 655350);
        ArgumentOutOfRangeException.ThrowIfNegative(repeatCount);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(repeatCount, ushort.MaxValue);

        string? outputFolder = Path.GetDirectoryName(outputFilePath);
        if (string.IsNullOrWhiteSpace(outputFolder))
        {
            throw new ArgumentException("An output folder is required.", nameof(outputFilePath));
        }

        Directory.CreateDirectory(outputFolder);
        string temporaryFilePath = Path.Combine(outputFolder,
            $".{Path.GetFileName(outputFilePath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            using Bitmap firstImage = LoadImage(imageFiles[0]);
            Size canvasSize = firstImage.Size;
            using (AnimatedGifCreator creator = new(temporaryFilePath, delay, repeatCount, loop))
            {
                AddFrame(creator, firstImage, canvasSize);

                foreach (string imageFile in imageFiles.Skip(1))
                {
                    using Bitmap source = LoadImage(imageFile);
                    AddFrame(creator, source, canvasSize);
                }
            }

            File.Move(temporaryFilePath, outputFilePath, true);
        }
        finally
        {
            if (File.Exists(temporaryFilePath))
            {
                File.Delete(temporaryFilePath);
            }
        }
    }

    private static void AddFrame(AnimatedGifCreator creator, Bitmap source, Size canvasSize)
    {
        using Bitmap frame = ImageResizerService.Resize(source, canvasSize.Width, canvasSize.Height,
            ImageResizeMode.Fit);
        creator.AddFrame(frame);
    }

    private static Bitmap LoadImage(string imageFile)
    {
        Bitmap? image = ImageHelpers.LoadImage(imageFile);
        return image ?? throw new InvalidDataException($"Unable to load image: {Path.GetFileName(imageFile)}");
    }

    private static void ValidateImageFiles(IReadOnlyList<string> imageFiles)
    {
        ArgumentNullException.ThrowIfNull(imageFiles);
        if (imageFiles.Count < 2)
        {
            throw new ArgumentException("At least two images are required.", nameof(imageFiles));
        }
    }

    private static Size GetPreviewSize(Size size)
    {
        int largestDimension = Math.Max(size.Width, size.Height);
        if (largestDimension <= MaxPreviewDimension)
        {
            return size;
        }

        double scale = MaxPreviewDimension / (double)largestDimension;
        return new Size(Math.Max(1, (int)Math.Round(size.Width * scale)),
            Math.Max(1, (int)Math.Round(size.Height * scale)));
    }
}

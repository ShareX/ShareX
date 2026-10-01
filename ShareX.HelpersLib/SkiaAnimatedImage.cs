using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace ShareX.HelpersLib;

/// <summary>Decodes animation frames together with the prior frames required by their disposal rules.</summary>
public sealed class SkiaAnimatedImage : IDisposable
{
    private readonly SKData data;
    private readonly SKCodec codec;
    public int FrameCount => Math.Max(1, codec.FrameCount);
    public int Width => codec.Info.Width;
    public int Height => codec.Info.Height;
    public SKEncodedImageFormat Format => codec.EncodedFormat;

    public SkiaAnimatedImage(string filePath)
    {
        data = SKData.CreateCopy(File.ReadAllBytes(filePath));
        codec = SKCodec.Create(data);
        if (codec == null)
        {
            data.Dispose();
            throw new InvalidDataException("The image format is not supported.");
        }
    }

    public SKBitmap GetFrame(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, FrameCount);
        SKBitmap bitmap = SkiaImageHelpers.CreateBitmap(Width, Height);
        try
        {
            SKCodecFrameInfo[] info = codec.FrameInfo;
            Stack<int> frames = new();
            int required = index;
            while (required >= 0)
            {
                frames.Push(required);
                required = info.Length > required ? info[required].RequiredFrame : -1;
            }
            int previous = -1;
            while (frames.Count > 0)
            {
                int frame = frames.Pop();
                SKCodecResult result = codec.GetPixels(bitmap.Info, bitmap.GetPixels(), new SKCodecOptions(frame, previous));
                if (result is not SKCodecResult.Success and not SKCodecResult.IncompleteInput)
                    throw new InvalidDataException($"Frame {frame} decoding failed: {result}.");
                previous = frame;
            }
            return bitmap;
        }
        catch
        {
            bitmap.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        codec.Dispose();
        data.Dispose();
    }
}

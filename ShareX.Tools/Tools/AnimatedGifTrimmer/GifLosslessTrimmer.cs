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

using ShareX.Tools.Localization;

namespace ShareX.Tools;

/// <summary>Copies GIF image, palette and control blocks without decoding or re-encoding their LZW data.</summary>
internal sealed class GifLosslessTrimmer
{
    private enum BlockKind { Extension, GraphicControl, Image }
    private readonly record struct Block(long Offset, long Length, BlockKind Kind, int FrameIndex);
    private readonly record struct Frame(int Delay, bool CoversScreen, bool MayContainTransparency, int Disposal);

    private readonly Block[] _blocks;
    private readonly Frame[] _frames;
    private readonly long _headerLength;

    public int FrameCount => _frames.Length;
    public int GetDelay(int index) => Math.Max(1, _frames[index].Delay);
    public bool Loop { get; }
    public int RepeatCount { get; }

    private GifLosslessTrimmer(long headerLength, List<Block> blocks, List<Frame> frames,
        bool loop, int repeatCount)
    {
        _headerLength = headerLength;
        _blocks = blocks.ToArray();
        _frames = frames.ToArray();
        Loop = loop;
        RepeatCount = repeatCount;
    }

    public bool CanCopySelection(int firstFrame, int endFrameExclusive)
    {
        if (firstFrame == 0) return true;
        Frame first = _frames[firstFrame];
        return endFrameExclusive == firstFrame + 1 || first.Disposal != 3;
    }

    public bool NeedsVisualValidation(int firstFrame) => firstFrame > 0 &&
        (!_frames[firstFrame].CoversScreen || _frames[firstFrame].MayContainTransparency);

    public static GifLosslessTrimmer Parse(string path, CancellationToken token)
    {
        using FileStream source = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        Span<byte> header = stackalloc byte[13];
        source.ReadExactly(header);
        if (!header[..6].SequenceEqual("GIF87a"u8) && !header[..6].SequenceEqual("GIF89a"u8))
            throw new InvalidDataException(Strings.AnimatedGifTrimmer_InvalidGif);

        int screenWidth = ReadWord(header, 6);
        int screenHeight = ReadWord(header, 8);
        if (screenWidth == 0 || screenHeight == 0)
            throw new InvalidDataException(Strings.AnimatedGifTrimmer_InvalidGif);
        if ((header[10] & 0x80) != 0)
            Skip(source, 3L << ((header[10] & 7) + 1));

        long headerLength = source.Position;
        List<Block> blocks = [];
        List<Frame> frames = [];
        int pendingControl = -1;
        int pendingDelay = 10;
        int pendingPacked = 0;
        bool loop = false;
        int repeatCount = 0;
        byte[] control = new byte[4];
        byte[] descriptor = new byte[9];

        while (true)
        {
            token.ThrowIfCancellationRequested();
            long offset = source.Position;
            switch (ReadByte(source))
            {
                case 0x21:
                    {
                        int label = ReadByte(source);
                        if (label == 0xF9)
                        {
                            if (pendingControl >= 0 || ReadByte(source) != 4)
                                throw new InvalidDataException(Strings.AnimatedGifTrimmer_InvalidGif);
                            source.ReadExactly(control);
                            if (ReadByte(source) != 0)
                                throw new InvalidDataException(Strings.AnimatedGifTrimmer_InvalidGif);
                            pendingControl = blocks.Count;
                            pendingPacked = control[0];
                            pendingDelay = ReadWord(control, 1);
                            blocks.Add(new Block(offset, source.Position - offset, BlockKind.GraphicControl, -1));
                        }
                        else
                        {
                            // Plain Text is a graphic-rendering block, so it may alter the canvas and consume a GCE.
                            if (label == 0x01) throw new InvalidDataException(Strings.AnimatedGifTrimmer_UnsupportedGif);
                            if (label == 0xFF && TryReadLoopExtension(source) is int repeat)
                            {
                                loop = true;
                                repeatCount = repeat;
                            }
                            SkipSubBlocks(source);
                            blocks.Add(new Block(offset, source.Position - offset, BlockKind.Extension, -1));
                        }
                        break;
                    }
                case 0x2C:
                    {
                        source.ReadExactly(descriptor);
                        int left = ReadWord(descriptor, 0), top = ReadWord(descriptor, 2);
                        int width = ReadWord(descriptor, 4), height = ReadWord(descriptor, 6);
                        if (width == 0 || height == 0 || left + width > screenWidth || top + height > screenHeight)
                            throw new InvalidDataException(Strings.AnimatedGifTrimmer_InvalidGif);
                        if ((descriptor[8] & 0x80) != 0)
                            Skip(source, 3L << ((descriptor[8] & 7) + 1));
                        ReadByte(source); // LZW minimum code size
                        SkipSubBlocks(source);

                        int frameIndex = frames.Count;
                        bool coversScreen = left == 0 && top == 0 && width == screenWidth && height == screenHeight;
                        frames.Add(new Frame(pendingDelay, coversScreen, (pendingPacked & 1) != 0,
                            (pendingPacked >> 2) & 7));
                        if (pendingControl >= 0)
                            blocks[pendingControl] = blocks[pendingControl] with { FrameIndex = frameIndex };
                        blocks.Add(new Block(offset, source.Position - offset, BlockKind.Image, frameIndex));
                        pendingControl = -1;
                        pendingDelay = 10;
                        pendingPacked = 0;
                        break;
                    }
                case 0x3B:
                    if (pendingControl >= 0 || frames.Count < 2)
                        throw new InvalidDataException(Strings.AnimatedGifTrimmer_InvalidGif);
                    return new GifLosslessTrimmer(headerLength, blocks, frames, loop, repeatCount);
                default:
                    throw new InvalidDataException(Strings.AnimatedGifTrimmer_InvalidGif);
            }
        }
    }

    private static int? TryReadLoopExtension(Stream source)
    {
        long start = source.Position;
        try
        {
            if (ReadByte(source) != 11) return null;
            Span<byte> identifier = stackalloc byte[11];
            source.ReadExactly(identifier);
            if (!identifier.SequenceEqual("NETSCAPE2.0"u8) && !identifier.SequenceEqual("ANIMEXTS1.0"u8)) return null;
            if (ReadByte(source) != 3) return null;
            Span<byte> data = stackalloc byte[3];
            source.ReadExactly(data);
            return data[0] == 1 ? ReadWord(data, 1) : null;
        }
        finally { source.Position = start; }
    }

    public void Copy(string inputPath, string outputPath, int firstFrame, int endFrameExclusive,
        IProgress<double>? progress, CancellationToken token)
    {
        if (!CanCopySelection(firstFrame, endFrameExclusive))
            throw new InvalidOperationException(Strings.AnimatedGifTrimmer_DependentFrame);

        using FileStream source = new(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using FileStream destination = new(outputPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        byte[] buffer = new byte[81920];
        CopyRange(source, destination, 0, _headerLength, buffer, token);
        for (int i = 0; i < _blocks.Length; i++)
        {
            Block block = _blocks[i];
            if (block.Kind != BlockKind.Extension &&
                (block.FrameIndex < firstFrame || block.FrameIndex >= endFrameExclusive)) continue;
            CopyRange(source, destination, block.Offset, block.Length, buffer, token);
            if (block.Kind == BlockKind.Image)
                progress?.Report((block.FrameIndex - firstFrame + 1d) / (endFrameExclusive - firstFrame) * 100);
        }
        token.ThrowIfCancellationRequested();
        destination.WriteByte(0x3B);
    }

    private static void CopyRange(Stream source, Stream destination, long offset, long length,
        byte[] buffer, CancellationToken token)
    {
        source.Position = offset;
        while (length > 0)
        {
            token.ThrowIfCancellationRequested();
            int read = source.Read(buffer, 0, (int)Math.Min(buffer.Length, length));
            if (read == 0) throw new EndOfStreamException();
            destination.Write(buffer, 0, read);
            length -= read;
        }
    }

    private static void SkipSubBlocks(Stream source)
    {
        int length;
        while ((length = ReadByte(source)) != 0) Skip(source, length);
    }

    private static void Skip(Stream source, long length)
    {
        if (length > source.Length - source.Position) throw new EndOfStreamException();
        source.Position += length;
    }

    private static int ReadByte(Stream source)
    {
        int value = source.ReadByte();
        if (value < 0) throw new EndOfStreamException();
        return value;
    }

    private static int ReadWord(ReadOnlySpan<byte> bytes, int offset) => bytes[offset] | bytes[offset + 1] << 8;
}

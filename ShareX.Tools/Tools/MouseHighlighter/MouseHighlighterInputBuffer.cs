#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using System.Drawing;

namespace ShareX.Tools;

internal enum MouseHighlightButton
{
    Primary,
    Secondary,
    Middle
}

internal readonly record struct MouseHighlighterButtonEvent(MouseHighlightButton Button, bool Pressed, Point Position, long Timestamp);

// Single producer (the hook thread), single consumer (the animation thread).
// The producer never waits for the consumer, allocates, or calls UI code.
internal sealed class MouseHighlighterInputBuffer
{
    public const int Capacity = 256;

    private readonly MouseHighlighterButtonEvent[] _events = new MouseHighlighterButtonEvent[Capacity];
    private int _readIndex;
    private int _writeIndex;
    private int _overflowed;
    private int _pressedButtons;
    private long _position;

    public Point Position
    {
        get
        {
            long position = Interlocked.Read(ref _position);
            return new Point((int)(position >> 32), (int)position);
        }
    }

    public int PressedButtons => Volatile.Read(ref _pressedButtons);

    public MouseHighlighterInputBuffer(Point position) => SetPosition(position);

    // Coalesce mouse moves instead of queueing every report from a high-rate mouse.
    public void SetPosition(Point position) => Interlocked.Exchange(ref _position, ((long)position.X << 32) | (uint)position.Y);

    public void PublishButton(MouseHighlighterButtonEvent input)
    {
        int button = 1 << (int)input.Button;
        Volatile.Write(ref _pressedButtons, input.Pressed ? _pressedButtons | button : _pressedButtons & ~button);

        int writeIndex = _writeIndex;
        int nextIndex = (writeIndex + 1) % Capacity;
        if (nextIndex == Volatile.Read(ref _readIndex))
        {
            // Retain current button state even if a UI stall fills the queue.
            // The consumer can then recover without leaving a button held forever.
            Interlocked.Exchange(ref _overflowed, 1);
            return;
        }
        _events[writeIndex] = input;
        Volatile.Write(ref _writeIndex, nextIndex);
    }

    public bool TryRead(out MouseHighlighterButtonEvent input)
    {
        int readIndex = _readIndex;
        if (readIndex == Volatile.Read(ref _writeIndex))
        {
            input = default;
            return false;
        }
        input = _events[readIndex];
        Volatile.Write(ref _readIndex, (readIndex + 1) % Capacity);
        return true;
    }

    public bool ConsumeOverflow() => Interlocked.Exchange(ref _overflowed, 0) != 0;

    public void DiscardPendingEvents() => Volatile.Write(ref _readIndex, Volatile.Read(ref _writeIndex));
}
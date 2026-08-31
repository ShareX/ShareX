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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace ShareX.ImageEditor.Presentation.Controls;

public enum SelectionResizeNodeKind
{
    TopLeft,
    TopCenter,
    TopRight,
    RightCenter,
    BottomRight,
    BottomCenter,
    BottomLeft,
    LeftCenter
}

public static class SelectionResizeNode
{
    public static IReadOnlyList<SelectionResizeNodeKind> RectangleNodes { get; } =
    [
        SelectionResizeNodeKind.TopLeft,
        SelectionResizeNodeKind.TopCenter,
        SelectionResizeNodeKind.TopRight,
        SelectionResizeNodeKind.RightCenter,
        SelectionResizeNodeKind.BottomRight,
        SelectionResizeNodeKind.BottomCenter,
        SelectionResizeNodeKind.BottomLeft,
        SelectionResizeNodeKind.LeftCenter
    ];

    public static Border Create(double x, double y, object tag, Cursor cursor, double coordinateOffset = 0)
    {
        Border node = new()
        {
            Width = 15,
            Height = 15,
            CornerRadius = new CornerRadius(10),
            Background = Brushes.White,
            Tag = tag,
            Cursor = cursor,
            // Keep node centers stable while dragging; layout rounding can cause
            // half-pixel positions to snap left/right on consecutive frames.
            UseLayoutRounding = false,
            BoxShadow = new BoxShadows(new BoxShadow
            {
                OffsetX = 0,
                OffsetY = 0,
                Blur = 8,
                Spread = 0,
                Color = Color.FromArgb(100, 0, 0, 0)
            })
        };
        node.SetValue(Panel.ZIndexProperty, 2);
        SetPosition(node, new Point(x, y), coordinateOffset);
        return node;
    }

    public static void SetPosition(Control node, Point position, double coordinateOffset = 0)
    {
        Canvas.SetLeft(node, position.X + coordinateOffset - node.Width / 2);
        Canvas.SetTop(node, position.Y + coordinateOffset - node.Height / 2);
    }

    public static Point GetPosition(Rect rectangle, SelectionResizeNodeKind node)
    {
        return node switch
        {
            SelectionResizeNodeKind.TopLeft => rectangle.TopLeft,
            SelectionResizeNodeKind.TopCenter => new Point(rectangle.Center.X, rectangle.Top),
            SelectionResizeNodeKind.TopRight => rectangle.TopRight,
            SelectionResizeNodeKind.RightCenter => new Point(rectangle.Right, rectangle.Center.Y),
            SelectionResizeNodeKind.BottomRight => rectangle.BottomRight,
            SelectionResizeNodeKind.BottomCenter => new Point(rectangle.Center.X, rectangle.Bottom),
            SelectionResizeNodeKind.BottomLeft => rectangle.BottomLeft,
            SelectionResizeNodeKind.LeftCenter => new Point(rectangle.Left, rectangle.Center.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(node))
        };
    }

    public static bool TryGetKind(object? tag, out SelectionResizeNodeKind node)
    {
        if (tag is SelectionResizeNodeKind resizeNode)
        {
            node = resizeNode;
            return true;
        }

        return Enum.TryParse(tag?.ToString(), out node);
    }

    public static bool TryGetDirection(string tag, out int horizontalDirection, out int verticalDirection)
    {
        horizontalDirection = tag.Contains("Left") ? -1 : tag.Contains("Right") ? 1 : 0;
        verticalDirection = tag.Contains("Top") ? -1 : tag.Contains("Bottom") ? 1 : 0;
        return horizontalDirection != 0 || verticalDirection != 0;
    }

    public static Rect Resize(Rect rectangle, SelectionResizeNodeKind node, Vector delta,
        double minimumWidth = 1, double minimumHeight = 1)
    {
        return Resize(rectangle, node.ToString(), delta, minimumWidth, minimumHeight);
    }

    public static Rect Resize(Rect rectangle, string nodeTag, Vector delta,
        double minimumWidth = 1, double minimumHeight = 1)
    {
        if (!TryGetDirection(nodeTag, out int horizontalDirection, out int verticalDirection))
        {
            return rectangle;
        }

        double left = rectangle.Left;
        double top = rectangle.Top;
        double width = rectangle.Width;
        double height = rectangle.Height;

        if (horizontalDirection > 0)
        {
            width = Math.Max(minimumWidth, width + delta.X);
        }
        else if (horizontalDirection < 0)
        {
            double change = Math.Min(width - minimumWidth, delta.X);
            left += change;
            width -= change;
        }

        if (verticalDirection > 0)
        {
            height = Math.Max(minimumHeight, height + delta.Y);
        }
        else if (verticalDirection < 0)
        {
            double change = Math.Min(height - minimumHeight, delta.Y);
            top += change;
            height -= change;
        }

        return new Rect(left, top, width, height);
    }
}

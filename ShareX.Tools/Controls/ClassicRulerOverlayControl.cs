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
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Rendering;
using ShareX.AvaloniaUI.Input;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System.Globalization;
using DrawingColor = System.Drawing.Color;
using DrawingPoint = System.Drawing.Point;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX.Tools.Controls;

/// <summary>
/// The ShareX 21 ruler (a freely drawn, adjustable rectangle over a frozen screenshot) drawn with the
/// ShareX 22 region capture visuals. All coordinates are physical pixels relative to the screenshot, so a
/// 1 px line is exactly one screen pixel. Cards, handles and the magnifier are sized in DIPs through
/// <see cref="HudScale"/>, like the region capture HUD.
/// </summary>
public sealed class ClassicRulerOverlayControl : Control, ICustomHitTest
{
    private enum Interaction
    {
        None,
        Pending,
        Creating,
        Moving,
        Resizing
    }

    [Flags]
    private enum Edges
    {
        None = 0,
        Left = 1,
        Top = 2,
        Right = 4,
        Bottom = 8
    }

    private const int MoveSpeedMinimum = 1;
    private const int MoveSpeedMaximum = 10;
    private const int DragThreshold = 4;

    // Region capture HUD metrics, in DIPs.
    private const double HandleSize = 15;
    private const double HandleShadowBlur = 8;
    private const double CardBorder = 1;
    private const double CardPaddingX = 7;
    private const double CardPaddingY = 4;
    private const double CardCornerRadius = 5;
    private const double CardFontSize = 12;
    private const double SelectionCardGap = 8;
    private const double PointerPanelOffset = 18;
    private const double PointerPanelSpacing = 10;

    private static readonly IBrush MagnifierGridBrush = new SolidColorBrush(Color.FromArgb(75, 0, 0, 0));
    private static readonly Color HandleShadowColor = Color.FromArgb(100, 0, 0, 0);

    private static readonly Edges[] HandleEdges =
    [
        Edges.Left | Edges.Top, Edges.Top, Edges.Right | Edges.Top, Edges.Right,
        Edges.Right | Edges.Bottom, Edges.Bottom, Edges.Left | Edges.Bottom, Edges.Left
    ];

    private Bitmap? _screenshot;
    private PixelPoint _origin;
    private IReadOnlyList<PixelRect> _monitors = [];
    private IReadOnlyList<DrawingRectangle> _windows = [];
    private ClassicRulerOptions _options = new();
    private int _width = 1;
    private int _height = 1;
    private double _hudScale = 1;
    private IBrush? _dimBrush;
    // Dashes and crosshair shadow use the accent's halo color (black or white), so a dark accent stays visible
    // over dark screen content.
    private Color _haloColor = Colors.Black;
    private IPen _antDashPen = CreateAntDashPen(Colors.Black);
    private IBrush _haloBrush = Brushes.Black;
    private Cursor _crosshairCursor = CursorAssetLoader.GetCrosshairCursor();
    private Cursor _openHandCursor = CursorAssetLoader.GetOpenHandCursor();
    private Cursor _closedHandCursor = CursorAssetLoader.GetClosedHandCursor();

    private DrawingRectangle? _selection;
    private DrawingRectangle? _hover;
    private Interaction _interaction;
    private Edges _resizeEdges;
    private PixelPoint _pressPoint;
    private PixelPoint _lastCreationPoint;
    private DrawingRectangle _startSelection;
    private PixelPoint? _pointer;
    private bool _movingDuringCreation;
    private bool _wasControlHeldDuringCreation;
    private bool _suppressRightRelease;

    public event EventHandler? CloseRequested;

    public ClassicRulerOverlayControl()
    {
        Focusable = true;
        Cursor = _crosshairCursor;
    }

    /// <summary>Scale applied to cards, handles, cursors and the magnifier. Set to the window render scaling.</summary>
    public double HudScale
    {
        get => _hudScale;
        set
        {
            _hudScale = double.IsFinite(value) && value > 0 ? value : 1;
            _crosshairCursor = CursorAssetLoader.GetCrosshairCursor(_hudScale);
            _openHandCursor = CursorAssetLoader.GetOpenHandCursor(_hudScale);
            _closedHandCursor = CursorAssetLoader.GetClosedHandCursor(_hudScale);
            Cursor = _crosshairCursor;
            InvalidateVisual();
        }
    }

    internal void Initialize(Bitmap screenshot, PixelPoint origin, IReadOnlyList<PixelRect> monitors, ClassicRulerOptions options)
    {
        _screenshot = screenshot;
        _origin = origin;
        _monitors = monitors;
        _options = options;
        _width = Math.Max(1, screenshot.PixelSize.Width);
        _height = Math.Max(1, screenshot.PixelSize.Height);
        Width = _width;
        Height = _height;

        int dimAlpha = (int)Math.Round(Math.Clamp(options.BackgroundDimStrength, 0, 100) / 100d * 255);
        _dimBrush = dimAlpha > 0 ? new SolidColorBrush(Color.FromArgb((byte)dimAlpha, 0, 0, 0)) : null;
        InvalidateVisual();
    }

    /// <summary>Sets detected window and control rectangles in screen coordinates, topmost first.</summary>
    internal void SetWindowRectangles(IReadOnlyList<DrawingRectangle> screenRectangles)
    {
        _windows = screenRectangles
            .Select(rectangle => new DrawingRectangle(rectangle.X - _origin.X, rectangle.Y - _origin.Y, rectangle.Width, rectangle.Height))
            .Where(rectangle => rectangle.Width > 0 && rectangle.Height > 0)
            .ToArray();
        UpdateHover();
        InvalidateVisual();
    }

    internal void SetPointer(PixelPoint point)
    {
        _pointer = Clamp(point);
        UpdateHover();
        InvalidateVisual();
    }

    public bool HitTest(Point point) => true;

    #region Input

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        PointerPoint pointer = e.GetCurrentPoint(this);
        PixelPoint point = ToPixel(pointer.Position);
        _pointer = point;

        if (pointer.Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
        {
            // Right click while drawing cancels the new rectangle instead of closing the ruler.
            if (_interaction is Interaction.Pending or Interaction.Creating)
            {
                _selection = null;
                EndInteraction(e.Pointer);
                _suppressRightRelease = true;
            }

            e.Handled = true;
            return;
        }

        if (pointer.Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonPressed)
        {
            return;
        }

        Focus();
        Edges edges = HitTestHandle(point);

        if (_selection is DrawingRectangle selection && edges != Edges.None)
        {
            _interaction = Interaction.Resizing;
            _resizeEdges = edges;
            _startSelection = selection;
        }
        else if (_selection is DrawingRectangle current && Contains(current, point))
        {
            _interaction = Interaction.Moving;
            _pressPoint = point;
            _startSelection = current;
        }
        else
        {
            // Like region capture: a click picks the hovered window, a drag draws a new rectangle.
            _interaction = Interaction.Pending;
            _selection = null;
            _pressPoint = point;
            _lastCreationPoint = point;
            _movingDuringCreation = false;
            _wasControlHeldDuringCreation = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        }

        Cursor = _interaction is Interaction.Moving or Interaction.Resizing ? _closedHandCursor : _crosshairCursor;
        e.Pointer.Capture(this);
        e.Handled = true;
        InvalidateVisual();
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        PixelPoint point = ToPixel(e.GetPosition(this));
        if (_pointer == point && _interaction == Interaction.None)
        {
            return;
        }

        _pointer = point;

        switch (_interaction)
        {
            case Interaction.Pending:
                if (Math.Abs(point.X - _pressPoint.X) >= DragThreshold || Math.Abs(point.Y - _pressPoint.Y) >= DragThreshold)
                {
                    _interaction = Interaction.Creating;
                    _hover = null;
                    UpdateSelectionDuringCreation(point, e.KeyModifiers);
                }
                break;
            case Interaction.Creating:
                UpdateSelectionDuringCreation(point, e.KeyModifiers);
                break;
            case Interaction.Moving:
                _selection = Move(_startSelection, point.X - _pressPoint.X, point.Y - _pressPoint.Y);
                break;
            case Interaction.Resizing:
                _selection = Resize(_startSelection, _resizeEdges, point);
                break;
            default:
                UpdateHover();
                UpdateCursor(point);
                break;
        }

        InvalidateVisual();
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        PixelPoint point = ToPixel(e.GetPosition(this));
        _pointer = point;

        if (e.InitialPressMouseButton == MouseButton.Right)
        {
            e.Handled = true;

            if (_suppressRightRelease)
            {
                _suppressRightRelease = false;
            }
            else if (_selection is DrawingRectangle selection && Contains(selection, point))
            {
                _selection = null;
                UpdateHover();
                UpdateCursor(point);
                InvalidateVisual();
            }
            else
            {
                CloseRequested?.Invoke(this, EventArgs.Empty);
            }

            return;
        }

        if (e.InitialPressMouseButton != MouseButton.Left || _interaction == Interaction.None)
        {
            return;
        }

        if (_interaction == Interaction.Pending)
        {
            _selection = _hover;
        }

        EndInteraction(e.Pointer);
        UpdateHover();
        UpdateCursor(point);
        e.Handled = true;
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);

        if (_interaction != Interaction.None)
        {
            _interaction = Interaction.None;
            Cursor = _crosshairCursor;
            InvalidateVisual();
        }
    }

    /// <summary>
    /// Arrow keys move the cursor while there is no rectangle, otherwise they move the rectangle.
    /// Ctrl resizes from the bottom-right corner, Alt from the top-left corner and Shift moves faster.
    /// </summary>
    internal bool HandleKeyDown(Key key, KeyModifiers modifiers)
    {
        int speed = modifiers.HasFlag(KeyModifiers.Shift) ? MoveSpeedMaximum : MoveSpeedMinimum;
        int dx = key == Key.Left ? -speed : key == Key.Right ? speed : 0;
        int dy = key == Key.Up ? -speed : key == Key.Down ? speed : 0;

        if (dx != 0 || dy != 0)
        {
            if (_selection is not DrawingRectangle selection || _interaction != Interaction.None)
            {
                DrawingPoint cursor = CaptureHelpers.GetCursorPosition();
                CaptureHelpers.SetCursorPosition(cursor.X + dx, cursor.Y + dy);
            }
            else if (modifiers.HasFlag(KeyModifiers.Control))
            {
                _selection = Clamp(new DrawingRectangle(selection.X, selection.Y,
                    Math.Max(1, selection.Width + dx), Math.Max(1, selection.Height + dy)));
            }
            else if (modifiers.HasFlag(KeyModifiers.Alt))
            {
                int left = Math.Min(selection.X + dx, selection.Right - 1);
                int top = Math.Min(selection.Y + dy, selection.Bottom - 1);
                _selection = Clamp(DrawingRectangle.FromLTRB(left, top, selection.Right, selection.Bottom));
            }
            else
            {
                _selection = Move(selection, dx, dy);
            }

            InvalidateVisual();
            return true;
        }

        if (key == Key.Delete && _interaction == Interaction.None && _selection != null)
        {
            _selection = null;
            UpdateHover();
            InvalidateVisual();
            return true;
        }

        return false;
    }

    /// <summary>Shift keeps the rectangle square; holding Ctrl moves it instead of resizing it, like region capture.</summary>
    private void UpdateSelectionDuringCreation(PixelPoint point, KeyModifiers modifiers)
    {
        bool controlHeld = modifiers.HasFlag(KeyModifiers.Control);

        if (!controlHeld)
        {
            _movingDuringCreation = false;
        }
        else if (_selection is DrawingRectangle selection && (_movingDuringCreation || !_wasControlHeldDuringCreation))
        {
            DrawingRectangle moved = Move(selection, point.X - _lastCreationPoint.X, point.Y - _lastCreationPoint.Y);
            _pressPoint = Clamp(new PixelPoint(_pressPoint.X + moved.X - selection.X, _pressPoint.Y + moved.Y - selection.Y));
            _selection = moved;
            _movingDuringCreation = true;
            _wasControlHeldDuringCreation = true;
            _lastCreationPoint = point;
            return;
        }

        _selection = CreateRectangle(_pressPoint, point, modifiers.HasFlag(KeyModifiers.Shift));
        _lastCreationPoint = point;
        _wasControlHeldDuringCreation = controlHeld;
    }

    private void EndInteraction(IPointer pointer)
    {
        _interaction = Interaction.None;
        _movingDuringCreation = false;
        pointer.Capture(null);
        Cursor = _crosshairCursor;
        InvalidateVisual();
    }

    /// <summary>Outlines the topmost detected window under the cursor while there is no rectangle.</summary>
    private void UpdateHover()
    {
        _hover = null;

        if (_selection != null || _pointer is not PixelPoint pointer ||
            _interaction is not (Interaction.None or Interaction.Pending))
        {
            return;
        }

        foreach (DrawingRectangle window in _windows)
        {
            if (Contains(window, pointer))
            {
                DrawingRectangle visible = DrawingRectangle.Intersect(window, new DrawingRectangle(0, 0, _width, _height));
                _hover = visible.Width > 0 && visible.Height > 0 ? visible : null;
                return;
            }
        }
    }

    private void UpdateCursor(PixelPoint point)
    {
        Cursor = HitTestHandle(point) != Edges.None ? _openHandCursor : _crosshairCursor;
    }

    private Edges HitTestHandle(PixelPoint point)
    {
        if (_selection is not DrawingRectangle selection || _interaction is Interaction.Creating or Interaction.Pending)
        {
            return Edges.None;
        }

        double radius = HandleSize * _hudScale / 2;

        foreach (Edges edges in HandleEdges)
        {
            Point center = GetHandleCenter(selection, edges);
            double deltaX = point.X + 0.5 - center.X;
            double deltaY = point.Y + 0.5 - center.Y;
            if (deltaX * deltaX + deltaY * deltaY <= radius * radius)
            {
                return edges;
            }
        }

        return Edges.None;
    }

    #endregion Input

    #region Rendering

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (_screenshot == null)
        {
            return;
        }

        IBrush accent = FindThemeResource<IBrush>("ShareX.Brush.Accent.Start") ?? Brushes.DodgerBlue;
        UpdateHalo(accent);
        DrawingRectangle? selection = _selection is DrawingRectangle current && current.Width > 0 && current.Height > 0
            ? current
            : null;
        DrawingRectangle? active = selection ?? _hover;

        DrawDim(context, selection ?? (_options.DimAllExceptSelection ? null : _hover));

        if (active is DrawingRectangle rectangle)
        {
            DrawAntRectangle(context, rectangle, accent);

            if (selection != null)
            {
                DrawTicks(context, rectangle, 5, 10, accent);
                DrawTicks(context, rectangle, 15, 100, accent);

                if (_options.ShowCenterCrosshair)
                {
                    DrawCenterCrosshair(context, rectangle, accent);
                }

                if (_interaction is not (Interaction.Creating or Interaction.Pending))
                {
                    DrawHandles(context, rectangle);
                }
            }

            if (_options.ShowInfo)
            {
                DrawSelectionCard(context, rectangle);
            }
        }

        if (_pointer is PixelPoint pointer)
        {
            if (_options.ShowScreenCrosshair)
            {
                DrawScreenCrosshair(context, pointer, accent);
            }

            if (_options.ShowMagnifier || _options.ShowInfo)
            {
                DrawPointerPanel(context, pointer, accent);
            }
        }
    }

    private void DrawDim(DrawingContext context, DrawingRectangle? clear)
    {
        if (_dimBrush == null)
        {
            return;
        }

        if (clear is not DrawingRectangle s)
        {
            context.FillRectangle(_dimBrush, new Rect(0, 0, _width, _height));
            return;
        }

        FillIfValid(context, _dimBrush, new Rect(0, 0, _width, s.Top));
        FillIfValid(context, _dimBrush, new Rect(0, s.Bottom, _width, _height - s.Bottom));
        FillIfValid(context, _dimBrush, new Rect(0, s.Top, s.Left, s.Height));
        FillIfValid(context, _dimBrush, new Rect(s.Right, s.Top, _width - s.Right, s.Height));
    }

    private static void FillIfValid(DrawingContext context, IBrush brush, Rect rectangle)
    {
        if (rectangle.Width > 0 && rectangle.Height > 0)
        {
            context.FillRectangle(brush, rectangle);
        }
    }

    private void UpdateHalo(IBrush accent)
    {
        Color halo = AccentPalette.GetHaloColor(accent);
        if (halo != _haloColor)
        {
            _haloColor = halo;
            _antDashPen = CreateAntDashPen(halo);
            _haloBrush = new SolidColorBrush(halo);
        }
    }

    private static IPen CreateAntDashPen(Color color) =>
        new Pen(new SolidColorBrush(Color.FromArgb(230, color.R, color.G, color.B)), 1, new DashStyle([5, 5], 0));

    /// <summary>Region capture selection outline: a 1 px accent line with halo colored dashes on top.</summary>
    private void DrawAntRectangle(DrawingContext context, DrawingRectangle rectangle, IBrush accent)
    {
        Rect line = new(rectangle.X + 0.5, rectangle.Y + 0.5, Math.Max(0, rectangle.Width - 1), Math.Max(0, rectangle.Height - 1));
        context.DrawRectangle(null, new Pen(accent, 1), line);
        context.DrawLine(_antDashPen, line.TopLeft, line.TopRight);
        context.DrawLine(_antDashPen, line.TopLeft, line.BottomLeft);
        context.DrawLine(_antDashPen, line.TopRight, line.BottomRight);
        context.DrawLine(_antDashPen, line.BottomLeft, line.BottomRight);
    }

    private void DrawAntLine(DrawingContext context, Point start, Point end, IBrush accent)
    {
        if (end.X <= start.X && end.Y <= start.Y)
        {
            return;
        }

        context.DrawLine(new Pen(accent, 1), start, end);
        context.DrawLine(_antDashPen, start, end);
    }

    /// <summary>Ruler ticks along every edge: short ones every 10 px and long ones every 100 px.</summary>
    private static void DrawTicks(DrawingContext context, DrawingRectangle selection, int size, int step, IBrush accent)
    {
        if (selection.Width < size || selection.Height < size)
        {
            return;
        }

        for (int i = 1; i * step < selection.Width; i++)
        {
            int x = selection.X + i * step;
            context.FillRectangle(accent, new Rect(x, selection.Top, 1, size));
            context.FillRectangle(accent, new Rect(x, selection.Bottom - size, 1, size));
        }

        for (int i = 1; i * step < selection.Height; i++)
        {
            int y = selection.Y + i * step;
            context.FillRectangle(accent, new Rect(selection.Left, y, size, 1));
            context.FillRectangle(accent, new Rect(selection.Right - size, y, size, 1));
        }
    }

    private void DrawCenterCrosshair(DrawingContext context, DrawingRectangle rectangle, IBrush accent)
    {
        int centerX = rectangle.X + rectangle.Width / 2;
        int centerY = rectangle.Y + rectangle.Height / 2;
        DrawPixelCross(context, _haloBrush, centerX - 1, centerY - 1);
        DrawPixelCross(context, accent, centerX, centerY);
    }

    private static void DrawPixelCross(DrawingContext context, IBrush brush, int centerX, int centerY)
    {
        const int radius = 10;
        const int diameter = radius * 2 + 1;
        context.FillRectangle(brush, new Rect(centerX - radius, centerY, diameter, 1));
        context.FillRectangle(brush, new Rect(centerX, centerY - radius, 1, diameter));
    }

    private void DrawScreenCrosshair(DrawingContext context, PixelPoint pointer, IBrush accent)
    {
        const double cursorGap = 5;
        double x = pointer.X + 0.5;
        double y = pointer.Y + 0.5;
        DrawAntLine(context, new Point(0, y), new Point(x - cursorGap, y), accent);
        DrawAntLine(context, new Point(x + cursorGap, y), new Point(_width, y), accent);
        DrawAntLine(context, new Point(x, 0), new Point(x, y - cursorGap), accent);
        DrawAntLine(context, new Point(x, y + cursorGap), new Point(x, _height), accent);
    }

    /// <summary>Same look as the region capture resize nodes: a white dot with a soft shadow.</summary>
    private void DrawHandles(DrawingContext context, DrawingRectangle selection)
    {
        double size = HandleSize * _hudScale;
        BoxShadows shadow = new(new BoxShadow { Blur = HandleShadowBlur * _hudScale, Color = HandleShadowColor });

        foreach (Edges edges in HandleEdges)
        {
            Point center = GetHandleCenter(selection, edges);
            Rect handle = new(center.X - size / 2, center.Y - size / 2, size, size);
            context.DrawRectangle(Brushes.White, null, new RoundedRect(handle, size / 2), shadow);
        }
    }

    private static Point GetHandleCenter(DrawingRectangle selection, Edges edges)
    {
        double x = edges.HasFlag(Edges.Left) ? selection.Left
            : edges.HasFlag(Edges.Right) ? selection.Right
            : selection.Left + selection.Width / 2d;
        double y = edges.HasFlag(Edges.Top) ? selection.Top
            : edges.HasFlag(Edges.Bottom) ? selection.Bottom
            : selection.Top + selection.Height / 2d;
        return new Point(x, y);
    }

    /// <summary>Measurement card, placed below the rectangle like the region capture size card, or above it when there is no room.</summary>
    private void DrawSelectionCard(DrawingContext context, DrawingRectangle rectangle)
    {
        FormattedText text = CreateText(GetSelectionText(rectangle));
        Size size = GetCardSize(text);
        Rect area = GetMonitorArea(new PixelPoint(rectangle.Left, Math.Max(rectangle.Top, rectangle.Bottom - 1)));
        double y = rectangle.Bottom + SelectionCardGap * _hudScale;

        if (y + size.Height > area.Bottom)
        {
            area = GetMonitorArea(new PixelPoint(rectangle.Left, rectangle.Top));
            y = rectangle.Top - size.Height;
        }

        double x = Math.Clamp(rectangle.Left, area.Left, Math.Max(area.Left, area.Right - size.Width));
        y = Math.Clamp(y, area.Top, Math.Max(area.Top, area.Bottom - size.Height));
        DrawCard(context, text, new Point(Math.Round(x), Math.Round(y)));
    }

    /// <summary>Magnifier with the pointer card under it, offset from the cursor like the region capture HUD.</summary>
    private void DrawPointerPanel(DrawingContext context, PixelPoint pointer, IBrush accent)
    {
        int pixelCount = GetMagnifierPixelCount();
        int cellSize = Math.Max(1, (int)Math.Round(Math.Clamp(_options.MagnifierSize,
            ClassicRulerOptions.MagnifierSizeMinimum, ClassicRulerOptions.MagnifierSizeMaximum) * _hudScale) / pixelCount);
        int magnifierSize = cellSize * pixelCount;
        double ring = Math.Max(1, Math.Round(_hudScale));
        double magnifierOuter = _options.ShowMagnifier ? magnifierSize + 2 * ring : 0;

        FormattedText? infoText = _options.ShowInfo ? CreateText(GetPointerText(pointer)) : null;
        Size infoSize = infoText != null ? GetCardSize(infoText) : default;
        double gap = magnifierOuter > 0 && infoText != null ? PointerPanelSpacing * _hudScale : 0;
        double width = Math.Max(magnifierOuter, infoSize.Width);
        double height = magnifierOuter + gap + infoSize.Height;

        Rect area = GetMonitorArea(pointer);
        double offset = PointerPanelOffset * _hudScale;
        double x = pointer.X + offset;
        double y = pointer.Y + offset;

        if (x + width > area.Right)
        {
            x = pointer.X - width - offset;
        }

        if (y + height > area.Bottom)
        {
            y = pointer.Y - height - offset;
        }

        x = Math.Round(Math.Clamp(x, area.Left, Math.Max(area.Left, area.Right - width)));
        y = Math.Round(Math.Clamp(y, area.Top, Math.Max(area.Top, area.Bottom - height)));

        if (magnifierOuter > 0)
        {
            Point location = new(x + Math.Round((width - magnifierOuter) / 2) + ring, y + ring);
            DrawMagnifier(context, pointer, location, pixelCount, cellSize, ring, accent);
        }

        if (infoText != null)
        {
            DrawCard(context, infoText, new Point(Math.Round(x + (width - infoSize.Width) / 2), Math.Round(y + magnifierOuter + gap)));
        }
    }

    /// <summary>Region capture magnifier: white outer ring, black inner ring, accent crosshair and a pixel grid.</summary>
    private void DrawMagnifier(DrawingContext context, PixelPoint pointer, Point location, int pixelCount, int cellSize,
        double ring, IBrush accent)
    {
        int size = pixelCount * cellSize;
        Rect bounds = new(location, new Size(size, size));
        Rect outer = bounds.Inflate(ring);
        bool square = _options.UseSquareMagnifier;

        context.DrawRectangle(Brushes.White, null, square ? new RoundedRect(outer) : new RoundedRect(outer, outer.Width / 2));

        int radius = pixelCount / 2;
        int sourceX = pointer.X - radius;
        int sourceY = pointer.Y - radius;

        using (context.PushClip(square ? new RoundedRect(bounds) : new RoundedRect(bounds, size / 2d)))
        {
            context.FillRectangle(Brushes.Black, bounds);

            int left = Math.Max(0, sourceX);
            int top = Math.Max(0, sourceY);
            int right = Math.Min(_width, sourceX + pixelCount);
            int bottom = Math.Min(_height, sourceY + pixelCount);

            if (_screenshot != null && right > left && bottom > top)
            {
                Rect source = new(left, top, right - left, bottom - top);
                Rect destination = new(
                    bounds.X + (left - sourceX) * cellSize,
                    bounds.Y + (top - sourceY) * cellSize,
                    (right - left) * cellSize,
                    (bottom - top) * cellSize);

                using (context.PushRenderOptions(new RenderOptions { BitmapInterpolationMode = BitmapInterpolationMode.None }))
                {
                    context.DrawImage(_screenshot, source, destination);
                }
            }

            double center = radius * cellSize;
            double far = center + cellSize;

            using (context.PushOpacity(75d / byte.MaxValue))
            {
                context.FillRectangle(accent, new Rect(bounds.X, bounds.Y + center, center, cellSize - 1));
                context.FillRectangle(accent, new Rect(bounds.X + far, bounds.Y + center, size - far, cellSize - 1));
                context.FillRectangle(accent, new Rect(bounds.X + center, bounds.Y, cellSize - 1, center));
                context.FillRectangle(accent, new Rect(bounds.X + center, bounds.Y + far, cellSize - 1, size - far));
            }

            for (int i = 1; i < pixelCount; i++)
            {
                double line = i * cellSize - 1;
                context.FillRectangle(MagnifierGridBrush, new Rect(bounds.X + line, bounds.Y, 1, size));
                context.FillRectangle(MagnifierGridBrush, new Rect(bounds.X, bounds.Y + line, size, 1));
            }

            // Grid lines sit on the pixel just before each cell, so the center outline starts there too.
            Rect cell = new(bounds.X + center - 1, bounds.Y + center - 1, cellSize + 1, cellSize + 1);
            DrawPixelOutline(context, Brushes.Black, cell);
            DrawPixelOutline(context, Brushes.White, cell.Deflate(1));
        }

        double inner = Math.Max(1, Math.Round(_hudScale));
        IPen innerPen = new Pen(Brushes.Black, inner);
        if (square)
        {
            context.DrawRectangle(null, innerPen, bounds.Deflate(inner / 2));
        }
        else
        {
            context.DrawEllipse(null, innerPen, bounds.Center, size / 2d - inner / 2, size / 2d - inner / 2);
        }
    }

    private static void DrawPixelOutline(DrawingContext context, IBrush brush, Rect rectangle)
    {
        if (rectangle.Width < 2 || rectangle.Height < 2)
        {
            return;
        }

        context.FillRectangle(brush, new Rect(rectangle.X, rectangle.Y, rectangle.Width, 1));
        context.FillRectangle(brush, new Rect(rectangle.X, rectangle.Bottom - 1, rectangle.Width, 1));
        context.FillRectangle(brush, new Rect(rectangle.X, rectangle.Y + 1, 1, rectangle.Height - 2));
        context.FillRectangle(brush, new Rect(rectangle.Right - 1, rectangle.Y + 1, 1, rectangle.Height - 2));
    }

    private int GetMagnifierPixelCount()
    {
        int count = Math.Clamp(_options.MagnifierPixelCount,
            ClassicRulerOptions.MagnifierPixelCountMinimum, ClassicRulerOptions.MagnifierPixelCountMaximum);
        return (count & 1) == 0 ? count - 1 : count;
    }

    // Cards match the region capture pointer and size panels: themed background, border and text,
    // 1 px border, 7x4 padding, 5 corner radius and 12 px semibold text, all in DIPs.
    private FormattedText CreateText(string text)
    {
        FontFamily fontFamily = FindThemeResource<FontFamily>("ShareX.FontFamily.Default") ?? FontFamily.Default;
        return new FormattedText(
            text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface(fontFamily, FontStyle.Normal, FontWeight.SemiBold),
            CardFontSize * _hudScale,
            FindThemeResource<IBrush>("ShareX.Brush.Text") ?? Brushes.White);
    }

    private Size GetCardSize(FormattedText text) => new(
        Math.Ceiling(text.WidthIncludingTrailingWhitespace + 2 * (CardPaddingX + CardBorder) * _hudScale),
        Math.Ceiling(text.Height + 2 * (CardPaddingY + CardBorder) * _hudScale));

    private void DrawCard(DrawingContext context, FormattedText text, Point location)
    {
        Rect box = new(location, GetCardSize(text));
        double border = CardBorder * _hudScale;
        IBrush background = FindThemeResource<IBrush>("ShareX.Brush.Background.Main") ?? Brushes.Black;
        IBrush borderBrush = FindThemeResource<IBrush>("ShareX.Brush.Border") ?? Brushes.Gray;

        context.DrawRectangle(background, new Pen(borderBrush, border),
            new RoundedRect(box.Deflate(border / 2), CardCornerRadius * _hudScale - border / 2));
        context.DrawText(text, new Point(
            box.X + (CardPaddingX + CardBorder) * _hudScale,
            box.Y + (CardPaddingY + CardBorder) * _hudScale));
    }

    private T? FindThemeResource<T>(string key) where T : class =>
        this.TryFindResource(key, ActualThemeVariant, out object? value) ? value as T : null;

    #endregion Rendering

    #region Measurement text

    public string GetMeasurementText()
    {
        if ((_selection ?? _hover) is DrawingRectangle rectangle)
        {
            return GetSelectionText(rectangle);
        }

        return _pointer is PixelPoint pointer ? GetPointerText(pointer) : string.Empty;
    }

    private string GetPointerText(PixelPoint pointer)
    {
        DrawingPoint position = new(_origin.X + pointer.X, _origin.Y + pointer.Y);

        if (_options.UseCustomInfoText && TryGetPixel(pointer, out DrawingColor color))
        {
            if (!string.IsNullOrEmpty(_options.CustomInfoText))
            {
                return CodeMenuEntryPixelInfo.Parse(_options.CustomInfoText, color, position);
            }

            return $"RGB: {color.R}, {color.G}, {color.B}{Environment.NewLine}" +
                $"Hex: {ColorHelpers.ColorToHex(color)}{Environment.NewLine}" +
                $"X: {position.X} Y: {position.Y}";
        }

        return $"X: {position.X} Y: {position.Y}";
    }

    private unsafe bool TryGetPixel(PixelPoint point, out DrawingColor color)
    {
        color = default;
        if (_screenshot is not WriteableBitmap bitmap)
        {
            return false;
        }

        using ILockedFramebuffer buffer = bitmap.Lock();
        byte* pixel = (byte*)buffer.Address + point.Y * buffer.RowBytes + point.X * 4;
        color = DrawingColor.FromArgb(255, pixel[2], pixel[1], pixel[0]);
        return true;
    }

    private string GetSelectionText(DrawingRectangle selection)
    {
        int x = _origin.X + selection.X;
        int y = _origin.Y + selection.Y;
        int right = x + selection.Width - 1;
        int bottom = y + selection.Height - 1;
        int deltaX = right - x;
        int deltaY = bottom - y;
        double distance = Math.Sqrt((double)deltaX * deltaX + (double)deltaY * deltaY);
        double angle = Math.Atan2(deltaY, deltaX) * 180 / Math.PI;
        long area = (long)selection.Width * selection.Height;
        long perimeter = 2L * (selection.Width + selection.Height);

        return string.Format(CultureInfo.CurrentCulture, Localization.Strings.ClassicRulerOverlayControl_Position, x, y, right, bottom) + Environment.NewLine +
            string.Format(CultureInfo.CurrentCulture, Localization.Strings.ClassicRulerOverlayControl_Size, selection.Width, selection.Height, area, perimeter) + Environment.NewLine +
            string.Format(CultureInfo.CurrentCulture, Localization.Strings.ClassicRulerOverlayControl_Distance, distance, angle);
    }

    #endregion Measurement text

    #region Geometry

    // Pointer positions are whole physical pixels but pick up floating point noise from the DPI transform,
    // so round them instead of flooring (same as region capture).
    private PixelPoint ToPixel(Point point) => Clamp(new PixelPoint((int)Math.Round(point.X), (int)Math.Round(point.Y)));

    private PixelPoint Clamp(PixelPoint point) => new(
        Math.Clamp(point.X, 0, _width - 1),
        Math.Clamp(point.Y, 0, _height - 1));

    private static bool Contains(DrawingRectangle rectangle, PixelPoint point) => rectangle.Contains(point.X, point.Y);

    private DrawingRectangle CreateRectangle(PixelPoint start, PixelPoint end, bool square)
    {
        int deltaX = end.X - start.X;
        int deltaY = end.Y - start.Y;

        if (square)
        {
            // Keep the square on screen in both drawing directions, like region capture.
            int availableWidth = deltaX < 0 ? start.X : _width - 1 - start.X;
            int availableHeight = deltaY < 0 ? start.Y : _height - 1 - start.Y;
            int side = Math.Min(Math.Max(Math.Abs(deltaX), Math.Abs(deltaY)), Math.Min(availableWidth, availableHeight));
            deltaX = deltaX < 0 ? -side : side;
            deltaY = deltaY < 0 ? -side : side;
        }

        int left = Math.Min(start.X, start.X + deltaX);
        int top = Math.Min(start.Y, start.Y + deltaY);
        return new DrawingRectangle(left, top, Math.Abs(deltaX) + 1, Math.Abs(deltaY) + 1);
    }

    private DrawingRectangle Move(DrawingRectangle rectangle, int dx, int dy)
    {
        int width = Math.Min(rectangle.Width, _width);
        int height = Math.Min(rectangle.Height, _height);
        return new DrawingRectangle(
            Math.Clamp(rectangle.X + dx, 0, _width - width),
            Math.Clamp(rectangle.Y + dy, 0, _height - height),
            width,
            height);
    }

    private DrawingRectangle Resize(DrawingRectangle source, Edges edges, PixelPoint point)
    {
        int left = source.Left;
        int top = source.Top;
        int right = source.Right - 1;
        int bottom = source.Bottom - 1;

        if (edges.HasFlag(Edges.Left)) left = point.X;
        if (edges.HasFlag(Edges.Right)) right = point.X;
        if (edges.HasFlag(Edges.Top)) top = point.Y;
        if (edges.HasFlag(Edges.Bottom)) bottom = point.Y;

        return Clamp(CreateRectangle(new PixelPoint(left, top), new PixelPoint(right, bottom), false));
    }

    private DrawingRectangle Clamp(DrawingRectangle rectangle)
    {
        int left = Math.Clamp(rectangle.Left, 0, _width - 1);
        int top = Math.Clamp(rectangle.Top, 0, _height - 1);
        int right = Math.Clamp(rectangle.Right, left + 1, _width);
        int bottom = Math.Clamp(rectangle.Bottom, top + 1, _height);
        return DrawingRectangle.FromLTRB(left, top, right, bottom);
    }

    /// <summary>Bounds of the monitor containing the point, so cards never land between monitors.</summary>
    private Rect GetMonitorArea(PixelPoint point)
    {
        foreach (PixelRect monitor in _monitors)
        {
            if (monitor.Contains(point))
            {
                return new Rect(monitor.X, monitor.Y, monitor.Width, monitor.Height);
            }
        }

        return new Rect(0, 0, _width, _height);
    }

    #endregion Geometry
}

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

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ShareX.AvaloniaUI.Input;
using ShareX.Platform;
using System.Collections.Concurrent;

namespace ShareX.AvaloniaUI.Windows;

/// <summary>
/// Hosts an unshown Avalonia <see cref="Window"/> above compositor panels and top bars by rendering its visual tree
/// into a reusable premultiplied BGRA8888 framebuffer and forwarding pointer, wheel, and keyboard input from an
/// <see cref="IPlatformOverlayWindow"/> via public Avalonia routed input APIs.
/// </summary>
public sealed class PlatformOverlayWindowHost : IDisposable
{
    private const double DoubleClickRadiusDip = 4.0;
    private const ulong DoubleClickTimeMs = 500;
    private static readonly ConcurrentDictionary<Type, bool> CustomRenderCache = new();
    private static readonly HashSet<Type> StandardContainerRenderTypes =
    [
        typeof(Visual),
        typeof(Control),
        typeof(Decorator),
        typeof(Panel),
        typeof(Border),
        typeof(ContentPresenter),
        typeof(TemplatedControl),
        typeof(ContentControl),
        typeof(ItemsControl),
        typeof(ItemsPresenter),
        typeof(VisualLayerManager),
        typeof(LayoutTransformControl),
        typeof(Viewbox)
    ];

    private readonly Window _window;
    private readonly IPlatformOverlayWindow _host;
    private readonly Action? _onCompositorClosed;
    private readonly Pointer _pointer;
    private readonly Style _popupOverlayStyle;
    private readonly DispatcherTimer _caretTimer;
    private readonly DispatcherTimer _keepAliveTimer;
    private DispatcherTimer? _keyRepeatTimer;

    private RenderTargetBitmap? _renderTarget;
    private WriteableBitmap? _stagingBitmap;
    private byte[]? _directReadbackBuffer;

    private bool _started;
    private bool _disposed;
    private bool _frameDirty;
    private bool _frameScheduled;
    private bool _isRendering;
    private bool _throttledRenderQueued;
    private readonly System.Diagnostics.Stopwatch _clock = System.Diagnostics.Stopwatch.StartNew();
    private TimeSpan _lastRenderStart = TimeSpan.FromHours(-1);
    private TimeSpan _lastRenderCost = TimeSpan.Zero;

    // The newest pointer move that has not reached the UI thread yet. A mouse reports hundreds of moves a second and
    // only the last one matters, so moves overwrite each other here instead of each getting a queue entry.
    private readonly object _pendingMoveLock = new();
    private OverlayPointerEvent? _pendingMove;
    private bool _moveDrainQueued;
    private int _moveGeneration;
    private DateTime _keepAliveUntilUtc;

    private IInputElement? _pointerOverElement;
    private AvaloniaObject? _cursorOwner;
    private OverlayCursor? _lastSentCursor;

    private Point _lastPointerPosition;
    private ulong _lastClickTimestamp;
    private Point _lastClickPoint;
    private MouseButton _lastClickButton;
    private MouseButton _initialPressButton;
    private int _clickCount;

    private int _keyRepeatRate;
    private int _keyRepeatDelay;
    private Key _repeatKey;
    private PhysicalKey _repeatPhysicalKey;
    private KeyModifiers _repeatModifiers;
    private string? _repeatKeySymbol;

    public PlatformOverlayWindowHost(
        Window window,
        IPlatformOverlayWindow host,
        int pixelWidth,
        int pixelHeight,
        Action? onCompositorClosed = null)
    {
        _window = window ?? throw new ArgumentNullException(nameof(window));
        _host = host ?? throw new ArgumentNullException(nameof(host));
        _onCompositorClosed = onCompositorClosed;

        PixelWidth = Math.Max(1, pixelWidth);
        PixelHeight = Math.Max(1, pixelHeight);
        RenderScaling = double.IsFinite(host.RenderScaling) && host.RenderScaling > 0 ? host.RenderScaling : 1.0;
        LogicalSize = new Size(PixelWidth / RenderScaling, PixelHeight / RenderScaling);

        _pointer = new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, isPrimary: true);
        _popupOverlayStyle = new Style(x => x.OfType<Popup>())
        {
            Setters = { new Setter(Popup.ShouldUseOverlayLayerProperty, true) }
        };
        _caretTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(250), DispatcherPriority.Background, (_, _) =>
        {
            if (!_disposed && _window.FocusManager?.GetFocusedElement() is TextBox)
            {
                RequestRender();
            }
        });
        _caretTimer.Stop();
        _keepAliveTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(16), DispatcherPriority.Render, OnKeepAliveTick);
        _keepAliveTimer.Stop();
    }

    public double RenderScaling { get; }

    public Size LogicalSize { get; }

    public int PixelWidth { get; }

    public int PixelHeight { get; }

    public IReadOnlyList<OverlayOutputInfo> Outputs => _host.Outputs;

    public static IPlatformOverlayWindow? TryCreateOverlay(
        PlatformRectangle screenBounds,
        int pixelWidth,
        int pixelHeight,
        bool transparent,
        string overlayNamespace)
    {
        if (!PlatformServices.IsInitialized)
        {
            return null;
        }

        return PlatformServices.Current.Windows.TryCreateOverlayWindow(
            new OverlayWindowOptions(screenBounds, pixelWidth, pixelHeight, transparent, overlayNamespace));
    }

    public double? GetOutputScalingAt(PixelPoint desktopPoint)
    {
        PlatformPoint pt = new(desktopPoint.X, desktopPoint.Y);
        IReadOnlyList<OverlayOutputInfo> outputs = _host.Outputs;

        for (int i = 0; i < outputs.Count; i++)
        {
            OverlayOutputInfo output = outputs[i];
            if (output.LogicalBounds.Contains(pt) && double.IsFinite(output.ScaleFactor) && output.ScaleFactor > 0)
            {
                return output.ScaleFactor;
            }
        }

        return null;
    }

    public void Start()
    {
        if (_disposed || _started)
        {
            return;
        }

        _started = true;

        if (!_window.Styles.Contains(_popupOverlayStyle))
        {
            _window.Styles.Add(_popupOverlayStyle);
        }

        _window.LayoutUpdated += OnWindowLayoutUpdated;
        _host.PointerEvent += OnHostPointerEvent;
        _host.WheelEvent += OnHostWheelEvent;
        _host.KeyEvent += OnHostKeyEvent;
        _host.KeyRepeatInfoChanged += OnHostKeyRepeatInfoChanged;
        _host.ClosedByCompositor += OnHostClosedByCompositor;

        // Render and commit the initial frame before Show() maps the native surfaces so there is no blank flash.
        RenderNow();
        _host.Show();
        _caretTimer.Start();
    }

    public void RequestRender()
    {
        if (_disposed)
        {
            return;
        }

        _frameDirty = true;
        _keepAliveUntilUtc = DateTime.UtcNow.AddMilliseconds(350);
        ScheduleAnimationFrame();
    }

    public void RenderNow()
    {
        if (_disposed || _isRendering)
        {
            return;
        }

        _isRendering = true;
        _lastRenderStart = _clock.Elapsed;
        try
        {
            EnsureLayout();
            EnsureRenderTarget();

            _renderTarget!.Render(_window);
            PresentRenderTarget();

            SyncCursor();
            _frameDirty = false;
        }
        finally
        {
            _isRendering = false;
            _lastRenderCost = _clock.Elapsed - _lastRenderStart;
        }
    }

    /// <summary>
    /// Renders unless the last frame was too recent. A frame renders and reads back the whole desktop, which can take longer
    /// than the 16 ms between frames; rendering again straight away would leave the UI thread no time for input, so the
    /// gap is at least as long as the last frame took, and the rest of the time goes to the mouse and keyboard.
    /// </summary>
    private void RenderThrottled()
    {
        if (_disposed)
        {
            return;
        }

        TimeSpan gap = TimeSpan.FromTicks(Math.Max(TimeSpan.FromMilliseconds(16).Ticks, Math.Min(_lastRenderCost.Ticks * 2, TimeSpan.FromMilliseconds(100).Ticks)));
        TimeSpan wait = _lastRenderStart + gap - _clock.Elapsed;
        if (wait <= TimeSpan.Zero)
        {
            RenderNow();
            return;
        }

        if (_throttledRenderQueued)
        {
            return;
        }

        _throttledRenderQueued = true;
        DispatcherTimer.RunOnce(() =>
        {
            _throttledRenderQueued = false;
            if (!_disposed && _frameDirty)
            {
                RenderThrottled();
            }
        }, wait, DispatcherPriority.Background);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _caretTimer.Stop();
        _keepAliveTimer.Stop();
        StopKeyRepeat();

        if (_started)
        {
            _window.LayoutUpdated -= OnWindowLayoutUpdated;
            _host.PointerEvent -= OnHostPointerEvent;
            _host.WheelEvent -= OnHostWheelEvent;
            _host.KeyEvent -= OnHostKeyEvent;
            _host.KeyRepeatInfoChanged -= OnHostKeyRepeatInfoChanged;
            _host.ClosedByCompositor -= OnHostClosedByCompositor;
        }

        SetCursorOwner(null);
        _pointer.Dispose();

        _stagingBitmap?.Dispose();
        _stagingBitmap = null;
        _renderTarget?.Dispose();
        _renderTarget = null;
        _directReadbackBuffer = null;

        _host.Dispose();
    }

    private void ScheduleAnimationFrame()
    {
        if (_frameScheduled || _disposed)
        {
            return;
        }

        _frameScheduled = true;
        Dispatcher.UIThread.Post(OnScheduledRender, DispatcherPriority.Render);
    }

    private void OnScheduledRender()
    {
        _frameScheduled = false;
        if (_disposed)
        {
            return;
        }

        if (_frameDirty)
        {
            RenderThrottled();
        }

        if (!_disposed && DateTime.UtcNow < _keepAliveUntilUtc && !_keepAliveTimer.IsEnabled)
        {
            _keepAliveTimer.Start();
        }
    }

    private void OnKeepAliveTick(object? sender, EventArgs e)
    {
        if (_disposed || DateTime.UtcNow >= _keepAliveUntilUtc)
        {
            _keepAliveTimer.Stop();
            return;
        }

        _frameDirty = true;
        RenderThrottled();
    }

    private void OnWindowLayoutUpdated(object? sender, EventArgs e)
    {
        if (!_disposed && !_isRendering)
        {
            RequestRender();
        }
    }

    private void EnsureLayout()
    {
        Control root = (_window.GetVisualParent() as Control) ?? _window;
        if (!root.IsMeasureValid || !root.IsArrangeValid || root.Bounds.Size != LogicalSize)
        {
            root.Measure(LogicalSize);
            root.Arrange(new Rect(LogicalSize));
        }

        root.UpdateLayout();
    }

    private void EnsureRenderTarget()
    {
        if (_renderTarget != null)
        {
            return;
        }

        PixelSize pixelSize = new(PixelWidth, PixelHeight);
        Vector dpi = new(96.0 * RenderScaling, 96.0 * RenderScaling);
        _renderTarget = new RenderTargetBitmap(pixelSize, dpi);
    }

    private unsafe void PresentRenderTarget()
    {
        RenderTargetBitmap target = _renderTarget!;
        if (target.Format == PixelFormat.Bgra8888)
        {
            int stride = checked(PixelWidth * 4);
            int byteLength = checked(stride * PixelHeight);
            if (_directReadbackBuffer == null || _directReadbackBuffer.Length < byteLength)
            {
                _directReadbackBuffer = new byte[byteLength];
            }

            fixed (byte* ptr = _directReadbackBuffer)
            {
                target.CopyPixels(new PixelRect(0, 0, PixelWidth, PixelHeight), (IntPtr)ptr, byteLength, stride);
                _host.PresentFrame((IntPtr)ptr, PixelWidth, PixelHeight, stride);
            }

            return;
        }

        _stagingBitmap ??= new WriteableBitmap(
            new PixelSize(PixelWidth, PixelHeight),
            new Vector(96.0 * RenderScaling, 96.0 * RenderScaling),
            PixelFormat.Bgra8888,
            AlphaFormat.Premul);

        using ILockedFramebuffer fb = _stagingBitmap.Lock();
        target.CopyPixels(fb);
        _host.PresentFrame(fb.Address, fb.Size.Width, fb.Size.Height, fb.RowBytes);
    }

    private void OnHostPointerEvent(OverlayPointerEvent ev)
    {
        if (ev.Kind != OverlayPointerEventKind.Move)
        {
            // A button or leave event goes after any move that came before it, so the selection ends where the user let go.
            OverlayPointerEvent? earlier = TakePendingMove();
            PostToUi(() =>
            {
                if (earlier.HasValue)
                {
                    HandlePointerEvent(earlier.Value);
                }

                HandlePointerEvent(ev);
            });
            return;
        }

        int generation = 0;
        bool post;
        lock (_pendingMoveLock)
        {
            _pendingMove = ev;
            post = !_moveDrainQueued;
            if (post)
            {
                _moveDrainQueued = true;
                generation = _moveGeneration;
            }
        }

        if (post)
        {
            PostToUi(() => DrainPendingMove(generation));
        }
    }

    // Takes the move that is waiting and starts a new generation, so a drain queued before this call does not pick up a move
    // that arrives after it and handle it ahead of the event the caller is about to post.
    private OverlayPointerEvent? TakePendingMove()
    {
        lock (_pendingMoveLock)
        {
            OverlayPointerEvent? move = _pendingMove;
            _pendingMove = null;
            _moveDrainQueued = false;
            _moveGeneration++;
            return move;
        }
    }

    private void DrainPendingMove(int generation)
    {
        OverlayPointerEvent? move;
        lock (_pendingMoveLock)
        {
            if (generation != _moveGeneration)
            {
                return;
            }

            move = _pendingMove;
            _pendingMove = null;
            _moveDrainQueued = false;
        }

        if (move.HasValue)
        {
            HandlePointerEvent(move.Value);
        }
    }

    private void OnHostWheelEvent(OverlayWheelEvent ev)
    {
        OverlayPointerEvent? earlier = TakePendingMove();
        PostToUi(() =>
        {
            if (earlier.HasValue)
            {
                HandlePointerEvent(earlier.Value);
            }

            HandleWheelEvent(ev);
        });
    }

    private void OnHostKeyEvent(OverlayKeyEvent ev)
    {
        OverlayPointerEvent? earlier = TakePendingMove();
        PostToUi(() =>
        {
            if (earlier.HasValue)
            {
                HandlePointerEvent(earlier.Value);
            }

            HandleKeyEvent(ev);
        });
    }

    private void OnHostKeyRepeatInfoChanged(int rate, int delay) => PostToUi(() =>
    {
        _keyRepeatRate = rate;
        _keyRepeatDelay = delay;
    });

    private void OnHostClosedByCompositor() => PostToUi(() =>
    {
        if (!_disposed)
        {
            _onCompositorClosed?.Invoke();
        }
    });

    private void PostToUi(Action action)
    {
        if (_disposed)
        {
            return;
        }

        if (Dispatcher.UIThread.CheckAccess())
        {
            if (!_disposed)
            {
                action();
            }
        }
        else
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (!_disposed)
                {
                    action();
                }
            }, DispatcherPriority.Input);
        }
    }

    private Point ToClientPoint(double logicalX, double logicalY)
    {
        PlatformRectangle bounds = _host.LogicalBounds;
        double sx = bounds.Width > 0 ? LogicalSize.Width / bounds.Width : 1.0;
        double sy = bounds.Height > 0 ? LogicalSize.Height / bounds.Height : 1.0;
        return new Point(logicalX * sx, logicalY * sy);
    }

    private Visual GetRootVisual() => (_window.GetVisualParent() as Visual) ?? _window;

    private IInputElement? HitTestEnabled(Visual rootVisual, Point pos)
    {
        return FirstEnabledAncestor(HitTestVisualTree(rootVisual, rootVisual, pos));
    }

    private static IInputElement? HitTestVisualTree(Visual root, Visual current, Point rootPoint)
    {
        if (!current.IsVisible || current is IInputElement { IsHitTestVisible: false })
        {
            return null;
        }

        Point localPoint;
        if (ReferenceEquals(current, root))
        {
            localPoint = rootPoint;
        }
        else
        {
            Matrix? toRoot = current.TransformToVisual(root);
            if (toRoot == null || !toRoot.Value.TryInvert(out Matrix fromRoot))
            {
                return null;
            }

            localPoint = rootPoint * fromRoot;
        }

        Rect localBounds = new(current.Bounds.Size);
        if (current.ClipToBounds && !localBounds.Contains(localPoint))
        {
            return null;
        }

        if (current.Clip is Geometry clip && !clip.FillContains(localPoint))
        {
            return null;
        }

        IEnumerable<Visual> visualChildren = current.GetVisualChildren();
        IReadOnlyList<Visual> children = visualChildren as IReadOnlyList<Visual> ?? visualChildren.ToList();
        int count = children.Count;
        if (count > 0)
        {
            bool hasCustomZIndex = false;
            for (int i = 0; i < count; i++)
            {
                if (children[i].ZIndex != 0)
                {
                    hasCustomZIndex = true;
                    break;
                }
            }

            if (!hasCustomZIndex)
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    IInputElement? hit = HitTestVisualTree(root, children[i], rootPoint);
                    if (hit != null)
                    {
                        return hit;
                    }
                }
            }
            else
            {
                List<Visual> sorted = children.OrderBy(static c => c.ZIndex).ToList();
                for (int i = sorted.Count - 1; i >= 0; i--)
                {
                    IInputElement? hit = HitTestVisualTree(root, sorted[i], rootPoint);
                    if (hit != null)
                    {
                        return hit;
                    }
                }
            }
        }

        if (current is not IInputElement inputElement)
        {
            return null;
        }

        if (localBounds.Width <= 0 || localBounds.Height <= 0 || !localBounds.Contains(localPoint))
        {
            return null;
        }

        return IsVisualHitTestableAt(current, localPoint) ? inputElement : null;
    }

    private static bool IsVisualHitTestableAt(Visual visual, Point localPoint)
    {
        if (visual is ICustomHitTest customHit)
        {
            return customHit.HitTest(localPoint);
        }

        switch (visual)
        {
            case Panel panel:
                return panel.Background != null;
            case Border border:
                return border.Background != null ||
                    (border.BorderBrush != null && border.BorderThickness != default);
            case ContentPresenter presenter:
                return presenter.Background != null ||
                    (presenter.BorderBrush != null && presenter.BorderThickness != default);
            case Shape shape:
                return (shape.Fill != null && (shape.DefiningGeometry?.FillContains(localPoint) ?? true)) ||
                    (shape.Stroke != null && shape.StrokeThickness > 0 &&
                     (shape.DefiningGeometry?.StrokeContains(new Pen(shape.Stroke, shape.StrokeThickness), localPoint) ?? true));
            case TextBlock tb:
                return tb.Background != null || !string.IsNullOrEmpty(tb.Text) || tb.Inlines is { Count: > 0 };
            case TextPresenter:
                return true;
            case Image img:
                return img.Source != null;
            case TemplatedControl tc:
                return tc.Background != null ||
                    (tc.BorderBrush != null && tc.BorderThickness != default);
            default:
                return HasCustomRenderOverride(visual.GetType());
        }
    }

    private static bool HasCustomRenderOverride(Type type)
    {
        return CustomRenderCache.GetOrAdd(type, static t =>
        {
            System.Reflection.MethodInfo? renderMethod = t.GetMethod(
                nameof(Visual.Render),
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public,
                [typeof(DrawingContext)]);
            Type? declaring = renderMethod?.DeclaringType;
            return declaring != null && !StandardContainerRenderTypes.Contains(declaring);
        });
    }

    private void HandlePointerEvent(OverlayPointerEvent ev)
    {
        EnsureLayout();

        Point pos = ToClientPoint(ev.LogicalX, ev.LogicalY);
        _lastPointerPosition = pos;

        Visual rootVisual = GetRootVisual();
        PointerUpdateKind updateKind = MapUpdateKind(ev.Kind);
        PointerPointProperties props = CreatePointProperties(ev.Modifiers, updateKind);
        KeyModifiers keyMods = ToKeyModifiers(ev.Modifiers);

        if (ev.Kind == OverlayPointerEventKind.Leave)
        {
            ClearPointerOver(rootVisual, pos, ev.Timestamp, props, keyMods);
            SyncCursor();
            RequestRender();
            return;
        }

        IInputElement? hit = HitTestEnabled(rootVisual, pos);
        IInputElement? effectiveHover = _pointer.Captured is { } captured && hit != captured ? null : hit;
        UpdatePointerOver(rootVisual, effectiveHover, pos, ev.Timestamp, props, keyMods);

        switch (ev.Kind)
        {
            case OverlayPointerEventKind.LeftDown:
            case OverlayPointerEventKind.RightDown:
            case OverlayPointerEventKind.MiddleDown:
            case OverlayPointerEventKind.X1Down:
            case OverlayPointerEventKind.X2Down:
            {
                if (CountPressedButtons(props) > 1)
                {
                    RaisePointerMove(rootVisual, hit, pos, ev.Timestamp, props, keyMods);
                }
                else
                {
                    IInputElement? source = _pointer.Captured ?? hit;
                    if (source != null)
                    {
                        _pointer.Capture(source);
                        MouseButton button = GetMouseButton(updateKind);
                        UpdateClickCount(pos, ev.Timestamp, button);
                        _initialPressButton = button;

                        PointerPressedEventArgs args = new(
                            source,
                            _pointer,
                            rootVisual,
                            pos,
                            ev.Timestamp,
                            props,
                            keyMods,
                            _clickCount);
                        source.RaiseEvent(args);
                    }
                }
                break;
            }

            case OverlayPointerEventKind.LeftUp:
            case OverlayPointerEventKind.RightUp:
            case OverlayPointerEventKind.MiddleUp:
            case OverlayPointerEventKind.X1Up:
            case OverlayPointerEventKind.X2Up:
            {
                if (CountPressedButtons(props) != 0)
                {
                    RaisePointerMove(rootVisual, hit, pos, ev.Timestamp, props, keyMods);
                }
                else
                {
                    IInputElement? source = _pointer.Captured ?? hit;
                    if (source != null)
                    {
                        PointerReleasedEventArgs args = new(
                            source,
                            _pointer,
                            rootVisual,
                            pos,
                            ev.Timestamp,
                            props,
                            keyMods,
                            _initialPressButton);
                        try
                        {
                            source.RaiseEvent(args);
                        }
                        finally
                        {
                            _pointer.Capture(null);
                            _initialPressButton = MouseButton.None;
                        }
                    }
                    else
                    {
                        _pointer.Capture(null);
                        _initialPressButton = MouseButton.None;
                    }
                }
                break;
            }

            case OverlayPointerEventKind.Move:
                RaisePointerMove(rootVisual, hit, pos, ev.Timestamp, props, keyMods);
                break;
        }

        SyncCursor();
        RequestRender();
    }

    private void RaisePointerMove(
        Visual rootVisual,
        IInputElement? hit,
        Point pos,
        ulong timestamp,
        PointerPointProperties props,
        KeyModifiers keyMods)
    {
        IInputElement? source = _pointer.Captured ?? hit;
        if (source == null)
        {
            return;
        }

        PointerEventArgs args = new(
            InputElement.PointerMovedEvent,
            source,
            _pointer,
            rootVisual,
            pos,
            timestamp,
            props,
            keyMods);
        source.RaiseEvent(args);
    }

    private void HandleWheelEvent(OverlayWheelEvent ev)
    {
        EnsureLayout();

        Point pos = ToClientPoint(ev.LogicalX, ev.LogicalY);
        _lastPointerPosition = pos;

        Visual rootVisual = GetRootVisual();
        IInputElement? hit = HitTestEnabled(rootVisual, pos);
        IInputElement? source = _pointer.Captured ?? hit;
        if (source == null)
        {
            return;
        }

        PointerPointProperties props = CreatePointProperties(ev.Modifiers, PointerUpdateKind.Other);
        KeyModifiers keyMods = ToKeyModifiers(ev.Modifiers);
        PointerWheelEventArgs args = new(
            source,
            _pointer,
            rootVisual,
            pos,
            ev.Timestamp,
            props,
            keyMods,
            new Vector(ev.DeltaX, ev.DeltaY));
        source.RaiseEvent(args);
        RequestRender();
    }

    private void HandleKeyEvent(OverlayKeyEvent ev)
    {
        (Key key, PhysicalKey physicalKey) = OverlayKeyMap.Resolve(ev.EvdevKeycode, ev.Keysym);
        KeyModifiers keyMods = ToKeyModifiers(ev.Modifiers);

        if (ev.IsDown)
        {
            DispatchKeyDown(key, physicalKey, keyMods, ev.Text);
            StartKeyRepeat(key, physicalKey, keyMods, ev.Text);
        }
        else
        {
            if (key == _repeatKey)
            {
                StopKeyRepeat();
            }

            DispatchKeyUp(key, physicalKey, keyMods, ev.Text);
        }

        RequestRender();
    }

    private void DispatchKeyDown(Key key, PhysicalKey physicalKey, KeyModifiers keyMods, string? keySymbol)
    {
        IInputElement target = _window.FocusManager?.GetFocusedElement() ?? _window;
        KeyEventArgs args = new()
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Source = target,
            Key = key,
            PhysicalKey = physicalKey,
            KeyModifiers = keyMods,
            KeySymbol = keySymbol,
            KeyDeviceType = KeyDeviceType.Keyboard
        };

        Visual? current = target as Visual;
        while (current != null && !args.Handled)
        {
            if (current is IInputElement { KeyBindings: { Count: > 0 } bindings })
            {
                KeyBinding[]? snapshot = null;
                foreach (KeyBinding binding in bindings)
                {
                    if (binding.Gesture?.Matches(args) == true)
                    {
                        snapshot = bindings.ToArray();
                        break;
                    }
                }

                if (snapshot != null)
                {
                    foreach (KeyBinding binding in snapshot)
                    {
                        if (args.Handled)
                        {
                            break;
                        }

                        binding.TryHandle(args);
                    }
                }
            }

            current = current.GetVisualParent();
        }

        if (!args.Handled)
        {
            target.RaiseEvent(args);
        }

        if (!args.Handled && !string.IsNullOrEmpty(keySymbol))
        {
            TextInputEventArgs textArgs = new()
            {
                RoutedEvent = InputElement.TextInputEvent,
                Source = target,
                Text = keySymbol
            };
            target.RaiseEvent(textArgs);
        }
    }

    private void DispatchKeyUp(Key key, PhysicalKey physicalKey, KeyModifiers keyMods, string? keySymbol)
    {
        IInputElement target = _window.FocusManager?.GetFocusedElement() ?? _window;
        KeyEventArgs args = new()
        {
            RoutedEvent = InputElement.KeyUpEvent,
            Source = target,
            Key = key,
            PhysicalKey = physicalKey,
            KeyModifiers = keyMods,
            KeySymbol = keySymbol,
            KeyDeviceType = KeyDeviceType.Keyboard
        };
        target.RaiseEvent(args);
    }

    private void StartKeyRepeat(Key key, PhysicalKey physicalKey, KeyModifiers modifiers, string? keySymbol)
    {
        StopKeyRepeat();

        if (_keyRepeatRate <= 0 || IsModifierKey(key))
        {
            return;
        }

        _repeatKey = key;
        _repeatPhysicalKey = physicalKey;
        _repeatModifiers = modifiers;
        _repeatKeySymbol = keySymbol;

        int initialDelay = _keyRepeatDelay > 0 ? _keyRepeatDelay : 400;
        int intervalMs = Math.Max(15, (int)Math.Round(1000.0 / _keyRepeatRate));

        _keyRepeatTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(initialDelay)
        };
        _keyRepeatTimer.Tick += (_, _) =>
        {
            if (_disposed || _keyRepeatTimer == null)
            {
                StopKeyRepeat();
                return;
            }

            _keyRepeatTimer.Interval = TimeSpan.FromMilliseconds(intervalMs);
            DispatchKeyDown(_repeatKey, _repeatPhysicalKey, _repeatModifiers, _repeatKeySymbol);
            RequestRender();
        };
        _keyRepeatTimer.Start();
    }

    private void StopKeyRepeat()
    {
        if (_keyRepeatTimer != null)
        {
            _keyRepeatTimer.Stop();
            _keyRepeatTimer = null;
        }
    }

    private static bool IsModifierKey(Key key) => key is
        Key.LeftShift or Key.RightShift or
        Key.LeftCtrl or Key.RightCtrl or
        Key.LeftAlt or Key.RightAlt or
        Key.LWin or Key.RWin;

    private void UpdatePointerOver(
        Visual rootVisual,
        IInputElement? nextElement,
        Point pos,
        ulong timestamp,
        PointerPointProperties props,
        KeyModifiers keyMods)
    {
        if (ReferenceEquals(_pointerOverElement, nextElement))
        {
            return;
        }

        if (nextElement == null)
        {
            ClearPointerOver(rootVisual, pos, timestamp, props, keyMods);
            return;
        }

        IInputElement? branch = FindCommonVisualAncestor(nextElement, _pointerOverElement);
        IInputElement? prev = _pointerOverElement;

        if (prev != null)
        {
            PointerEventArgs exitArgs = new(
                InputElement.PointerExitedEvent,
                prev,
                _pointer,
                rootVisual,
                pos,
                timestamp,
                props,
                keyMods);

            while (prev != null && !ReferenceEquals(prev, branch))
            {
                exitArgs.Source = prev;
                exitArgs.Handled = false;
                prev.RaiseEvent(exitArgs);
                prev = GetVisualParentInputElement(prev);
            }
        }

        _pointerOverElement = nextElement;

        PointerEventArgs enterArgs = new(
            InputElement.PointerEnteredEvent,
            nextElement,
            _pointer,
            rootVisual,
            pos,
            timestamp,
            props,
            keyMods);

        IInputElement? cur = nextElement;
        while (cur != null && !ReferenceEquals(cur, branch))
        {
            enterArgs.Source = cur;
            enterArgs.Handled = false;
            cur.RaiseEvent(enterArgs);
            cur = GetVisualParentInputElement(cur);
        }
    }

    private void ClearPointerOver(
        Visual rootVisual,
        Point pos,
        ulong timestamp,
        PointerPointProperties props,
        KeyModifiers keyMods)
    {
        IInputElement? el = _pointerOverElement;
        if (el == null)
        {
            return;
        }

        PointerEventArgs exitArgs = new(
            InputElement.PointerExitedEvent,
            el,
            _pointer,
            null,
            default,
            timestamp,
            props,
            keyMods);

        while (el != null)
        {
            exitArgs.Source = el;
            exitArgs.Handled = false;
            el.RaiseEvent(exitArgs);
            el = GetVisualParentInputElement(el);
        }

        _pointerOverElement = null;
        _ = rootVisual;
        _ = pos;
    }

    private static IInputElement? FindCommonVisualAncestor(IInputElement first, IInputElement? second)
    {
        if (first is not Visual v1 || second is not Visual v2)
        {
            return null;
        }

        HashSet<IInputElement> ancestors = [];
        for (Visual? v = v2; v != null; v = v.GetVisualParent())
        {
            if (v is IInputElement ie)
            {
                ancestors.Add(ie);
            }
        }

        for (Visual? v = v1; v != null; v = v.GetVisualParent())
        {
            if (v is IInputElement ie && ancestors.Contains(ie))
            {
                return ie;
            }
        }

        return null;
    }

    private static IInputElement? GetVisualParentInputElement(IInputElement element) =>
        (element as Visual)?.GetVisualParent() as IInputElement;

    private void SyncCursor()
    {
        if (_disposed)
        {
            return;
        }

        IInputElement? owner = _pointer.Captured ?? _pointerOverElement ?? _window;
        (IInputElement? cursorSource, Cursor? effectiveCursor) = ResolveEffectiveCursor(owner);
        SetCursorOwner(cursorSource as AvaloniaObject);

        OverlayCursor next = CursorAssetLoader.ToOverlayCursor(effectiveCursor);
        if (_lastSentCursor != null && CursorsEqual(_lastSentCursor, next))
        {
            return;
        }

        _lastSentCursor = next;
        _host.SetCursor(next);
    }

    private static (IInputElement? Owner, Cursor? Cursor) ResolveEffectiveCursor(IInputElement? start)
    {
        Visual? visual = start as Visual;
        while (visual != null)
        {
            if (visual is IInputElement { Cursor: { } cursor } element)
            {
                return (element, cursor);
            }

            visual = visual.GetVisualParent();
        }

        return (null, null);
    }

    private void SetCursorOwner(AvaloniaObject? owner)
    {
        if (ReferenceEquals(_cursorOwner, owner))
        {
            return;
        }

        if (_cursorOwner != null)
        {
            _cursorOwner.PropertyChanged -= OnCursorOwnerPropertyChanged;
        }

        _cursorOwner = owner;

        if (_cursorOwner != null)
        {
            _cursorOwner.PropertyChanged += OnCursorOwnerPropertyChanged;
        }
    }

    private void OnCursorOwnerPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (!_disposed && e.Property == InputElement.CursorProperty)
        {
            SyncCursor();
        }
    }

    private static bool CursorsEqual(OverlayCursor a, OverlayCursor b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if (a.IsBitmap || b.IsBitmap)
        {
            return false;
        }

        return a.Shape == b.Shape;
    }

    private void UpdateClickCount(Point pos, ulong timestamp, MouseButton button)
    {
        double dx = Math.Abs(pos.X - _lastClickPoint.X);
        double dy = Math.Abs(pos.Y - _lastClickPoint.Y);
        if (button != _lastClickButton ||
            dx > DoubleClickRadiusDip ||
            dy > DoubleClickRadiusDip ||
            timestamp - _lastClickTimestamp > DoubleClickTimeMs)
        {
            _clickCount = 0;
        }

        _clickCount++;
        _lastClickButton = button;
        _lastClickPoint = pos;
        _lastClickTimestamp = timestamp;
    }

    private static IInputElement? FirstEnabledAncestor(IInputElement? candidate)
    {
        while (candidate != null && !candidate.IsEffectivelyEnabled)
        {
            candidate = (candidate as Visual)?.GetVisualParent() as IInputElement;
        }

        return candidate;
    }

    private static PointerUpdateKind MapUpdateKind(OverlayPointerEventKind kind) => kind switch
    {
        OverlayPointerEventKind.LeftDown => PointerUpdateKind.LeftButtonPressed,
        OverlayPointerEventKind.LeftUp => PointerUpdateKind.LeftButtonReleased,
        OverlayPointerEventKind.RightDown => PointerUpdateKind.RightButtonPressed,
        OverlayPointerEventKind.RightUp => PointerUpdateKind.RightButtonReleased,
        OverlayPointerEventKind.MiddleDown => PointerUpdateKind.MiddleButtonPressed,
        OverlayPointerEventKind.MiddleUp => PointerUpdateKind.MiddleButtonReleased,
        OverlayPointerEventKind.X1Down => PointerUpdateKind.XButton1Pressed,
        OverlayPointerEventKind.X1Up => PointerUpdateKind.XButton1Released,
        OverlayPointerEventKind.X2Down => PointerUpdateKind.XButton2Pressed,
        OverlayPointerEventKind.X2Up => PointerUpdateKind.XButton2Released,
        _ => PointerUpdateKind.Other
    };

    private static MouseButton GetMouseButton(PointerUpdateKind kind) => kind switch
    {
        PointerUpdateKind.LeftButtonPressed or PointerUpdateKind.LeftButtonReleased => MouseButton.Left,
        PointerUpdateKind.RightButtonPressed or PointerUpdateKind.RightButtonReleased => MouseButton.Right,
        PointerUpdateKind.MiddleButtonPressed or PointerUpdateKind.MiddleButtonReleased => MouseButton.Middle,
        PointerUpdateKind.XButton1Pressed or PointerUpdateKind.XButton1Released => MouseButton.XButton1,
        PointerUpdateKind.XButton2Pressed or PointerUpdateKind.XButton2Released => MouseButton.XButton2,
        _ => MouseButton.None
    };

    private static KeyModifiers ToKeyModifiers(OverlayInputModifiers modifiers)
    {
        KeyModifiers result = KeyModifiers.None;
        if (modifiers.HasFlag(OverlayInputModifiers.Alt)) result |= KeyModifiers.Alt;
        if (modifiers.HasFlag(OverlayInputModifiers.Control)) result |= KeyModifiers.Control;
        if (modifiers.HasFlag(OverlayInputModifiers.Shift)) result |= KeyModifiers.Shift;
        if (modifiers.HasFlag(OverlayInputModifiers.Meta)) result |= KeyModifiers.Meta;
        return result;
    }

    private static PointerPointProperties CreatePointProperties(OverlayInputModifiers modifiers, PointerUpdateKind updateKind)
    {
        RawInputModifiers raw = RawInputModifiers.None;
        if (modifiers.HasFlag(OverlayInputModifiers.Alt)) raw |= RawInputModifiers.Alt;
        if (modifiers.HasFlag(OverlayInputModifiers.Control)) raw |= RawInputModifiers.Control;
        if (modifiers.HasFlag(OverlayInputModifiers.Shift)) raw |= RawInputModifiers.Shift;
        if (modifiers.HasFlag(OverlayInputModifiers.Meta)) raw |= RawInputModifiers.Meta;
        if (modifiers.HasFlag(OverlayInputModifiers.LeftMouseButton)) raw |= RawInputModifiers.LeftMouseButton;
        if (modifiers.HasFlag(OverlayInputModifiers.RightMouseButton)) raw |= RawInputModifiers.RightMouseButton;
        if (modifiers.HasFlag(OverlayInputModifiers.MiddleMouseButton)) raw |= RawInputModifiers.MiddleMouseButton;
        if (modifiers.HasFlag(OverlayInputModifiers.XButton1MouseButton)) raw |= RawInputModifiers.XButton1MouseButton;
        if (modifiers.HasFlag(OverlayInputModifiers.XButton2MouseButton)) raw |= RawInputModifiers.XButton2MouseButton;
        return new PointerPointProperties(raw, updateKind);
    }

    private static int CountPressedButtons(PointerPointProperties props)
    {
        int count = 0;
        if (props.IsLeftButtonPressed) count++;
        if (props.IsRightButtonPressed) count++;
        if (props.IsMiddleButtonPressed) count++;
        if (props.IsXButton1Pressed) count++;
        if (props.IsXButton2Pressed) count++;
        return count;
    }
}

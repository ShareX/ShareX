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

using Avalonia.Input;
using Avalonia.Threading;
using ShareX.PhotoEditor.UI;

namespace ShareX.PhotoEditor.Tools;

/// <summary>The surface a painting tool writes to: layer pixels, a layer mask, the quick mask or an alpha channel.</summary>
public sealed class PaintTarget
{
    public required PixelSurface Surface { get; init; }
    public SKPointI Offset { get; init; }
    public bool IsMask => Surface.IsMask;
    public RasterLayer? Layer { get; init; }
    public bool LockTransparency { get; init; }
    public required string Description { get; init; }
    public Action? Apply { get; init; }
}

/// <summary>Common behavior of brush-based tools: target resolution, smoothing, airbrush, straight lines and history.</summary>
public abstract class BrushToolBase : ToolBase
{
    private DispatcherTimer? _airbrushTimer;
    private SKPoint _smoothed;
    private SKPoint _lastInput;
    private SKPoint? _lastStrokeEnd;
    private float _lastPressure = 1;

    protected BrushToolBase(IEditorHost host) : base(host)
    {
    }

    protected BrushStroke? Stroke { get; private set; }

    protected PaintTarget? Target { get; private set; }

    protected SKPoint? Hover { get; private set; }

    public BrushPreset Tip { get; set; } = new() { Name = "Soft Round", Hardness = 0, Spacing = 0.1f };

    public float Size { get; set; } = 30;

    public float Opacity { get; set; } = 1;

    public float Flow { get; set; } = 1;

    public LayerBlendMode BlendMode { get; set; } = LayerBlendMode.Normal;

    /// <summary>Lazy-mouse smoothing 0..1.</summary>
    public float Smoothing { get; set; } = 0.1f;

    public bool Airbrush { get; set; }

    public override ToolCursor Cursor => ToolCursor.Brush;

    public override float BrushOutlineSize => Size;

    protected virtual StrokeMode StrokeMode => StrokeMode.Accumulate;

    protected virtual string HistoryName => Name.Replace(" Tool", string.Empty);

    /// <summary>Whether the tool can paint on masks (quick mask, layer masks, channels).</summary>
    protected virtual bool SupportsMaskTargets => true;

    protected virtual bool ShowFlow => true;

    protected abstract void ConfigureStroke(BrushStroke stroke, ToolInput input);

    protected virtual bool CanStart(ToolInput input) => true;

    protected virtual void OnStrokeFinished(BrushStroke stroke, PaintTarget target)
    {
        target.Apply?.Invoke();
        PushHistory(HistoryName);
    }

    /// <summary>Resolves where painting goes, creating masks when needed.</summary>
    protected PaintTarget? ResolveTarget()
    {
        PhotoDocument? doc = Doc;
        if (doc == null) return null;
        if (SupportsMaskTargets && doc.QuickMask)
        {
            PixelSurface mask = doc.Selection?.Clone() ?? PixelSurface.CreateMask(doc.Width, doc.Height, 255);
            return new PaintTarget
            {
                Surface = mask,
                Description = "Quick Mask",
                Apply = () => doc.SetSelectionLive(mask)
            };
        }
        if (SupportsMaskTargets && doc.ActiveChannel is AlphaChannel channel)
        {
            if (channel.Mask.Frozen) channel.Mask = channel.Mask.Clone();
            return new PaintTarget { Surface = channel.Mask, Description = channel.Name };
        }
        if (doc.EditingMask && doc.ActiveLayer is Layer masked && masked.Mask != null)
        {
            if (!SupportsMaskTargets)
            {
                Host.ShowMessage($"The {Name} cannot paint on a layer mask.");
                return null;
            }
            return new PaintTarget { Surface = masked.GetWritableMask(doc.Width, doc.Height), Description = "Layer Mask" };
        }
        if (!RequireEditableLayer(out RasterLayer layer)) return null;
        layer.EnsureCovers(doc.Bounds);
        return new PaintTarget
        {
            Surface = layer.GetWritableSurface(),
            Offset = layer.Offset,
            Layer = layer,
            LockTransparency = layer.LockTransparency
        , Description = layer.Name };
    }

    public override void PointerDown(ToolInput input)
    {
        Hover = input.Doc;
        if (!input.LeftButton || Doc == null || !CanStart(input)) return;
        PaintTarget? target = ResolveTarget();
        if (target == null) return;
        Target = target;
        BrushStroke stroke = new(target.Surface, target.Offset, Tip, StrokeMode)
        {
            Size = Size,
            Opacity = Opacity,
            Flow = ShowFlow ? Flow : 1,
            BlendMode = BlendMode,
            LockTransparency = target.LockTransparency,
            Selection = Doc.QuickMask && target.Description == "Quick Mask" ? null : Doc.Selection
        };
        ConfigureStroke(stroke, input);
        Stroke = stroke;
        _smoothed = input.Doc;
        _lastInput = input.Doc;
        _lastPressure = input.Pressure;
        if (input.Shift && _lastStrokeEnd is SKPoint last)
        {
            stroke.Begin(last, input.Pressure);
            stroke.LineTo(input.Doc, input.Pressure);
        }
        else
        {
            stroke.Begin(input.Doc, input.Pressure);
        }
        target.Apply?.Invoke();
        Host.InvalidateCanvas(stroke.Dirty);
        if (Airbrush)
        {
            _airbrushTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(40) };
            _airbrushTimer.Tick += (_, _) =>
            {
                if (Stroke == null) return;
                Stroke.Dab(_smoothed, _lastPressure);
                Host.InvalidateCanvas(DabRect(_smoothed));
            };
            _airbrushTimer.Start();
        }
    }

    private SKRectI DabRect(SKPoint p)
    {
        int r = (int)MathF.Ceiling(Size / 2 + Tip.Scatter * Size + 2);
        return new SKRectI((int)p.X - r, (int)p.Y - r, (int)p.X + r, (int)p.Y + r);
    }

    public override void PointerMove(ToolInput input)
    {
        Hover = input.Doc;
        if (Stroke == null)
        {
            Host.InvalidateOverlay();
            return;
        }
        _lastInput = input.Doc;
        _lastPressure = input.Pressure;
        SKPoint target = input.Doc;
        if (Smoothing > 0)
        {
            // Pulled-string smoothing: the brush follows the pointer only after it moves beyond a radius.
            float radius = Smoothing * Math.Max(10, Size) * 0.8f;
            SKPoint d = target - _smoothed;
            float distance = d.Length;
            if (distance <= radius) return;
            float move = distance - radius;
            target = _smoothed + new SKPoint(d.X / distance * move, d.Y / distance * move);
        }
        SKRectI before = Stroke.Dirty;
        Stroke.LineTo(target, input.Pressure);
        _smoothed = target;
        Target?.Apply?.Invoke();
        SKRectI changed = DabRect(target);
        changed.Union(DabRect(_smoothed));
        Host.InvalidateCanvas(Stroke.Dirty == before ? changed : SKRectI.Union(changed, DabRect(target)));
        OnStrokeMoved(Stroke);
    }

    protected virtual void OnStrokeMoved(BrushStroke stroke)
    {
    }

    public override void PointerUp(ToolInput input)
    {
        _airbrushTimer?.Stop();
        _airbrushTimer = null;
        if (Stroke == null || Target == null) return;
        if (Smoothing > 0) Stroke.LineTo(_lastInput, _lastPressure);
        BrushStroke stroke = Stroke;
        PaintTarget target = Target;
        Stroke = null;
        Target = null;
        _lastStrokeEnd = _lastInput;
        if (stroke.HasPainted) OnStrokeFinished(stroke, target);
        Host.InvalidateCanvas(stroke.Dirty);
        stroke.Dispose();
    }

    public override void Deactivate()
    {
        Hover = null;
        _lastStrokeEnd = null;
        _airbrushTimer?.Stop();
        _airbrushTimer = null;
        base.Deactivate();
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers)
    {
        if (key == Key.OemOpenBrackets && !modifiers.HasFlag(KeyModifiers.Shift))
        {
            Size = Math.Max(1, Size < 10 ? Size - 1 : Size < 100 ? Size - 5 : Size * 0.9f);
            Host.RefreshOptions();
            Host.InvalidateOverlay();
            return true;
        }
        if (key == Key.OemCloseBrackets && !modifiers.HasFlag(KeyModifiers.Shift))
        {
            Size = Math.Min(5000, Size < 10 ? Size + 1 : Size < 100 ? Size + 5 : Size * 1.1f);
            Host.RefreshOptions();
            Host.InvalidateOverlay();
            return true;
        }
        if (key == Key.OemOpenBrackets || key == Key.OemCloseBrackets)
        {
            Tip.Hardness = Math.Clamp(Tip.Hardness + (key == Key.OemCloseBrackets ? 0.25f : -0.25f), 0, 1);
            Host.RefreshOptions();
            return true;
        }
        if (key >= Key.D0 && key <= Key.D9 && modifiers == KeyModifiers.None)
        {
            int digit = key - Key.D0;
            Opacity = digit == 0 ? 1 : digit / 10f;
            Host.RefreshOptions();
            return true;
        }
        return base.KeyDown(key, modifiers);
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (Hover is SKPoint p && Size * context.Zoom >= 3)
        {
            if (Tip.Roundness < 0.99f || Tip.Shape == BrushTipShape.Square)
            {
                float r = Size / 2;
                float angle = Tip.Angle * MathF.PI / 180;
                List<SKPoint> points = [];
                for (int i = 0; i < 48; i++)
                {
                    float t = i / 48f * MathF.PI * 2;
                    float u = MathF.Cos(t) * r, v = MathF.Sin(t) * r * Tip.Roundness;
                    points.Add(new SKPoint(p.X + u * MathF.Cos(angle) - v * MathF.Sin(angle), p.Y + u * MathF.Sin(angle) + v * MathF.Cos(angle)));
                }
                context.Polygon(points, true);
            }
            else
            {
                context.Circle(p, Size / 2 * context.Zoom);
            }
        }
    }

    protected void BuildBrushOptions(OptionsBuilder options, bool blendMode = true, bool opacity = true, bool flow = true)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.Separator();
        if (blendMode) options.BlendMode(() => BlendMode, m => BlendMode = m);
        if (opacity) options.Percent("Opacity:", () => Opacity, v => Opacity = v, "Maximum paint coverage per stroke (0-9 keys)");
        if (flow && ShowFlow) options.Percent("Flow:", () => Flow, v => Flow = v, "Paint applied per dab");
        if (flow && ShowFlow) options.IconButton("airbrush", "Enable airbrush-style build-up effects", () => { Airbrush = !Airbrush; Host.RefreshOptions(); }, Airbrush);
        options.Percent("Smoothing:", () => Smoothing, v => Smoothing = v);
        options.Check("Pressure Size", () => Tip.PressureSize, v => Tip.PressureSize = v, "Pen pressure controls size");
    }
}

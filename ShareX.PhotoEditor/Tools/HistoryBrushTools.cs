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

using ShareX.PhotoEditor.UI;

namespace ShareX.PhotoEditor.Tools;

public sealed class HistoryBrushTool : BrushToolBase
{
    public HistoryBrushTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.HistoryBrush;

    public override string Name => "History Brush Tool";

    public override string Hint => "Paint pixels back from the history state selected as the source in the History panel.";

    protected override bool CanStart(ToolInput input)
    {
        if (Doc?.History.Source == null)
        {
            Host.ShowMessage("Set a history source in the History panel first.");
            return false;
        }
        if (Doc.History.Source.State.Width != Doc.Width || Doc.History.Source.State.Height != Doc.Height)
        {
            Host.ShowMessage("Could not use the History Brush because the history state does not have the same canvas size.");
            return false;
        }
        return true;
    }

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input)
    {
        stroke.Source = HistorySampler(Doc!, Target?.Layer);
    }

    /// <summary>Samples the history source: the same layer in that state if it exists, otherwise the flattened state.</summary>
    public static Func<int, int, uint> HistorySampler(PhotoDocument doc, RasterLayer? layer)
    {
        DocumentState state = doc.History.Source!.State;
        RasterLayer? sourceLayer = layer != null ? state.FindRasterLayer(layer.Id) : null;
        if (sourceLayer != null)
        {
            SKBitmap bitmap = sourceLayer.Surface.Bitmap;
            SKPointI offset = sourceLayer.Offset;
            return (x, y) => ImageOps.GetPixel(bitmap, x - offset.X, y - offset.Y);
        }
        SKBitmap composite = state.GetComposite();
        return (x, y) => ImageOps.GetPixel(composite, x, y);
    }

    public override void BuildOptions(OptionsBuilder options) => BuildBrushOptions(options);
}

/// <summary>Paints stylized strokes using colors from the history source.</summary>
public sealed class ArtHistoryBrushTool : BrushToolBase
{
    public enum ArtStyle { TightShort, TightMedium, TightLong, LooseMedium, LooseLong, Dab, TightCurl, LooseCurl }

    public ArtHistoryBrushTool(IEditorHost host) : base(host)
    {
        Size = 10;
        Tip = new BrushPreset { Hardness = 0.5f, Spacing = 0.5f };
    }

    public override ToolId Id => ToolId.ArtHistoryBrush;

    public override string Name => "Art History Brush Tool";

    public override string Hint => "Paint stylized strokes sampled from the history source.";

    public ArtStyle Style { get; set; } = ArtStyle.TightShort;

    public float Area { get; set; } = 50;

    public int Tolerance { get; set; }

    protected override StrokeMode StrokeMode => StrokeMode.Direct;

    protected override bool SupportsMaskTargets => false;

    protected override bool CanStart(ToolInput input)
    {
        if (Doc?.History.Source == null)
        {
            Host.ShowMessage("Set a history source in the History panel first.");
            return false;
        }
        return true;
    }

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input)
    {
        Func<int, int, uint> sample = HistoryBrushTool.HistorySampler(Doc!, Target?.Layer);
        Random random = new();
        float area = Area;
        float size = Size;
        ArtStyle style = Style;
        int tolerance = Tolerance;
        stroke.SpacingPixels = Math.Max(4, area / 3);
        stroke.Processor = (s, rect, alpha, center) =>
        {
            using SKCanvas canvas = new(s.Target.Bitmap);
            int count = (int)Math.Clamp(area / 4, 4, 40);
            (float length, float curl, float spread) = style switch
            {
                ArtStyle.TightShort => (size * 1.5f, 0f, 0.5f),
                ArtStyle.TightMedium => (size * 3, 0f, 0.5f),
                ArtStyle.TightLong => (size * 5, 0f, 0.5f),
                ArtStyle.LooseMedium => (size * 3, 0.3f, 1f),
                ArtStyle.LooseLong => (size * 5, 0.3f, 1f),
                ArtStyle.TightCurl => (size * 3, 1.2f, 0.5f),
                ArtStyle.LooseCurl => (size * 4, 1.6f, 1f),
                _ => (size * 0.2f, 0f, 1f)
            };
            using SKPaint paint = new() { IsAntialias = true, StrokeWidth = Math.Max(1, size / 3), StrokeCap = SKStrokeCap.Round, Style = SKPaintStyle.Stroke };
            for (int i = 0; i < count; i++)
            {
                float px = center.X + (random.NextSingle() * 2 - 1) * area * spread;
                float py = center.Y + (random.NextSingle() * 2 - 1) * area * spread;
                uint c = sample((int)px + s.Offset.X, (int)py + s.Offset.Y);
                if ((c >> 24) == 0) continue;
                uint current = ImageOps.GetPixel(s.Target.Bitmap, (int)px, (int)py);
                if (tolerance > 0 && ColorMath.Distance(current, c) <= tolerance * 2.55f) continue;
                if (s.Selection != null && MaskOps.Get(s.Selection, (int)px + s.Offset.X, (int)py + s.Offset.Y) < 128) continue;
                paint.Color = ColorMath.ToColor(c).WithAlpha((byte)(s.Opacity * 255));
                float angle = random.NextSingle() * MathF.PI * 2;
                using SKPath path = new();
                path.MoveTo(px, py);
                float x = px, y = py;
                for (int k = 0; k < 6; k++)
                {
                    angle += curl * (random.NextSingle() - 0.3f);
                    x += MathF.Cos(angle) * length / 6;
                    y += MathF.Sin(angle) * length / 6;
                    path.LineTo(x, y);
                }
                canvas.DrawPath(path, paint);
            }
        };
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.Percent("Opacity:", () => Opacity, v => Opacity = v);
        options.EnumCombo("Style:", () => Style, v => Style = v, 120);
        options.Number("Area:", 1, 500, () => Area, v => Area = (float)v, "0", "px");
        options.Number("Tolerance:", 0, 100, () => Tolerance, v => Tolerance = (int)v, "0", "%");
    }
}

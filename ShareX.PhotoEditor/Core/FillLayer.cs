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

namespace ShareX.PhotoEditor.Core;

public enum FillKind
{
    SolidColor,
    Gradient,
    Pattern
}

public sealed class FillLayer : Layer
{
    public FillLayer(string name) : base(name)
    {
    }

    public override LayerKind Kind => LayerKind.Fill;

    public FillKind FillKind { get; set; }

    public SKColor Color { get; set; } = SKColors.Gray;

    public GradientDefinition Gradient { get; set; } = GradientDefinition.CreateDefault(SKColors.Black, SKColors.White);

    public PatternDefinition? Pattern { get; set; }

    public float PatternScale { get; set; } = 1;

    protected override Layer CreateShallowCopy() => new FillLayer(Name)
    {
        FillKind = FillKind,
        Color = Color,
        Gradient = Gradient.Clone(),
        Pattern = Pattern,
        PatternScale = PatternScale
    };

    public override SKRect GetContentBounds() => SKRect.Empty;

    public override void RenderContent(SKCanvas canvas, RenderContext context)
    {
        SKRect area = SKRect.Create(0, 0, context.DocumentWidth, context.DocumentHeight);
        using SKPaint paint = new() { IsAntialias = false };
        switch (FillKind)
        {
            case FillKind.SolidColor:
                paint.Color = Color;
                break;
            case FillKind.Gradient:
                paint.Shader = Gradient.CreateShader(area);
                break;
            case FillKind.Pattern when Pattern != null:
                paint.Shader = SKShader.CreateBitmap(Pattern.Tile, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat,
                    SKMatrix.CreateScale(PatternScale, PatternScale));
                break;
        }
        canvas.DrawRect(area, paint);
    }
}

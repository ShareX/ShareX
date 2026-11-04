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
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.Media;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.AvaloniaUI.Controls;

/// <summary>
/// Shows an icon string in the Foreground at FontSize: a vector <see cref="ShareXIcons"/> icon, or otherwise
/// a glyph of the icon font, such as a <see cref="LucideIcons"/> constant.
/// </summary>
public class IconPresenter : Control
{
    public static readonly StyledProperty<string?> IconProperty =
        AvaloniaProperty.Register<IconPresenter, string?>(nameof(Icon));

    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        TextElement.ForegroundProperty.AddOwner<IconPresenter>();

    public static readonly StyledProperty<double> FontSizeProperty =
        TextElement.FontSizeProperty.AddOwner<IconPresenter>();

    private ShareXIcons.Shape[]? _shapes;
    private TextBlock? _glyph;

    static IconPresenter()
    {
        AffectsRender<IconPresenter>(ForegroundProperty);
        AffectsMeasure<IconPresenter>(FontSizeProperty);
    }

    public string? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IconProperty)
        {
            UpdateIcon();
        }
    }

    private void UpdateIcon()
    {
        _shapes = ShareXIcons.GetShapes(Icon);

        if (_shapes == null && _glyph == null)
        {
            _glyph = new TextBlock
            {
                FontWeight = FontWeight.Normal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            _glyph.Bind(TextBlock.FontFamilyProperty, _glyph.GetResourceObservable("ShareX.FontFamily.Icon"));
            // Bound rather than inherited, so TextBlock styles (such as an active button's text color)
            // don't recolor the glyph: glyphs and vector icons both follow the presenter.
            _glyph.Bind(TextBlock.ForegroundProperty, this.GetObservable(ForegroundProperty));
            _glyph.Bind(TextBlock.FontSizeProperty, this.GetObservable(FontSizeProperty));
            LogicalChildren.Add(_glyph);
            VisualChildren.Add(_glyph);
        }
        else if (_shapes != null && _glyph != null)
        {
            LogicalChildren.Remove(_glyph);
            VisualChildren.Remove(_glyph);
            _glyph = null;
        }

        if (_glyph != null)
        {
            _glyph.Text = Icon;
        }

        InvalidateMeasure();
        InvalidateVisual();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (_glyph != null)
        {
            _glyph.Measure(availableSize);
            return _glyph.DesiredSize;
        }

        // A vector icon fills the same em square as a glyph of the icon font.
        return _shapes == null ? default : new Size(FontSize, FontSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _glyph?.Arrange(new Rect(finalSize));
        return finalSize;
    }

    public override void Render(DrawingContext context)
    {
        if (_shapes == null || Foreground is not IBrush foreground)
        {
            return;
        }

        double size = Math.Min(Bounds.Width, Bounds.Height);
        double scale = size / ShareXIcons.GridSize;
        Matrix transform = Matrix.CreateScale(scale, scale) * Matrix.CreateTranslation((Bounds.Width - size) / 2, (Bounds.Height - size) / 2);

        using (context.PushTransform(transform))
        {
            foreach (ShareXIcons.Shape shape in _shapes)
            {
                shape.Draw(context, foreground);
            }
        }
    }
}

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
using Avalonia.Media;
using Avalonia.Styling;
using ShareX.Icons;

namespace ShareX.AvaloniaUI.Controls;

/// <summary>
/// Shows a ShareX icon, a key of one of the icon sets such as <see cref="AppIcons"/>, in the Foreground as a square
/// of FontSize, so it lines up with the text around it.
/// </summary>
public class IconPresenter : Control
{
    public static readonly StyledProperty<string?> IconProperty =
        AvaloniaProperty.Register<IconPresenter, string?>(nameof(Icon));

    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        TextElement.ForegroundProperty.AddOwner<IconPresenter>();

    public static readonly StyledProperty<double> FontSizeProperty =
        TextElement.FontSizeProperty.AddOwner<IconPresenter>();

    private VectorIcon? _icon;

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

        if (change.Property == IconProperty || change.Property == IconPackScope.PackProperty)
        {
            UpdateIcon();
        }
        else if (change.Property == ThemeVariantScope.ActualThemeVariantProperty)
        {
            InvalidateVisual();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        IconCatalog.PackChanged += OnPackChanged;
        UpdateIcon();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        IconCatalog.PackChanged -= OnPackChanged;
        base.OnDetachedFromVisualTree(e);
    }

    private void OnPackChanged(object? sender, EventArgs e) => UpdateIcon();

    private void UpdateIcon()
    {
        _icon = IconCatalog.Find(Icon, IconPackScope.GetPack(this) ?? IconCatalog.Pack);
        InvalidateMeasure();
        InvalidateVisual();
    }

    protected override Size MeasureOverride(Size availableSize) =>
        _icon == null ? default : new Size(FontSize, FontSize);

    public override void Render(DrawingContext context)
    {
        if (_icon != null && Foreground is IBrush foreground)
        {
            _icon.Draw(context, new Rect(Bounds.Size), foreground, IsDarkTheme(ActualThemeVariant));
        }
    }

    private static bool? IsDarkTheme(ThemeVariant? theme)
    {
        for (; theme != null; theme = theme.InheritVariant)
        {
            if (theme == ThemeVariant.Dark)
            {
                return true;
            }

            if (theme == ThemeVariant.Light)
            {
                return false;
            }
        }

        return null;
    }
}

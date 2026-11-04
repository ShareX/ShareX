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

namespace ShareX.Icons;

/// <summary>
/// Shows a ShareX icon in the Foreground. Without a set Width and Height it takes the size of the icon's grid,
/// where it is sharpest.
/// </summary>
public class IconView : Control
{
    public static readonly StyledProperty<string?> IconProperty =
        AvaloniaProperty.Register<IconView, string?>(nameof(Icon));

    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        TextElement.ForegroundProperty.AddOwner<IconView>();

    private VectorIcon? _icon;

    static IconView()
    {
        AffectsRender<IconView>(ForegroundProperty);
    }

    public IconView()
    {
    }

    public IconView(string icon, double size)
    {
        Icon = icon;
        Width = size;
        Height = size;
    }

    /// <summary>The icon's key, a constant of <see cref="ImageEditorIcons"/> or <see cref="PhotoEditorIcons"/>.</summary>
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IconProperty)
        {
            _icon = IconCatalog.Find(Icon);
            InvalidateMeasure();
            InvalidateVisual();
        }
    }

    protected override Size MeasureOverride(Size availableSize) =>
        _icon == null ? default : new Size(_icon.GridSize, _icon.GridSize);

    public override void Render(DrawingContext context)
    {
        if (_icon != null && Foreground is IBrush foreground)
        {
            _icon.Draw(context, new Rect(Bounds.Size), foreground);
        }
    }
}

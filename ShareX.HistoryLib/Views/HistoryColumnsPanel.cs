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
using System;
using System.Collections.Specialized;

namespace ShareX.HistoryLib;

/// <summary>
/// Lays out history list cells side by side using a shared <see cref="HistoryColumnLayout"/>, so the header and every row line up.
/// Hidden columns are collapsed and the last visible column fills the remaining width.
/// </summary>
public sealed class HistoryColumnsPanel : Panel
{
    public static readonly StyledProperty<HistoryColumnLayout?> LayoutProperty =
        AvaloniaProperty.Register<HistoryColumnsPanel, HistoryColumnLayout?>(nameof(Layout));

    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<HistoryColumnsPanel, double>(nameof(Spacing), 8);

    public static readonly AttachedProperty<HistoryColumn> ColumnProperty =
        AvaloniaProperty.RegisterAttached<HistoryColumnsPanel, Control, HistoryColumn>("Column");

    static HistoryColumnsPanel()
    {
        AffectsMeasure<HistoryColumnsPanel>(LayoutProperty, SpacingProperty);
        AffectsParentMeasure<HistoryColumnsPanel>(ColumnProperty);
        ClipToBoundsProperty.OverrideDefaultValue<HistoryColumnsPanel>(true);
    }

    private HistoryColumnLayout? _subscribedLayout;

    public HistoryColumnLayout? Layout
    {
        get => GetValue(LayoutProperty);
        set => SetValue(LayoutProperty, value);
    }

    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public static HistoryColumn GetColumn(Control control) => control.GetValue(ColumnProperty);

    public static void SetColumn(Control control, HistoryColumn value) => control.SetValue(ColumnProperty, value);

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Subscribe(Layout);
        UpdateChildVisibility();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        Subscribe(null);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == LayoutProperty)
        {
            if (VisualRoot != null)
            {
                Subscribe(Layout);
            }

            UpdateChildVisibility();
        }
    }

    protected override void ChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        base.ChildrenChanged(sender, e);
        UpdateChildVisibility();
    }

    private void Subscribe(HistoryColumnLayout? layout)
    {
        if (ReferenceEquals(_subscribedLayout, layout))
        {
            return;
        }

        if (_subscribedLayout != null)
        {
            _subscribedLayout.Changed -= OnLayoutChanged;
        }

        _subscribedLayout = layout;

        if (_subscribedLayout != null)
        {
            _subscribedLayout.Changed += OnLayoutChanged;
        }
    }

    private void OnLayoutChanged(object? sender, EventArgs e)
    {
        UpdateChildVisibility();
        InvalidateMeasure();
    }

    private void UpdateChildVisibility()
    {
        HistoryColumnLayout? layout = Layout;

        foreach (Control child in Children)
        {
            child.IsVisible = layout?.IsVisible(GetColumn(child)) ?? GetColumn(child) == HistoryColumn.Icon;
        }
    }

    private double GetCellWidth(Control child, HistoryColumnLayout layout, double fillWidth)
    {
        HistoryColumn column = GetColumn(child);
        return column == layout.FillColumn ? fillWidth : layout.GetWidth(column);
    }

    private double GetFillWidth(HistoryColumnLayout layout, double availableWidth)
    {
        HistoryColumn? fillColumn = layout.FillColumn;

        if (fillColumn == null)
        {
            return 0;
        }

        if (double.IsInfinity(availableWidth))
        {
            return layout.GetWidth(fillColumn.Value);
        }

        double used = 0;
        int count = 0;

        foreach (Control child in Children)
        {
            if (!child.IsVisible) continue;

            HistoryColumn column = GetColumn(child);
            if (column != fillColumn) used += layout.GetWidth(column);
            count++;
        }

        used += Math.Max(0, count - 1) * Spacing;
        return Math.Max(HistoryColumnLayout.MinColumnWidth, availableWidth - used);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        HistoryColumnLayout? layout = Layout;
        if (layout == null)
        {
            return base.MeasureOverride(availableSize);
        }

        double fillWidth = GetFillWidth(layout, availableSize.Width);
        double width = 0;
        double height = 0;
        int count = 0;

        foreach (Control child in Children)
        {
            if (!child.IsVisible) continue;

            double cellWidth = GetCellWidth(child, layout, fillWidth);
            child.Measure(new Size(cellWidth, availableSize.Height));
            width += cellWidth;
            height = Math.Max(height, child.DesiredSize.Height);
            count++;
        }

        width += Math.Max(0, count - 1) * Spacing;

        if (!double.IsInfinity(availableSize.Width))
        {
            width = Math.Min(width, availableSize.Width);
        }

        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        HistoryColumnLayout? layout = Layout;
        if (layout == null)
        {
            return base.ArrangeOverride(finalSize);
        }

        double fillWidth = GetFillWidth(layout, finalSize.Width);
        double x = 0;

        foreach (Control child in Children)
        {
            if (!child.IsVisible) continue;

            double cellWidth = GetCellWidth(child, layout, fillWidth);
            child.Arrange(new Rect(x, 0, cellWidth, finalSize.Height));
            x += cellWidth + Spacing;
        }

        return finalSize;
    }
}

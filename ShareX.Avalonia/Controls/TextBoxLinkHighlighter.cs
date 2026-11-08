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
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using System.Text.RegularExpressions;

namespace ShareX.AvaloniaUI.Controls;

/// <summary>
/// A text link found by <see cref="TextBoxLinkHighlighter.FindLinks"/>: where it sits in the text and the URL it opens.
/// </summary>
public readonly record struct TextLink(int Start, int Length, string URL);

/// <summary>
/// Underlines the URLs in a <see cref="TextBox"/> and opens one on Ctrl+click. Place it over the text box, in the same
/// grid cell, and point <see cref="Target"/> at it. It doesn't take input itself, so the text box still edits as usual.
/// </summary>
public partial class TextBoxLinkHighlighter : Control
{
    public static readonly StyledProperty<TextBox?> TargetProperty =
        AvaloniaProperty.Register<TextBoxLinkHighlighter, TextBox?>(nameof(Target));

    public static readonly StyledProperty<IBrush?> LinkBrushProperty =
        AvaloniaProperty.Register<TextBoxLinkHighlighter, IBrush?>(nameof(LinkBrush));

    private static readonly Cursor HandCursor = new(StandardCursorType.Hand);

    private TextPresenter? _presenter;
    private ScrollViewer? _scrollViewer;
    private string? _linksText;
    private IReadOnlyList<TextLink> _links = [];
    private Point? _lastPointerPosition;
    private TextPresenter? _handCursorElement;

    static TextBoxLinkHighlighter()
    {
        IsHitTestVisibleProperty.OverrideDefaultValue<TextBoxLinkHighlighter>(false);
        AffectsRender<TextBoxLinkHighlighter>(LinkBrushProperty);
    }

    public TextBox? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    public IBrush? LinkBrush
    {
        get => GetValue(LinkBrushProperty);
        set => SetValue(LinkBrushProperty, value);
    }

    /// <summary>
    /// Raised with the URL when a link is Ctrl+clicked.
    /// </summary>
    public event EventHandler<string>? LinkClicked;

    public static IReadOnlyList<TextLink> FindLinks(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return [];
        }

        List<TextLink> links = [];

        foreach (Match match in LinkRegex().Matches(text))
        {
            string value = TrimTrailingPunctuation(match.Value);

            if (value.Length > 0 && value.Contains('.'))
            {
                string url = value.StartsWith("www.", StringComparison.OrdinalIgnoreCase) ? "https://" + value : value;
                links.Add(new TextLink(match.Index, value.Length, url));
            }
        }

        return links;
    }

    // Sentence punctuation right after a URL ("Visit https://getsharex.com.") isn't part of it, and a closing bracket
    // only is when the URL opened one ("https://en.wikipedia.org/wiki/Foo_(bar)").
    private static string TrimTrailingPunctuation(string value)
    {
        int end = value.Length;

        while (end > 0)
        {
            char c = value[end - 1];

            if (".,;:!?'\"".Contains(c) ||
                (c == ')' && Count(value, end, '(') < Count(value, end, ')')) ||
                (c == ']' && Count(value, end, '[') < Count(value, end, ']')) ||
                (c == '}' && Count(value, end, '{') < Count(value, end, '}')))
            {
                end--;
            }
            else
            {
                break;
            }
        }

        return value[..end];

        static int Count(string value, int length, char c) => value.AsSpan(0, length).Count(c);
    }

    [GeneratedRegex(@"\b(?:(?:https?|ftp)://|www\.)[^\s<>""]+", RegexOptions.IgnoreCase)]
    private static partial Regex LinkRegex();

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TargetProperty)
        {
            Detach(change.GetOldValue<TextBox?>());
            Attach(change.GetNewValue<TextBox?>());
        }
    }

    private void Attach(TextBox? textBox)
    {
        if (textBox == null)
        {
            return;
        }

        textBox.TemplateApplied += OnTargetTemplateApplied;
        textBox.AddHandler(PointerPressedEvent, OnTargetPointerPressed, RoutingStrategies.Tunnel, handledEventsToo: true);
        textBox.AddHandler(PointerMovedEvent, OnTargetPointerMoved, RoutingStrategies.Tunnel, handledEventsToo: true);
        textBox.AddHandler(PointerExitedEvent, OnTargetPointerExited, RoutingStrategies.Direct | RoutingStrategies.Bubble, handledEventsToo: true);
        textBox.AddHandler(KeyDownEvent, OnTargetModifierKeyChanged, RoutingStrategies.Tunnel, handledEventsToo: true);
        textBox.AddHandler(KeyUpEvent, OnTargetModifierKeyChanged, RoutingStrategies.Tunnel, handledEventsToo: true);

        // The template may already be applied when the target is set after the text box is loaded.
        List<Visual> templateChildren = textBox.GetVisualDescendants().ToList();
        SetTemplateParts(templateChildren.OfType<ScrollViewer>().FirstOrDefault(), templateChildren.OfType<TextPresenter>().FirstOrDefault());
    }

    private void Detach(TextBox? textBox)
    {
        if (textBox == null)
        {
            return;
        }

        textBox.TemplateApplied -= OnTargetTemplateApplied;
        textBox.RemoveHandler(PointerPressedEvent, OnTargetPointerPressed);
        textBox.RemoveHandler(PointerMovedEvent, OnTargetPointerMoved);
        textBox.RemoveHandler(PointerExitedEvent, OnTargetPointerExited);
        textBox.RemoveHandler(KeyDownEvent, OnTargetModifierKeyChanged);
        textBox.RemoveHandler(KeyUpEvent, OnTargetModifierKeyChanged);
        SetTemplateParts(null, null);
    }

    private void OnTargetTemplateApplied(object? sender, TemplateAppliedEventArgs e)
    {
        SetTemplateParts(e.NameScope.Find<ScrollViewer>("PART_ScrollViewer"), e.NameScope.Find<TextPresenter>("PART_TextPresenter"));
    }

    private void SetTemplateParts(ScrollViewer? scrollViewer, TextPresenter? presenter)
    {
        SetHandCursor(false);

        if (_presenter != null)
        {
            _presenter.LayoutUpdated -= OnTargetLayoutUpdated;
        }

        if (_scrollViewer != null)
        {
            _scrollViewer.ScrollChanged -= OnTargetScrollChanged;
        }

        _presenter = presenter;
        _scrollViewer = scrollViewer;

        if (_presenter != null)
        {
            _presenter.LayoutUpdated += OnTargetLayoutUpdated;
        }

        if (_scrollViewer != null)
        {
            _scrollViewer.ScrollChanged += OnTargetScrollChanged;
        }

        InvalidateVisual();
    }

    private void OnTargetLayoutUpdated(object? sender, EventArgs e) => InvalidateVisual();

    private void OnTargetScrollChanged(object? sender, ScrollChangedEventArgs e) => InvalidateVisual();

    private IReadOnlyList<TextLink> GetLinks()
    {
        string? text = Target?.Text;

        if (!string.Equals(text, _linksText, StringComparison.Ordinal))
        {
            _linksText = text;
            _links = FindLinks(text);
        }

        return _links;
    }

    // Where the presenter draws its text layout, in its own coordinates. It aligns the layout vertically inside itself
    // the same way when it hit tests the caret.
    private static Point GetTextOrigin(TextPresenter presenter)
    {
        double top = 0;
        double extraHeight = presenter.Bounds.Height - presenter.TextLayout.Height;

        if (extraHeight > 0)
        {
            top = presenter.VerticalAlignment switch
            {
                VerticalAlignment.Center => extraHeight / 2,
                VerticalAlignment.Bottom => extraHeight,
                _ => 0
            };
        }

        return new Point(0, top);
    }

    private TextLink? HitTestLink(Point pointInPresenter)
    {
        if (_presenter == null)
        {
            return null;
        }

        IReadOnlyList<TextLink> links = GetLinks();

        if (links.Count == 0)
        {
            return null;
        }

        Point point = pointInPresenter - GetTextOrigin(_presenter);
        TextHitTestResult hit = _presenter.TextLayout.HitTestPoint(point);

        if (!hit.IsInside)
        {
            return null;
        }

        int index = hit.TextPosition;

        foreach (TextLink link in links)
        {
            if (index >= link.Start && index < link.Start + link.Length)
            {
                return link;
            }
        }

        return null;
    }

    private TextLink? HitTestLink(PointerEventArgs e)
    {
        return _presenter == null ? null : HitTestLink(e.GetPosition(_presenter));
    }

    private void OnTargetPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not TextBox textBox || !e.KeyModifiers.HasFlag(KeyModifiers.Control) ||
            !e.GetCurrentPoint(textBox).Properties.IsLeftButtonPressed)
        {
            return;
        }

        if (HitTestLink(e) is { } link)
        {
            e.Handled = true;
            LinkClicked?.Invoke(this, link.URL);
        }
    }

    private void OnTargetPointerMoved(object? sender, PointerEventArgs e)
    {
        _lastPointerPosition = _presenter == null ? null : e.GetPosition(_presenter);
        UpdateCursor(e.KeyModifiers.HasFlag(KeyModifiers.Control));
    }

    private void OnTargetPointerExited(object? sender, PointerEventArgs e)
    {
        _lastPointerPosition = null;
        SetHandCursor(false);
    }

    private void OnTargetModifierKeyChanged(object? sender, KeyEventArgs e)
    {
        if (e.Key is Key.LeftCtrl or Key.RightCtrl)
        {
            UpdateCursor(e.RoutedEvent == KeyDownEvent);
        }
    }

    private void UpdateCursor(bool controlPressed)
    {
        SetHandCursor(controlPressed && _lastPointerPosition is { } position && HitTestLink(position) != null);
    }

    // The hand goes on the text presenter, the element under the pointer. Setting it on the text box does nothing,
    // because the text box template gives the presenter's parent its own I-beam cursor.
    private void SetHandCursor(bool show)
    {
        TextPresenter? element = show ? _presenter : null;

        if (element == _handCursorElement)
        {
            return;
        }

        _handCursorElement?.ClearValue(CursorProperty);
        _handCursorElement = element;

        if (element != null)
        {
            element.Cursor = HandCursor;
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (_presenter == null || _scrollViewer == null || LinkBrush is not { } brush || !_presenter.IsEffectivelyVisible)
        {
            return;
        }

        IReadOnlyList<TextLink> links = GetLinks();

        if (links.Count == 0 || _presenter.TranslatePoint(GetTextOrigin(_presenter), this) is not { } origin ||
            _scrollViewer.TranslatePoint(default, this) is not { } viewportOrigin)
        {
            return;
        }

        Rect viewport = new Rect(viewportOrigin, _scrollViewer.Bounds.Size);
        Pen pen = new Pen(brush, 1);

        using (context.PushClip(viewport))
        {
            foreach (TextLink link in links)
            {
                foreach (Rect rect in _presenter.TextLayout.HitTestTextRange(link.Start, link.Length))
                {
                    if (rect.Width <= 0)
                    {
                        continue;
                    }

                    // Snap to the pixel grid so the 1px underline stays crisp.
                    double y = Math.Round(origin.Y + rect.Bottom) - 0.5;
                    context.DrawLine(pen, new Point(origin.X + rect.Left, y), new Point(origin.X + rect.Right, y));
                }
            }
        }
    }
}

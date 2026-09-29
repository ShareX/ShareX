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
using Avalonia.Threading;
using System.ComponentModel;

namespace ShareX.Tools;

/// <summary>Filmstrip with independent selection handles and playhead. Numeric fields provide an accessible alternative.</summary>
public sealed class AnimatedGifTrimmerTimeline : Control
{
    public static readonly StyledProperty<IBrush?> AccentProperty =
        AvaloniaProperty.Register<AnimatedGifTrimmerTimeline, IBrush?>(nameof(Accent), Brushes.DodgerBlue);
    public IBrush? Accent { get => GetValue(AccentProperty); set => SetValue(AccentProperty, value); }

    public static readonly StyledProperty<IBrush?> TrackBackgroundProperty =
        AvaloniaProperty.Register<AnimatedGifTrimmerTimeline, IBrush?>(nameof(TrackBackground), Brushes.Transparent);
    public IBrush? TrackBackground { get => GetValue(TrackBackgroundProperty); set => SetValue(TrackBackgroundProperty, value); }

    private AnimatedGifTrimmerViewModel? _model;
    private int _drag;
    private const double Inset = 14;
    private const double TrackTop = 12;
    private const double TrackHeight = 48;
    private const double TrackBottom = TrackTop + TrackHeight;
    private double TrackWidth => Math.Max(1, Bounds.Width - Inset * 2);
    private double X(double value) => Inset + value / Math.Max(0.001, _model?.Duration ?? 0) * TrackWidth;

    static AnimatedGifTrimmerTimeline()
    {
        AffectsRender<AnimatedGifTrimmerTimeline>(AccentProperty, TrackBackgroundProperty);
    }

    public AnimatedGifTrimmerTimeline()
    {
        Focusable = true;
        ClipToBounds = true;
        DataContextChanged += (_, _) => Subscribe();
        AttachedToVisualTree += (_, _) => Subscribe();
        DetachedFromVisualTree += (_, _) =>
        {
            if (_model != null) _model.PropertyChanged -= ModelChanged;
            _model = null;
        };
    }

    private void Subscribe()
    {
        if (_model != null) _model.PropertyChanged -= ModelChanged;
        _model = DataContext as AnimatedGifTrimmerViewModel;
        if (_model != null) _model.PropertyChanged += ModelChanged;
        InvalidateVisual();
    }

    private void ModelChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != null && e.PropertyName != nameof(AnimatedGifTrimmerViewModel.HasGif) &&
            e.PropertyName != nameof(AnimatedGifTrimmerViewModel.Duration) &&
            e.PropertyName != nameof(AnimatedGifTrimmerViewModel.Start) &&
            e.PropertyName != nameof(AnimatedGifTrimmerViewModel.End) &&
            e.PropertyName != nameof(AnimatedGifTrimmerViewModel.Position) &&
            e.PropertyName != nameof(AnimatedGifTrimmerViewModel.Thumbnails)) return;

        if (Dispatcher.UIThread.CheckAccess()) InvalidateVisual();
        else Dispatcher.UIThread.Post(() =>
        {
            if (ReferenceEquals(_model, sender)) InvalidateVisual();
        });
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        Rect track = new(Inset, TrackTop, TrackWidth, TrackHeight);
        context.DrawRectangle(TrackBackground, null, track, 4, 4);
        if (_model is not { HasGif: true } model) return;

        using (context.PushClip(new RoundedRect(track, 4)))
        {
            double cellWidth = TrackWidth / 12;
            for (int i = 0; i < model.Thumbnails.Count; i++)
            {
                var bitmap = model.Thumbnails[i].Image;
                double cropWidth = Math.Min(bitmap.Size.Width, bitmap.Size.Height * cellWidth / TrackHeight);
                double cropHeight = Math.Min(bitmap.Size.Height, bitmap.Size.Width * TrackHeight / cellWidth);
                Rect source = new((bitmap.Size.Width - cropWidth) / 2, (bitmap.Size.Height - cropHeight) / 2, cropWidth, cropHeight);
                context.DrawImage(bitmap, source, new Rect(Inset + i * cellWidth, TrackTop, cellWidth, TrackHeight));
            }

            IBrush shade = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0));
            context.DrawRectangle(shade, null, new Rect(Inset, TrackTop, X(model.Start) - Inset, TrackHeight));
            context.DrawRectangle(shade, null, new Rect(X(model.End), TrackTop, X(model.Duration) - X(model.End), TrackHeight));
        }

        double left = X(model.Start);
        double right = X(model.End);
        context.DrawRectangle(null, new Pen(Accent, 2), new Rect(left, TrackTop - 1, right - left, TrackHeight + 2), 4, 4);
        foreach (double x in new[] { left, right })
        {
            context.DrawRectangle(Accent, null, new Rect(x - 5, TrackTop - 3, 10, TrackHeight + 6), 3, 3);
            context.DrawLine(new Pen(Brushes.White, 2), new Point(x, TrackTop + 16), new Point(x, TrackBottom - 16));
        }

        double playhead = X(model.Position);
        context.DrawLine(new Pen(Brushes.Black, 4), new Point(playhead, 4), new Point(playhead, TrackBottom + 8));
        context.DrawLine(new Pen(Brushes.White, 2), new Point(playhead, 4), new Point(playhead, TrackBottom + 8));
        context.DrawEllipse(Brushes.White, null, new Point(playhead, 5), 4, 4);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (_model is not { CanEdit: true } model || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        Focus();
        Point point = e.GetPosition(this);
        double startDistance = Math.Abs(point.X - X(model.Start));
        double endDistance = Math.Abs(point.X - X(model.End));
        _drag = point.Y >= TrackTop - 3 && point.Y <= TrackBottom + 3 && Math.Min(startDistance, endDistance) <= 12
            ? (startDistance < endDistance ? 1 : 2) : 3;
        e.Pointer.Capture(this);
        UpdatePosition(point.X);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_drag != 0) UpdatePosition(e.GetPosition(this).X);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_drag == 0) return;
        _drag = 0;
        e.Pointer.Capture(null);
        e.Handled = true;
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        _drag = 0;
        base.OnPointerCaptureLost(e);
    }

    private void UpdatePosition(double x)
    {
        if (_model is not { CanEdit: true } model) return;
        double value = Math.Clamp((x - Inset) / TrackWidth, 0, 1) * model.Duration;
        if (_drag == 1) model.SetStart(value);
        else if (_drag == 2) model.SetEnd(value);
        else model.Seek(value);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (_model is not { CanEdit: true } model) return;
        int step = e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? 10 : 1;
        switch (e.Key)
        {
            case Key.Left: model.StepPosition(-step); break;
            case Key.Right: model.StepPosition(step); break;
            case Key.Home: model.Seek(model.Start); break;
            case Key.End: model.Seek(model.End); break;
            case Key.I: model.SetStartHereCommand.Execute(null); break;
            case Key.O: model.SetEndHereCommand.Execute(null); break;
            default: return;
        }
        e.Handled = true;
    }
}


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
using Avalonia.Layout;
using Avalonia.Media;
using ShareX.PhotoEditor.Tools;

namespace ShareX.PhotoEditor.UI.Dialogs;

/// <summary>Builds parameter editors for every adjustment kind.</summary>
public static class AdjustmentEditor
{
    public static Control Build(AdjustmentSettings s, Action changed, SKBitmap? histogramSource, IEditorHost? host)
    {
        StackPanel root = new() { Spacing = 8 };
        Grid form = DialogBuilder.Form();
        void Slider(string label, string key, double min, double max, double defaultValue = 0, double step = 1, string format = "0")
        {
            DialogBuilder.AddRow(form, label, DialogBuilder.SliderNumber(min, max, s.Get(key, (float)defaultValue), v => { s.Set(key, (float)v); changed(); }, step, format));
        }
        void Check(string label, string key)
        {
            CheckBox box = new() { Content = label, IsChecked = s.Get(key) > 0 };
            box.IsCheckedChanged += (_, _) => { s.Set(key, box.IsChecked == true ? 1 : 0); changed(); };
            DialogBuilder.AddFull(form, box);
        }
        void ColorRow(string label)
        {
            Button button = new() { Content = new Border { Width = 40, Height = 16, Background = DialogBuilder.ToBrush(s.Color) } };
            button.Click += async (_, _) =>
            {
                Window? owner = TopLevel.GetTopLevel(button) as Window;
                if (owner == null) return;
                SKColor? c = await ColorPickerDialog.ShowAsync(owner, s.Color, label, host);
                if (c is SKColor color)
                {
                    s.Color = color;
                    button.Content = new Border { Width = 40, Height = 16, Background = DialogBuilder.ToBrush(color) };
                    changed();
                }
            };
            DialogBuilder.AddRow(form, label, button);
        }

        switch (s.Kind)
        {
            case AdjustmentKind.BrightnessContrast:
                Slider("Brightness:", "brightness", -150, 150);
                Slider("Contrast:", "contrast", -50, 100);
                Check("Use Legacy", "legacy");
                break;
            case AdjustmentKind.Levels:
                root.Children.Add(LevelsEditor(s, changed, histogramSource));
                break;
            case AdjustmentKind.Curves:
                root.Children.Add(CurvesEditor(s, changed, histogramSource));
                break;
            case AdjustmentKind.Exposure:
                Slider("Exposure:", "exposure", -20, 20, 0, 0.01, "0.00");
                Slider("Offset:", "offset", -0.5, 0.5, 0, 0.0001, "0.0000");
                Slider("Gamma Correction:", "gamma", 0.01, 9.99, 1, 0.01, "0.00");
                break;
            case AdjustmentKind.Vibrance:
                Slider("Vibrance:", "vibrance", -100, 100);
                Slider("Saturation:", "saturation", -100, 100);
                break;
            case AdjustmentKind.HueSaturation:
                Slider("Hue:", "hue", -180, 180);
                Slider("Saturation:", "saturation", -100, 100);
                Slider("Lightness:", "lightness", -100, 100);
                Check("Colorize", "colorize");
                break;
            case AdjustmentKind.ColorBalance:
            {
                string range = "midtones";
                Grid balance = DialogBuilder.Form();
                void Build()
                {
                    balance.Children.Clear();
                    balance.RowDefinitions.Clear();
                    DialogBuilder.AddRow(balance, "Cyan / Red:", DialogBuilder.SliderNumber(-100, 100, s.Get($"{range}.cr"), v => { s.Set($"{range}.cr", (float)v); changed(); }));
                    DialogBuilder.AddRow(balance, "Magenta / Green:", DialogBuilder.SliderNumber(-100, 100, s.Get($"{range}.mg"), v => { s.Set($"{range}.mg", (float)v); changed(); }));
                    DialogBuilder.AddRow(balance, "Yellow / Blue:", DialogBuilder.SliderNumber(-100, 100, s.Get($"{range}.yb"), v => { s.Set($"{range}.yb", (float)v); changed(); }));
                }
                DialogBuilder.AddRow(form, "Tone:", DialogBuilder.Combo([("Shadows", "shadows"), ("Midtones", "midtones"), ("Highlights", "highlights")], range, v => { range = v; Build(); }));
                Build();
                DialogBuilder.AddFull(form, balance);
                Check("Preserve Luminosity", "preserve");
                break;
            }
            case AdjustmentKind.BlackAndWhite:
                Slider("Reds:", "reds", -200, 300, 40);
                Slider("Yellows:", "yellows", -200, 300, 60);
                Slider("Greens:", "greens", -200, 300, 40);
                Slider("Cyans:", "cyans", -200, 300, 60);
                Slider("Blues:", "blues", -200, 300, 20);
                Slider("Magentas:", "magentas", -200, 300, 80);
                Check("Tint", "tint");
                ColorRow("Tint Color:");
                break;
            case AdjustmentKind.PhotoFilter:
                ColorRow("Filter Color:");
                Slider("Density:", "density", 1, 100, 25);
                Check("Preserve Luminosity", "preserve");
                break;
            case AdjustmentKind.ChannelMixer:
            {
                string output = "r";
                Grid mixer = DialogBuilder.Form();
                void Build()
                {
                    mixer.Children.Clear();
                    mixer.RowDefinitions.Clear();
                    foreach (var (label, source) in new[] { ("Red:", "r"), ("Green:", "g"), ("Blue:", "b"), ("Constant:", "c") })
                    {
                        string key = $"{output}.{source}";
                        DialogBuilder.AddRow(mixer, label, DialogBuilder.SliderNumber(-200, 200, s.Get(key, output == source ? 100 : 0), v => { s.Set(key, (float)v); changed(); }));
                    }
                }
                DialogBuilder.AddRow(form, "Output Channel:", DialogBuilder.Combo([("Red", "r"), ("Green", "g"), ("Blue", "b")], output, v => { output = v; Build(); }));
                Build();
                DialogBuilder.AddFull(form, mixer);
                Check("Monochrome", "monochrome");
                break;
            }
            case AdjustmentKind.Posterize:
                Slider("Levels:", "levels", 2, 255, 4);
                break;
            case AdjustmentKind.Threshold:
                if (histogramSource != null) DialogBuilder.AddFull(form, new HistogramControl(Adjustments.Histogram(histogramSource)[0]) { Height = 80 });
                Slider("Threshold Level:", "level", 1, 255, 128);
                break;
            case AdjustmentKind.GradientMap:
            {
                s.Gradient ??= host != null ? GradientDefinition.CreateDefault(host.Foreground, host.Background) : GradientDefinition.CreateDefault(SKColors.Black, SKColors.White);
                Button edit = new() { Content = "Edit Gradient..." };
                edit.Click += async (_, _) =>
                {
                    if (TopLevel.GetTopLevel(edit) is not Window owner || host == null) return;
                    GradientDefinition? g = await GradientEditorDialog.ShowAsync(owner, s.Gradient, host);
                    if (g != null) { s.Gradient = g; changed(); }
                };
                CheckBox reverse = new() { Content = "Reverse", IsChecked = s.Gradient.Reverse };
                reverse.IsCheckedChanged += (_, _) => { s.Gradient!.Reverse = reverse.IsChecked == true; changed(); };
                DialogBuilder.AddFull(form, edit);
                DialogBuilder.AddFull(form, reverse);
                break;
            }
            case AdjustmentKind.SelectiveColor:
            {
                string group = "reds";
                Grid selective = DialogBuilder.Form();
                void Build()
                {
                    selective.Children.Clear();
                    selective.RowDefinitions.Clear();
                    foreach (var (label, c) in new[] { ("Cyan:", "c"), ("Magenta:", "m"), ("Yellow:", "y"), ("Black:", "k") })
                    {
                        string key = $"{group}.{c}";
                        DialogBuilder.AddRow(selective, label, DialogBuilder.SliderNumber(-100, 100, s.Get(key), v => { s.Set(key, (float)v); changed(); }));
                    }
                }
                DialogBuilder.AddRow(form, "Colors:", DialogBuilder.Combo(new[] { "reds", "yellows", "greens", "cyans", "blues", "magentas", "whites", "neutrals", "blacks" }
                    .Select(g => (char.ToUpper(g[0]) + g[1..], g)), group, v => { group = v; Build(); }));
                Build();
                DialogBuilder.AddFull(form, selective);
                break;
            }
            default:
                DialogBuilder.AddFull(form, new TextBlock { Text = "This adjustment has no options." });
                break;
        }
        root.Children.Add(form);
        return root;
    }

    private static Control LevelsEditor(AdjustmentSettings s, Action changed, SKBitmap? source)
    {
        int[][] histograms = source != null ? Adjustments.Histogram(source) : [new int[256], new int[256], new int[256], new int[256]];
        string channel = "rgb";
        StackPanel root = new() { Spacing = 6 };
        HistogramControl histogram = new(histograms[0]) { Height = 110 };
        Grid inputs = DialogBuilder.Form();
        void Build()
        {
            inputs.Children.Clear();
            inputs.RowDefinitions.Clear();
            histogram.Values = channel switch { "r" => histograms[1], "g" => histograms[2], "b" => histograms[3], _ => histograms[0] };
            DialogBuilder.AddRow(inputs, "Input Black:", DialogBuilder.SliderNumber(0, 253, s.Get($"{channel}.inBlack"), v => { s.Set($"{channel}.inBlack", (float)v); changed(); }));
            DialogBuilder.AddRow(inputs, "Gamma:", DialogBuilder.SliderNumber(0.1, 9.99, s.Get($"{channel}.gamma", 1), v => { s.Set($"{channel}.gamma", (float)v); changed(); }, 0.01, "0.00"));
            DialogBuilder.AddRow(inputs, "Input White:", DialogBuilder.SliderNumber(2, 255, s.Get($"{channel}.inWhite", 255), v => { s.Set($"{channel}.inWhite", (float)v); changed(); }));
            DialogBuilder.AddRow(inputs, "Output Black:", DialogBuilder.SliderNumber(0, 255, s.Get($"{channel}.outBlack"), v => { s.Set($"{channel}.outBlack", (float)v); changed(); }));
            DialogBuilder.AddRow(inputs, "Output White:", DialogBuilder.SliderNumber(0, 255, s.Get($"{channel}.outWhite", 255), v => { s.Set($"{channel}.outWhite", (float)v); changed(); }));
        }
        Grid top = DialogBuilder.Form();
        DialogBuilder.AddRow(top, "Channel:", DialogBuilder.Combo([("RGB", "rgb"), ("Red", "r"), ("Green", "g"), ("Blue", "b")], channel, v => { channel = v; Build(); }));
        Button auto = new() { Content = "Auto" };
        auto.Click += (_, _) =>
        {
            if (source == null) return;
            AdjustmentSettings computed = Adjustments.AutoLevels(source, true);
            foreach (var pair in computed.Values) s.Values[pair.Key] = pair.Value;
            Build();
            changed();
        };
        root.Children.Add(top);
        root.Children.Add(histogram);
        root.Children.Add(inputs);
        root.Children.Add(auto);
        Build();
        return root;
    }

    private static Control CurvesEditor(AdjustmentSettings s, Action changed, SKBitmap? source)
    {
        int[] histogram = source != null ? Adjustments.Histogram(source)[0] : new int[256];
        CurveControl curve = new(s, 0, histogram, changed) { Width = 256, Height = 256 };
        StackPanel root = new() { Spacing = 6 };
        Grid top = DialogBuilder.Form();
        DialogBuilder.AddRow(top, "Channel:", DialogBuilder.Combo([("RGB", 0), ("Red", 1), ("Green", 2), ("Blue", 3)], 0, v => curve.Channel = v));
        Button reset = new() { Content = "Reset Channel" };
        reset.Click += (_, _) => curve.ResetChannel();
        root.Children.Add(top);
        root.Children.Add(new Border { BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1), Child = curve, HorizontalAlignment = HorizontalAlignment.Left });
        root.Children.Add(new TextBlock { Text = "Click to add points, drag to adjust, drag a point off the graph to delete it.", FontSize = 11, TextWrapping = TextWrapping.Wrap });
        root.Children.Add(reset);
        return root;
    }
}

public sealed class HistogramControl : Control
{
    private int[] _values;

    public HistogramControl(int[] values)
    {
        _values = values;
        MinWidth = 256;
    }

    public int[] Values
    {
        get => _values;
        set
        {
            _values = value;
            InvalidateVisual();
        }
    }

    public override void Render(DrawingContext context)
    {
        context.FillRectangle(new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x20)), new Rect(Bounds.Size));
        double max = Math.Max(1, _values.OrderByDescending(v => v).Skip(2).FirstOrDefault());
        double barWidth = Bounds.Width / 256;
        IBrush brush = new SolidColorBrush(Color.FromRgb(0xB0, 0xB0, 0xB0));
        for (int i = 0; i < 256; i++)
        {
            double h = Math.Min(1, _values[i] / max) * Bounds.Height;
            context.FillRectangle(brush, new Rect(i * barWidth, Bounds.Height - h, Math.Max(1, barWidth), h));
        }
    }
}

/// <summary>Interactive curve editor with points in 0..255 space.</summary>
public sealed class CurveControl : Control
{
    private readonly AdjustmentSettings _settings;
    private readonly int[] _histogram;
    private readonly Action _changed;
    private int _channel;
    private int _dragIndex = -1;

    public CurveControl(AdjustmentSettings settings, int channel, int[] histogram, Action changed)
    {
        _settings = settings;
        _channel = channel;
        _histogram = histogram;
        _changed = changed;
        for (int i = 0; i < 4; i++)
        {
            if (_settings.Curves[i] == null || _settings.Curves[i].Count < 2) _settings.Curves[i] = [new SKPoint(0, 0), new SKPoint(255, 255)];
        }
        Cursor = new Cursor(StandardCursorType.Cross);
    }

    public int Channel
    {
        get => _channel;
        set
        {
            _channel = value;
            InvalidateVisual();
        }
    }

    private List<SKPoint> Points => _settings.Curves[_channel];

    public void ResetChannel()
    {
        _settings.Curves[_channel] = [new SKPoint(0, 0), new SKPoint(255, 255)];
        InvalidateVisual();
        _changed();
    }

    private Point ToScreen(SKPoint p) => new(p.X / 255 * Bounds.Width, Bounds.Height - p.Y / 255 * Bounds.Height);

    private SKPoint ToCurve(Point p) => new((float)Math.Clamp(p.X / Bounds.Width * 255, 0, 255), (float)Math.Clamp((Bounds.Height - p.Y) / Bounds.Height * 255, 0, 255));

    public override void Render(DrawingContext context)
    {
        context.FillRectangle(new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E)), new Rect(Bounds.Size));
        double max = Math.Max(1, _histogram.OrderByDescending(v => v).Skip(2).FirstOrDefault());
        IBrush histogramBrush = new SolidColorBrush(Color.FromArgb(80, 200, 200, 200));
        for (int i = 0; i < 256; i++)
        {
            double h = Math.Min(1, _histogram[i] / max) * Bounds.Height;
            context.FillRectangle(histogramBrush, new Rect(i * Bounds.Width / 256, Bounds.Height - h, Bounds.Width / 256 + 0.5, h));
        }
        Pen grid = new(new SolidColorBrush(Color.FromArgb(60, 255, 255, 255)), 1);
        for (int i = 1; i < 4; i++)
        {
            context.DrawLine(grid, new Point(Bounds.Width * i / 4, 0), new Point(Bounds.Width * i / 4, Bounds.Height));
            context.DrawLine(grid, new Point(0, Bounds.Height * i / 4), new Point(Bounds.Width, Bounds.Height * i / 4));
        }
        context.DrawLine(grid, new Point(0, Bounds.Height), new Point(Bounds.Width, 0));
        float[] lut = Adjustments.CurveLut(Points);
        IBrush color = _channel switch { 1 => Brushes.Red, 2 => Brushes.LimeGreen, 3 => Brushes.DodgerBlue, _ => Brushes.White };
        Pen pen = new(color, 1.5);
        for (int x = 1; x < 256; x++)
        {
            context.DrawLine(pen, ToScreen(new SKPoint(x - 1, lut[x - 1] * 255)), ToScreen(new SKPoint(x, lut[x] * 255)));
        }
        foreach (SKPoint p in Points) context.DrawRectangle(Brushes.White, new Pen(Brushes.Black, 1), new Rect(ToScreen(p) - new Point(4, 4), new Size(8, 8)));
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Point p = e.GetPosition(this);
        _dragIndex = Points.FindIndex(pt => Math.Abs(ToScreen(pt).X - p.X) < 7 && Math.Abs(ToScreen(pt).Y - p.Y) < 7);
        if (_dragIndex < 0)
        {
            SKPoint added = ToCurve(p);
            Points.Add(added);
            Points.Sort((a, b) => a.X.CompareTo(b.X));
            _dragIndex = Points.IndexOf(added);
            _changed();
        }
        e.Pointer.Capture(this);
        InvalidateVisual();
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_dragIndex < 0) return;
        Point p = e.GetPosition(this);
        bool outside = p.X < -20 || p.Y < -20 || p.X > Bounds.Width + 20 || p.Y > Bounds.Height + 20;
        if (outside && Points.Count > 2 && _dragIndex > 0 && _dragIndex < Points.Count - 1)
        {
            Points.RemoveAt(_dragIndex);
            _dragIndex = -1;
        }
        else
        {
            SKPoint c = ToCurve(p);
            float minX = _dragIndex > 0 ? Points[_dragIndex - 1].X + 1 : 0;
            float maxX = _dragIndex < Points.Count - 1 ? Points[_dragIndex + 1].X - 1 : 255;
            Points[_dragIndex] = new SKPoint(Math.Clamp(c.X, minX, maxX), c.Y);
        }
        InvalidateVisual();
        _changed();
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        _dragIndex = -1;
        e.Pointer.Capture(null);
    }
}

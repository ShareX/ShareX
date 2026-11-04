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
using Avalonia.Layout;
using Avalonia.Media;

namespace ShareX.PhotoEditor.UI.Dialogs;

/// <summary>Parameter editor of the HDR Toning adjustment.</summary>
public sealed class HdrToningEditor : UserControl
{
    private const int MaxEditorHeight = 640;

    private readonly AdjustmentSettings _settings;
    private readonly Action _changed;
    private readonly int[] _histogram;
    private readonly StackPanel _body = new() { Spacing = 8 };

    public HdrToningEditor(AdjustmentSettings settings, Action changed, SKBitmap? source)
    {
        _settings = settings;
        _changed = changed;
        _histogram = source != null ? Adjustments.Histogram(source)[0] : new int[256];

        Grid top = DialogBuilder.Form();
        DialogBuilder.AddRow(top, "Preset:", DialogBuilder.Combo(HdrToning.Presets.Select((preset, index) => (preset.Name, index)), 0, index =>
        {
            HdrToning.ApplyPreset(_settings, index);
            Rebuild();
            _changed();
        }));
        DialogBuilder.AddRow(top, "Method:", DialogBuilder.Combo(
        [
            ("Local Adaptation", HdrMethod.LocalAdaptation),
            ("Equalize Histogram", HdrMethod.EqualizeHistogram),
            ("Exposure and Gamma", HdrMethod.ExposureAndGamma),
            ("Highlight Compression", HdrMethod.HighlightCompression)
        ], (HdrMethod)(int)settings.Get("method"), method =>
        {
            _settings.Set("method", (float)method);
            Rebuild();
            _changed();
        }));

        StackPanel root = new() { Spacing = 8 };
        root.Children.Add(top);
        root.Children.Add(new Border { Classes = { "hsep" } });
        root.Children.Add(_body);
        Content = new ScrollViewer { Content = root, MaxHeight = MaxEditorHeight, HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled };
        Rebuild();
    }

    private void Rebuild()
    {
        _body.Children.Clear();
        switch ((HdrMethod)(int)_settings.Get("method"))
        {
            case HdrMethod.LocalAdaptation:
                BuildLocalAdaptation();
                break;
            case HdrMethod.ExposureAndGamma:
                Grid form = DialogBuilder.Form();
                AddSlider(form, "Exposure:", "globalExposure", -20, 20, 0, 0.01, "0.00");
                AddSlider(form, "Gamma:", "globalGamma", 0.01, 9.99, 1, 0.01, "0.00");
                _body.Children.Add(form);
                break;
            default:
                _body.Children.Add(new TextBlock { Text = "This method has no options.", Classes = { "dim" } });
                break;
        }
    }

    private void BuildLocalAdaptation()
    {
        Grid glow = Section("Edge Glow");
        AddSlider(glow, "Radius:", "radius", 1, 500, 300);
        AddSlider(glow, "Strength:", "strength", 0.1, 4, 0.52, 0.01, "0.00");
        CheckBox smooth = new() { Content = "Smooth Edges", IsChecked = _settings.Get("smoothEdges") > 0 };
        smooth.IsCheckedChanged += (_, _) =>
        {
            _settings.Set("smoothEdges", smooth.IsChecked == true ? 1 : 0);
            _changed();
        };
        DialogBuilder.AddFull(glow, smooth);

        Grid tone = Section("Tone and Detail");
        AddSlider(tone, "Gamma:", "gamma", 0.1, 2, 1, 0.01, "0.00");
        AddSlider(tone, "Exposure:", "exposure", -5, 5, 0, 0.01, "0.00");
        AddSlider(tone, "Detail:", "detail", -100, 300, 30);

        Grid advanced = Section("Advanced");
        AddSlider(advanced, "Shadow:", "shadow", -100, 100, 0);
        AddSlider(advanced, "Highlight:", "highlight", -100, 100, 0);
        AddSlider(advanced, "Vibrance:", "vibrance", -100, 100, 0);
        AddSlider(advanced, "Saturation:", "saturation", -100, 100, 20);

        CurveControl curve = new(_settings, 0, _histogram, _changed) { Width = 256, Height = 256 };
        Button reset = new() { Content = "Reset Curve" };
        reset.Click += (_, _) => curve.ResetChannel();
        StackPanel curvePanel = new() { Spacing = 6 };
        curvePanel.Children.Add(new Border { BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1), Child = curve, HorizontalAlignment = HorizontalAlignment.Left });
        curvePanel.Children.Add(reset);
        _body.Children.Add(new Expander { Header = "Toning Curve and Histogram", Content = curvePanel, HorizontalAlignment = HorizontalAlignment.Stretch });
    }

    private Grid Section(string title)
    {
        _body.Children.Add(new TextBlock { Text = title, Classes = { "header" } });
        Grid form = DialogBuilder.Form();
        _body.Children.Add(form);
        return form;
    }

    private void AddSlider(Grid form, string label, string key, double minimum, double maximum, double defaultValue, double step = 1, string format = "0")
    {
        DialogBuilder.AddRow(form, label, DialogBuilder.SliderNumber(minimum, maximum, _settings.Get(key, (float)defaultValue), v =>
        {
            _settings.Set(key, (float)v);
            _changed();
        }, step, format));
    }
}

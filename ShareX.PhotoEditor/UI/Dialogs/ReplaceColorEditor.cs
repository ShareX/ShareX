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
using Avalonia.Media.Imaging;
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Panels;

namespace ShareX.PhotoEditor.UI.Dialogs;

/// <summary>Parameter editor of the Replace Color adjustment: samples colors from a preview and shifts their hue, saturation and lightness.</summary>
public sealed class ReplaceColorEditor : UserControl
{
    private const int PreviewSize = 240;

    private enum SampleMode
    {
        Replace,
        Add,
        Subtract
    }

    private readonly AdjustmentSettings _settings;
    private readonly Action _changed;
    private readonly SKBitmap? _source;
    private readonly IEditorHost? _host;
    private readonly SKBitmap? _thumbnail;
    private readonly Image _preview = new() { Stretch = Stretch.None };
    private readonly Border _sampleSwatch = Swatch();
    private readonly Border _resultSwatch = Swatch();
    private readonly List<(Button Button, SampleMode Mode)> _modeButtons = [];
    private WriteableBitmap? _shown;
    private SampleMode _mode = SampleMode.Replace;
    private bool _showImage;

    public ReplaceColorEditor(AdjustmentSettings settings, Action changed, SKBitmap? source, IEditorHost? host)
    {
        _settings = settings;
        _changed = changed;
        _source = source;
        _host = host;
        if (settings.SampleColors.Count == 0) settings.SampleColors.Add(host?.Foreground ?? SKColors.Black);

        StackPanel root = new() { Spacing = 8 };
        root.Children.Add(new TextBlock { Text = Strings.ReplaceColorEditor_Selection, Classes = { "header" } });
        root.Children.Add(BuildSampleRow());
        Grid selection = DialogBuilder.Form();
        AddSlider(selection, Strings.EditorCommands_FuzzinessColon, "fuzziness", 0, 200, 40);
        root.Children.Add(selection);
        if (source != null)
        {
            float scale = Math.Min(1, PreviewSize / (float)Math.Max(source.Width, source.Height));
            _thumbnail = ImageOps.Resize(source, Math.Max(1, (int)(source.Width * scale)), Math.Max(1, (int)(source.Height * scale)), ResampleMethod.Bilinear);
            root.Children.Add(BuildPreview());
        }
        root.Children.Add(new Border { Classes = { "hsep" } });
        root.Children.Add(new TextBlock { Text = Strings.ReplaceColorEditor_Replacement, Classes = { "header" } });
        Grid replacement = DialogBuilder.Form();
        AddSlider(replacement, Strings.AdjustmentEditor_HueColon, "hue", -180, 180, 0);
        AddSlider(replacement, Strings.AdjustmentEditor_SaturationColon, "saturation", -100, 100, 0);
        AddSlider(replacement, Strings.AdjustmentEditor_LightnessColon, "lightness", -100, 100, 0);
        DialogBuilder.AddRow(replacement, Strings.ReplaceColorEditor_ResultColon, _resultSwatch);
        root.Children.Add(replacement);
        Content = root;
        RefreshVisuals();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _thumbnail?.Dispose();
        _preview.Source = null;
        _shown?.Dispose();
        _shown = null;
    }

    private static Border Swatch() => new() { Width = 40, Height = 18, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1), HorizontalAlignment = HorizontalAlignment.Left };

    private void AddSlider(Grid form, string label, string key, double minimum, double maximum, double defaultValue)
    {
        DialogBuilder.AddRow(form, label, DialogBuilder.SliderNumber(minimum, maximum, _settings.Get(key, (float)defaultValue), v =>
        {
            _settings.Set(key, (float)v);
            Update();
        }));
    }

    private Control BuildSampleRow()
    {
        Button swatchButton = new() { Content = _sampleSwatch, Padding = new Thickness(2) };
        ToolTip.SetTip(swatchButton, Strings.ReplaceColorEditor_SelectColorTip);
        swatchButton.Click += async (_, _) =>
        {
            if (TopLevel.GetTopLevel(swatchButton) is not Window owner) return;
            SKColor? color = await ColorPickerDialog.ShowAsync(owner, _settings.SampleColors[^1], Strings.EditorCommands_Color, _host);
            if (color is SKColor picked) ApplySample(picked, SampleMode.Replace);
        };
        StackPanel row = new() { Orientation = Orientation.Horizontal, Spacing = 4 };
        row.Children.Add(ModeButton(Strings.ReplaceColorEditor_EyedropperTip, null, SampleMode.Replace));
        row.Children.Add(ModeButton(Strings.ReplaceColorEditor_AddToSampleTip, "+", SampleMode.Add));
        row.Children.Add(ModeButton(Strings.ReplaceColorEditor_SubtractFromSampleTip, "-", SampleMode.Subtract));
        row.Children.Add(new TextBlock { Text = Strings.EditorCommands_ColorColon, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(12, 0, 2, 0) });
        row.Children.Add(swatchButton);
        return row;
    }

    private Button ModeButton(string tip, string? suffix, SampleMode mode)
    {
        StackPanel content = new() { Orientation = Orientation.Horizontal };
        content.Children.Add(new IconView(PhotoEditorIcons.Eyedropper, 16));
        if (suffix != null) content.Children.Add(new TextBlock { Text = suffix, VerticalAlignment = VerticalAlignment.Center });
        Button button = new() { Classes = { "icon" }, Content = content };
        ToolTip.SetTip(button, tip);
        button.Click += (_, _) =>
        {
            _mode = mode;
            RefreshModeButtons();
        };
        _modeButtons.Add((button, mode));
        return button;
    }

    private void RefreshModeButtons()
    {
        foreach ((Button button, SampleMode mode) in _modeButtons) button.Classes.Set("checked", mode == _mode);
    }

    private Control BuildPreview()
    {
        Border frame = new()
        {
            Child = _preview,
            Background = Brushes.Black,
            Width = _thumbnail!.Width,
            Height = _thumbnail.Height,
            HorizontalAlignment = HorizontalAlignment.Center,
            Cursor = new Cursor(StandardCursorType.Cross)
        };
        ToolTip.SetTip(frame, Strings.ReplaceColorEditor_ClickToSample);
        frame.PointerPressed += (_, e) =>
        {
            Point position = e.GetPosition(frame);
            int x = Math.Clamp((int)(position.X / frame.Bounds.Width * _source!.Width), 0, _source.Width - 1);
            int y = Math.Clamp((int)(position.Y / frame.Bounds.Height * _source.Height), 0, _source.Height - 1);
            SampleMode mode = e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? SampleMode.Add : e.KeyModifiers.HasFlag(KeyModifiers.Alt) ? SampleMode.Subtract : _mode;
            ApplySample(_source.GetPixel(x, y).WithAlpha(255), mode);
            e.Handled = true;
        };

        RadioButton selection = new() { Content = Strings.ReplaceColorEditor_Selection, GroupName = "ReplaceColorPreview", IsChecked = true };
        RadioButton image = new() { Content = Strings.ReplaceColorEditor_Image, GroupName = "ReplaceColorPreview" };
        image.IsCheckedChanged += (_, _) =>
        {
            _showImage = image.IsChecked == true;
            RefreshPreview();
        };
        StackPanel radios = new() { Orientation = Orientation.Horizontal, Spacing = 16, HorizontalAlignment = HorizontalAlignment.Center };
        radios.Children.Add(selection);
        radios.Children.Add(image);

        StackPanel panel = new() { Spacing = 6 };
        panel.Children.Add(frame);
        panel.Children.Add(radios);
        return panel;
    }

    private void ApplySample(SKColor color, SampleMode mode)
    {
        switch (mode)
        {
            case SampleMode.Add:
                _settings.ExcludedColors.Remove(color);
                _settings.SampleColors.Add(color);
                break;
            case SampleMode.Subtract:
                _settings.SampleColors.Remove(color);
                if (_settings.SampleColors.Count == 0) _settings.SampleColors.Add(color);
                else _settings.ExcludedColors.Add(color);
                break;
            default:
                _settings.SampleColors.Clear();
                _settings.ExcludedColors.Clear();
                _settings.SampleColors.Add(color);
                break;
        }
        Update();
    }

    private void Update()
    {
        RefreshVisuals();
        _changed();
    }

    private void RefreshVisuals()
    {
        SKColor sample = _settings.SampleColors[^1];
        _sampleSwatch.Background = DialogBuilder.ToBrush(sample);
        float r = sample.Red / 255f, g = sample.Green / 255f, b = sample.Blue / 255f;
        Adjustments.ShiftHsl(ref r, ref g, ref b, _settings.Get("hue"), _settings.Get("saturation") / 100f, _settings.Get("lightness") / 100f);
        _resultSwatch.Background = DialogBuilder.ToBrush(new SKColor(ToByte(r), ToByte(g), ToByte(b)));
        RefreshModeButtons();
        RefreshPreview();
    }

    private static byte ToByte(float value) => (byte)Math.Clamp(value * 255 + 0.5f, 0, 255);

    private void RefreshPreview()
    {
        if (_thumbnail == null) return;
        WriteableBitmap? previous = _shown;
        if (_showImage)
        {
            _shown = BitmapHelper.ToAvalonia(_thumbnail);
        }
        else
        {
            using SKBitmap mask = CreateMask(_thumbnail);
            _shown = BitmapHelper.ToAvalonia(mask);
        }
        _preview.Source = _shown;
        previous?.Dispose();
    }

    /// <summary>Renders the affected area as grayscale: white is fully replaced, black is untouched.</summary>
    private unsafe SKBitmap CreateMask(SKBitmap image)
    {
        SKBitmap mask = ImageOps.CreateBitmap(image.Width, image.Height);
        SKColor[] samples = _settings.SampleColors.ToArray();
        SKColor[] excluded = _settings.ExcludedColors.ToArray();
        float range = Math.Max(1, _settings.Get("fuzziness", 40));
        uint* source = (uint*)image.GetPixels();
        uint* target = (uint*)mask.GetPixels();
        int sourceStride = image.RowBytes / 4, targetStride = mask.RowBytes / 4;
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                uint pixel = source[y * sourceStride + x];
                float coverage = 0;
                if ((pixel >> 24) != 0)
                {
                    ColorMath.Unpack(pixel, out float r, out float g, out float b, out _);
                    coverage = Adjustments.ReplaceCoverage(r, g, b, samples, excluded, range);
                }
                uint gray = (uint)(coverage * 255);
                target[y * targetStride + x] = 0xFF000000 | gray << 16 | gray << 8 | gray;
            }
        }
        return mask;
    }
}

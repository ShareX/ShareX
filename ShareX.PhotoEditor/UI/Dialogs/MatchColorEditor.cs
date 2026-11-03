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

using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Panels;

namespace ShareX.PhotoEditor.UI.Dialogs;

/// <summary>Parameter editor of the Match Color adjustment: measures the target and an optional source and stores their statistics in the settings.</summary>
public sealed class MatchColorEditor : UserControl
{
    private const int PreviewSize = 128;

    private readonly AdjustmentSettings _settings;
    private readonly Action _changed;
    private readonly SKBitmap? _target;
    private readonly PhotoDocument? _document;
    private readonly Layer? _targetLayer;
    private readonly ContentControl _layerHost = new();
    private readonly Image _sourcePreview = new() { Stretch = Stretch.Uniform, HorizontalAlignment = HorizontalAlignment.Center, IsVisible = false };
    private DocumentView? _sourceView;
    private Layer? _sourceLayer;
    private bool _useSourceSelection;
    private bool _useTargetSelection;

    public MatchColorEditor(AdjustmentSettings settings, Action changed, SKBitmap? target, IEditorHost? host)
    {
        _settings = settings;
        _changed = changed;
        _target = target;
        _document = host?.Document;
        _targetLayer = _document?.ActiveLayer;

        StackPanel root = new() { Spacing = 8 };
        root.Children.Add(new TextBlock { Text = "Destination Image", Classes = { "header" } });
        Grid destination = DialogBuilder.Form();
        DialogBuilder.AddRow(destination, "Target:", new TextBlock
        {
            Text = $"{_document?.DisplayName} ({_targetLayer?.Name}, RGB/8)",
            VerticalAlignment = VerticalAlignment.Center,
            TextTrimming = TextTrimming.CharacterEllipsis
        });
        root.Children.Add(destination);

        root.Children.Add(new Border { Classes = { "hsep" } });
        root.Children.Add(new TextBlock { Text = "Image Options", Classes = { "header" } });
        Grid options = DialogBuilder.Form();
        AddSlider(options, "Luminance:", "luminance", 1, 200, 100);
        AddSlider(options, "Color Intensity:", "intensity", 1, 200, 100);
        AddSlider(options, "Fade:", "fade", 0, 100, 0);
        CheckBox neutralize = new() { Content = "Neutralize", IsChecked = settings.Get("neutralize") > 0 };
        neutralize.IsCheckedChanged += (_, _) =>
        {
            settings.Set("neutralize", neutralize.IsChecked == true ? 1 : 0);
            changed();
        };
        DialogBuilder.AddFull(options, neutralize);
        root.Children.Add(options);

        root.Children.Add(new Border { Classes = { "hsep" } });
        root.Children.Add(new TextBlock { Text = "Image Statistics", Classes = { "header" } });
        root.Children.Add(BuildStatistics(host as PhotoEditorWindow));
        Content = root;
        MeasureTarget();
    }

    private void AddSlider(Grid form, string label, string key, double minimum, double maximum, double defaultValue)
    {
        DialogBuilder.AddRow(form, label, DialogBuilder.SliderNumber(minimum, maximum, _settings.Get(key, (float)defaultValue), v =>
        {
            _settings.Set(key, (float)v);
            _changed();
        }));
    }

    private Control BuildStatistics(PhotoEditorWindow? editor)
    {
        CheckBox useSource = new() { Content = "Use Selection in Source to Calculate Colors", IsEnabled = false };
        useSource.IsCheckedChanged += (_, _) =>
        {
            _useSourceSelection = useSource.IsChecked == true;
            MeasureSource();
            _changed();
        };
        CheckBox useTarget = new() { Content = "Use Selection in Target to Calculate Adjustment", IsEnabled = _document?.Selection != null };
        useTarget.IsCheckedChanged += (_, _) =>
        {
            _useTargetSelection = useTarget.IsChecked == true;
            MeasureTarget();
            _changed();
        };

        List<(string Text, DocumentView? Value)> documents = [("None", null)];
        if (editor != null) documents.AddRange(editor.Views.Select(view => (view.Document.DisplayName, (DocumentView?)view)));
        ComboBox source = DialogBuilder.Combo(documents, null, view =>
        {
            _sourceView = view;
            _sourceLayer = null;
            useSource.IsEnabled = view?.Document.Selection != null;
            RebuildLayers();
            MeasureSource();
            _changed();
        });
        RebuildLayers();

        Grid form = DialogBuilder.Form();
        DialogBuilder.AddFull(form, useSource);
        DialogBuilder.AddFull(form, useTarget);
        DialogBuilder.AddRow(form, "Source:", source);
        DialogBuilder.AddRow(form, "Layer:", _layerHost);
        DialogBuilder.AddFull(form, _sourcePreview);
        return form;
    }

    private void RebuildLayers()
    {
        List<(string Text, Layer? Value)> layers = [("Merged", null)];
        PhotoDocument? document = _sourceView?.Document;
        if (document != null)
        {
            // Layers list top-down; the target layer itself is changing under the live preview.
            layers.AddRange(document.AllLayers.Where(l => l is not (GroupLayer or AdjustmentLayer) && l != _targetLayer).Reverse().Select(l => (l.Name, (Layer?)l)));
        }
        ComboBox combo = DialogBuilder.Combo(layers, null, layer =>
        {
            _sourceLayer = layer;
            MeasureSource();
            _changed();
        });
        combo.IsEnabled = document != null;
        _layerHost.Content = combo;
    }

    private void MeasureTarget()
    {
        if (_target == null) return;
        bool selection = _useTargetSelection && _document?.Selection != null && _target.Width == _document.Width && _target.Height == _document.Height;
        ColorMatch.Statistics? statistics = ColorMatch.Measure(_target, selection ? _document!.SelectionAt : null);
        if (statistics is ColorMatch.Statistics measured) ColorMatch.Store(_settings, "target", measured);
    }

    private void MeasureSource()
    {
        PhotoDocument? document = _sourceView?.Document;
        _settings.Set("hasSource", 0);
        _sourcePreview.IsVisible = false;
        if (document == null) return;
        using SKBitmap bitmap = _sourceLayer != null ? Compositor.RenderLayer(document, _sourceLayer) : Compositor.Flatten(document);
        ColorMatch.Statistics? statistics = ColorMatch.Measure(bitmap, _useSourceSelection && document.Selection != null ? document.SelectionAt : null);
        if (statistics is ColorMatch.Statistics measured)
        {
            ColorMatch.Store(_settings, "source", measured);
            _settings.Set("hasSource", 1);
        }
        float scale = Math.Min(1, PreviewSize / (float)Math.Max(bitmap.Width, bitmap.Height));
        using SKBitmap thumbnail = ImageOps.Resize(bitmap, Math.Max(1, (int)(bitmap.Width * scale)), Math.Max(1, (int)(bitmap.Height * scale)), ResampleMethod.Bilinear);
        (_sourcePreview.Source as WriteableBitmap)?.Dispose();
        _sourcePreview.Source = BitmapHelper.ToAvalonia(thumbnail);
        _sourcePreview.Width = thumbnail.Width;
        _sourcePreview.Height = thumbnail.Height;
        _sourcePreview.IsVisible = true;
    }
}

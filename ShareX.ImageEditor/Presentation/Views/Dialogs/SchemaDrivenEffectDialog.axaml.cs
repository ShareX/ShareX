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
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Localization;
using ShareX.ImageEditor.Presentation.Effects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace ShareX.ImageEditor.Presentation.Views.Dialogs;

public partial class SchemaDrivenEffectDialog : UserControl, IEffectDialog
{
    private const int MaxParameterSnapshotChars = 400;

    private bool _isReady;

    private readonly List<Action> _visualDebugUnsubscribes = [];

    public EffectDefinition Definition { get; }

    public ObservableCollection<EffectParameterState> ParameterStates { get; }

    public string Title => EffectBrowserLocalization.GetEffectName(Definition.Id, Definition.Name);

    public event EventHandler<EffectEventArgs>? ApplyRequested;

    public event EventHandler<EffectEventArgs>? PreviewRequested;

    public event EventHandler? CancelRequested;

    public SchemaDrivenEffectDialog()
        : this(ImageEffectCatalog.Definitions[0])
    {
    }

    public SchemaDrivenEffectDialog(EffectDefinition definition)
    {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        IEnumerable<EffectParameterState> parameterStates = definition.CoreParameters.Count > 0
            ? definition.CoreParameters.Select(EffectParameterState.Create)
            : definition.Parameters.Select(EffectParameterState.Create);

        ParameterStates = new ObservableCollection<EffectParameterState>(parameterStates);

        foreach (EffectParameterState parameterState in ParameterStates)
        {
            parameterState.PropertyChanged += OnParameterStateChanged;
        }

        AvaloniaXamlLoader.Load(this);
        DataContext = this;

        AttachedToVisualTree += OnAttachedToVisualTree;
        DetachedFromVisualTree += OnDetachedFromVisualTree;

        // Handle Esc even when a TextBox inside the dialog has focus (handledEventsToo: true
        // ensures this fires even after a child TextBox marks the KeyUp event as handled).
        AddHandler(KeyUpEvent, OnEscapeKeyUp, RoutingStrategies.Bubble, handledEventsToo: true);

        EditorServices.ReportDebug(
            nameof(SchemaDrivenEffectDialog),
            $"Constructed effectId={Definition.Id} name={Definition.Name} category={Definition.Category} paramCount={ParameterStates.Count}");
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        object? dcBefore = DataContext;
        string beforeType = dcBefore?.GetType().Name ?? "null";
        bool selfBefore = ReferenceEquals(dcBefore, this);

        // ContentControl's ContentPresenter can replace an inherited DataContext with the parent's
        // (e.g. MainViewModel). Parameter bindings then fail silently and preview/apply keep defaults.
        // Old bespoke dialogs used FindControl and did not depend on DataContext; schema-driven UI does.
        DataContext = this;
        _isReady = true;

        EditorServices.ReportDebug(
            nameof(SchemaDrivenEffectDialog),
            $"AttachedToVisualTree effectId={Definition.Id} DataContextBefore={beforeType} ReferenceEquals(self)={selfBefore} AfterSet ReferenceEquals(self)={ReferenceEquals(DataContext, this)} snapshot={TruncateSnapshot(BuildParameterSnapshot())}");

        Dispatcher.UIThread.Post(AttachVisualBindingDiagnostics, DispatcherPriority.Loaded);

        RequestPreview();
    }

    private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        _isReady = false;
        foreach (Action unsubscribe in _visualDebugUnsubscribes)
        {
            unsubscribe();
        }

        _visualDebugUnsubscribes.Clear();
    }

    private void OnParameterStateChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is EffectParameterState state)
        {
            EditorServices.ReportDebug(
                nameof(SchemaDrivenEffectDialog),
                $"ParameterState PropertyChanged effectId={Definition.Id} key={state.Key} prop={e.PropertyName ?? "(null)"}");
        }

        if (!_isReady)
        {
            return;
        }

        RequestPreview();
    }

    private void AttachVisualBindingDiagnostics()
    {
        if (!_isReady || !this.IsAttachedToVisualTree())
        {
            return;
        }

        int sliderIndex = 0;
        foreach (Slider slider in this.GetVisualDescendants().OfType<Slider>())
        {
            object? dc = slider.DataContext;
            if (dc is not SliderParameterState)
            {
                continue;
            }

            string dcType = dc?.GetType().Name ?? "null";
            bool inCollection = dc is EffectParameterState eps && ParameterStates.Contains(eps);
            string? key = dc is SliderParameterState sps ? sps.Key : null;
            double modelValue = dc is SliderParameterState sps2 ? sps2.Value : double.NaN;

            EditorServices.ReportDebug(
                nameof(SchemaDrivenEffectDialog),
                $"Slider[{sliderIndex}] effectId={Definition.Id} DataContextType={dcType} key={key ?? "?"} inParameterStates={inCollection} controlValue={slider.Value.ToString("0.##", CultureInfo.InvariantCulture)} modelValue={(double.IsNaN(modelValue) ? "n/a" : modelValue.ToString("0.##", CultureInfo.InvariantCulture))}");

            sliderIndex++;

            void OnSliderPropertyChanged(object? s, AvaloniaPropertyChangedEventArgs ev)
            {
                if (ev.Property != Slider.ValueProperty)
                {
                    return;
                }

                object? d = slider.DataContext;
                string k = d is SliderParameterState sp ? sp.Key : "?";
                double mv = d is SliderParameterState sp2 ? sp2.Value : double.NaN;
                EditorServices.ReportDebug(
                    nameof(SchemaDrivenEffectDialog),
                    $"Slider ValueProperty changed effectId={Definition.Id} key={k} controlValue={slider.Value.ToString("0.##", CultureInfo.InvariantCulture)} modelValue={(double.IsNaN(mv) ? "n/a" : mv.ToString("0.##", CultureInfo.InvariantCulture))}");
            }

            slider.PropertyChanged += OnSliderPropertyChanged;
            _visualDebugUnsubscribes.Add(() => slider.PropertyChanged -= OnSliderPropertyChanged);
        }

        int toggleIndex = 0;
        foreach (ToggleSwitch toggle in this.GetVisualDescendants().OfType<ToggleSwitch>())
        {
            object? dc = toggle.DataContext;
            if (dc is not CheckboxParameterState)
            {
                continue;
            }

            string key = ((CheckboxParameterState)dc).Key;
            EditorServices.ReportDebug(
                nameof(SchemaDrivenEffectDialog),
                $"ToggleSwitch[{toggleIndex}] effectId={Definition.Id} key={key} inParameterStates={ParameterStates.Contains((EffectParameterState)dc)} IsChecked={toggle.IsChecked}");

            toggleIndex++;

            void OnCheckPropertyChanged(object? s, AvaloniaPropertyChangedEventArgs ev)
            {
                if (ev.Property.Name != nameof(ToggleSwitch.IsChecked))
                {
                    return;
                }

                object? d = toggle.DataContext;
                string k = d is CheckboxParameterState cb ? cb.Key : "?";
                bool mv = d is CheckboxParameterState cb2 && cb2.Value;
                EditorServices.ReportDebug(
                    nameof(SchemaDrivenEffectDialog),
                    $"ToggleSwitch IsChecked changed effectId={Definition.Id} key={k} controlIsChecked={toggle.IsChecked} modelValue={mv}");
            }

            toggle.PropertyChanged += OnCheckPropertyChanged;
            _visualDebugUnsubscribes.Add(() => toggle.PropertyChanged -= OnCheckPropertyChanged);
        }
    }

    private void RequestPreview()
    {
        EditorServices.ReportDebug(
            nameof(SchemaDrivenEffectDialog),
            $"PreviewRequested effectId={Definition.Id} params={TruncateSnapshot(BuildParameterSnapshot())}");

        PreviewRequested?.Invoke(this, BuildEffectEventArgs(Definition.Name));
    }

    private void OnApplyClick(object? sender, RoutedEventArgs e)
    {
        EditorServices.ReportDebug(
            nameof(SchemaDrivenEffectDialog),
            $"ApplyClicked effectId={Definition.Id} params={TruncateSnapshot(BuildParameterSnapshot())}");

        ApplyRequested?.Invoke(this, BuildEffectEventArgs($"Applied {Definition.Name}"));
    }

    private void OnEscapeKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && e.KeyModifiers == KeyModifiers.None)
        {
            CancelRequested?.Invoke(this, EventArgs.Empty);
            e.Handled = true;
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CancelRequested?.Invoke(this, EventArgs.Empty);
    }

    private EffectEventArgs BuildEffectEventArgs(string statusMessage)
    {
        return new EffectEventArgs(
            img => Definition.CreateConfiguredEffect(ParameterStates).Apply(img),
            statusMessage);
    }

    private async void OnBrowseFilePathClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not FilePathParameterState parameterState)
        {
            return;
        }

        TopLevel? topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.StorageProvider == null)
        {
            return;
        }

        IReadOnlyList<FilePickerFileType> fileTypes = ParseFileTypes(parameterState.FileFilter);
        IReadOnlyList<IStorageFile> files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = $"Select {parameterState.Label}",
            AllowMultiple = false,
            FileTypeFilter = fileTypes
        });

        if (files.Count > 0)
        {
            parameterState.Value = files[0].Path.LocalPath;
        }
    }

    private static IReadOnlyList<FilePickerFileType> ParseFileTypes(string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return [FilePickerFileTypes.All];
        }

        string[] parts = filter.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
        {
            return [FilePickerFileTypes.All];
        }

        string[] patterns = parts[1].Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (patterns.Length == 0)
        {
            return [FilePickerFileTypes.All];
        }

        return
        [
            new FilePickerFileType(parts[0])
            {
                Patterns = patterns
            }
        ];
    }

    private static string TruncateSnapshot(string snapshot)
    {
        if (snapshot.Length <= MaxParameterSnapshotChars)
        {
            return snapshot;
        }

        return snapshot[..MaxParameterSnapshotChars] + "…";
    }

    private string BuildParameterSnapshot()
    {
        if (ParameterStates.Count == 0)
        {
            return "(no parameters)";
        }

        var sb = new StringBuilder();
        foreach (EffectParameterState state in ParameterStates)
        {
            if (sb.Length > 0)
            {
                sb.Append("; ");
            }

            sb.Append(state.Key);
            sb.Append('=');
            switch (state)
            {
                case SliderParameterState slider:
                    sb.Append(slider.Value.ToString("0.##", CultureInfo.InvariantCulture));
                    break;
                case CheckboxParameterState check:
                    sb.Append(check.Value ? "true" : "false");
                    break;
                case EnumParameterState en:
                    sb.Append(en.SelectedOption.Label);
                    break;
                case ColorParameterState color:
                    sb.Append(color.Value.ToString());
                    break;
                case NumericParameterState num:
                    sb.Append(num.Value?.ToString(CultureInfo.InvariantCulture) ?? "null");
                    break;
                case TextParameterState text:
                    sb.Append('"');
                    sb.Append(text.Value.Length > 40 ? text.Value[..40] + "…" : text.Value);
                    sb.Append('"');
                    break;
                case FilePathParameterState path:
                    string p = path.Value;
                    sb.Append('"');
                    sb.Append(p.Length > 48 ? "…" + p[^48..] : p);
                    sb.Append('"');
                    break;
                default:
                    sb.Append(state.GetType().Name);
                    break;
            }
        }

        return sb.ToString();
    }
}

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
using ShareX.ImageEditor.Core.ImageEffects;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PhotoEditor.UI.Commands;

public static partial class EditorCommands
{
    private static readonly Lazy<IReadOnlyList<ImageEffectBase>> LibraryEffects = new(() =>
        typeof(ImageEffectBase).Assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsClass: true } && typeof(ImageEffectBase).IsAssignableFrom(t) && t.GetConstructor(Type.EmptyTypes) != null)
            .Select(t => { try { return Activator.CreateInstance(t) as ImageEffectBase; } catch { return null; } })
            .Where(e => e != null)
            .Select(e => e!)
            .OrderBy(e => e.Category).ThenBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray());

    private static void RegisterFilter()
    {
        R("filter.last", "Last Filter", "Ctrl+Alt+F", w => w.RepeatLastFilter?.Invoke(), w => w.RepeatLastFilter != null && HasDoc(w));
        R("filter.convertSmart", "Convert for Smart Filters", null, ConvertToSmartObject, HasLayer);
        foreach (FilterDefinition filter in Filters.All)
        {
            FilterDefinition captured = filter;
            R($"filter.{filter.Category}.{filter.Name}", filter.Name + (filter.Parameters.Count > 0 ? "..." : string.Empty), null, w => _ = RunFilterAsync(w, captured), HasDoc);
        }
    }

    private static IEnumerable<MenuNode> FilterCategoryNodes() =>
        Filters.All.GroupBy(f => f.Category).Select(g => MenuNode.Sub(g.Key, g.Select(f => MenuNode.Cmd($"filter.{f.Category}.{f.Name}")).ToArray()));

    private static IEnumerable<MenuNode> LibraryNodes(PhotoEditorWindow w) =>
        LibraryEffects.Value.GroupBy(e => e.Category).Select(g => new MenuNode
        {
            Header = OptionsBuilder.SplitWords(g.Key.ToString()),
            Children = g.Select(e => new MenuNode { Header = e.BrowserLabel, Action = win => _ = RunLibraryEffectAsync(win, e.GetType()) }).ToList()
        });

    private static (RasterLayer? Raster, SmartObjectLayer? Smart) FilterTarget(PhotoEditorWindow w)
    {
        if (w.Document?.ActiveLayer is SmartObjectLayer smart) return (null, smart);
        return (EditableRaster(w, "run the filter"), null);
    }

    private static async Task RunFilterAsync(PhotoEditorWindow w, FilterDefinition filter, Dictionary<string, float>? preset = null)
    {
        var (raster, smart) = FilterTarget(w);
        if (raster == null && smart == null) return;
        Dictionary<string, float> values = preset ?? filter.Defaults;
        FilterContext context = new() { Foreground = w.Foreground, Background = w.Background, Seed = Environment.TickCount };
        Func<SKBitmap, SKBitmap> Make()
        {
            Dictionary<string, float> snapshot = new(values);
            return b => filter.Apply(b, snapshot, context);
        }
        if (preset == null && filter.Parameters.Count > 0)
        {
            PreviewSession? session = raster != null ? new PreviewSession(w, raster, 96) : null;
            Grid form = DialogBuilder.Form();
            foreach (FilterParameter p in filter.Parameters)
            {
                if (p.Options != null)
                {
                    DialogBuilder.AddRow(form, p.Label + ":", DialogBuilder.Combo(p.Options.Select((o, i) => (o, (float)i)), values[p.Key], v => { values[p.Key] = v; session?.Preview(Make()); }));
                }
                else
                {
                    DialogBuilder.AddRow(form, p.Label + ":", DialogBuilder.SliderNumber(p.Minimum, p.Maximum, values[p.Key], v => { values[p.Key] = (float)v; session?.Preview(Make()); }, p.Step, p.Step < 1 ? "0.0" : "0"));
                }
            }
            CheckBox preview = new() { Content = "Preview", IsChecked = true };
            preview.IsCheckedChanged += (_, _) =>
            {
                if (session == null) return;
                session.PreviewEnabled = preview.IsChecked == true;
                session.Preview(Make());
            };
            DialogBuilder.AddFull(form, preview);
            session?.Preview(Make());
            bool ok = await DialogBuilder.ShowAsync(w, filter.Name, form, 480);
            if (!ok)
            {
                session?.Cancel();
                return;
            }
            if (session != null)
            {
                await session.CommitAsync(Make(), filter.Name);
                RememberFilter(w, filter, values);
                return;
            }
        }
        if (smart != null)
        {
            AddSmartFilter(w, smart, SmartFilterFactory.CreateFilter(filter, values, context, smart.Filters.Count));
        }
        else
        {
            PreviewSession session = new(w, raster!, 96);
            await session.CommitAsync(Make(), filter.Name);
        }
        RememberFilter(w, filter, values);
    }

    private static void RememberFilter(PhotoEditorWindow w, FilterDefinition filter, Dictionary<string, float> values)
    {
        Dictionary<string, float> copy = new(values);
        w.LastFilterName = filter.Name;
        w.RepeatLastFilter = () => _ = RunFilterAsync(w, filter, copy);
    }

    private static Task LibraryEffectAsync(PhotoEditorWindow w, string typeNameContains)
    {
        ImageEffectBase? effect = LibraryEffects.Value.FirstOrDefault(e => e.GetType().Name.Contains(typeNameContains, StringComparison.OrdinalIgnoreCase));
        if (effect == null)
        {
            w.ShowMessage("This effect is not available.");
            return Task.CompletedTask;
        }
        return RunLibraryEffectAsync(w, effect.GetType());
    }

    /// <summary>Runs an effect from the shared ShareX image effects library with a generated parameter dialog.</summary>
    private static async Task RunLibraryEffectAsync(PhotoEditorWindow w, Type effectType, Dictionary<string, object?>? preset = null)
    {
        var (raster, smart) = FilterTarget(w);
        if (raster == null && smart == null) return;
        ImageEffectBase template = (ImageEffectBase)Activator.CreateInstance(effectType)!;
        Dictionary<string, object?> values = preset ?? [];
        Func<SKBitmap, SKBitmap> Make() => SmartFilterFactory.LibraryFunc(effectType, values);
        if (preset == null && template.Parameters.Count > 0)
        {
            PreviewSession? session = raster != null ? new PreviewSession(w, raster, 96) : null;
            Grid form = DialogBuilder.Form();
            foreach (EffectParameter parameter in template.Parameters)
            {
                Control? editor = CreateParameterEditor(w, parameter, values, () => session?.Preview(Make()));
                if (editor != null) DialogBuilder.AddRow(form, parameter.Label + ":", editor);
            }
            session?.Preview(Make());
            if (!await DialogBuilder.ShowAsync(w, template.Name, new ScrollViewer { Content = form, MaxHeight = 560 }, 520))
            {
                session?.Cancel();
                return;
            }
            if (session != null)
            {
                await session.CommitAsync(Make(), template.Name);
                RememberLibrary(w, effectType, template.Name, values);
                return;
            }
        }
        if (smart != null) AddSmartFilter(w, smart, SmartFilterFactory.CreateLibrary(template.Name, effectType, values, smart.Filters.Count));
        else await new PreviewSession(w, raster!, 96).CommitAsync(Make(), template.Name);
        RememberLibrary(w, effectType, template.Name, values);
    }

    private static void RememberLibrary(PhotoEditorWindow w, Type type, string name, Dictionary<string, object?> values)
    {
        Dictionary<string, object?> copy = new(values);
        w.LastFilterName = name;
        w.RepeatLastFilter = () => _ = RunLibraryEffectAsync(w, type, copy);
    }

    private static Control? CreateParameterEditor(PhotoEditorWindow w, EffectParameter parameter, Dictionary<string, object?> values, Action changed)
    {
        switch (parameter)
        {
            case SliderEffectParameter slider:
                values.TryAdd(slider.Key, slider.DefaultValue);
                return DialogBuilder.SliderNumber(slider.Minimum, slider.Maximum, (double)values[slider.Key]!, v => { values[slider.Key] = v; changed(); },
                    slider.IsSnapToTickEnabled ? Math.Max(0.01, slider.TickFrequency) : 0.01, slider.TickFrequency >= 1 && slider.IsSnapToTickEnabled ? "0" : "0.00");
            case NumericEffectParameter numeric:
                values.TryAdd(numeric.Key, numeric.DefaultValue);
                return DialogBuilder.SliderNumber((double)numeric.Minimum, (double)numeric.Maximum, (double)(decimal)values[numeric.Key]!, v => { values[numeric.Key] = (decimal)v; changed(); }, (double)numeric.Increment, numeric.Increment >= 1 ? "0" : "0.00");
            case CheckboxEffectParameter check:
            {
                values.TryAdd(check.Key, check.DefaultValue);
                CheckBox box = new() { IsChecked = (bool)values[check.Key]! };
                box.IsCheckedChanged += (_, _) => { values[check.Key] = box.IsChecked == true; changed(); };
                return box;
            }
            case EnumEffectParameter enumeration:
                values.TryAdd(enumeration.Key, enumeration.Options[enumeration.DefaultIndex].Value);
                return DialogBuilder.Combo(enumeration.Options.Select(o => (o.Label, o.Value)), values[enumeration.Key]!, v => { values[enumeration.Key] = v; changed(); });
            case ColorEffectParameter color:
            {
                values.TryAdd(color.Key, color.DefaultValue);
                SKColor current = (SKColor)values[color.Key]!;
                Button button = new() { Content = new Border { Width = 40, Height = 16, Background = DialogBuilder.ToBrush(current) } };
                button.Click += async (_, _) =>
                {
                    SKColor? picked = await ColorPickerDialog.ShowAsync(w, (SKColor)values[color.Key]!, color.Label, w);
                    if (picked is SKColor c)
                    {
                        values[color.Key] = c;
                        button.Content = new Border { Width = 40, Height = 16, Background = DialogBuilder.ToBrush(c) };
                        changed();
                    }
                };
                return button;
            }
            case TextEffectParameter text:
            {
                values.TryAdd(text.Key, text.DefaultValue);
                TextBox box = new() { Text = (string?)values[text.Key] };
                box.TextChanged += (_, _) => { values[text.Key] = box.Text ?? string.Empty; changed(); };
                return box;
            }
            default:
                return null;
        }
    }
}

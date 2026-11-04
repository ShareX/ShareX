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

using System.Globalization;
using ShareX.ImageEditor.Core.ImageEffects;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;

namespace ShareX.PhotoEditor.Imaging;

/// <summary>
/// Builds the delegates of smart filters from serializable recipes, so smart filters survive saving and loading a project.
/// </summary>
public static class SmartFilterFactory
{
    public const string FilterKind = "filter";
    public const string AdjustmentKind = "adjustment";
    public const string LibraryKind = "library";

    private static readonly Lazy<IReadOnlyList<Type>> LibraryTypes = new(() =>
        typeof(ImageEffectBase).Assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsClass: true } && typeof(ImageEffectBase).IsAssignableFrom(t) && t.GetConstructor(Type.EmptyTypes) != null)
            .ToArray());

    public static Func<SKBitmap, SKBitmap> FilterFunc(FilterDefinition filter, IReadOnlyDictionary<string, float> values, FilterContext context)
    {
        Dictionary<string, float> snapshot = new(values);
        return b => filter.Apply(b, snapshot, context);
    }

    public static Func<SKBitmap, SKBitmap> AdjustmentFunc(AdjustmentSettings settings)
    {
        AdjustmentSettings snapshot = settings.Clone();
        return b =>
        {
            SKBitmap copy = b.Copy();
            Adjustments.Apply(snapshot, copy, new SKRectI(0, 0, copy.Width, copy.Height));
            return copy;
        };
    }

    public static Func<SKBitmap, SKBitmap> LibraryFunc(Type effectType, IReadOnlyDictionary<string, object?> values)
    {
        Dictionary<string, object?> snapshot = new(values);
        return b =>
        {
            ImageEffectBase effect = (ImageEffectBase)Activator.CreateInstance(effectType)!;
            foreach (EffectParameter parameter in effect.Parameters)
            {
                snapshot.TryGetValue(parameter.Key, out object? value);
                parameter.Apply(effect, value);
            }
            using SKBitmap input = ToUnpremul(b);
            SKBitmap output = effect.Apply(input);
            SKBitmap converted = ImageOps.Convert(output);
            if (!ReferenceEquals(output, input)) output.Dispose();
            return converted;
        };
    }

    /// <summary>The shared effects library treats BGRA pixels as unpremultiplied SKColor values.</summary>
    private static SKBitmap ToUnpremul(SKBitmap source)
    {
        SKBitmap result = new(new SKImageInfo(source.Width, source.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul));
        using SKPixmap pixmap = source.PeekPixels();
        pixmap.ReadPixels(result.Info, result.GetPixels(), result.RowBytes);
        return result;
    }

    public static SmartFilter CreateFilter(FilterDefinition filter, Dictionary<string, float> values, FilterContext context, int index)
    {
        SmartFilterRecipe recipe = new()
        {
            Kind = FilterKind,
            Id = filter.Category + "|" + filter.Name,
            Values = new Dictionary<string, float>(values),
            Seed = context.Seed,
            Foreground = context.Foreground,
            Background = context.Background
        };
        return new SmartFilter(filter.DisplayName, filter.Key(values) + "#" + index, FilterFunc(filter, values, context)) { Recipe = recipe };
    }

    public static SmartFilter CreateAdjustment(string name, AdjustmentSettings settings, int index)
    {
        AdjustmentSettings snapshot = settings.Clone();
        SmartFilterRecipe recipe = new() { Kind = AdjustmentKind, Id = snapshot.Kind.ToString(), Adjustment = snapshot };
        string key = $"{snapshot.Kind}:{string.Join(",", snapshot.Values.OrderBy(v => v.Key).Select(v => v.Key + v.Value))}:{snapshot.Lut?.Name}:{string.Join(",", snapshot.SampleColors)}:{string.Join(",", snapshot.ExcludedColors)}#{index}";
        return new SmartFilter(name, key, AdjustmentFunc(snapshot)) { Recipe = recipe };
    }

    public static SmartFilter CreateLibrary(string name, Type effectType, IReadOnlyDictionary<string, object?> values, int index)
    {
        SmartFilterRecipe recipe = new()
        {
            Kind = LibraryKind,
            Id = effectType.FullName ?? effectType.Name,
            Library = values.ToDictionary(v => v.Key, v => EncodeValue(v.Value))
        };
        string key = effectType.Name + string.Join(",", values.Select(v => $"{v.Key}={v.Value}")) + "#" + index;
        return new SmartFilter(name, key, LibraryFunc(effectType, values)) { Recipe = recipe };
    }

    /// <summary>Rebuilds a smart filter from a stored recipe; returns null if the filter no longer exists.</summary>
    public static SmartFilter? Restore(string name, string key, bool enabled, SmartFilterRecipe recipe)
    {
        try
        {
            SmartFilter? result = null;
            switch (recipe.Kind)
            {
                case FilterKind:
                {
                    FilterDefinition? definition = Filters.All.FirstOrDefault(f => f.Category + "|" + f.Name == recipe.Id);
                    if (definition == null) return null;
                    Dictionary<string, float> values = definition.Defaults;
                    foreach (KeyValuePair<string, float> pair in recipe.Values ?? []) values[pair.Key] = pair.Value;
                    FilterContext context = new() { Foreground = recipe.Foreground, Background = recipe.Background, Seed = recipe.Seed };
                    result = new SmartFilter(name, key, FilterFunc(definition, values, context)) { Recipe = recipe };
                    break;
                }
                case AdjustmentKind when recipe.Adjustment != null:
                    result = new SmartFilter(name, key, AdjustmentFunc(recipe.Adjustment)) { Recipe = recipe };
                    break;
                case LibraryKind:
                {
                    Type? type = LibraryTypes.Value.FirstOrDefault(t => (t.FullName ?? t.Name) == recipe.Id);
                    if (type == null) return null;
                    Dictionary<string, object?> values = (recipe.Library ?? []).ToDictionary(v => v.Key, v => DecodeValue(v.Value));
                    result = new SmartFilter(name, key, LibraryFunc(type, values)) { Recipe = recipe };
                    break;
                }
            }
            if (result != null) result.Enabled = enabled;
            return result;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static string EncodeValue(object? value) => value switch
    {
        null => "n:",
        double d => "d:" + d.ToString("R", CultureInfo.InvariantCulture),
        decimal m => "m:" + m.ToString(CultureInfo.InvariantCulture),
        bool b => "b:" + (b ? "1" : "0"),
        string s => "s:" + s,
        SKColor c => "c:" + ((uint)c).ToString("X8", CultureInfo.InvariantCulture),
        int i => "i:" + i.ToString(CultureInfo.InvariantCulture),
        float f => "f:" + f.ToString("R", CultureInfo.InvariantCulture),
        Enum e => "e:" + e.GetType().FullName + "|" + e,
        _ => "n:"
    };

    private static object? DecodeValue(string text)
    {
        if (text.Length < 2) return null;
        string body = text[2..];
        switch (text[0])
        {
            case 'd': return double.Parse(body, CultureInfo.InvariantCulture);
            case 'm': return decimal.Parse(body, CultureInfo.InvariantCulture);
            case 'b': return body == "1";
            case 's': return body;
            case 'c': return new SKColor(uint.Parse(body, NumberStyles.HexNumber, CultureInfo.InvariantCulture));
            case 'i': return int.Parse(body, CultureInfo.InvariantCulture);
            case 'f': return float.Parse(body, CultureInfo.InvariantCulture);
            case 'e':
            {
                int split = body.IndexOf('|');
                Type? type = typeof(ImageEffectBase).Assembly.GetType(body[..split]) ?? Type.GetType(body[..split]);
                return type != null && Enum.TryParse(type, body[(split + 1)..], out object? parsed) ? parsed : null;
            }
            default: return null;
        }
    }
}

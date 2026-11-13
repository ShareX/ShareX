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
using Avalonia.Media.Imaging;

namespace ShareX.PixelEditor.UI.Dialogs;

/// <summary>The frame ranges offered by the export dialogs: all frames, the selection, the current frame or a tag.</summary>
internal static class FrameChoices
{
    public static List<(string Text, int Value)> For(Sprite sprite, bool asPlayed = true)
    {
        List<(string, int)> items = [(Strings.Export_AllFrames, -1), (Strings.Export_SelectedFrames, -2), (Strings.Export_CurrentFrame, -3)];
        if (asPlayed && sprite.Tags.Count > 0) items.Insert(1, (Strings.Export_AsPlayed, -4));
        for (int i = 0; i < sprite.Tags.Count; i++) items.Add((string.Format(Strings.Export_Tag, sprite.Tags[i].Name), i));
        return items;
    }

    public static List<int> Frames(Sprite sprite, int choice) => choice switch
    {
        -1 => SpriteOps.FrameSequence(0, sprite.Frames.Count - 1, TagDirection.Forward),
        -2 => SpriteOps.FrameSequence(sprite.RangeFrom, sprite.RangeTo, TagDirection.Forward),
        -3 => [sprite.ActiveFrame],
        -4 => SpriteOps.AnimationSequence(sprite),
        _ when choice < sprite.Tags.Count => SpriteOps.FrameSequence(sprite.Tags[choice].From, sprite.Tags[choice].To, sprite.Tags[choice].Direction),
        _ => [sprite.ActiveFrame]
    };

    public static int Default(Sprite sprite) => sprite.RangeTo > sprite.RangeFrom ? -2 : -1;
}

public static class ExportDialog
{
    private enum Kind
    {
        Png,
        Gif,
        Apng,
        Sequence,
        WebP,
        Jpeg
    }

    public static async Task ShowAsync(PixelEditorWindow w)
    {
        Sprite sprite = w.Sprite!;
        EditorSettings settings = w.Settings;
        bool animated = sprite.Frames.Count > 1;
        Kind initial = settings.ExportFormat switch
        {
            ".gif" => Kind.Gif,
            ".apng" => Kind.Apng,
            ".seq" => Kind.Sequence,
            ".webp" => Kind.WebP,
            ".jpg" => Kind.Jpeg,
            _ => Kind.Png
        };
        if (!animated && initial is Kind.Gif or Kind.Apng or Kind.Sequence) initial = Kind.Png;
        Form form = new(w);
        Func<Kind> kind = form.Combo(Strings.Export_Format,
        [
            (Strings.Export_Png, Kind.Png), (Strings.Export_Gif, Kind.Gif), (Strings.Export_Apng, Kind.Apng), (Strings.Export_Sequence, Kind.Sequence), ("WebP", Kind.WebP), ("JPEG", Kind.Jpeg)
        ], initial);
        Func<int> frames = form.Combo(Strings.Export_Frames, FrameChoices.For(sprite), FrameChoices.Default(sprite));
        Func<int> scale = form.Number(Strings.Export_Scale, 1, 64, settings.ExportScale);
        Func<bool> smooth = form.Check(Strings.Export_SmoothScale, settings.ExportSmoothScale);
        Func<bool> loop = form.Check(Strings.Command_Loop, true);
        Func<int> split = form.Combo(Strings.Export_Split, [(Strings.Export_SplitNone, 0), (Strings.Export_SplitLayers, 1), (Strings.Export_SplitTags, 2)], Math.Clamp(settings.ExportSplit, 0, 2));
        TextBlock info = new() { Classes = { "dim" }, TextWrapping = TextWrapping.Wrap };
        form.Add(info);
        void Update()
        {
            bool single = kind() is Kind.Png or Kind.WebP or Kind.Jpeg;
            int count = single ? 1 : FrameChoices.Frames(sprite, frames()).Count;
            info.Text = string.Format(Strings.Export_Info, sprite.Width * scale(), sprite.Height * scale(), count) + (single && animated ? "  " + Strings.Export_SingleNote : string.Empty);
            int files = Jobs(sprite, "x.png", split(), new ExportOptions { Frames = FrameChoices.Frames(sprite, frames()) }, sprite.ActiveFrame).Count;
            if (split() != 0) info.Text += "  " + string.Format(Strings.Export_FileCount, files);
        }
        form.Changed += Update;
        Update();
        if (!await form.ShowAsync(Strings.Command_Export.Replace("...", string.Empty), 420, Strings.Export_Button) || !ReferenceEquals(w.Sprite, sprite)) return;

        Kind chosen = kind();
        (string Name, string[] Extensions) type = chosen switch
        {
            Kind.Gif => ("GIF", [".gif"]),
            Kind.Apng => ("Animated PNG", [".png", ".apng"]),
            Kind.WebP => ("WebP", [".webp"]),
            Kind.Jpeg => ("JPEG", [".jpg", ".jpeg"]),
            _ => ("PNG", [".png"])
        };
        string? path = await w.PickSaveFileAsync(Strings.Command_Export.Replace("...", string.Empty), Path.GetFileNameWithoutExtension(sprite.DisplayName) + type.Extensions[0], type);
        if (path == null) return;
        settings.ExportFormat = chosen switch
        {
            Kind.Gif => ".gif",
            Kind.Apng => ".apng",
            Kind.Sequence => ".seq",
            Kind.WebP => ".webp",
            Kind.Jpeg => ".jpg",
            _ => ".png"
        };
        settings.ExportScale = scale();
        settings.ExportSmoothScale = smooth();
        ExportOptions options = new() { Scale = scale(), Loop = loop(), Frames = FrameChoices.Frames(sprite, frames()), SmoothScale = smooth() };
        settings.ExportSplit = split();
        List<(string Path, ExportOptions Options, int Frame)> jobs = Jobs(sprite, path, split(), options, sprite.ActiveFrame);
        await w.RunBusyAsync(Strings.Status_Exporting, async progress =>
        {
            await Task.Run(() =>
            {
                foreach ((string file, ExportOptions o, int frame) in jobs)
                {
                    switch (chosen)
                    {
                        case Kind.Gif:
                            SpriteFile.ExportGif(sprite, file, o, jobs.Count == 1 ? progress : null);
                            break;
                        case Kind.Apng:
                            SpriteFile.ExportApng(sprite, file, o, jobs.Count == 1 ? progress : null);
                            break;
                        case Kind.Sequence:
                            SpriteFile.ExportSequence(sprite, file, o);
                            break;
                        default:
                            SpriteFile.ExportImage(sprite, file, frame, o);
                            break;
                    }
                }
            });
            w.SetStatus(jobs.Count == 1 ? string.Format(Strings.Status_Exported, Path.GetFileName(jobs[0].Path)) : string.Format(Strings.Status_ExportedFiles, jobs.Count));
        });
    }

    /// <summary>The files an export writes: one, or one per visible layer or per tag, named after the chosen file.</summary>
    private static List<(string Path, ExportOptions Options, int Frame)> Jobs(Sprite sprite, string path, int split, ExportOptions options, int current)
    {
        string directory = Path.GetDirectoryName(path) ?? string.Empty, name = Path.GetFileNameWithoutExtension(path), extension = Path.GetExtension(path);
        string Named(string part) => Path.Combine(directory, $"{name}-{string.Concat(part.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c))}{extension}");
        List<(string, ExportOptions, int)> jobs = [];
        if (split == 1)
        {
            foreach (PixelLayer layer in sprite.PixelLayers.Where(l => l.IsEffectivelyVisible && !l.IsReference).Reverse())
            {
                PixelLayer captured = layer;
                ExportOptions o = new() { Scale = options.Scale, SmoothScale = options.SmoothScale, Loop = options.Loop, Frames = options.Frames, LayerFilter = l => l == captured || l is GroupLayer g && captured.IsDescendantOf(g) };
                jobs.Add((Named(layer.Name), o, current));
            }
        }
        else if (split == 2)
        {
            foreach (Tag tag in sprite.Tags)
            {
                List<int> frames = SpriteOps.FrameSequence(tag.From, Math.Min(tag.To, sprite.Frames.Count - 1), tag.Direction);
                ExportOptions o = new() { Scale = options.Scale, SmoothScale = options.SmoothScale, Loop = options.Loop, Frames = frames, LayerFilter = options.LayerFilter };
                jobs.Add((Named(tag.Name), o, tag.From));
            }
        }
        if (jobs.Count == 0) jobs.Add((path, options, current));
        return jobs;
    }
}

public static class SheetDialogs
{
    public static async Task ExportAsync(PixelEditorWindow w)
    {
        Sprite sprite = w.Sprite!;
        EditorSettings settings = w.Settings;
        Form form = new(w);
        Func<int> frames = form.Combo(Strings.Export_Frames, FrameChoices.For(sprite, false), FrameChoices.Default(sprite));
        Func<SheetLayout> layout = form.Combo(Strings.Sheet_Layout, EnumNames.SheetLayouts, settings.SheetLayout);
        Func<int> columns = form.Number(Strings.Sheet_Columns, 1, 256, settings.SheetColumns);
        Func<int> padding = form.Number(Strings.Sheet_Padding, 0, 256, settings.SheetPadding);
        Func<int> spacing = form.Number(Strings.Sheet_Spacing, 0, 256, settings.SheetSpacing);
        Func<int> scale = form.Number(Strings.Export_Scale, 1, 64, settings.ExportScale);
        Func<bool> smooth = form.Check(Strings.Export_SmoothScale, settings.ExportSmoothScale);
        Func<bool> trim = form.Check(Strings.Sheet_Trim, settings.SheetTrim);
        Func<bool> merge = form.Check(Strings.Sheet_MergeDuplicates, settings.SheetMergeDuplicates);
        Func<bool> metadata = form.Check(Strings.Sheet_Metadata, settings.SheetMetadata);
        TextBlock info = new() { Classes = { "dim" } };
        form.Add(info);
        ExportOptions Options() => new()
        {
            Scale = scale(),
            SmoothScale = smooth(),
            Frames = FrameChoices.Frames(sprite, frames()),
            Layout = layout(),
            Columns = columns(),
            Padding = padding(),
            Spacing = spacing(),
            WriteMetadata = metadata(),
            Trim = trim(),
            MergeDuplicates = merge()
        };
        void Update()
        {
            ExportOptions o = Options();
            SpriteFile.SheetPlan plan = SpriteFile.PlanSheet(sprite, o);
            info.Text = string.Format(Strings.Sheet_Info, plan.Width, plan.Height, o.Frames.Count) + (plan.Images.Count < o.Frames.Count ? "  " + string.Format(Strings.Sheet_Unique, plan.Images.Count) : string.Empty);
        }
        form.Changed += Update;
        Update();
        if (!await form.ShowAsync(Strings.Command_ExportSheet.Replace("...", string.Empty), 420, Strings.Export_Button) || !ReferenceEquals(w.Sprite, sprite)) return;
        string? path = await w.PickSaveFileAsync(Strings.Command_ExportSheet.Replace("...", string.Empty), Path.GetFileNameWithoutExtension(sprite.DisplayName) + "-sheet.png", ("PNG", [".png"]), ("WebP", [".webp"]));
        if (path == null) return;
        ExportOptions options = Options();
        (settings.SheetLayout, settings.SheetColumns, settings.SheetPadding, settings.SheetSpacing, settings.SheetMetadata, settings.ExportScale) = (options.Layout, options.Columns, options.Padding, options.Spacing, options.WriteMetadata, options.Scale);
        (settings.SheetTrim, settings.SheetMergeDuplicates, settings.ExportSmoothScale) = (options.Trim, options.MergeDuplicates, options.SmoothScale);
        await w.RunBusyAsync(Strings.Status_Exporting, async _ =>
        {
            await Task.Run(() => SpriteFile.ExportSheet(sprite, path, options));
            w.SetStatus(string.Format(Strings.Status_Exported, Path.GetFileName(path)));
        });
    }

    public static async Task ImportAsync(PixelEditorWindow w, string path)
    {
        PixelBuffer sheet;
        try
        {
            using SKBitmap bitmap = ShareX.HelpersLib.SkiaImageHelpers.LoadImage(path) ?? throw new InvalidDataException();
            sheet = PixelBuffer.FromBitmap(bitmap);
        }
        catch (Exception ex)
        {
            w.ShowMessage(string.Format(Strings.Message_CouldNotOpen, Path.GetFileName(path), ex.Message));
            return;
        }
        // A strip of square frames is the most common sheet, so guess that first.
        int guess = sheet.Width % sheet.Height == 0 ? sheet.Height : sheet.Height % sheet.Width == 0 ? sheet.Width : Math.Min(32, Math.Min(sheet.Width, sheet.Height));
        Form form = new(w);
        form.Note(string.Format(Strings.Sheet_ImportSize, sheet.Width, sheet.Height));
        Func<int> cellWidth = form.Number(Strings.Sheet_CellWidth, 1, sheet.Width, guess), cellHeight = form.Number(Strings.Sheet_CellHeight, 1, sheet.Height, guess);
        Func<int> padding = form.Number(Strings.Sheet_Padding, 0, 256, 0), spacing = form.Number(Strings.Sheet_Spacing, 0, 256, 0);
        Func<int> duration = form.Number(Strings.Dialog_DurationMs, 1, 65535, 100, 10);
        Func<bool> skipEmpty = form.Check(Strings.Sheet_SkipEmpty, true);
        TextBlock info = new() { Classes = { "dim" } };
        form.Add(info);
        void Update()
        {
            int cols = Math.Max(0, (sheet.Width - padding() * 2 + spacing()) / (cellWidth() + spacing())), rows = Math.Max(0, (sheet.Height - padding() * 2 + spacing()) / (cellHeight() + spacing()));
            info.Text = string.Format(Strings.Sheet_ImportInfo, cols, rows, cols * rows);
        }
        form.Changed += Update;
        Update();
        if (!await form.ShowAsync(Strings.Command_ImportSheet.Replace("...", string.Empty), 400, Strings.Sheet_ImportButton)) return;
        Sprite sprite = SpriteFile.FromSheet(sheet, cellWidth(), cellHeight(), padding(), spacing(), skipEmpty(), Path.GetFileNameWithoutExtension(path), Strings.Layer_DefaultName, duration());
        List<uint> colors = Palettes.FromImage(sheet, 257);
        if (colors.Count is > 0 and <= 256) sprite.Palette = colors;
        w.OpenSprite(sprite);
        sprite.History.MarkModified();
        w.RefreshPanels();
    }
}

public static class TextDialog
{
    /// <summary>Asks for text and font settings and returns the rendered pixels in the foreground color.</summary>
    public static async Task<PixelBuffer?> ShowAsync(PixelEditorWindow w)
    {
        EditorSettings s = w.Settings;
        Form form = new(w);
        Func<string> text = form.Text(Strings.Text_Text, string.Empty, true);
        List<(string, string)> fonts = [(PixelFont.BuiltInName, PixelFont.BuiltInName), (PixelFont.BuiltInMonoName, PixelFont.BuiltInMonoName)];
        fonts.AddRange(SKFontManager.Default.FontFamilies.OrderBy(f => f).Select(f => (f, f)));
        Func<string> font = form.Combo(Strings.Text_Font, fonts, fonts.Any(f => f.Item2 == s.TextFont) ? s.TextFont : PixelFont.BuiltInName);
        Func<int> size = form.Number(Strings.Options_Size, 4, 256, s.TextSize);
        Func<bool> bold = form.Check(Strings.Text_Bold, s.TextBold), italic = form.Check(Strings.Text_Italic, s.TextItalic), antialias = form.Check(Strings.Text_Antialias, s.TextAntialias);
        form.Note(Strings.Text_Note);
        Image preview = new() { Stretch = Stretch.None, HorizontalAlignment = HorizontalAlignment.Left };
        RenderOptions.SetBitmapInterpolationMode(preview, BitmapInterpolationMode.None);
        form.Add(new Border { Height = 96, Background = Ui.Checker(0xFF8C8C8C, 0xFFA6A6A6, 4), ClipToBounds = true, Padding = new Thickness(6), Child = preview });

        PixelBuffer? Render()
        {
            string value = text();
            if (string.IsNullOrWhiteSpace(value)) return null;
            try
            {
                // The built-in font is 7 pixels tall and grows in whole steps.
                return PixelFont.IsBuiltIn(font())
                    ? PixelFont.RenderBuiltIn(value, w.ForegroundColor, Math.Max(1, size() / 8), font() == PixelFont.BuiltInMonoName)
                    : PixelFont.RenderSystem(value, font(), size(), bold(), italic(), antialias(), w.ForegroundColor);
            }
            catch (Exception)
            {
                return null;
            }
        }
        form.Changed += () =>
        {
            PixelBuffer? image = Render();
            preview.Source = image != null ? Ui.ToBitmap(image) : null;
        };
        if (!await form.ShowAsync(Strings.Tool_Text, 440)) return null;
        (s.TextFont, s.TextSize, s.TextBold, s.TextItalic, s.TextAntialias) = (font(), size(), bold(), italic(), antialias());
        return Render();
    }
}

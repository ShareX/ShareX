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

using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShareX.PixelEditor.UI;

public sealed class DockGroupLayout
{
    public List<string> Panels { get; set; } = [];

    public string? Selected { get; set; }

    public bool Collapsed { get; set; }

    public double Height { get; set; } = 1;
}

public enum TiledMode
{
    None,
    Horizontal,
    Vertical,
    Both
}

/// <summary>Persisted Pixel editor preferences, tool options and workspace layout.</summary>
public sealed class EditorSettings
{
    // View
    public bool ShowPixelGrid { get; set; } = true;
    public int PixelGridZoom { get; set; } = 8;
    public bool ShowGrid { get; set; }
    public int GridWidth { get; set; } = 16;
    public int GridHeight { get; set; } = 16;
    public string GridColor { get; set; } = "#00A0FF";
    public bool SnapToGrid { get; set; }
    public int CheckerSize { get; set; } = 8;
    public string CheckerColor1 { get; set; } = "#8C8C8C";
    public string CheckerColor2 { get; set; } = "#A6A6A6";
    public bool ShowSlices { get; set; } = true;
    public TiledMode Tiled { get; set; }
    public SymmetryMode Symmetry { get; set; }
    public bool OnionSkin { get; set; }
    public int OnionPrevious { get; set; } = 1;
    public int OnionNext { get; set; } = 1;
    public int OnionOpacity { get; set; } = 45;
    public bool OnionTint { get; set; } = true;
    public bool ShowBrushPreview { get; set; } = true;

    // Tools
    public int BrushSize { get; set; } = 1;
    public bool BrushRound { get; set; } = true;
    public Ink Ink { get; set; } = Ink.Normal;
    public int Opacity { get; set; } = 100;
    public BrushPattern Pattern { get; set; } = BrushPattern.Solid;
    public bool PixelPerfect { get; set; } = true;
    public int EraserSize { get; set; } = 1;
    public int SprayRadius { get; set; } = 8;
    public int SprayDensity { get; set; } = 20;
    public int FillTolerance { get; set; }
    public bool FillContiguous { get; set; } = true;
    public bool FillDiagonal { get; set; }
    public bool FillAllLayers { get; set; }
    public bool ShapeFilled { get; set; }
    public bool GradientRadial { get; set; }
    public DitherMode GradientDither { get; set; } = DitherMode.Bayer4;
    public bool GradientToTransparent { get; set; }
    public SelectionMode SelectionMode { get; set; } = SelectionMode.Replace;
    public int WandTolerance { get; set; }
    public bool WandContiguous { get; set; } = true;
    public bool WandAllLayers { get; set; }
    public bool EyedropperAllLayers { get; set; } = true;
    public string TextFont { get; set; } = PixelFont.BuiltInName;
    public int TextSize { get; set; } = 12;
    public bool TextBold { get; set; }
    public bool TextItalic { get; set; }
    public bool TextAntialias { get; set; }

    // Animation
    public bool LoopPlayback { get; set; } = true;
    public bool PlayTagOnly { get; set; } = true;
    public int TimelineCellSize { get; set; } = 24;

    /// <summary>Cels show a small picture of their pixels instead of a dot.</summary>
    public bool TimelineThumbnails { get; set; } = true;
    public int PreviewZoom { get; set; }

    // Documents
    public string Foreground { get; set; } = "#000000";
    public string Background { get; set; } = "#FFFFFF";
    public int NewWidth { get; set; } = 32;
    public int NewHeight { get; set; } = 32;
    public bool NewTransparent { get; set; } = true;
    public string NewPalette { get; set; } = string.Empty;
    public List<string> RecentFiles { get; set; } = [];

    /// <summary>Rotating moved pixels by angles other than right angles smooths their edges.</summary>
    public bool SmoothRotation { get; set; } = true;

    /// <summary>Minutes between recovery copies of sprites with unsaved changes; 0 turns them off.</summary>
    public int AutoSaveMinutes { get; set; } = 2;

    /// <summary>Command palette entries run most recently, newest first.</summary>
    public List<string> RecentCommands { get; set; } = [];
    public int HistoryStates { get; set; } = 100;
    public string ExportFormat { get; set; } = ".png";
    public int ExportScale { get; set; } = 1;
    public SheetLayout SheetLayout { get; set; } = SheetLayout.Horizontal;
    public int SheetColumns { get; set; } = 8;
    public int SheetPadding { get; set; }
    public int SheetSpacing { get; set; }
    public bool SheetMetadata { get; set; } = true;

    public bool SheetTrim { get; set; }

    public bool SheetMergeDuplicates { get; set; }

    // Workspace
    public Dictionary<string, string> Shortcuts { get; set; } = [];
    public List<DockGroupLayout> Panels { get; set; } = [];
    public double RightDockWidth { get; set; } = 290;
    public double TimelineHeight { get; set; } = 196;
    public bool RightDockVisible { get; set; } = true;
    public bool TimelineVisible { get; set; } = true;
    public bool ToolbarVisible { get; set; } = true;
    public bool ToolbarTwoColumns { get; set; }
    public bool OptionsVisible { get; set; } = true;
    public bool RememberWindowState { get; set; } = true;
    public ShareX.HelpersLib.WindowState WindowState { get; set; } = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static EditorSettings Load(string? path)
    {
        try
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                return (JsonSerializer.Deserialize<EditorSettings>(File.ReadAllText(path), JsonOptions) ?? new EditorSettings()).Normalize();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Pixel editor settings could not be loaded: {ex.Message}");
        }
        return new EditorSettings();
    }

    /// <summary>Replaces missing or out-of-range values read from a hand-edited or older settings file.</summary>
    private EditorSettings Normalize()
    {
        RecentFiles ??= [];
        RecentCommands ??= [];
        AutoSaveMinutes = Math.Clamp(AutoSaveMinutes, 0, 60);
        Shortcuts ??= [];
        Panels ??= [];
        WindowState ??= new();
        Panels.RemoveAll(p => p == null);
        foreach (DockGroupLayout group in Panels) group.Panels ??= [];
        BrushSize = Math.Clamp(BrushSize, 1, 64);
        EraserSize = Math.Clamp(EraserSize, 1, 64);
        Opacity = Math.Clamp(Opacity, 1, 100);
        GridWidth = Math.Clamp(GridWidth, 1, 1024);
        GridHeight = Math.Clamp(GridHeight, 1, 1024);
        CheckerSize = Math.Clamp(CheckerSize, 1, 64);
        PixelGridZoom = Math.Clamp(PixelGridZoom, 2, 64);
        OnionPrevious = Math.Clamp(OnionPrevious, 0, 5);
        OnionNext = Math.Clamp(OnionNext, 0, 5);
        OnionOpacity = Math.Clamp(OnionOpacity, 5, 100);
        HistoryStates = Math.Clamp(HistoryStates, 10, 1000);
        NewWidth = Math.Clamp(NewWidth, 1, Sprite.MaxSize);
        NewHeight = Math.Clamp(NewHeight, 1, Sprite.MaxSize);
        ExportScale = Math.Clamp(ExportScale, 1, 64);
        TimelineCellSize = Math.Clamp(TimelineCellSize, 16, 48);
        TextSize = Math.Clamp(TextSize, 4, 256);
        if (!(RightDockWidth >= 0)) RightDockWidth = 290;
        if (!(TimelineHeight >= 0)) TimelineHeight = 196;
        if (string.IsNullOrEmpty(ExportFormat)) ExportFormat = ".png";
        if (string.IsNullOrEmpty(TextFont)) TextFont = PixelFont.BuiltInName;
        return this;
    }

    public void Save(string? path)
    {
        if (string.IsNullOrEmpty(path)) return;
        try
        {
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(path, JsonSerializer.Serialize(this, JsonOptions));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Pixel editor settings could not be saved: {ex.Message}");
        }
    }

    public static uint ParseColor(string? text, uint fallback) => Argb.TryParseHex(text, out uint c) ? c : fallback;

    public void AddRecentFile(string path)
    {
        RecentFiles.RemoveAll(p => string.Equals(p, path, StringComparison.OrdinalIgnoreCase));
        RecentFiles.Insert(0, path);
        if (RecentFiles.Count > 15) RecentFiles.RemoveRange(15, RecentFiles.Count - 15);
    }
}

/// <summary>Localized names of the option enums.</summary>
public static class EnumNames
{
    public static IEnumerable<(string, Ink)> Inks =>
    [
        (Strings.Ink_Normal, Ink.Normal),
        (Strings.Ink_Replace, Ink.Replace),
        (Strings.Ink_LockAlpha, Ink.LockAlpha),
        (Strings.Ink_Behind, Ink.Behind),
        (Strings.Ink_Lighten, Ink.Lighten),
        (Strings.Ink_Darken, Ink.Darken)
    ];

    public static IEnumerable<(string, BrushPattern)> Patterns =>
    [
        (Strings.Pattern_Solid, BrushPattern.Solid),
        (Strings.Pattern_Checker, BrushPattern.Checker),
        (Strings.Pattern_Sparse, BrushPattern.Sparse),
        (Strings.Pattern_Dense, BrushPattern.Dense),
        (Strings.Pattern_HorizontalLines, BrushPattern.HorizontalLines),
        (Strings.Pattern_VerticalLines, BrushPattern.VerticalLines),
        (Strings.Pattern_Diagonal, BrushPattern.Diagonal),
        (Strings.Pattern_Dots, BrushPattern.Dots)
    ];

    public static IEnumerable<(string, DitherMode)> Dithers =>
    [
        (Strings.Dither_None, DitherMode.None),
        (Strings.Dither_Bayer4, DitherMode.Bayer4),
        (Strings.Dither_Bayer8, DitherMode.Bayer8)
    ];

    public static IEnumerable<(string, RemapDither)> RemapDithers =>
    [
        (Strings.Dither_None, RemapDither.None),
        (Strings.Dither_Ordered, RemapDither.Ordered),
        (Strings.Dither_FloydSteinberg, RemapDither.FloydSteinberg)
    ];

    public static IEnumerable<(string, BlendMode)> BlendModes =>
    [
        (Strings.Blend_Normal, BlendMode.Normal),
        (Strings.Blend_Darken, BlendMode.Darken),
        (Strings.Blend_Multiply, BlendMode.Multiply),
        (Strings.Blend_ColorBurn, BlendMode.ColorBurn),
        (Strings.Blend_Lighten, BlendMode.Lighten),
        (Strings.Blend_Screen, BlendMode.Screen),
        (Strings.Blend_ColorDodge, BlendMode.ColorDodge),
        (Strings.Blend_Addition, BlendMode.Addition),
        (Strings.Blend_Overlay, BlendMode.Overlay),
        (Strings.Blend_SoftLight, BlendMode.SoftLight),
        (Strings.Blend_HardLight, BlendMode.HardLight),
        (Strings.Blend_Difference, BlendMode.Difference),
        (Strings.Blend_Exclusion, BlendMode.Exclusion),
        (Strings.Blend_Subtract, BlendMode.Subtract),
        (Strings.Blend_Divide, BlendMode.Divide),
        (Strings.Blend_Hue, BlendMode.Hue),
        (Strings.Blend_Saturation, BlendMode.Saturation),
        (Strings.Blend_Color, BlendMode.Color),
        (Strings.Blend_Luminosity, BlendMode.Luminosity)
    ];

    public static IEnumerable<(string, TagDirection)> Directions =>
    [
        (Strings.Direction_Forward, TagDirection.Forward),
        (Strings.Direction_Reverse, TagDirection.Reverse),
        (Strings.Direction_PingPong, TagDirection.PingPong),
        (Strings.Direction_PingPongReverse, TagDirection.PingPongReverse)
    ];

    public static IEnumerable<(string, CelScope)> Scopes =>
    [
        (Strings.Scope_Cel, CelScope.Cel),
        (Strings.Scope_SelectedFrames, CelScope.SelectedFrames),
        (Strings.Scope_Layer, CelScope.Layer),
        (Strings.Scope_Sprite, CelScope.Sprite)
    ];

    public static IEnumerable<(string, ResizeMethod)> ResizeMethods =>
    [
        (Strings.Resize_Nearest, ResizeMethod.Nearest),
        (Strings.Resize_PixelArt, ResizeMethod.PixelArtScale),
        (Strings.Resize_Smooth, ResizeMethod.Smooth)
    ];

    public static IEnumerable<(string, SheetLayout)> SheetLayouts =>
    [
        (Strings.Sheet_Horizontal, SheetLayout.Horizontal),
        (Strings.Sheet_Vertical, SheetLayout.Vertical),
        (Strings.Sheet_Grid, SheetLayout.Grid),
        (Strings.Sheet_Packed, SheetLayout.Packed)
    ];
}

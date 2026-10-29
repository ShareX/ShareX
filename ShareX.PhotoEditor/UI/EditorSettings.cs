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

namespace ShareX.PhotoEditor.UI;

public sealed class ToolbarGroupLayout
{
    public List<string> Tools { get; set; } = [];
}

public sealed class PanelGroupLayout
{
    public List<string> Panels { get; set; } = [];
    public string? Selected { get; set; }
    public bool Collapsed { get; set; }
    public double Height { get; set; } = 1;
}

/// <summary>Persisted Photo editor preferences, shortcuts and workspace layout.</summary>
public sealed class EditorSettings
{
    public int HistoryStates { get; set; } = 50;
    public float GridSpacing { get; set; } = 64;
    public int GridSubdivisions { get; set; } = 4;
    public string GridColor { get; set; } = "#80808080";
    public string GuideColor { get; set; } = "#FF00C8FF";
    public int CheckerSize { get; set; } = 8;
    public bool ShowRulers { get; set; } = true;
    public bool ShowGrid { get; set; }
    public bool ShowGuides { get; set; } = true;
    public bool ShowSelectionEdges { get; set; } = true;
    public bool ShowSlices { get; set; } = true;
    public bool ShowNotes { get; set; } = true;
    public bool ShowCount { get; set; } = true;
    public bool ShowPixelGrid { get; set; } = true;
    public bool Snap { get; set; } = true;
    public bool SnapToGuides { get; set; } = true;
    public bool SnapToGrid { get; set; }
    public bool SnapToDocument { get; set; } = true;
    public bool LockGuides { get; set; }
    public string Foreground { get; set; } = "#000000";
    public string Background { get; set; } = "#FFFFFF";
    public ResampleMethod DefaultResample { get; set; } = ResampleMethod.Bicubic;
    public List<string> RecentFiles { get; set; } = [];
    public List<string> Swatches { get; set; } = [];

    /// <summary>Command id to key gesture (e.g. "edit.undo" = "Ctrl+Z"). Missing entries use defaults.</summary>
    public Dictionary<string, string> Shortcuts { get; set; } = [];

    /// <summary>Customized toolbar; empty means the default layout.</summary>
    public List<ToolbarGroupLayout> Toolbar { get; set; } = [];

    /// <summary>Tools removed from the toolbar through Edit Toolbar.</summary>
    public List<string> HiddenTools { get; set; } = [];

    public List<PanelGroupLayout> Panels { get; set; } = [];

    public double RightDockWidth { get; set; } = 300;

    public bool RightDockVisible { get; set; } = true;

    public bool ToolbarVisible { get; set; } = true;

    public bool ToolbarTwoColumns { get; set; }

    public bool OptionsVisible { get; set; } = true;

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
                return JsonSerializer.Deserialize<EditorSettings>(File.ReadAllText(path), JsonOptions) ?? new EditorSettings();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Photo editor settings could not be loaded: {ex.Message}");
        }
        return new EditorSettings();
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
            System.Diagnostics.Debug.WriteLine($"Photo editor settings could not be saved: {ex.Message}");
        }
    }

    public static SKColor ParseColor(string? text, SKColor fallback) => ColorMath.TryParseHex(text, out SKColor c) ? c : fallback;

    public void AddRecentFile(string path)
    {
        RecentFiles.RemoveAll(p => string.Equals(p, path, StringComparison.OrdinalIgnoreCase));
        RecentFiles.Insert(0, path);
        if (RecentFiles.Count > 15) RecentFiles.RemoveRange(15, RecentFiles.Count - 15);
    }
}

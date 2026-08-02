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

using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Localization;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.ImageEditor.Presentation.ViewModels;

public sealed class ToolbarCustomizationItemViewModel : ViewModelBase
{
    public const string FileItemId = "File";
    public const string BackgroundItemId = "Background";
    public const string ImageEffectsItemId = "ImageEffects";

    private sealed record ToolbarItemDefinition(
        string Id,
        EditorTool? Tool,
        string Name,
        string Icon,
        string DefaultHotkey,
        bool IsHotkeyEditable,
        bool IsVisibilityEditable,
        bool BeginGroupByDefault);

    private static readonly IReadOnlyList<ToolbarItemDefinition> ItemDefinitions = new[]
    {
        new ToolbarItemDefinition(FileItemId, null, Strings.ToolbarCustomizationItemViewModel_File, EditorIcons.FileMenu, "", true, false, false),
        CreateToolDefinition(EditorTool.Select, Strings.ToolbarCustomizationItemViewModel_Select, EditorIcons.ToolSelect, "V", beginGroupByDefault: true),
        CreateToolDefinition(EditorTool.Rectangle, Strings.ToolbarCustomizationItemViewModel_Rectangle, EditorIcons.ToolRectangle, "R"),
        CreateToolDefinition(EditorTool.Ellipse, Strings.ToolbarCustomizationItemViewModel_Ellipse, EditorIcons.ToolEllipse, "E"),
        CreateToolDefinition(EditorTool.Line, Strings.ToolbarCustomizationItemViewModel_Line, EditorIcons.ToolLine, "L"),
        CreateToolDefinition(EditorTool.Arrow, Strings.ToolbarCustomizationItemViewModel_Arrow, EditorIcons.ToolArrow, "A"),
        CreateToolDefinition(EditorTool.Freehand, Strings.ToolbarCustomizationItemViewModel_Freehand, EditorIcons.ToolFreehand, "F"),
        CreateToolDefinition(EditorTool.Text, Strings.ToolbarCustomizationItemViewModel_Text, EditorIcons.ToolText, "T"),
        CreateToolDefinition(EditorTool.SpeechBalloon, Strings.ToolbarCustomizationItemViewModel_SpeechBalloon, EditorIcons.ToolSpeechBalloon, "O"),
        CreateToolDefinition(EditorTool.Step, Strings.ToolbarCustomizationItemViewModel_Step, EditorIcons.ToolStep, "N"),
        CreateToolDefinition(EditorTool.Image, Strings.ToolbarCustomizationItemViewModel_Image, EditorIcons.ToolImage, "I"),
        CreateToolDefinition(EditorTool.Emoji, Strings.ToolbarCustomizationItemViewModel_Emoji, EditorIcons.ToolEmoji, "J"),
        CreateToolDefinition(EditorTool.Cursor, Strings.ToolbarCustomizationItemViewModel_Cursor, EditorIcons.ToolCursor, "K"),
        CreateToolDefinition(EditorTool.Highlight, Strings.ToolbarCustomizationItemViewModel_Highlight, EditorIcons.ToolHighlight, "H"),
        CreateToolDefinition(EditorTool.SmartEraser, Strings.ToolbarCustomizationItemViewModel_SmartEraser, EditorIcons.ToolSmartEraser, "W"),
        CreateToolDefinition(EditorTool.Blur, Strings.ToolbarCustomizationItemViewModel_Blur, EditorIcons.ToolBlur, "B"),
        CreateToolDefinition(EditorTool.Pixelate, Strings.ToolbarCustomizationItemViewModel_Pixelate, EditorIcons.ToolPixelate, "P"),
        CreateToolDefinition(EditorTool.Magnify, Strings.ToolbarCustomizationItemViewModel_Magnify, EditorIcons.ToolMagnify, "M"),
        CreateToolDefinition(EditorTool.Spotlight, Strings.ToolbarCustomizationItemViewModel_Spotlight, EditorIcons.ToolSpotlight, "S"),
        CreateToolDefinition(EditorTool.Crop, Strings.ToolbarCustomizationItemViewModel_Crop, EditorIcons.ToolCrop, "C", beginGroupByDefault: true),
        CreateToolDefinition(EditorTool.CutOut, Strings.ToolbarCustomizationItemViewModel_CutOut, EditorIcons.ToolCutOut, "U"),
        new ToolbarItemDefinition(BackgroundItemId, null, Strings.ToolbarCustomizationItemViewModel_Background, EditorIcons.PanelBackground, "", true, true, false),
        new ToolbarItemDefinition(ImageEffectsItemId, null, Strings.ToolbarCustomizationItemViewModel_ImageEffects, EditorIcons.PanelEffects, "", true, true, false)
    };

    private static readonly IReadOnlyDictionary<string, ToolbarItemDefinition> ItemDefinitionsById =
        ItemDefinitions.ToDictionary(definition => definition.Id, StringComparer.OrdinalIgnoreCase);

    private bool _isVisible = true;
    private bool _isActive;
    private bool _canMoveUp;
    private bool _canMoveDown;
    private bool _beginGroup;
    private string _hotkey = "";

    private ToolbarCustomizationItemViewModel(
        string id,
        EditorTool? tool,
        string name,
        string icon,
        string hotkey,
        bool isVisible,
        bool beginGroup,
        bool isHotkeyEditable,
        bool isVisibilityEditable)
    {
        Id = id;
        Tool = tool;
        Name = name;
        Icon = icon;
        _hotkey = hotkey;
        _isVisible = isVisible;
        _beginGroup = beginGroup;
        IsHotkeyEditable = isHotkeyEditable;
        IsVisibilityEditable = isVisibilityEditable;
    }

    public string Id { get; }
    public bool IsFileMenu => string.Equals(Id, FileItemId, StringComparison.OrdinalIgnoreCase);
    public bool IsRegularButton => !IsFileMenu;
    public bool IsTool => Tool.HasValue;
    public EditorTool? Tool { get; }
    public bool IsHotkeyEditable { get; }
    public bool IsVisibilityEditable { get; }
    public string Name { get; }
    public string Icon { get; }

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (SetProperty(ref _isVisible, value))
            {
                OnPropertyChanged(nameof(IsButtonVisible));
                OnPropertyChanged(nameof(IsGroupSeparatorVisible));
            }
        }
    }

    public string Hotkey
    {
        get => _hotkey;
        set
        {
            if (SetProperty(ref _hotkey, value ?? ""))
            {
                OnPropertyChanged(nameof(ToolTip));
                OnPropertyChanged(nameof(HotkeyDisplayText));
            }
        }
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public bool CanMoveUp
    {
        get => _canMoveUp;
        set => SetProperty(ref _canMoveUp, value);
    }

    public bool CanMoveDown
    {
        get => _canMoveDown;
        set => SetProperty(ref _canMoveDown, value);
    }

    public bool BeginGroup
    {
        get => _beginGroup;
        set
        {
            if (SetProperty(ref _beginGroup, value))
            {
                OnPropertyChanged(nameof(IsGroupSeparatorVisible));
            }
        }
    }

    public bool IsButtonVisible => IsVisible;
    public bool IsGroupSeparatorVisible => IsVisible && BeginGroup;
    public string HotkeyDisplayText => Hotkey;

    public string ToolTip
    {
        get
        {
            string hotkey = Hotkey.Trim();
            if (Id == ImageEffectsItemId)
            {
                string header = IsHotkeyEditable && hotkey.Length > 0 ? $"{Name} ({hotkey})" : Name;
                return string.Format(Strings.ToolbarCustomizationItemViewModel_FavoriteImageEffectsRightClick, header);
            }

            return IsHotkeyEditable && hotkey.Length > 0 ? $"{Name} ({hotkey})" : Name;
        }
    }

    public ToolbarCustomizationItemViewModel Clone()
    {
        return new ToolbarCustomizationItemViewModel(Id, Tool, Name, Icon, Hotkey, IsVisible, BeginGroup, IsHotkeyEditable, IsVisibilityEditable)
        {
            IsActive = IsActive,
            CanMoveUp = CanMoveUp,
            CanMoveDown = CanMoveDown
        };
    }

    public ImageEditorToolbarItemOptions ToOptions()
    {
        return new ImageEditorToolbarItemOptions
        {
            Id = Id,
            BeginGroup = BeginGroup,
            IsVisible = !IsVisibilityEditable || IsVisible,
            Hotkey = IsHotkeyEditable ? Hotkey.Trim() : ""
        };
    }

    public static IReadOnlyList<ToolbarCustomizationItemViewModel> CreateDefaultItems()
    {
        return ItemDefinitions.Select(definition => CreateItem(definition)).ToList();
    }

    public static IReadOnlyList<ToolbarCustomizationItemViewModel> CreateFromOptions(IReadOnlyList<ImageEditorToolbarItemOptions>? options)
    {
        if (options == null || options.Count == 0)
        {
            return CreateDefaultItems();
        }

        List<ToolbarCustomizationItemViewModel> items = new();
        HashSet<string> usedToolIds = new(StringComparer.OrdinalIgnoreCase);

        foreach (ImageEditorToolbarItemOptions option in options)
        {
            if (ItemDefinitionsById.TryGetValue(option.Id, out ToolbarItemDefinition? definition))
            {
                items.Add(CreateItem(definition, option.IsVisible, option.Hotkey ?? definition.DefaultHotkey, option.BeginGroup));
                usedToolIds.Add(option.Id);
            }
        }

        foreach (ToolbarItemDefinition missingDefinition in ItemDefinitions.Where(definition => !usedToolIds.Contains(definition.Id)))
        {
            items.Add(CreateItem(missingDefinition));
        }

        return items.Count > 0 ? items : CreateDefaultItems();
    }

    private static ToolbarCustomizationItemViewModel CreateItem(ToolbarItemDefinition definition, bool isVisible = true, string? hotkey = null, bool? beginGroup = null)
    {
        return new ToolbarCustomizationItemViewModel(
            definition.Id,
            definition.Tool,
            definition.Name,
            definition.Icon,
            definition.IsHotkeyEditable ? hotkey ?? definition.DefaultHotkey : "",
            definition.IsVisibilityEditable ? isVisible : true,
            beginGroup ?? definition.BeginGroupByDefault,
            definition.IsHotkeyEditable,
            definition.IsVisibilityEditable);
    }

    private static ToolbarItemDefinition CreateToolDefinition(
        EditorTool tool,
        string name,
        string icon,
        string defaultHotkey,
        bool beginGroupByDefault = false)
    {
        return new ToolbarItemDefinition(tool.ToString(), tool, name, icon, defaultHotkey, true, true, beginGroupByDefault);
    }
}

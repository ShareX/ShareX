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

using ShareX.ImageEditor.Localization;

namespace ShareX.ImageEditor.Presentation.ViewModels;

/// <summary>
/// One place in the toolbar customization dialog: a toolbar item, or a spacer that starts a new group of items.
/// </summary>
public sealed class ToolbarLayoutSlot : ViewModelBase
{
    /// <summary>Width of a spacer with no extra space in the customization dialog's mock toolbar.</summary>
    public const double BaseSpacerSlotWidth = 15;

    private bool _isSelected;
    private int _spacerWidth;
    private bool _showLine;

    public ToolbarLayoutSlot(ToolbarCustomizationItemViewModel? item, int spacerWidth = 0, bool showLine = true)
    {
        Item = item;
        _spacerWidth = ToolbarCustomizationItemViewModel.ClampSpacerWidth(spacerWidth);
        _showLine = showLine;
    }

    /// <summary>The toolbar item, or null for a spacer.</summary>
    public ToolbarCustomizationItemViewModel? Item { get; }
    public bool IsSpacer => Item == null;
    public bool IsItem => Item != null;
    public string Icon => Item?.Icon ?? "";
    public string Name => Item?.Name ??
        (ShowLine ? Strings.ToolbarCustomizationDialogView_Spacer : Strings.ToolbarCustomizationDialogView_BlankSpacer);

    /// <summary>Tooltip on the mock toolbar, where spacers can also be resized.</summary>
    public string ToolbarToolTip => Item?.Name ?? Strings.ToolbarCustomizationDialogView_SpacerToolTip;

    /// <summary>False for items that must stay on the toolbar, such as the File menu.</summary>
    public bool CanRemove => Item == null || Item.IsVisibilityEditable;

    /// <summary>For a spacer, the extra space it adds. Changes while the spacer is being resized.</summary>
    public int SpacerWidth
    {
        get => _spacerWidth;
        set
        {
            if (SetProperty(ref _spacerWidth, ToolbarCustomizationItemViewModel.ClampSpacerWidth(value)))
            {
                OnPropertyChanged(nameof(SlotWidth));
            }
        }
    }

    /// <summary>For a spacer, whether it draws a separator line or only leaves space.</summary>
    public bool ShowLine
    {
        get => _showLine;
        set
        {
            if (SetProperty(ref _showLine, value))
            {
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    /// <summary>How wide a spacer is drawn in the mock toolbar.</summary>
    public double SlotWidth => BaseSpacerSlotWidth + SpacerWidth;

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

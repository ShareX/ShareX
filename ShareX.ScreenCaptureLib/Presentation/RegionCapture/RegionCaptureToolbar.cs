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

#nullable enable

using ShareX.Icons;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Presentation.ViewModels;
using ShareX.ScreenCaptureLib.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

public static class RegionCaptureToolbar
{
    public const string EllipseRegionItemId = "RegionEllipse";
    public const string FreehandRegionItemId = "RegionFreehand";

    // The ShareX 21 ellipse and freehand region tools. Hidden until turned on in the toolbar customization.
    private static IReadOnlyList<ToolbarHostItemDefinition> CreateRegionItems() =>
    [
        new(EllipseRegionItemId, Strings.RegionCaptureWindow_EllipseRegion, ImageEditorIcons.RegionEllipse, IsVisibleByDefault: false),
        new(FreehandRegionItemId, Strings.RegionCaptureWindow_FreehandRegion, ImageEditorIcons.RegionFreehand, IsVisibleByDefault: false)
    ];

    public static IReadOnlyList<ToolbarCustomizationItemViewModel> CreateItems(IReadOnlyList<ImageEditorToolbarItemOptions>? options = null)
    {
        return ToolbarCustomizationItemViewModel.CreateFromOptions(options, CreateRegionItems())
            .Where(item => item.IsHostItem ||
                (item.Tool.HasValue && item.Tool.Value is not EditorTool.Crop and not EditorTool.CutOut))
            .ToList();
    }

    internal static RegionShape? GetRegionShape(string id)
    {
        if (string.Equals(id, EllipseRegionItemId, StringComparison.OrdinalIgnoreCase))
        {
            return RegionShape.Ellipse;
        }

        if (string.Equals(id, FreehandRegionItemId, StringComparison.OrdinalIgnoreCase))
        {
            return RegionShape.Freehand;
        }

        return null;
    }

    /// <summary>Returns the toolbar item of a region shape, or null for the rectangle region, which has its own button.</summary>
    internal static string? GetItemId(RegionShape shape) => shape switch
    {
        RegionShape.Ellipse => EllipseRegionItemId,
        RegionShape.Freehand => FreehandRegionItemId,
        _ => null
    };
}

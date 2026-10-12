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

using Avalonia.Controls;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShareX.HistoryLib;

internal static class HistoryContextMenuHelpers
{
    public static void UpdateVisibility(ContextMenu menu, HistoryItem[] selected, HistoryWindowServices services)
    {
        bool single = selected.Length == 1;
        HistoryItem? item = single ? selected[0] : null;
        bool file = single && File.Exists(item!.FilePath);
        bool imageFile = file && FileHelpers.IsImageFile(item!.FilePath);
        bool textFile = file && FileHelpers.IsTextFile(item!.FilePath);
        bool hasFile = selected.Any(item => File.Exists(item.FilePath));
        bool hasPath = selected.Any(item => !string.IsNullOrWhiteSpace(item.FilePath));
        bool hasFolder = selected.Any(item => !string.IsNullOrWhiteSpace(Path.GetDirectoryName(item.FilePath)));
        bool hasUrl = selected.Any(item => !string.IsNullOrWhiteSpace(item.URL));
        bool hasShortUrl = selected.Any(item => !string.IsNullOrWhiteSpace(item.ShortenedURL));
        bool hasThumbnailUrl = selected.Any(item => !string.IsNullOrWhiteSpace(item.ThumbnailURL));
        bool hasDeletionUrl = selected.Any(item => !string.IsNullOrWhiteSpace(item.DeletionURL));
        bool hasImageUrl = selected.Any(item => !string.IsNullOrWhiteSpace(item.URL) && FileHelpers.IsImageFile(item.URL));
        bool hasLinkedImage = selected.Any(item => !string.IsNullOrWhiteSpace(item.URL) &&
            FileHelpers.IsImageFile(item.URL) && !string.IsNullOrWhiteSpace(item.ThumbnailURL));

        bool IsAvailable(MenuItem menuItem) => menuItem.Name switch
        {
            "OpenUrlMenu" => single && hasUrl,
            "OpenShortUrlMenu" => single && hasShortUrl,
            "OpenThumbnailUrlMenu" => single && hasThumbnailUrl,
            "OpenDeletionUrlMenu" => single && hasDeletionUrl,
            "OpenFileMenu" or "OpenFolderMenu" or "RenameMenu" => file,
            "CopyUrlMenu" or "CopyHtmlLinkMenu" or "CopyForumLinkMenu" or "CopyMarkdownLinkMenu" => hasUrl,
            "CopyShortUrlMenu" => hasShortUrl,
            "CopyThumbnailUrlMenu" => hasThumbnailUrl,
            "CopyDeletionUrlMenu" => hasDeletionUrl,
            "CopyFileMenu" or "DeleteFileMenu" => hasFile,
            "CopyImageMenu" => imageFile,
            "CopyTextMenu" => textFile,
            "CopyFilePathMenu" or "CopyFileNameMenu" or "CopyFileNameExtensionMenu" => hasPath,
            "CopyFolderMenu" => hasFolder,
            "CopyHtmlImageMenu" or "CopyForumImageMenu" or "CopyMarkdownImageMenu" => hasImageUrl,
            "CopyHtmlLinkedImageMenu" or "CopyForumLinkedImageMenu" or "CopyMarkdownLinkedImageMenu" => hasLinkedImage,
            "TagMenu" or "EditMenu" => single,
            "PreviewMenu" => imageFile && (services.ShowImage != null || services.ShowImages != null),
            "UploadMenu" => file && services.UploadFile != null,
            "EditImageMenu" => imageFile && services.EditImage != null,
            "TrimVideoMenu" => file && FileHelpers.IsVideoFile(item!.FilePath) && services.TrimVideo != null,
            "TrimAnimatedGifMenu" => file &&
                string.Equals(Path.GetExtension(item!.FilePath), ".gif", StringComparison.OrdinalIgnoreCase) &&
                services.TrimAnimatedGif != null,
            "PinMenu" => imageFile && services.PinToScreen != null,
            "AnalyzeMenu" => imageFile && services.AnalyzeImage != null,
            _ => selected.Length > 0
        };

        UpdateItems(menu.Items, IsAvailable);
    }

    private static bool UpdateItems(IEnumerable<object?> items, Func<MenuItem, bool> isAvailable)
    {
        bool hasVisibleItem = false;
        Separator? pendingSeparator = null;

        foreach (object? item in items)
        {
            if (item is Separator separator)
            {
                separator.IsVisible = false;
                if (hasVisibleItem) pendingSeparator ??= separator;
            }
            else if (item is MenuItem menuItem)
            {
                menuItem.IsVisible = menuItem.Items.Count > 0
                    ? UpdateItems(menuItem.Items, isAvailable)
                    : isAvailable(menuItem);
                if (!menuItem.IsVisible) continue;

                if (pendingSeparator != null)
                {
                    pendingSeparator.IsVisible = true;
                    pendingSeparator = null;
                }
                hasVisibleItem = true;
            }
        }

        return hasVisibleItem;
    }
}

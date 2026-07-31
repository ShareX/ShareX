#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

#nullable enable

using ShareX.AvaloniaUI.Theming;
using System.Collections.Generic;
using System.Linq;

namespace ShareX;

/// <summary>
/// Describes an action shown in the notification's hover toolbar. The order of
/// items in <see cref="NotificationWindowConfig.ActionButtons"/> is the display order.
/// </summary>
public sealed class NotificationActionButton
{
    public ToastClickAction Action { get; set; }
    public string? Label { get; set; }
    public string? Icon { get; set; }
    public bool DismissNotification { get; set; } = true;

    public NotificationActionButton()
    {
    }

    public NotificationActionButton(ToastClickAction action)
    {
        Action = action;
    }

    public NotificationActionButton(ToastClickAction action, string? label, string? icon = null)
    {
        Action = action;
        Label = label;
        Icon = icon;
    }

    public NotificationActionButton Clone() => new(Action, Label, Icon)
    {
        DismissNotification = DismissNotification
    };

    public static List<NotificationActionButton> CreateDefaultButtons() =>
    [
        new(ToastClickAction.CopyImageToClipboard),
        new(ToastClickAction.AnnotateImage),
        new(ToastClickAction.PinToScreen),
        new(ToastClickAction.Upload)
    ];

    public static List<NotificationActionButton> CloneButtons(IEnumerable<NotificationActionButton>? buttons) =>
        buttons?.Where(button => button != null).Select(button => button.Clone()).ToList() ?? [];

    public static (string Label, string Icon) GetDefaultPresentation(ToastClickAction action) => action switch
    {
        ToastClickAction.AnnotateImage => ("Edit", LucideIcons.pen_line),
        ToastClickAction.CopyImageToClipboard => ("Copy image", LucideIcons.copy),
        ToastClickAction.CopyFile => ("Copy file", LucideIcons.files),
        ToastClickAction.CopyFilePath => ("Copy path", LucideIcons.clipboard),
        ToastClickAction.CopyUrl => ("Copy link", LucideIcons.link),
        ToastClickAction.OpenFile => ("Open", LucideIcons.external_link),
        ToastClickAction.OpenFolder => ("Folder", LucideIcons.folder_open),
        ToastClickAction.OpenUrl => ("Open link", LucideIcons.external_link),
        ToastClickAction.Upload => ("Upload", LucideIcons.upload),
        ToastClickAction.PinToScreen => ("Pin", LucideIcons.pin),
        ToastClickAction.DeleteFile => ("Delete", LucideIcons.trash_2),
        _ => ("Close", LucideIcons.x)
    };
}

#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using ShareX.AvaloniaUI.Controls;
using ShareX.AvaloniaUI.Theming;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ShareX.UploadersLib;

public sealed class DestinationSettingsViewModel : INotifyPropertyChanged
{
    private SettingsNavigationItem? _selectedNavigationItem;

    internal static DestinationCategoryDefinition[] Categories { get; } =
    [
        Category("image-uploaders", "Image uploaders", LucideIcons.image,
            Page("imgur", "Imgur", "Imgur"), Page("imageshack", "ImageShack", "ImageShack"),
            Page("flickr", "Flickr", "Flickr"), Page("photobucket", "Photobucket", "Photobucket"),
            Page("chevereto", "Chevereto", "Chevereto"), Page("vgyme", "vgy.me", "Vgyme", "vgyme")),
        Category("text-uploaders", "Text uploaders", LucideIcons.file_text,
            Page("pastebin", "Pastebin", "Pastebin"), Page("paste-ee", "Paste.ee", "Paste_ee", "Pasteee"),
            Page("gist", "GitHub Gist", "Gist", "GitHubGist"), Page("upaste", "uPaste", "Upaste"),
            Page("hastebin", "Hastebin", "Hastebin"), Page("one-time-secret", "OneTimeSecret", "OneTimeSecret"),
            Page("pastie", "Pastie", "Pastie"), Page("privatebin", "PrivateBin", "PrivateBin")),
        Category("file-uploaders", "File uploaders", LucideIcons.file_up,
            Page("ftp", "FTP / FTPS / SFTP", "FTP", "FTP", "FTPS", "SFTP"),
            Page("dropbox", "Dropbox", "Dropbox"), Page("onedrive", "OneDrive", "OneDrive"),
            Page("google-drive", "Google Drive", "GoogleDrive"), Page("puush", "puush", "Puush"),
            Page("box", "Box", "Box"), Page("amazon-s3", "Amazon S3", "AmazonS3"),
            Page("google-cloud-storage", "Google Cloud Storage", "GoogleCloudStorage"),
            Page("azure-storage", "Azure Storage", "AzureStorage"), Page("backblaze-b2", "Backblaze B2", "B2", "BackblazeB2"),
            Page("owncloud", "ownCloud / Nextcloud", "OwnCloud", "ownCloud", "Nextcloud"),
            Page("mediafire", "MediaFire", "MediaFire"), Page("pushbullet", "Pushbullet", "Pushbullet"),
            Page("sendspace", "SendSpace", "SendSpace"), Page("hostr", "Hostr", "Localhostr", "Hostr"),
            Page("lambda", "Lambda", "Lambda"), Page("lobfile", "LobFile", "Lithiio", "LobFile"),
            Page("pomf", "Pomf", "Pomf"), Page("seafile", "Seafile", "Seafile"), Page("sul", "s-ul", "Sul", "sul"),
            Page("streamable", "Streamable", "Streamable"), Page("plik", "Plik", "Plik"),
            Page("youtube", "YouTube", "YouTube"),
            new DestinationPageDefinition("shared-folder", "Shared folder", ["LocalhostAccount", "LocalhostSelected"], "SharedFolder"),
            Page("email", "Email", "Email")),
        Category("url-shorteners", "URL shorteners", LucideIcons.link_2,
            Page("bitly", "bit.ly", "Bitly", "Bitly"), Page("yourls", "YOURLS", "Yourls"),
            Page("polr", "Polr", "Polr"), Page("firebase", "Firebase Dynamic Links", "Firebase"),
            Page("kutt", "Kutt", "Kutt"), Page("zero-width", "Zero Width Shortener", "ZeroWidthShortener"))
    ];

    public ObservableCollection<SettingsNavigationItem> NavigationItems { get; } = [];

    public SettingsNavigationItem? SelectedNavigationItem
    {
        get => _selectedNavigationItem;
        set
        {
            if (value is { Children.Count: > 0 })
            {
                value = value.Children[0];
            }

            if (ReferenceEquals(_selectedNavigationItem, value)) return;
            _selectedNavigationItem = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedNavigationItem)));
            SelectedPageChanged?.Invoke(value?.Id);
        }
    }

    public event Action<string?>? SelectedPageChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    public DestinationSettingsViewModel()
    {
        foreach (DestinationCategoryDefinition category in Categories)
        {
            NavigationItems.Add(new SettingsNavigationItem(category.Id, category.Title, category.Icon,
                children: category.Pages.Select(page => new SettingsNavigationItem(page.Id, page.Title))));
        }

        SelectedNavigationItem = NavigationItems[0].Children[0];
    }

    public void NavigateTo(string? pageId)
    {
        SettingsNavigationItem? item = NavigationItems.SelectMany(x => x.Children).FirstOrDefault(x => x.Id == pageId);
        if (item != null) SelectedNavigationItem = item;
    }

    private static DestinationCategoryDefinition Category(string id, string title, string icon, params DestinationPageDefinition[] pages) => new(id, title, icon, pages);
    private static DestinationPageDefinition Page(string id, string title, string prefix, params string[] aliases) => new(id, title, [prefix], aliases);
}

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

namespace ShareX.Tools;

public sealed class OCROptions
{
    public string Language { get; set; } = "en";
    public float ScaleFactor { get; set; } = 2f;
    public bool SingleLine { get; set; }
    public bool Silent { get; set; }
    public bool AutoCopy { get; set; }
    public List<OCRServiceLinkOption> ServiceLinks { get; set; } = DefaultServiceLinks;
    public bool CloseWindowAfterOpeningServiceLink { get; set; }
    public int SelectedServiceLink { get; set; }

    public static List<OCRServiceLinkOption> DefaultServiceLinks =>
    [
        new("Google Translate", "https://translate.google.com/?sl=auto&tl=en&text={0}&op=translate"),
        new("Google Search", "https://www.google.com/search?q={0}"),
        new("Google Images", "https://www.google.com/search?q={0}&tbm=isch"),
        new("Bing", "https://www.bing.com/search?q={0}"),
        new("DuckDuckGo", "https://duckduckgo.com/?q={0}"),
        new("DeepL", "https://www.deepl.com/translator#auto/en/{0}")
    ];
}

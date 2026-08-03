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
            new(Localization.Strings.OCROptions_Google_Translate, "https://translate.google.com/?sl=auto&tl=en&text={0}&op=translate"),
            new(Localization.Strings.OCROptions_Google_Search, "https://www.google.com/search?q={0}"),
            new(Localization.Strings.OCROptions_Google_Images, "https://www.google.com/search?q={0}&tbm=isch"),
            new(Localization.Strings.OCROptions_Bing, "https://www.bing.com/search?q={0}"),
            new(Localization.Strings.OCROptions_DuckDuckGo, "https://duckduckgo.com/?q={0}"),
            new(Localization.Strings.OCROptions_DeepL, "https://www.deepl.com/translator#auto/en/{0}")
    ];
}

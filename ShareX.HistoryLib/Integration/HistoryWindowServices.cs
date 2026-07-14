#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using System;
using System.Collections.Generic;

namespace ShareX.HistoryLib;

public sealed class HistoryWindowServices
{
    public Action<string>? UploadFile { get; init; }
    public Action<string>? EditImage { get; init; }
    public Action<string>? PinToScreen { get; init; }
    public Action<string>? AnalyzeImage { get; init; }
    public Action<string>? ShowImage { get; init; }
    public Action<IReadOnlyList<string>, int>? ShowImages { get; init; }
}

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

public enum AnalyzeImageProvider
{
    OpenAI,
    Gemini,
    OpenRouter,
    OpenAILegacy
}

public sealed class AnalyzeImageOptions
{
    public AnalyzeImageProvider Provider { get; set; } = AnalyzeImageProvider.OpenAI;
    public string? OpenAIAPIKey { get; set; }
    public string OpenAIModel { get; set; } = "gpt-5-mini";
    public string? OpenAICustomURL { get; set; }
    public string OpenAIReasoningEffort { get; set; } = "minimal";
    public string OpenAIVerbosity { get; set; } = "medium";
    public string? GeminiAPIKey { get; set; }
    public string GeminiModel { get; set; } = "gemini-1.5-flash-latest";
    public string? OpenRouterAPIKey { get; set; }
    public string OpenRouterModel { get; set; } = "google/gemini-flash-1.5";
    public string Input { get; set; } = "What is in this image?";
    public bool AutoStartRegion { get; set; } = true;
    public bool AutoStartAnalyze { get; set; } = true;
    public bool AutoCopyResult { get; set; }

    public bool HasAPIKey => Provider switch
    {
        AnalyzeImageProvider.OpenAI or AnalyzeImageProvider.OpenAILegacy => !string.IsNullOrWhiteSpace(OpenAIAPIKey),
        AnalyzeImageProvider.Gemini => !string.IsNullOrWhiteSpace(GeminiAPIKey),
        AnalyzeImageProvider.OpenRouter => !string.IsNullOrWhiteSpace(OpenRouterAPIKey),
        _ => false
    };

    public AnalyzeImageOptions Clone() => new()
    {
        Provider = Provider,
        OpenAIAPIKey = OpenAIAPIKey,
        OpenAIModel = OpenAIModel,
        OpenAICustomURL = OpenAICustomURL,
        OpenAIReasoningEffort = OpenAIReasoningEffort,
        OpenAIVerbosity = OpenAIVerbosity,
        GeminiAPIKey = GeminiAPIKey,
        GeminiModel = GeminiModel,
        OpenRouterAPIKey = OpenRouterAPIKey,
        OpenRouterModel = OpenRouterModel,
        Input = Input,
        AutoStartRegion = AutoStartRegion,
        AutoStartAnalyze = AutoStartAnalyze,
        AutoCopyResult = AutoCopyResult
    };

    public void CopyFrom(AnalyzeImageOptions source)
    {
        Provider = source.Provider;
        OpenAIAPIKey = source.OpenAIAPIKey;
        OpenAIModel = source.OpenAIModel;
        OpenAICustomURL = source.OpenAICustomURL;
        OpenAIReasoningEffort = source.OpenAIReasoningEffort;
        OpenAIVerbosity = source.OpenAIVerbosity;
        GeminiAPIKey = source.GeminiAPIKey;
        GeminiModel = source.GeminiModel;
        OpenRouterAPIKey = source.OpenRouterAPIKey;
        OpenRouterModel = source.OpenRouterModel;
        Input = source.Input;
        AutoStartRegion = source.AutoStartRegion;
        AutoStartAnalyze = source.AutoStartAnalyze;
        AutoCopyResult = source.AutoCopyResult;
    }
}

public sealed record AnalyzeImageConnectionResult(bool Success, string Message);

public delegate Task<string> AnalyzeImageHandler(string? imagePath, byte[]? imageData, AnalyzeImageOptions options);
public delegate Task<byte[]?> AnalyzeImageRegionCaptureHandler();
public delegate Task<AnalyzeImageConnectionResult> AnalyzeImageTestConnectionHandler(AnalyzeImageOptions options);
public delegate Task<IReadOnlyList<string>> AnalyzeImageLoadModelsHandler(AnalyzeImageOptions options);

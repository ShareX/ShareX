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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.HelpersLib;
using System.Collections.ObjectModel;

namespace ShareX.Tools;

public sealed partial class AnalyzeImageOptionsViewModel : ViewModelBase
{
    private readonly AIOptions _target;
    private readonly AnalyzeImageService _service;

    public IReadOnlyList<AIProvider> Providers { get; } = Enum.GetValues<AIProvider>();
    public ObservableCollection<string> OpenAIModels { get; } = ["gpt-5.2", "gpt-5.1", "gpt-5", "gpt-5-mini", "gpt-5-nano"];
    public ObservableCollection<string> GeminiModels { get; } = ["gemini-2.0-flash", "gemini-2.0-flash-lite", "gemini-1.5-flash", "gemini-1.5-pro"];
    public ObservableCollection<string> OpenRouterModels { get; } = ["openai/gpt-4o", "anthropic/claude-3.5-sonnet", "google/gemini-pro-1.5"];
    public IReadOnlyList<string> ReasoningEfforts { get; } = ["minimal", "low", "medium", "high"];
    public IReadOnlyList<string> VerbosityLevels { get; } = ["low", "medium", "high"];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOpenAI))]
    [NotifyPropertyChangedFor(nameof(IsGemini))]
    [NotifyPropertyChangedFor(nameof(IsOpenRouter))]
    private AIProvider _provider;

    [ObservableProperty] private string? _openAIAPIKey;
    [ObservableProperty] private string _openAIModel;
    [ObservableProperty] private string? _openAICustomURL;
    [ObservableProperty] private string _openAIReasoningEffort;
    [ObservableProperty] private string _openAIVerbosity;
    [ObservableProperty] private string? _geminiAPIKey;
    [ObservableProperty] private string _geminiModel;
    [ObservableProperty] private string? _openRouterAPIKey;
    [ObservableProperty] private string _openRouterModel;
    [ObservableProperty] private bool _autoStartRegion;
    [ObservableProperty] private bool _autoStartAnalyze;
    [ObservableProperty] private bool _autoCopyResult;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _statusText = string.Empty;
    [ObservableProperty] private bool _statusSuccess;

    public Action<bool>? CloseRequested { get; set; }

    public bool IsOpenAI => Provider is AIProvider.OpenAI or AIProvider.OpenAILegacy;
    public bool IsGemini => Provider == AIProvider.Gemini;
    public bool IsOpenRouter => Provider == AIProvider.OpenRouter;
    public bool HasStatus => !string.IsNullOrWhiteSpace(StatusText);

    public AnalyzeImageOptionsViewModel(AIOptions target, AnalyzeImageService service)
    {
        _target = target;
        _service = service;

        AIOptions draft = target.Clone();
        _provider = draft.Provider;
        _openAIAPIKey = draft.OpenAIAPIKey;
        _openAIModel = draft.OpenAIModel;
        _openAICustomURL = draft.OpenAICustomURL;
        _openAIReasoningEffort = draft.OpenAIReasoningEffort;
        _openAIVerbosity = draft.OpenAIVerbosity;
        _geminiAPIKey = draft.GeminiAPIKey;
        _geminiModel = draft.GeminiModel;
        _openRouterAPIKey = draft.OpenRouterAPIKey;
        _openRouterModel = draft.OpenRouterModel;
        _autoStartRegion = draft.AutoStartRegion;
        _autoStartAnalyze = draft.AutoStartAnalyze;
        _autoCopyResult = draft.AutoCopyResult;

        AddCurrentModel(OpenAIModels, OpenAIModel);
        AddCurrentModel(GeminiModels, GeminiModel);
        AddCurrentModel(OpenRouterModels, OpenRouterModel);
    }

    partial void OnStatusTextChanged(string value) => OnPropertyChanged(nameof(HasStatus));

    [RelayCommand]
    private async Task TestConnectionAsync()
    {
        IsBusy = true;
        StatusSuccess = false;
        StatusText = "Testing...";

        try
        {
            AnalyzeImageConnectionResult result = await _service.TestConnectionAsync(CreateOptions());
            StatusSuccess = result.Success;
            StatusText = result.Message;
        }
        catch (Exception ex)
        {
            StatusText = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadModelsAsync()
    {
        IsBusy = true;
        StatusSuccess = false;
        StatusText = "Loading models...";

        try
        {
            IReadOnlyList<string> models = await _service.LoadModelsAsync(CreateOptions());
            OpenAIModels.Clear();
            foreach (string model in models)
            {
                OpenAIModels.Add(model);
            }

            if (models.Count > 0)
            {
                OpenAIModel = models.Contains(OpenAIModel) ? OpenAIModel : models[0];
                StatusSuccess = true;
                StatusText = $"{models.Count} models loaded.";
            }
            else
            {
                StatusText = "No models found.";
            }
        }
        catch (Exception ex)
        {
            StatusText = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void OpenAPIKeyPage()
    {
        string url = Provider switch
        {
            AIProvider.OpenAI or AIProvider.OpenAILegacy => "https://platform.openai.com/api-keys",
            AIProvider.Gemini => "https://aistudio.google.com/app/apikey",
            AIProvider.OpenRouter => "https://openrouter.ai/keys",
            _ => string.Empty
        };

        if (!string.IsNullOrWhiteSpace(url))
        {
            URLHelpers.OpenURL(url);
        }
    }

    [RelayCommand]
    private void Save()
    {
        AIOptions updated = CreateOptions();
        updated.Input = _target.Input;
        _target.CopyFrom(updated);
        CloseRequested?.Invoke(true);
    }

    [RelayCommand]
    private void Cancel() => CloseRequested?.Invoke(false);

    private AIOptions CreateOptions() => new()
    {
        Provider = Provider,
        OpenAIAPIKey = OpenAIAPIKey,
        OpenAIModel = OpenAIModel ?? string.Empty,
        OpenAICustomURL = OpenAICustomURL,
        OpenAIReasoningEffort = OpenAIReasoningEffort ?? "minimal",
        OpenAIVerbosity = OpenAIVerbosity ?? "medium",
        GeminiAPIKey = GeminiAPIKey,
        GeminiModel = GeminiModel ?? string.Empty,
        OpenRouterAPIKey = OpenRouterAPIKey,
        OpenRouterModel = OpenRouterModel ?? string.Empty,
        AutoStartRegion = AutoStartRegion,
        AutoStartAnalyze = AutoStartAnalyze,
        AutoCopyResult = AutoCopyResult
    };

    private static void AddCurrentModel(ObservableCollection<string> models, string model)
    {
        if (!string.IsNullOrWhiteSpace(model) && !models.Contains(model))
        {
            models.Add(model);
        }
    }
}

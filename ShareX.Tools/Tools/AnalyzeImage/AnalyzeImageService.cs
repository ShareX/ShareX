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

using ShareX.HelpersLib;
using System.Drawing;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ShareX.Tools;

public sealed class AnalyzeImageService
{
    public async Task<string> AnalyzeAsync(string? imagePath, byte[]? imageData, AIOptions options)
    {
        IAIProvider provider = AIProviderFactory.GetProvider(options);

        if (!string.IsNullOrWhiteSpace(imagePath))
        {
            return await provider.AnalyzeImage(imagePath, options.Input, options.OpenAIReasoningEffort, options.OpenAIVerbosity);
        }

        if (imageData is not { Length: > 0 })
        {
            return string.Empty;
        }

        using Bitmap image = ImageHelpers.ByteArrayToBitmap(imageData);
        return await provider.AnalyzeImage(image, options.Input, options.OpenAIReasoningEffort, options.OpenAIVerbosity);
    }

    public async Task<AnalyzeImageConnectionResult> TestConnectionAsync(AIOptions options)
    {
        using HttpRequestMessage? request = CreateModelsRequest(options, out string? error);
        if (request == null)
        {
            return new AnalyzeImageConnectionResult(false, error ?? "Unable to create request.");
        }

        HttpClient client = HttpClientFactory.Create();
        using HttpResponseMessage response = await client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return new AnalyzeImageConnectionResult(true, "Connection OK.");
        }

        string summary = string.IsNullOrWhiteSpace(response.ReasonPhrase) ? response.StatusCode.ToString() : response.ReasonPhrase;
        if (!string.IsNullOrWhiteSpace(content))
        {
            summary += ": " + (content.Length > 200 ? content[..200] + "..." : content);
        }

        return new AnalyzeImageConnectionResult(false, summary);
    }

    public async Task<IReadOnlyList<string>> LoadModelsAsync(AIOptions options)
    {
        if (options.Provider is not (AIProvider.OpenAI or AIProvider.OpenAILegacy))
        {
            return [];
        }

        using HttpRequestMessage? request = CreateModelsRequest(options, out string? error);
        if (request == null)
        {
            throw new InvalidOperationException(error);
        }

        HttpClient client = HttpClientFactory.Create();
        using HttpResponseMessage response = await client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        using JsonDocument document = JsonDocument.Parse(content);
        return document.RootElement.GetProperty("data")
            .EnumerateArray()
            .Select(x => x.GetProperty("id").GetString())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static HttpRequestMessage? CreateModelsRequest(AIOptions options, out string? error)
    {
        error = null;

        switch (options.Provider)
        {
            case AIProvider.OpenAI:
            case AIProvider.OpenAILegacy:
                if (string.IsNullOrWhiteSpace(options.OpenAIAPIKey))
                {
                    error = "Missing OpenAI API key.";
                    return null;
                }

                string baseURL = string.IsNullOrWhiteSpace(options.OpenAICustomURL) ? "https://api.openai.com" : options.OpenAICustomURL;
                HttpRequestMessage openAIRequest = new(HttpMethod.Get, URLHelpers.CombineURL(baseURL, "v1/models"));
                openAIRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.OpenAIAPIKey.Trim());
                return openAIRequest;

            case AIProvider.Gemini:
                if (string.IsNullOrWhiteSpace(options.GeminiAPIKey))
                {
                    error = "Missing Gemini API key.";
                    return null;
                }

                return new HttpRequestMessage(HttpMethod.Get,
                    "https://generativelanguage.googleapis.com/v1beta/models?key=" + Uri.EscapeDataString(options.GeminiAPIKey.Trim()));

            case AIProvider.OpenRouter:
                if (string.IsNullOrWhiteSpace(options.OpenRouterAPIKey))
                {
                    error = "Missing OpenRouter API key.";
                    return null;
                }

                HttpRequestMessage openRouterRequest = new(HttpMethod.Get, "https://openrouter.ai/api/v1/models");
                openRouterRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.OpenRouterAPIKey.Trim());
                return openRouterRequest;

            default:
                error = "Select a provider first.";
                return null;
        }
    }
}

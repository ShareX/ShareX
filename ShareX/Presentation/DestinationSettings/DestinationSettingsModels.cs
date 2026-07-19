#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using System.Linq;

namespace ShareX;

internal sealed record DestinationPageDefinition(string Id, string Title, string[] Prefixes, params string[] Aliases)
{
    public bool MatchesService(string serviceName, string serviceIdentifier)
    {
        string name = Normalize(serviceName);
        string identifier = Normalize(serviceIdentifier);
        return Normalize(Title) == name || Normalize(Title) == identifier ||
            Aliases.Any(x => Normalize(x) == name || Normalize(x) == identifier);
    }

    private static string Normalize(string value) => new(value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
}

internal sealed record DestinationCategoryDefinition(string Id, string Title, string Icon, DestinationPageDefinition[] Pages);

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareX.Platform.Linux;

/// <summary>Reads and writes freedesktop.org .desktop files (Desktop Entry Specification 1.5).</summary>
internal static class DesktopEntry
{
    // Characters that force an Exec argument to be quoted.
    private const string ReservedCharacters = " \t\n\"'\\><~|&;$*?#()`";

    /// <summary>Builds an Exec value, quoting arguments as the specification requires.</summary>
    public static string BuildExec(string executable, IEnumerable<string> arguments, bool appendFileCode = false)
    {
        IEnumerable<string> parts = new[] { executable }.Concat(arguments).Select(QuoteArgument);

        if (appendFileCode)
        {
            parts = parts.Append("%F");
        }

        return string.Join(" ", parts);
    }

    public static string QuoteArgument(string argument)
    {
        // Percent signs are field codes, a literal one is written as %%.
        string value = argument.Replace("%", "%%");

        if (value.Length > 0 && value.IndexOfAny(ReservedCharacters.ToCharArray()) < 0)
        {
            return value;
        }

        StringBuilder builder = new StringBuilder(value.Length + 2);
        builder.Append('"');

        foreach (char c in value)
        {
            if (c is '"' or '`' or '$' or '\\')
            {
                builder.Append('\\');
            }

            builder.Append(c);
        }

        builder.Append('"');
        return builder.ToString();
    }

    /// <summary>Escapes a string value (Name, Comment). Backslashes and line breaks must be escaped.</summary>
    public static string EscapeValue(string value) =>
        value.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");

    /// <summary>Splits an Exec value into arguments, reversing <see cref="BuildExec"/>. Field codes are dropped.</summary>
    public static List<string> ParseExec(string exec)
    {
        List<string> arguments = new List<string>();
        StringBuilder current = new StringBuilder();
        bool inQuotes = false;
        bool hasToken = false;

        for (int i = 0; i < exec.Length; i++)
        {
            char c = exec[i];

            if (inQuotes)
            {
                if (c == '\\' && i + 1 < exec.Length)
                {
                    current.Append(exec[++i]);
                }
                else if (c == '"')
                {
                    inQuotes = false;
                }
                else
                {
                    current.Append(c);
                }
            }
            else if (c == '"')
            {
                inQuotes = true;
                hasToken = true;
            }
            else if (char.IsWhiteSpace(c))
            {
                Flush();
            }
            else
            {
                current.Append(c);
                hasToken = true;
            }
        }

        Flush();
        return arguments;

        void Flush()
        {
            if (!hasToken)
            {
                return;
            }

            string token = current.ToString();
            current.Clear();
            hasToken = false;

            if (token.Length == 2 && token[0] == '%' && token != "%%")
            {
                return;
            }

            arguments.Add(token.Replace("%%", "%"));
        }
    }

    /// <summary>Parses the [Desktop Entry] group into key value pairs. Localised keys such as Name[de] are kept as is.</summary>
    public static Dictionary<string, string> Parse(string content, string group = "Desktop Entry")
    {
        Dictionary<string, string> values = new Dictionary<string, string>(StringComparer.Ordinal);
        bool inGroup = false;

        foreach (string rawLine in content.Split('\n'))
        {
            string line = rawLine.TrimEnd('\r');

            if (line.StartsWith('['))
            {
                inGroup = line == $"[{group}]";
                continue;
            }

            if (!inGroup || line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            int equals = line.IndexOf('=');

            if (equals > 0)
            {
                values[line[..equals].Trim()] = line[(equals + 1)..].Trim();
            }
        }

        return values;
    }
}

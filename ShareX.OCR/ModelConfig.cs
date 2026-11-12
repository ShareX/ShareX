#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System.Globalization;
using System.Text.RegularExpressions;

namespace ShareX.OCR;

// Reads the few values the OCR pipeline needs from a PaddleOCR inference.yml file. The files are plain YAML
// written by PyYAML, so reading "key: value" lines and "- item" lists is enough and avoids a YAML dependency.
internal sealed class ModelConfig
{
    private readonly string[] _lines;

    private ModelConfig(string[] lines)
    {
        _lines = lines;
    }

    public static ModelConfig Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The OCR model configuration file is missing.", filePath);
        }

        return new ModelConfig(File.ReadAllLines(filePath));
    }

    public float GetFloat(string key, float defaultValue)
    {
        string? value = GetScalar(key);
        return value != null && float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result) ? result : defaultValue;
    }

    public int GetInt(string key, int defaultValue)
    {
        string? value = GetScalar(key);
        return value != null && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ? result : defaultValue;
    }

    public float[] GetFloats(string key, float[] defaultValue)
    {
        List<string> items = GetList(key);
        float[] values = new float[items.Count];

        for (int i = 0; i < items.Count; i++)
        {
            if (!float.TryParse(items[i], NumberStyles.Float, CultureInfo.InvariantCulture, out values[i]))
            {
                return defaultValue;
            }
        }

        return values.Length == defaultValue.Length ? values : defaultValue;
    }

    // The first "key: value" line with a value, at any depth.
    public string? GetScalar(string key)
    {
        string prefix = key + ":";

        foreach (string line in _lines)
        {
            string trimmed = line.Trim();

            if (trimmed.StartsWith(prefix, StringComparison.Ordinal))
            {
                string value = trimmed[prefix.Length..].Trim();

                if (value.Length > 0)
                {
                    return Unquote(value);
                }
            }
        }

        return null;
    }

    // The "- item" lines that follow the first "key:" line. PyYAML writes the items at the indentation of the key.
    public List<string> GetList(string key)
    {
        string header = key + ":";
        List<string> items = [];

        for (int i = 0; i < _lines.Length; i++)
        {
            string line = _lines[i];
            string trimmed = line.Trim();

            if (trimmed != header)
            {
                continue;
            }

            int indent = GetIndent(line);

            for (int j = i + 1; j < _lines.Length; j++)
            {
                string itemLine = _lines[j].TrimEnd('\r', '\n');
                int itemIndent = GetIndent(itemLine);

                if (itemIndent < indent || itemLine.Length < itemIndent + 2 || itemLine[itemIndent] != '-' || itemLine[itemIndent + 1] != ' ')
                {
                    break;
                }

                items.Add(Unquote(itemLine[(itemIndent + 2)..]));
            }

            break;
        }

        return items;
    }

    private static int GetIndent(string line)
    {
        int indent = 0;

        while (indent < line.Length && line[indent] == ' ')
        {
            indent++;
        }

        return indent;
    }

    private static string Unquote(string value)
    {
        if (value.Length >= 2 && value[0] == '\'' && value[^1] == '\'')
        {
            return value[1..^1].Replace("''", "'");
        }

        if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
        {
            return Regex.Unescape(value[1..^1]);
        }

        // A plain scalar cannot end with spaces, so anything after the value is formatting.
        return value.TrimEnd();
    }
}

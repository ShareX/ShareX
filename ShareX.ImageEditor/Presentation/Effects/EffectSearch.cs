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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareX.ImageEditor.Presentation.Effects;

/// <summary>
/// Ranks effects for the effect browser search. Matches the name first, then extra keywords and the description,
/// ignores case, punctuation and filler words, and allows one typo in longer words.
/// </summary>
public static class EffectSearch
{
    // Plain-language words people search for that are not in the effect name.
    private static readonly Dictionary<string, string> Keywords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["black_and_white"] = "bw b&w monochrome mono",
        ["grayscale"] = "grey gray greyscale desaturate monochrome",
        ["blur"] = "blurry soften smooth",
        ["gaussian_blur"] = "blurry soften",
        ["rotate_90_clockwise"] = "turn right",
        ["rotate_90_counter_clockwise"] = "turn left",
        ["rotate_180"] = "turn upside down",
        ["rotate_custom_angle"] = "turn angle tilt straighten",
        ["resize_image"] = "smaller bigger shrink enlarge scale size dimensions",
        ["resize_canvas"] = "padding expand canvas size",
        ["brightness"] = "brighter darker lighten darken light",
        ["sharpen"] = "sharper crisp clarity detail",
        ["pixelate"] = "censor hide mosaic blocks",
        ["shadow"] = "drop shadow",
        ["invert"] = "negative inverse colors",
        ["flip_horizontal"] = "mirror",
        ["flip_vertical"] = "mirror upside down",
        ["rounded_corners"] = "round corners radius",
        ["remove_background"] = "transparent background remove bg cutout",
        ["text_watermark"] = "watermark copyright",
        ["border"] = "frame outline edge",
        ["add_noise"] = "grain",
        ["sepia"] = "old vintage brown",
        ["crop_image"] = "trim cut",
        ["vignette"] = "dark edges"
    };

    private static readonly HashSet<string> FillerWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "make", "the", "a", "an", "my", "image", "picture", "photo", "screenshot", "it", "to", "more", "add"
    };

    /// <summary>Returns 0 when the effect does not match, otherwise a higher number for a better match.</summary>
    public static int Score(string query, string name, string description, string? effectId)
    {
        string q = Normalize(query, removeFillerWords: true);
        if (q.Length == 0)
        {
            return 0;
        }

        string label = Normalize(name);
        string keywords = effectId != null && Keywords.TryGetValue(effectId, out string? words) ? Normalize(words) : string.Empty;
        string[] tokens = q.Split(' ');
        string[] labelWords = label.Split(' ');
        string[] keywordWords = keywords.Length > 0 ? keywords.Split(' ') : [];

        if (q == label) return 1000;
        if (label.StartsWith(q, StringComparison.Ordinal)) return 900;
        if (tokens.All(t => MatchesAnyWord(t, labelWords))) return 800;

        string compact = q.Replace(" ", "");
        if (label.Replace(" ", "").Contains(compact, StringComparison.Ordinal) ||
            label.Replace(" and ", " ").Replace(" ", "").Contains(compact, StringComparison.Ordinal)) return 700;

        if (keywords.Contains(q, StringComparison.Ordinal) || tokens.All(t => MatchesAnyWord(t, keywordWords))) return 600;
        if (tokens.All(t => MatchesAnyWord(t, labelWords) || MatchesAnyWord(t, keywordWords))) return 500;

        string desc = Normalize(description);
        if (desc.Contains(q, StringComparison.Ordinal) || tokens.All(t => MatchesAnyWord(t, desc.Split(' ')))) return 300;

        return 0;
    }

    private static bool MatchesAnyWord(string token, string[] words)
    {
        foreach (string word in words)
        {
            if (word.StartsWith(token, StringComparison.Ordinal))
            {
                return true;
            }

            if (token.Length >= 5 && IsOneEditApart(token, word.Length > token.Length ? word[..(token.Length + 1)] : word))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsOneEditApart(string a, string b)
    {
        if (Math.Abs(a.Length - b.Length) > 1)
        {
            return false;
        }

        if (a.Length > b.Length)
        {
            (a, b) = (b, a);
        }

        int i = 0, j = 0, edits = 0;
        while (i < a.Length && j < b.Length)
        {
            if (a[i] != b[j])
            {
                if (++edits > 1)
                {
                    return false;
                }

                if (a.Length == b.Length)
                {
                    i++;
                }

                j++;
            }
            else
            {
                i++;
                j++;
            }
        }

        return edits + (b.Length - j) <= 1;
    }

    private static string Normalize(string text, bool removeFillerWords = false)
    {
        StringBuilder sb = new(text.Length + 8);
        foreach (char c in text.ToLowerInvariant())
        {
            if (c == '&') sb.Append(" and ");
            else if (char.IsLetterOrDigit(c)) sb.Append(c);
            else sb.Append(' ');
        }

        IEnumerable<string> words = sb.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (removeFillerWords)
        {
            string[] kept = words.Where(w => !FillerWords.Contains(w)).ToArray();
            if (kept.Length > 0)
            {
                words = kept;
            }
        }

        return string.Join(' ', words);
    }
}

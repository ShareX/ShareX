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

using Avalonia;
using Avalonia.Media;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ShareX.AvaloniaUI.Windows;

public static partial class ScreenColorPickerTextFormatter
{
    public static string Format(string? format, Color color, PixelPoint position)
    {
        if (string.IsNullOrEmpty(format))
        {
            return string.Empty;
        }

        string upperHex = $"{color.R:X2}{color.G:X2}{color.B:X2}";
        (double cyan, double magenta, double yellow, double key) = ToCmyk(color);

        string text = format
            .Replace("$r255", color.R.ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("$g255", color.G.ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("$b255", color.B.ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("$rHEX", color.R.ToString("X2"), StringComparison.Ordinal)
            .Replace("$gHEX", color.G.ToString("X2"), StringComparison.Ordinal)
            .Replace("$bHEX", color.B.ToString("X2"), StringComparison.Ordinal)
            .Replace("$rhex", color.R.ToString("x2"), StringComparison.OrdinalIgnoreCase)
            .Replace("$ghex", color.G.ToString("x2"), StringComparison.OrdinalIgnoreCase)
            .Replace("$bhex", color.B.ToString("x2"), StringComparison.OrdinalIgnoreCase)
            .Replace("$HEX", upperHex, StringComparison.Ordinal)
            .Replace("$hex", upperHex.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase)
            .Replace("$c100", Math.Round(cyan * 100, 2, MidpointRounding.AwayFromZero).ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("$m100", Math.Round(magenta * 100, 2, MidpointRounding.AwayFromZero).ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("$y100", Math.Round(yellow * 100, 2, MidpointRounding.AwayFromZero).ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("$k100", Math.Round(key * 100, 2, MidpointRounding.AwayFromZero).ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("$name", GetColorName(color), StringComparison.OrdinalIgnoreCase)
            .Replace("$x", position.X.ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("$y", position.Y.ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("$n", Environment.NewLine, StringComparison.OrdinalIgnoreCase);

        text = ReplaceNormalizedComponents(text, color);

        return text
            .Replace("$r", color.R.ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("$g", color.G.ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("$b", color.B.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    private static string ReplaceNormalizedComponents(string text, Color color)
    {
        return NormalizedComponentRegex().Replace(text, match =>
        {
            char component = char.ToLowerInvariant(match.Groups[1].Value[0]);
            int precision = 3;

            if (match.Groups[2].Success && int.TryParse(match.Groups[2].Value, out int requestedPrecision))
            {
                precision = Math.Clamp(requestedPrecision, 0, 15);
            }

            byte value = component switch
            {
                'r' => color.R,
                'g' => color.G,
                _ => color.B
            };

            return Math.Round(value / 255d, precision, MidpointRounding.AwayFromZero)
                .ToString(CultureInfo.CurrentCulture);
        });
    }

    private static (double Cyan, double Magenta, double Yellow, double Key) ToCmyk(Color color)
    {
        double red = color.R / 255d;
        double green = color.G / 255d;
        double blue = color.B / 255d;
        double key = 1 - Math.Max(red, Math.Max(green, blue));

        if (key >= 1)
        {
            return (0, 0, 0, 1);
        }

        double denominator = 1 - key;
        return ((1 - red - key) / denominator,
            (1 - green - key) / denominator,
            (1 - blue - key) / denominator,
            key);
    }

    private static string GetColorName(Color color)
    {
        System.Drawing.Color drawingColor = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

        foreach (System.Drawing.KnownColor knownColor in Enum.GetValues<System.Drawing.KnownColor>())
        {
            System.Drawing.Color candidate = System.Drawing.Color.FromKnownColor(knownColor);

            if (candidate.A == drawingColor.A && candidate.R == drawingColor.R &&
                candidate.G == drawingColor.G && candidate.B == drawingColor.B)
            {
                return candidate.Name;
            }
        }

        return string.Empty;
    }

    [GeneratedRegex(@"\$(r|g|b)1(?:\{(\d+)\})?", RegexOptions.IgnoreCase)]
    private static partial Regex NormalizedComponentRegex();
}

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

using Avalonia.Media;
using System.Diagnostics;

namespace ShareX.AvaloniaUI.Theming
{
    /// <summary>
    /// Colors derived from the user's accent color that meet WCAG AA contrast whatever accent is picked:
    /// text on an accent fill reaches 4.5:1, accent colored text on a theme surface reaches 4.5:1 and accent
    /// colored indicators (borders, rings) on a theme surface reach 3:1.
    /// </summary>
    public sealed class AccentPalette
    {
        public const double TextContrast = 4.5;
        public const double NonTextContrast = 3.0;
        public const double GradientDarkenAmount = 0.10;

        public static readonly Color LightText = Color.FromRgb(0xFF, 0xFF, 0xFF);
        public static readonly Color DarkText = Color.FromRgb(0x11, 0x11, 0x11);

        private const int Steps = 100;

        /// <summary>The user's accent color, made opaque.</summary>
        public Color Accent { get; }

        /// <summary>Start of the accent fill. Equal to <see cref="Accent"/> unless it had to be nudged so text on it is readable.</summary>
        public Color FillStart { get; }

        /// <summary>End of the accent fill gradient, slightly darker than <see cref="FillStart"/>.</summary>
        public Color FillEnd { get; }

        /// <summary>Text and icon color for content drawn on the accent fill.</summary>
        public Color OnFill { get; }

        /// <summary>Accent color for text and icons drawn on a theme surface (window, panel, menu, control backgrounds).</summary>
        public Color Text { get; }

        /// <summary>Accent color for borders, rings and other indicators drawn on a theme surface.</summary>
        public Color Border { get; }

        /// <summary>Lighter fill variants for hover/pressed states. Each keeps <see cref="OnFill"/> readable.</summary>
        public Color FillLight1 { get; }
        public Color FillLight2 { get; }
        public Color FillLight3 { get; }

        /// <summary>Darker fill variants for hover/pressed states. Each keeps <see cref="OnFill"/> readable.</summary>
        public Color FillDark1 { get; }
        public Color FillDark2 { get; }
        public Color FillDark3 { get; }

        private AccentPalette(Color accent, IReadOnlyList<Color> surfaces)
        {
            Accent = Opaque(accent);

            (FillStart, OnFill) = GetFill(Accent);
            FillEnd = Darken(FillStart, GradientDarkenAmount);

            Color onFill = OnFill;
            bool readableOnFill(Color fill) => GetContrastRatio(onFill, fill) >= TextContrast;
            FillLight1 = Approach(FillStart, Colors.White, 0.10, readableOnFill);
            FillLight2 = Approach(FillStart, Colors.White, 0.20, readableOnFill);
            FillLight3 = Approach(FillStart, Colors.White, 0.30, readableOnFill);
            FillDark1 = Approach(FillStart, Colors.Black, 0.10, readableOnFill);
            FillDark2 = Approach(FillStart, Colors.Black, 0.20, readableOnFill);
            FillDark3 = Approach(FillStart, Colors.Black, 0.30, readableOnFill);

            Color[] surfaceColors = surfaces.Select(Opaque).ToArray();
            Color away = GetAverageLuminance(surfaceColors) < 0.5 ? Colors.White : Colors.Black;
            Text = Reach(Accent, away, color => MeetsAll(color, surfaceColors, TextContrast));
            Border = Reach(FillStart, away, color => MeetsAll(color, surfaceColors, NonTextContrast));

            Debug.Assert(GetFillContrast(OnFill, FillStart) >= TextContrast, "Text on the accent fill is below WCAG AA.");
            Debug.Assert(MeetsAll(Text, surfaceColors, TextContrast), "Accent text on a theme surface is below WCAG AA.");
            Debug.Assert(MeetsAll(Border, surfaceColors, NonTextContrast), "Accent indicator on a theme surface is below 3:1.");
        }

        /// <param name="accent">The user's accent color. Alpha is ignored.</param>
        /// <param name="surfaces">The theme backgrounds that accent colored text and indicators are drawn on.</param>
        public static AccentPalette Create(Color accent, IReadOnlyList<Color> surfaces)
        {
            ArgumentNullException.ThrowIfNull(surfaces);

            if (surfaces.Count == 0)
            {
                throw new ArgumentException("At least one surface color is required.", nameof(surfaces));
            }

            return new AccentPalette(accent, surfaces);
        }

        /// <summary>
        /// Black or white, whichever contrasts more with the color. Use it as a halo or under-stroke when an accent
        /// colored line, handle or label is drawn over arbitrary image or screen content.
        /// </summary>
        public static Color GetHaloColor(Color color)
        {
            return GetContrastRatio(color, Colors.Black) >= GetContrastRatio(color, Colors.White) ? Colors.Black : Colors.White;
        }

        /// <summary>White or near-black text, whichever is more readable on the color.</summary>
        public static Color GetTextColorOn(Color background)
        {
            return GetContrastRatio(LightText, background) >= GetContrastRatio(DarkText, background) ? LightText : DarkText;
        }

        public static double GetContrastRatio(Color first, Color second)
        {
            double firstLuminance = GetRelativeLuminance(first);
            double secondLuminance = GetRelativeLuminance(second);
            return (Math.Max(firstLuminance, secondLuminance) + 0.05) /
                (Math.Min(firstLuminance, secondLuminance) + 0.05);
        }

        public static double GetRelativeLuminance(Color color)
        {
            static double Linearize(byte channel)
            {
                double value = channel / 255d;
                return value <= 0.03928 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);
            }

            return (0.2126 * Linearize(color.R)) +
                (0.7152 * Linearize(color.G)) +
                (0.0722 * Linearize(color.B));
        }

        // Picks white or near-black text, whichever needs the smaller change to the accent to reach 4.5:1 against
        // both gradient stops. White text darkens the fill, dark text lightens it. Most accents need no change.
        private static (Color Fill, Color Text) GetFill(Color accent)
        {
            (Color fill, int steps) light = Nudge(accent, LightText, Colors.Black);
            (Color fill, int steps) dark = Nudge(accent, DarkText, Colors.White);

            if (light.steps != dark.steps)
            {
                return light.steps < dark.steps ? (light.fill, LightText) : (dark.fill, DarkText);
            }

            return GetFillContrast(LightText, light.fill) >= GetFillContrast(DarkText, dark.fill)
                ? (light.fill, LightText)
                : (dark.fill, DarkText);
        }

        private static (Color Fill, int Steps) Nudge(Color accent, Color text, Color target)
        {
            for (int step = 0; step <= Steps; step++)
            {
                Color fill = Blend(accent, target, step / (double)Steps);

                if (GetFillContrast(text, fill) >= TextContrast)
                {
                    return (fill, step);
                }
            }

            return (target, Steps + 1);
        }

        private static double GetFillContrast(Color text, Color fillStart)
        {
            return Math.Min(GetContrastRatio(text, fillStart), GetContrastRatio(text, Darken(fillStart, GradientDarkenAmount)));
        }

        // The closest blend from color toward target that satisfies the condition.
        private static Color Reach(Color color, Color target, Func<Color, bool> condition)
        {
            for (int step = 0; step <= Steps; step++)
            {
                Color candidate = Blend(color, target, step / (double)Steps);

                if (condition(candidate))
                {
                    return candidate;
                }
            }

            return target;
        }

        // The furthest blend from color toward target, up to maxAmount, that still satisfies the condition.
        private static Color Approach(Color color, Color target, double maxAmount, Func<Color, bool> condition)
        {
            Color result = color;
            int maxStep = (int)Math.Round(maxAmount * Steps);

            for (int step = 1; step <= maxStep; step++)
            {
                Color candidate = Blend(color, target, step / (double)Steps);

                if (!condition(candidate))
                {
                    break;
                }

                result = candidate;
            }

            return result;
        }

        private static bool MeetsAll(Color color, Color[] surfaces, double ratio)
        {
            foreach (Color surface in surfaces)
            {
                if (GetContrastRatio(color, surface) < ratio)
                {
                    return false;
                }
            }

            return true;
        }

        private static double GetAverageLuminance(Color[] colors)
        {
            return colors.Average(GetRelativeLuminance);
        }

        private static Color Opaque(Color color)
        {
            return Color.FromRgb(color.R, color.G, color.B);
        }

        private static Color Darken(Color color, double amount)
        {
            double factor = Math.Clamp(1 - amount, 0, 1);
            return Color.FromRgb(
                (byte)Math.Round(color.R * factor),
                (byte)Math.Round(color.G * factor),
                (byte)Math.Round(color.B * factor));
        }

        private static Color Blend(Color from, Color to, double amount)
        {
            return Color.FromRgb(
                (byte)Math.Round(from.R + ((to.R - from.R) * amount)),
                (byte)Math.Round(from.G + ((to.G - from.G) * amount)),
                (byte)Math.Round(from.B + ((to.B - from.B) * amount)));
        }
    }
}

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
using Newtonsoft.Json;
using ShareX.ImageEditor.Core.Annotations;

namespace ShareX.ImageEditor.Integration
{
    public class ImageEditorOptions
    {
        public static readonly Color PrimaryColor = Color.FromArgb(255, 242, 60, 60);
        public static readonly Color SecondaryColor = Color.FromArgb(255, 250, 250, 250);
        public static readonly Color DefaultShadowColor = Color.Parse(Annotation.DefaultShadowColorHex);
        public static readonly IReadOnlyList<string> DefaultFavoriteEffects = new[]
        {
            "resize_image",
            "resize_canvas",
            "crop_image",
            "auto_crop_image",
            "rotate_90_clockwise",
            "rotate_90_counter_clockwise",
            "rotate_180",
            "flip_horizontal",
            "flip_vertical"
        };

        private static string ColorToHex(Color c) => $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";
        private static Color HexToColor(string hex) => Color.Parse(hex);

        // Editor
        public EditorTool LastUsedAnnotationTool { get; set; } = EditorTool.Rectangle;

        public bool RememberWindowState { get; set; } = true;
        public bool IsWindowMaximized { get; set; } = true;
        public double WindowWidth { get; set; } = 1280;
        public double WindowHeight { get; set; } = 720;
        public bool ShowExitConfirmation { get; set; } = true;
        public bool ZoomToFitOnOpen { get; set; } = false;
        public bool QuickCrop { get; set; } = true;
        public bool AutoCloseEditorOnTask { get; set; } = false;
        public bool AutoCopyImageToClipboard { get; set; } = false;
        public bool ShowInsertImageDialog { get; set; } = true;
        public bool ShowNotifications { get; set; } = true;
        public List<ImageEditorToolbarItemOptions> ToolbarItems { get; set; } = new List<ImageEditorToolbarItemOptions>();

        // Shared
        public string BorderColorHex { get; set; } = ColorToHex(PrimaryColor);
        [JsonIgnore]
        public Color BorderColor { get => HexToColor(BorderColorHex); set => BorderColorHex = ColorToHex(value); }

        public string FillColorHex { get; set; } = ColorToHex(Colors.Transparent);
        [JsonIgnore]
        public Color FillColor { get => HexToColor(FillColorHex); set => FillColorHex = ColorToHex(value); }

        public int Thickness { get; set; } = 4;
        public int CornerRadius { get; set; } = 4;
        public bool Shadow { get; set; } = false;
        public string ShadowColorHex { get; set; } = Annotation.DefaultShadowColorHex;
        [JsonIgnore]
        public Color ShadowColor { get => HexToColor(ShadowColorHex); set => ShadowColorHex = ColorToHex(value); }
        public double ShadowBlurRadius { get; set; } = Annotation.DefaultShadowBlurRadius;
        public double ShadowOpacity { get; set; } = Annotation.DefaultShadowOpacity;
        public double ShadowOffsetX { get; set; } = Annotation.DefaultShadowOffsetX;
        public double ShadowOffsetY { get; set; } = Annotation.DefaultShadowOffsetY;
        public BorderStyle BorderStyle { get; set; } = BorderStyle.Solid;
        public ArrowStyle ArrowStyle { get; set; } = ArrowStyle.Classic;
        public CursorType CursorType { get; set; } = CursorType.Default;

        // Text
        public string TextBorderColorHex { get; set; } = ColorToHex(PrimaryColor);
        [JsonIgnore]
        public Color TextBorderColor { get => HexToColor(TextBorderColorHex); set => TextBorderColorHex = ColorToHex(value); }

        public string TextTextColorHex { get; set; } = ColorToHex(SecondaryColor);
        [JsonIgnore]
        public Color TextTextColor { get => HexToColor(TextTextColorHex); set => TextTextColorHex = ColorToHex(value); }

        public int TextThickness { get; set; } = 8;
        public float TextFontSize { get; set; } = 48;
        public string TextFontFamily { get; set; } = "Segoe UI";
        public TextHorizontalAlignment TextHorizontalAlignment { get; set; } = TextHorizontalAlignment.Center;
        public bool TextBold { get; set; } = true;
        public bool TextItalic { get; set; } = false;

        // Speech Balloon
        public string SpeechBalloonBorderColorHex { get; set; } = ColorToHex(Colors.Transparent);
        [JsonIgnore]
        public Color SpeechBalloonBorderColor { get => HexToColor(SpeechBalloonBorderColorHex); set => SpeechBalloonBorderColorHex = ColorToHex(value); }

        public string SpeechBalloonFillColorHex { get; set; } = ColorToHex(PrimaryColor);
        [JsonIgnore]
        public Color SpeechBalloonFillColor { get => HexToColor(SpeechBalloonFillColorHex); set => SpeechBalloonFillColorHex = ColorToHex(value); }

        public string SpeechBalloonTextColorHex { get; set; } = ColorToHex(SecondaryColor);
        [JsonIgnore]
        public Color SpeechBalloonTextColor { get => HexToColor(SpeechBalloonTextColorHex); set => SpeechBalloonTextColorHex = ColorToHex(value); }

        public int SpeechBalloonThickness { get; set; } = 4;
        public float SpeechBalloonFontSize { get; set; } = 48;
        public string SpeechBalloonFontFamily { get; set; } = "Segoe UI";
        public TextHorizontalAlignment SpeechBalloonTextHorizontalAlignment { get; set; } = TextHorizontalAlignment.Center;
        public bool SpeechBalloonTextBold { get; set; } = true;
        public bool SpeechBalloonTextItalic { get; set; } = false;
        public bool SpeechBalloonTail { get; set; } = true;

        // Step
        public string StepBorderColorHex { get; set; } = ColorToHex(Colors.Transparent);
        [JsonIgnore]
        public Color StepBorderColor { get => HexToColor(StepBorderColorHex); set => StepBorderColorHex = ColorToHex(value); }

        public string StepFillColorHex { get; set; } = ColorToHex(PrimaryColor);
        [JsonIgnore]
        public Color StepFillColor { get => HexToColor(StepFillColorHex); set => StepFillColorHex = ColorToHex(value); }

        public string StepTextColorHex { get; set; } = ColorToHex(SecondaryColor);
        [JsonIgnore]
        public Color StepTextColor { get => HexToColor(StepTextColorHex); set => StepTextColorHex = ColorToHex(value); }

        public int StepThickness { get; set; } = 4;
        public float StepFontSize { get; set; } = 30;
        public bool StepTextBold { get; set; } = true;
        public StepType StepType { get; set; } = StepType.Numeric;
        public bool StepTail { get; set; } = true;

        // Highlight
        public string HighlightFillColorHex { get; set; } = ColorToHex(Colors.Yellow);
        [JsonIgnore]
        public Color HighlightFillColor { get => HexToColor(HighlightFillColorHex); set => HighlightFillColorHex = ColorToHex(value); }

        // Effects
        public float BlurStrength { get; set; } = 30;
        public float PixelateStrength { get; set; } = 20;
        public float MagnifierStrength { get; set; } = 2;
        public bool MagnifierEllipse { get; set; }
        public float SpotlightStrength { get; set; } = 30;
        public float SpotlightBlur { get; set; } = 0;
        public bool SpotlightEllipse { get; set; }

        // Background
        public double BackgroundMargin { get; set; } = 80;
        public double BackgroundPadding { get; set; } = 40;
        public bool BackgroundSmartPadding { get; set; } = true;
        public double BackgroundRoundedCorner { get; set; } = 20;
        public double BackgroundShadowRadius { get; set; } = 30;
        public string BackgroundType { get; set; } = "Transparent";
        public string BackgroundGradientColor1Hex { get; set; } = ColorToHex(Color.FromArgb(255, 221, 36, 118));
        [JsonIgnore]
        public Color BackgroundGradientColor1 { get => HexToColor(BackgroundGradientColor1Hex); set => BackgroundGradientColor1Hex = ColorToHex(value); }
        public string BackgroundGradientColor2Hex { get; set; } = ColorToHex(Color.FromArgb(255, 255, 81, 47));
        [JsonIgnore]
        public Color BackgroundGradientColor2 { get => HexToColor(BackgroundGradientColor2Hex); set => BackgroundGradientColor2Hex = ColorToHex(value); }
        public string BackgroundColorHex { get; set; } = ColorToHex(Color.FromArgb(255, 34, 34, 34));
        [JsonIgnore]
        public Color BackgroundColor { get => HexToColor(BackgroundColorHex); set => BackgroundColorHex = ColorToHex(value); }
        public string BackgroundImagePath { get; set; } = "";

        // Recent image files
        public List<string> RecentImageFiles { get; set; } = new List<string>();
        public int MaxRecentImageFiles { get; set; } = 10;

        public void AddRecentImageFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            RecentImageFiles.Remove(filePath);
            RecentImageFiles.Insert(0, filePath);

            if (RecentImageFiles.Count > MaxRecentImageFiles)
            {
                RecentImageFiles.RemoveRange(MaxRecentImageFiles, RecentImageFiles.Count - MaxRecentImageFiles);
            }
        }

        // Image effects
        public List<string> RecentEffects { get; set; } = new List<string>();
        public List<string> FavoriteEffects { get; set; } = new List<string>(DefaultFavoriteEffects);
    }
}

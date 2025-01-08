#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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

using ShareX.HelpersLib;
using System.Collections.Generic;
using System.Drawing;

namespace ShareX.ScreenCaptureLib
{
    public class AnnotationOptions
    {
        public static readonly string DefaultFont = "Arial";
        public static readonly Color PrimaryColor = Color.FromArgb(242, 60, 60);
        public static readonly Color SecondaryColor = Color.White;
        public static readonly Color TransparentColor = Color.FromArgb(0, 0, 0, 0);

        // Region
        public int RegionCornerRadius { get; set; } = 0;

        // Drawing
        public Color BorderColor { get; set; } = PrimaryColor;
        public int BorderSize { get; set; } = 4;
        public BorderStyle BorderStyle { get; set; } = BorderStyle.Solid;
        public Color FillColor { get; set; } = TransparentColor;
        public int DrawingCornerRadius { get; set; } = 3;
        public bool Shadow { get; set; } = true;
        public Color ShadowColor { get; set; } = Color.FromArgb(125, 0, 0, 0);
        public Point ShadowOffset { get; set; } = new Point(0, 1);

        // Line, arrow drawing
        public int LineCenterPointCount { get; set; } = 1;

        // Arrow drawing
        public ArrowHeadDirection ArrowHeadDirection { get; set; } = ArrowHeadDirection.End;

        // Text (Outline) drawing
        public TextDrawingOptions TextOutlineOptions { get; set; } = new TextDrawingOptions()
        {
            Color = SecondaryColor,
            Size = 25,
            Bold = true
        };
        public Color TextOutlineBorderColor { get; set; } = PrimaryColor;
        public int TextOutlineBorderSize { get; set; } = 5;

        // Text (Background) drawing
        public TextDrawingOptions TextOptions { get; set; } = new TextDrawingOptions()
        {
            Color = SecondaryColor,
            Size = 18
        };
        public Color TextBorderColor { get; set; } = SecondaryColor;
        public int TextBorderSize { get; set; } = 0;
        public Color TextFillColor { get; set; } = PrimaryColor;

        // Image drawing
        public ImageInterpolationMode ImageInterpolationMode = ImageInterpolationMode.NearestNeighbor;
        public string LastImageFilePath { get; set; }

        // Step drawing
        public Color StepBorderColor { get; set; } = SecondaryColor;
        public int StepBorderSize { get; set; } = 0;
        public Color StepFillColor { get; set; } = PrimaryColor;
        public int StepFontSize { get; set; } = 18;
        public StepType StepType { get; set; } = StepType.Numbers;

        // Magnify drawing
        public int MagnifyStrength { get; set; } = 200;

        // Sticker drawing
        public List<StickerPackInfo> StickerPacks = new List<StickerPackInfo>()
        {
            new StickerPackInfo(@"Stickers\BlobEmoji", "Blob Emoji")
        };
        public int SelectedStickerPack = 0;
        public int StickerSize { get; set; } = 64;
        public string LastStickerPath { get; set; }

        // Blur effect
        public int BlurRadius { get; set; } = 35;

        // Pixelate effect
        public int PixelateSize { get; set; } = 15;

        // Highlight effect
        public Color HighlightColor { get; set; } = Color.Yellow;

        // Cut out tool
        public CutOutEffectType CutOutEffectType { get; set; } = CutOutEffectType.None;
        public int CutOutEffectSize { get; set; } = 10;
        public Color CutOutBackgroundColor { get; set; } = Color.Transparent;
    }
}
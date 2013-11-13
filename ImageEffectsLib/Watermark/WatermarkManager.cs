#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using HelpersLib;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace ImageEffectsLib
{
    public static class WatermarkManager
    {
        public static Image ApplyWatermark(Image img, WatermarkConfig config)
        {
            switch (config.WatermarkMode)
            {
                case WatermarkType.TEXT:
                    DrawText drawText = new DrawText
                    {
                        Position = (PositionType)config.WatermarkPositionMode,
                        Offset = config.WatermarkOffset,
                        AutoHide = config.WatermarkAutoHide,
                        Text = config.WatermarkText,
                        TextFont = config.WatermarkFont,
                        TextColor = config.WatermarkFontArgb,
                        DrawBackground = true,
                        BackgroundPadding = 5,
                        CornerRadius = config.WatermarkCornerRadius,
                        BorderColor = config.WatermarkBorderArgb,
                        BackgroundColor = config.WatermarkGradient1Argb,
                        UseGradient = true,
                        BackgroundColor2 = config.WatermarkGradient2Argb,
                        UseCustomGradient = false,
                        CustomGradientList = null,
                        GradientType = config.WatermarkGradientType
                    };

                    return drawText.Apply(img);
                case WatermarkType.IMAGE:
                    DrawImage drawImage = new DrawImage
                    {
                        Position = (PositionType)config.WatermarkPositionMode,
                        Offset = config.WatermarkOffset,
                        AutoHide = config.WatermarkAutoHide,
                        ImageLocation = config.WatermarkImageLocation
                    };

                    return drawImage.Apply(img);
            }

            return img;
        }
    }
}
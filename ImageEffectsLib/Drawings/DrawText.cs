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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace ImageEffectsLib
{
    [Description("Text")]
    internal class DrawText : ImageEffect
    {
        [DefaultValue(PositionType.Bottom_Right), TypeConverter(typeof(EnumDescriptionConverter))]
        public PositionType Position { get; set; }

        [DefaultValue(5)]
        public int Offset { get; set; }

        [DefaultValue(false), Description("If watermark size bigger than source image then don't draw it.")]
        public bool AutoHide { get; set; }

        [DefaultValue("%h:%mi")]
        public string Text { get; set; }

        [DefaultValue(typeof(Font), "Arial, 8pt")]
        public Font TextFont { get; set; }

        [DefaultValue(typeof(Color), "White"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color TextColor { get; set; }

        [DefaultValue(true)]
        public bool DrawBackground { get; set; }

        [DefaultValue(5)]
        public int BackgroundPadding { get; set; }

        [DefaultValue(4)]
        public int CornerRadius { get; set; }

        [DefaultValue(typeof(Color), "Black"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color BorderColor { get; set; }

        [DefaultValue(typeof(Color), "85, 85, 85"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color BackgroundColor { get; set; }

        [DefaultValue(true)]
        public bool UseGradient { get; set; }

        [DefaultValue(typeof(Color), "Black"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color BackgroundColor2 { get; set; }

        [DefaultValue(LinearGradientMode.Vertical)]
        public LinearGradientMode GradientType { get; set; }

        public DrawText()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Image Apply(Image img)
        {
            if (string.IsNullOrEmpty(Text) || TextFont == null || TextFont.Size < 1)
            {
                return img;
            }

            NameParser parser = new NameParser(NameParserType.Text) { Picture = img };
            string parsedText = parser.Parse(Text);
            Size textSize = TextRenderer.MeasureText(parsedText, TextFont);
            Size labelSize = new Size(textSize.Width + BackgroundPadding * 2, textSize.Height + BackgroundPadding * 2);

            if (AutoHide && ((labelSize.Width + Offset > img.Width) || (labelSize.Height + Offset > img.Height)))
            {
                return img;
            }

            Rectangle labelRectangle = new Rectangle(Point.Empty, labelSize);

            using (Bitmap bmp = new Bitmap(labelRectangle.Width, labelRectangle.Height))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                if (DrawBackground)
                {
                    using (GraphicsPath gPath = new GraphicsPath())
                    {
                        gPath.AddRoundedRectangle(labelRectangle, CornerRadius);

                        Brush backgroundBrush = null;

                        try
                        {
                            if (UseGradient)
                            {
                                backgroundBrush = new LinearGradientBrush(labelRectangle, BackgroundColor, BackgroundColor2, GradientType);
                            }
                            else
                            {
                                backgroundBrush = new SolidBrush(BackgroundColor);
                            }

                            g.FillPath(backgroundBrush, gPath);
                        }
                        finally
                        {
                            if (backgroundBrush != null) backgroundBrush.Dispose();
                        }

                        using (Pen borderPen = new Pen(BorderColor))
                        {
                            g.DrawPath(borderPen, gPath);
                        }
                    }
                }

                using (Brush textBrush = new SolidBrush(TextColor))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(parsedText, TextFont, textBrush, bmp.Width / 2f, bmp.Height / 2f, sf);
                }

                using (Graphics gImg = Graphics.FromImage(img))
                {
                    gImg.SetHighQuality();

                    Point labelPosition = Helpers.GetPosition(Position, Offset, img.Size, labelSize);
                    gImg.DrawImage(bmp, labelPosition.X, labelPosition.Y, bmp.Width, bmp.Height);
                }
            }

            return img;
        }
    }
}
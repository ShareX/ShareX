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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShareX.ScreenCaptureLib
{
    public class TextOutlineDrawingShape : TextDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingTextOutline;

        public override bool SupportGradient { get; } = true;

        public override void OnConfigLoad()
        {
            TextOptions = AnnotationOptions.TextOutlineOptions.Copy();
            BorderColor = AnnotationOptions.TextOutlineBorderColor;
            BorderSize = AnnotationOptions.TextOutlineBorderSize;
            Shadow = AnnotationOptions.Shadow;
            ShadowColor = AnnotationOptions.ShadowColor;
            ShadowOffset = AnnotationOptions.ShadowOffset;
        }

        public override void OnConfigSave()
        {
            AnnotationOptions.TextOutlineOptions = TextOptions;
            AnnotationOptions.TextOutlineBorderColor = BorderColor;
            AnnotationOptions.TextOutlineBorderSize = BorderSize;
            AnnotationOptions.Shadow = Shadow;
            AnnotationOptions.ShadowColor = ShadowColor;
            AnnotationOptions.ShadowOffset = ShadowOffset;
        }

        public override void OnDraw(Graphics g)
        {
            DrawTextWithOutline(g, Text, TextOptions, TextOptions.Color, BorderColor, BorderSize, Rectangle);
        }

        protected void DrawTextWithOutline(Graphics g, string text, TextDrawingOptions options, Color textColor, Color borderColor, int borderSize, RectangleF rect)
        {
            if (!string.IsNullOrEmpty(text) && rect.Width > 10 && rect.Height > 10)
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.FillMode = FillMode.Winding;

                    using (Font font = new Font(options.Font, options.Size, options.Style))
                    using (StringFormat sf = new StringFormat { Alignment = options.AlignmentHorizontal, LineAlignment = options.AlignmentVertical })
                    {
                        float emSize = g.DpiY * font.SizeInPoints / 72;
                        gp.AddString(text, font.FontFamily, (int)font.Style, emSize, rect, sf);
                    }

                    RectangleF pathRect = gp.GetBounds();

                    if (pathRect.IsEmpty) return;

                    g.SmoothingMode = SmoothingMode.HighQuality;

                    if (Shadow)
                    {
                        using (Matrix matrix = new Matrix())
                        {
                            matrix.Translate(ShadowOffset.X, ShadowOffset.Y);
                            gp.Transform(matrix);

                            if (IsBorderVisible)
                            {
                                using (Pen shadowPen = new Pen(ShadowColor, borderSize) { LineJoin = LineJoin.Round })
                                {
                                    g.DrawPath(shadowPen, gp);
                                }
                            }
                            else
                            {
                                using (Brush shadowBrush = new SolidBrush(ShadowColor))
                                {
                                    g.FillPath(shadowBrush, gp);
                                }
                            }

                            matrix.Reset();
                            matrix.Translate(-ShadowOffset.X, -ShadowOffset.Y);
                            gp.Transform(matrix);
                        }
                    }

                    if (IsBorderVisible)
                    {
                        using (Pen borderPen = new Pen(borderColor, borderSize) { LineJoin = LineJoin.Round })
                        {
                            g.DrawPath(borderPen, gp);
                        }
                    }

                    Brush textBrush = null;

                    try
                    {
                        if (TextOptions.Gradient)
                        {
                            textBrush = new LinearGradientBrush(pathRect.Round().Offset(1), textColor, TextOptions.Color2, TextOptions.GradientMode);
                        }
                        else
                        {
                            textBrush = new SolidBrush(textColor);
                        }

                        g.FillPath(textBrush, gp);
                    }
                    finally
                    {
                        if (textBrush != null)
                        {
                            textBrush.Dispose();
                        }
                    }

                    g.SmoothingMode = SmoothingMode.None;
                }
            }
        }
    }
}
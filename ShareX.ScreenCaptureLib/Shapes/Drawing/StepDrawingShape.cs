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
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace ShareX.ScreenCaptureLib
{
    public class StepDrawingShape : EllipseDrawingShape
    {
        private const int DefaultSize = 30;

        public override ShapeType ShapeType { get; } = ShapeType.DrawingStep;

        public int FontSize { get; set; }
        public int Number { get; set; }
        public StepType StepType { get; set; } = StepType.Numbers;
        public bool IsTailActive { get; set; }

        private PointF tailPosition;

        public PointF TailPosition
        {
            get
            {
                return tailPosition;
            }
            private set
            {
                tailPosition = value;
                TailNode.Position = tailPosition;
            }
        }

        public bool TailVisible => !Rectangle.Contains(TailPosition);

        internal ResizeNode TailNode => Manager.ResizeNodes[(int)NodePosition.Extra];

        protected const float TailWidthMultiplier = 1f;

        public StepDrawingShape()
        {
            Rectangle = new Rectangle(0, 0, DefaultSize, DefaultSize);
        }

        public override void OnCreating()
        {
            Manager.IsMoving = true;
            PointF pos = Manager.Form.ScaledClientMousePosition;
            Rectangle = new RectangleF(new PointF(pos.X - (Rectangle.Width / 2), pos.Y - (Rectangle.Height / 2)), Rectangle.Size);
            int tailOffset = 5;
            TailPosition = Rectangle.Location.Add(Rectangle.Width + tailOffset, Rectangle.Height + tailOffset);
            OnCreated();
        }

        protected override void UseLightResizeNodes()
        {
            Manager.ResizeNodes[(int)NodePosition.Extra].Shape = NodeShape.Circle;
        }

        public override void OnNodeVisible()
        {
            TailNode.Position = TailPosition;
            TailNode.Visible = true;
        }

        public override void OnNodeUpdate()
        {
            if (TailNode.IsDragging)
            {
                IsTailActive = true;
                TailPosition = Manager.Form.ScaledClientMousePosition;
            }
        }

        public override void OnNodePositionUpdate()
        {
        }

        public override void OnConfigLoad()
        {
            BorderColor = AnnotationOptions.StepBorderColor;
            BorderSize = AnnotationOptions.StepBorderSize;
            FillColor = AnnotationOptions.StepFillColor;
            Shadow = AnnotationOptions.Shadow;
            ShadowColor = AnnotationOptions.ShadowColor;
            ShadowOffset = AnnotationOptions.ShadowOffset;
            FontSize = AnnotationOptions.StepFontSize;
            StepType = AnnotationOptions.StepType;
        }

        public override void OnConfigSave()
        {
            AnnotationOptions.StepBorderColor = BorderColor;
            AnnotationOptions.StepBorderSize = BorderSize;
            AnnotationOptions.StepFillColor = FillColor;
            AnnotationOptions.Shadow = Shadow;
            AnnotationOptions.ShadowColor = ShadowColor;
            AnnotationOptions.ShadowOffset = ShadowOffset;
            AnnotationOptions.StepFontSize = FontSize;
            AnnotationOptions.StepType = StepType;
        }

        public override void OnDraw(Graphics g)
        {
            DrawNumber(g);
        }

        private string GetText()
        {
            switch (StepType)
            {
                case StepType.LettersUppercase:
                    return Helpers.NumberToLetters(Number);
                case StepType.LettersLowercase:
                    return Helpers.NumberToLetters(Number).ToLowerInvariant();
                case StepType.RomanNumeralsUppercase:
                    return Helpers.NumberToRomanNumeral(Number);
                case StepType.RomanNumeralsLowercase:
                    return Helpers.NumberToRomanNumeral(Number).ToLowerInvariant();
                default:
                    return Number.ToString();
            }
        }

        protected void DrawNumber(Graphics g)
        {
            string text = GetText();

            using (Font font = new Font(FontFamily.GenericSansSerif, FontSize, FontStyle.Bold))
            {
                Size textSize = g.MeasureString(text, font).ToSize();
                int maxSize = Math.Max(textSize.Width, textSize.Height);
                int padding = 3;

                PointF center = Rectangle.Center();
                Rectangle = new RectangleF(center.X - (maxSize / 2f) - padding, center.Y - (maxSize / 2f) - padding, maxSize + (padding * 2), maxSize + (padding * 2));

                if (Shadow)
                {
                    if (IsBorderVisible)
                    {
                        DrawEllipse(g, ShadowColor, BorderSize, BorderStyle, Color.Transparent, Rectangle.LocationOffset(ShadowOffset));
                    }
                    else if (FillColor.A == 255)
                    {
                        DrawEllipse(g, Color.Transparent, 0, BorderStyle, ShadowColor, Rectangle.LocationOffset(ShadowOffset));
                    }
                }

                if (IsTailActive && TailVisible)
                {
                    if (Shadow)
                    {
                        DrawTail(g, ShadowColor, Rectangle.Offset(BorderSize / 2).LocationOffset(ShadowOffset), TailPosition.Add(ShadowOffset));
                    }

                    Color tailColor;

                    if (IsBorderVisible)
                    {
                        tailColor = BorderColor;
                    }
                    else
                    {
                        tailColor = FillColor;
                    }

                    DrawTail(g, tailColor, Rectangle.Offset(BorderSize / 2), TailPosition);
                }

                DrawEllipse(g, BorderColor, BorderSize, BorderStyle, FillColor, Rectangle);

                if (Shadow)
                {
                    DrawNumber(g, text, font, ShadowColor, Rectangle.LocationOffset(ShadowOffset));
                }

                DrawNumber(g, text, font, BorderColor, Rectangle);
            }
        }

        protected void DrawNumber(Graphics g, string text, Font font, Color textColor, RectangleF rect)
        {
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (Brush textBrush = new SolidBrush(textColor))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                rect = rect.LocationOffset(0, 1);
                g.DrawString(text, font, textBrush, rect, sf);
                g.TextRenderingHint = TextRenderingHint.SystemDefault;
            }
        }

        private void DrawTail(Graphics g, Color tailColor, RectangleF rectangle, PointF tailPosition)
        {
            using (GraphicsPath gpTail = CreateTailPath(rectangle, tailPosition))
            {
                if (gpTail != null)
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;

                    using (Brush brush = new SolidBrush(tailColor))
                    {
                        g.FillPath(brush, gpTail);
                    }

                    g.SmoothingMode = SmoothingMode.None;
                }
            }
        }

        public override void Move(float x, float y)
        {
            base.Move(x, y);

            TailPosition = TailPosition.Add(x, y);
        }

        public override void Resize(int x, int y, bool fromBottomRight)
        {
            Move(x, y);
        }

        protected GraphicsPath CreateTailPath(RectangleF rect, PointF tailPosition)
        {
            GraphicsPath gpTail = new GraphicsPath();
            PointF center = rect.Center();
            float rectAverageSize = (rect.Width + rect.Height) / 2;
            float tailWidth = TailWidthMultiplier * rectAverageSize;
            tailWidth = Math.Min(Math.Min(tailWidth, rect.Width), rect.Height);
            float tailOrigin = tailWidth / 2;
            float tailLength = MathHelpers.Distance(center, tailPosition);
            gpTail.AddLine(0, -tailOrigin, 0, tailOrigin);
            gpTail.AddLine(0, tailOrigin, tailLength, 0);
            gpTail.CloseFigure();
            using (Matrix matrix = new Matrix())
            {
                matrix.Translate(center.X, center.Y);
                float tailDegree = MathHelpers.LookAtDegree(center, tailPosition);
                matrix.Rotate(tailDegree);
                gpTail.Transform(matrix);
            }
            return gpTail;
        }
    }
}
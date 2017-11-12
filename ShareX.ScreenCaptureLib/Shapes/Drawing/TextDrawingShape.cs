#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.Drawing.Text;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class TextDrawingShape : RectangleDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingTextBackground;

        public string Text { get; set; }
        public TextDrawingOptions TextOptions { get; set; }

        public override void OnConfigLoad()
        {
            TextOptions = AnnotationOptions.TextOptions.Copy();
            BorderColor = AnnotationOptions.TextBorderColor;
            BorderSize = AnnotationOptions.TextBorderSize;
            FillColor = AnnotationOptions.TextFillColor;
            CornerRadius = AnnotationOptions.DrawingCornerRadius;
            Shadow = AnnotationOptions.Shadow;
        }

        public override void OnConfigSave()
        {
            AnnotationOptions.TextOptions = TextOptions;
            AnnotationOptions.TextBorderColor = BorderColor;
            AnnotationOptions.TextBorderSize = BorderSize;
            AnnotationOptions.TextFillColor = FillColor;
            AnnotationOptions.DrawingCornerRadius = CornerRadius;
            AnnotationOptions.Shadow = Shadow;
        }

        public override void OnDraw(Graphics g)
        {
            DrawRectangle(g);
            DrawText(g);
        }

        protected void DrawText(Graphics g)
        {
            if (Shadow)
            {
                DrawText(g, Text, ShadowColor, TextOptions, Rectangle.LocationOffset(ShadowOffset));
            }

            DrawText(g, Text, TextOptions, Rectangle);
        }

        protected void DrawText(Graphics g, string text, TextDrawingOptions options, Rectangle rect)
        {
            DrawText(g, text, options.Color, options, rect);
        }

        protected void DrawText(Graphics g, string text, Color textColor, TextDrawingOptions options, Rectangle rect)
        {
            if (!string.IsNullOrEmpty(text) && rect.Width > 10 && rect.Height > 10)
            {
                using (Font font = new Font(options.Font, options.Size, options.Style))
                using (Brush textBrush = new SolidBrush(textColor))
                using (StringFormat sf = new StringFormat { Alignment = options.AlignmentHorizontal, LineAlignment = options.AlignmentVertical })
                {
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    g.DrawString(text, font, textBrush, rect, sf);
                    g.TextRenderingHint = TextRenderingHint.SystemDefault;
                }
            }
        }

        public override void OnCreating()
        {
            StartPosition = EndPosition = InputManager.ClientMousePosition;

            if (ShowTextInputBox())
            {
                OnCreated();
            }
            else
            {
                Remove();
            }
        }

        public override void OnCreated()
        {
            AutoSize(true);
            ShowNodes();
        }

        public override void OnDoubleClicked()
        {
            ShowTextInputBox();
        }

        private bool ShowTextInputBox()
        {
            bool result;

            Manager.Form.Pause();

            using (TextDrawingInputBox inputBox = new TextDrawingInputBox(Text, TextOptions))
            {
                result = inputBox.ShowDialog(Manager.Form) == DialogResult.OK;
                Text = inputBox.InputText;
                OnConfigSave();
            }

            Manager.Form.Resume();

            return result;
        }

        public void AutoSize(bool center)
        {
            Size size;

            if (!string.IsNullOrEmpty(Text))
            {
                using (Font font = new Font(TextOptions.Font, TextOptions.Size, TextOptions.Style))
                {
                    size = Helpers.MeasureText(Text, font).Offset(15, 20);
                }
            }
            else
            {
                size = new Size(100, 60);
            }

            Point location;

            if (center)
            {
                location = new Point(Rectangle.X - size.Width / 2, Rectangle.Y - size.Height / 2);
            }
            else
            {
                location = Rectangle.Location;
            }

            StartPosition = location;
            EndPosition = new Point(location.X + size.Width - 1, location.Y + size.Height - 1);
        }
    }
}
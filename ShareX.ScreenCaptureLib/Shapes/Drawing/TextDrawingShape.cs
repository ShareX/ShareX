#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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

namespace ShareX.ScreenCaptureLib
{
    public class TextDrawingShape : RectangleDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingText;

        public string Text { get; set; }
        public TextDrawingOptions TextOptions { get; set; }

        public override void OnConfigLoad()
        {
            TextOptions = AnnotationOptions.TextOptions.Copy();
            BorderColor = AnnotationOptions.TextBorderColor;
            BorderSize = AnnotationOptions.TextBorderSize;
            FillColor = AnnotationOptions.TextFillColor;
        }

        public override void OnConfigSave()
        {
            AnnotationOptions.TextOptions = TextOptions;
            AnnotationOptions.TextBorderColor = BorderColor;
            AnnotationOptions.TextBorderSize = BorderSize;
            AnnotationOptions.TextFillColor = FillColor;
        }

        public override void OnDraw(Graphics g)
        {
            base.OnDraw(g);

            DrawText(g);
        }

        protected void DrawText(Graphics g)
        {
            if (!string.IsNullOrEmpty(Text) && Rectangle.Width > 10 && Rectangle.Height > 10)
            {
                using (Font font = new Font(TextOptions.Font, TextOptions.Size, TextOptions.Style))
                using (Brush textBrush = new SolidBrush(TextOptions.Color))
                using (StringFormat sf = new StringFormat { Alignment = TextOptions.AlignmentHorizontal, LineAlignment = TextOptions.AlignmentVertical })
                {
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    g.DrawString(Text, font, textBrush, Rectangle, sf);
                    g.TextRenderingHint = TextRenderingHint.SystemDefault;
                }
            }
        }

        public override void OnCreating()
        {
            StartPosition = EndPosition = InputManager.MousePosition0Based;

            ShowTextInputBox();

            if (string.IsNullOrEmpty(Text))
            {
                Remove();
            }
            else
            {
                OnCreated();
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

        private void ShowTextInputBox()
        {
            Manager.PauseForm();

            using (TextDrawingInputBox inputBox = new TextDrawingInputBox(Text, TextOptions))
            {
                inputBox.ShowDialog();
                Text = inputBox.InputText;
                OnConfigSave();
            }

            Manager.ResumeForm();
        }

        public void AutoSize(bool center)
        {
            Size size;

            using (Font font = new Font(TextOptions.Font, TextOptions.Size, TextOptions.Style))
            {
                size = Helpers.MeasureText(Text, font).Offset(10, 15);
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

            Rectangle = new Rectangle(location, size);
        }
    }
}
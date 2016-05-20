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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class TextDrawingShape : BaseDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingText;

        public string Text { get; set; }
        public TextDrawingOptions Options { get; set; } = new TextDrawingOptions();

        public override void UpdateShapeConfig()
        {
            Options = AnnotationOptions.TextOptions.Copy();
        }

        public override void Draw(Graphics g)
        {
            base.Draw(g);

            DrawFinal(g, null);
        }

        public override void DrawFinal(Graphics g, Bitmap bmp)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                DrawText(g);
            }
        }

        private void DrawText(Graphics g)
        {
            using (Font font = new Font(Options.Font, Options.Size, Options.Style))
            using (StringFormat sf = new StringFormat { Alignment = Options.AlignmentHorizontal, LineAlignment = Options.AlignmentVertical })
            {
                using (Brush textBrush = new SolidBrush(Options.Color))
                {
                    g.DrawString(Text, font, textBrush, Rectangle, sf);
                }
            }
        }

        private void UpdateText()
        {
            Manager.PauseForm();

            using (TextDrawingInputBox inputBox = new TextDrawingInputBox(Text, Options))
            {
                inputBox.ShowDialog();
                Text = inputBox.InputText;
                AnnotationOptions.TextOptions = Options;
            }

            Manager.ResumeForm();
        }

        public override void OnShapeCreated()
        {
            UpdateText();
        }

        public override void OnShapeDoubleClicked()
        {
            UpdateText();
        }
    }
}
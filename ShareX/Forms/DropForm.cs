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
using ShareX.Properties;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX
{
    public class DropForm : LayeredForm
    {
        private static DropForm instance;

        public static DropForm GetInstance(int size, int offset, ContentAlignment alignment, int opacity, int hoverOpacity, TaskSettings taskSettings = null)
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new DropForm(size, offset, alignment, opacity, hoverOpacity);
            }

            instance.taskSettings = taskSettings;

            return instance;
        }

        public int DropSize { get; set; }
        public int DropOffset { get; set; }
        public ContentAlignment DropAlignment { get; set; }
        public int DropOpacity { get; set; }
        public int DropHoverOpacity { get; set; }

        private Bitmap backgroundImage;
        private bool isHovered;
        private TaskSettings taskSettings;

        private DropForm(int size, int offset, ContentAlignment alignment, int opacity, int hoverOpacity)
        {
            InitializeComponent();

            DropSize = size.Clamp(10, 300);
            DropOffset = offset;
            DropAlignment = alignment;
            DropOpacity = opacity.Clamp(1, 255);
            DropHoverOpacity = hoverOpacity.Clamp(1, 255);

            backgroundImage = DrawDropImage(DropSize);

            Location = Helpers.GetPosition(DropAlignment, DropOffset, Screen.PrimaryScreen.WorkingArea, backgroundImage.Size);

            SelectBitmap(backgroundImage, DropOpacity);
        }

        private Bitmap DrawDropImage(int size)
        {
            Bitmap bmp = new Bitmap(size, size);
            Rectangle rect = new Rectangle(0, 0, size, size);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.CornflowerBlue, rect);

                g.DrawRectangleProper(Pens.Black, rect);

                using (Pen pen = new Pen(Color.WhiteSmoke, 5) { Alignment = PenAlignment.Inset })
                {
                    g.DrawRectangleProper(pen, rect.Offset(-1));
                }

                string text = Resources.DropForm_DrawDropImage_Drop_here;

                using (Font font = new Font("Arial", 20, FontStyle.Bold))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(text, font, Brushes.Black, rect.LocationOffset(1), sf);
                    g.DrawString(text, font, Brushes.White, rect, sf);
                }
            }

            return bmp;
        }

        private void DropForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(Handle, (uint)WindowsMessages.NCLBUTTONDOWN, (IntPtr)WindowHitTestRegions.HTCAPTION, IntPtr.Zero);
            }
        }

        private void DropForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Close();
            }
        }

        private void DropForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) ||
                e.Data.GetDataPresent(DataFormats.Bitmap, false) ||
                e.Data.GetDataPresent(DataFormats.Text, false))
            {
                e.Effect = DragDropEffects.Copy;

                if (!isHovered)
                {
                    SelectBitmap(backgroundImage, DropHoverOpacity);
                    isHovered = true;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void DropForm_DragDrop(object sender, DragEventArgs e)
        {
            UploadManager.DragDropUpload(e.Data, taskSettings);

            if (isHovered)
            {
                SelectBitmap(backgroundImage, DropOpacity);
                isHovered = false;
            }
        }

        private void DropForm_DragLeave(object sender, EventArgs e)
        {
            if (isHovered)
            {
                SelectBitmap(backgroundImage, DropOpacity);
                isHovered = false;
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (backgroundImage != null) backgroundImage.Dispose();

            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            AllowDrop = true;
            AutoScaleMode = AutoScaleMode.Font;
            Cursor = Cursors.SizeAll;
            Text = "DropForm";
            TopMost = true;

            MouseDown += DropForm_MouseDown;
            MouseUp += DropForm_MouseUp;
            DragEnter += DropForm_DragEnter;
            DragDrop += DropForm_DragDrop;
            DragLeave += DropForm_DragLeave;
        }

        #endregion Windows Form Designer generated code
    }
}
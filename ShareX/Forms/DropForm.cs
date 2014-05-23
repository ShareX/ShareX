#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
using System.Windows.Forms;

namespace ShareX
{
    public partial class DropForm : LayeredForm
    {
        private static DropForm instance;

        public static DropForm GetInstance(int size, int offset, ContentAlignment alignment, int opacity, int hoverOpacity)
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new DropForm(size, offset, alignment, opacity, hoverOpacity);
            }

            return instance;
        }

        public int DropSize { get; set; }
        public int DropOffset { get; set; }
        public ContentAlignment DropAlignment { get; set; }
        public int DropOpacity { get; set; }
        public int DropHoverOpacity { get; set; }

        private Bitmap logo = null;
        private bool isHovered = false;

        private DropForm(int size, int offset, ContentAlignment alignment, int opacity, int hoverOpacity)
        {
            InitializeComponent();
            DropSize = size.Between(10, 300);
            DropOffset = offset;
            DropAlignment = alignment;
            DropOpacity = opacity.Between(1, 255);
            DropHoverOpacity = hoverOpacity.Between(1, 255);

            logo = (Bitmap)ImageHelpers.ResizeImage(ShareXResources.Logo, DropSize, DropSize);

            Point position = Helpers.GetPosition(DropAlignment, new Point(DropOffset, DropOffset), Screen.PrimaryScreen.WorkingArea.Size, logo.Size);
            Location = position;

            SelectBitmap(logo, DropOpacity);
        }

        private void DropForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(Handle, (uint)WindowsMessages.NCLBUTTONDOWN, (IntPtr)NativeMethods.HT_CAPTION, IntPtr.Zero);
            }
        }

        private void DropForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.Close();
            }
        }

        private void DropForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;

                if (!isHovered)
                {
                    SelectBitmap(logo, DropHoverOpacity);
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
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, true);
            UploadManager.UploadFile(files);

            if (isHovered)
            {
                SelectBitmap(logo, DropOpacity);
                isHovered = false;
            }
        }

        private void DropForm_DragLeave(object sender, EventArgs e)
        {
            if (isHovered)
            {
                SelectBitmap(logo, DropOpacity);
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
            if (logo != null) logo.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "DropForm";
            this.AllowDrop = true;

            MouseDown += DropForm_MouseDown;
            MouseUp += DropForm_MouseUp;
            DragEnter += DropForm_DragEnter;
            DragDrop += DropForm_DragDrop;
            DragLeave += DropForm_DragLeave;
        }

        #endregion Windows Form Designer generated code
    }
}
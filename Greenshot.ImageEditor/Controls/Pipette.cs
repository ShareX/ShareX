/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using Greenshot.Forms;
using GreenshotPlugin.UnmanagedHelpers;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Greenshot.Controls
{
    /// <summary>
    /// This code was supplied by Hi-Coder as a patch for Greenshot
    /// Needed some modifications to be stable.
    /// </summary>
    internal class Pipette : Label, IMessageFilter, IDisposable
    {
        private MovableShowColorForm movableShowColorForm;
        private bool dragging;
        private Cursor _cursor;
        private Bitmap _image;
        private const int VK_ESC = 27;

        public event EventHandler<PipetteUsedArgs> PipetteUsed;

        public Pipette()
        {
            BorderStyle = BorderStyle.FixedSingle;
            dragging = false;
            _image = (Bitmap)new ComponentResourceManager(typeof(ColorDialog)).GetObject("pipette.Image");
            Image = _image;
            _cursor = CreateCursor((Bitmap)_image, 1, 14);
            movableShowColorForm = new MovableShowColorForm();
            Application.AddMessageFilter(this);
        }

        /// <summary>
        /// Create a cursor from the supplied bitmap & hotspot coordinates
        /// </summary>
        /// <param name="bitmap">Bitmap to create an icon from</param>
        /// <param name="hotspotX">Hotspot X coordinate</param>
        /// <param name="hotspotY">Hotspot Y coordinate</param>
        /// <returns>Cursor</returns>
        private static Cursor CreateCursor(Bitmap bitmap, int hotspotX, int hotspotY)
        {
            using (SafeIconHandle iconHandle = new SafeIconHandle(bitmap.GetHicon()))
            {
                IntPtr icon;
                IconInfo iconInfo = new IconInfo();
                User32.GetIconInfo(iconHandle, out iconInfo);
                iconInfo.xHotspot = hotspotX;
                iconInfo.yHotspot = hotspotY;
                iconInfo.fIcon = false;
                icon = User32.CreateIconIndirect(ref iconInfo);
                return new Cursor(icon);
            }
        }

        /// <summary>
        /// The bulk of the clean-up code is implemented in Dispose(bool)
        /// </summary>
        public new void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// This Dispose is called from the Dispose and the Destructor.
        /// </summary>
        /// <param name="disposing">When disposing==true all non-managed resources should be freed too!</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_cursor != null)
                {
                    _cursor.Dispose();
                }
                if (movableShowColorForm != null)
                {
                    movableShowColorForm.Dispose();
                }
            }
            movableShowColorForm = null;
            _cursor = null;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handle the mouse down on the Pipette "label", we take the capture and move the zoomer to the current location
        /// </summary>
        /// <param name="e">MouseEventArgs</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                User32.SetCapture(Handle);
                movableShowColorForm.MoveTo(PointToScreen(new Point(e.X, e.Y)));
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Handle the mouse up on the Pipette "label", we release the capture and fire the PipetteUsed event
        /// </summary>
        /// <param name="e">MouseEventArgs</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //Release Capture should consume MouseUp when canceled with the escape key
                User32.ReleaseCapture();
                PipetteUsed(this, new PipetteUsedArgs(movableShowColorForm.color));
            }
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Handle the mouse Move event, we move the ColorUnderCursor to the current location.
        /// </summary>
        /// <param name="e">MouseEventArgs</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (dragging)
            {
                //display the form on the right side of the cursor by default;
                Point zp = PointToScreen(new Point(e.X, e.Y));
                movableShowColorForm.MoveTo(zp);
            }
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Handle the MouseCaptureChanged event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            if (Capture)
            {
                dragging = true;
                Image = null;
                Cursor c = _cursor;
                Cursor = c;
                movableShowColorForm.Visible = true;
            }
            else
            {
                dragging = false;
                Image = _image;
                Cursor = Cursors.Arrow;
                movableShowColorForm.Visible = false;
            }
            Update();
            base.OnMouseCaptureChanged(e);
        }

        #region IMessageFilter Members

        public bool PreFilterMessage(ref Message m)
        {
            if (dragging)
            {
                if (m.Msg == (int)WindowsMessages.WM_CHAR)
                {
                    if ((int)m.WParam == VK_ESC)
                    {
                        User32.ReleaseCapture();
                    }
                }
            }
            return false;
        }

        #endregion IMessageFilter Members
    }

    public class PipetteUsedArgs : EventArgs
    {
        public Color color;

        public PipetteUsedArgs(Color c)
        {
            color = c;
        }
    }
}
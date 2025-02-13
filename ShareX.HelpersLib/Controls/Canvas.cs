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

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class Canvas : UserControl
    {
        public delegate void DrawEventHandler(Graphics g);

        public event DrawEventHandler Draw;

        public int Interval { get; set; }

        private Timer timer;
        private bool needPaint;

        public Canvas()
        {
            Interval = 100;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public void Start()
        {
            if (timer == null || !timer.Enabled)
            {
                Stop();

                timer = new Timer();
                timer.Interval = Interval;
                timer.Tick += timer_Tick;
                timer.Start();
            }
        }

        public void Start(int interval)
        {
            Interval = interval;
            Start();
        }

        public void Stop()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            Stop();
            base.Dispose(disposing);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            needPaint = true;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (needPaint)
            {
                OnDraw(e.Graphics);
                needPaint = false;
            }
        }

        protected void OnDraw(Graphics g)
        {
            Draw?.Invoke(g);
        }
    }
}
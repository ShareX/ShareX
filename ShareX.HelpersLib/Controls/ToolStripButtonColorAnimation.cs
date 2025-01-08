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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class ToolStripButtonColorAnimation : ToolStripButton
    {
        [DefaultValue(typeof(Color), "ControlText")]
        public Color FromColor { get; set; }

        [DefaultValue(typeof(Color), "Red")]
        public Color ToColor { get; set; }

        [DefaultValue(1f)]
        public float AnimationSpeed { get; set; }

        private Timer timer;
        private float progress;
        private float direction = 1;
        private float speed;

        public ToolStripButtonColorAnimation()
        {
            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += timer_Tick;

            FromColor = SystemColors.ControlText;
            ToColor = Color.Red;
            AnimationSpeed = 1f;
        }

        public void StartAnimation()
        {
            speed = AnimationSpeed / (1000f / timer.Interval);
            timer.Start();
        }

        public void StopAnimation()
        {
            timer.Stop();
        }

        public void ResetAnimation()
        {
            StopAnimation();
            ForeColor = FromColor;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            progress += direction * speed;

            if (progress < 0)
            {
                progress = 0;
                direction = -direction;
            }
            else if (progress > 1)
            {
                progress = 1;
                direction = -direction;
            }

            ForeColor = ColorHelpers.Lerp(FromColor, ToColor, progress);
        }

        protected override void Dispose(bool disposing)
        {
            if (timer != null)
            {
                timer.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
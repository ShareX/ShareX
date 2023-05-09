#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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
using System.Windows.Forms;

namespace ShareX
{
    public partial class FirstTimeUploadForm : Form
    {
        private int countdown = 5;
        private string textYes;

        public FirstTimeUploadForm()
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this);
            lblHeader.BackColor = Color.DarkRed;
            lblHeader.ForeColor = Color.WhiteSmoke;

            btnYes.Enabled = false;
            textYes = btnYes.Text;
            UpdateCountdown();
            tCountdown.Start();
        }

        private void UpdateCountdown()
        {
            if (countdown < 1)
            {
                btnYes.Text = textYes;
                btnYes.Enabled = true;
                tCountdown.Stop();
            }
            else
            {
                btnYes.Text = textYes + " (" + countdown + ")";
                countdown--;
            }
        }

        private void FirstTimeUploadForm_Shown(object sender, System.EventArgs e)
        {
            this.ForceActivate();
        }

        private void tCountdown_Tick(object sender, System.EventArgs e)
        {
            if (!IsDisposed && NativeMethods.IsActive(Handle))
            {
                UpdateCountdown();
            }
        }

        private void btnYes_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void btnNo_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        public static bool ShowForm()
        {
            using (FirstTimeUploadForm form = new FirstTimeUploadForm())
            {
                return form.ShowDialog() == DialogResult.Yes;
            }
        }
    }
}
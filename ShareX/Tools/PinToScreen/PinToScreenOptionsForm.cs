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
using System.Windows.Forms;

namespace ShareX
{
    public partial class PinToScreenOptionsForm : Form
    {
        public PinToScreenOptions Options { get; private set; }

        public PinToScreenOptionsForm(PinToScreenOptions options)
        {
            Options = options;

            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            InitOptions();
            LoadOptions();
        }

        private void InitOptions()
        {
            cbPlacement.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<ContentAlignment>());
        }

        private void LoadOptions()
        {
            cbPlacement.SelectedIndex = Options.Placement.GetIndex();
            nudPlacementOffset.SetValue(Options.PlacementOffset);
            cbTopMost.Checked = Options.TopMost;
            cbKeepCenterLocation.Checked = Options.KeepCenterLocation;
            cbShadow.Checked = Options.Shadow;
            cbBorder.Checked = Options.Border;
            nudBorderSize.SetValue(Options.BorderSize);
            btnBorderColor.Color = Options.BorderColor;
            nudMinimizeSizeWidth.SetValue(Options.MinimizeSize.Width);
            nudMinimizeSizeHeight.SetValue(Options.MinimizeSize.Height);
        }

        private void SaveOptions()
        {
            Options.Placement = Helpers.GetEnumFromIndex<ContentAlignment>(cbPlacement.SelectedIndex);
            Options.PlacementOffset = (int)nudPlacementOffset.Value;
            Options.TopMost = cbTopMost.Checked;
            Options.KeepCenterLocation = cbKeepCenterLocation.Checked;
            Options.Shadow = cbShadow.Checked;
            Options.Border = cbBorder.Checked;
            Options.BorderSize = (int)nudBorderSize.Value;
            Options.BorderColor = btnBorderColor.Color;
            Options.MinimizeSize = new Size((int)nudMinimizeSizeWidth.Value, (int)nudMinimizeSizeHeight.Value);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveOptions();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
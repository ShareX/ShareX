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
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public partial class AccountsControl : UserControl
    {
        public AccountsControl()
        {
            InitializeComponent();
        }

        public void AddItem(object item)
        {
            lbAccounts.Items.Add(item);
            lbAccounts.SelectedIndex = lbAccounts.Items.Count - 1;
        }

        public bool RemoveItem(int selected)
        {
            if (selected.IsBetween(0, lbAccounts.Items.Count - 1))
            {
                lbAccounts.Items.RemoveAt(selected);

                if (lbAccounts.Items.Count > 0)
                {
                    lbAccounts.SelectedIndex = selected == lbAccounts.Items.Count ? lbAccounts.Items.Count - 1 : selected;
                    pgSettings.SelectedObject = lbAccounts.Items[lbAccounts.SelectedIndex];
                }
                else
                {
                    pgSettings.SelectedObject = null;
                }

                return true;
            }

            return false;
        }

        private void pgSettings_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (lbAccounts.SelectedIndex > -1)
            {
                lbAccounts.Items[lbAccounts.SelectedIndex] = pgSettings.SelectedObject;
            }
        }

        private void lbAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemove.Enabled = btnDuplicate.Enabled = btnTest.Enabled = lbAccounts.SelectedIndex > -1;

            if (lbAccounts.SelectedIndex > -1)
            {
                pgSettings.SelectedObject = lbAccounts.Items[lbAccounts.SelectedIndex];
            }
        }
    }
}
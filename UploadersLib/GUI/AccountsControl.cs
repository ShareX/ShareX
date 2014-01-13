#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
using System.Windows.Forms;

namespace UploadersLib
{
    public partial class AccountsControl : UserControl
    {
        public AccountsControl()
        {
            InitializeComponent();
        }

        public virtual bool RemoveItem(int selected)
        {
            if (selected.IsBetween(0, AccountsList.Items.Count - 1))
            {
                AccountsList.Items.RemoveAt(selected);

                if (AccountsList.Items.Count > 0)
                {
                    AccountsList.SelectedIndex = (selected > 0) ? (selected - 1) : 0;
                    SettingsGrid.SelectedObject = AccountsList.Items[selected.Between(0, selected - 1)];
                }
                else
                {
                    SettingsGrid.SelectedObject = null;
                }
                return true;
            }

            return false;
        }

        private void SettingsGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (AccountsList.SelectedIndex > -1)
            {
                AccountsList.Items[AccountsList.SelectedIndex] = SettingsGrid.SelectedObject;
            }
        }

        public virtual void AccountsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnClone.Enabled = AccountsList.SelectedIndex > -1;
        }
    }
}
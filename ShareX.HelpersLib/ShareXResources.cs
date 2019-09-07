#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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

using ShareX.HelpersLib.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public static class ShareXResources
    {
        public static string UserAgent
        {
            get
            {
                Version version = Version.Parse(Application.ProductVersion);
                return $"ShareX/{version.Major}.{version.Minor}.{version.Build}";
            }
        }

        public static bool UseDarkTheme { get; set; }

        private static bool experimentalDarkTheme;

        public static bool ExperimentalDarkTheme
        {
            get
            {
                return UseDarkTheme && experimentalDarkTheme;
            }
            set
            {
                experimentalDarkTheme = value;
            }
        }

        public static bool UseWhiteIcon { get; set; }

        public static Icon Icon => UseWhiteIcon ? Resources.ShareX_Icon_White : Resources.ShareX_Icon;
        public static Image Logo => Resources.ShareX_Logo;
        public static Image LogoBlack => Resources.ShareX_Logo_Black;

        public static ShareXTheme Theme { get; set; } = new ShareXTheme();

        public static void ApplyTheme(Form form, bool setIcon = true)
        {
            if (setIcon)
            {
                form.Icon = Icon;
            }

            if (ExperimentalDarkTheme)
            {
                ApplyDarkThemeToControl(form);

                if (form.IsHandleCreated)
                {
                    NativeMethods.UseImmersiveDarkMode(form.Handle, true);
                }
                else
                {
                    form.HandleCreated += (s, e) => NativeMethods.UseImmersiveDarkMode(form.Handle, true);
                }
            }
        }

        private static void ApplyDarkThemeToControl(Control control)
        {
            if (control.ContextMenuStrip != null)
            {
                control.ContextMenuStrip.Renderer = new ToolStripDarkRenderer();
            }

            if (control is MenuButton mb && mb.Menu != null)
            {
                mb.Menu.Renderer = new ToolStripDarkRenderer();
            }

            switch (control)
            {
                case Button btn:
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = Theme.BorderColor;
                    btn.ForeColor = Theme.TextColor;
                    btn.BackColor = Theme.BackgroundColor2;
                    return;
                case CheckBox cb when cb.Appearance == Appearance.Button:
                    cb.FlatStyle = FlatStyle.Flat;
                    cb.FlatAppearance.BorderColor = Theme.BorderColor;
                    cb.ForeColor = Theme.TextColor;
                    cb.BackColor = Theme.BackgroundColor2;
                    return;
                case TextBox tb:
                    tb.ForeColor = Theme.TextColor;
                    tb.BackColor = Theme.BackgroundColor2;
                    tb.BorderStyle = BorderStyle.FixedSingle;
                    return;
                case ComboBox cb:
                    cb.FlatStyle = FlatStyle.Flat;
                    cb.ForeColor = Theme.TextColor;
                    cb.BackColor = Theme.BackgroundColor2;
                    return;
                case ListBox lb:
                    lb.ForeColor = Theme.TextColor;
                    lb.BackColor = Theme.BackgroundColor2;
                    return;
                case ListView lv:
                    lv.ForeColor = Theme.TextColor;
                    lv.BackColor = Theme.BackgroundColor2;
                    lv.SupportDarkTheme();
                    return;
                case SplitContainer sc:
                    sc.Panel1.BackColor = Theme.BackgroundColor;
                    sc.Panel2.BackColor = Theme.BackgroundColor;
                    break;
                case PropertyGrid pg:
                    pg.CategoryForeColor = Theme.TextColor;
                    pg.CategorySplitterColor = Theme.BackgroundColor;
                    pg.LineColor = Theme.BackgroundColor;
                    pg.SelectedItemWithFocusForeColor = Theme.BackgroundColor;
                    pg.SelectedItemWithFocusBackColor = Theme.TextColor;
                    pg.ViewForeColor = Theme.TextColor;
                    pg.ViewBackColor = Theme.BackgroundColor2;
                    pg.ViewBorderColor = Theme.BorderColor;
                    pg.HelpForeColor = Theme.TextColor;
                    pg.HelpBackColor = Theme.BackgroundColor;
                    pg.HelpBorderColor = Theme.BorderColor;
                    return;
                case DataGridView dgv:
                    dgv.BackgroundColor = Theme.BackgroundColor2;
                    dgv.GridColor = Theme.BorderColor;
                    dgv.DefaultCellStyle.BackColor = Theme.BackgroundColor2;
                    dgv.DefaultCellStyle.SelectionBackColor = Theme.BackgroundColor2;
                    dgv.DefaultCellStyle.ForeColor = Theme.TextColor;
                    dgv.DefaultCellStyle.SelectionForeColor = Theme.TextColor;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = Theme.BackgroundColor;
                    dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Theme.BackgroundColor;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Theme.TextColor;
                    dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Theme.TextColor;
                    dgv.EnableHeadersVisualStyles = false;
                    break;
                case ToolStrip ts:
                    ts.Renderer = new ToolStripDarkRenderer();
                    ApplyDarkThemeToToolStripItemCollection(ts.Items);
                    return;
                case LinkLabel ll:
                    ll.LinkColor = Theme.LinkColor;
                    break;
            }

            control.ForeColor = Theme.TextColor;
            control.BackColor = Theme.BackgroundColor;

            foreach (Control child in control.Controls)
            {
                ApplyDarkThemeToControl(child);
            }
        }

        private static void ApplyDarkThemeToToolStripItemCollection(ToolStripItemCollection collection)
        {
            foreach (ToolStripItem tsi in collection)
            {
                switch (tsi)
                {
                    case ToolStripControlHost tsch:
                        ApplyDarkThemeToControl(tsch.Control);
                        break;
                    case ToolStripDropDownItem tsddi:
                        ApplyDarkThemeToToolStripItemCollection(tsddi.DropDownItems);
                        break;
                }
            }
        }
    }
}
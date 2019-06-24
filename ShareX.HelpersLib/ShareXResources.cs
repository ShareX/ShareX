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
        public static bool UseWhiteIcon { get; set; }
        public static bool ApplyTheme { get; set; } = true;

        public static Icon Icon => UseWhiteIcon ? Resources.ShareX_Icon_White : Resources.ShareX_Icon;

        public static Image Logo => Resources.ShareX_Logo;
        public static Image LogoBlack => Resources.ShareX_Logo_Black;

        public static Color BackgroundColor => UseDarkTheme ? DarkBackgroundColor : SystemColors.Window;
        public static Color TextColor => UseDarkTheme ? DarkTextColor : SystemColors.ControlText;
        public static Color BorderColor => UseDarkTheme ? DarkBorderColor : ProfessionalColors.SeparatorDark;
        public static Color CheckerColor1 => UseDarkTheme ? DarkCheckerColor1 : SystemColors.ControlLightLight;
        public static Color CheckerColor2 => UseDarkTheme ? DarkCheckerColor2 : SystemColors.ControlLight;

        public static Color DarkBackgroundColor { get; } = Color.FromArgb(42, 47, 56);
        public static Color DarkTextColor { get; } = Color.FromArgb(235, 235, 235);
        public static Color DarkBorderColor { get; } = Color.FromArgb(28, 32, 38);
        public static Color DarkCheckerColor1 { get; } = Color.FromArgb(60, 60, 60);
        public static Color DarkCheckerColor2 { get; } = Color.FromArgb(50, 50, 50);

        public static int CheckerSize { get; } = 15;

        public static void ApplyThemeToForm(Form form)
        {
            form.Icon = Icon;

            if (ApplyTheme)
            {
                ApplyThemeToControl(form);

                if (form.IsHandleCreated)
                {
                    NativeMethods.UseImmersiveDarkMode(form.Handle, UseDarkTheme);
                }
                else
                {
                    form.HandleCreated += (s, e) => NativeMethods.UseImmersiveDarkMode(form.Handle, UseDarkTheme);
                }
            }
        }

        private static void ApplyThemeToControl(Control control)
        {
            switch (control)
            {
                case Button btn:
                case CheckBox cb when cb.Appearance == Appearance.Button:
                    // Buttons looks better with system colors
                    control.ForeColor = SystemColors.ControlText;
                    return;
                case SplitContainer sc:
                    sc.Panel1.BackColor = BackgroundColor;
                    sc.Panel2.BackColor = BackgroundColor;
                    break;
                case PropertyGrid pg:
                    pg.CategoryForeColor = TextColor;
                    pg.CategorySplitterColor = BorderColor;
                    pg.LineColor = BorderColor;
                    pg.SelectedItemWithFocusForeColor = BorderColor;
                    pg.SelectedItemWithFocusBackColor = TextColor;
                    break;
                case DataGridView dgv:
                    dgv.BackgroundColor = BackgroundColor;
                    dgv.GridColor = BorderColor;
                    dgv.DefaultCellStyle.BackColor = BackgroundColor;
                    dgv.DefaultCellStyle.SelectionBackColor = BackgroundColor;
                    dgv.DefaultCellStyle.ForeColor = TextColor;
                    dgv.DefaultCellStyle.SelectionForeColor = TextColor;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = BorderColor;
                    dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = BorderColor;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextColor;
                    dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextColor;
                    dgv.EnableHeadersVisualStyles = false;
                    break;
                case ToolStrip ts:
                    ts.Renderer = new ToolStripDarkRenderer();
                    ApplyThemeToToolStripItemCollection(ts.Items);
                    return;
                case LinkLabel ll:
                    ll.LinkColor = Color.FromArgb(166, 212, 255);
                    break;
            }

            control.ForeColor = TextColor;
            control.BackColor = BackgroundColor;

            foreach (Control child in control.Controls)
            {
                ApplyThemeToControl(child);
            }
        }

        private static void ApplyThemeToToolStripItemCollection(ToolStripItemCollection collection)
        {
            foreach (ToolStripItem tsi in collection)
            {
                switch (tsi)
                {
                    case ToolStripControlHost tsch:
                        ApplyThemeToControl(tsch.Control);
                        break;
                    case ToolStripDropDownItem tsddi:
                        ApplyThemeToToolStripItemCollection(tsddi.DropDownItems);
                        break;
                }
            }
        }
    }
}
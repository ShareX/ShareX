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

        private static bool experimentalDarkTheme;

        public static bool UseWhiteIcon { get; set; }

        public static Icon Icon => UseWhiteIcon ? Resources.ShareX_Icon_White : Resources.ShareX_Icon;
        public static Image Logo => Resources.ShareX_Logo;
        public static Image LogoBlack => Resources.ShareX_Logo_Black;

        public static Color BackgroundColor => UseDarkTheme ? DarkBackgroundColor : SystemColors.Window;
        public static Color TextColor => UseDarkTheme ? DarkTextColor : SystemColors.ControlText;
        public static Color BorderColor => UseDarkTheme ? DarkBorderColor : ProfessionalColors.SeparatorDark;
        public static Color CheckerColor1 => UseDarkTheme ? DarkCheckerColor1 : SystemColors.ControlLightLight;
        public static Color CheckerColor2 => UseDarkTheme ? DarkCheckerColor2 : SystemColors.ControlLight;
        public static int CheckerSize { get; } = 15;

        public static Color DarkBackgroundColor { get; } = Color.FromArgb(42, 47, 56);
        public static Color DarkBackgroundVariantColor { get; } = ColorHelpers.LighterColor(DarkBackgroundColor, 0.05f);
        public static Color DarkTextColor { get; } = Color.FromArgb(235, 235, 235);
        public static Color DarkBorderColor { get; } = Color.FromArgb(28, 32, 38);
        public static Color DarkCheckerColor1 { get; } = Color.FromArgb(60, 60, 60);
        public static Color DarkCheckerColor2 { get; } = Color.FromArgb(50, 50, 50);
        public static Color DarkLinkColor { get; } = Color.FromArgb(166, 212, 255);

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
                    btn.FlatAppearance.BorderColor = DarkBorderColor;
                    btn.ForeColor = DarkTextColor;
                    btn.BackColor = DarkBackgroundVariantColor;
                    return;
                case CheckBox cb when cb.Appearance == Appearance.Button:
                    cb.FlatStyle = FlatStyle.Flat;
                    cb.FlatAppearance.BorderColor = DarkBorderColor;
                    cb.ForeColor = DarkTextColor;
                    cb.BackColor = DarkBackgroundVariantColor;
                    return;
                case TextBox tb:
                    tb.ForeColor = DarkTextColor;
                    tb.BackColor = DarkBackgroundVariantColor;
                    tb.BorderStyle = BorderStyle.FixedSingle;
                    return;
                case ComboBox cb:
                    cb.FlatStyle = FlatStyle.Flat;
                    cb.ForeColor = DarkTextColor;
                    cb.BackColor = DarkBackgroundVariantColor;
                    return;
                case ListBox lb:
                    lb.ForeColor = DarkTextColor;
                    lb.BackColor = DarkBackgroundVariantColor;
                    return;
                case ListView lv:
                    lv.ForeColor = DarkTextColor;
                    lv.BackColor = DarkBackgroundVariantColor;
                    lv.SupportDarkTheme();
                    return;
                case SplitContainer sc:
                    sc.Panel1.BackColor = DarkBackgroundColor;
                    sc.Panel2.BackColor = DarkBackgroundColor;
                    break;
                case PropertyGrid pg:
                    pg.CategoryForeColor = DarkTextColor;
                    pg.CategorySplitterColor = DarkBackgroundColor;
                    pg.LineColor = DarkBackgroundColor;
                    pg.SelectedItemWithFocusForeColor = DarkBackgroundColor;
                    pg.SelectedItemWithFocusBackColor = DarkTextColor;
                    pg.ViewForeColor = DarkTextColor;
                    pg.ViewBackColor = DarkBackgroundVariantColor;
                    pg.ViewBorderColor = DarkBorderColor;
                    pg.HelpForeColor = DarkTextColor;
                    pg.HelpBackColor = DarkBackgroundColor;
                    pg.HelpBorderColor = DarkBorderColor;
                    return;
                case DataGridView dgv:
                    dgv.BackgroundColor = DarkBackgroundVariantColor;
                    dgv.GridColor = DarkBorderColor;
                    dgv.DefaultCellStyle.BackColor = DarkBackgroundVariantColor;
                    dgv.DefaultCellStyle.SelectionBackColor = DarkBackgroundVariantColor;
                    dgv.DefaultCellStyle.ForeColor = DarkTextColor;
                    dgv.DefaultCellStyle.SelectionForeColor = DarkTextColor;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = DarkBackgroundColor;
                    dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = DarkBackgroundColor;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = DarkTextColor;
                    dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = DarkTextColor;
                    dgv.EnableHeadersVisualStyles = false;
                    break;
                case ToolStrip ts:
                    ts.Renderer = new ToolStripDarkRenderer();
                    ApplyDarkThemeToToolStripItemCollection(ts.Items);
                    return;
                case LinkLabel ll:
                    ll.LinkColor = DarkLinkColor;
                    break;
            }

            control.ForeColor = DarkTextColor;
            control.BackColor = DarkBackgroundColor;

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
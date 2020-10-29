#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
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

        private static bool useCustomTheme;

        public static bool UseCustomTheme
        {
            get
            {
                return useCustomTheme && Theme != null;
            }
            set
            {
                useCustomTheme = value;
            }
        }

        public static bool IsDarkTheme => UseCustomTheme && Theme.IsDarkTheme;

        public static bool UseWhiteIcon { get; set; }

        public static Icon Icon => UseWhiteIcon ? Resources.ShareX_Icon_White : Resources.ShareX_Icon;

        public static Bitmap Logo => Resources.ShareX_Logo;

        public static ShareXTheme Theme { get; set; } = new ShareXTheme();

        public static void ApplyTheme(Form form, bool setIcon = true)
        {
            if (setIcon)
            {
                form.Icon = Icon;
            }

            if (UseCustomTheme)
            {
                ApplyCustomThemeToControl(form);

                IContainer components = form.GetType().GetField("components", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(form) as IContainer;
                ApplyCustomThemeToComponents(components);

                if (form.IsHandleCreated)
                {
                    NativeMethods.UseImmersiveDarkMode(form.Handle, Theme.IsDarkTheme);
                }
                else
                {
                    form.HandleCreated += (s, e) => NativeMethods.UseImmersiveDarkMode(form.Handle, Theme.IsDarkTheme);
                }
            }
        }

        private static void ApplyCustomThemeToControl(Control control)
        {
            if (control.ContextMenuStrip != null)
            {
                ApplyCustomThemeToContextMenuStrip(control.ContextMenuStrip);
            }

            if (control is MenuButton mb && mb.Menu != null)
            {
                ApplyCustomThemeToContextMenuStrip(mb.Menu);
            }

            switch (control)
            {
                case Button btn:
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = Theme.BorderColor;
                    btn.ForeColor = Theme.TextColor;
                    btn.BackColor = Theme.LightBackgroundColor;
                    return;
                case CheckBox cb when cb.Appearance == Appearance.Button:
                    cb.FlatStyle = FlatStyle.Flat;
                    cb.FlatAppearance.BorderColor = Theme.BorderColor;
                    cb.ForeColor = Theme.TextColor;
                    cb.BackColor = Theme.LightBackgroundColor;
                    return;
                case TextBox tb:
                    tb.ForeColor = Theme.TextColor;
                    tb.BackColor = Theme.LightBackgroundColor;
                    tb.BorderStyle = BorderStyle.FixedSingle;
                    return;
                case ComboBox cb:
                    cb.FlatStyle = FlatStyle.Flat;
                    cb.ForeColor = Theme.TextColor;
                    cb.BackColor = Theme.LightBackgroundColor;
                    return;
                case ListBox lb:
                    lb.ForeColor = Theme.TextColor;
                    lb.BackColor = Theme.LightBackgroundColor;
                    return;
                case ListView lv:
                    lv.ForeColor = Theme.TextColor;
                    lv.BackColor = Theme.LightBackgroundColor;
                    lv.SupportCustomTheme();
                    return;
                case SplitContainerCustomSplitter sccs:
                    sccs.SplitterColor = Theme.BackgroundColor;
                    sccs.SplitterLineColor = Theme.BorderColor;
                    sccs.Panel1.BackColor = Theme.BackgroundColor;
                    sccs.Panel2.BackColor = Theme.BackgroundColor;
                    break;
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
                    pg.ViewBackColor = Theme.LightBackgroundColor;
                    pg.ViewBorderColor = Theme.BorderColor;
                    pg.HelpForeColor = Theme.TextColor;
                    pg.HelpBackColor = Theme.BackgroundColor;
                    pg.HelpBorderColor = Theme.BorderColor;
                    return;
                case DataGridView dgv:
                    dgv.BackgroundColor = Theme.LightBackgroundColor;
                    dgv.GridColor = Theme.BorderColor;
                    dgv.DefaultCellStyle.BackColor = Theme.LightBackgroundColor;
                    dgv.DefaultCellStyle.SelectionBackColor = Theme.LightBackgroundColor;
                    dgv.DefaultCellStyle.ForeColor = Theme.TextColor;
                    dgv.DefaultCellStyle.SelectionForeColor = Theme.TextColor;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = Theme.BackgroundColor;
                    dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Theme.BackgroundColor;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Theme.TextColor;
                    dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Theme.TextColor;
                    dgv.EnableHeadersVisualStyles = false;
                    break;
                case ContextMenuStrip cms:
                    ApplyCustomThemeToContextMenuStrip(cms);
                    return;
                case ToolStrip ts:
                    ts.Renderer = new ToolStripDarkRenderer();
                    ApplyCustomThemeToToolStripItemCollection(ts.Items);
                    return;
                case LinkLabel ll:
                    ll.LinkColor = Theme.LinkColor;
                    break;
            }

            control.ForeColor = Theme.TextColor;
            control.BackColor = Theme.BackgroundColor;

            foreach (Control child in control.Controls)
            {
                ApplyCustomThemeToControl(child);
            }
        }

        private static void ApplyCustomThemeToComponents(IContainer container)
        {
            if (container != null)
            {
                foreach (IComponent component in container.Components)
                {
                    switch (component)
                    {
                        case ContextMenuStrip cms:
                            ApplyCustomThemeToContextMenuStrip(cms);
                            break;
                        case ToolTip tt:
                            tt.ForeColor = Theme.TextColor;
                            tt.BackColor = Theme.BackgroundColor;
                            tt.OwnerDraw = true;
                            tt.Draw -= ToolTip_Draw;
                            tt.Draw += ToolTip_Draw;
                            break;
                    }
                }
            }
        }

        private static void ToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();
            e.DrawBorder();
            e.DrawText(TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding);
        }

        public static void ApplyCustomThemeToContextMenuStrip(ContextMenuStrip cms)
        {
            if (cms != null)
            {
                cms.Renderer = new ToolStripDarkRenderer();
                cms.Font = Theme.ContextMenuFont;
                cms.Opacity = Theme.ContextMenuOpacityDouble;
                ApplyCustomThemeToToolStripItemCollection(cms.Items);
            }
        }

        private static void ApplyCustomThemeToToolStripItemCollection(ToolStripItemCollection collection)
        {
            foreach (ToolStripItem tsi in collection)
            {
                switch (tsi)
                {
                    case ToolStripControlHost tsch:
                        ApplyCustomThemeToControl(tsch.Control);
                        break;
                    case ToolStripDropDownItem tsddi:
                        if (tsddi.DropDown != null)
                        {
                            tsddi.DropDown.Opacity = Theme.ContextMenuOpacityDouble;
                            ApplyCustomThemeToToolStripItemCollection(tsddi.DropDownItems);
                        }
                        break;
                }
            }
        }
    }
}
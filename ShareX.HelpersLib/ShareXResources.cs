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

using ShareX.HelpersLib.Properties;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public static class ShareXResources
    {
        public static string Name { get; set; } = "ShareX";

        public static string UserAgent
        {
            get
            {
                return $"{Name}/{Helpers.GetApplicationVersion()}";
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

        private static bool useWhiteIcon;

        public static bool UseWhiteIcon
        {
            get
            {
                return useWhiteIcon;
            }
            set
            {
                if (useWhiteIcon != value)
                {
                    useWhiteIcon = value;

                    if (useWhiteIcon)
                    {
                        Icon = Resources.ShareX_Icon_White;
                    }
                    else
                    {
                        Icon = Resources.ShareX_Icon;
                    }
                }
            }
        }

        private static Icon icon = Resources.ShareX_Icon;

        public static Icon Icon
        {
            get
            {
                return icon.CloneSafe();
            }
            set
            {
                if (icon != value)
                {
                    icon?.Dispose();
                    icon = value;
                }
            }
        }

        private static Bitmap logo = Resources.ShareX_Logo;

        public static Bitmap Logo
        {
            get
            {
                return logo.CloneSafe();
            }
            set
            {
                if (logo != value)
                {
                    logo?.Dispose();
                    logo = value;
                }
            }
        }

        public static ShareXTheme Theme { get; set; } = ShareXTheme.DarkTheme;

        public static void ApplyTheme(Form form, bool closeOnEscape = false, bool setIcon = true)
        {
            if (closeOnEscape)
            {
                form.CloseOnEscape();
            }

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

        public static void ApplyCustomThemeToControl(Control control)
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
                case ColorButton colorButton:
                    colorButton.FlatStyle = FlatStyle.Flat;
                    colorButton.FlatAppearance.BorderColor = Theme.BorderColor;
                    colorButton.ForeColor = Theme.TextColor;
                    colorButton.BackColor = Theme.LightBackgroundColor;
                    colorButton.BorderColor = Theme.BorderColor;
                    return;
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
                    ts.Font = Theme.MenuFont;
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

            switch (control)
            {
                case TabToTreeView tttv:
                    tttv.LeftPanelBackColor = Theme.DarkBackgroundColor;
                    tttv.SeparatorColor = Theme.SeparatorDarkColor;
                    break;
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
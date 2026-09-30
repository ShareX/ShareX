#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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
    // Windows-only members of ShareXResources, kept in the same namespace so existing call sites keep compiling.
    public static class ShareXResourcesWindows
    {
        private static bool useWhiteIcon;
        private static Icon icon;
        private static Bitmap logo;
        private static ShareXTheme theme;

        extension(ShareXResources)
        {
            public static bool IsDarkTheme => ShareXResources.Theme.IsDarkTheme;


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
                            ShareXResources.Icon = Resources.ShareX_Icon_White;
                        }
                        else
                        {
                            ShareXResources.Icon = Resources.ShareX_Icon;
                        }
                    }
                }
            }


            public static Icon Icon
            {
                get
                {
                    return (icon ??= Resources.ShareX_Icon).CloneSafe();
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


            public static Bitmap Logo
            {
                get
                {
                    return (logo ??= Resources.ShareX_Logo).CloneSafe();
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


            public static ShareXTheme Theme
            {
                get => theme ??= ShareXTheme.DarkTheme;
                set => theme = value;
            }

            public static void ApplyTheme(Form form, bool closeOnEscape = false, bool setIcon = true)
            {
                if (closeOnEscape)
                {
                    form.CloseOnEscape();
                }

                if (setIcon)
                {
                    form.Icon = ShareXResources.Icon;
                }

                ApplyCustomThemeToControl(form);

                IContainer components = form.GetType().GetField("components", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(form) as IContainer;
                ApplyCustomThemeToComponents(components);

                if (form.IsHandleCreated)
                {
                    NativeMethods.UseImmersiveDarkMode(form.Handle, ShareXResources.Theme.IsDarkTheme);
                }
                else
                {
                    form.HandleCreated += (s, e) => NativeMethods.UseImmersiveDarkMode(form.Handle, ShareXResources.Theme.IsDarkTheme);
                }
            }

            public static void ApplyCustomThemeToControl(Control control)
            {
                if (control.ContextMenuStrip != null)
                {
                    ApplyCustomThemeToContextMenuStrip(control.ContextMenuStrip);
                }

                switch (control)
                {
                    case Button btn:
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.FlatAppearance.BorderColor = ShareXResources.Theme.BorderColor;
                        btn.ForeColor = ShareXResources.Theme.TextColor;
                        btn.BackColor = ShareXResources.Theme.LightBackgroundColor;
                        return;
                    case CheckBox cb when cb.Appearance == Appearance.Button:
                        cb.FlatStyle = FlatStyle.Flat;
                        cb.FlatAppearance.BorderColor = ShareXResources.Theme.BorderColor;
                        cb.ForeColor = ShareXResources.Theme.TextColor;
                        cb.BackColor = ShareXResources.Theme.LightBackgroundColor;
                        return;
                    case TextBox tb:
                        tb.ForeColor = ShareXResources.Theme.TextColor;
                        tb.BackColor = ShareXResources.Theme.LightBackgroundColor;
                        tb.BorderStyle = BorderStyle.FixedSingle;
                        return;
                    case ComboBox cb:
                        cb.FlatStyle = FlatStyle.Flat;
                        cb.ForeColor = ShareXResources.Theme.TextColor;
                        cb.BackColor = ShareXResources.Theme.LightBackgroundColor;
                        return;
                    case ListBox lb:
                        lb.ForeColor = ShareXResources.Theme.TextColor;
                        lb.BackColor = ShareXResources.Theme.LightBackgroundColor;
                        return;
                    case ListView lv:
                        lv.ForeColor = ShareXResources.Theme.TextColor;
                        lv.BackColor = ShareXResources.Theme.LightBackgroundColor;
                        lv.SupportCustomTheme();
                        return;
                    case SplitContainer sc:
                        sc.Panel1.BackColor = ShareXResources.Theme.BackgroundColor;
                        sc.Panel2.BackColor = ShareXResources.Theme.BackgroundColor;
                        break;
                    case PropertyGrid pg:
                        pg.CategoryForeColor = ShareXResources.Theme.TextColor;
                        pg.CategorySplitterColor = ShareXResources.Theme.BackgroundColor;
                        pg.LineColor = ShareXResources.Theme.BackgroundColor;
                        pg.SelectedItemWithFocusForeColor = ShareXResources.Theme.BackgroundColor;
                        pg.SelectedItemWithFocusBackColor = ShareXResources.Theme.TextColor;
                        pg.ViewForeColor = ShareXResources.Theme.TextColor;
                        pg.ViewBackColor = ShareXResources.Theme.LightBackgroundColor;
                        pg.ViewBorderColor = ShareXResources.Theme.BorderColor;
                        pg.HelpForeColor = ShareXResources.Theme.TextColor;
                        pg.HelpBackColor = ShareXResources.Theme.BackgroundColor;
                        pg.HelpBorderColor = ShareXResources.Theme.BorderColor;
                        return;
                    case DataGridView dgv:
                        dgv.BackgroundColor = ShareXResources.Theme.LightBackgroundColor;
                        dgv.GridColor = ShareXResources.Theme.BorderColor;
                        dgv.DefaultCellStyle.BackColor = ShareXResources.Theme.LightBackgroundColor;
                        dgv.DefaultCellStyle.SelectionBackColor = ShareXResources.Theme.LightBackgroundColor;
                        dgv.DefaultCellStyle.ForeColor = ShareXResources.Theme.TextColor;
                        dgv.DefaultCellStyle.SelectionForeColor = ShareXResources.Theme.TextColor;
                        dgv.ColumnHeadersDefaultCellStyle.BackColor = ShareXResources.Theme.BackgroundColor;
                        dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = ShareXResources.Theme.BackgroundColor;
                        dgv.ColumnHeadersDefaultCellStyle.ForeColor = ShareXResources.Theme.TextColor;
                        dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = ShareXResources.Theme.TextColor;
                        dgv.EnableHeadersVisualStyles = false;
                        break;
                    case ContextMenuStrip cms:
                        ApplyCustomThemeToContextMenuStrip(cms);
                        return;
                    case ToolStrip ts:
                        ts.Font = ShareXResources.Theme.MenuFont;
                        ApplyCustomThemeToToolStripItemCollection(ts.Items);
                        return;
                    case LinkLabel ll:
                        ll.LinkColor = ShareXResources.Theme.LinkColor;
                        break;
                }

                control.ForeColor = ShareXResources.Theme.TextColor;
                control.BackColor = ShareXResources.Theme.BackgroundColor;

                foreach (Control child in control.Controls)
                {
                    ApplyCustomThemeToControl(child);
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
                    cms.Font = ShareXResources.Theme.ContextMenuFont;
                    cms.Opacity = ShareXResources.Theme.ContextMenuOpacityDouble;
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
                                tsddi.DropDown.Opacity = ShareXResources.Theme.ContextMenuOpacityDouble;
                                ApplyCustomThemeToToolStripItemCollection(tsddi.DropDownItems);
                            }
                            break;
                    }
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
                                tt.ForeColor = ShareXResources.Theme.TextColor;
                                tt.BackColor = ShareXResources.Theme.BackgroundColor;
                                tt.OwnerDraw = true;
                                tt.Draw -= ToolTip_Draw;
                                tt.Draw += ToolTip_Draw;
                                break;
                        }
                    }
                }
            }

        }
    }
}

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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public static class FormExtensions
    {
        public static void ForceActivate(this Form form)
        {
            if (!form.IsDisposed)
            {
                if (!form.Visible)
                {
                    form.Show();
                }

                if (form.WindowState == FormWindowState.Minimized)
                {
                    NativeMethods.ShowWindow(form.Handle, (int)WindowShowStyle.Restore);
                }

                form.Activate();
                form.BringToFront();
            }
        }

        public static void CloseOnEscape(this Form form)
        {
            bool escapePressed = false;

            form.KeyPreview = true;

            form.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    escapePressed = true;
                    e.SuppressKeyPress = true;
                }
            };

            form.KeyUp += (sender, e) =>
            {
                if (e.KeyCode == Keys.Escape && escapePressed)
                {
                    escapePressed = false;
                    form.DialogResult = DialogResult.Cancel;
                    form.Close();
                }
            };
        }

        public static void InvokeSafe(this Control control, Action action)
        {
            if (control != null && !control.IsDisposed)
            {
                if (control.InvokeRequired)
                {
                    control.Invoke(action);
                }
                else
                {
                    action();
                }
            }
        }

        public static void ChangeFontStyle(this Control control, FontStyle fontStyle)
        {
            control.Font = new Font(control.Font, fontStyle);
        }

        public static void BeginUpdate(this RichTextBox rtb)
        {
            NativeMethods.SendMessage(rtb.Handle, (int)WindowsMessages.SETREDRAW, 0, 0);
        }

        public static void EndUpdate(this RichTextBox rtb)
        {
            NativeMethods.SendMessage(rtb.Handle, (int)WindowsMessages.SETREDRAW, 1, 0);
            rtb.Invalidate();
        }

        public static void SetFontRegular(this RichTextBox rtb)
        {
            rtb.SelectionFont = new Font(rtb.Font, FontStyle.Regular);
        }

        public static void SetFontBold(this RichTextBox rtb)
        {
            rtb.SelectionFont = new Font(rtb.Font, FontStyle.Bold);
        }

        public static void AppendText(this RichTextBox rtb, string text, FontStyle fontStyle, float fontSize = 0)
        {
            Font font;

            if (fontSize > 0)
            {
                font = new Font(rtb.Font.FontFamily, fontSize, fontStyle);
            }
            else
            {
                font = new Font(rtb.Font, fontStyle);
            }

            rtb.SelectionFont = font;
            rtb.AppendText(text);
        }

        public static void AppendLine(this RichTextBox rtb, string text = "")
        {
            rtb.AppendText(text + Environment.NewLine);
        }

        public static void AppendLine(this RichTextBox rtb, string text, FontStyle fontStyle, float fontSize = 0)
        {
            rtb.AppendText(text + Environment.NewLine, fontStyle, fontSize);
        }

        public static void AddContextMenu(this RichTextBox rtb)
        {
            if (rtb.ContextMenuStrip == null)
            {
                ContextMenuStrip cms = new ContextMenuStrip()
                {
                    ShowImageMargin = false
                };

                ToolStripMenuItem tsmiUndo = new ToolStripMenuItem(Resources.Extensions_AddContextMenu_Undo);
                tsmiUndo.Click += (sender, e) => rtb.Undo();
                cms.Items.Add(tsmiUndo);

                ToolStripMenuItem tsmiRedo = new ToolStripMenuItem(Resources.Extensions_AddContextMenu_Redo);
                tsmiRedo.Click += (sender, e) => rtb.Redo();
                cms.Items.Add(tsmiRedo);

                cms.Items.Add(new ToolStripSeparator());

                ToolStripMenuItem tsmiCut = new ToolStripMenuItem(Resources.Extensions_AddContextMenu_Cut);
                tsmiCut.Click += (sender, e) => rtb.Cut();
                cms.Items.Add(tsmiCut);

                ToolStripMenuItem tsmiCopy = new ToolStripMenuItem(Resources.Extensions_AddContextMenu_Copy);
                tsmiCopy.Click += (sender, e) => rtb.Copy();
                cms.Items.Add(tsmiCopy);

                ToolStripMenuItem tsmiPaste = new ToolStripMenuItem(Resources.Extensions_AddContextMenu_Paste);
                tsmiPaste.Click += (sender, e) => rtb.Paste();
                cms.Items.Add(tsmiPaste);

                ToolStripMenuItem tsmiDelete = new ToolStripMenuItem(Resources.Extensions_AddContextMenu_Delete);
                tsmiDelete.Click += (sender, e) => rtb.SelectedText = "";
                cms.Items.Add(tsmiDelete);

                cms.Items.Add(new ToolStripSeparator());

                ToolStripMenuItem tsmiSelectAll = new ToolStripMenuItem(Resources.Extensions_AddContextMenu_SelectAll);
                tsmiSelectAll.Click += (sender, e) => rtb.SelectAll();
                cms.Items.Add(tsmiSelectAll);

                cms.Opening += (sender, e) =>
                {
                    tsmiUndo.Enabled = !rtb.ReadOnly && rtb.CanUndo;
                    tsmiRedo.Enabled = !rtb.ReadOnly && rtb.CanRedo;
                    tsmiCut.Enabled = !rtb.ReadOnly && rtb.SelectionLength > 0;
                    tsmiCopy.Enabled = rtb.SelectionLength > 0;
                    tsmiPaste.Enabled = !rtb.ReadOnly && ClipboardHelpers.ContainsText();
                    tsmiDelete.Enabled = !rtb.ReadOnly && rtb.SelectionLength > 0;
                    tsmiSelectAll.Enabled = rtb.TextLength > 0 && rtb.SelectionLength < rtb.TextLength;
                };

                rtb.ContextMenuStrip = cms;
            }
        }

        public static void SupportSelectAll(this TextBox tb)
        {
            tb.KeyDown += (sender, e) =>
            {
                if (e.KeyData == (Keys.Control | Keys.A))
                {
                    tb.SelectAll();

                    e.SuppressKeyPress = true;
                }
            };
        }

        public static void SetWatermark(this TextBox textBox, string watermarkText, bool showCueWhenFocus = false)
        {
            if (textBox != null && textBox.IsHandleCreated && watermarkText != null)
            {
                NativeMethods.SendMessage(textBox.Handle, (int)NativeConstants.EM_SETCUEBANNER, showCueWhenFocus ? 1 : 0, watermarkText);
            }
        }

        public static void AppendTextToSelection(this TextBoxBase tb, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                int start = tb.SelectionStart;
                tb.Text = tb.Text.Insert(start, text);
                tb.SelectionStart = start + text.Length;
            }
        }

        public static void RadioCheck(this ToolStripMenuItem tsmi)
        {
            ToolStripDropDownItem tsddiParent = tsmi.OwnerItem as ToolStripDropDownItem;

            foreach (ToolStripMenuItem tsmiChild in tsddiParent.DropDownItems.OfType<ToolStripMenuItem>())
            {
                tsmiChild.Checked = tsmiChild == tsmi;
            }
        }

        public static void UpdateCheckedAll(this ToolStripMenuItem tsmi, bool check)
        {
            foreach (ToolStripMenuItem tsmiChild in tsmi.DropDownItems.OfType<ToolStripMenuItem>())
            {
                tsmiChild.Checked = check;
            }
        }

        public static void RadioCheck(this ToolStripButton tsb)
        {
            ToolStrip parent = tsb.GetCurrentParent();

            foreach (ToolStripButton tsbParent in parent.Items.OfType<ToolStripButton>())
            {
                if (tsbParent != tsb)
                {
                    tsbParent.Checked = false;
                }
            }

            tsb.Checked = true;
        }

        public static void SupportCustomTheme(this ListView lv)
        {
            if (!lv.OwnerDraw)
            {
                lv.OwnerDraw = true;

                lv.DrawItem += (sender, e) =>
                {
                    e.DrawDefault = true;
                };

                lv.DrawSubItem += (sender, e) =>
                {
                    e.DrawDefault = true;
                };

                lv.DrawColumnHeader += (sender, e) =>
                {
                    if (ShareXResources.UseCustomTheme)
                    {
                        using (Brush brush = new SolidBrush(ShareXResources.Theme.BackgroundColor))
                        {
                            e.Graphics.FillRectangle(brush, e.Bounds);
                        }

                        TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font, e.Bounds.LocationOffset(2, 0).SizeOffset(-4, 0), ShareXResources.Theme.TextColor,
                            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                        if (e.Bounds.Right < lv.ClientRectangle.Right)
                        {
                            using (Pen pen = new Pen(ShareXResources.Theme.SeparatorDarkColor))
                            using (Pen pen2 = new Pen(ShareXResources.Theme.SeparatorLightColor))
                            {
                                e.Graphics.DrawLine(pen, e.Bounds.Right - 2, e.Bounds.Top, e.Bounds.Right - 2, e.Bounds.Bottom - 1);
                                e.Graphics.DrawLine(pen2, e.Bounds.Right - 1, e.Bounds.Top, e.Bounds.Right - 1, e.Bounds.Bottom - 1);
                            }
                        }
                    }
                    else
                    {
                        e.DrawDefault = true;
                    }
                };
            }
        }

        public static void MoveUp(this ListViewItem lvi)
        {
            ListView lv = lvi.ListView;

            if (lv.Items.Count > 1)
            {
                int index = lvi.Index;

                if (index == 0)
                {
                    index = lv.Items.Count - 1;
                }
                else
                {
                    index--;
                }

                lv.Items.Remove(lvi);
                lv.Items.Insert(index, lvi);
            }

            lv.Focus();
            lvi.EnsureVisible();
            lvi.Selected = true;
        }

        public static void MoveDown(this ListViewItem lvi)
        {
            ListView lv = lvi.ListView;

            if (lv.Items.Count > 1)
            {
                int index = lvi.Index;

                if (index == lv.Items.Count - 1)
                {
                    index = 0;
                }
                else
                {
                    index++;
                }

                lv.Items.Remove(lvi);
                lv.Items.Insert(index, lvi);
            }

            lv.Focus();
            lvi.EnsureVisible();
            lvi.Selected = true;
        }

        public static void HideImageMargin(this ToolStripDropDownItem tsddi)
        {
            ((ToolStripDropDownMenu)tsddi.DropDown).ShowImageMargin = false;
        }

        public static void DisableMenuCloseOnClick(this ToolStripDropDownItem tsddi)
        {
            tsddi.DropDown.Closing -= DisableMenuCloseOnClick_DropDown_Closing;
            tsddi.DropDown.Closing += DisableMenuCloseOnClick_DropDown_Closing;
        }

        private static void DisableMenuCloseOnClick_DropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            e.Cancel = e.CloseReason == ToolStripDropDownCloseReason.ItemClicked;
        }

        public static void SetValue(this NumericUpDown nud, decimal number)
        {
            nud.Value = number.Clamp(nud.Minimum, nud.Maximum);
        }

        public static void SetValue(this TrackBar tb, int number)
        {
            tb.Value = number.Clamp(tb.Minimum, tb.Maximum);
        }

        public static bool IsValidImage(this PictureBox pb)
        {
            return pb.Image != null && pb.Image != pb.InitialImage && pb.Image != pb.ErrorImage;
        }

        public static void IgnoreSeparatorClick(this ContextMenuStrip cms)
        {
            bool cancelClose = false;

            cms.ItemClicked += (sender, e) =>
            {
                cancelClose = e.ClickedItem is ToolStripSeparator;
            };

            cms.Closing += (sender, e) =>
            {
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked && cancelClose)
                {
                    e.Cancel = true;
                }
            };
        }

        public static void RefreshItems(this ComboBox cb)
        {
            typeof(ComboBox).InvokeMember("RefreshItems", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, cb, new object[] { });
        }

        public static void AutoSizeDropDown(this ComboBox cb)
        {
            int maxWidth = 0;
            int verticalScrollBarWidth = cb.Items.Count > cb.MaxDropDownItems ? SystemInformation.VerticalScrollBarWidth : 0;

            foreach (object item in cb.Items)
            {
                int tempWidth = TextRenderer.MeasureText(cb.GetItemText(item), cb.Font).Width + verticalScrollBarWidth;

                if (tempWidth > maxWidth)
                {
                    maxWidth = tempWidth;
                }
            }

            cb.DropDownWidth = maxWidth;
        }

        public static void RefreshItem(this ListBox lb, int index)
        {
            typeof(ListBox).InvokeMember("RefreshItem", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, lb, new object[] { index });
        }

        public static void RefreshSelectedItem(this ListBox lb)
        {
            if (lb.SelectedIndex > -1)
            {
                lb.RefreshItem(lb.SelectedIndex);
            }
        }

        public static void RefreshItems(this ListBox lb)
        {
            typeof(ListBox).InvokeMember("RefreshItems", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, lb, new object[] { });
        }

        public static void DoubleBuffered(this DataGridView dgv, bool value)
        {
            PropertyInfo pi = dgv.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, value, null);
        }

        public static IEnumerable<TreeNode> All(this TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                yield return node;

                foreach (TreeNode child in node.Nodes.All())
                {
                    yield return child;
                }
            }
        }

        public static void SelectTabWithoutFocus(this TabControl tabControl, TabPage tabPage)
        {
            tabControl.Enabled = false;
            tabControl.SelectedTab = tabPage;
            tabControl.Enabled = true;
        }
    }
}
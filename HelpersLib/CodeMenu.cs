using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HelpersLib
{
    public static class CodeMenu
    {
        public static ContextMenuStrip Create<TEntry>(TextBox tb, params TEntry[] ignoreList)
            where TEntry : CodeMenuEntry
        {
            ContextMenuStrip cms = new ContextMenuStrip {
                Font = new Font("Lucida Console", 8),
                AutoClose = false,
                Opacity = 0.9,
                ShowImageMargin = false
            };

            var variables = Helpers.GetValueFields<TEntry>().Where(x => !ignoreList.Contains(x)).
                Select(x => new {
                    Name = x.ToPrefixString(),
                    Description = x.Description,
                });

            foreach (var variable in variables) {
                ToolStripMenuItem tsmi = new ToolStripMenuItem { Text = string.Format("{0} - {1}", variable.Name, variable.Description), Tag = variable.Name };
                tsmi.Click += (sender, e) =>
                {
                    string text = ((ToolStripMenuItem)sender).Tag.ToString();
                    tb.AppendTextToSelection(text);
                };
                cms.Items.Add(tsmi);
            }

            cms.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsmiClose = new ToolStripMenuItem("Close");
            tsmiClose.Click += (sender, e) => cms.Close();
            cms.Items.Add(tsmiClose);

            tb.MouseDown += (sender, e) =>
            {
                if (cms.Items.Count > 0) cms.Show(tb, new Point(tb.Width + 1, 0));
            };

            tb.Leave += (sender, e) =>
            {
                if (cms.Visible) cms.Close();
            };

            tb.KeyDown += (sender, e) =>
            {
                if ((e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape) && cms.Visible) {
                    cms.Close();
                    e.SuppressKeyPress = true;
                }
            };

            tb.Disposed += (sender, e) => cms.Dispose();

            return cms;
        }
    }
}

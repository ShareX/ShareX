#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HelpersLib
{
    public enum ReplacementVariables
    {
        [Description("Title of active window")]
        t,
        [Description("Process name of active window")]
        pn,
        [Description("Current year")]
        y,
        [Description("Current month")]
        mo,
        [Description("Current month name (Local language)")]
        mon,
        [Description("Current month name (English)")]
        mon2,
        [Description("Current day")]
        d,
        [Description("Current hour")]
        h,
        [Description("Current minute")]
        mi,
        [Description("Current second")]
        s,
        [Description("Current millisecond")]
        ms,
        [Description("Gets AM/PM")]
        pm,
        [Description("Current week name (Local language)")]
        w,
        [Description("Current week name (English)")]
        w2,
        [Description("Unix timestamp")]
        unix,
        [Description("Auto increment number")]
        i,
        [Description("Random number 0 to 9")]
        rn,
        [Description("Random alphanumeric char")]
        ra,
        [Description("Gets image width")]
        width,
        [Description("Gets image height")]
        height,
        [Description("User name")]
        un,
        [Description("User login name")]
        uln,
        [Description("Computer name")]
        cn,
        [Description("New line")]
        n
    }

    public static class ReplacementExtension
    {
        public const char Prefix = '%';

        public static string ToPrefixString(this ReplacementVariables replacement)
        {
            return Prefix + replacement.ToString();
        }
    }

    public enum NameParserType
    {
        Text, // Allows new line
        FileName,
        FolderPath,
        FilePath,
        URL
    }

    public class NameParser
    {
        public NameParserType Type { get; private set; }
        public int MaxNameLength { get; set; }
        public int MaxTitleLength { get; set; }
        public int AutoIncrementNumber { get; set; } // %i
        public Image Picture { get; set; } // %width, %height
        public string WindowText { get; set; } // %t
        public string ProcessName { get; set; } // %pn

        protected NameParser()
        {
        }

        public NameParser(NameParserType nameParserType)
        {
            Type = nameParserType;
        }

        public static string Parse(NameParserType nameParserType, string pattern)
        {
            return new NameParser(nameParserType).Parse(pattern);
        }

        public string Parse(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder(pattern);

            if (WindowText != null)
            {
                string windowText = WindowText.Replace(' ', '_');
                if (MaxTitleLength > 0 && windowText.Length > MaxTitleLength)
                {
                    windowText = windowText.Remove(MaxTitleLength);
                }
                sb.Replace(ReplacementVariables.t.ToPrefixString(), windowText);
            }

            if (ProcessName != null)
            {
                sb.Replace(ReplacementVariables.pn.ToPrefixString(), ProcessName);
            }

            string width = string.Empty, height = string.Empty;

            if (Picture != null)
            {
                width = Picture.Width.ToString();
                height = Picture.Height.ToString();
            }

            sb.Replace(ReplacementVariables.width.ToPrefixString(), width);
            sb.Replace(ReplacementVariables.height.ToPrefixString(), height);

            DateTime dt = DateTime.Now;

            sb.Replace(ReplacementVariables.mon2.ToPrefixString(), CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(dt.Month))
                .Replace(ReplacementVariables.mon.ToPrefixString(), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dt.Month))
                .Replace(ReplacementVariables.y.ToPrefixString(), dt.Year.ToString())
                .Replace(ReplacementVariables.mo.ToPrefixString(), Helpers.AddZeroes(dt.Month))
                .Replace(ReplacementVariables.d.ToPrefixString(), Helpers.AddZeroes(dt.Day));

            string hour;

            if (sb.ToString().Contains(ReplacementVariables.pm.ToPrefixString()))
            {
                hour = Helpers.HourTo12(dt.Hour);
            }
            else
            {
                hour = Helpers.AddZeroes(dt.Hour);
            }

            sb.Replace(ReplacementVariables.h.ToPrefixString(), hour)
                .Replace(ReplacementVariables.mi.ToPrefixString(), Helpers.AddZeroes(dt.Minute))
                .Replace(ReplacementVariables.s.ToPrefixString(), Helpers.AddZeroes(dt.Second))
                .Replace(ReplacementVariables.ms.ToPrefixString(), Helpers.AddZeroes(dt.Millisecond, 3))
                .Replace(ReplacementVariables.w2.ToPrefixString(), CultureInfo.InvariantCulture.DateTimeFormat.GetDayName(dt.DayOfWeek))
                .Replace(ReplacementVariables.w.ToPrefixString(), CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(dt.DayOfWeek))
                .Replace(ReplacementVariables.pm.ToPrefixString(), (dt.Hour >= 12 ? "PM" : "AM"));

            sb.Replace(ReplacementVariables.unix.ToPrefixString(), DateTime.UtcNow.ToUnix().ToString());

            if (sb.ToString().Contains(ReplacementVariables.i.ToPrefixString()))
            {
                AutoIncrementNumber++;
                sb.Replace(ReplacementVariables.i.ToPrefixString(), AutoIncrementNumber.ToString());
            }

            sb.Replace(ReplacementVariables.un.ToPrefixString(), Environment.UserName);
            sb.Replace(ReplacementVariables.uln.ToPrefixString(), Environment.UserDomainName);
            sb.Replace(ReplacementVariables.cn.ToPrefixString(), Environment.MachineName);

            if (Type == NameParserType.Text)
            {
                sb.Replace(ReplacementVariables.n.ToPrefixString(), Environment.NewLine);
            }

            string result = sb.ToString();

            result = result.ReplaceAll(ReplacementVariables.rn.ToPrefixString(), () => Helpers.GetRandomChar(Helpers.Numbers).ToString());
            result = result.ReplaceAll(ReplacementVariables.ra.ToPrefixString(), () => Helpers.GetRandomChar(Helpers.Alphanumeric).ToString());

            if (Type == NameParserType.FolderPath)
            {
                result = Helpers.GetValidFolderPath(result);
            }
            else if (Type == NameParserType.FileName)
            {
                result = Helpers.GetValidFileName(result);
            }
            else if (Type == NameParserType.FilePath)
            {
                result = Helpers.GetValidFilePath(result);
            }
            else if (Type == NameParserType.URL)
            {
                result = Helpers.GetValidURL(result);
            }

            if (MaxNameLength > 0 && result.Length > MaxNameLength)
            {
                result = result.Remove(MaxNameLength);
            }

            return result;
        }

        public static ContextMenuStrip CreateCodesMenu(TextBox tb, params ReplacementVariables[] ignoreList)
        {
            ContextMenuStrip cms = new ContextMenuStrip
            {
                Font = new Font("Lucida Console", 8),
                AutoClose = false,
                Opacity = 0.9,
                ShowImageMargin = false
            };

            var variables = Helpers.GetEnums<ReplacementVariables>().Where(x => !ignoreList.Contains(x)).
                Select(x => new
                {
                    Name = ReplacementExtension.Prefix + Enum.GetName(typeof(ReplacementVariables), x),
                    Description = x.GetDescription(),
                    Enum = x
                });

            foreach (var variable in variables)
            {
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
                if ((e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape) && cms.Visible)
                {
                    cms.Close();
                    e.SuppressKeyPress = true;
                }
            };

            tb.Disposed += (sender, e) => cms.Dispose();

            return cms;
        }
    }
}
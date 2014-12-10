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

using HelpersLib.Properties;
using System;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace HelpersLib
{
    public class ReplCodeMenuEntry : CodeMenuEntry
    {
        public ReplCodeMenuEntry(string value, string description)
            : base(value, description)
        {
        }

        public override String ToPrefixString()
        {
            return '%' + _value;
        }

        public static readonly ReplCodeMenuEntry t = new ReplCodeMenuEntry("t", Resources.ReplCodeMenuEntry_t_Title_of_active_window);
        public static readonly ReplCodeMenuEntry pn = new ReplCodeMenuEntry("pn", Resources.ReplCodeMenuEntry_pn_Process_name_of_active_window);
        public static readonly ReplCodeMenuEntry y = new ReplCodeMenuEntry("y", Resources.ReplCodeMenuEntry_y_Current_year);
        public static readonly ReplCodeMenuEntry yy = new ReplCodeMenuEntry("yy", Resources.ReplCodeMenuEntry_yy_Current_year__2_digits_);
        public static readonly ReplCodeMenuEntry mo = new ReplCodeMenuEntry("mo", Resources.ReplCodeMenuEntry_mo_Current_month);
        public static readonly ReplCodeMenuEntry mon = new ReplCodeMenuEntry("mon", Resources.ReplCodeMenuEntry_mon_Current_month_name__Local_language_);
        public static readonly ReplCodeMenuEntry mon2 = new ReplCodeMenuEntry("mon2", Resources.ReplCodeMenuEntry_mon2_Current_month_name__English_);
        public static readonly ReplCodeMenuEntry d = new ReplCodeMenuEntry("d", Resources.ReplCodeMenuEntry_d_Current_day);
        public static readonly ReplCodeMenuEntry h = new ReplCodeMenuEntry("h", Resources.ReplCodeMenuEntry_h_Current_hour);
        public static readonly ReplCodeMenuEntry mi = new ReplCodeMenuEntry("mi", Resources.ReplCodeMenuEntry_mi_Current_minute);
        public static readonly ReplCodeMenuEntry s = new ReplCodeMenuEntry("s", Resources.ReplCodeMenuEntry_s_Current_second);
        public static readonly ReplCodeMenuEntry ms = new ReplCodeMenuEntry("ms", Resources.ReplCodeMenuEntry_ms_Current_millisecond);
        public static readonly ReplCodeMenuEntry pm = new ReplCodeMenuEntry("pm", Resources.ReplCodeMenuEntry_pm_Gets_AM_PM);
        public static readonly ReplCodeMenuEntry w = new ReplCodeMenuEntry("w", Resources.ReplCodeMenuEntry_w_Current_week_name__Local_language_);
        public static readonly ReplCodeMenuEntry w2 = new ReplCodeMenuEntry("w2", Resources.ReplCodeMenuEntry_w2_Current_week_name__English_);
        public static readonly ReplCodeMenuEntry unix = new ReplCodeMenuEntry("unix", Resources.ReplCodeMenuEntry_unix_Unix_timestamp);
        public static readonly ReplCodeMenuEntry i = new ReplCodeMenuEntry("i", Resources.ReplCodeMenuEntry_i_Auto_increment_number);
        public static readonly ReplCodeMenuEntry rn = new ReplCodeMenuEntry("rn", Resources.ReplCodeMenuEntry_rn_Random_number_0_to_9);
        public static readonly ReplCodeMenuEntry ra = new ReplCodeMenuEntry("ra", Resources.ReplCodeMenuEntry_ra_Random_alphanumeric_char);
        public static readonly ReplCodeMenuEntry width = new ReplCodeMenuEntry("width", Resources.ReplCodeMenuEntry_width_Gets_image_width);
        public static readonly ReplCodeMenuEntry height = new ReplCodeMenuEntry("height", Resources.ReplCodeMenuEntry_height_Gets_image_height);
        public static readonly ReplCodeMenuEntry un = new ReplCodeMenuEntry("un", Resources.ReplCodeMenuEntry_un_User_name);
        public static readonly ReplCodeMenuEntry uln = new ReplCodeMenuEntry("uln", Resources.ReplCodeMenuEntry_uln_User_login_name);
        public static readonly ReplCodeMenuEntry cn = new ReplCodeMenuEntry("cn", Resources.ReplCodeMenuEntry_cn_Computer_name);
        public static readonly ReplCodeMenuEntry n = new ReplCodeMenuEntry("n", Resources.ReplCodeMenuEntry_n_New_line);
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
                sb.Replace(ReplCodeMenuEntry.t.ToPrefixString(), windowText);
            }

            if (ProcessName != null)
            {
                sb.Replace(ReplCodeMenuEntry.pn.ToPrefixString(), ProcessName);
            }

            string width = string.Empty, height = string.Empty;

            if (Picture != null)
            {
                width = Picture.Width.ToString();
                height = Picture.Height.ToString();
            }

            sb.Replace(ReplCodeMenuEntry.width.ToPrefixString(), width);
            sb.Replace(ReplCodeMenuEntry.height.ToPrefixString(), height);

            DateTime dt = DateTime.Now;

            sb.Replace(ReplCodeMenuEntry.mon2.ToPrefixString(), CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(dt.Month))
                .Replace(ReplCodeMenuEntry.mon.ToPrefixString(), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dt.Month))
                .Replace(ReplCodeMenuEntry.yy.ToPrefixString(), dt.ToString("yy"))
                .Replace(ReplCodeMenuEntry.y.ToPrefixString(), dt.Year.ToString())
                .Replace(ReplCodeMenuEntry.mo.ToPrefixString(), Helpers.AddZeroes(dt.Month))
                .Replace(ReplCodeMenuEntry.d.ToPrefixString(), Helpers.AddZeroes(dt.Day));

            string hour;

            if (sb.ToString().Contains(ReplCodeMenuEntry.pm.ToPrefixString()))
            {
                hour = Helpers.HourTo12(dt.Hour);
            }
            else
            {
                hour = Helpers.AddZeroes(dt.Hour);
            }

            sb.Replace(ReplCodeMenuEntry.h.ToPrefixString(), hour)
                .Replace(ReplCodeMenuEntry.mi.ToPrefixString(), Helpers.AddZeroes(dt.Minute))
                .Replace(ReplCodeMenuEntry.s.ToPrefixString(), Helpers.AddZeroes(dt.Second))
                .Replace(ReplCodeMenuEntry.ms.ToPrefixString(), Helpers.AddZeroes(dt.Millisecond, 3))
                .Replace(ReplCodeMenuEntry.w2.ToPrefixString(), CultureInfo.InvariantCulture.DateTimeFormat.GetDayName(dt.DayOfWeek))
                .Replace(ReplCodeMenuEntry.w.ToPrefixString(), CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(dt.DayOfWeek))
                .Replace(ReplCodeMenuEntry.pm.ToPrefixString(), (dt.Hour >= 12 ? "PM" : "AM"));

            sb.Replace(ReplCodeMenuEntry.unix.ToPrefixString(), DateTime.UtcNow.ToUnix().ToString());

            if (sb.ToString().Contains(ReplCodeMenuEntry.i.ToPrefixString()))
            {
                AutoIncrementNumber++;
                sb.Replace(ReplCodeMenuEntry.i.ToPrefixString(), AutoIncrementNumber.ToString());
            }

            sb.Replace(ReplCodeMenuEntry.un.ToPrefixString(), Environment.UserName);
            sb.Replace(ReplCodeMenuEntry.uln.ToPrefixString(), Environment.UserDomainName);
            sb.Replace(ReplCodeMenuEntry.cn.ToPrefixString(), Environment.MachineName);

            if (Type == NameParserType.Text)
            {
                sb.Replace(ReplCodeMenuEntry.n.ToPrefixString(), Environment.NewLine);
            }

            string result = sb.ToString();

            result = result.ReplaceAll(ReplCodeMenuEntry.rn.ToPrefixString(), () => Helpers.GetRandomChar(Helpers.Numbers).ToString());
            result = result.ReplaceAll(ReplCodeMenuEntry.ra.ToPrefixString(), () => Helpers.GetRandomChar(Helpers.Alphanumeric).ToString());

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
    }
}
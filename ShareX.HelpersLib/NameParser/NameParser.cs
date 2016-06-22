#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using System.Globalization;
using System.Text;

namespace ShareX.HelpersLib
{
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
        public int AutoIncrementNumber { get; set; } // %i, %ia, %ib, %iAa, %ix
        public int ImageWidth { get; set; } // %width
        public int ImageHeight { get; set; } // %height
        public string WindowText { get; set; } // %t
        public string ProcessName { get; set; } // %pn
        public TimeZoneInfo CustomTimeZone { get; set; }

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
                return "";
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

            string width = "", height = "";

            if (ImageWidth > 0)
            {
                width = ImageWidth.ToString();
            }

            if (ImageHeight > 0)
            {
                height = ImageHeight.ToString();
            }

            sb.Replace(ReplCodeMenuEntry.width.ToPrefixString(), width);
            sb.Replace(ReplCodeMenuEntry.height.ToPrefixString(), height);

            DateTime dt = DateTime.Now;

            if (CustomTimeZone != null)
            {
                dt = TimeZoneInfo.ConvertTime(dt, CustomTimeZone);
            }

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
                .Replace(ReplCodeMenuEntry.pm.ToPrefixString(), dt.Hour >= 12 ? "PM" : "AM");

            sb.Replace(ReplCodeMenuEntry.unix.ToPrefixString(), DateTime.UtcNow.ToUnix().ToString());

            if (sb.ToString().Contains(ReplCodeMenuEntry.i.ToPrefixString())
                || sb.ToString().Contains(ReplCodeMenuEntry.ib.ToPrefixString())
                || sb.ToString().Contains(ReplCodeMenuEntry.ib.ToPrefixString().Replace('b', 'B'))
                || sb.ToString().Contains(ReplCodeMenuEntry.iAa.ToPrefixString())
                || sb.ToString().Contains(ReplCodeMenuEntry.iAa.ToPrefixString().Replace("Aa", "aA"))
                || sb.ToString().Contains(ReplCodeMenuEntry.ia.ToPrefixString())
                || sb.ToString().Contains(ReplCodeMenuEntry.ia.ToPrefixString().Replace('a', 'A'))
                || sb.ToString().Contains(ReplCodeMenuEntry.ix.ToPrefixString())
                || sb.ToString().Contains(ReplCodeMenuEntry.ix.ToPrefixString().Replace('x', 'X')))
            {
                AutoIncrementNumber++;

                // Base
                try
                {
                    foreach (Tuple<string, int[]> entry in ListEntryWithValues(sb.ToString(), ReplCodeMenuEntry.ib.ToPrefixString(), 2))
                    {
                        sb.Replace(entry.Item1, Helpers.AddZeroes(AutoIncrementNumber.ToBase(entry.Item2[0], Helpers.AlphanumericInverse), entry.Item2[1]));
                    }
                    foreach (Tuple<string, int[]> entry in ListEntryWithValues(sb.ToString(), ReplCodeMenuEntry.ib.ToPrefixString().Replace('b', 'B'), 2))
                    {
                        sb.Replace(entry.Item1, Helpers.AddZeroes(AutoIncrementNumber.ToBase(entry.Item2[0], Helpers.Alphanumeric), entry.Item2[1]));
                    }
                }
                catch
                {
                }

                // Alphanumeric Dual Case (Base 62)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), ReplCodeMenuEntry.iAa.ToPrefixString()))
                {
                    sb.Replace(entry.Item1, Helpers.AddZeroes(AutoIncrementNumber.ToBase(62, Helpers.Alphanumeric), entry.Item2));
                }
                sb.Replace(ReplCodeMenuEntry.iAa.ToPrefixString(), AutoIncrementNumber.ToBase(62, Helpers.Alphanumeric));

                // Alphanumeric Dual Case (Base 62)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), ReplCodeMenuEntry.iAa.ToPrefixString().Replace("Aa", "aA")))
                {
                    sb.Replace(entry.Item1, Helpers.AddZeroes(AutoIncrementNumber.ToBase(62, Helpers.AlphanumericInverse), entry.Item2));
                }
                sb.Replace(ReplCodeMenuEntry.iAa.ToPrefixString().Replace("Aa", "aA"), AutoIncrementNumber.ToBase(62, Helpers.AlphanumericInverse));

                // Alphanumeric Single Case (Base 36)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), ReplCodeMenuEntry.ia.ToPrefixString()))
                {
                    sb.Replace(entry.Item1, Helpers.AddZeroes(AutoIncrementNumber.ToBase(36, Helpers.Alphanumeric), entry.Item2).ToLowerInvariant());
                }
                sb.Replace(ReplCodeMenuEntry.ia.ToPrefixString(), AutoIncrementNumber.ToBase(36, Helpers.Alphanumeric).ToLowerInvariant());

                // Alphanumeric Single Case Capital (Base 36)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), ReplCodeMenuEntry.ia.ToPrefixString().Replace('a', 'A')))
                {
                    sb.Replace(entry.Item1, Helpers.AddZeroes(AutoIncrementNumber.ToBase(36, Helpers.Alphanumeric), entry.Item2).ToUpperInvariant());
                }
                sb.Replace(ReplCodeMenuEntry.ia.ToPrefixString().Replace('a', 'A'), AutoIncrementNumber.ToBase(36, Helpers.Alphanumeric).ToUpperInvariant());

                // Hexadecimal (Base 16)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), ReplCodeMenuEntry.ix.ToPrefixString()))
                {
                    sb.Replace(entry.Item1, AutoIncrementNumber.ToString("x" + entry.Item2.ToString()));
                }
                sb.Replace(ReplCodeMenuEntry.ix.ToPrefixString(), AutoIncrementNumber.ToString("x"));

                // Hexadecimal Capital (Base 16)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), ReplCodeMenuEntry.ix.ToPrefixString().Replace('x', 'X')))
                {
                    sb.Replace(entry.Item1, AutoIncrementNumber.ToString("X" + entry.Item2.ToString()));
                }
                sb.Replace(ReplCodeMenuEntry.ix.ToPrefixString().Replace('x', 'X'), AutoIncrementNumber.ToString("X"));

                // Number (Base 10)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), ReplCodeMenuEntry.i.ToPrefixString()))
                {
                    sb.Replace(entry.Item1, AutoIncrementNumber.ToString("d" + entry.Item2.ToString()));
                }
                sb.Replace(ReplCodeMenuEntry.i.ToPrefixString(), AutoIncrementNumber.ToString("d"));
            }

            sb.Replace(ReplCodeMenuEntry.un.ToPrefixString(), Environment.UserName);
            sb.Replace(ReplCodeMenuEntry.uln.ToPrefixString(), Environment.UserDomainName);
            sb.Replace(ReplCodeMenuEntry.cn.ToPrefixString(), Environment.MachineName);

            if (Type == NameParserType.Text)
            {
                sb.Replace(ReplCodeMenuEntry.n.ToPrefixString(), Environment.NewLine);
            }

            string result = sb.ToString();

            foreach (Tuple<string, int> entry in ListEntryWithValue(result, ReplCodeMenuEntry.rn.ToPrefixString()))
            {
                result = result.ReplaceAll(entry.Item1, () => Helpers.RepeatGenerator(entry.Item2, () => Helpers.GetRandomChar(Helpers.Numbers).ToString()));
            }
            foreach (Tuple<string, int> entry in ListEntryWithValue(result, ReplCodeMenuEntry.ra.ToPrefixString()))
            {
                result = result.ReplaceAll(entry.Item1, () => Helpers.RepeatGenerator(entry.Item2, () => Helpers.GetRandomChar(Helpers.Alphanumeric).ToString()));
            }
            foreach (Tuple<string, int> entry in ListEntryWithValue(result, ReplCodeMenuEntry.rx.ToPrefixString()))
            {
                result = result.ReplaceAll(entry.Item1, () => Helpers.RepeatGenerator(entry.Item2, () => Helpers.GetRandomChar(Helpers.Hexadecimal.ToLowerInvariant()).ToString()));
            }
            foreach (Tuple<string, int> entry in ListEntryWithValue(result, ReplCodeMenuEntry.rx.ToPrefixString().Replace('x', 'X')))
            {
                result = result.ReplaceAll(entry.Item1, () => Helpers.RepeatGenerator(entry.Item2, () => Helpers.GetRandomChar(Helpers.Hexadecimal.ToUpperInvariant()).ToString()));
            }

            result = result.ReplaceAll(ReplCodeMenuEntry.rn.ToPrefixString(), () => Helpers.GetRandomChar(Helpers.Numbers).ToString());
            result = result.ReplaceAll(ReplCodeMenuEntry.ra.ToPrefixString(), () => Helpers.GetRandomChar(Helpers.Alphanumeric).ToString());
            result = result.ReplaceAll(ReplCodeMenuEntry.rx.ToPrefixString(), () => Helpers.GetRandomChar(Helpers.Hexadecimal.ToLowerInvariant()).ToString());
            result = result.ReplaceAll(ReplCodeMenuEntry.rx.ToPrefixString().Replace('x', 'X'), () => Helpers.GetRandomChar(Helpers.Hexadecimal.ToUpperInvariant()).ToString());

            result = result.ReplaceAll(ReplCodeMenuEntry.guid.ToPrefixString().ToLowerInvariant(), () => Guid.NewGuid().ToString().ToLowerInvariant());
            result = result.ReplaceAll(ReplCodeMenuEntry.guid.ToPrefixString().ToUpperInvariant(), () => Guid.NewGuid().ToString().ToUpperInvariant());

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

        private IEnumerable<Tuple<string, string[]>> ListEntryWithArguments(string text, string entry, int elements)
        {
            foreach (Tuple<string, string> o in text.ForEachBetween(entry + "{", "}"))
            {
                string[] s = o.Item2.Split(',');
                if (elements > s.Length)
                {
                    Array.Resize(ref s, elements);
                }
                yield return new Tuple<string, string[]>(o.Item1, s);
            }
        }

        private IEnumerable<Tuple<string, int[]>> ListEntryWithValues(string text, string entry, int elements)
        {
            foreach (Tuple<string, string[]> o in ListEntryWithArguments(text, entry, elements))
            {
                int[] a = new int[o.Item2.Length];
                for (int i = o.Item2.Length - 1; i >= 0; --i)
                {
                    int n = 0;
                    if (int.TryParse(o.Item2[i], out n))
                    {
                        a[i] = n;
                    }
                }
                yield return new Tuple<string, int[]>(o.Item1, a);
            }
        }

        private IEnumerable<Tuple<string, int>> ListEntryWithValue(string text, string entry)
        {
            foreach (Tuple<string, int[]> o in ListEntryWithValues(text, entry, 1))
            {
                yield return new Tuple<string, int>(o.Item1, o.Item2[0]);
            }
        }
    }
}
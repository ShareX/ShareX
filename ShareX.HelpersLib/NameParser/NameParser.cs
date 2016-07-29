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

using System;
using System.Collections.Generic;
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
                sb.Replace(CodeMenuEntryFilename.t.ToPrefixString(), windowText);
            }

            if (ProcessName != null)
            {
                sb.Replace(CodeMenuEntryFilename.pn.ToPrefixString(), ProcessName);
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

            sb.Replace(CodeMenuEntryFilename.width.ToPrefixString(), width);
            sb.Replace(CodeMenuEntryFilename.height.ToPrefixString(), height);

            DateTime dt = DateTime.Now;

            if (CustomTimeZone != null)
            {
                dt = TimeZoneInfo.ConvertTime(dt, CustomTimeZone);
            }

            sb.Replace(CodeMenuEntryFilename.mon2.ToPrefixString(), CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(dt.Month))
                .Replace(CodeMenuEntryFilename.mon.ToPrefixString(), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dt.Month))
                .Replace(CodeMenuEntryFilename.yy.ToPrefixString(), dt.ToString("yy"))
                .Replace(CodeMenuEntryFilename.y.ToPrefixString(), dt.Year.ToString())
                .Replace(CodeMenuEntryFilename.mo.ToPrefixString(), Helpers.AddZeroes(dt.Month))
                .Replace(CodeMenuEntryFilename.d.ToPrefixString(), Helpers.AddZeroes(dt.Day));

            string hour;

            if (sb.ToString().Contains(CodeMenuEntryFilename.pm.ToPrefixString()))
            {
                hour = Helpers.HourTo12(dt.Hour);
            }
            else
            {
                hour = Helpers.AddZeroes(dt.Hour);
            }

            sb.Replace(CodeMenuEntryFilename.h.ToPrefixString(), hour)
                .Replace(CodeMenuEntryFilename.mi.ToPrefixString(), Helpers.AddZeroes(dt.Minute))
                .Replace(CodeMenuEntryFilename.s.ToPrefixString(), Helpers.AddZeroes(dt.Second))
                .Replace(CodeMenuEntryFilename.ms.ToPrefixString(), Helpers.AddZeroes(dt.Millisecond, 3))
                .Replace(CodeMenuEntryFilename.w2.ToPrefixString(), CultureInfo.InvariantCulture.DateTimeFormat.GetDayName(dt.DayOfWeek))
                .Replace(CodeMenuEntryFilename.w.ToPrefixString(), CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(dt.DayOfWeek))
                .Replace(CodeMenuEntryFilename.pm.ToPrefixString(), dt.Hour >= 12 ? "PM" : "AM");

            sb.Replace(CodeMenuEntryFilename.unix.ToPrefixString(), DateTime.UtcNow.ToUnix().ToString());

            if (sb.ToString().Contains(CodeMenuEntryFilename.i.ToPrefixString())
                || sb.ToString().Contains(CodeMenuEntryFilename.ib.ToPrefixString())
                || sb.ToString().Contains(CodeMenuEntryFilename.ib.ToPrefixString().Replace('b', 'B'))
                || sb.ToString().Contains(CodeMenuEntryFilename.iAa.ToPrefixString())
                || sb.ToString().Contains(CodeMenuEntryFilename.iAa.ToPrefixString().Replace("Aa", "aA"))
                || sb.ToString().Contains(CodeMenuEntryFilename.ia.ToPrefixString())
                || sb.ToString().Contains(CodeMenuEntryFilename.ia.ToPrefixString().Replace('a', 'A'))
                || sb.ToString().Contains(CodeMenuEntryFilename.ix.ToPrefixString())
                || sb.ToString().Contains(CodeMenuEntryFilename.ix.ToPrefixString().Replace('x', 'X')))
            {
                AutoIncrementNumber++;

                // Base
                try
                {
                    foreach (Tuple<string, int[]> entry in ListEntryWithValues(sb.ToString(), CodeMenuEntryFilename.ib.ToPrefixString(), 2))
                    {
                        sb.Replace(entry.Item1, Helpers.AddZeroes(AutoIncrementNumber.ToBase(entry.Item2[0], Helpers.AlphanumericInverse), entry.Item2[1]));
                    }
                    foreach (Tuple<string, int[]> entry in ListEntryWithValues(sb.ToString(), CodeMenuEntryFilename.ib.ToPrefixString().Replace('b', 'B'), 2))
                    {
                        sb.Replace(entry.Item1, Helpers.AddZeroes(AutoIncrementNumber.ToBase(entry.Item2[0], Helpers.Alphanumeric), entry.Item2[1]));
                    }
                }
                catch
                {
                }

                // Alphanumeric Dual Case (Base 62)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), CodeMenuEntryFilename.iAa.ToPrefixString()))
                {
                    sb.Replace(entry.Item1, Helpers.AddZeroes(AutoIncrementNumber.ToBase(62, Helpers.Alphanumeric), entry.Item2));
                }
                sb.Replace(CodeMenuEntryFilename.iAa.ToPrefixString(), AutoIncrementNumber.ToBase(62, Helpers.Alphanumeric));

                // Alphanumeric Dual Case (Base 62)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), CodeMenuEntryFilename.iAa.ToPrefixString().Replace("Aa", "aA")))
                {
                    sb.Replace(entry.Item1, Helpers.AddZeroes(AutoIncrementNumber.ToBase(62, Helpers.AlphanumericInverse), entry.Item2));
                }
                sb.Replace(CodeMenuEntryFilename.iAa.ToPrefixString().Replace("Aa", "aA"), AutoIncrementNumber.ToBase(62, Helpers.AlphanumericInverse));

                // Alphanumeric Single Case (Base 36)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), CodeMenuEntryFilename.ia.ToPrefixString()))
                {
                    sb.Replace(entry.Item1, Helpers.AddZeroes(AutoIncrementNumber.ToBase(36, Helpers.Alphanumeric), entry.Item2).ToLowerInvariant());
                }
                sb.Replace(CodeMenuEntryFilename.ia.ToPrefixString(), AutoIncrementNumber.ToBase(36, Helpers.Alphanumeric).ToLowerInvariant());

                // Alphanumeric Single Case Capital (Base 36)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), CodeMenuEntryFilename.ia.ToPrefixString().Replace('a', 'A')))
                {
                    sb.Replace(entry.Item1, Helpers.AddZeroes(AutoIncrementNumber.ToBase(36, Helpers.Alphanumeric), entry.Item2).ToUpperInvariant());
                }
                sb.Replace(CodeMenuEntryFilename.ia.ToPrefixString().Replace('a', 'A'), AutoIncrementNumber.ToBase(36, Helpers.Alphanumeric).ToUpperInvariant());

                // Hexadecimal (Base 16)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), CodeMenuEntryFilename.ix.ToPrefixString()))
                {
                    sb.Replace(entry.Item1, AutoIncrementNumber.ToString("x" + entry.Item2.ToString()));
                }
                sb.Replace(CodeMenuEntryFilename.ix.ToPrefixString(), AutoIncrementNumber.ToString("x"));

                // Hexadecimal Capital (Base 16)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), CodeMenuEntryFilename.ix.ToPrefixString().Replace('x', 'X')))
                {
                    sb.Replace(entry.Item1, AutoIncrementNumber.ToString("X" + entry.Item2.ToString()));
                }
                sb.Replace(CodeMenuEntryFilename.ix.ToPrefixString().Replace('x', 'X'), AutoIncrementNumber.ToString("X"));

                // Number (Base 10)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), CodeMenuEntryFilename.i.ToPrefixString()))
                {
                    sb.Replace(entry.Item1, AutoIncrementNumber.ToString("d" + entry.Item2.ToString()));
                }
                sb.Replace(CodeMenuEntryFilename.i.ToPrefixString(), AutoIncrementNumber.ToString("d"));
            }

            sb.Replace(CodeMenuEntryFilename.un.ToPrefixString(), Environment.UserName);
            sb.Replace(CodeMenuEntryFilename.uln.ToPrefixString(), Environment.UserDomainName);
            sb.Replace(CodeMenuEntryFilename.cn.ToPrefixString(), Environment.MachineName);

            if (Type == NameParserType.Text)
            {
                sb.Replace(CodeMenuEntryFilename.n.ToPrefixString(), Environment.NewLine);
            }

            string result = sb.ToString();

            foreach (Tuple<string, int> entry in ListEntryWithValue(result, CodeMenuEntryFilename.rn.ToPrefixString()))
            {
                result = result.ReplaceAll(entry.Item1, () => Helpers.RepeatGenerator(entry.Item2, () => Helpers.GetRandomChar(Helpers.Numbers).ToString()));
            }
            foreach (Tuple<string, int> entry in ListEntryWithValue(result, CodeMenuEntryFilename.ra.ToPrefixString()))
            {
                result = result.ReplaceAll(entry.Item1, () => Helpers.RepeatGenerator(entry.Item2, () => Helpers.GetRandomChar(Helpers.Alphanumeric).ToString()));
            }
            foreach (Tuple<string, int> entry in ListEntryWithValue(result, CodeMenuEntryFilename.rx.ToPrefixString()))
            {
                result = result.ReplaceAll(entry.Item1, () => Helpers.RepeatGenerator(entry.Item2, () => Helpers.GetRandomChar(Helpers.Hexadecimal.ToLowerInvariant()).ToString()));
            }
            foreach (Tuple<string, int> entry in ListEntryWithValue(result, CodeMenuEntryFilename.rx.ToPrefixString().Replace('x', 'X')))
            {
                result = result.ReplaceAll(entry.Item1, () => Helpers.RepeatGenerator(entry.Item2, () => Helpers.GetRandomChar(Helpers.Hexadecimal.ToUpperInvariant()).ToString()));
            }

            result = result.ReplaceAll(CodeMenuEntryFilename.rn.ToPrefixString(), () => Helpers.GetRandomChar(Helpers.Numbers).ToString());
            result = result.ReplaceAll(CodeMenuEntryFilename.ra.ToPrefixString(), () => Helpers.GetRandomChar(Helpers.Alphanumeric).ToString());
            result = result.ReplaceAll(CodeMenuEntryFilename.rx.ToPrefixString(), () => Helpers.GetRandomChar(Helpers.Hexadecimal.ToLowerInvariant()).ToString());
            result = result.ReplaceAll(CodeMenuEntryFilename.rx.ToPrefixString().Replace('x', 'X'), () => Helpers.GetRandomChar(Helpers.Hexadecimal.ToUpperInvariant()).ToString());

            result = result.ReplaceAll(CodeMenuEntryFilename.guid.ToPrefixString().ToLowerInvariant(), () => Guid.NewGuid().ToString().ToLowerInvariant());
            result = result.ReplaceAll(CodeMenuEntryFilename.guid.ToPrefixString().ToUpperInvariant(), () => Guid.NewGuid().ToString().ToUpperInvariant());

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
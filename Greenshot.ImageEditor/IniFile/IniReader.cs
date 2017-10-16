/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Greenshot.IniFile
{
    public static class IniReader
    {
        private const string SECTION_START = "[";
        private const string SECTION_END = "]";
        private const string COMMENT = ";";
        private static readonly char[] ASSIGNMENT = new[] { '=' };

        /**
         * Read an ini file to a Dictionary, each key is a section and the value is a Dictionary with name and values.
         */

        public static Dictionary<string, Dictionary<string, string>> read(string path, Encoding encoding)
        {
            Dictionary<string, Dictionary<string, string>> ini = new Dictionary<string, Dictionary<string, string>>();
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1024))
            {
                using (StreamReader reader = new StreamReader(fileStream, encoding))
                {
                    Dictionary<string, string> nameValues = new Dictionary<string, string>();
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line != null)
                        {
                            string cleanLine = line.Trim();
                            if (cleanLine.Length == 0 || cleanLine.StartsWith(COMMENT))
                            {
                                continue;
                            }
                            if (cleanLine.StartsWith(SECTION_START))
                            {
                                string section = line.Replace(SECTION_START, "").Replace(SECTION_END, "").Trim();
                                nameValues = new Dictionary<string, string>();
                                ini.Add(section, nameValues);
                            }
                            else
                            {
                                string[] keyvalueSplitter = line.Split(ASSIGNMENT, 2);
                                string name = keyvalueSplitter[0];
                                string inivalue = keyvalueSplitter.Length > 1 ? keyvalueSplitter[1] : null;
                                if (nameValues.ContainsKey(name))
                                {
                                    nameValues[name] = inivalue;
                                }
                                else
                                {
                                    nameValues.Add(name, inivalue);
                                }
                            }
                        }
                    }
                }
            }
            return ini;
        }
    }
}
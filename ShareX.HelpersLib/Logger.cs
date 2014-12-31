#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2015 ShareX Developers

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
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ShareX.HelpersLib
{
    public class Logger
    {
        public delegate void MessageAddedEventHandler(string message);

        public event MessageAddedEventHandler MessageAdded;

        private readonly object loggerLock = new object();
        private StringBuilder sbMessages = new StringBuilder(4096);
        private int lastSaveIndex;

        protected void OnMessageAdded(string message)
        {
            if (MessageAdded != null)
            {
                MessageAdded(message);
            }
        }

        public void WriteLine(string message = "")
        {
            lock (loggerLock)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    message = string.Format("{0:yyyy-MM-dd HH:mm:ss.fff} - {1}", FastDateTime.Now, message);
                }

                sbMessages.AppendLine(message);
                Debug.WriteLine(message);
                OnMessageAdded(message);
            }
        }

        public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        public void WriteException(string exception, string message = "Exception")
        {
            WriteLine("{0}:{1}{2}", message, Environment.NewLine, exception);
        }

        public void WriteException(Exception exception, string message = "Exception")
        {
            WriteException(exception.ToString(), message);
        }

        public void SaveLog(string filepath)
        {
            lock (loggerLock)
            {
                if (sbMessages != null && sbMessages.Length > 0 && !string.IsNullOrEmpty(filepath))
                {
                    string messages = sbMessages.ToString(lastSaveIndex, sbMessages.Length - lastSaveIndex);

                    if (!string.IsNullOrEmpty(messages))
                    {
                        Helpers.CreateDirectoryIfNotExist(filepath);
                        File.AppendAllText(filepath, messages, Encoding.UTF8);
                        lastSaveIndex = sbMessages.Length;
                    }
                }
            }
        }

        public void Clear()
        {
            lock (loggerLock)
            {
                sbMessages.Length = 0;
                lastSaveIndex = 0;
            }
        }

        public override string ToString()
        {
            lock (loggerLock)
            {
                if (sbMessages != null && sbMessages.Length > 0)
                {
                    return sbMessages.ToString();
                }

                return null;
            }
        }
    }
}
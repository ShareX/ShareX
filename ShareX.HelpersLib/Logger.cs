#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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

        public bool Async { get; set; } = true;
        public bool DebugWrite { get; set; } = true;
        public bool StoreInMemory { get; set; } = true;
        public bool FileWrite { get; set; } = false;
        public string LogFilePath { get; private set; }

        private readonly object loggerLock = new object();
        private StringBuilder sbMessages = new StringBuilder();

        public Logger()
        {
        }

        public Logger(string logFilePath)
        {
            FileWrite = true;
            LogFilePath = logFilePath;
            Helpers.CreateDirectoryFromFilePath(LogFilePath);
        }

        protected void OnMessageAdded(string message)
        {
            if (MessageAdded != null)
            {
                MessageAdded(message);
            }
        }

        public void WriteLine(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (Async)
                {
                    TaskEx.Run(() => WriteLineInternal(message));
                }
                else
                {
                    WriteLineInternal(message);
                }
            }
        }

        private void WriteLineInternal(string message)
        {
            lock (loggerLock)
            {
                message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}";

                if (DebugWrite)
                {
                    Debug.WriteLine(message);
                }

                if (StoreInMemory && sbMessages != null)
                {
                    sbMessages.AppendLine(message);
                }

                if (FileWrite && !string.IsNullOrEmpty(LogFilePath))
                {
                    try
                    {
                        File.AppendAllText(LogFilePath, message + Environment.NewLine, Encoding.UTF8);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }

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

        public void Clear()
        {
            lock (loggerLock)
            {
                if (sbMessages != null)
                {
                    sbMessages.Clear();
                }
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
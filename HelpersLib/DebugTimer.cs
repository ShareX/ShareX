#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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

namespace HelpersLib
{
    public class DebugTimer : IDisposable
    {
        private Stopwatch timer;
        private string text;

        public DebugTimer()
        {
            timer = Stopwatch.StartNew();
        }

        public DebugTimer(string text)
        {
            this.text = text;
            timer = Stopwatch.StartNew();
        }

        private void Write(string text, string timeText)
        {
            if (!string.IsNullOrEmpty(text))
            {
                timeText = text + ": " + timeText;
            }

            Debug.WriteLine(timeText);
        }

        public void WriteElapsedSeconds(string text = "")
        {
            Write(text, timer.Elapsed.TotalSeconds.ToString("0.000") + " seconds.");
        }

        public void WriteElapsedMilliseconds(string text = "")
        {
            Write(text, timer.ElapsedMilliseconds + " millisecond.");
        }

        public void Dispose()
        {
            WriteElapsedMilliseconds(text);
        }
    }
}
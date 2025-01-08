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

using ShareX.HelpersLib;
using System;
using System.Drawing;

namespace ShareX
{
    public class TaskMetadata : IDisposable
    {
        private const int WindowInfoMaxLength = 255;

        public Bitmap Image { get; set; }

        private string windowTitle;

        public string WindowTitle
        {
            get
            {
                return windowTitle;
            }
            set
            {
                windowTitle = value.Truncate(WindowInfoMaxLength);
            }
        }

        private string processName;

        public string ProcessName
        {
            get
            {
                return processName;
            }
            set
            {
                processName = value.Truncate(WindowInfoMaxLength);
            }
        }

        public TaskMetadata()
        {
        }

        public TaskMetadata(Bitmap image)
        {
            Image = image;
        }

        public void UpdateInfo(WindowInfo windowInfo)
        {
            if (windowInfo != null)
            {
                WindowTitle = windowInfo.Text;
                ProcessName = windowInfo.ProcessName;
            }
        }

        public void Dispose()
        {
            Image?.Dispose();
        }
    }
}
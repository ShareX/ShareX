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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareX
{
    public class QuickTaskInfo
    {
        public string Name { get; set; }
        public AfterCaptureTasks AfterCaptureTasks { get; set; }
        public AfterUploadTasks AfterUploadTasks { get; set; }

        public QuickTaskInfo(string name, AfterCaptureTasks afterCaptureTasks, AfterUploadTasks afterUploadTasks = AfterUploadTasks.None)
        {
            Name = name;
            AfterCaptureTasks = afterCaptureTasks;
            AfterUploadTasks = afterUploadTasks;
        }

        public QuickTaskInfo(AfterCaptureTasks afterCaptureTasks, AfterUploadTasks afterUploadTasks = AfterUploadTasks.None) : this(null, afterCaptureTasks, afterUploadTasks)
        {
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return Name;
            }

            string result = string.Join(", ", AfterCaptureTasks.GetFlags<AfterCaptureTasks>().Select(x => x.GetLocalizedDescription()));

            if (AfterCaptureTasks.HasFlag(AfterCaptureTasks.UploadImageToHost))
            {
                result += string.Join(", ", AfterUploadTasks.GetFlags<AfterUploadTasks>().Select(x => x.GetLocalizedDescription()));
            }

            return result;
        }
    }
}
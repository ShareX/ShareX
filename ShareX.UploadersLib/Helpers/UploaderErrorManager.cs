#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2022 ShareX Team

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

using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.UploadersLib
{
    public class UploaderErrorManager
    {
        public List<UploaderErrorInfo> Errors { get; private set; }

        public int Count => Errors.Count;

        public UploaderErrorManager()
        {
            Errors = new List<UploaderErrorInfo>();
        }

        public void Add(string text)
        {
            Add(Resources.Error, text);
        }

        public void Add(string title, string text)
        {
            Errors.Add(new UploaderErrorInfo(title, text));
        }

        public void Add(UploaderErrorManager manager)
        {
            Errors.AddRange(manager.Errors);
        }

        public void Insert(int index, string text)
        {
            Insert(index, Resources.Error, text);
        }

        public void Insert(int index, string title, string text)
        {
            Errors.Insert(index, new UploaderErrorInfo(title, text));
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine + Environment.NewLine, Errors.Select(x => x.Text));
        }
    }
}
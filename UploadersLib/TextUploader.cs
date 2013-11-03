#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using System.IO;
using System.Text;
using UploadersLib.HelperClasses;

namespace UploadersLib
{
    public abstract class TextUploader : Uploader
    {
        public abstract UploadResult UploadText(string text, string fileName);

        public UploadResult UploadText(Stream stream, string fileName)
        {
            using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
            {
                return UploadText(sr.ReadToEnd(), fileName);
            }
        }

        public UploadResult UploadTextFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return UploadText(stream, Path.GetFileName(filePath));
                }
            }

            return null;
        }
    }
}
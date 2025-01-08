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

using System;
using System.IO;
using System.Text;

namespace ShareX.HelpersLib
{
    public class NativeMessagingHost
    {
        public string Read()
        {
            string input = null;

            Stream inputStream = Console.OpenStandardInput();

            byte[] bytesLength = new byte[4];
            inputStream.Read(bytesLength, 0, bytesLength.Length);
            int inputLength = BitConverter.ToInt32(bytesLength, 0);

            if (inputLength > 0)
            {
                byte[] bytesInput = new byte[inputLength];
                inputStream.Read(bytesInput, 0, bytesInput.Length);
                input = Encoding.UTF8.GetString(bytesInput);
            }

            return input;
        }

        public void Write(string data)
        {
            Stream outputStream = Console.OpenStandardOutput();

            byte[] bytesData = Encoding.UTF8.GetBytes(data);
            byte[] bytesLength = BitConverter.GetBytes(bytesData.Length);

            outputStream.Write(bytesLength, 0, bytesLength.Length);

            if (bytesData.Length > 0)
            {
                outputStream.Write(bytesData, 0, bytesData.Length);
            }

            outputStream.Flush();
        }
    }
}
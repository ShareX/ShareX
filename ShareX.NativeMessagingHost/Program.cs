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

using Newtonsoft.Json;
using ShareX.HelpersLib;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ShareX.NativeMessagingHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                try
                {
                    Run();
                }
                catch (Exception e)
                {
                    e.ShowError();
                }
            }
            else
            {
                MessageBox.Show("This executable is used to receive data from browser addon and send it to ShareX.",
                    "ShareX NativeMessagingHost", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static void Run()
        {
            string input = GetInput();

            if (!string.IsNullOrEmpty(input))
            {
                NativeMessagingInput nativeMessagingInput = JsonConvert.DeserializeObject<NativeMessagingInput>(input);

                if (nativeMessagingInput != null)
                {
                    string argument = null;

                    if (!string.IsNullOrEmpty(nativeMessagingInput.URL))
                    {
                        argument = Helpers.EscapeCLIText(nativeMessagingInput.URL);
                    }
                    else if (!string.IsNullOrEmpty(nativeMessagingInput.Text))
                    {
                        string filepath = Helpers.GetTempPath("txt");
                        File.WriteAllText(filepath, nativeMessagingInput.Text, Encoding.UTF8);
                        argument = $"\"{filepath}\"";
                    }

                    if (!string.IsNullOrEmpty(argument))
                    {
                        string path = Helpers.GetAbsolutePath("ShareX.exe");

                        NativeMethods.CreateProcess(path, argument, CreateProcessFlags.CREATE_BREAKAWAY_FROM_JOB);
                    }
                }
            }
        }

        private static string GetInput()
        {
            Stream inputStream = Console.OpenStandardInput();

            byte[] bytesLength = new byte[4];
            inputStream.Read(bytesLength, 0, bytesLength.Length);
            int inputLength = BitConverter.ToInt32(bytesLength, 0);

            byte[] bytesInput = new byte[inputLength];
            inputStream.Read(bytesInput, 0, bytesInput.Length);
            return Encoding.UTF8.GetString(bytesInput);
        }
    }
}
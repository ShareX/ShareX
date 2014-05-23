#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

namespace HelpersLib
{
    public abstract class ExternalCLIManager : IDisposable
    {
        public event DataReceivedEventHandler OutputDataReceived;
        public event DataReceivedEventHandler ErrorDataReceived;

        private Process process = new Process();

        public virtual int Open(string path, string args = null)
        {
            DebugHelper.WriteLine("CLI: \"{0}\" {1}", path, args);

            if (File.Exists(path))
            {
                ProcessStartInfo psi = new ProcessStartInfo(path);
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.Arguments = args;
                psi.WorkingDirectory = Path.GetDirectoryName(path);

                process.EnableRaisingEvents = true;
                process.OutputDataReceived += cli_OutputDataReceived;
                process.ErrorDataReceived += cli_ErrorDataReceived;
                process.StartInfo = psi;
                process.Start();
                process.BeginErrorReadLine();
                process.WaitForExit();
                return process.ExitCode;
            }

            return -1;
        }

        private void cli_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (OutputDataReceived != null)
                {
                    OutputDataReceived(sender, e);
                }
            }
        }

        private void cli_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (ErrorDataReceived != null)
                {
                    ErrorDataReceived(sender, e);
                }
            }
        }

        public void WriteInput(string input)
        {
            if (process != null && process.StartInfo != null && process.StartInfo.RedirectStandardInput)
            {
                process.StandardInput.WriteLine(input);
            }
        }

        public virtual void Close()
        {
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        public void Dispose()
        {
            if (process != null)
            {
                process.Dispose();
            }
        }
    }
}
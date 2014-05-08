using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace HelpersLib
{
    public abstract class ExternalCLIManager : IDisposable
    {
        private Process cli = new Process();

        public StringBuilder Output = new StringBuilder();
        public StringBuilder Errors = new StringBuilder();

        public delegate void ErrorDataReceivedHandler();
        public event ErrorDataReceivedHandler ErrorDataReceived;

        public virtual void Open(string cliPath, string args = null)
        {
            if (File.Exists(cliPath))
            {
                ProcessStartInfo psi = new ProcessStartInfo(cliPath);
                psi.UseShellExecute = false;
                psi.ErrorDialog = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardInput = true;
                psi.RedirectStandardError = true;
                psi.RedirectStandardOutput = true;
                psi.WorkingDirectory = Path.GetDirectoryName(cliPath);
                psi.Arguments = args;

                cli.OutputDataReceived += (sender, e) => { if (e.Data != null) { Output.AppendLine(e.Data); } };
                cli.ErrorDataReceived += (sender, e) => { if (e.Data == null) { Errors.AppendLine(e.Data); } };

                cli.EnableRaisingEvents = true;
                cli.StartInfo = psi;
                cli.Start();

                Console.WriteLine("CLI Path: " + cliPath);
                Console.WriteLine("CLI Args: " + psi.Arguments);

                cli.BeginOutputReadLine();
                cli.BeginErrorReadLine();

                System.Windows.Forms.MessageBox.Show(Output.ToString());
                System.Windows.Forms.MessageBox.Show(Errors.ToString());

                cli.WaitForExit();
            }
        }

        public void SendCommand(string command)
        {
            if (cli != null)
            {
                cli.StandardInput.WriteLine(command);
            }
        }

        public virtual void Close()
        {
            cli.CloseMainWindow();
        }

        public virtual void OnErrorDataReceived()
        {
            if (ErrorDataReceived != null)
            {
                ErrorDataReceived();
            }
        }

        public void Dispose()
        {
            if (cli != null)
            {
                cli.Dispose();
            }
        }
    }
}
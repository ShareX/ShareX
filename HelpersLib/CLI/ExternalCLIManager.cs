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
        protected Process CLI = new Process();

        public StringBuilder Output = new StringBuilder();
        public StringBuilder Errors = new StringBuilder();

        public delegate void ErrorDataReceivedHandler();
        public event ErrorDataReceivedHandler ErrorDataReceived;

        public virtual void Run(string cliPath, string args = "")
        {
            ProcessStartInfo psi = new ProcessStartInfo(cliPath);
            psi.UseShellExecute = false;
            psi.ErrorDialog = false;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.Arguments = args;
            psi.WindowStyle = ProcessWindowStyle.Normal;

            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                CLI.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        outputWaitHandle.Set();
                    }
                    else
                    {
                        Output.AppendLine(e.Data);
                    }
                };

                CLI.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        errorWaitHandle.Set();
                    }
                    else
                    {
                        Errors.AppendLine(e.Data);
                    }
                };

                CLI.StartInfo = psi;
                CLI.Start();

                CLI.BeginOutputReadLine();
                CLI.WaitForExit();
            }
        }

        public void SendCommand(string command)
        {
            if (CLI != null)
            {
                CLI.StandardInput.WriteLine(command);
            }
        }

        public virtual void Close()
        {
            CLI.CloseMainWindow();
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
            if (CLI != null)
            {
                CLI.Dispose();
            }
        }
    }
}
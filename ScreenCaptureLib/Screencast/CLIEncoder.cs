using HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScreenCaptureLib
{
    public abstract class CLIEncoder : ExternalCLIManager
    {
        public abstract void Record();
    }
}
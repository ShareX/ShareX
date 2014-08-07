using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX
{
    class ShareXApplicationBase : WindowsFormsApplicationBase
    {
        public ShareXApplicationBase(bool isSingleInstance)
        {
            IsSingleInstance = isSingleInstance;
            EnableVisualStyles = true;
        }

        public new Form MainForm
        {
            get { return base.MainForm; }
            set { base.MainForm = value; }
        }
    }
}

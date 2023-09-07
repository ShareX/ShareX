using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareX.HelpersLib;
using ShareX.ScreenCaptureLib;

namespace ShareX.HelpersLib
{
    public partial class WindowOpacityForm : Form
    {
        public WindowInfo SelectedWindow { get; private set; }

        public WindowOpacityForm()
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);
            SelectHandle(true);
        }

        private void SelectHandle(bool isWindow)
        {
            RegionCaptureOptions options = new RegionCaptureOptions()
            {
                DetectControls = !isWindow
            };

            SelectedWindow = null;

            SimpleWindowInfo simpleWindowInfo = RegionCaptureTasks.GetWindowInfo(options);

            if (simpleWindowInfo != null)
            {
                SelectedWindow = new WindowInfo(simpleWindowInfo.Handle);
            }

            UpdateWindowInfo();
        }

        private void UpdateWindowInfo()
        {
            tbSelectedItemData.Text = "";
            trkbOpacityValue.Value = 100;

            trkbOpacityValue.Enabled =
                btnRefreshOpacitySelectedElemet.Enabled = SelectedWindow != null;
            
            if (SelectedWindow != null)
            {
                tbSelectedItemData.Text = SelectedWindow.Handle.ToString("X8") + ", " + SelectedWindow.ProcessId + ", " + SelectedWindow.ProcessName;
                trkbOpacityValue.Value = (int)NativeMethods.GetWindowOpacity(SelectedWindow.Handle);
            }
        }

        private void UpdateWindowOpacity()
        {
            if (SelectedWindow != null)
            {
                NativeMethods.SetWindowOpacity(SelectedWindow.Handle, (uint)trkbOpacityValue.Value);
            }
        }

        private void btnSelectWindow_Click(object sender, EventArgs e)
        {
            SelectHandle(true);
        }

        private void btnSelectControl_Click(object sender, EventArgs e)
        {
            SelectHandle(false);
        }

        private void btnRefreshOpacitySelectedElemet_Click(object sender, EventArgs e)
        {
            UpdateWindowOpacity();
        }

        private void trkbOpacityValue_ValueChanged(object sender, EventArgs e)
        {
            tbOpacityValue.Text = trkbOpacityValue.Value.ToString();
        }
    }
}

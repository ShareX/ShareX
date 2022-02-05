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

namespace ShareX
{
    public partial class TimedRecordingForm : Form
    {
        private const string InvalidResultMessage = "Please enter a valid recording length";

        public TimedRecordingForm()
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this);
        }

        private void startRecordingBtn_Click(object sender, EventArgs e)
        {
            double totalTime;
            double regionDelay = 3000;
            double recordingLength = CheckValidInput();

            if (recordingLength != 0)
            {
                if (secondsRadioBtn.Checked)
                {
                    totalTime = (recordingLength * 1000) + regionDelay;
                }
                else
                {
                    totalTime = (recordingLength * 60000) + regionDelay;
                }

                this.Hide();
                TimedScreenRecordingManager.StartTimedRecording(totalTime);

            }
        }

        private double CheckValidInput()
        {
            bool validLength = double.TryParse(lengthTxt.Text, out double recordingLength);

            if (validLength)
                return recordingLength;
            else
            {
                MessageBox.Show(InvalidResultMessage);
                return 0;
            }
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void TimedRecordingForm_Load(object sender, EventArgs e)
        {

        }
    }
}

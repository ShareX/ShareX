using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ShareX.ScreenCaptureLib;

namespace ShareX
{
    public class TimedScreenRecordingManager
    {
        private static Timer recordingTimer;

        public static void StartTimedRecording(double timeAmountMillisecond)
        {
            TaskHelpers.StartScreenRecording(ScreenRecordOutput.FFmpeg, ScreenRecordStartMethod.Region);
            recordingTimer = new Timer(timeAmountMillisecond);
            recordingTimer.Elapsed += StopTimedRecording;
            recordingTimer.Start();
        }
        private static void StopTimedRecording(object sender, ElapsedEventArgs e)
        {
            TaskHelpers.StopScreenRecording();
            recordingTimer.Stop();
        }
    }
}

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
        private static bool _timedRecordingRunning = false;
        private static Timer recordingTimer;

        public static void StartTimedRecording(double timeAmountMillisecond)
        {
            _timedRecordingRunning = true;

            TaskHelpers.StartScreenRecording(ScreenRecordOutput.FFmpeg, ScreenRecordStartMethod.Region);
            recordingTimer = new Timer(timeAmountMillisecond);
            recordingTimer.Elapsed += StopTimedRecording;
            recordingTimer.Start();
        }

        public static void StopRecordingTimer()
        {
            _timedRecordingRunning = false;

            if (recordingTimer != null)
            {
                recordingTimer.Stop();
                recordingTimer.Close();
            }
        }
        private static void StopTimedRecording(object sender, ElapsedEventArgs e)
        {
            if (_timedRecordingRunning)
            {
                TaskHelpers.StopScreenRecording();
                recordingTimer.Stop();
                recordingTimer.Close();
            }
        }
    }
}

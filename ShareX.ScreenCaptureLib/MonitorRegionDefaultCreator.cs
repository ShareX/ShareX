using System.Linq;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public static class MonitorRegionDefaultCreator
    {
        private static readonly int firstMonitorNumber = 1;
        private static int monitorCounter;

        /// <summary>
        /// Return the list of screens available on this computer
        /// </summary>
        public static MonitorRegion[] AllMonitorsRegions
        {
            get
            {
                Screen[] screens = Screen.AllScreens;
                monitorCounter = firstMonitorNumber;

                return screens.Select(screen => new MonitorRegion(screen, monitorCounter++)).ToArray();
            }
        }

        /// <summary>
        /// Return the screen region for the primary monitor.
        /// </summary>
        public static MonitorRegion DefaultMonitorRegion
        {
            get
            {
                Screen defaultScreen = Screen.PrimaryScreen;

                return new MonitorRegion(defaultScreen, firstMonitorNumber);
            }
        }
    }
}


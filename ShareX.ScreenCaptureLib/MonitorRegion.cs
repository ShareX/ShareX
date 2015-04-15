using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ShareX.ScreenCaptureLib.Properties;

namespace ShareX.ScreenCaptureLib
{
    public class MonitorRegion
    {
        #region Properties

        [DefaultValue("Monitor1 X:0 Y:0 Height:0 Width:0"), Description("The default monitor name.")]
        public string MonitorName { get; private set; }

        [DefaultValue(0)]
        public int X { get; private set; }

        [DefaultValue(0)]
        public int Y { get; private set; }

        [DefaultValue(0)]
        public int Height { get; private set; }

        [DefaultValue(0)]
        public int Width { get; private set; }

        [DefaultValue(0)]
        public Rectangle MonitorBounds { get; private set; }

        #endregion

        #region Constructor

        public MonitorRegion(Screen monitor, int monitorNumber)
        {
            SetBoundsFromTheScreenBounds(monitor);
            CreateTheNameFromBoundsAndMonitorNumber(monitor.Bounds, monitorNumber);
            MonitorBounds = monitor.Bounds;
        }

        #endregion

        #region private Methods

        private void SetBoundsFromTheScreenBounds(Screen screen)
        {
            X = screen.Bounds.X;
            Y = screen.Bounds.Y;
            Height = screen.Bounds.Height;
            Width = screen.Bounds.Width;
        }

        private void CreateTheNameFromBoundsAndMonitorNumber(Rectangle monitorBounds, int monitorNumber)
        {
            MonitorName = String.Format(Resources.ScreenRegion_Name_Monitor_0___X__1__Y__2__Height__3__Width__4_, monitorNumber, X, Y, Height, Width);
        }

        #endregion

        #region overrides Methods

        public override string ToString()
        {
            return MonitorName;
        }

        #endregion
    }
}

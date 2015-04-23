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
        public string MonitorIdentifier { get; private set; }

        [DefaultValue(typeof(Rectangle), "{X=0,Y=0,Width=0,Height=0}")]
        public Rectangle Bounds { get; private set; }

        #endregion

        #region Constructor

        public MonitorRegion(Screen monitor, int monitorNumber)
        {
            Bounds = monitor.Bounds;
            CreateTheNameFromBoundsAndMonitorNumber(monitorNumber);
        }

        #endregion

        #region private Methods

        private void CreateTheNameFromBoundsAndMonitorNumber(int monitorNumber)
        {
            MonitorIdentifier = String.Format(Resources.ScreenRegion_Name_Monitor_0___X__1__Y__2__Height__3__Width__4_, monitorNumber, Bounds.X, Bounds.Y, Bounds.Height, Bounds.Width);
        }

        #endregion

        #region overrides Methods

        public override string ToString()
        {
            return MonitorIdentifier;
        }

        #endregion
    }
}

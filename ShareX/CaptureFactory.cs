using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareX
{
    class CaptureFactory
    {
        //Singleton: one instance of factory
        private static CaptureFactory instance;

        private CaptureFactory() { }

        public static CaptureFactory getInstance()
        {
            if (instance == null)
            {
                instance = new CaptureFactory();
            }
            return instance;
        }

        public CaptureStrategy getStrategy(CaptureType captureType, MainForm mainForm)
        {
            CaptureStrategy strategy = null;

            switch (captureType)
            {
                case CaptureType.Screen:
                    strategy = new CaptureScreen(mainForm);
                    break;
                case CaptureType.ActiveWindow:
                    strategy = new CaptureActiveWindow(mainForm);
                    break;
                case CaptureType.ActiveMonitor:
                    strategy = new CaptureActiveMonitor(mainForm);
                    break;
                case CaptureType.Rectangle:
                case CaptureType.RectangleWindow:
                case CaptureType.Polygon:
                case CaptureType.Freehand:
                    strategy = new CaptureRegion(mainForm);
                    break;
                case CaptureType.CustomRegion:
                    strategy = new CaptureCustomRegion(mainForm);
                    break;
                case CaptureType.LastRegion:
                    strategy = new CaptureLastRegion(mainForm);
                    break;
            }
            return strategy;
        }

    }
}

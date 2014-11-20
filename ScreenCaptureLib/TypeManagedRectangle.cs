using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace ScreenCaptureLib
{
    class TypeManagedRectangle : iCaptureType
    {
        public Image Capture(Rectangle rect, IntPtr handle, bool captureCursor)
        {
            if (rect.Width == 0 || rect.Height == 0)
            {
                return null;
            }

            Image img = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(img))
            {
                // Managed can't use SourceCopy | CaptureBlt because of .NET bug
                g.CopyFromScreen(rect.Location, Point.Empty, rect.Size, CopyPixelOperation.SourceCopy);
            }

            return img;
        }

        public TypeManagedRectangle()
        {
        }
    }
}

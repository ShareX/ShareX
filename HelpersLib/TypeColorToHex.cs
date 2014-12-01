using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HelpersLib
{
    public class TypeColorToHex : iColorFormatType<string>
    {
        public string ColorFormat(Color color, String hex, int dec, ColorFormat format)
        {
            switch (format)
            {
                default:
                case HelpersLib.ColorFormat.RGB:
                    return string.Format("{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
                case HelpersLib.ColorFormat.RGBA:
                    return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", color.R, color.G, color.B, color.A);
                case HelpersLib.ColorFormat.ARGB:
                    return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
            }
        }

        public TypeColorToHex()
        {
        }
    }
}

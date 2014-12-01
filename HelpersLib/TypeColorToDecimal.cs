using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HelpersLib
{
    class TypeColorToDecimal : iColorFormatType<int>
    {
        public int ColorFormat(Color color, String hex, int dec, ColorFormat format)
        {

            switch (format)
            {
                default:
                case HelpersLib.ColorFormat.RGB:
                    return color.R << 16 | color.G << 8 | color.B;
                case HelpersLib.ColorFormat.RGBA:
                    return color.R << 24 | color.G << 16 | color.B << 8 | color.A;
                case HelpersLib.ColorFormat.ARGB:
                    return color.A << 24 | color.R << 16 | color.G << 8 | color.B;
            }
        }

        public TypeColorToDecimal()
        {
        }
    }
}
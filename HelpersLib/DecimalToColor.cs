using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HelpersLib
{
    class DecimalToColor : iColorHelpers<Color>
    {
        iColorFormatType<Color> ColorFormatType = new TypeDecimalToColor();
        int dec;
        ColorFormat colorFormat = HelpersLib.ColorFormat.RGB;

        public void setDecimal(int dec)
        {
            this.dec = dec;
        }

        public int getDecimal()
        {
            return dec;
        }

        public void setColorFormat(ColorFormat colorFormat)
        {
            this.colorFormat = colorFormat;
        }

        public ColorFormat getColorFormat()
        {
            return colorFormat;
        }

        public override Color ColorHelpers()
        {
            return ColorFormatType.ColorFormat(Color.Empty, "", dec, colorFormat);
        }
    }
}
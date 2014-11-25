using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HelpersLib
{
    class DecimalToColor : iColorHelpers
    {
        iColorFormatType ColorFormatType = new TypeDecimalToColor();
        int dec;

        public void setDecimal(int dec)
        {
            this.dec = dec;
        }

        public int getDecimal()
        {
            return dec;
        }

        public override void ColorHelpers()
        {
            ColorFormatType.ColorFormat(Color.Empty, "", dec);
        }
    }
}
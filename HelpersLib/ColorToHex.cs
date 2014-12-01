using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HelpersLib
{
    public class ColorToHex : iColorHelpers<String>
    {
        iColorFormatType<String> ColorFormatType = new TypeColorToHex();
        Color color;
        ColorFormat colorFormat = HelpersLib.ColorFormat.RGB;

        public void setColor(Color color)
        {
            this.color = color;
        }

        public Color getColor()
        {
            return color;
        }

        public void setColorFormat(ColorFormat colorFormat)
        {
            this.colorFormat = colorFormat;
        }

        public ColorFormat getColorFormat()
        {
            return colorFormat;
        }

        public override String ColorHelpers()
        {
           return ColorFormatType.ColorFormat(color, "", 0, colorFormat);
        }
    }
}

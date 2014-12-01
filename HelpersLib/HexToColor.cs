using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HelpersLib
{
    class HexToColor : iColorHelpers<Color>
    {
        iColorFormatType<Color> ColorFormatType = new TypeHexToColor();
        String hex;
        ColorFormat colorFormat = HelpersLib.ColorFormat.RGB;

        public void setHex(String hex)
        {
            this.hex = hex;
        }

        public String getHex()
        {
            return hex;
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
            return ColorFormatType.ColorFormat(Color.Empty, hex, 0, colorFormat);
        }
    }
}

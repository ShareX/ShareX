using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HelpersLib
{
    class HexToColor : iColorHelpers
    {
        iColorFormatType ColorFormatType = new TypeHexToColor();
        String hex;

        public void setHex(String hex)
        {
            this.hex = hex;
        }

        public String getHex()
        {
            return hex;
        }

        public override void ColorHelpers()
        {
            ColorFormatType.ColorFormat(Color.Empty, hex, 0);
        }
    }
}

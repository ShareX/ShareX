using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HelpersLib
{
    class ColorToHex : iColorHelpers
    {
        iColorFormatType ColorFormatType = new TypeColorToHex();
        Color color;

        public void setColor(Color color)
        {
            this.color = color;
        }

        public Color getColor()
        {
            return color;
        }

        public override void ColorHelpers()
        {
            ColorFormatType.ColorFormat(color, "", 0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HelpersLib
{
    class TypeDecimalToColor : iColorFormatType
    {
        public Color ColorFormat(Color color, String hex, int dec)
        {
            ColorFormat format = HelpersLib.ColorFormat.RGB;

            switch (format)
            {
                default:
                case HelpersLib.ColorFormat.RGB:
                    return Color.FromArgb((dec >> 16) & 0xFF, (dec >> 8) & 0xFF, dec & 0xFF);
                case HelpersLib.ColorFormat.RGBA:
                    return Color.FromArgb(dec & 0xFF, (dec >> 24) & 0xFF, (dec >> 16) & 0xFF, (dec >> 8) & 0xFF);
                case HelpersLib.ColorFormat.ARGB:
                    return Color.FromArgb((dec >> 24) & 0xFF, (dec >> 16) & 0xFF, (dec >> 8) & 0xFF, dec & 0xFF);
            }
        }

        public TypeDecimalToColor() 
        {
        }
    }
}

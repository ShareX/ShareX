using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HelpersLib
{
    class TypeHexToColor : iColorFormatType
    {
        public Color ColorFormat(Color color, String hex, int dec)
        {
            ColorFormat format = HelpersLib.ColorFormat.RGB;

            if (string.IsNullOrEmpty(hex))
            {
                return Color.Empty;
            }

            if (hex[0] == '#')
            {
                hex = hex.Remove(0, 1);
            }

            if (((format == HelpersLib.ColorFormat.RGBA || format == HelpersLib.ColorFormat.ARGB) && hex.Length != 8) ||
                (format == HelpersLib.ColorFormat.RGB && hex.Length != 6))
            {
                return Color.Empty;
            }

            int r, g, b, a;

            switch (format)
            {
                default:
                case HelpersLib.ColorFormat.RGB:
                    r = HexToDecimal(hex.Substring(0, 2));
                    g = HexToDecimal(hex.Substring(2, 2));
                    b = HexToDecimal(hex.Substring(4, 2));
                    a = 255;
                    break;
                case HelpersLib.ColorFormat.RGBA:
                    r = HexToDecimal(hex.Substring(0, 2));
                    g = HexToDecimal(hex.Substring(2, 2));
                    b = HexToDecimal(hex.Substring(4, 2));
                    a = HexToDecimal(hex.Substring(6, 2));
                    break;
                case HelpersLib.ColorFormat.ARGB:
                    a = HexToDecimal(hex.Substring(0, 2));
                    r = HexToDecimal(hex.Substring(2, 2));
                    g = HexToDecimal(hex.Substring(4, 2));
                    b = HexToDecimal(hex.Substring(6, 2));
                    break;
            }

            return Color.FromArgb(a, r, g, b);

        }

        public static int HexToDecimal(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }

        public TypeHexToColor()
        {
        }

    }
}
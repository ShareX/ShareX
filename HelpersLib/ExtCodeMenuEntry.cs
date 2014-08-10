using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelpersLib
{
    public class ExtCodeMenuEntry : CodeMenuEntry
    {
        public ExtCodeMenuEntry(string value, string description) : base(value, description) { }

        public override String ToPrefixString() { return '.' + _value; }

        public static readonly ExtCodeMenuEntry bmp = new ExtCodeMenuEntry("bmp", "Bitmap Image File");
        public static readonly ExtCodeMenuEntry gif = new ExtCodeMenuEntry("gif", "Graphical Interchange Format File");
        public static readonly ExtCodeMenuEntry jpg = new ExtCodeMenuEntry("jpg", "JPEG Image");
        public static readonly ExtCodeMenuEntry png = new ExtCodeMenuEntry("png", "Portable Network Graphic");
        public static readonly ExtCodeMenuEntry tif = new ExtCodeMenuEntry("tif", "Tagged Image File");
    }
}

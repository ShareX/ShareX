using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    public class CombineBoundingBox
    {
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public CombineBoundingBox(int x, int y, int w, int h)
        {
            XPosition = x;
            YPosition = y;
            Width = w;
            Height = h;
        }
    }
}

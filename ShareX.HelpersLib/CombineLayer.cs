using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    public class CombineLayer
    {
        public int YPosition { get; set; }
        public int Height { get; set; }

        public CombineLayer(int y, int height)
        {
            YPosition = y;
            Height = height;
        }
    }
}

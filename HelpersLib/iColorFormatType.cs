using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace HelpersLib
{
    public interface iColorFormatType<Generic>
    {
        Generic ColorFormat(Color color, String hex, int dec, ColorFormat colorFormat);
    }
}

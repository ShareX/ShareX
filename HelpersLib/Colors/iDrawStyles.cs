using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelpersLib
{
    public abstract class iDrawStyles
    {

         
     public abstract void SetBoxMarker(Point lastPos, MyColor SelectedColor, int ClientWidth, int ClientHeight);
     public abstract void SetSliderMarker(Point lastPos, MyColor SelectedColor, int ClientHeight);
     public abstract void GetBoxColor(Point lastPos, MyColor selectedColor, int ClientWidth, int ClientHeight);
     public abstract void GetSliderColor(Point lastPos, MyColor selectedColor, int ClientWidth, int ClientHeight);




        protected int Round(double val)
        {
            int ret_val = (int)val;

            int temp = (int)(val * 100);

            if ((temp % 100) >= 50)
                ret_val += 1;

            return ret_val;
        }
    }
}

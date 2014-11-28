using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace HelpersLib
{
    class Blue : iDrawStyles
    {
        public override void SetBoxMarker(Point lastPos, MyColor SelectedColor, int ClientWidth, int ClientHeight)
        {
            lastPos.X = Round((ClientWidth - 1) * (double)SelectedColor.RGBA.Red / 255);
            lastPos.Y = Round((ClientHeight - 1) * (1.0 - (double)SelectedColor.RGBA.Green / 255));
        }
        public override void SetSliderMarker(Point lastPos, MyColor SelectedColor, int ClientHeight)
        {
            lastPos.Y = (ClientHeight - 1) - Round((ClientHeight - 1) * (double)SelectedColor.RGBA.Blue / 255);
        }
        public override void GetBoxColor(Point lastPos, MyColor selectedColor, int ClientWidth, int ClientHeight)
        {
            selectedColor.RGBA.Red = Round(255 * (double)lastPos.X / (ClientWidth - 1));
            selectedColor.RGBA.Green = Round(255 * (1.0 - (double)lastPos.Y / (ClientHeight - 1)));
            selectedColor.RGBAUpdate();
        }
        public override void GetSliderColor(Point lastPos, MyColor selectedColor, int ClientWidth, int ClientHeight)
        {
            selectedColor.RGBA.Blue = 255 - Round(255 * (double)lastPos.Y / (ClientHeight - 1));
            selectedColor.RGBAUpdate();
        }
    }
}
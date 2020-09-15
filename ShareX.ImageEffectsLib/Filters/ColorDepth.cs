using System.ComponentModel;
using System.Drawing;
using ShareX.HelpersLib;

namespace ShareX.ImageEffectsLib
{
    [Description("Color depth")]
    internal class ColorDepth : ImageEffect
    {
        private int _bitsPerChannel;

        [DefaultValue(4)]
        public int BitsPerChannel
        {
            get => this._bitsPerChannel;
            set => this._bitsPerChannel = value.Max(1).Min(8);
        }

        public ColorDepth()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            ImageHelpers.ColorDepth(bmp, this.BitsPerChannel);
            return bmp;
        }
    }
}
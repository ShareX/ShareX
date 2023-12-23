using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.ScreenCaptureLib
{
    internal class ImageEditorMemento
    {
        public ImageEditorMemento(List<BaseShape> shapes)
        {
            Shapes = shapes;
        }

        public ImageEditorMemento(List<BaseShape> shapes, Bitmap canvas)
        {
            Shapes = shapes;
            Canvas = canvas;
        }

        public List<BaseShape> Shapes { get; private set; }
        public Bitmap Canvas { get; private set; }

    }
}

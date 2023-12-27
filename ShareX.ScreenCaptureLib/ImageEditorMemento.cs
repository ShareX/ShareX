using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    internal class ImageEditorMemento : IDisposable
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

        public void Dispose()
        {
            if(Canvas != null)
            {
                Canvas.Dispose();
            }
            foreach (BaseShape shape in Shapes)
            {
                shape.Dispose();
            }

            Shapes.Clear();
        }
    }
}

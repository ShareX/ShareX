using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.ScreenCaptureLib
{
    internal class ImageEditorHistory : IDisposable
    {
        private Stack<ImageEditorMemento> mementoStack = new Stack<ImageEditorMemento>();
        private Stack<ImageEditorMemento> redoMementoStack = new Stack<ImageEditorMemento>();
        private readonly ShapeManager shapeManager;
        public ImageEditorHistory(ShapeManager shapeManager)
        {

            this.shapeManager = shapeManager;
        }

        public void AddMemento(ImageEditorMemento memento)
        {
            mementoStack.Push(memento);
            foreach (var mmnt in redoMementoStack)
            {
                mmnt.Dispose();
            }
            redoMementoStack.Clear();
        }


        private ImageEditorMemento GetMementoFromCanvas()
        {
            Bitmap canvas = shapeManager.Form.Canvas;

            Rectangle rectClone = new Rectangle(0, 0, canvas.Width, canvas.Height);
            Bitmap currentCanvas = canvas.Clone(rectClone, canvas.PixelFormat);
            var shapes = shapeManager.Shapes.Select(x => x.Duplicate()).ToList();

            return new ImageEditorMemento(shapes, currentCanvas);

        }

        public void CreateCanvasMemento()
        {
            AddMemento(GetMementoFromCanvas());
        }

        public void CreateShapesMemento()
        {
            var shapes = shapeManager.Shapes.Select(x => x.Duplicate()).ToList();
            AddMemento(new ImageEditorMemento(shapes));
        }

        public void Undo()
        {
            if (mementoStack.Count > 0)
            {
                ImageEditorMemento redoMemento;
                var memento = mementoStack.Pop();

                if (memento.Shapes != null)
                {
                    if (memento.Canvas == null)
                    {
                        redoMemento = new ImageEditorMemento(shapeManager.Shapes.Select(x => x.Duplicate()).ToList());
                        redoMementoStack.Push(redoMemento);

                        shapeManager.RestoreState(memento.Shapes);
                    }
                    else
                    {
                        redoMemento = GetMementoFromCanvas();
                        redoMementoStack.Push(redoMemento);

                        shapeManager.RestoreState(memento.Shapes, memento.Canvas);
                    }

                }

            }
        }

        public void Redo()
        {
            if (redoMementoStack.Count > 0)
            {
                var redoMemento = redoMementoStack.Pop();
                if (redoMemento.Shapes != null)
                {
                    if (redoMemento.Canvas == null)
                    {
                        var shapes = shapeManager.Shapes.Select(x => x.Duplicate()).ToList();
                        var memento = new ImageEditorMemento(shapes);
                        mementoStack.Push(memento);

                        shapeManager.RestoreState(redoMemento.Shapes);
                    }
                    else
                    {
                        var memento = GetMementoFromCanvas();
                        mementoStack.Push(memento);

                        shapeManager.RestoreState(redoMemento.Shapes, redoMemento.Canvas);
                    }

                }
            }
        }
        public void Dispose()
        {
            foreach (var memnto in mementoStack.Concat(redoMementoStack))
            {
                memnto.Dispose();
            }
            mementoStack.Clear();
            redoMementoStack.Clear();
        }

        public bool HasMementos()
        {
            return mementoStack.Count > 0;
        }

        public bool HasRedoMementos()
        {
            return redoMementoStack.Count > 0;
        }
    }

}

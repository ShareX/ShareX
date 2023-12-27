#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ShareX.ScreenCaptureLib
{
    internal class ImageEditorHistory : IDisposable
    {
        public bool HasMementos => mementoStack.Count > 0;

        public bool HasRedoMementos => redoMementoStack.Count > 0;

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

            foreach (ImageEditorMemento redoMemento in redoMementoStack)
            {
                redoMemento.Dispose();
            }

            redoMementoStack.Clear();
        }

        private ImageEditorMemento GetMementoFromCanvas()
        {
            Bitmap canvas = shapeManager.Form.Canvas;
            Rectangle rectClone = new Rectangle(0, 0, canvas.Width, canvas.Height);
            Bitmap currentCanvas = canvas.Clone(rectClone, canvas.PixelFormat);
            List<BaseShape> shapes = shapeManager.Shapes.Select(x => x.Duplicate()).ToList();

            return new ImageEditorMemento(shapes, currentCanvas);
        }

        public void CreateCanvasMemento()
        {
            AddMemento(GetMementoFromCanvas());
        }

        public void CreateShapesMemento()
        {
            List<BaseShape> shapes = shapeManager.Shapes.Select(x => x.Duplicate()).ToList();
            AddMemento(new ImageEditorMemento(shapes));
        }

        public void Undo()
        {
            if (mementoStack.Count > 0)
            {
                ImageEditorMemento redoMemento;
                ImageEditorMemento memento = mementoStack.Pop();

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
                ImageEditorMemento redoMemento = redoMementoStack.Pop();

                if (redoMemento.Shapes != null)
                {
                    if (redoMemento.Canvas == null)
                    {
                        List<BaseShape> shapes = shapeManager.Shapes.Select(x => x.Duplicate()).ToList();
                        ImageEditorMemento memento = new ImageEditorMemento(shapes);
                        mementoStack.Push(memento);

                        shapeManager.RestoreState(redoMemento.Shapes);
                    }
                    else
                    {
                        ImageEditorMemento memento = GetMementoFromCanvas();
                        mementoStack.Push(memento);

                        shapeManager.RestoreState(redoMemento.Shapes, redoMemento.Canvas);
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (ImageEditorMemento memento in mementoStack)
            {
                memento.Dispose();
            }

            mementoStack.Clear();

            foreach (ImageEditorMemento redoMemento in redoMementoStack)
            {
                redoMemento.Dispose();
            }

            redoMementoStack.Clear();
        }
    }
}
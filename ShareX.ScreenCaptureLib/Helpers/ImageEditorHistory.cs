#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
        public bool CanUndo => undoMementoStack.Count > 0;
        public bool CanRedo => redoMementoStack.Count > 0;

        private readonly ShapeManager shapeManager;
        private Stack<ImageEditorMemento> undoMementoStack = new Stack<ImageEditorMemento>();
        private Stack<ImageEditorMemento> redoMementoStack = new Stack<ImageEditorMemento>();

        public ImageEditorHistory(ShapeManager shapeManager)
        {
            this.shapeManager = shapeManager;
        }

        private void AddMemento(ImageEditorMemento memento)
        {
            undoMementoStack.Push(memento);

            foreach (ImageEditorMemento redoMemento in redoMementoStack)
            {
                redoMemento?.Dispose();
            }

            redoMementoStack.Clear();
        }

        private ImageEditorMemento GetMementoFromCanvas()
        {
            List<BaseShape> shapes = shapeManager.Shapes.Select(x => x.Duplicate()).ToList();
            Bitmap canvas = (Bitmap)shapeManager.Form.Canvas.Clone();
            return new ImageEditorMemento(shapes, shapeManager.Form.CanvasRectangle, canvas);
        }

        private ImageEditorMemento GetMementoFromShapes()
        {
            List<BaseShape> shapes = shapeManager.Shapes.Select(x => x.Duplicate()).ToList();
            return new ImageEditorMemento(shapes, shapeManager.Form.CanvasRectangle);
        }

        public void CreateCanvasMemento()
        {
            ImageEditorMemento memento = GetMementoFromCanvas();
            AddMemento(memento);
        }

        public void CreateShapesMemento()
        {
            if (!shapeManager.IsCurrentShapeTypeRegion && shapeManager.CurrentTool != ShapeType.ToolCrop && shapeManager.CurrentTool != ShapeType.ToolCutOut)
            {
                ImageEditorMemento memento = GetMementoFromShapes();
                AddMemento(memento);
            }
        }

        public void Undo()
        {
            if (CanUndo)
            {
                ImageEditorMemento undoMemento = undoMementoStack.Pop();

                if (undoMemento.Shapes != null)
                {
                    if (undoMemento.Canvas == null)
                    {
                        ImageEditorMemento redoMemento = GetMementoFromShapes();
                        redoMementoStack.Push(redoMemento);

                        shapeManager.RestoreState(undoMemento);
                    }
                    else
                    {
                        ImageEditorMemento redoMemento = GetMementoFromCanvas();
                        redoMementoStack.Push(redoMemento);

                        shapeManager.RestoreState(undoMemento);
                    }
                }
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                ImageEditorMemento redoMemento = redoMementoStack.Pop();

                if (redoMemento.Shapes != null)
                {
                    if (redoMemento.Canvas == null)
                    {
                        ImageEditorMemento undoMemento = GetMementoFromShapes();
                        undoMementoStack.Push(undoMemento);

                        shapeManager.RestoreState(redoMemento);
                    }
                    else
                    {
                        ImageEditorMemento undoMemento = GetMementoFromCanvas();
                        undoMementoStack.Push(undoMemento);

                        shapeManager.RestoreState(redoMemento);
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (ImageEditorMemento undoMemento in undoMementoStack)
            {
                undoMemento?.Dispose();
            }

            undoMementoStack.Clear();

            foreach (ImageEditorMemento redoMemento in redoMementoStack)
            {
                redoMemento?.Dispose();
            }

            redoMementoStack.Clear();
        }
    }
}
#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class StickerDrawingShape : ImageDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingSticker;

        public override void OnConfigLoad()
        {
            ImageInterpolationMode = ImageEditorInterpolationMode.NearestNeighbor;
        }

        public override void OnConfigSave()
        {
        }

        public override void ShowNodes()
        {
        }

        public override void OnCreating()
        {
            Point pos = InputManager.ClientMousePosition;
            Rectangle = new Rectangle(pos.X, pos.Y, 1, 1);

            Manager.Form.Pause();

            try
            {
                using (StickerForm stickerForm = new StickerForm(AnnotationOptions.StickerPacks, AnnotationOptions.StickerSize))
                {
                    if (stickerForm.ShowDialog() == DialogResult.OK)
                    {
                        AnnotationOptions.StickerSize = stickerForm.StickerSize;

                        if (!string.IsNullOrEmpty(stickerForm.SelectedImageFile))
                        {
                            Image img = ImageHelpers.LoadImage(stickerForm.SelectedImageFile);

                            img = ImageHelpers.ResizeImage(img, new Size(stickerForm.StickerSize, stickerForm.StickerSize));

                            if (img != null)
                            {
                                SetImage(img, true);
                                OnCreated();
                                return;
                            }
                        }
                    }
                }

                Remove();
            }
            finally
            {
                Manager.Form.Resume();
            }
        }
    }
}
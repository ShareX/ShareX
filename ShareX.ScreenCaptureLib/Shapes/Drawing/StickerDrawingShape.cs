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

using ShareX.HelpersLib;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class StickerDrawingShape : ImageDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingSticker;

        public override void OnConfigLoad()
        {
            ImageInterpolationMode = ImageInterpolationMode.NearestNeighbor;
        }

        public override void OnConfigSave()
        {
        }

        public override void ShowNodes()
        {
        }

        public override void OnCreating()
        {
            PointF pos = Manager.Form.ScaledClientMousePosition;
            Rectangle = new RectangleF(pos.X, pos.Y, 1, 1);

            if (Manager.IsCtrlModifier && LoadSticker(AnnotationOptions.LastStickerPath, AnnotationOptions.StickerSize))
            {
                OnCreated();
                Manager.IsMoving = true;
            }
            else if (OpenStickerForm())
            {
                OnCreated();
            }
            else
            {
                Remove();
            }
        }

        public override void OnDoubleClicked()
        {
            OpenStickerForm();
        }

        public override void Resize(int x, int y, bool fromBottomRight)
        {
            Move(x, y);
        }

        private bool OpenStickerForm()
        {
            Manager.Form.Pause();

            try
            {
                using (StickerForm stickerForm = new StickerForm(AnnotationOptions.StickerPacks, AnnotationOptions.SelectedStickerPack, AnnotationOptions.StickerSize))
                {
                    if (stickerForm.ShowDialog(Manager.Form) == DialogResult.OK)
                    {
                        AnnotationOptions.SelectedStickerPack = stickerForm.SelectedStickerPack;
                        AnnotationOptions.StickerSize = stickerForm.StickerSize;

                        return LoadSticker(stickerForm.SelectedImageFile, stickerForm.StickerSize);
                    }
                }
            }
            finally
            {
                Manager.Form.Resume();
            }

            return false;
        }

        private bool LoadSticker(string filePath, int stickerSize)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                Bitmap bmp = ImageHelpers.LoadImage(filePath);

                if (bmp != null)
                {
                    AnnotationOptions.LastStickerPath = filePath;

                    bmp = ImageHelpers.ResizeImageLimit(bmp, stickerSize);

                    SetImage(bmp, true);

                    return true;
                }
            }

            return false;
        }
    }
}
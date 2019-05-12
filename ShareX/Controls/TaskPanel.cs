#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareX.HelpersLib;

namespace ShareX
{
    public partial class TaskPanel : UserControl
    {
        public WorkerTask Task { get; private set; }

        public string Filename
        {
            get
            {
                return filename;
            }
            set
            {
                filename = value;

                if (lblFilename.Text != filename)
                {
                    lblFilename.Text = filename;
                }
            }
        }

        private string filename;

        public int Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = value;

                if (pbProgress.Value != progress)
                {
                    pbProgress.Value = progress;
                }
            }
        }

        private int progress;

        public bool ProgressVisible
        {
            get
            {
                return progressVisible;
            }
            set
            {
                progressVisible = value;

                if (pbProgress.Visible != progressVisible)
                {
                    pbProgress.Visible = progressVisible;
                }
            }
        }

        private bool progressVisible;

        public Image ThumbnailImage { get; private set; }
        public Size ThumbnailSize { get; private set; }
        public string ThumbnailSourceFilePath { get; private set; }

        public new event MouseEventHandler MouseDown
        {
            add
            {
                base.MouseDown += value;
                lblFilename.MouseDown += value;
                pThumbnail.MouseDown += value;
                pbThumbnail.MouseDown += value;
                pbProgress.MouseDown += value;
            }
            remove
            {
                base.MouseDown -= value;
                lblFilename.MouseDown -= value;
                pThumbnail.MouseDown -= value;
                pbThumbnail.MouseDown -= value;
                pbProgress.MouseDown -= value;
            }
        }

        public new event MouseEventHandler MouseUp
        {
            add
            {
                base.MouseUp += value;
                lblFilename.MouseUp += value;
                pThumbnail.MouseUp += value;
                pbThumbnail.MouseUp += value;
                pbProgress.MouseUp += value;
            }
            remove
            {
                base.MouseUp -= value;
                lblFilename.MouseUp -= value;
                pThumbnail.MouseUp -= value;
                pbThumbnail.MouseUp -= value;
                pbProgress.MouseUp -= value;
            }
        }

        public TaskPanel(WorkerTask task)
        {
            InitializeComponent();

            Task = task;
            UpdateFilename();
        }

        public void ChangeThumbnailSize(Size size)
        {
            ThumbnailSize = size;
            Size = new Size(pThumbnail.Padding.Horizontal + ThumbnailSize.Width, pThumbnail.Top + pThumbnail.Padding.Vertical + ThumbnailSize.Height);
            UpdateThumbnail();
        }

        public void UpdateFilename()
        {
            Filename = Task.Info.FileName;
        }

        public void UpdateThumbnail()
        {
            ClearThumbnail();

            if (!ThumbnailSize.IsEmpty && Task.Info != null)
            {
                string filePath = Task.Info.FilePath;

                using (Image img = ImageHelpers.LoadImage(filePath))
                {
                    if (img != null)
                    {
                        ThumbnailSourceFilePath = filePath;

                        ThumbnailImage = ImageHelpers.CreateThumbnail(img, ThumbnailSize.Width, ThumbnailSize.Height);
                        pbThumbnail.Image = ThumbnailImage;
                    }
                }
            }
        }

        public void ClearThumbnail()
        {
            pbThumbnail.Image = null;

            if (ThumbnailImage != null)
            {
                ThumbnailImage.Dispose();
                ThumbnailImage = null;
            }
        }

        public void UpdateProgress()
        {
            Progress = (int)Task.Info.Progress.Percentage;
        }
    }
}
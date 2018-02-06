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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class StickerForm : Form
    {
        public StickerForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            toolStrip2.Renderer = new CustomToolStripProfessionalRenderer();
            ilvStickers.SetRenderer(new StickerImageListViewRenderer());
            ilvStickers.ThumbnailSize = new Size(64, 64);
            ilvStickers.Colors.SelectedColor1 = Color.Transparent;
            ilvStickers.Colors.SelectedColor2 = Color.FromArgb(252, 221, 132);
            ilvStickers.Colors.HoverColor1 = Color.Transparent;
            ilvStickers.Colors.HoverColor2 = Color.FromArgb(252, 221, 132);
            ilvStickers.Items.AddRange(Directory.GetFiles("blobs", "*.png"));
            toolStripComboBox1.SelectedIndex = 0;
        }
    }
}
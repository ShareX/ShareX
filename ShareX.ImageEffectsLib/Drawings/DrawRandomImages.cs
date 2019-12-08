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

using ShareX.HelpersLib;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace ShareX.ImageEffectsLib
{
    [Description("Random images")]
    public class DrawRandomImages : ImageEffect
    {
        [DefaultValue(""), Editor(typeof(DirectoryNameEditor), typeof(UITypeEditor))]
        public string ImageFolder { get; set; }

        private int imageCount;

        [DefaultValue(1)]
        public int ImageCount
        {
            get
            {
                return imageCount;
            }
            set
            {
                imageCount = value.Clamp(1, 1000);
            }
        }

        [DefaultValue(false)]
        public bool RandomSize { get; set; }

        [DefaultValue(64)]
        public int RandomSizeMin { get; set; }

        [DefaultValue(128)]
        public int RandomSizeMax { get; set; }

        [DefaultValue(false)]
        public bool RandomRotate { get; set; }

        [DefaultValue(0)]
        public int RandomRotateMin { get; set; }

        [DefaultValue(360)]
        public int RandomRotateMax { get; set; }

        public DrawRandomImages()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Image Apply(Image img)
        {
            string imageFolder = Helpers.ExpandFolderVariables(ImageFolder);

            if (!string.IsNullOrEmpty(imageFolder) && Directory.Exists(imageFolder))
            {
                string[] files = Helpers.GetFilesByExtensions(imageFolder, ".png", ".jpg").ToArray();

                if (files.Length > 0)
                {
                    using (Graphics g = Graphics.FromImage(img))
                    using (ImageFilesCache imageCache = new ImageFilesCache())
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        for (int i = 0; i < ImageCount; i++)
                        {
                            string file = MathHelpers.RandomPick(files);

                            Image img2 = imageCache.GetImage(file);

                            if (img2 != null)
                            {
                                DrawImage(img, img2, g);
                            }
                        }
                    }
                }
            }

            return img;
        }

        private void DrawImage(Image img, Image img2, Graphics g)
        {
            int xOffset = img.Width - img2.Width - 1;
            int yOffset = img.Height - img2.Height - 1;
            int width, height;

            if (RandomSize)
            {
                width = height = MathHelpers.Random(Math.Min(RandomSizeMin, RandomSizeMax), Math.Max(RandomSizeMin, RandomSizeMax));
            }
            else
            {
                width = img2.Width;
                height = img2.Height;
            }

            if (width < 1 || height < 1)
            {
                return;
            }

            Rectangle rect = new Rectangle(MathHelpers.Random(Math.Min(0, xOffset), Math.Max(0, xOffset)),
                MathHelpers.Random(Math.Min(0, yOffset), Math.Max(0, yOffset)), width, height);

            if (RandomRotate)
            {
                float moveX = rect.X + (rect.Width / 2f);
                float moveY = rect.Y + (rect.Height / 2f);
                g.TranslateTransform(moveX, moveY);
                g.RotateTransform(MathHelpers.Random(Math.Min(RandomRotateMin, RandomRotateMax), Math.Max(RandomRotateMin, RandomRotateMax)));
                g.TranslateTransform(-moveX, -moveY);
            }

            g.DrawImage(img2, rect);

            if (RandomRotate)
            {
                g.ResetTransform();
            }
        }
    }
}
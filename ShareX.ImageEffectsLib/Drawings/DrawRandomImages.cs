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
        public bool RandomRotate { get; set; }

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
                        g.SetHighQuality();

                        for (int i = 0; i < ImageCount; i++)
                        {
                            string randomFile = MathHelpers.RandomPick(files);

                            Image img2 = imageCache.GetImage(randomFile);

                            if (img2 != null)
                            {
                                int widthOffset = img.Width - img2.Width - 1;
                                int heightOffset = img.Height - img2.Height - 1;
                                Rectangle rect = new Rectangle(MathHelpers.Random(Math.Min(0, widthOffset), Math.Max(0, widthOffset)),
                                    MathHelpers.Random(Math.Min(0, heightOffset), Math.Max(0, heightOffset)), img2.Width, img2.Height);

                                if (RandomRotate)
                                {
                                    float moveX = rect.X + (rect.Width / 2f);
                                    float moveY = rect.Y + (rect.Height / 2f);
                                    g.TranslateTransform(moveX, moveY);
                                    g.RotateTransform(MathHelpers.Random(0, 360));
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
                }
            }

            return img;
        }
    }
}
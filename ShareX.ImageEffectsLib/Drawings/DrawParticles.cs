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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ShareX.ImageEffectsLib
{
    [Description("Particles")]
    public class DrawParticles : ImageEffect
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
        public bool Background { get; set; }

        [DefaultValue(false)]
        public bool RandomSize { get; set; }

        [DefaultValue(64)]
        public int RandomSizeMin { get; set; }

        [DefaultValue(128)]
        public int RandomSizeMax { get; set; }

        [DefaultValue(false)]
        public bool RandomAngle { get; set; }

        [DefaultValue(0)]
        public int RandomAngleMin { get; set; }

        [DefaultValue(360)]
        public int RandomAngleMax { get; set; }

        [DefaultValue(false)]
        public bool RandomOpacity { get; set; }

        [DefaultValue(0)]
        public int RandomOpacityMin { get; set; }

        [DefaultValue(100)]
        public int RandomOpacityMax { get; set; }

        [DefaultValue(false)]
        public bool NoOverlap { get; set; }

        [DefaultValue(0)]
        public int NoOverlapOffset { get; set; }

        [DefaultValue(false)]
        public bool EdgeOverlap { get; set; }

        private List<Rectangle> imageRectangles = new List<Rectangle>();

        public DrawParticles()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            if (Background)
            {
                Bitmap result = bmp.CreateEmptyBitmap();

                DrawParticlesFromFolder(result, ImageFolder);

                using (Graphics g = Graphics.FromImage(result))
                {
                    g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
                }

                bmp.Dispose();

                return result;
            }
            else
            {
                DrawParticlesFromFolder(bmp, ImageFolder);

                return bmp;
            }
        }

        private void DrawParticlesFromFolder(Bitmap bmp, string imageFolder)
        {
            imageFolder = FileHelpers.ExpandFolderVariables(imageFolder, true);

            if (!string.IsNullOrEmpty(imageFolder) && Directory.Exists(imageFolder))
            {
                string[] files = FileHelpers.GetFilesByExtensions(imageFolder, ".png", ".jpg").ToArray();

                if (files.Length > 0)
                {
                    imageRectangles.Clear();

                    using (Graphics g = Graphics.FromImage(bmp))
                    using (ImageFilesCache imageCache = new ImageFilesCache())
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        for (int i = 0; i < ImageCount; i++)
                        {
                            string file = RandomFast.Pick(files);
                            Bitmap bmpCached = imageCache.GetImage(file);

                            if (bmpCached != null)
                            {
                                DrawImage(bmp, bmpCached, g);
                            }
                        }
                    }
                }
            }
        }

        private void DrawImage(Image img, Image img2, Graphics g)
        {
            int width, height;

            if (RandomSize)
            {
                int size = RandomFast.Next(Math.Min(RandomSizeMin, RandomSizeMax), Math.Max(RandomSizeMin, RandomSizeMax));
                width = size;
                height = size;

                if (img2.Width > img2.Height)
                {
                    height = (int)Math.Round(size * ((double)img2.Height / img2.Width));
                }
                else if (img2.Width < img2.Height)
                {
                    width = (int)Math.Round(size * ((double)img2.Width / img2.Height));
                }
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

            int minOffsetX = EdgeOverlap ? -width + 1 : 0;
            int minOffsetY = EdgeOverlap ? -height + 1 : 0;
            int maxOffsetX = img.Width - (EdgeOverlap ? 0 : width) - 1;
            int maxOffsetY = img.Height - (EdgeOverlap ? 0 : height) - 1;

            Rectangle rect, overlapRect;
            int attemptCount = 0;

            do
            {
                attemptCount++;

                if (attemptCount > 1000)
                {
                    return;
                }

                int x = RandomFast.Next(Math.Min(minOffsetX, maxOffsetX), Math.Max(minOffsetX, maxOffsetX));
                int y = RandomFast.Next(Math.Min(minOffsetY, maxOffsetY), Math.Max(minOffsetY, maxOffsetY));
                rect = new Rectangle(x, y, width, height);

                overlapRect = rect.Offset(NoOverlapOffset);
            } while (NoOverlap && imageRectangles.Any(x => x.IntersectsWith(overlapRect)));

            imageRectangles.Add(rect);

            if (RandomAngle)
            {
                float moveX = rect.X + (rect.Width / 2f);
                float moveY = rect.Y + (rect.Height / 2f);
                int rotate = RandomFast.Next(Math.Min(RandomAngleMin, RandomAngleMax), Math.Max(RandomAngleMin, RandomAngleMax));

                g.TranslateTransform(moveX, moveY);
                g.RotateTransform(rotate);
                g.TranslateTransform(-moveX, -moveY);
            }

            g.PixelOffsetMode = PixelOffsetMode.Half;

            if (RandomOpacity)
            {
                float opacity = RandomFast.Next(Math.Min(RandomOpacityMin, RandomOpacityMax), Math.Max(RandomOpacityMin, RandomOpacityMax)).Clamp(0, 100) / 100f;

                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = opacity;
                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(matrix);
                    g.DrawImage(img2, rect, 0, 0, img2.Width, img2.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            else
            {
                g.DrawImage(img2, rect);
            }

            if (RandomAngle)
            {
                g.ResetTransform();
            }

            g.PixelOffsetMode = PixelOffsetMode.Default;
        }

        protected override string GetSummary()
        {
            if (!string.IsNullOrEmpty(ImageFolder))
            {
                return FileHelpers.GetFileNameSafe(ImageFolder);
            }

            return null;
        }
    }
}
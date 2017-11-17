#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.Drawing.Drawing2D;

namespace ShareX.ScreenCaptureLib
{
    public abstract class BaseEffectShape : BaseShape
    {
        public override ShapeCategory ShapeCategory { get; } = ShapeCategory.Effect;

        public abstract string OverlayText { get; }

        private bool isEffectCaching, cachePending, cacheClearingPending;
        private Image cachedEffect;

        public abstract void ApplyEffect(Bitmap bmp);

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (cacheClearingPending)
            {
                ClearCache();
            }

            if (cachePending)
            {
                CacheEffect();
            }
        }

        public virtual void OnDraw(Graphics g)
        {
            if (!cacheClearingPending && isEffectCaching)
            {
                OnDrawOverlay(g, "Processing...");
            }
            else if (!cacheClearingPending && cachedEffect != null)
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(cachedEffect, RectangleInsideCanvas);
                g.InterpolationMode = InterpolationMode.Bilinear;
            }
            else
            {
                OnDrawOverlay(g);
            }
        }

        public virtual void OnDrawOverlay(Graphics g)
        {
            OnDrawOverlay(g, OverlayText);
        }

        public void OnDrawOverlay(Graphics g, string overlayText)
        {
            using (Brush brush = new SolidBrush(Color.FromArgb(150, Color.Black)))
            {
                g.FillRectangle(brush, Rectangle);
            }

            g.DrawCornerLines(Rectangle.Offset(1), Pens.White, 25);

            using (Font font = new Font("Verdana", 12))
            {
                Size textSize = g.MeasureString(overlayText, font).ToSize();

                if (Rectangle.Width > textSize.Width && Rectangle.Height > textSize.Height)
                {
                    using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(overlayText, font, Brushes.White, Rectangle, sf);
                    }
                }
            }
        }

        public virtual void OnDrawFinal(Graphics g, Bitmap bmp)
        {
            Rectangle rect = Rectangle.Intersect(new Rectangle(0, 0, bmp.Width, bmp.Height), Rectangle);

            if (!rect.IsEmpty)
            {
                using (Bitmap croppedImage = ImageHelpers.CropBitmap(bmp, rect))
                {
                    ApplyEffect(croppedImage);

                    g.DrawImage(croppedImage, rect);
                }
            }
        }

        public override void OnCreated()
        {
            CacheEffect();
        }

        public override void OnMoving()
        {
            ClearCache();
        }

        public override void OnMoved()
        {
            CacheEffect();
        }

        public override void OnResizing()
        {
            ClearCache();
        }

        public override void OnResized()
        {
            CacheEffect();
        }

        private void CacheEffect()
        {
            if (!isEffectCaching)
            {
                cachePending = false;

                ClearCache();

                if (IsInsideCanvas)
                {
                    isEffectCaching = true;

                    cachedEffect = Manager.CropImage(RectangleInsideCanvas);

                    TaskEx.Run(() =>
                    {
                        ApplyEffect((Bitmap)cachedEffect);

                        isEffectCaching = false;
                    });
                }
            }
            else
            {
                cachePending = true;
            }
        }

        private void ClearCache()
        {
            if (isEffectCaching)
            {
                cacheClearingPending = true;
            }
            else if (cachedEffect != null)
            {
                cachedEffect.Dispose();
                cachedEffect = null;
                cacheClearingPending = false;
            }
        }

        public override void Dispose()
        {
            ClearCache();
        }
    }
}
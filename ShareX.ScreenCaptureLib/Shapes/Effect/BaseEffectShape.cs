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

using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShareX.ScreenCaptureLib
{
    public abstract class BaseEffectShape : BaseShape
    {
        public override ShapeCategory ShapeCategory { get; } = ShapeCategory.Effect;

        private Image cachedEffect;

        public abstract void ApplyEffect(Bitmap bmp);

        public virtual void OnDraw(Graphics g)
        {
            if (cachedEffect != null)
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(cachedEffect, Rectangle);
                g.InterpolationMode = InterpolationMode.Bilinear;
            }
            else
            {
                OnDrawOverlay(g);
            }
        }

        public abstract void OnDrawOverlay(Graphics g);

        public virtual void OnDrawFinal(Graphics g, Bitmap bmp)
        {
            OnDraw(g);
        }

        public override void OnCreated()
        {
            CacheEffect();
        }

        public override void OnMoving()
        {
            Dispose();
        }

        public override void OnMoved()
        {
            CacheEffect();
        }

        public override void OnResizing()
        {
            Dispose();
        }

        public override void OnResized()
        {
            CacheEffect();
        }

        private void CacheEffect()
        {
            Dispose();
            cachedEffect = Manager.CropImage(Rectangle);
            ApplyEffect((Bitmap)cachedEffect);
        }

        public override void Dispose()
        {
            if (cachedEffect != null)
            {
                cachedEffect.Dispose();
                cachedEffect = null;
            }
        }
    }
}
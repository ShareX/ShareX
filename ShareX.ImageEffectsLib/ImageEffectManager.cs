#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ImageEffectsLib
{
    public static class ImageEffectManager
    {
        public static Image ApplyEffects(Image img, List<ImageEffect> imageEffects)
        {
            Image result = (Image)img.Clone();
            ((Bitmap)result).SetResolution(96f, 96f);

            if (imageEffects != null && imageEffects.Count > 0)
            {
                foreach (ImageEffect imageEffect in imageEffects.Where(x => x.Enabled))
                {
                    result = imageEffect.Apply(result);

                    if (result == null)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        public static List<ImageEffect> GetDefaultImageEffects()
        {
            List<ImageEffect> imageEffects = new List<ImageEffect>();

            Canvas canvas = new Canvas();
            canvas.Margin = new Padding(0, 0, 0, 30);
            imageEffects.Add(canvas);

            DrawText text = new DrawText();
            text.Offset = new Point(0, 0);
            text.UseCustomGradient = true;
            imageEffects.Add(text);

            return imageEffects;
        }
    }
}
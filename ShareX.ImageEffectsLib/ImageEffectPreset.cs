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

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ImageEffectsLib
{
    public class ImageEffectPreset
    {
        public string Name { get; set; } = "";

        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.Auto)]
        public List<ImageEffect> Effects { get; set; } = new List<ImageEffect>();

        public Bitmap ApplyEffects(Bitmap bmp)
        {
            Bitmap result = (Bitmap)bmp.Clone();
            result.SetResolution(96f, 96f);

            if (Effects != null && Effects.Count > 0)
            {
                foreach (ImageEffect effect in Effects.Where(x => x.Enabled))
                {
                    result = effect.Apply(result);

                    if (result == null)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return Name;
            }

            return "Name";
        }

        public static ImageEffectPreset GetDefaultPreset()
        {
            ImageEffectPreset preset = new ImageEffectPreset();

            Canvas canvas = new Canvas();
            canvas.Margin = new Padding(0, 0, 0, 30);
            preset.Effects.Add(canvas);

            DrawText text = new DrawText();
            text.Offset = new Point(0, 0);
            text.UseGradient = true;
            preset.Effects.Add(text);

            return preset;
        }
    }
}
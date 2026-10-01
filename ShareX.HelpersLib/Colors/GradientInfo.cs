#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace ShareX.HelpersLib
{
    /// <summary>A linear gradient. Drawn with SkiaSharp; the WinForms application gets a GDI+ brush from ShareX.HelpersLib.Windows.</summary>
    public class GradientInfo
    {
        [DefaultValue(GradientDirection.Vertical)]
        public GradientDirection Type { get; set; }

        public List<GradientStop> Colors { get; set; }

        [JsonIgnore]
        public bool IsValid => Colors != null && Colors.Count > 0;

        [JsonIgnore]
        public bool IsVisible => IsValid && Colors.Any(x => x.Color.A > 0);

        [JsonIgnore]
        public bool IsTransparent => IsValid && Colors.Any(x => x.Color.IsTransparent());

        public GradientInfo() : this(GradientDirection.Vertical)
        {
        }

        public GradientInfo(GradientDirection type)
        {
            Type = type;
            Colors = new List<GradientStop>();
        }

        public GradientInfo(GradientDirection type, params GradientStop[] colors) : this(type)
        {
            Colors = colors.ToList();
        }

        public GradientInfo(GradientDirection type, params Color[] colors) : this(type)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                Colors.Add(new GradientStop(colors[i], (int)Math.Round(100f / (colors.Length - 1) * i)));
            }
        }

        public GradientInfo(params GradientStop[] colors) : this(GradientDirection.Vertical, colors)
        {
        }

        public GradientInfo(params Color[] colors) : this(GradientDirection.Vertical, colors)
        {
        }

        public void Clear()
        {
            Colors.Clear();
        }

        public void Sort()
        {
            Colors.Sort((x, y) => x.Location.CompareTo(y.Location));
        }

        public void Reverse()
        {
            Colors.Reverse();

            foreach (GradientStop color in Colors)
            {
                color.Location = 100 - color.Location;
            }
        }

        /// <summary>Stops sorted by position, with the first and last colours extended to 0 and 100 like GDI+ requires.</summary>
        public List<GradientStop> GetNormalizedStops()
        {
            List<GradientStop> colors = new List<GradientStop>(Colors.OrderBy(x => x.Location));

            if (!colors.Any(x => x.Location == 0))
            {
                colors.Insert(0, new GradientStop(colors[0].Color, 0f));
            }

            if (!colors.Any(x => x.Location == 100))
            {
                colors.Add(new GradientStop(colors[colors.Count - 1].Color, 100f));
            }

            return colors;
        }

        /// <summary>The gradient as a SkiaSharp shader over the rectangle.</summary>
        public SKShader CreateShader(SKRect rect)
        {
            List<GradientStop> stops = GetNormalizedStops();
            (SKPoint start, SKPoint end) = Type switch
            {
                GradientDirection.Horizontal => (new SKPoint(rect.Left, rect.Top), new SKPoint(rect.Right, rect.Top)),
                GradientDirection.ForwardDiagonal => (new SKPoint(rect.Left, rect.Top), new SKPoint(rect.Right, rect.Bottom)),
                GradientDirection.BackwardDiagonal => (new SKPoint(rect.Right, rect.Top), new SKPoint(rect.Left, rect.Bottom)),
                _ => (new SKPoint(rect.Left, rect.Top), new SKPoint(rect.Left, rect.Bottom))
            };

            return SKShader.CreateLinearGradient(start, end, stops.Select(x => new SKColor(x.Color.R, x.Color.G, x.Color.B, x.Color.A)).ToArray(),
                stops.Select(x => x.Location / 100f).ToArray(), SKShaderTileMode.Clamp);
        }

        public void Draw(SKCanvas canvas, SKRect rect)
        {
            if (IsValid)
            {
                using SKShader shader = CreateShader(new SKRect(0, 0, rect.Width, rect.Height));
                using SKPaint paint = new SKPaint { Shader = shader };
                canvas.Save();
                canvas.Translate(rect.Left, rect.Top);
                canvas.DrawRect(0, 0, rect.Width, rect.Height, paint);
                canvas.Restore();
            }
        }

        public override string ToString()
        {
            return "Gradient";
        }
    }
}

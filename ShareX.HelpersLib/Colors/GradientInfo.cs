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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using SkiaSharp;
using System.Linq;

namespace ShareX.HelpersLib
{
    public class GradientInfo
    {
        [DefaultValue(ImageGradientMode.Vertical)]
        public ImageGradientMode Type { get; set; }

        public List<GradientStop> Colors { get; set; }

        [JsonIgnore]
        public bool IsValid => Colors != null && Colors.Count > 0;

        [JsonIgnore]
        public bool IsVisible => IsValid && Colors.Any(x => x.Color.A > 0);

        [JsonIgnore]
        public bool IsTransparent => IsValid && Colors.Any(x => x.Color.IsTransparent());

        public GradientInfo() : this(ImageGradientMode.Vertical)
        {
        }

        public GradientInfo(ImageGradientMode type)
        {
            Type = type;
            Colors = new List<GradientStop>();
        }

        public GradientInfo(ImageGradientMode type, params GradientStop[] colors) : this(type)
        {
            Colors = colors.ToList();
        }

        public GradientInfo(ImageGradientMode type, params Color[] colors) : this(type)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                Colors.Add(new GradientStop(colors[i], colors.Length == 1 ? 0 : (int)Math.Round(100f / (colors.Length - 1) * i)));
            }
        }

        public GradientInfo(params GradientStop[] colors) : this(ImageGradientMode.Vertical, colors)
        {
        }

        public GradientInfo(params Color[] colors) : this(ImageGradientMode.Vertical, colors)
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

        public SKPaint GetGradientBrush(Rectangle rectangle) => this.GetSkiaPaint(rectangle);

        public void Draw(SKCanvas canvas, Rectangle rectangle)
        {
            if (!IsValid) return;
            using SKPaint paint = this.GetSkiaPaint(rectangle);
            canvas.DrawRect(rectangle.ToSKRect(), paint);
        }

        public void Draw(SKBitmap image) => this.DrawSkia(image);

        public SKBitmap CreateGradientPreview(int width, int height, bool border = false, bool checkers = false)
        {
            SKBitmap bitmap = SkiaImageHelpers.CreateBitmap(width, height);
            Rectangle rectangle = new(0, 0, width, height);
            using SKCanvas canvas = new(bitmap);
            if (checkers && IsTransparent)
            {
                using SKBitmap pattern = SkiaImageHelpers.CreateCheckerPattern();
                using SKPaint paint = SkiaDrawing.Texture(pattern);
                canvas.DrawRect(rectangle.ToSKRect(), paint);
            }
            Draw(canvas, rectangle);
            if (border)
            {
                using SKPaint paint = SkiaDrawing.Stroke(Color.Black);
                canvas.DrawRectangleProper(paint, rectangle);
            }
            return bitmap;
        }

        public override string ToString()
        {
            return "Gradient";
        }
    }
}
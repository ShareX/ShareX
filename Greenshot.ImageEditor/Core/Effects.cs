/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using GreenshotPlugin;
using GreenshotPlugin.Core;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;

namespace Greenshot.Core
{
    /// <summary>
    /// Interface to describe an effect
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// Apply this IEffect to the supplied sourceImage.
        /// In the process of applying the supplied matrix will be modified to represent the changes.
        /// </summary>
        /// <param name="sourceImage">Image to apply the effect to</param>
        /// <param name="matrix">Matrix with the modifications like rotate, translate etc. this can be used to calculate the new location of elements on a canvas</param>
        /// <returns>new image with applied effect</returns>
        Image Apply(Image sourceImage, Matrix matrix);

        /// <summary>
        /// Reset all values to their defaults
        /// </summary>
        void Reset();
    }

    /// <summary>
    /// DropShadowEffect
    /// </summary>
    [TypeConverter(typeof(EffectConverter))]
    public class DropShadowEffect : IEffect
    {
        public DropShadowEffect()
        {
            Reset();
        }

        public float Darkness
        {
            get;
            set;
        }
        public int ShadowSize
        {
            get;
            set;
        }
        public Point ShadowOffset
        {
            get;
            set;
        }

        public virtual void Reset()
        {
            Darkness = 0.6f;
            ShadowSize = 7;
            ShadowOffset = new Point(-1, -1);
        }

        public virtual Image Apply(Image sourceImage, Matrix matrix)
        {
            return ImageHelper.CreateShadow(sourceImage, Darkness, ShadowSize, ShadowOffset, matrix, PixelFormat.Format32bppArgb);
        }
    }

    /// <summary>
    /// TornEdgeEffect extends on DropShadowEffect
    /// </summary>
    [TypeConverter(typeof(EffectConverter))]
    public class TornEdgeEffect : DropShadowEffect
    {
        public TornEdgeEffect()
        {
            Reset();
        }

        public int ToothHeight
        {
            get;
            set;
        }
        public int HorizontalToothRange
        {
            get;
            set;
        }
        public int VerticalToothRange
        {
            get;
            set;
        }
        public bool[] Edges
        {
            get;
            set;
        }
        public bool GenerateShadow
        {
            get;
            set;
        }

        public override void Reset()
        {
            base.Reset();
            ShadowSize = 7;
            ToothHeight = 12;
            HorizontalToothRange = 20;
            VerticalToothRange = 20;
            Edges = new[] { true, true, true, true };
            GenerateShadow = true;
        }

        public override Image Apply(Image sourceImage, Matrix matrix)
        {
            Image tmpTornImage = ImageHelper.CreateTornEdge(sourceImage, ToothHeight, HorizontalToothRange, VerticalToothRange, Edges);
            if (GenerateShadow)
            {
                using (tmpTornImage)
                {
                    return ImageHelper.CreateShadow(tmpTornImage, Darkness, ShadowSize, ShadowOffset, matrix, PixelFormat.Format32bppArgb);
                }
            }
            return tmpTornImage;
        }
    }

    /// <summary>
    /// GrayscaleEffect
    /// </summary>
    public class GrayscaleEffect : IEffect
    {
        public Image Apply(Image sourceImage, Matrix matrix)
        {
            return ImageHelper.CreateGrayscale(sourceImage);
        }

        public void Reset()
        {
            // No settings to reset
        }
    }

    /// <summary>
    /// MonochromeEffect
    /// </summary>
    public class MonochromeEffect : IEffect
    {
        private readonly byte _threshold;

        /// <param name="threshold">Threshold for monochrome filter (0 - 255), lower value means less black</param>
        public MonochromeEffect(byte threshold)
        {
            _threshold = threshold;
        }

        public void Reset()
        {
            // Modify the threshold to have a default, which is reset here
        }

        public Image Apply(Image sourceImage, Matrix matrix)
        {
            return ImageHelper.CreateMonochrome(sourceImage, _threshold);
        }
    }

    /// <summary>
    /// AdjustEffect
    /// </summary>
    public class AdjustEffect : IEffect
    {
        public AdjustEffect()
        {
            Reset();
        }

        public float Contrast
        {
            get;
            set;
        }
        public float Brightness
        {
            get;
            set;
        }
        public float Gamma
        {
            get;
            set;
        }

        public void Reset()
        {
            Contrast = 1f;
            Brightness = 1f;
            Gamma = 1f;
        }

        public Image Apply(Image sourceImage, Matrix matrix)
        {
            return ImageHelper.Adjust(sourceImage, Brightness, Contrast, Gamma);
        }
    }

    /// <summary>
    /// ReduceColorsEffect
    /// </summary>
    public class ReduceColorsEffect : IEffect
    {
        public ReduceColorsEffect()
        {
            Reset();
        }

        public int Colors
        {
            get;
            set;
        }

        public void Reset()
        {
            Colors = 256;
        }

        public Image Apply(Image sourceImage, Matrix matrix)
        {
            using (WuQuantizer quantizer = new WuQuantizer((Bitmap)sourceImage))
            {
                int colorCount = quantizer.GetColorCount();
                if (colorCount > Colors)
                {
                    try
                    {
                        return quantizer.GetQuantizedImage(Colors);
                    }
                    catch (Exception e)
                    {
                        LOG.Warn("Error occurred while Quantizing the image, ignoring and using original. Error: ", e);
                    }
                }
            }
            return null;
        }
    }

    /// <summary>
    /// InvertEffect
    /// </summary>
    public class InvertEffect : IEffect
    {
        public Image Apply(Image sourceImage, Matrix matrix)
        {
            return ImageHelper.CreateNegative(sourceImage);
        }

        public void Reset()
        {
            // No settings to reset
        }
    }

    /// <summary>
    /// BorderEffect
    /// </summary>
    public class BorderEffect : IEffect
    {
        public BorderEffect()
        {
            Reset();
        }

        public Color Color
        {
            get;
            set;
        }
        public int Width
        {
            get;
            set;
        }

        public void Reset()
        {
            Width = 2;
            Color = Color.Black;
        }

        public Image Apply(Image sourceImage, Matrix matrix)
        {
            return ImageHelper.CreateBorder(sourceImage, Width, Color, sourceImage.PixelFormat, matrix);
        }
    }

    /// <summary>
    /// RotateEffect
    /// </summary>
    public class RotateEffect : IEffect
    {
        public RotateEffect(int angle)
        {
            Angle = angle;
        }

        public int Angle
        {
            get;
            set;
        }

        public void Reset()
        {
            // Angle doesn't have a default value
        }

        public Image Apply(Image sourceImage, Matrix matrix)
        {
            RotateFlipType flipType;
            if (Angle == 90)
            {
                matrix.Rotate(90, MatrixOrder.Append);
                matrix.Translate(sourceImage.Height, 0, MatrixOrder.Append);
                flipType = RotateFlipType.Rotate90FlipNone;
            }
            else if (Angle == -90 || Angle == 270)
            {
                flipType = RotateFlipType.Rotate270FlipNone;
                matrix.Rotate(-90, MatrixOrder.Append);
                matrix.Translate(0, sourceImage.Width, MatrixOrder.Append);
            }
            else
            {
                throw new NotSupportedException("Currently only an angle of 90 or -90 (270) is supported.");
            }
            return ImageHelper.RotateFlip(sourceImage, flipType);
        }
    }

    /// <summary>
    /// ResizeEffect
    /// </summary>
    public class ResizeEffect : IEffect
    {
        public ResizeEffect(int width, int height, bool maintainAspectRatio)
        {
            Width = width;
            Height = height;
            MaintainAspectRatio = maintainAspectRatio;
        }

        public int Width
        {
            get;
            set;
        }
        public int Height
        {
            get;
            set;
        }
        public bool MaintainAspectRatio
        {
            get;
            set;
        }

        public void Reset()
        {
            // values don't have a default value
        }

        public Image Apply(Image sourceImage, Matrix matrix)
        {
            return ImageHelper.ResizeImage(sourceImage, MaintainAspectRatio, Width, Height, matrix);
        }
    }

    /// <summary>
    /// ResizeCanvasEffect
    /// </summary>
    public class ResizeCanvasEffect : IEffect
    {
        public ResizeCanvasEffect(int left, int right, int top, int bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
            BackgroundColor = Color.Empty;  // Uses the default background color depending on the format
        }

        public int Left
        {
            get;
            set;
        }
        public int Right
        {
            get;
            set;
        }
        public int Top
        {
            get;
            set;
        }
        public int Bottom
        {
            get;
            set;
        }
        public Color BackgroundColor
        {
            get;
            set;
        }

        public void Reset()
        {
            // values don't have a default value
        }

        public Image Apply(Image sourceImage, Matrix matrix)
        {
            return ImageHelper.ResizeCanvas(sourceImage, BackgroundColor, Left, Right, Top, Bottom, matrix);
        }
    }

    public class EffectConverter : TypeConverter
    {
        // Fix to prevent BUG-1753
        private readonly NumberFormatInfo numberFormatInfo = new NumberFormatInfo();

        public EffectConverter()
        {
            numberFormatInfo.NumberDecimalSeparator = ".";
            numberFormatInfo.NumberGroupSeparator = ",";
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            if (destinationType == typeof(DropShadowEffect))
            {
                return true;
            }
            if (destinationType == typeof(TornEdgeEffect))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            // to string
            if (destinationType == typeof(string))
            {
                StringBuilder sb = new StringBuilder();
                if (value.GetType() == typeof(DropShadowEffect))
                {
                    DropShadowEffect effect = value as DropShadowEffect;
                    RetrieveDropShadowEffectValues(effect, sb);
                    return sb.ToString();
                }
                if (value.GetType() == typeof(TornEdgeEffect))
                {
                    TornEdgeEffect effect = value as TornEdgeEffect;
                    RetrieveDropShadowEffectValues(effect, sb);
                    sb.Append("|");
                    RetrieveTornEdgeEffectValues(effect, sb);
                    return sb.ToString();
                }
            }
            // from string
            if (value is string)
            {
                string settings = value as string;
                if (destinationType == typeof(DropShadowEffect))
                {
                    DropShadowEffect effect = new DropShadowEffect();
                    ApplyDropShadowEffectValues(settings, effect);
                    return effect;
                }
                if (destinationType == typeof(TornEdgeEffect))
                {
                    TornEdgeEffect effect = new TornEdgeEffect();
                    ApplyDropShadowEffectValues(settings, effect);
                    ApplyTornEdgeEffectValues(settings, effect);
                    return effect;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value != null && value is string)
            {
                string settings = value as string;
                if (settings.Contains("ToothHeight"))
                {
                    return ConvertTo(context, culture, value, typeof(TornEdgeEffect));
                }
                return ConvertTo(context, culture, value, typeof(DropShadowEffect));
            }
            return base.ConvertFrom(context, culture, value);
        }

        private void ApplyDropShadowEffectValues(string valuesString, DropShadowEffect effect)
        {
            string[] values = valuesString.Split('|');
            foreach (string nameValuePair in values)
            {
                string[] pair = nameValuePair.Split(':');
                switch (pair[0])
                {
                    case "Darkness":
                        float darkness;
                        // Fix to prevent BUG-1753
                        if (pair[1] != null && float.TryParse(pair[1], NumberStyles.Float, numberFormatInfo, out darkness))
                        {
                            if (darkness <= 1.0)
                            {
                                effect.Darkness = darkness;
                            }
                        }
                        break;
                    case "ShadowSize":
                        int shadowSize;
                        if (int.TryParse(pair[1], out shadowSize))
                        {
                            effect.ShadowSize = shadowSize;
                        }
                        break;
                    case "ShadowOffset":
                        Point shadowOffset = new Point();
                        int shadowOffsetX;
                        int shadowOffsetY;
                        string[] coordinates = pair[1].Split(',');
                        if (int.TryParse(coordinates[0], out shadowOffsetX))
                        {
                            shadowOffset.X = shadowOffsetX;
                        }
                        if (int.TryParse(coordinates[1], out shadowOffsetY))
                        {
                            shadowOffset.Y = shadowOffsetY;
                        }
                        effect.ShadowOffset = shadowOffset;
                        break;
                }
            }
        }

        private void ApplyTornEdgeEffectValues(string valuesString, TornEdgeEffect effect)
        {
            string[] values = valuesString.Split('|');
            foreach (string nameValuePair in values)
            {
                string[] pair = nameValuePair.Split(':');
                switch (pair[0])
                {
                    case "GenerateShadow":
                        bool generateShadow;
                        if (bool.TryParse(pair[1], out generateShadow))
                        {
                            effect.GenerateShadow = generateShadow;
                        }
                        break;
                    case "ToothHeight":
                        int toothHeight;
                        if (int.TryParse(pair[1], out toothHeight))
                        {
                            effect.ToothHeight = toothHeight;
                        }
                        break;
                    case "HorizontalToothRange":
                        int horizontalToothRange;
                        if (int.TryParse(pair[1], out horizontalToothRange))
                        {
                            effect.HorizontalToothRange = horizontalToothRange;
                        }
                        break;
                    case "VerticalToothRange":
                        int verticalToothRange;
                        if (int.TryParse(pair[1], out verticalToothRange))
                        {
                            effect.VerticalToothRange = verticalToothRange;
                        }
                        break;
                    case "Edges":
                        string[] edges = pair[1].Split(',');
                        bool edge;
                        if (bool.TryParse(edges[0], out edge))
                        {
                            effect.Edges[0] = edge;
                        }
                        if (bool.TryParse(edges[1], out edge))
                        {
                            effect.Edges[1] = edge;
                        }
                        if (bool.TryParse(edges[2], out edge))
                        {
                            effect.Edges[2] = edge;
                        }
                        if (bool.TryParse(edges[3], out edge))
                        {
                            effect.Edges[3] = edge;
                        }
                        break;
                }
            }
        }

        private void RetrieveDropShadowEffectValues(DropShadowEffect effect, StringBuilder sb)
        {
            // Fix to prevent BUG-1753 is to use the numberFormatInfo
            sb.AppendFormat("Darkness:{0}|ShadowSize:{1}|ShadowOffset:{2},{3}", effect.Darkness.ToString("F2", numberFormatInfo), effect.ShadowSize, effect.ShadowOffset.X, effect.ShadowOffset.Y);
        }

        private void RetrieveTornEdgeEffectValues(TornEdgeEffect effect, StringBuilder sb)
        {
            sb.AppendFormat("GenerateShadow:{0}|ToothHeight:{1}|HorizontalToothRange:{2}|VerticalToothRange:{3}|Edges:{4},{5},{6},{7}", effect.GenerateShadow, effect.ToothHeight, effect.HorizontalToothRange, effect.VerticalToothRange, effect.Edges[0], effect.Edges[1], effect.Edges[2], effect.Edges[3]);
        }
    }
}
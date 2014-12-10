/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
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

using Greenshot.Drawing;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Greenshot.Helpers
{
    /// <summary>
    /// Offers a few helper functions for scaling/aligning an element with another element
    /// </summary>
    public static class ScaleHelper
    {
        [Flags]
        public enum ScaleOptions
        {
            /// <summary>
            /// Default scale behavior.
            /// </summary>
            Default = 0x00,
            /// <summary>
            /// Scale a rectangle in two our four directions, mirrored at it's center coordinates
            /// </summary>
            Centered = 0x01,
            /// <summary>
            /// Scale a rectangle maintaining it's aspect ratio
            /// </summary>
            Rational = 0x02
        }

        /// <summary>
        /// calculates the Size an element must be resized to, in order to fit another element, keeping aspect ratio
        /// </summary>
        /// <param name="currentSize">the size of the element to be resized</param>
        /// <param name="targetSize">the target size of the element</param>
        /// <param name="crop">in case the aspect ratio of currentSize and targetSize differs: shall the scaled size fit into targetSize (i.e. that one of its dimensions is smaller - false) or vice versa (true)</param>
        /// <returns>a new SizeF object indicating the width and height the element should be scaled to</returns>
        public static SizeF GetScaledSize(SizeF currentSize, SizeF targetSize, bool crop)
        {
            float wFactor = targetSize.Width / currentSize.Width;
            float hFactor = targetSize.Height / currentSize.Height;

            float factor = crop ? Math.Max(wFactor, hFactor) : Math.Min(wFactor, hFactor);
            return new SizeF(currentSize.Width * factor, currentSize.Height * factor);
        }

        /// <summary>
        /// calculates the position of an element depending on the desired alignment within a RectangleF
        /// </summary>
        /// <param name="currentRect">the bounds of the element to be aligned</param>
        /// <param name="targetRect">the rectangle reference for aligment of the element</param>
        /// <param name="alignment">the System.Drawing.ContentAlignment value indicating how the element is to be aligned should the width or height differ from targetSize</param>
        /// <returns>a new RectangleF object with Location aligned aligned to targetRect</returns>
        public static RectangleF GetAlignedRectangle(RectangleF currentRect, RectangleF targetRect, ContentAlignment alignment)
        {
            RectangleF newRect = new RectangleF(targetRect.Location, currentRect.Size);
            switch (alignment)
            {
                case ContentAlignment.TopCenter:
                    newRect.X = (targetRect.Width - currentRect.Width) / 2;
                    break;
                case ContentAlignment.TopRight:
                    newRect.X = (targetRect.Width - currentRect.Width);
                    break;
                case ContentAlignment.MiddleLeft:
                    newRect.Y = (targetRect.Height - currentRect.Height) / 2;
                    break;
                case ContentAlignment.MiddleCenter:
                    newRect.Y = (targetRect.Height - currentRect.Height) / 2;
                    newRect.X = (targetRect.Width - currentRect.Width) / 2;
                    break;
                case ContentAlignment.MiddleRight:
                    newRect.Y = (targetRect.Height - currentRect.Height) / 2;
                    newRect.X = (targetRect.Width - currentRect.Width);
                    break;
                case ContentAlignment.BottomLeft:
                    newRect.Y = (targetRect.Height - currentRect.Height);
                    break;
                case ContentAlignment.BottomCenter:
                    newRect.Y = (targetRect.Height - currentRect.Height);
                    newRect.X = (targetRect.Width - currentRect.Width) / 2;
                    break;
                case ContentAlignment.BottomRight:
                    newRect.Y = (targetRect.Height - currentRect.Height);
                    newRect.X = (targetRect.Width - currentRect.Width);
                    break;
            }
            return newRect;
        }

        /// <summary>
        /// calculates the Rectangle an element must be resized an positioned to, in ordder to fit another element, keeping aspect ratio
        /// </summary>
        /// <param name="currentRect">the rectangle of the element to be resized/repositioned</param>
        /// <param name="targetRect">the target size/position of the element</param>
        /// <param name="crop">in case the aspect ratio of currentSize and targetSize differs: shall the scaled size fit into targetSize (i.e. that one of its dimensions is smaller - false) or vice versa (true)</param>
        /// <param name="alignment">the System.Drawing.ContentAlignment value indicating how the element is to be aligned should the width or height differ from targetSize</param>
        /// <returns>a new RectangleF object indicating the width and height the element should be scaled to and the position that should be applied to it for proper alignment</returns>
        public static RectangleF GetScaledRectangle(RectangleF currentRect, RectangleF targetRect, bool crop, ContentAlignment alignment)
        {
            SizeF newSize = GetScaledSize(currentRect.Size, targetRect.Size, crop);
            RectangleF newRect = new RectangleF(new Point(0, 0), newSize);
            return GetAlignedRectangle(newRect, targetRect, alignment);
        }

        public static void RationalScale(ref RectangleF originalRectangle, int resizeHandlePosition, PointF resizeHandleCoords)
        {
            Scale(ref originalRectangle, resizeHandlePosition, resizeHandleCoords, ScaleOptions.Rational);
        }

        public static void CenteredScale(ref RectangleF originalRectangle, int resizeHandlePosition, PointF resizeHandleCoords)
        {
            Scale(ref originalRectangle, resizeHandlePosition, resizeHandleCoords, ScaleOptions.Centered);
        }

        public static void Scale(ref RectangleF originalRectangle, int resizeHandlePosition, PointF resizeHandleCoords)
        {
            Scale(ref originalRectangle, resizeHandlePosition, resizeHandleCoords, null);
        }

        /// <summary>
        /// Calculates target size of a given rectangle scaled by dragging one of its handles (corners)
        /// </summary>
        /// <param name="originalRectangle">bounds of the current rectangle, scaled values will be written to this reference</param>
        /// <param name="resizeHandlePosition">position of the handle/gripper being used for resized, see constants in Gripper.cs, e.g. Gripper.POSITION_TOP_LEFT</param>
        /// <param name="resizeHandleCoords">coordinates of the used handle/gripper</param>
        /// <param name="options">ScaleOptions to use when scaling</param>
        public static void Scale(ref RectangleF originalRectangle, int resizeHandlePosition, PointF resizeHandleCoords, ScaleOptions? options)
        {
            if (options == null)
            {
                options = GetScaleOptions();
            }

            if ((options & ScaleOptions.Rational) == ScaleOptions.Rational)
            {
                adjustCoordsForRationalScale(originalRectangle, resizeHandlePosition, ref resizeHandleCoords);
            }

            if ((options & ScaleOptions.Centered) == ScaleOptions.Centered)
            {
                // store center coordinates of rectangle
                float rectCenterX = originalRectangle.Left + originalRectangle.Width / 2;
                float rectCenterY = originalRectangle.Top + originalRectangle.Height / 2;
                // scale rectangle using handle coordinates
                scale(ref originalRectangle, resizeHandlePosition, resizeHandleCoords);
                // mirror handle coordinates via rectangle center coordinates
                resizeHandleCoords.X -= 2 * (resizeHandleCoords.X - rectCenterX);
                resizeHandleCoords.Y -= 2 * (resizeHandleCoords.Y - rectCenterY);
                // scale again with opposing handle and mirrored coordinates
                resizeHandlePosition = (resizeHandlePosition + 4) % 8;
                scale(ref originalRectangle, resizeHandlePosition, resizeHandleCoords);
            }
            else
            {
                scale(ref originalRectangle, resizeHandlePosition, resizeHandleCoords);
            }
        }

        /// <summary>
        /// Calculates target size of a given rectangle scaled by dragging one of its handles (corners)
        /// </summary>
        /// <param name="originalRectangle">bounds of the current rectangle, scaled values will be written to this reference</param>
        /// <param name="resizeHandlePosition">position of the handle/gripper being used for resized, see constants in Gripper.cs, e.g. Gripper.POSITION_TOP_LEFT</param>
        /// <param name="resizeHandleCoords">coordinates of the used handle/gripper</param>
        private static void scale(ref RectangleF originalRectangle, int resizeHandlePosition, PointF resizeHandleCoords)
        {
            switch (resizeHandlePosition)
            {
                case Gripper.POSITION_TOP_LEFT:
                    originalRectangle.Width = originalRectangle.Left + originalRectangle.Width - resizeHandleCoords.X;
                    originalRectangle.Height = originalRectangle.Top + originalRectangle.Height - resizeHandleCoords.Y;
                    originalRectangle.X = resizeHandleCoords.X;
                    originalRectangle.Y = resizeHandleCoords.Y;
                    break;

                case Gripper.POSITION_TOP_CENTER:
                    originalRectangle.Height = originalRectangle.Top + originalRectangle.Height - resizeHandleCoords.Y;
                    originalRectangle.Y = resizeHandleCoords.Y;
                    break;

                case Gripper.POSITION_TOP_RIGHT:
                    originalRectangle.Width = resizeHandleCoords.X - originalRectangle.Left;
                    originalRectangle.Height = originalRectangle.Top + originalRectangle.Height - resizeHandleCoords.Y;
                    originalRectangle.Y = resizeHandleCoords.Y;
                    break;

                case Gripper.POSITION_MIDDLE_LEFT:
                    originalRectangle.Width = originalRectangle.Left + originalRectangle.Width - resizeHandleCoords.X;
                    originalRectangle.X = resizeHandleCoords.X;
                    break;

                case Gripper.POSITION_MIDDLE_RIGHT:
                    originalRectangle.Width = resizeHandleCoords.X - originalRectangle.Left;
                    break;

                case Gripper.POSITION_BOTTOM_LEFT:
                    originalRectangle.Width = originalRectangle.Left + originalRectangle.Width - resizeHandleCoords.X;
                    originalRectangle.Height = resizeHandleCoords.Y - originalRectangle.Top;
                    originalRectangle.X = resizeHandleCoords.X;
                    break;

                case Gripper.POSITION_BOTTOM_CENTER:
                    originalRectangle.Height = resizeHandleCoords.Y - originalRectangle.Top;
                    break;

                case Gripper.POSITION_BOTTOM_RIGHT:
                    originalRectangle.Width = resizeHandleCoords.X - originalRectangle.Left;
                    originalRectangle.Height = resizeHandleCoords.Y - originalRectangle.Top;
                    break;

                default:
                    throw new ArgumentException("Position cannot be handled: " + resizeHandlePosition);
            }
        }

        /// <summary>
        /// Adjusts resizeHandleCoords so that aspect ratio is kept after resizing a given rectangle with provided arguments
        /// </summary>
        /// <param name="originalRectangle">bounds of the current rectangle</param>
        /// <param name="resizeHandlePosition">position of the handle/gripper being used for resized, see constants in Gripper.cs, e.g. Gripper.POSITION_TOP_LEFT</param>
        /// <param name="resizeHandleCoords">coordinates of the used handle/gripper, adjusted coordinates will be written to this reference</param>
        private static void adjustCoordsForRationalScale(RectangleF originalRectangle, int resizeHandlePosition, ref PointF resizeHandleCoords)
        {
            float originalRatio = originalRectangle.Width / originalRectangle.Height;
            float newWidth, newHeight, newRatio;
            switch (resizeHandlePosition)
            {
                case Gripper.POSITION_TOP_LEFT:
                    newWidth = originalRectangle.Right - resizeHandleCoords.X;
                    newHeight = originalRectangle.Bottom - resizeHandleCoords.Y;
                    newRatio = newWidth / newHeight;
                    if (newRatio > originalRatio)
                    { // FIXME
                        resizeHandleCoords.X = originalRectangle.Right - newHeight * originalRatio;
                    }
                    else if (newRatio < originalRatio)
                    {
                        resizeHandleCoords.Y = originalRectangle.Bottom - newWidth / originalRatio;
                    }
                    break;

                case Gripper.POSITION_TOP_RIGHT:
                    newWidth = resizeHandleCoords.X - originalRectangle.Left;
                    newHeight = originalRectangle.Bottom - resizeHandleCoords.Y;
                    newRatio = newWidth / newHeight;
                    if (newRatio > originalRatio)
                    { // FIXME
                        resizeHandleCoords.X = newHeight * originalRatio + originalRectangle.Left;
                    }
                    else if (newRatio < originalRatio)
                    {
                        resizeHandleCoords.Y = originalRectangle.Bottom - newWidth / originalRatio;
                    }
                    break;

                case Gripper.POSITION_BOTTOM_LEFT:
                    newWidth = originalRectangle.Right - resizeHandleCoords.X;
                    newHeight = resizeHandleCoords.Y - originalRectangle.Top;
                    newRatio = newWidth / newHeight;
                    if (newRatio > originalRatio)
                    {
                        resizeHandleCoords.X = originalRectangle.Right - newHeight * originalRatio;
                    }
                    else if (newRatio < originalRatio)
                    {
                        resizeHandleCoords.Y = newWidth / originalRatio + originalRectangle.Top;
                    }
                    break;

                case Gripper.POSITION_BOTTOM_RIGHT:
                    newWidth = resizeHandleCoords.X - originalRectangle.Left;
                    newHeight = resizeHandleCoords.Y - originalRectangle.Top;
                    newRatio = newWidth / newHeight;
                    if (newRatio > originalRatio)
                    {
                        resizeHandleCoords.X = newHeight * originalRatio + originalRectangle.Left;
                    }
                    else if (newRatio < originalRatio)
                    {
                        resizeHandleCoords.Y = newWidth / originalRatio + originalRectangle.Top;
                    }
                    break;
            }
        }

        public static void Scale(Rectangle boundsBeforeResize, int cursorX, int cursorY, ref RectangleF boundsAfterResize)
        {
            Scale(boundsBeforeResize, cursorX, cursorY, ref boundsAfterResize, null);
        }

        public static void Scale(Rectangle boundsBeforeResize, int cursorX, int cursorY, ref RectangleF boundsAfterResize, IDoubleProcessor angleRoundBehavior)
        {
            Scale(boundsBeforeResize, Gripper.POSITION_TOP_LEFT, cursorX, cursorY, ref boundsAfterResize, angleRoundBehavior);
        }

        public static void Scale(Rectangle boundsBeforeResize, int gripperPosition, int cursorX, int cursorY, ref RectangleF boundsAfterResize, IDoubleProcessor angleRoundBehavior)
        {
            ScaleOptions opts = GetScaleOptions();

            bool rationalScale = (opts & ScaleOptions.Rational) == ScaleOptions.Rational;
            bool centeredScale = (opts & ScaleOptions.Centered) == ScaleOptions.Centered;

            if (rationalScale)
            {
                double angle = GeometryHelper.Angle2D(boundsBeforeResize.X, boundsBeforeResize.Y, cursorX, cursorY);

                if (angleRoundBehavior != null)
                {
                    angle = angleRoundBehavior.Process(angle);
                }

                int dist = GeometryHelper.Distance2D(boundsBeforeResize.X, boundsBeforeResize.Y, cursorX, cursorY);

                boundsAfterResize.Width = (int)Math.Round(dist * Math.Cos(angle / 180 * Math.PI));
                boundsAfterResize.Height = (int)Math.Round(dist * Math.Sin(angle / 180 * Math.PI));
            }

            if (centeredScale)
            {
                float wdiff = boundsAfterResize.Width - boundsBeforeResize.Width;
                float hdiff = boundsAfterResize.Height - boundsBeforeResize.Height;
                boundsAfterResize.Width += wdiff;
                boundsAfterResize.Height += hdiff;
                boundsAfterResize.X -= wdiff;
                boundsAfterResize.Y -= hdiff;
            }
        }

        /// <returns>the current ScaleOptions depending on modifier keys held down</returns>
        public static ScaleOptions GetScaleOptions()
        {
            bool anchorAtCenter = (Control.ModifierKeys & Keys.Control) != 0;
            bool maintainAspectRatio = ((Control.ModifierKeys & Keys.Shift) != 0);
            ScaleOptions opts = ScaleOptions.Default;
            if (anchorAtCenter) opts |= ScaleOptions.Centered;
            if (maintainAspectRatio) opts |= ScaleOptions.Rational;
            return opts;
        }

        public interface IDoubleProcessor
        {
            double Process(double d);
        }

        public class ShapeAngleRoundBehavior : IDoubleProcessor
        {
            public static ShapeAngleRoundBehavior Instance = new ShapeAngleRoundBehavior();

            private ShapeAngleRoundBehavior()
            {
            }

            public double Process(double angle)
            {
                return Math.Round((angle + 45) / 90) * 90 - 45;
            }
        }

        public class LineAngleRoundBehavior : IDoubleProcessor
        {
            public static LineAngleRoundBehavior Instance = new LineAngleRoundBehavior();

            private LineAngleRoundBehavior()
            {
            }

            public double Process(double angle)
            {
                return Math.Round(angle / 15) * 15;
            }
        }

        public class FixedAngleRoundBehavior : IDoubleProcessor
        {
            private double fixedAngle;

            public FixedAngleRoundBehavior(double fixedAngle)
            {
                this.fixedAngle = fixedAngle;
            }

            public double Process(double angle)
            {
                return fixedAngle;
            }
        }

        /*public static int FindGripperPostition(float anchorX, float anchorY, float gripperX, float gripperY) {
            if(gripperY > anchorY) {
                if(gripperX > anchorY) return Gripper.POSITION_BOTTOM_RIGHT;
                else return Gripper.POSITION_BOTTOM_LEFT;
            } else {
                if(gripperX > anchorY) return Gripper.POSITION_TOP_RIGHT;
                else return Gripper.POSITION_TOP_LEFT;
            }
        }*/
    }
}
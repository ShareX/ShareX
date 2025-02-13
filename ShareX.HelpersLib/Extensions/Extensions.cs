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

using ShareX.HelpersLib.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (T item in source)
            {
                action(item);
            }
        }

        public static byte[] GetBytes(this Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, img.RawFormat);
                return ms.ToArray();
            }
        }

        public static Stream GetStream(this Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, img.RawFormat);
            return ms;
        }

        public static ImageCodecInfo GetCodecInfo(this ImageFormat format)
        {
            return ImageCodecInfo.GetImageEncoders().FirstOrDefault(info => info.FormatID.Equals(format.Guid));
        }

        public static string GetMimeType(this ImageFormat format)
        {
            ImageCodecInfo codec = format.GetCodecInfo();

            if (codec != null) return codec.MimeType;

            return "image/unknown";
        }

        public static bool IsValidIndex<T>(this T[] array, int index)
        {
            return array != null && index >= 0 && index < array.Length;
        }

        public static bool IsValidIndex<T>(this List<T> list, int index)
        {
            return list != null && index >= 0 && index < list.Count;
        }

        public static T ReturnIfValidIndex<T>(this T[] array, int index)
        {
            if (array.IsValidIndex(index)) return array[index];
            return default;
        }

        public static T ReturnIfValidIndex<T>(this List<T> list, int index)
        {
            if (list.IsValidIndex(index)) return list[index];
            return default;
        }

        public static T Last<T>(this T[] array, int index = 0)
        {
            if (array.Length > index) return array[array.Length - index - 1];
            return default;
        }

        public static T Last<T>(this List<T> list, int index = 0)
        {
            if (list.Count > index) return list[list.Count - index - 1];
            return default;
        }

        public static double ToDouble(this Version value)
        {
            return (Math.Max(value.Major, 0) * Math.Pow(10, 12)) +
                (Math.Max(value.Minor, 0) * Math.Pow(10, 9)) +
                (Math.Max(value.Build, 0) * Math.Pow(10, 6)) +
                Math.Max(value.Revision, 0);
        }

        public static bool IsValid(this Rectangle rect)
        {
            return rect.Width > 0 && rect.Height > 0;
        }

        public static bool IsValid(this RectangleF rect)
        {
            return rect.Width > 0 && rect.Height > 0;
        }

        public static Point Add(this Point point, int offsetX, int offsetY)
        {
            return new Point(point.X + offsetX, point.Y + offsetY);
        }

        public static Point Add(this Point point, Point offset)
        {
            return new Point(point.X + offset.X, point.Y + offset.Y);
        }

        public static Point Add(this Point point, int offset)
        {
            return point.Add(offset, offset);
        }

        public static PointF Add(this PointF point, float offsetX, float offsetY)
        {
            return new PointF(point.X + offsetX, point.Y + offsetY);
        }

        public static PointF Add(this PointF point, PointF offset)
        {
            return new PointF(point.X + offset.X, point.Y + offset.Y);
        }

        public static PointF Scale(this Point point, float scaleFactor)
        {
            return new PointF(point.X * scaleFactor, point.Y * scaleFactor);
        }

        public static PointF Scale(this PointF point, float scaleFactor)
        {
            return new PointF(point.X * scaleFactor, point.Y * scaleFactor);
        }

        public static Point Round(this PointF point)
        {
            return Point.Round(point);
        }

        public static void Offset(this PointF point, PointF offset)
        {
            point.X += offset.X;
            point.Y += offset.Y;
        }

        public static Size Offset(this Size size, int offset)
        {
            return size.Offset(offset, offset);
        }

        public static Size Offset(this Size size, int width, int height)
        {
            return new Size(size.Width + width, size.Height + height);
        }

        public static Rectangle Offset(this Rectangle rect, int offset)
        {
            return new Rectangle(rect.X - offset, rect.Y - offset, rect.Width + (offset * 2), rect.Height + (offset * 2));
        }

        public static RectangleF Offset(this RectangleF rect, float offset)
        {
            return new RectangleF(rect.X - offset, rect.Y - offset, rect.Width + (offset * 2), rect.Height + (offset * 2));
        }

        public static RectangleF Scale(this RectangleF rect, float scaleFactor)
        {
            return new RectangleF(rect.X * scaleFactor, rect.Y * scaleFactor, rect.Width * scaleFactor, rect.Height * scaleFactor);
        }

        public static Rectangle Round(this RectangleF rect)
        {
            return Rectangle.Round(rect);
        }

        public static Rectangle LocationOffset(this Rectangle rect, int x, int y)
        {
            return new Rectangle(rect.X + x, rect.Y + y, rect.Width, rect.Height);
        }

        public static RectangleF LocationOffset(this RectangleF rect, float x, float y)
        {
            return new RectangleF(rect.X + x, rect.Y + y, rect.Width, rect.Height);
        }

        public static RectangleF LocationOffset(this RectangleF rect, PointF offset)
        {
            return rect.LocationOffset(offset.X, offset.Y);
        }

        public static Rectangle LocationOffset(this Rectangle rect, Point offset)
        {
            return rect.LocationOffset(offset.X, offset.Y);
        }

        public static Rectangle LocationOffset(this Rectangle rect, int offset)
        {
            return rect.LocationOffset(offset, offset);
        }

        public static Rectangle SizeOffset(this Rectangle rect, int width, int height)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width + width, rect.Height + height);
        }

        public static RectangleF SizeOffset(this RectangleF rect, float width, float height)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width + width, rect.Height + height);
        }

        public static Rectangle SizeOffset(this Rectangle rect, int offset)
        {
            return rect.SizeOffset(offset, offset);
        }

        public static RectangleF SizeOffset(this RectangleF rect, float offset)
        {
            return rect.SizeOffset(offset, offset);
        }

        public static string Join<T>(this T[] array, string separator = " ")
        {
            StringBuilder sb = new StringBuilder();

            if (array != null)
            {
                foreach (T t in array)
                {
                    if (sb.Length > 0 && !string.IsNullOrEmpty(separator)) sb.Append(separator);
                    sb.Append(t);
                }
            }

            return sb.ToString();
        }

        public static int WeekOfYear(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        public static Icon ToIcon(this Bitmap bmp)
        {
            IntPtr handle = bmp.GetHicon();
            return Icon.FromHandle(handle);
        }

        public static void DisposeHandle(this Icon icon)
        {
            if (icon.Handle != IntPtr.Zero)
            {
                NativeMethods.DestroyIcon(icon.Handle);
            }
        }

        public static void ApplyDefaultPropertyValues(this object self)
        {
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(self))
            {
                if (prop.Attributes[typeof(DefaultValueAttribute)] is DefaultValueAttribute attr)
                {
                    prop.SetValue(self, attr.Value);
                }
            }
        }

        public static Bitmap CreateEmptyBitmap(this Image img, int widthOffset = 0, int heightOffset = 0, PixelFormat pixelFormat = PixelFormat.Format32bppArgb)
        {
            Bitmap bmp = new Bitmap(img.Width + widthOffset, img.Height + heightOffset, pixelFormat);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);
            return bmp;
        }

        public static Bitmap CreateEmptyBitmap(this Image img, PixelFormat pixelFormat)
        {
            return img.CreateEmptyBitmap(0, 0, pixelFormat);
        }

        public static string GetDescription(this Type type)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])type.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : type.Name;
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count)
        {
            return source.Reverse().Take(count).Reverse();
        }

        public static Version Normalize(this Version version, bool ignoreRevision = false, bool ignoreBuild = false, bool ignoreMinor = false)
        {
            return new Version(Math.Max(version.Major, 0),
                ignoreMinor ? 0 : Math.Max(version.Minor, 0),
                ignoreBuild ? 0 : Math.Max(version.Build, 0),
                ignoreRevision ? 0 : Math.Max(version.Revision, 0));
        }

        public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
        {
            T obj = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, obj);
        }

        public static Rectangle Combine(this IEnumerable<Rectangle> rects)
        {
            Rectangle result = Rectangle.Empty;

            foreach (Rectangle rect in rects)
            {
                if (result.IsEmpty)
                {
                    result = rect;
                }
                else
                {
                    result = Rectangle.Union(result, rect);
                }
            }

            return result;
        }

        public static RectangleF Combine(this IEnumerable<RectangleF> rects)
        {
            RectangleF result = RectangleF.Empty;

            foreach (RectangleF rect in rects)
            {
                if (result.IsEmpty)
                {
                    result = rect;
                }
                else
                {
                    result = RectangleF.Union(result, rect);
                }
            }

            return result;
        }

        public static RectangleF AddPoint(this RectangleF rect, PointF point)
        {
            return RectangleF.Union(rect, new RectangleF(point, new SizeF(1, 1)));
        }

        public static RectangleF CreateRectangle(this IEnumerable<PointF> points)
        {
            RectangleF result = Rectangle.Empty;

            foreach (PointF point in points)
            {
                if (result.IsEmpty)
                {
                    result = new RectangleF(point, new Size(1, 1));
                }
                else
                {
                    result = result.AddPoint(point);
                }
            }

            return result;
        }

        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));
        }

        public static PointF Center(this RectangleF rect)
        {
            return new PointF(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));
        }

        public static float Area(this RectangleF rect)
        {
            return rect.Width * rect.Height;
        }

        public static float Perimeter(this RectangleF rect)
        {
            return 2 * (rect.Width + rect.Height);
        }

        public static PointF Restrict(this PointF point, RectangleF rect)
        {
            point.X = Math.Max(point.X, rect.X);
            point.Y = Math.Max(point.Y, rect.Y);
            point.X = Math.Min(point.X, rect.X + rect.Width - 1);
            point.Y = Math.Min(point.Y, rect.Y + rect.Height - 1);
            return point;
        }

        public static void ShowError(this Exception e, bool fullError = true)
        {
            string error = fullError ? e.ToString() : e.Message;
            MessageBox.Show(error, "ShareX - " + Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static Task ContinueInCurrentContext(this Task task, Action action)
        {
            TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return task.ContinueWith(t => action(), scheduler);
        }

        public static List<T> Range<T>(this List<T> source, int start, int end)
        {
            List<T> list = new List<T>();

            if (start > end)
            {
                for (int i = start; i >= end; i--)
                {
                    list.Add(source[i]);
                }
            }
            else
            {
                for (int i = start; i <= end; i++)
                {
                    list.Add(source[i]);
                }
            }

            return list;
        }

        public static List<T> Range<T>(this List<T> source, T start, T end)
        {
            int startIndex = source.IndexOf(start);
            if (startIndex == -1) return new List<T>();

            int endIndex = source.IndexOf(end);
            if (endIndex == -1) return new List<T>();

            return Range(source, startIndex, endIndex);
        }

        public static T CloneSafe<T>(this T obj) where T : class, ICloneable
        {
            try
            {
                if (obj != null)
                {
                    return obj.Clone() as T;
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return null;
        }

        public static bool IsTransparent(this Color color)
        {
            return color.A < 255;
        }

        public static string ToStringProper(this Rectangle rect)
        {
            return $"X: {rect.X}, Y: {rect.Y}, Width: {rect.Width}, Height: {rect.Height}";
        }
    }
}
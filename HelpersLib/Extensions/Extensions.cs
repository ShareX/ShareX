#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

using HelpersLib.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Encoder = System.Drawing.Imaging.Encoder;

namespace HelpersLib
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            foreach (T item in source)
            {
                action(item);
            }
        }

        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            using (IEnumerator<TFirst> e1 = first.GetEnumerator())
            using (IEnumerator<TSecond> e2 = second.GetEnumerator())
            {
                while (e1.MoveNext() && e2.MoveNext())
                {
                    yield return resultSelector(e1.Current, e2.Current);
                }
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
            return default(T);
        }

        public static T ReturnIfValidIndex<T>(this List<T> list, int index)
        {
            if (list.IsValidIndex(index)) return list[index];
            return default(T);
        }

        public static T Last<T>(this T[] array, int index = 0)
        {
            if (array.Length > index) return array[array.Length - index - 1];
            return default(T);
        }

        public static T Last<T>(this List<T> list, int index = 0)
        {
            if (list.Count > index) return list[list.Count - index - 1];
            return default(T);
        }

        public static double ToDouble(this Version value)
        {
            return Math.Max(value.Major, 0) * Math.Pow(10, 12) +
                Math.Max(value.Minor, 0) * Math.Pow(10, 9) +
                Math.Max(value.Build, 0) * Math.Pow(10, 6) +
                Math.Max(value.Revision, 0);
        }

        public static bool IsValid(this Rectangle rect)
        {
            return rect.Width > 0 && rect.Height > 0;
        }

        public static Size Offset(this Size size, int offset)
        {
            return new Size(size.Width + offset, size.Height + offset);
        }

        public static Rectangle Offset(this Rectangle rect, int offset)
        {
            return new Rectangle(rect.X - offset, rect.Y - offset, rect.Width + offset * 2, rect.Height + offset * 2);
        }

        public static Rectangle LocationOffset(this Rectangle rect, int x, int y)
        {
            return new Rectangle(rect.X + x, rect.Y + y, rect.Width, rect.Height);
        }

        public static Rectangle LocationOffset(this Rectangle rect, int offset)
        {
            return rect.LocationOffset(offset, offset);
        }

        public static Rectangle SizeOffset(this Rectangle rect, int width, int height)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width + width, rect.Height + height);
        }

        public static Rectangle SizeOffset(this Rectangle rect, int offset)
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

        public static void BeginUpdate(this RichTextBox rtb)
        {
            NativeMethods.SendMessage(rtb.Handle, (int)WindowsMessages.SETREDRAW, 0, 0);
        }

        public static void EndUpdate(this RichTextBox rtb)
        {
            NativeMethods.SendMessage(rtb.Handle, (int)WindowsMessages.SETREDRAW, 1, 0);
            rtb.Invalidate();
        }

        public static void AddContextMenu(this RichTextBox rtb)
        {
            if (rtb.ContextMenuStrip == null)
            {
                ContextMenuStrip cms = new ContextMenuStrip { ShowImageMargin = false };
                ToolStripMenuItem tsmiCut = new ToolStripMenuItem(Resources.Extensions_AddContextMenu_Cut);
                tsmiCut.Click += (sender, e) => rtb.Cut();
                cms.Items.Add(tsmiCut);
                ToolStripMenuItem tsmiCopy = new ToolStripMenuItem(Resources.Extensions_AddContextMenu_Copy);
                tsmiCopy.Click += (sender, e) => rtb.Copy();
                cms.Items.Add(tsmiCopy);
                ToolStripMenuItem tsmiPaste = new ToolStripMenuItem(Resources.Extensions_AddContextMenu_Paste);
                tsmiPaste.Click += (sender, e) => rtb.Paste();
                cms.Items.Add(tsmiPaste);
                rtb.ContextMenuStrip = cms;
            }
        }

        public static void SaveJPG(this Image img, Stream stream, int quality)
        {
            quality = quality.Between(0, 100);
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            img.Save(stream, ImageFormat.Jpeg.GetCodecInfo(), encoderParameters);
        }

        public static void SaveJPG(this Image img, string filepath, int quality)
        {
            quality = quality.Between(0, 100);
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            img.Save(filepath, ImageFormat.Jpeg.GetCodecInfo(), encoderParameters);
        }

        public static void SaveGIF(this Image img, Stream stream, GIFQuality quality)
        {
            if (quality == GIFQuality.Default)
            {
                img.Save(stream, ImageFormat.Gif);
            }
            else
            {
                Quantizer quantizer;
                switch (quality)
                {
                    case GIFQuality.Grayscale:
                        quantizer = new GrayscaleQuantizer();
                        break;
                    case GIFQuality.Bit4:
                        quantizer = new OctreeQuantizer(15, 4);
                        break;
                    default:
                    case GIFQuality.Bit8:
                        quantizer = new OctreeQuantizer(255, 4);
                        break;
                }

                using (Bitmap quantized = quantizer.Quantize(img))
                {
                    quantized.Save(stream, ImageFormat.Gif);
                }
            }
        }

        public static long ToUnix(this DateTime dateTime)
        {
            return (dateTime.Ticks - 621355968000000000) / 10000000;
        }

        public static void AppendTextToSelection(this TextBox tb, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                int start = tb.SelectionStart;
                tb.Text = tb.Text.Insert(start, text);
                tb.SelectionStart = start + text.Length;
            }
        }

        public static void RadioCheck(this ToolStripMenuItem tsmi)
        {
            ToolStrip parent = tsmi.GetCurrentParent();

            foreach (ToolStripMenuItem tsmiParent in parent.Items)
            {
                if (tsmiParent != tsmi)
                {
                    tsmiParent.Checked = false;
                }
            }

            tsmi.Checked = true;
        }

        public static void InvokeSafe(this Control control, Action action)
        {
            if (control != null && !control.IsDisposed)
            {
                if (control.InvokeRequired)
                {
                    control.Invoke(action);
                }
                else
                {
                    action();
                }
            }
        }

        public static void ShowActivate(this Form form)
        {
            if (!form.Visible)
            {
                form.Show();
            }

            if (form.WindowState == FormWindowState.Minimized)
            {
                form.WindowState = FormWindowState.Normal;
            }

            form.BringToFront();
            form.Activate();
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
                DefaultValueAttribute attr = prop.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
                if (attr != null) prop.SetValue(self, attr.Value);
            }
        }

        public static void MoveUp(this ListViewItem lvi)
        {
            ListView lv = lvi.ListView;

            if (lv.Items.Count > 1)
            {
                int index = lvi.Index;

                if (index == 0)
                {
                    index = lv.Items.Count - 1;
                }
                else
                {
                    index--;
                }

                lv.Items.Remove(lvi);
                lv.Items.Insert(index, lvi);
            }

            lv.Focus();
            lvi.EnsureVisible();
            lvi.Selected = true;
        }

        public static void MoveDown(this ListViewItem lvi)
        {
            ListView lv = lvi.ListView;

            if (lv.Items.Count > 1)
            {
                int index = lvi.Index;

                if (index == lv.Items.Count - 1)
                {
                    index = 0;
                }
                else
                {
                    index++;
                }

                lv.Items.Remove(lvi);
                lv.Items.Insert(index, lvi);
            }

            lv.Focus();
            lvi.EnsureVisible();
            lvi.Selected = true;
        }

        public static Bitmap CreateEmptyBitmap(this Image img, int widthOffset = 0, int heightOffset = 0, PixelFormat pixelFormat = PixelFormat.Format32bppArgb)
        {
            Bitmap bmp = new Bitmap(img.Width + widthOffset, img.Height + heightOffset, pixelFormat);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);
            return bmp;
        }

        public static string GetDescription(this Type type)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])type.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : type.Name;
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count)
        {
            return source.Reverse().Take(count).Reverse();
        }

        public static void Check(this ToolStripMenuItem tsmi)
        {
            if (tsmi != null)
            {
                foreach (ToolStripMenuItem item in tsmi.GetCurrentParent().Items)
                {
                    if (item != null)
                    {
                        item.Checked = item == tsmi;
                    }
                }
            }
        }

        public static Version Normalize(this Version version)
        {
            return new Version(Math.Max(version.Major, 0), Math.Max(version.Minor, 0), Math.Max(version.Build, 0), Math.Max(version.Revision, 0));
        }
    }
}
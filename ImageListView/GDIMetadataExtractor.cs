// ImageListView - A listview control for image files
// Copyright (C) 2009 Ozgur Ozcitak
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Ozgur Ozcitak (ozcitak@yahoo.com)
//
// WIC support coded by Jens

using System.Drawing.Imaging;
using System.Text;

namespace ShareX.ImageListView;

/// <summary>
/// Read metadata.
/// </summary>
public partial class GDIExtractor : IExtractor
{
    #region Properties
    /// <summary>
    /// Gets the name of this extractor.
    /// </summary>
    public virtual string Name => "GDI Thumbnail Extractor";
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the GDIExtractor class.
    /// </summary>
    public GDIExtractor()
    {
        ;
    }
    #endregion

    #region Exif Tag IDs
    private const int TagImageDescription = 0x010E;
    private const int TagEquipmentModel = 0x0110;
    private const int TagDateTimeOriginal = 0x9003;
    private const int TagArtist = 0x013B;
    private const int TagCopyright = 0x8298;
    private const int TagExposureTime = 0x829A;
    private const int TagFNumber = 0x829D;
    private const int TagISOSpeed = 0x8827;
    private const int TagUserComment = 0x9286;
    private const int TagRating = 0x4746;
    private const int TagRatingPercent = 0x4749;
    private const int TagEquipmentManufacturer = 0x010F;
    private const int TagFocalLength = 0x920A;
    private const int TagSoftware = 0x0131;
    #endregion

    #region Exif Format Conversion
    /// <summary>
    /// Converts the given Exif data to an ASCII encoded string.
    /// </summary>
    /// <param name="value">Exif data as a byte array.</param>
    private static string ExifAscii(byte[] value)
    {
        if (value == null || value.Length == 0)
            return string.Empty;

        string str = Encoding.ASCII.GetString(value);
        str = str.Trim(new char[] { '\0' });
        return str;
    }
    /// <summary>
    /// Converts the given Exif data to DateTime.
    /// </summary>
    /// <param name="value">Exif data as a byte array.</param>
    private static DateTime ExifDateTime(byte[] value)
    {
        return ExifDateTime(ExifAscii(value));
    }
    /// <summary>
    /// Converts the given Exif data to DateTime.
    /// Value must be formatted as yyyy:MM:dd HH:mm:ss.
    /// </summary>
    /// <param name="value">Exif data as a string.</param>
    private static DateTime ExifDateTime(string value)
    {
        string[] parts = value.Split(new char[] { ':', ' ' });
        try
        {
            if (parts.Length == 6)
            {
                // yyyy:MM:dd HH:mm:ss
                // This is the expected format though some cameras
                // can use single digits. See Issue 21.
                return new DateTime(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]), int.Parse(parts[4]), int.Parse(parts[5]));
            } else if (parts.Length == 3)
            {
                // yyyy:MM:dd
                return new DateTime(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
            } else
            {
                return DateTime.MinValue;
            }
        } catch
        {
            return DateTime.MinValue;
        }
    }
    /// <summary>
    /// Converts the given Exif data to an 16-bit unsigned integer.
    /// The value must have 2 bytes.
    /// </summary>
    /// <param name="value">Exif data as a byte array.</param>
    private static ushort ExifUShort(byte[] value)
    {
        return BitConverter.ToUInt16(value, 0);
    }
    /// <summary>
    /// Converts the given Exif data to an 32-bit unsigned integer.
    /// The value must have 4 bytes.
    /// </summary>
    /// <param name="value">Exif data as a byte array.</param>
    private static uint ExifUInt(byte[] value)
    {
        return BitConverter.ToUInt32(value, 0);
    }
    /// <summary>
    /// Converts the given Exif data to an 32-bit signed integer.
    /// The value must have 4 bytes.
    /// </summary>
    /// <param name="value">Exif data as a byte array.</param>
    private static int ExifInt(byte[] value)
    {
        return BitConverter.ToInt32(value, 0);
    }
    /// <summary>
    /// Converts the given Exif data to an unsigned rational value
    /// represented as a string.
    /// The value must have 8 bytes.
    /// </summary>
    /// <param name="value">Exif data as a byte array.</param>
    private static string ExifURational(byte[] value)
    {
        return BitConverter.ToUInt32(value, 0).ToString() + "/" +
                BitConverter.ToUInt32(value, 4).ToString();
    }
    /// <summary>
    /// Converts the given Exif data to a signed rational value
    /// represented as a string.
    /// The value must have 8 bytes.
    /// </summary>
    /// <param name="value">Exif data as a byte array.</param>
    private static string ExifRational(byte[] value)
    {
        return BitConverter.ToInt32(value, 0).ToString() + "/" +
                BitConverter.ToInt32(value, 4).ToString();
    }
    /// <summary>
    /// Converts the given Exif data to a double number.
    /// The value must have 8 bytes.
    /// </summary>
    /// <param name="value">Exif data as a byte array.</param>
    private static double ExifDouble(byte[] value)
    {
        uint num = BitConverter.ToUInt32(value, 0);
        uint den = BitConverter.ToUInt32(value, 4);
        return den == 0 ? 0.0 : num / (double)den;
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Open image and read metadata (.NET 2.0).
    /// </summary>
    /// <param name="path">Filepath of image</param>
    private static Metadata InitViaBmp(string path)
    {
        using (FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            if (IsImage(stream))
            {
                using Image img = Image.FromStream(stream, false, false);
                if (img != null)
                {
                    return InitViaBmp(img);
                }
            }
        }

        return new Metadata();
    }
    /// <summary>
    /// Read metadata using .NET 2.0 methods.
    /// </summary>
    /// <param name="img">Opened image</param>
    private static Metadata InitViaBmp(Image img)
    {
        Metadata m = new();

        m.Width = img.Width;
        m.Height = img.Height;
        m.DPIX = img.HorizontalResolution;
        m.DPIY = img.VerticalResolution;

        double dVal;
        int iVal;
        DateTime dateTime;
        string str;
        foreach (PropertyItem prop in img.PropertyItems)
        {
            if (prop.Value != null && prop.Value.Length != 0)
            {
                switch (prop.Id)
                {
                    case TagImageDescription:
                        str = ExifAscii(prop.Value).Trim();
                        if (str != String.Empty)
                        {
                            m.ImageDescription = str;
                        }
                        break;
                    case TagArtist:
                        str = ExifAscii(prop.Value).Trim();
                        if (str != String.Empty)
                        {
                            m.Artist = str;
                        }
                        break;
                    case TagEquipmentManufacturer:
                        str = ExifAscii(prop.Value).Trim();
                        if (str != String.Empty)
                        {
                            m.EquipmentManufacturer = str;
                        }
                        break;
                    case TagEquipmentModel:
                        str = ExifAscii(prop.Value).Trim();
                        if (str != String.Empty)
                        {
                            m.EquipmentModel = str;
                        }
                        break;
                    case TagDateTimeOriginal:
                        dateTime = ExifDateTime(prop.Value);
                        if (dateTime != DateTime.MinValue)
                        {
                            m.DateTaken = dateTime;
                        }
                        break;
                    case TagExposureTime:
                        if (prop.Value.Length == 8)
                        {
                            dVal = ExifDouble(prop.Value);
                            if (dVal != 0.0)
                            {
                                m.ExposureTime = dVal;
                            }
                        }
                        break;
                    case TagFNumber:
                        if (prop.Value.Length == 8)
                        {
                            dVal = ExifDouble(prop.Value);
                            if (dVal != 0.0)
                            {
                                m.FNumber = dVal;
                            }
                        }
                        break;
                    case TagISOSpeed:
                        if (prop.Value.Length == 2)
                        {
                            iVal = ExifUShort(prop.Value);
                            if (iVal != 0)
                            {
                                m.ISOSpeed = iVal;
                            }
                        }
                        break;
                    case TagCopyright:
                        str = ExifAscii(prop.Value);
                        if (str != String.Empty)
                        {
                            m.Copyright = str;
                        }
                        break;
                    case TagRating:
                        if (m.Rating == 0 && prop.Value.Length == 2)
                        {
                            iVal = ExifUShort(prop.Value);
                            if (iVal == 1)
                                m.Rating = 1;
                            else if (iVal == 2)
                                m.Rating = 25;
                            else if (iVal == 3)
                                m.Rating = 50;
                            else if (iVal == 4)
                                m.Rating = 75;
                            else if (iVal == 5)
                                m.Rating = 99;
                        }
                        break;
                    case TagRatingPercent:
                        if (prop.Value.Length == 2)
                        {
                            iVal = ExifUShort(prop.Value);
                            m.Rating = iVal;
                        }
                        break;
                    case TagUserComment:
                        str = ExifAscii(prop.Value);
                        if (str != String.Empty)
                        {
                            m.Comment = str;
                        }
                        break;
                    case TagSoftware:
                        str = ExifAscii(prop.Value).Trim();
                        if (str != String.Empty)
                        {
                            m.Software = str;
                        }
                        break;
                    case TagFocalLength:
                        if (prop.Value.Length == 8)
                        {
                            dVal = ExifDouble(prop.Value);
                            if (dVal != 0.0)
                            {
                                m.FocalLength = dVal;
                            }
                        }
                        break;
                }
            }
        }

        return m;
    }
    /// <summary>
    /// Convert FileTime to DateTime.
    /// </summary>
    /// <param name="ft">FileTime</param>
    /// <returns>DateTime</returns>
    private static DateTime ConvertFileTime(System.Runtime.InteropServices.ComTypes.FILETIME ft)
    {
        long longTime = (((long)ft.dwHighDateTime) << 32) | ((uint)ft.dwLowDateTime);
        return DateTime.FromFileTimeUtc(longTime); // using UTC???
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Returns image metadata.
    /// </summary>
    /// <param name="path">Filepath of image</param>
    public virtual Metadata GetMetadata(string path)
    {
        Metadata m = new();
        try
        {
            m = InitViaBmp(path);
        } catch (Exception e)
        {
            m.Error = e;
        }
        return m;
    }
    #endregion
}

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

namespace ShareX.ImageListView;

/// <summary>
/// Contains image metadata.
/// </summary>
public class Metadata
{
    /// <summary>
    /// Error.
    /// </summary>
    public Exception Error = null;
    /// <summary>
    /// Image width.
    /// </summary>
    public int Width = 0;
    /// <summary>
    /// Image height.
    /// </summary>
    public int Height = 0;
    /// <summary>
    /// Horizontal DPI.
    /// </summary>
    public double DPIX = 0.0;
    /// <summary>
    /// Vertical DPI.
    /// </summary>
    public double DPIY = 0.0;
    /// <summary>
    /// Date taken.
    /// </summary>
    public DateTime DateTaken = DateTime.MinValue;
    /// <summary>
    /// Image description (null = not available).
    /// </summary>
    public string ImageDescription = null;
    /// <summary>
    /// Camera manufacturer (null = not available).
    /// </summary>
    public string EquipmentManufacturer = null;
    /// <summary>
    /// Camera model (null = not available).
    /// </summary>
    public string EquipmentModel = null;
    /// <summary>
    /// Image creator (null = not available).
    /// </summary>
    public string Artist = null;
    /// <summary>
    /// Iso speed rating.
    /// </summary>
    public int ISOSpeed = 0;
    /// <summary>
    /// Exposure time.
    /// </summary>
    public double ExposureTime = 0.0;
    /// <summary>
    /// F number.
    /// </summary>
    public double FNumber = 0.0;
    /// <summary>
    /// Copyright information (null = not available).
    /// </summary>
    public string Copyright = null;
    /// <summary>
    /// Rating value between 0-99.
    /// </summary>
    public int Rating = 0;
    /// <summary>
    /// User comment (null = not available).
    /// </summary>
    public string Comment = null;
    /// <summary>
    /// Software used (null = not available).
    /// </summary>
    public string Software = null;
    /// <summary>
    /// Focal length.
    /// </summary>
    public double FocalLength = 0.0;
}

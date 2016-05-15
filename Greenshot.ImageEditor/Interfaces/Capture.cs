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

using System;
using System.Collections.Generic;
using System.Drawing;

namespace Greenshot.Plugin
{
    /// <summary>
    /// The capture mode for Greenshot
    /// </summary>
    public enum CaptureMode { None, Region, FullScreen, ActiveWindow, Window, LastRegion, Clipboard, File, IE, Import }; //, Video };
    public enum ScreenCaptureMode { Auto, FullScreen, Fixed };

    /// <summary>
    /// Details for the capture, like the window title and date/time etc.
    /// </summary>
    public interface ICaptureDetails
    {
        string Filename
        {
            get;
            set;
        }
        string Title
        {
            get;
            set;
        }

        DateTime DateTime
        {
            get;
            set;
        }

        List<IDestination> CaptureDestinations
        {
            get;
            set;
        }

        Dictionary<string, string> MetaData
        {
            get;
        }

        /// <summary>
        /// Helper method to prevent complex code which needs to check every key
        /// </summary>
        /// <param name="key">The key for the meta-data</param>
        /// <param name="value">The value for the meta-data</param>
        void AddMetaData(string key, string value);

        void ClearDestinations();

        void RemoveDestination(IDestination captureDestination);

        void AddDestination(IDestination captureDestination);

        bool HasDestination(string designation);

        CaptureMode CaptureMode
        {
            get;
            set;
        }

        float DpiX
        {
            get;
            set;
        }
        float DpiY
        {
            get;
            set;
        }
    }

    public interface ICaptureElement
    {
        List<ICaptureElement> Children
        {
            get;
            set;
        }
        Rectangle Bounds
        {
            get;
            set;
        }
        string Name
        {
            get;
            set;
        }
    }

    /// <summary>
    /// The interface to the Capture object, so Plugins can use it.
    /// </summary>
    public interface ICapture : IDisposable
    {
        // The Capture Details
        ICaptureDetails CaptureDetails
        {
            get;
            set;
        }

        // The captured Image
        Image Image
        {
            get;
            set;
        }

        void NullImage();

        Rectangle ScreenBounds
        {
            get;
            set;
        }

        Icon Cursor
        {
            get;
            set;
        }

        // Boolean to specify if the cursor is available
        bool CursorVisible
        {
            get;
            set;
        }

        Point CursorLocation
        {
            get;
            set;
        }

        Point Location
        {
            get;
            set;
        }

        /// <summary>
        /// Crops the capture to the specified rectangle (with Bitmap coordinates!)
        /// </summary>
        /// <param name="cropRectangle">Rectangle with bitmap coordinates</param>
        bool Crop(Rectangle cropRectangle);

        /// <summary>
        /// Apply a translate to the mouse location. e.g. needed for crop
        /// </summary>
        /// <param name="x">x coordinates to move the mouse</param>
        /// <param name="y">y coordinates to move the mouse</param>
        void MoveMouseLocation(int x, int y);

        // / Enable when the elements are usable again.
        ///// <summary>
        ///// Apply a translate to the elements e.g. needed for crop
        ///// </summary>
        ///// <param name="x">x coordinates to move the elements</param>
        ///// <param name="y">y coordinates to move the elements</param>
        //void MoveElements(int x, int y);

        ///// <summary>
        ///// Add a new element to the capture
        ///// </summary>
        ///// <param name="element">Rectangle</param>
        //void AddElement(ICaptureElement element);

        ///// <summary>
        ///// Returns a list of rectangles which represent objects that are "on" the capture
        ///// </summary>
        //List<ICaptureElement> Elements {
        //    get;
        //    set;
        //}
    }
}
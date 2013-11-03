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

using Greenshot.IniFile;
using Greenshot.Plugin;
using GreenshotPlugin.UnmanagedHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GreenshotPlugin.Core
{
    /// <summary>
    /// This Class is used to pass details about the capture around.
    /// The time the Capture was taken and the Title of the window (or a region of) that is captured
    /// </summary>
    public class CaptureDetails : ICaptureDetails
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string filename;
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        private DateTime dateTime;
        public DateTime DateTime
        {
            get { return dateTime; }
            set { dateTime = value; }
        }

        private float dpiX;
        public float DpiX
        {
            get
            {
                return dpiX;
            }
            set
            {
                dpiX = value;
            }
        }

        private float dpiY;
        public float DpiY
        {
            get
            {
                return dpiY;
            }
            set
            {
                dpiY = value;
            }
        }
        private Dictionary<string, string> metaData = new Dictionary<string, string>();
        public Dictionary<string, string> MetaData
        {
            get { return metaData; }
        }

        public void AddMetaData(string key, string value)
        {
            if (metaData.ContainsKey(key))
            {
                metaData[key] = value;
            }
            else
            {
                metaData.Add(key, value);
            }
        }

        private CaptureMode captureMode;
        public CaptureMode CaptureMode
        {
            get { return captureMode; }
            set { captureMode = value; }
        }

        private List<IDestination> captureDestinations = new List<IDestination>();
        public List<IDestination> CaptureDestinations
        {
            get { return captureDestinations; }
            set { captureDestinations = value; }
        }

        public void ClearDestinations()
        {
            captureDestinations.Clear();
        }

        public void RemoveDestination(IDestination destination)
        {
            if (captureDestinations.Contains(destination))
            {
                captureDestinations.Remove(destination);
            }
        }

        public void AddDestination(IDestination captureDestination)
        {
            if (!captureDestinations.Contains(captureDestination))
            {
                captureDestinations.Add(captureDestination);
            }
        }

        public bool HasDestination(string designation)
        {
            foreach (IDestination destination in captureDestinations)
            {
                if (designation.Equals(destination.Designation))
                {
                    return true;
                }
            }
            return false;
        }

        public CaptureDetails()
        {
            dateTime = DateTime.Now;
        }
    }

    /// <summary>
    /// This class is used to pass an instance of the "Capture" around
    /// Having the Bitmap, eventually the Windows Title and cursor all together.
    /// </summary>
    public class Capture : IDisposable, ICapture
    {
        private List<ICaptureElement> elements = new List<ICaptureElement>();

        private Rectangle screenBounds;
        /// <summary>
        /// Get/Set the Screenbounds
        /// </summary>
        public Rectangle ScreenBounds
        {
            get
            {
                if (screenBounds == null)
                {
                    screenBounds = WindowCapture.GetScreenBounds();
                }
                return screenBounds;
            }
            set { screenBounds = value; }
        }

        private Image image;
        /// <summary>
        /// Get/Set the Image
        /// </summary>
        public Image Image
        {
            get { return image; }
            set
            {
                if (image != null)
                {
                    image.Dispose();
                }
                image = value;
                if (value != null)
                {
                    if (value.PixelFormat.Equals(PixelFormat.Format8bppIndexed) || value.PixelFormat.Equals(PixelFormat.Format1bppIndexed) || value.PixelFormat.Equals(PixelFormat.Format4bppIndexed))
                    {
                        LOG.Debug("Converting Bitmap to PixelFormat.Format32bppArgb as we don't support: " + value.PixelFormat);
                        try
                        {
                            // Default Bitmap PixelFormat is Format32bppArgb
                            image = new Bitmap(value);
                        }
                        finally
                        {
                            // Always dispose, even when a exception occured
                            value.Dispose();
                        }
                    }
                    LOG.DebugFormat("Image is set with the following specifications: {0} - {1}", image.Size, image.PixelFormat);
                }
                else
                {
                    LOG.Debug("Image is removed.");
                }
            }
        }

        public void NullImage()
        {
            image = null;
        }

        private Icon cursor;
        /// <summary>
        /// Get/Set the image for the Cursor
        /// </summary>
        public Icon Cursor
        {
            get { return cursor; }
            set
            {
                if (cursor != null)
                {
                    cursor.Dispose();
                }
                cursor = (Icon)value.Clone();
            }
        }

        private bool cursorVisible = false;
        /// <summary>
        /// Set if the cursor is visible
        /// </summary>
        public bool CursorVisible
        {
            get { return cursorVisible; }
            set { cursorVisible = value; }
        }

        private Point cursorLocation = Point.Empty;
        /// <summary>
        /// Get/Set the CursorLocation
        /// </summary>
        public Point CursorLocation
        {
            get { return cursorLocation; }
            set { cursorLocation = value; }
        }

        private Point location = Point.Empty;
        /// <summary>
        /// Get/set the Location
        /// </summary>
        public Point Location
        {
            get { return location; }
            set { location = value; }
        }

        private CaptureDetails captureDetails;
        /// <summary>
        /// Get/set the CaptureDetails
        /// </summary>
        public ICaptureDetails CaptureDetails
        {
            get { return captureDetails; }
            set { captureDetails = (CaptureDetails)value; }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Capture()
        {
            screenBounds = WindowCapture.GetScreenBounds();
            captureDetails = new CaptureDetails();
        }

        /// <summary>
        /// Constructor with Image
        /// Note: the supplied bitmap can be disposed immediately or when constructor is called.
        /// </summary>
        /// <param name="newImage">Image</param>
        public Capture(Image newImage)
            : this()
        {
            Image = newImage;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Capture()
        {
            Dispose(false);
        }

        /// <summary>
        /// The public accessible Dispose
        /// Will call the GarbageCollector to SuppressFinalize, preventing being cleaned twice
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This Dispose is called from the Dispose and the Destructor.
        /// When disposing==true all non-managed resources should be freed too!
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (image != null)
                {
                    image.Dispose();
                }
                if (cursor != null)
                {
                    cursor.Dispose();
                }
            }
            image = null;
            cursor = null;
        }

        /// <summary>
        /// Crops the capture to the specified rectangle (with Bitmap coordinates!)
        /// </summary>
        /// <param name="cropRectangle">Rectangle with bitmap coordinates</param>
        public bool Crop(Rectangle cropRectangle)
        {
            LOG.Debug("Cropping to: " + cropRectangle.ToString());
            if (ImageHelper.Crop(ref image, ref cropRectangle))
            {
                location = cropRectangle.Location;
                // Change mouse location according to the cropRegtangle (including screenbounds) offset
                MoveMouseLocation(-cropRectangle.Location.X, -cropRectangle.Location.Y);
                // Move all the elements
                // TODO: Enable when the elements are usable again.
                // MoveElements(-cropRectangle.Location.X, -cropRectangle.Location.Y);

                // Remove invisible elements
                List<ICaptureElement> newElements = new List<ICaptureElement>();
                foreach (ICaptureElement captureElement in elements)
                {
                    if (captureElement.Bounds.IntersectsWith(cropRectangle))
                    {
                        newElements.Add(captureElement);
                    }
                }
                elements = newElements;

                return true;
            }
            return false;
        }

        /// <summary>
        /// Apply a translate to the mouse location.
        /// e.g. needed for crop
        /// </summary>
        /// <param name="x">x coordinates to move the mouse</param>
        /// <param name="y">y coordinates to move the mouse</param>
        public void MoveMouseLocation(int x, int y)
        {
            cursorLocation.Offset(x, y);
        }

        // TODO: Enable when the elements are usable again.
        ///// <summary>
        ///// Apply a translate to the elements
        ///// e.g. needed for crop
        ///// </summary>
        ///// <param name="x">x coordinates to move the elements</param>
        ///// <param name="y">y coordinates to move the elements</param>
        //public void MoveElements(int x, int y) {
        //    MoveElements(elements, x, y);
        //}

        //private void MoveElements(List<ICaptureElement> listOfElements, int x, int y) {
        //    foreach(ICaptureElement childElement in listOfElements) {
        //        Rectangle bounds = childElement.Bounds;
        //        bounds.Offset(x, y);
        //        childElement.Bounds = bounds;
        //        MoveElements(childElement.Children, x, y);
        //    }
        //}

        ///// <summary>
        ///// Add a new element to the capture
        ///// </summary>
        ///// <param name="element">CaptureElement</param>
        //public void AddElement(ICaptureElement element) {
        //    int match = elements.IndexOf(element);
        //    if (match >= 0) {
        //        if (elements[match].Children.Count < element.Children.Count) {
        //            elements.RemoveAt(match);
        //            elements.Add(element);
        //        }
        //    } else {
        //        elements.Add(element);
        //    }
        //}

        ///// <summary>
        ///// Returns a list of rectangles which represent object that are on the capture
        ///// </summary>
        //public List<ICaptureElement> Elements {
        //    get {
        //        return elements;
        //    }
        //    set {
        //        elements = value;
        //    }
        //}
    }

    /// <summary>
    /// A class representing an element in the capture
    /// </summary>
    public class CaptureElement : ICaptureElement
    {
        public CaptureElement(Rectangle bounds)
        {
            Bounds = bounds;
        }

        public CaptureElement(string name)
        {
            Name = name;
        }

        public CaptureElement(string name, Rectangle bounds)
        {
            Name = name;
            Bounds = bounds;
        }

        private List<ICaptureElement> children = new List<ICaptureElement>();
        public List<ICaptureElement> Children
        {
            get
            {
                return children;
            }
            set
            {
                children = value;
            }
        }

        public string Name
        {
            get;
            set;
        }
        public Rectangle Bounds
        {
            get;
            set;
        }

        // CaptureElements are regarded equal if their bounds are equal. this should be sufficient.
        public override bool Equals(object obj)
        {
            bool ret = false;
            if (obj != null && GetType().Equals(obj.GetType()))
            {
                CaptureElement other = obj as CaptureElement;
                if (Bounds.Equals(other.Bounds))
                {
                    ret = true;
                }
            }
            return ret;
        }

        public override int GetHashCode()
        {
            return Bounds.GetHashCode();
        }
    }

    /// <summary>
    /// The Window Capture code
    /// </summary>
    public class WindowCapture
    {
        private static CoreConfiguration conf = IniConfig.GetIniSection<CoreConfiguration>();

        /// <summary>
        /// Used to cleanup the unmanged resource in the iconInfo for the CaptureCursor method
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [DllImport("gdi32", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        private WindowCapture()
        {
        }

        /// <summary>
        /// Get the bounds of all screens combined.
        /// </summary>
        /// <returns>A Rectangle of the bounds of the entire display area.</returns>
        public static Rectangle GetScreenBounds()
        {
            int left = 0, top = 0, bottom = 0, right = 0;
            foreach (Screen screen in Screen.AllScreens)
            {
                left = Math.Min(left, screen.Bounds.X);
                top = Math.Min(top, screen.Bounds.Y);
                int screenAbsRight = screen.Bounds.X + screen.Bounds.Width;
                int screenAbsBottom = screen.Bounds.Y + screen.Bounds.Height;
                right = Math.Max(right, screenAbsRight);
                bottom = Math.Max(bottom, screenAbsBottom);
            }
            return new Rectangle(left, top, (right + Math.Abs(left)), (bottom + Math.Abs(top)));
        }

        /// <summary>
        /// Retrieves the cursor location safely, accounting for DPI settings in Vista/Windows 7.
        /// <returns>Point with cursor location, relative to the origin of the monitor setup (i.e. negative coordinates are
        /// possible in multiscreen setups)</returns>
        public static Point GetCursorLocation()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                POINT cursorLocation;
                if (User32.GetPhysicalCursorPos(out cursorLocation))
                {
                    return new Point(cursorLocation.X, cursorLocation.Y);
                }
                else
                {
                    Win32Error error = Win32.GetLastErrorCode();
                    LOG.ErrorFormat("Error retrieving PhysicalCursorPos : {0}", Win32.GetMessage(error));
                }
            }
            return new Point(Cursor.Position.X, Cursor.Position.Y);
        }

        /// <summary>
        /// Retrieves the cursor location safely, accounting for DPI settings in Vista/Windows 7. This implementation
        /// can conveniently be used when the cursor location is needed to deal with a fullscreen bitmap.
        /// <returns>Point with cursor location, relative to the top left corner of the monitor setup (which itself might
        /// actually not be on any screen)</returns>
        public static Point GetCursorLocationRelativeToScreenBounds()
        {
            return GetLocationRelativeToScreenBounds(GetCursorLocation());
        }

        /// <summary>
        /// Converts locationRelativeToScreenOrigin to be relative to top left corner of all screen bounds, which might
        /// be different in multiscreen setups. This implementation
        /// can conveniently be used when the cursor location is needed to deal with a fullscreen bitmap.
        /// </summary>
        /// <param name="locationRelativeToScreenOrigin"></param>
        /// <returns></returns>
        public static Point GetLocationRelativeToScreenBounds(Point locationRelativeToScreenOrigin)
        {
            Point ret = locationRelativeToScreenOrigin;
            Rectangle bounds = GetScreenBounds();
            ret.Offset(-bounds.X, -bounds.Y);
            return ret;
        }

        /// <summary>
        /// This method will capture the current Cursor by using User32 Code
        /// </summary>
        /// <returns>A Capture Object with the Mouse Cursor information in it.</returns>
        public static ICapture CaptureCursor(ICapture capture)
        {
            LOG.Debug("Capturing the mouse cursor.");
            if (capture == null)
            {
                capture = new Capture();
            }
            int x, y;
            CursorInfo cursorInfo = new CursorInfo();
            IconInfo iconInfo;
            cursorInfo.cbSize = Marshal.SizeOf(cursorInfo);
            if (User32.GetCursorInfo(out cursorInfo))
            {
                if (cursorInfo.flags == User32.CURSOR_SHOWING)
                {
                    using (SafeIconHandle safeIcon = User32.CopyIcon(cursorInfo.hCursor))
                    {
                        if (User32.GetIconInfo(safeIcon, out iconInfo))
                        {
                            Point cursorLocation = GetCursorLocation();
                            // Allign cursor location to Bitmap coordinates (instead of Screen coordinates)
                            x = cursorLocation.X - iconInfo.xHotspot - capture.ScreenBounds.X;
                            y = cursorLocation.Y - iconInfo.yHotspot - capture.ScreenBounds.Y;
                            // Set the location
                            capture.CursorLocation = new Point(x, y);

                            using (Icon icon = Icon.FromHandle(safeIcon.DangerousGetHandle()))
                            {
                                capture.Cursor = icon;
                            }

                            if (iconInfo.hbmMask != IntPtr.Zero)
                            {
                                DeleteObject(iconInfo.hbmMask);
                            }
                            if (iconInfo.hbmColor != IntPtr.Zero)
                            {
                                DeleteObject(iconInfo.hbmColor);
                            }
                        }
                    }
                }
            }
            return capture;
        }

        /// <summary>
        /// This method will call the CaptureRectangle with the screenbounds, therefor Capturing the whole screen.
        /// </summary>
        /// <returns>A Capture Object with the Screen as an Image</returns>
        public static ICapture CaptureScreen(ICapture capture)
        {
            if (capture == null)
            {
                capture = new Capture();
            }
            return CaptureRectangle(capture, capture.ScreenBounds);
        }

        /// <summary>
        /// Helper method to create an exception that might explain what is wrong while capturing
        /// </summary>
        /// <param name="method">string with current method</param>
        /// <param name="capture">ICapture</param>
        /// <param name="captureBounds">Rectangle of what we want to capture</param>
        /// <returns></returns>
        private static Exception CreateCaptureException(string method, Rectangle captureBounds)
        {
            Exception exceptionToThrow = User32.CreateWin32Exception(method);
            if (!captureBounds.IsEmpty)
            {
                exceptionToThrow.Data.Add("Height", captureBounds.Height);
                exceptionToThrow.Data.Add("Width", captureBounds.Width);
            }
            return exceptionToThrow;
        }

        /// <summary>
        /// Helper method to check if it is allowed to capture the process using DWM
        /// </summary>
        /// <param name="process">Process owning the window</param>
        /// <returns>true if it's allowed</returns>
        public static bool isDWMAllowed(Process process)
        {
            if (process != null)
            {
                if (conf.NoDWMCaptureForProduct != null && conf.NoDWMCaptureForProduct.Count > 0)
                {
                    try
                    {
                        string productName = process.MainModule.FileVersionInfo.ProductName;
                        if (productName != null && conf.NoDWMCaptureForProduct.Contains(productName.ToLower()))
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        LOG.Warn(ex.Message);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Helper method to check if it is allowed to capture the process using GDI
        /// </summary>
        /// <param name="processName">Process owning the window</param>
        /// <returns>true if it's allowed</returns>
        public static bool isGDIAllowed(Process process)
        {
            if (process != null)
            {
                if (conf.NoGDICaptureForProduct != null && conf.NoGDICaptureForProduct.Count > 0)
                {
                    try
                    {
                        string productName = process.MainModule.FileVersionInfo.ProductName;
                        if (productName != null && conf.NoGDICaptureForProduct.Contains(productName.ToLower()))
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        LOG.Warn(ex.Message);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// This method will use User32 code to capture the specified captureBounds from the screen
        /// </summary>
        /// <param name="capture">ICapture where the captured Bitmap will be stored</param>
        /// <param name="captureBounds">Rectangle with the bounds to capture</param>
        /// <returns>A Capture Object with a part of the Screen as an Image</returns>
        public static ICapture CaptureRectangle(ICapture capture, Rectangle captureBounds)
        {
            if (capture == null)
            {
                capture = new Capture();
            }
            capture.Image = CaptureRectangle(captureBounds);
            capture.Location = captureBounds.Location;
            if (capture.CaptureDetails != null)
            {
                ((Bitmap)capture.Image).SetResolution(capture.CaptureDetails.DpiX, capture.CaptureDetails.DpiY);
            }
            if (capture.Image == null)
            {
                return null;
            }
            return capture;
        }

        /// <summary>
        /// This method will use User32 code to capture the specified captureBounds from the screen
        /// </summary>
        /// <param name="captureBounds">Rectangle with the bounds to capture</param>
        /// <returns>Bitmap which is captured from the screen at the location specified by the captureBounds</returns>
        public static Bitmap CaptureRectangle(Rectangle captureBounds)
        {
            Bitmap returnBitmap = null;
            if (captureBounds.Height <= 0 || captureBounds.Width <= 0)
            {
                LOG.Warn("Nothing to capture, ignoring!");
                return null;
            }
            else
            {
                LOG.Debug("CaptureRectangle Called!");
            }
            // .NET GDI+ Solution, according to some post this has a GDI+ leak...
            // See http://connect.microsoft.com/VisualStudio/feedback/details/344752/gdi-object-leak-when-calling-graphics-copyfromscreen
            // Bitmap capturedBitmap = new Bitmap(captureBounds.Width, captureBounds.Height);
            // using (Graphics graphics = Graphics.FromImage(capturedBitmap)) {
            //	graphics.CopyFromScreen(captureBounds.Location, Point.Empty, captureBounds.Size, CopyPixelOperation.CaptureBlt);
            // }
            // capture.Image = capturedBitmap;
            // capture.Location = captureBounds.Location;

            using (SafeWindowDCHandle desktopDCHandle = SafeWindowDCHandle.fromDesktop())
            {
                if (desktopDCHandle.IsInvalid)
                {
                    // Get Exception before the error is lost
                    Exception exceptionToThrow = CreateCaptureException("desktopDCHandle", captureBounds);
                    // throw exception
                    throw exceptionToThrow;
                }

                // create a device context we can copy to
                using (SafeCompatibleDCHandle safeCompatibleDCHandle = GDI32.CreateCompatibleDC(desktopDCHandle))
                {
                    // Check if the device context is there, if not throw an error with as much info as possible!
                    if (safeCompatibleDCHandle.IsInvalid)
                    {
                        // Get Exception before the error is lost
                        Exception exceptionToThrow = CreateCaptureException("CreateCompatibleDC", captureBounds);
                        // throw exception
                        throw exceptionToThrow;
                    }
                    // Create BitmapInfoHeader for CreateDIBSection
                    BitmapInfoHeader bmi = new BitmapInfoHeader(captureBounds.Width, captureBounds.Height, 24);

                    // Make sure the last error is set to 0
                    Win32.SetLastError(0);

                    // create a bitmap we can copy it to, using GetDeviceCaps to get the width/height
                    IntPtr bits0; // not used for our purposes. It returns a pointer to the raw bits that make up the bitmap.
                    using (SafeDibSectionHandle safeDibSectionHandle = GDI32.CreateDIBSection(desktopDCHandle, ref bmi, BitmapInfoHeader.DIB_RGB_COLORS, out bits0, IntPtr.Zero, 0))
                    {
                        if (safeDibSectionHandle.IsInvalid)
                        {
                            // Get Exception before the error is lost
                            Exception exceptionToThrow = CreateCaptureException("CreateDIBSection", captureBounds);
                            exceptionToThrow.Data.Add("hdcDest", safeCompatibleDCHandle.DangerousGetHandle().ToInt32());
                            exceptionToThrow.Data.Add("hdcSrc", desktopDCHandle.DangerousGetHandle().ToInt32());

                            // Throw so people can report the problem
                            throw exceptionToThrow;
                        }
                        else
                        {
                            // select the bitmap object and store the old handle
                            using (SafeSelectObjectHandle selectObject = safeCompatibleDCHandle.SelectObject(safeDibSectionHandle))
                            {
                                // bitblt over (make copy)
                                GDI32.BitBlt(safeCompatibleDCHandle, 0, 0, captureBounds.Width, captureBounds.Height, desktopDCHandle, captureBounds.X, captureBounds.Y, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
                            }

                            // get a .NET image object for it
                            // A suggestion for the "A generic error occurred in GDI+." E_FAIL/0×80004005 error is to re-try...
                            bool success = false;
                            ExternalException exception = null;
                            for (int i = 0; i < 3; i++)
                            {
                                try
                                {
                                    // Collect all screens inside this capture
                                    List<Screen> screensInsideCapture = new List<Screen>();
                                    foreach (Screen screen in Screen.AllScreens)
                                    {
                                        if (screen.Bounds.IntersectsWith(captureBounds))
                                        {
                                            screensInsideCapture.Add(screen);
                                        }
                                    }
                                    // Check all all screens are of an equal size
                                    bool offscreenContent = false;
                                    using (Region captureRegion = new Region(captureBounds))
                                    {
                                        // Exclude every visible part
                                        foreach (Screen screen in screensInsideCapture)
                                        {
                                            captureRegion.Exclude(screen.Bounds);
                                        }
                                        // If the region is not empty, we have "offscreenContent"
                                        using (Graphics screenGraphics = Graphics.FromHwnd(User32.GetDesktopWindow()))
                                        {
                                            offscreenContent = !captureRegion.IsEmpty(screenGraphics);
                                        }
                                    }
                                    // Check if we need to have a transparent background, needed for offscreen content
                                    if (offscreenContent)
                                    {
                                        using (Bitmap tmpBitmap = Bitmap.FromHbitmap(safeDibSectionHandle.DangerousGetHandle()))
                                        {
                                            // Create a new bitmap which has a transparent background
                                            returnBitmap = ImageHelper.CreateEmpty(tmpBitmap.Width, tmpBitmap.Height, PixelFormat.Format32bppArgb, Color.Transparent, tmpBitmap.HorizontalResolution, tmpBitmap.VerticalResolution);
                                            // Content will be copied here
                                            using (Graphics graphics = Graphics.FromImage(returnBitmap))
                                            {
                                                // For all screens copy the content to the new bitmap
                                                foreach (Screen screen in Screen.AllScreens)
                                                {
                                                    Rectangle screenBounds = screen.Bounds;
                                                    // Make sure the bounds are offsetted to the capture bounds
                                                    screenBounds.Offset(-captureBounds.X, -captureBounds.Y);
                                                    graphics.DrawImage(tmpBitmap, screenBounds, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height, GraphicsUnit.Pixel);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // All screens, which are inside the capture, are of equal size
                                        // assign image to Capture, the image will be disposed there..
                                        returnBitmap = Bitmap.FromHbitmap(safeDibSectionHandle.DangerousGetHandle());
                                    }
                                    // We got through the capture without exception
                                    success = true;
                                    break;
                                }
                                catch (ExternalException ee)
                                {
                                    LOG.Warn("Problem getting bitmap at try " + i + " : ", ee);
                                    exception = ee;
                                }
                            }
                            if (!success)
                            {
                                LOG.Error("Still couldn't create Bitmap!");
                                throw exception;
                            }
                        }
                    }
                }
            }
            return returnBitmap;
        }
    }
}
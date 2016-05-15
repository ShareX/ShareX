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

using Greenshot.IniFile;
using Greenshot.Plugin;
using GreenshotPlugin.Controls;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Encoder = System.Drawing.Imaging.Encoder;

namespace GreenshotPlugin.Core
{
    /// <summary>
    /// Description of ImageOutput.
    /// </summary>
    public static class ImageOutput
    {
        private static readonly CoreConfiguration conf = IniConfig.GetIniSection<CoreConfiguration>();
        private static readonly int PROPERTY_TAG_SOFTWARE_USED = 0x0131;
        private static Cache<string, string> tmpFileCache = new Cache<string, string>(10 * 60 * 60, RemoveExpiredTmpFile);

        /// <summary>
        /// Creates a PropertyItem (Metadata) to store with the image.
        /// For the possible ID's see: http://msdn.microsoft.com/de-de/library/system.drawing.imaging.propertyitem.id(v=vs.80).aspx
        /// This code uses Reflection to create a PropertyItem, although it's not adviced it's not as stupid as having a image in the project so we can read a PropertyItem from that!
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="text">Text</param>
        /// <returns></returns>
        private static PropertyItem CreatePropertyItem(int id, string text)
        {
            PropertyItem propertyItem = null;
            try
            {
                ConstructorInfo ci = typeof(PropertyItem).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, new Type[] { }, null);
                propertyItem = (PropertyItem)ci.Invoke(null);
                // Make sure it's of type string
                propertyItem.Type = 2;
                // Set the ID
                propertyItem.Id = id;
                // Set the text
                byte[] byteString = Encoding.ASCII.GetBytes(text + " ");
                // Set Zero byte for String end.
                byteString[byteString.Length - 1] = 0;
                propertyItem.Value = byteString;
                propertyItem.Len = text.Length + 1;
            }
            catch (Exception e)
            {
                LOG.WarnFormat("Error creating a PropertyItem: {0}", e.Message);
            }
            return propertyItem;
        }

        #region save

        /// <summary>
        /// Saves ISurface to stream with specified output settings
        /// </summary>
        /// <param name="surface">ISurface to save</param>
        /// <param name="stream">Stream to save to</param>
        /// <param name="outputSettings">SurfaceOutputSettings</param>
        public static void SaveToStream(ISurface surface, Stream stream, SurfaceOutputSettings outputSettings)
        {
            Image imageToSave;
            bool disposeImage = CreateImageFromSurface(surface, outputSettings, out imageToSave);
            SaveToStream(imageToSave, surface, stream, outputSettings);
            // cleanup if needed
            if (disposeImage && imageToSave != null)
            {
                imageToSave.Dispose();
            }
        }

        /// <summary>
        /// Saves image to stream with specified quality
        /// To prevent problems with GDI version of before Windows 7:
        /// the stream is checked if it's seekable and if needed a MemoryStream as "cache" is used.
        /// </summary>
        /// <param name="imageToSave">image to save</param>
        /// <param name="surface">surface for the elements, needed if the greenshot format is used</param>
        /// <param name="stream">Stream to save to</param>
        /// <param name="outputSettings">SurfaceOutputSettings</param>
        public static void SaveToStream(Image imageToSave, ISurface surface, Stream stream, SurfaceOutputSettings outputSettings)
        {
            ImageFormat imageFormat;
            bool useMemoryStream = false;
            MemoryStream memoryStream = null;
            if (outputSettings.Format == OutputFormat.greenshot && surface == null)
            {
                throw new ArgumentException("Surface needs to be se when using OutputFormat.Greenshot");
            }

            try
            {
                switch (outputSettings.Format)
                {
                    case OutputFormat.bmp:
                        imageFormat = ImageFormat.Bmp;
                        break;
                    case OutputFormat.gif:
                        imageFormat = ImageFormat.Gif;
                        break;
                    case OutputFormat.jpg:
                        imageFormat = ImageFormat.Jpeg;
                        break;
                    case OutputFormat.tiff:
                        imageFormat = ImageFormat.Tiff;
                        break;
                    default:
                        // Problem with non-seekable streams most likely doesn't happen with Windows 7 (OS Version 6.1 and later)
                        // http://stackoverflow.com/questions/8349260/generic-gdi-error-on-one-machine-but-not-the-other
                        if (!stream.CanSeek)
                        {
                            int majorVersion = Environment.OSVersion.Version.Major;
                            int minorVersion = Environment.OSVersion.Version.Minor;
                            if (majorVersion < 6 || (majorVersion == 6 && minorVersion == 0))
                            {
                                useMemoryStream = true;
                                LOG.Warn("Using memorystream prevent an issue with saving to a non seekable stream.");
                            }
                        }
                        imageFormat = ImageFormat.Png;
                        break;
                }
                LOG.DebugFormat("Saving image to stream with Format {0} and PixelFormat {1}", imageFormat, imageToSave.PixelFormat);

                // Check if we want to use a memory stream, to prevent a issue which happens with Windows before "7".
                // The save is made to the targetStream, this is directed to either the MemoryStream or the original
                Stream targetStream = stream;
                if (useMemoryStream)
                {
                    memoryStream = new MemoryStream();
                    targetStream = memoryStream;
                }

                if (Equals(imageFormat, ImageFormat.Jpeg))
                {
                    bool foundEncoder = false;
                    foreach (ImageCodecInfo imageCodec in ImageCodecInfo.GetImageEncoders())
                    {
                        if (imageCodec.FormatID == imageFormat.Guid)
                        {
                            EncoderParameters parameters = new EncoderParameters(1);
                            parameters.Param[0] = new EncoderParameter(Encoder.Quality, outputSettings.JPGQuality);
                            // Removing transparency if it's not supported in the output
                            if (Image.IsAlphaPixelFormat(imageToSave.PixelFormat))
                            {
                                Image nonAlphaImage = ImageHelper.Clone(imageToSave, PixelFormat.Format24bppRgb);
                                AddTag(nonAlphaImage);
                                nonAlphaImage.Save(targetStream, imageCodec, parameters);
                                nonAlphaImage.Dispose();
                                nonAlphaImage = null;
                            }
                            else
                            {
                                AddTag(imageToSave);
                                imageToSave.Save(targetStream, imageCodec, parameters);
                            }
                            foundEncoder = true;
                            break;
                        }
                    }
                    if (!foundEncoder)
                    {
                        throw new ApplicationException("No JPG encoder found, this should not happen.");
                    }
                }
                else
                {
                    bool needsDispose = false;
                    // Removing transparency if it's not supported in the output
                    if (!Equals(imageFormat, ImageFormat.Png) && Image.IsAlphaPixelFormat(imageToSave.PixelFormat))
                    {
                        imageToSave = ImageHelper.Clone(imageToSave, PixelFormat.Format24bppRgb);
                        needsDispose = true;
                    }
                    AddTag(imageToSave);
                    // Added for OptiPNG
                    bool processed = false;
                    if (Equals(imageFormat, ImageFormat.Png) && !string.IsNullOrEmpty(conf.OptimizePNGCommand))
                    {
                        processed = ProcessPNGImageExternally(imageToSave, targetStream);
                    }
                    if (!processed)
                    {
                        imageToSave.Save(targetStream, imageFormat);
                    }
                    if (needsDispose)
                    {
                        imageToSave.Dispose();
                        imageToSave = null;
                    }
                }

                // If we used a memory stream, we need to stream the memory stream to the original stream.
                if (useMemoryStream)
                {
                    memoryStream.WriteTo(stream);
                }

                // Output the surface elements, size and marker to the stream
                if (outputSettings.Format == OutputFormat.greenshot)
                {
                    using (MemoryStream tmpStream = new MemoryStream())
                    {
                        long bytesWritten = surface.SaveElementsToStream(tmpStream);
                        using (BinaryWriter writer = new BinaryWriter(tmpStream))
                        {
                            writer.Write(bytesWritten);
                            Version v = Assembly.GetExecutingAssembly().GetName().Version;
                            byte[] marker = Encoding.ASCII.GetBytes(String.Format("Greenshot{0:00}.{1:00}", v.Major, v.Minor));
                            writer.Write(marker);
                            tmpStream.WriteTo(stream);
                        }
                    }
                }
            }
            finally
            {
                if (memoryStream != null)
                {
                    memoryStream.Dispose();
                }
            }
        }

        /// <summary>
        /// Write the passed Image to a tmp-file and call an external process, than read the file back and write it to the targetStream
        /// </summary>
        /// <param name="imageToProcess">Image to pass to the external process</param>
        /// <param name="targetStream">stream to write the processed image to</param>
        /// <returns></returns>
        private static bool ProcessPNGImageExternally(Image imageToProcess, Stream targetStream)
        {
            if (string.IsNullOrEmpty(conf.OptimizePNGCommand))
            {
                return false;
            }
            if (!File.Exists(conf.OptimizePNGCommand))
            {
                LOG.WarnFormat("Can't find 'OptimizePNGCommand' {0}", conf.OptimizePNGCommand);
                return false;
            }
            string tmpFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".png");
            try
            {
                using (FileStream tmpStream = File.Create(tmpFileName))
                {
                    LOG.DebugFormat("Writing png to tmp file: {0}", tmpFileName);
                    imageToProcess.Save(tmpStream, ImageFormat.Png);
                    if (LOG.IsDebugEnabled)
                    {
                        LOG.DebugFormat("File size before processing {0}", new FileInfo(tmpFileName).Length);
                    }
                }
                if (LOG.IsDebugEnabled)
                {
                    LOG.DebugFormat("Starting : {0}", conf.OptimizePNGCommand);
                }

                ProcessStartInfo processStartInfo = new ProcessStartInfo(conf.OptimizePNGCommand);
                processStartInfo.Arguments = string.Format(conf.OptimizePNGCommandArguments, tmpFileName);
                processStartInfo.CreateNoWindow = true;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.UseShellExecute = false;
                using (Process process = Process.Start(processStartInfo))
                {
                    if (process != null)
                    {
                        process.WaitForExit();
                        if (process.ExitCode == 0)
                        {
                            if (LOG.IsDebugEnabled)
                            {
                                LOG.DebugFormat("File size after processing {0}", new FileInfo(tmpFileName).Length);
                                LOG.DebugFormat("Reading back tmp file: {0}", tmpFileName);
                            }
                            byte[] processedImage = File.ReadAllBytes(tmpFileName);
                            targetStream.Write(processedImage, 0, processedImage.Length);
                            return true;
                        }
                        LOG.ErrorFormat("Error while processing PNG image: {0}", process.ExitCode);
                        LOG.ErrorFormat("Output: {0}", process.StandardOutput.ReadToEnd());
                        LOG.ErrorFormat("Error: {0}", process.StandardError.ReadToEnd());
                    }
                }
            }
            catch (Exception e)
            {
                LOG.Error("Error while processing PNG image: ", e);
            }
            finally
            {
                if (File.Exists(tmpFileName))
                {
                    LOG.DebugFormat("Cleaning up tmp file: {0}", tmpFileName);
                    File.Delete(tmpFileName);
                }
            }
            return false;
        }

        /// <summary>
        /// Create an image from a surface with the settings from the output settings applied
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="outputSettings"></param>
        /// <param name="imageToSave"></param>
        /// <returns>true if the image must be disposed</returns>
        public static bool CreateImageFromSurface(ISurface surface, SurfaceOutputSettings outputSettings, out Image imageToSave)
        {
            bool disposeImage = false;

            if (outputSettings.Format == OutputFormat.greenshot || outputSettings.SaveBackgroundOnly)
            {
                // We save the image of the surface, this should not be disposed
                imageToSave = surface.Image;
            }
            else
            {
                // We create the export image of the surface to save
                imageToSave = surface.GetImageForExport();
                disposeImage = true;
            }

            // The following block of modifications should be skipped when saving the greenshot format, no effects or otherwise!
            if (outputSettings.Format == OutputFormat.greenshot)
            {
                return disposeImage;
            }
            Image tmpImage;
            if (outputSettings.Effects != null && outputSettings.Effects.Count > 0)
            {
                // apply effects, if there are any
                using (Matrix matrix = new Matrix())
                {
                    tmpImage = ImageHelper.ApplyEffects(imageToSave, outputSettings.Effects, matrix);
                }
                if (tmpImage != null)
                {
                    if (disposeImage)
                    {
                        imageToSave.Dispose();
                    }
                    imageToSave = tmpImage;
                    disposeImage = true;
                }
            }

            // check for color reduction, forced or automatically, only when the DisableReduceColors is false
            if (outputSettings.DisableReduceColors || (!conf.OutputFileAutoReduceColors && !outputSettings.ReduceColors))
            {
                return disposeImage;
            }
            bool isAlpha = Image.IsAlphaPixelFormat(imageToSave.PixelFormat);
            if (outputSettings.ReduceColors || (!isAlpha && conf.OutputFileAutoReduceColors))
            {
                using (var quantizer = new WuQuantizer((Bitmap)imageToSave))
                {
                    int colorCount = quantizer.GetColorCount();
                    LOG.InfoFormat("Image with format {0} has {1} colors", imageToSave.PixelFormat, colorCount);
                    if (!outputSettings.ReduceColors && colorCount >= 256)
                    {
                        return disposeImage;
                    }
                    try
                    {
                        LOG.Info("Reducing colors on bitmap to 256.");
                        tmpImage = quantizer.GetQuantizedImage(conf.OutputFileReduceColorsTo);
                        if (disposeImage)
                        {
                            imageToSave.Dispose();
                        }
                        imageToSave = tmpImage;
                        // Make sure the "new" image is disposed
                        disposeImage = true;
                    }
                    catch (Exception e)
                    {
                        LOG.Warn("Error occurred while Quantizing the image, ignoring and using original. Error: ", e);
                    }
                }
            }
            else if (isAlpha && !outputSettings.ReduceColors)
            {
                LOG.Info("Skipping 'optional' color reduction as the image has alpha");
            }
            return disposeImage;
        }

        /// <summary>
        /// Add the greenshot property!
        /// </summary>
        /// <param name="imageToSave"></param>
        private static void AddTag(Image imageToSave)
        {
            // Create meta-data
            PropertyItem softwareUsedPropertyItem = CreatePropertyItem(PROPERTY_TAG_SOFTWARE_USED, "Greenshot");
            if (softwareUsedPropertyItem != null)
            {
                try
                {
                    imageToSave.SetPropertyItem(softwareUsedPropertyItem);
                }
                catch (Exception)
                {
                    LOG.WarnFormat("Couldn't set property {0}", softwareUsedPropertyItem.Id);
                }
            }
        }

        /// <summary>
        /// Load a Greenshot surface
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="returnSurface"></param>
        /// <returns></returns>
        public static ISurface LoadGreenshotSurface(string fullPath, ISurface returnSurface)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }
            Image fileImage;
            LOG.InfoFormat("Loading image from file {0}", fullPath);
            // Fixed lock problem Bug #3431881
            using (Stream surfaceFileStream = File.OpenRead(fullPath))
            {
                // And fixed problem that the bitmap stream is disposed... by Cloning the image
                // This also ensures the bitmap is correctly created

                // We create a copy of the bitmap, so everything else can be disposed
                surfaceFileStream.Position = 0;
                using (Image tmpImage = Image.FromStream(surfaceFileStream, true, true))
                {
                    LOG.DebugFormat("Loaded {0} with Size {1}x{2} and PixelFormat {3}", fullPath, tmpImage.Width, tmpImage.Height, tmpImage.PixelFormat);
                    fileImage = ImageHelper.Clone(tmpImage);
                }
                // Start at -14 read "GreenshotXX.YY" (XX=Major, YY=Minor)
                const int markerSize = 14;
                surfaceFileStream.Seek(-markerSize, SeekOrigin.End);
                string greenshotMarker;
                using (StreamReader streamReader = new StreamReader(surfaceFileStream))
                {
                    greenshotMarker = streamReader.ReadToEnd();
                    if (!greenshotMarker.StartsWith("Greenshot"))
                    {
                        throw new ArgumentException(string.Format("{0} is not a Greenshot file!", fullPath));
                    }
                    LOG.InfoFormat("Greenshot file format: {0}", greenshotMarker);
                    const int filesizeLocation = 8 + markerSize;
                    surfaceFileStream.Seek(-filesizeLocation, SeekOrigin.End);
                    using (BinaryReader reader = new BinaryReader(surfaceFileStream))
                    {
                        long bytesWritten = reader.ReadInt64();
                        surfaceFileStream.Seek(-(bytesWritten + filesizeLocation), SeekOrigin.End);
                        returnSurface.LoadElementsFromStream(surfaceFileStream);
                    }
                }
            }
            if (fileImage != null)
            {
                returnSurface.Image = fileImage;
                LOG.InfoFormat("Information about file {0}: {1}x{2}-{3} Resolution {4}x{5}", fullPath, fileImage.Width, fileImage.Height, fileImage.PixelFormat, fileImage.HorizontalResolution, fileImage.VerticalResolution);
            }
            return returnSurface;
        }

        /// <summary>
        /// Saves image to specific path with specified quality
        /// </summary>
        public static void Save(ISurface surface, string fullPath, bool allowOverwrite, SurfaceOutputSettings outputSettings, bool copyPathToClipboard)
        {
            fullPath = FilenameHelper.MakeFQFilenameSafe(fullPath);
            string path = Path.GetDirectoryName(fullPath);

            // check whether path exists - if not create it
            if (path != null)
            {
                DirectoryInfo di = new DirectoryInfo(path);
                if (!di.Exists)
                {
                    Directory.CreateDirectory(di.FullName);
                }
            }

            if (!allowOverwrite && File.Exists(fullPath))
            {
                ArgumentException throwingException = new ArgumentException("File '" + fullPath + "' already exists.");
                throwingException.Data.Add("fullPath", fullPath);
                throw throwingException;
            }
            LOG.DebugFormat("Saving surface to {0}", fullPath);
            // Create the stream and call SaveToStream
            using (FileStream stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                SaveToStream(surface, stream, outputSettings);
            }

            if (copyPathToClipboard)
            {
                ClipboardHelper.SetClipboardData(fullPath);
            }
        }

        /// <summary>
        /// Get the OutputFormat for a filename
        /// </summary>
        /// <param name="fullPath">filename (can be a complete path)</param>
        /// <returns>OutputFormat</returns>
        public static OutputFormat FormatForFilename(string fullPath)
        {
            // Fix for bug 2912959
            string extension = fullPath.Substring(fullPath.LastIndexOf(".") + 1);
            OutputFormat format = OutputFormat.png;
            try
            {
                format = (OutputFormat)Enum.Parse(typeof(OutputFormat), extension.ToLower());
            }
            catch (ArgumentException ae)
            {
                LOG.Warn("Couldn't parse extension: " + extension, ae);
            }
            return format;
        }

        #endregion save

        #region save-as

        /// <summary>
        /// Save with showing a dialog
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="captureDetails"></param>
        /// <returns>Path to filename</returns>
        public static string SaveWithDialog(ISurface surface, ICaptureDetails captureDetails)
        {
            string returnValue = null;
            using (SaveImageFileDialog saveImageFileDialog = new SaveImageFileDialog(captureDetails))
            {
                DialogResult dialogResult = saveImageFileDialog.ShowDialog();
                if (dialogResult.Equals(DialogResult.OK))
                {
                    try
                    {
                        string fileNameWithExtension = saveImageFileDialog.FileNameWithExtension;
                        SurfaceOutputSettings outputSettings = new SurfaceOutputSettings(FormatForFilename(fileNameWithExtension));
                        if (conf.OutputFilePromptQuality)
                        {
                            QualityDialog qualityDialog = new QualityDialog(outputSettings);
                            qualityDialog.ShowDialog();
                        }
                        // For now we always overwrite, should be changed
                        Save(surface, fileNameWithExtension, true, outputSettings, conf.OutputFileCopyPathToClipboard);
                        returnValue = fileNameWithExtension;
                        IniConfig.Save();
                    }
                    catch (ExternalException)
                    {
                        MessageBox.Show(string.Format("Cannot save file to {0}.\r\nPlease check write accessibility of the selected storage location.",
                            saveImageFileDialog.FileName).Replace(@"\\", @"\"), "Error");
                    }
                }
            }
            return returnValue;
        }

        #endregion save-as

        /// <summary>
        /// Create a tmpfile which has the name like in the configured pattern.
        /// Used e.g. by the email export
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="captureDetails"></param>
        /// <param name="outputSettings"></param>
        /// <returns>Path to image file</returns>
        public static string SaveNamedTmpFile(ISurface surface, ICaptureDetails captureDetails, SurfaceOutputSettings outputSettings)
        {
            string pattern = conf.OutputFileFilenamePattern;
            if (pattern == null || string.IsNullOrEmpty(pattern.Trim()))
            {
                pattern = "greenshot ${capturetime}";
            }
            string filename = FilenameHelper.GetFilenameFromPattern(pattern, outputSettings.Format, captureDetails);
            // Prevent problems with "other characters", which causes a problem in e.g. Outlook 2007 or break our HTML
            filename = Regex.Replace(filename, @"[^\d\w\.]", "_");
            // Remove multiple "_"
            filename = Regex.Replace(filename, @"_+", "_");
            string tmpFile = Path.Combine(Path.GetTempPath(), filename);

            LOG.Debug("Creating TMP File: " + tmpFile);

            // Catching any exception to prevent that the user can't write in the directory.
            // This is done for e.g. bugs #2974608, #2963943, #2816163, #2795317, #2789218
            try
            {
                Save(surface, tmpFile, true, outputSettings, false);
                tmpFileCache.Add(tmpFile, tmpFile);
            }
            catch (Exception e)
            {
                // Show the problem
                MessageBox.Show(e.Message, "Error");
                // when save failed we present a SaveWithDialog
                tmpFile = SaveWithDialog(surface, captureDetails);
            }
            return tmpFile;
        }

        /// <summary>
        /// Remove a tmpfile which was created by SaveNamedTmpFile
        /// Used e.g. by the email export
        /// </summary>
        /// <param name="tmpfile"></param>
        /// <returns>true if it worked</returns>
        public static bool DeleteNamedTmpFile(string tmpfile)
        {
            LOG.Debug("Deleting TMP File: " + tmpfile);
            try
            {
                if (File.Exists(tmpfile))
                {
                    File.Delete(tmpfile);
                    tmpFileCache.Remove(tmpfile);
                }
                return true;
            }
            catch (Exception ex)
            {
                LOG.Warn("Error deleting tmp file: ", ex);
            }
            return false;
        }

        /// <summary>
        /// Helper method to create a temp image file
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="outputSettings"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        public static string SaveToTmpFile(ISurface surface, SurfaceOutputSettings outputSettings, string destinationPath)
        {
            string tmpFile = Path.GetRandomFileName() + "." + outputSettings.Format.ToString();
            // Prevent problems with "other characters", which could cause problems
            tmpFile = Regex.Replace(tmpFile, @"[^\d\w\.]", "");
            if (destinationPath == null)
            {
                destinationPath = Path.GetTempPath();
            }
            string tmpPath = Path.Combine(destinationPath, tmpFile);
            LOG.Debug("Creating TMP File : " + tmpPath);

            try
            {
                Save(surface, tmpPath, true, outputSettings, false);
                tmpFileCache.Add(tmpPath, tmpPath);
            }
            catch (Exception)
            {
                return null;
            }
            return tmpPath;
        }

        /// <summary>
        /// Cleanup all created tmpfiles
        /// </summary>
        public static void RemoveTmpFiles()
        {
            foreach (string tmpFile in tmpFileCache.Elements)
            {
                if (File.Exists(tmpFile))
                {
                    LOG.DebugFormat("Removing old temp file {0}", tmpFile);
                    File.Delete(tmpFile);
                }
                tmpFileCache.Remove(tmpFile);
            }
        }

        /// <summary>
        /// Cleanup handler for expired tempfiles
        /// </summary>
        /// <param name="filekey"></param>
        /// <param name="filename"></param>
        private static void RemoveExpiredTmpFile(string filekey, object filename)
        {
            string path = filename as string;
            if (path != null && File.Exists(path))
            {
                LOG.DebugFormat("Removing expired file {0}", path);
                File.Delete(path);
            }
        }
    }
}
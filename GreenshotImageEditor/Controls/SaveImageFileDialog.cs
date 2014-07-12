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
using GreenshotPlugin.Core;
using System;
using System.IO;
using System.Windows.Forms;

namespace GreenshotPlugin.Controls
{
    /// <summary>
    /// Custom dialog for saving images, wraps SaveFileDialog.
    /// For some reason SFD is sealed :(
    /// </summary>
    public class SaveImageFileDialog : IDisposable
    {
        private static CoreConfiguration conf = IniConfig.GetIniSection<CoreConfiguration>();
        protected SaveFileDialog saveFileDialog;
        private FilterOption[] filterOptions;
        private DirectoryInfo eagerlyCreatedDirectory;
        private ICaptureDetails captureDetails = null;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (saveFileDialog != null)
                {
                    saveFileDialog.Dispose();
                    saveFileDialog = null;
                }
            }
        }

        public SaveImageFileDialog()
        {
            init();
        }

        public SaveImageFileDialog(ICaptureDetails captureDetails)
        {
            this.captureDetails = captureDetails;
            init();
        }

        private void init()
        {
            saveFileDialog = new SaveFileDialog();
            applyFilterOptions();
            string initialDirectory = null;
            try
            {
                initialDirectory = Path.GetDirectoryName(conf.OutputFileAsFullpath);
            }
            catch
            {
                LOG.WarnFormat("OutputFileAsFullpath was set to {0}, ignoring due to problem in path.", conf.OutputFileAsFullpath);
            }

            if (!string.IsNullOrEmpty(initialDirectory) && Directory.Exists(initialDirectory))
            {
                saveFileDialog.InitialDirectory = initialDirectory;
            }
            else if (Directory.Exists(conf.OutputFilePath))
            {
                saveFileDialog.InitialDirectory = conf.OutputFilePath;
            }
            // The following property fixes a problem that the directory where we save is locked (bug #2899790)
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.CheckPathExists = false;
            saveFileDialog.AddExtension = true;
            ApplySuggestedValues();
        }

        private void applyFilterOptions()
        {
            prepareFilterOptions();
            string fdf = "";
            int preselect = 0;
            for (int i = 0; i < filterOptions.Length; i++)
            {
                FilterOption fo = filterOptions[i];
                fdf += fo.Label + "|*." + fo.Extension + "|";
                if (conf.OutputFileAsFullpath.EndsWith(fo.Extension, StringComparison.CurrentCultureIgnoreCase)) preselect = i;
            }
            fdf = fdf.Substring(0, fdf.Length - 1);
            saveFileDialog.Filter = fdf;
            saveFileDialog.FilterIndex = preselect + 1;
        }

        private void prepareFilterOptions()
        {
            OutputFormat[] supportedImageFormats = (OutputFormat[])Enum.GetValues(typeof(OutputFormat));
            filterOptions = new FilterOption[supportedImageFormats.Length];
            for (int i = 0; i < filterOptions.Length; i++)
            {
                string ifo = supportedImageFormats[i].ToString();
                if (ifo.ToLower().Equals("jpeg")) ifo = "Jpg"; // we dont want no jpeg files, so let the dialog check for jpg
                FilterOption fo = new FilterOption();
                fo.Label = ifo.ToUpper();
                fo.Extension = ifo.ToLower();
                filterOptions.SetValue(fo, i);
            }
        }

        /// <summary>
        /// filename exactly as typed in the filename field
        /// </summary>
        public string FileName
        {
            get { return saveFileDialog.FileName; }
            set { saveFileDialog.FileName = value; }
        }

        /// <summary>
        /// initial directory of the dialog
        /// </summary>
        public string InitialDirectory
        {
            get { return saveFileDialog.InitialDirectory; }
            set { saveFileDialog.InitialDirectory = value; }
        }

        /// <summary>
        /// returns filename as typed in the filename field with extension.
        /// if filename field value ends with selected extension, the value is just returned.
        /// otherwise, the selected extension is appended to the filename.
        /// </summary>
        public string FileNameWithExtension
        {
            get
            {
                string fn = saveFileDialog.FileName;
                // if the filename contains a valid extension, which is the same like the selected filter item's extension, the filename is okay
                if (fn.EndsWith(Extension, StringComparison.CurrentCultureIgnoreCase)) return fn;
                // otherwise we just add the selected filter item's extension
                else return fn + "." + Extension;
            }
            set
            {
                FileName = Path.GetFileNameWithoutExtension(value);
                Extension = Path.GetExtension(value);
            }
        }

        /// <summary>
        /// gets or sets selected extension
        /// </summary>
        public string Extension
        {
            get
            {
                return filterOptions[saveFileDialog.FilterIndex - 1].Extension;
            }
            set
            {
                for (int i = 0; i < filterOptions.Length; i++)
                {
                    if (value.Equals(filterOptions[i].Extension, StringComparison.CurrentCultureIgnoreCase))
                    {
                        saveFileDialog.FilterIndex = i + 1;
                    }
                }
            }
        }

        public DialogResult ShowDialog()
        {
            DialogResult ret = saveFileDialog.ShowDialog();
            CleanUp();
            return ret;
        }

        /// <summary>
        /// sets InitialDirectory and FileName property of a SaveFileDialog smartly, considering default pattern and last used path
        /// </summary>
        /// <param name="sfd">a SaveFileDialog instance</param>
        private void ApplySuggestedValues()
        {
            // build the full path and set dialog properties
            FileName = FilenameHelper.GetFilenameWithoutExtensionFromPattern(conf.OutputFileFilenamePattern, captureDetails);
        }

        private string GetRootDirFromConfig()
        {
            string rootDir = conf.OutputFilePath;
            rootDir = FilenameHelper.FillVariables(rootDir, false);
            return rootDir;
        }

        private class FilterOption
        {
            public string Label;
            public string Extension;
        }

        private void CleanUp()
        {
            // fix for bug #3379053
            try
            {
                if (eagerlyCreatedDirectory != null && eagerlyCreatedDirectory.GetFiles().Length == 0 && eagerlyCreatedDirectory.GetDirectories().Length == 0)
                {
                    eagerlyCreatedDirectory.Delete();
                    eagerlyCreatedDirectory = null;
                }
            }
            catch (Exception e)
            {
                LOG.WarnFormat("Couldn't cleanup directory due to: {0}", e.Message);
                eagerlyCreatedDirectory = null;
            }
        }

        private string CreateDirectoryIfNotExists(string fullPath)
        {
            string dirName = null;
            try
            {
                dirName = Path.GetDirectoryName(fullPath);
                DirectoryInfo di = new DirectoryInfo(dirName);
                if (!di.Exists)
                {
                    di = Directory.CreateDirectory(dirName);
                    eagerlyCreatedDirectory = di;
                }
            }
            catch (Exception e)
            {
                LOG.Error("Error in CreateDirectoryIfNotExists", e);
            }
            return dirName;
        }
    }
}
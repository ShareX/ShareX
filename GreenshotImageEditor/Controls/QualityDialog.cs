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

namespace GreenshotPlugin.Controls
{
    /// <summary>
    /// Description of JpegQualityDialog.
    /// </summary>
    internal partial class QualityDialog : GreenshotForm
    {
        private static CoreConfiguration conf = IniConfig.GetIniSection<CoreConfiguration>();
        public SurfaceOutputSettings Settings
        {
            get;
            set;
        }

        public QualityDialog(SurfaceOutputSettings outputSettings)
        {
            Settings = outputSettings;
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            Icon = GreenshotResources.getGreenshotIcon();

            checkBox_reduceColors.Checked = Settings.ReduceColors;
            trackBarJpegQuality.Enabled = OutputFormat.jpg.Equals(outputSettings.Format);
            trackBarJpegQuality.Value = Settings.JPGQuality;
            textBoxJpegQuality.Enabled = OutputFormat.jpg.Equals(outputSettings.Format);
            textBoxJpegQuality.Text = Settings.JPGQuality.ToString();
        }

        private void Button_okClick(object sender, EventArgs e)
        {
            Settings.JPGQuality = trackBarJpegQuality.Value;
            Settings.ReduceColors = checkBox_reduceColors.Checked;
            if (checkbox_dontaskagain.Checked)
            {
                conf.OutputFileJpegQuality = Settings.JPGQuality;
                conf.OutputFilePromptQuality = false;
                conf.OutputFileReduceColors = Settings.ReduceColors;
                IniConfig.Save();
            }
        }

        private void TrackBarJpegQualityScroll(object sender, EventArgs e)
        {
            textBoxJpegQuality.Text = trackBarJpegQuality.Value.ToString();
        }
    }
}
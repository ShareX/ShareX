#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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

using ShareX.HelpersLib;
using ShareX.UploadersLib.OtherServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public partial class OCRSpaceResultForm : Form
    {
        public Stream Data { get; private set; }
        public string Filename { get; private set; }
        public OCRSpaceLanguages Language { get; set; } = OCRSpaceLanguages.eng;
        public string Result { get; set; }

        public OCRSpaceResultForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            cbLanguages.Items.AddRange(Helpers.GetEnumDescriptions<OCRSpaceLanguages>());
        }

        public OCRSpaceResultForm(Stream data, string filename) : this()
        {
            Data = data;
            Filename = filename;
        }

        private void OCRSpaceResultForm_Shown(object sender, EventArgs e)
        {
            UpdateValues();

            if (string.IsNullOrEmpty(Result))
            {
                StartOCR();
            }
        }

        public void UpdateValues()
        {
            cbLanguages.SelectedIndex = (int)Language;

            if (!string.IsNullOrEmpty(Result))
            {
                txtResult.Text = Result;
            }

            btnStartOCR.Visible = Data != null && Data.Length > 0 && !string.IsNullOrEmpty(Filename);
        }

        private void StartOCR()
        {
            if (Data != null && Data.Length > 0 && !string.IsNullOrEmpty(Filename))
            {
                cbLanguages.Enabled = btnStartOCR.Enabled = false;

                try
                {
                    OCRSpace ocr = new OCRSpace(Language, false);
                    OCRSpaceResponse response = ocr.DoOCR(Data, Filename);

                    if (response != null && !response.IsErroredOnProcessing && response.ParsedResults.Count > 0)
                    {
                        Result = response.ParsedResults[0].ParsedText;
                        UpdateValues();
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }

                cbLanguages.Enabled = btnStartOCR.Enabled = true;
            }
        }

        private void cbLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            Language = (OCRSpaceLanguages)cbLanguages.SelectedIndex;
        }

        private void btnStartOCR_Click(object sender, EventArgs e)
        {
            StartOCR();
        }

        private void llAttribution_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            URLHelpers.OpenURL("https://ocr.space");
        }
    }
}
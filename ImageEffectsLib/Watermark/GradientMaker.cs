#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using HelpersLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace ImageEffectsLib
{
    public partial class GradientMaker : Form
    {
        public GradientData GradientData { get; private set; }

        private bool isEditable;
        private bool isEditing;
        private string lastData;

        public GradientMaker()
        {
            InitializeComponent();
            cboGradientDirection.Items.AddRange(Enum.GetNames(typeof(LinearGradientMode)));
            cboGradientDirection.SelectedIndex = 0;
        }

        public GradientMaker(GradientData gradientData)
            : this()
        {
            GradientData = gradientData;

            if (GradientData != null)
            {
                rtbCodes.Text = GradientData.Data;
                cboGradientDirection.SelectedIndex = (int)GradientData.Type;

                UpdatePreview();
            }
        }

        private void UpdatePreview()
        {
            GradientData = GetNewBrushData();

            if (GradientData != null && !string.IsNullOrEmpty(GradientData.Data))
            {
                try
                {
                    using (LinearGradientBrush brush = CreateGradientBrush(pbPreview.ClientSize, GradientData))
                    {
                        Bitmap bmp = new Bitmap(pbPreview.ClientSize.Width, pbPreview.ClientSize.Height);

                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.FillRectangle(brush, 0, 0, pbPreview.ClientSize.Width, pbPreview.ClientSize.Height);
                        }

                        pbPreview.Image = bmp;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
        }

        private static GradientStop[] ParseGradientData(string gradientData)
        {
            List<GradientStop> gradient = new List<GradientStop>();
            string[] lines = gradientData.Split('\n').Select(x => x.Trim()).ToArray();
            foreach (string line in lines)
            {
                if (line.Contains('\t'))
                {
                    gradient.Add(ParseLine(line));
                }
            }

            return gradient.ToArray();
        }

        private static GradientStop ParseLine(string line)
        {
            return new GradientStop(line.Substring(0, line.IndexOf('\t')), line.Remove(0, line.IndexOf('\t') + 1));
        }

        public static LinearGradientBrush CreateGradientBrush(Size size, GradientData gradientData)
        {
            IEnumerable<GradientStop> gradient = ParseGradientData(gradientData.Data);
            gradient = gradient.OrderBy(x => x.Offset);

            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, size.Width, size.Height), Color.Black, Color.Black, gradientData.Type);
            ColorBlend blend = new ColorBlend();
            blend.Colors = gradient.Select(x => x.Color).ToArray();
            blend.Positions = gradient.Select(x => x.Offset).ToArray();
            brush.InterpolationColors = blend;
            return brush;
        }

        private GradientData GetNewBrushData()
        {
            return new GradientData(rtbCodes.Text, (LinearGradientMode)cboGradientDirection.SelectedIndex);
        }

        #region Form events

        private void rtbCodes_SelectionChanged(object sender, EventArgs e)
        {
            if (!isEditing)
            {
                isEditable = false;
                int firstcharindex = rtbCodes.GetFirstCharIndexOfCurrentLine();
                int currentline = rtbCodes.GetLineFromCharIndex(firstcharindex);
                if (rtbCodes.Lines.Length > currentline)
                {
                    string line = rtbCodes.Lines[currentline];
                    if (line.Contains('\t'))
                    {
                        txtColor.Text = line.Substring(0, line.IndexOf('\t'));
                        txtOffset.Text = line.Remove(0, line.IndexOf('\t') + 1);
                        isEditable = true;
                        if (rtbCodes.Text != lastData)
                        {
                            UpdatePreview();
                            lastData = rtbCodes.Text;
                        }
                    }
                }
            }
        }

        private void rtbCodes_TextChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void btnBrowseColor_Click(object sender, EventArgs e)
        {
            using (DialogColor colorPicker = new DialogColor())
            {
                if (!string.IsNullOrEmpty(txtColor.Text))
                {
                    colorPicker.SetCurrentColor(ColorHelpers.ParseColor(txtColor.Text));
                }

                if (colorPicker.ShowDialog() == DialogResult.OK)
                {
                    Color color = colorPicker.NewColor;
                    txtColor.Text = string.Format("{0},{1},{2},{3}", color.A, color.R, color.G, color.B);
                }
            }
        }

        private void btnAddColor_Click(object sender, EventArgs e)
        {
            isEditing = true;
            if (isEditable)
            {
                int firstcharindex = rtbCodes.GetFirstCharIndexOfCurrentLine();
                int currentline = rtbCodes.GetLineFromCharIndex(firstcharindex);
                if (rtbCodes.Lines.Length > currentline)
                {
                    rtbCodes.SelectionStart = firstcharindex;
                    rtbCodes.SelectionLength = rtbCodes.Lines[currentline].Length;
                }
            }

            rtbCodes.SelectedText = string.Format("{0}\t{1}", txtColor.Text, txtOffset.Text);
            isEditing = false;
        }

        private void cboGradientDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            GradientData = GetNewBrushData();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion Form events
    }
}
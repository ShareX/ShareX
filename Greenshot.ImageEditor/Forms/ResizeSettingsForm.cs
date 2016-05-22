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

using Greenshot.Core;
using GreenshotPlugin.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Greenshot
{
    /// <summary>
    /// A form to set the resize settings
    /// </summary>
    public partial class ResizeSettingsForm : BaseForm
    {
        private readonly ResizeEffect effect;
        private readonly string value_pixel;
        private readonly string value_percent;
        private double newWidth, newHeight;

        public ResizeSettingsForm(ResizeEffect effect)
        {
            this.effect = effect;
            InitializeComponent();
            this.Icon = GreenshotResources.getGreenshotIcon();
            value_pixel = "Pixels";
            value_percent = "Percent";
            combobox_width.Items.Add(value_pixel);
            combobox_width.Items.Add(value_percent);
            combobox_width.SelectedItem = value_pixel;
            combobox_height.Items.Add(value_pixel);
            combobox_height.Items.Add(value_percent);
            combobox_height.SelectedItem = value_pixel;

            textbox_width.Text = effect.Width.ToString();
            textbox_height.Text = effect.Height.ToString();
            newWidth = effect.Width;
            newHeight = effect.Height;
            combobox_width.SelectedIndexChanged += new EventHandler(combobox_SelectedIndexChanged);
            combobox_height.SelectedIndexChanged += new EventHandler(combobox_SelectedIndexChanged);

            checkbox_aspectratio.Checked = effect.MaintainAspectRatio;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (newWidth != effect.Width || newHeight != effect.Height)
            {
                effect.Width = (int)newWidth;
                effect.Height = (int)newHeight;
                effect.MaintainAspectRatio = checkbox_aspectratio.Checked;
                DialogResult = DialogResult.OK;
            }
        }

        private bool validate(object sender)
        {
            TextBox textbox = sender as TextBox;
            if (textbox != null)
            {
                double numberEntered;
                if (!double.TryParse(textbox.Text, out numberEntered))
                {
                    textbox.BackColor = Color.Red;
                    return false;
                }
                else
                {
                    textbox.BackColor = Color.White;
                }
            }
            return true;
        }

        private void displayWidth()
        {
            double displayValue;
            if (value_percent.Equals(combobox_width.SelectedItem))
            {
                displayValue = (double)newWidth / (double)effect.Width * 100d;
            }
            else
            {
                displayValue = newWidth;
            }
            textbox_width.Text = ((int)displayValue).ToString();
        }

        private void displayHeight()
        {
            double displayValue;
            if (value_percent.Equals(combobox_height.SelectedItem))
            {
                displayValue = (double)newHeight / (double)effect.Height * 100d;
            }
            else
            {
                displayValue = newHeight;
            }
            textbox_height.Text = ((int)displayValue).ToString();
        }

        private void textbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!validate(sender))
            {
                return;
            }
            TextBox textbox = sender as TextBox;
            if (textbox.Text.Length == 0)
            {
                return;
            }
            bool isWidth = textbox == textbox_width;
            if (!checkbox_aspectratio.Checked)
            {
                if (isWidth)
                {
                    newWidth = double.Parse(textbox_width.Text);
                }
                else
                {
                    newHeight = double.Parse(textbox_height.Text);
                }
                return;
            }
            bool isPercent = false;
            if (isWidth)
            {
                isPercent = value_percent.Equals(combobox_width.SelectedItem);
            }
            else
            {
                isPercent = value_percent.Equals(combobox_height.SelectedItem);
            }
            double percent;
            if (isWidth)
            {
                if (isPercent)
                {
                    percent = double.Parse(textbox_width.Text);
                    newWidth = (double)effect.Width / 100d * percent;
                }
                else
                {
                    newWidth = double.Parse(textbox_width.Text);
                    percent = (double)double.Parse(textbox_width.Text) / (double)effect.Width * 100d;
                }
                if (checkbox_aspectratio.Checked)
                {
                    newHeight = (double)effect.Height / 100d * percent;
                    displayHeight();
                }
            }
            else
            {
                if (isPercent)
                {
                    percent = double.Parse(textbox_height.Text);
                    newHeight = (double)effect.Height / 100d * percent;
                }
                else
                {
                    newHeight = double.Parse(textbox_height.Text);
                    percent = (double)double.Parse(textbox_height.Text) / (double)effect.Height * 100d;
                }
                if (checkbox_aspectratio.Checked)
                {
                    newWidth = (double)effect.Width / 100d * percent;
                    displayWidth();
                }
            }
        }

        private void textbox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            validate(sender);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (validate(textbox_width))
            {
                displayWidth();
            }
            if (validate(textbox_height))
            {
                displayHeight();
            }
        }
    }
}
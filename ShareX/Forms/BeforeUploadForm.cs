#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UploadersLib;

namespace ShareX
{
    public partial class BeforeUploadForm : Form
    {
        private TaskInfo Info;

        public BeforeUploadForm(TaskInfo info)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            Info = info;

            switch (Info.DataType)
            {
                case EDataType.Image:

                    Enum.GetValues(typeof(ImageDestination)).Cast<ImageDestination>().ForEach(x =>
                    {
                        if (x != ImageDestination.FileUploader)
                        {
                            AddDestination<ImageDestination>((int)x);
                        }
                    });

                    Enum.GetValues(typeof(FileDestination)).Cast<FileDestination>().ForEach(x =>
                    {
                        AddDestination<FileDestination>((int)x);
                    });

                    ucBeforeUpload.flp.Controls.OfType<RadioButton>().ForEach(x =>
                    {
                        x.Checked = (x.Tag is ImageDestination && (ImageDestination)x.Tag == (ImageDestination)Info.TaskSettings.ImageDestination) ||
                          (x.Tag is FileDestination && (FileDestination)x.Tag == (FileDestination)Info.TaskSettings.ImageFileDestination);
                    });
                    break;
                case EDataType.Text:
                    Enum.GetValues(typeof(TextDestination)).Cast<TextDestination>().ForEach(x =>
                    {
                        if (x != TextDestination.FileUploader)
                        {
                            AddDestination<TextDestination>((int)x);
                        }
                    });

                    Enum.GetValues(typeof(FileDestination)).Cast<FileDestination>().ForEach(x =>
                    {
                        AddDestination<FileDestination>((int)x);
                    });

                    ucBeforeUpload.flp.Controls.OfType<RadioButton>().ForEach(x =>
                    {
                        x.Checked = (x.Tag is TextDestination && (TextDestination)x.Tag == (TextDestination)Info.TaskSettings.ImageDestination) ||
                            (x.Tag is FileDestination && (FileDestination)x.Tag == (FileDestination)Info.TaskSettings.TextFileDestination);
                    });
                    break;
                case EDataType.File:
                    Enum.GetValues(typeof(FileDestination)).Cast<FileDestination>().ForEach(x =>
                    {
                        AddDestination<FileDestination>((int)x);
                    });

                    ucBeforeUpload.flp.Controls.OfType<RadioButton>().ForEach(x =>
                    {
                        x.Checked = x.Tag is FileDestination && (FileDestination)x.Tag == (FileDestination)Info.TaskSettings.FileDestination;
                    });
                    break;
                case EDataType.URL:
                    Enum.GetValues(typeof(UrlShortenerType)).Cast<UrlShortenerType>().ForEach(x =>
                    {
                        AddDestination<UrlShortenerType>((int)x);
                    });

                    ucBeforeUpload.flp.Controls.OfType<RadioButton>().ForEach(x =>
                    {
                        x.Checked = x.Tag is UrlShortenerType && (UrlShortenerType)x.Tag == (UrlShortenerType)Info.TaskSettings.URLShortenerDestination;
                    });

                    break;
            }

            lblTitle.Text = string.Format("{0} is about to be uploaded to {1}. You may choose a different destination.", info.FileName, ucBeforeUpload.flp.Controls.OfType<RadioButton>().First<RadioButton>(x => x.Checked).Text);
        }

        private void UpdateUI<T>(int index)
        {
            Enum destination = (Enum)Enum.ToObject(typeof(T), index);
        }

        private void AddDestination<T>(int index)
        {
            Enum destination = (Enum)Enum.ToObject(typeof(T), index);

            if (Program.UploadersConfig.IsActive<T>(index))
            {
                RadioButton rb = new RadioButton() { AutoSize = true };
                rb.Text = destination.GetDescription();
                rb.Tag = destination;
                rb.CheckedChanged += (sender, e) => SetDestinations(rb.Checked, rb.Tag);
                ucBeforeUpload.flp.Controls.Add(rb);
            }
        }

        private void SetDestinations(bool isActive, object destination)
        {
            if (isActive)
            {
                switch (Info.DataType)
                {
                    case EDataType.Image:
                        if (destination is ImageDestination)
                        {
                            Info.TaskSettings.ImageDestination = (ImageDestination)destination;
                        }
                        else if (destination is FileDestination)
                        {
                            Info.TaskSettings.ImageDestination = ImageDestination.FileUploader;
                            Info.TaskSettings.ImageFileDestination = (FileDestination)destination;
                        }
                        break;
                    case EDataType.Text:
                        if (destination is TextDestination)
                        {
                            Info.TaskSettings.TextDestination = (TextDestination)destination;
                        }
                        else if (destination is FileDestination)
                        {
                            Info.TaskSettings.TextDestination = TextDestination.FileUploader;
                            Info.TaskSettings.TextFileDestination = (FileDestination)destination;
                        }
                        break;
                    case EDataType.File:
                        if (destination is FileDestination)
                        {
                            Info.TaskSettings.FileDestination = (FileDestination)destination;
                        }
                        break;
                    case EDataType.URL:
                        if (destination is UrlShortenerType)
                        {
                            Info.TaskSettings.URLShortenerDestination = (UrlShortenerType)destination;
                        }
                        break;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
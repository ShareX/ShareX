#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
using System.Linq;
using System.Windows.Forms;
using UploadersLib;

namespace ShareX
{
    public partial class BeforeUploadControl : UserControl
    {
        public delegate void EventHandler(string currentDestination);
        public event EventHandler InitCompleted;

        public BeforeUploadControl()
        {
            InitializeComponent();
        }

        public void Init(TaskInfo info)
        {
            switch (info.DataType)
            {
                case EDataType.Image:
                    InitCapture(info.TaskSettings);
                    break;
                case EDataType.Text:
                    Helpers.GetEnums<TextDestination>().ForEach(x =>
                    {
                        if (x != TextDestination.FileUploader)
                        {
                            AddDestination<TextDestination>((int)x, EDataType.Text, info.TaskSettings);
                        }
                    });

                    Helpers.GetEnums<FileDestination>().ForEach(x =>
                    {
                        AddDestination<FileDestination>((int)x, EDataType.Text, info.TaskSettings);
                    });

                    flp.Controls.OfType<RadioButton>().ForEach(x =>
                    {
                        x.Checked = (x.Tag is TextDestination && (TextDestination)x.Tag == info.TaskSettings.TextDestination) ||
                            (x.Tag is FileDestination && (FileDestination)x.Tag == info.TaskSettings.TextFileDestination);
                    });
                    break;
                case EDataType.File:
                    Helpers.GetEnums<FileDestination>().ForEach(x =>
                    {
                        AddDestination<FileDestination>((int)x, EDataType.File, info.TaskSettings);
                    });

                    flp.Controls.OfType<RadioButton>().ForEach(x =>
                    {
                        x.Checked = x.Tag is FileDestination && (FileDestination)x.Tag == info.TaskSettings.FileDestination;
                    });
                    break;
                case EDataType.URL:
                    Helpers.GetEnums<UrlShortenerType>().ForEach(x =>
                    {
                        AddDestination<UrlShortenerType>((int)x, EDataType.URL, info.TaskSettings);
                    });

                    flp.Controls.OfType<RadioButton>().ForEach(x =>
                    {
                        x.Checked = x.Tag is UrlShortenerType && (UrlShortenerType)x.Tag == info.TaskSettings.URLShortenerDestination;
                    });

                    break;
            }

            OnInitCompleted();
        }

        public void InitCapture(TaskSettings taskSettings)
        {
            Helpers.GetEnums<ImageDestination>().ForEach(x =>
            {
                if (x != ImageDestination.FileUploader)
                {
                    AddDestination<ImageDestination>((int)x, EDataType.Image, taskSettings);
                }
            });

            Helpers.GetEnums<FileDestination>().ForEach(x =>
            {
                AddDestination<FileDestination>((int)x, EDataType.File, taskSettings);
            });

            flp.Controls.OfType<RadioButton>().ForEach(x =>
            {
                x.Checked = (x.Tag is ImageDestination && (ImageDestination)x.Tag == taskSettings.ImageDestination) ||
                    (x.Tag is FileDestination && (FileDestination)x.Tag == taskSettings.ImageFileDestination);
            });
        }

        private void OnInitCompleted()
        {
            if (InitCompleted != null)
            {
                RadioButton rbDestination = flp.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Checked);
                string currentDestination = string.Empty;
                if (rbDestination != null)
                {
                    currentDestination = rbDestination.Text;
                }
                InitCompleted(currentDestination);
            }
        }

        private void AddDestination<T>(int index, EDataType dataType, TaskSettings taskSettings)
        {
            Enum destination = (Enum)Enum.ToObject(typeof(T), index);

            if (Program.UploadersConfig.IsValid<T>(index))
            {
                RadioButton rb = new RadioButton() { AutoSize = true };
                rb.Text = destination.GetDescription();
                rb.Tag = destination;
                rb.CheckedChanged += (sender, e) => SetDestinations(rb.Checked, dataType, rb.Tag, taskSettings);

                flp.Controls.Add(rb);
            }
        }

        private void SetDestinations(bool isActive, EDataType dataType, object destination, TaskSettings taskSettings)
        {
            if (isActive)
            {
                switch (dataType)
                {
                    case EDataType.Image:
                        if (destination is ImageDestination)
                        {
                            taskSettings.ImageDestination = (ImageDestination)destination;
                        }
                        else if (destination is FileDestination)
                        {
                            taskSettings.ImageDestination = ImageDestination.FileUploader;
                            taskSettings.ImageFileDestination = (FileDestination)destination;
                        }
                        break;
                    case EDataType.Text:
                        if (destination is TextDestination)
                        {
                            taskSettings.TextDestination = (TextDestination)destination;
                        }
                        else if (destination is FileDestination)
                        {
                            taskSettings.TextDestination = TextDestination.FileUploader;
                            taskSettings.TextFileDestination = (FileDestination)destination;
                        }
                        break;
                    case EDataType.File:
                        if (destination is FileDestination)
                        {
                            taskSettings.ImageFileDestination = taskSettings.FileDestination = (FileDestination)destination;
                        }
                        break;
                    case EDataType.URL:
                        if (destination is UrlShortenerType)
                        {
                            taskSettings.URLShortenerDestination = (UrlShortenerType)destination;
                        }
                        break;
                }
            }
        }
    }
}
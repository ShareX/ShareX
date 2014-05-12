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
                    Enum.GetValues(typeof(TextDestination)).Cast<TextDestination>().ForEach(x =>
                    {
                        if (x != TextDestination.FileUploader)
                        {
                            AddDestination<TextDestination>((int)x, EDataType.Text, info.TaskSettings);
                        }
                    });

                    Enum.GetValues(typeof(FileDestination)).Cast<FileDestination>().ForEach(x =>
                    {
                        AddDestination<FileDestination>((int)x, EDataType.Text, info.TaskSettings);
                    });

                    flp.Controls.OfType<RadioButton>().ForEach(x =>
                    {
                        x.Checked = (x.Tag is TextDestination && (TextDestination)x.Tag == (TextDestination)info.TaskSettings.ImageDestination) ||
                            (x.Tag is FileDestination && (FileDestination)x.Tag == (FileDestination)info.TaskSettings.TextFileDestination);
                    });
                    break;
                case EDataType.File:
                    Enum.GetValues(typeof(FileDestination)).Cast<FileDestination>().ForEach(x =>
                    {
                        AddDestination<FileDestination>((int)x, EDataType.File, info.TaskSettings);
                    });

                    flp.Controls.OfType<RadioButton>().ForEach(x =>
                    {
                        x.Checked = x.Tag is FileDestination && (FileDestination)x.Tag == (FileDestination)info.TaskSettings.FileDestination;
                    });
                    break;
                case EDataType.URL:
                    Enum.GetValues(typeof(UrlShortenerType)).Cast<UrlShortenerType>().ForEach(x =>
                    {
                        AddDestination<UrlShortenerType>((int)x, EDataType.URL, info.TaskSettings);
                    });

                    flp.Controls.OfType<RadioButton>().ForEach(x =>
                    {
                        x.Checked = x.Tag is UrlShortenerType && (UrlShortenerType)x.Tag == (UrlShortenerType)info.TaskSettings.URLShortenerDestination;
                    });

                    break;
            }

            OnInitCompleted();
        }

        public void InitCapture(TaskSettings taskSettings)
        {
            Enum.GetValues(typeof(ImageDestination)).Cast<ImageDestination>().ForEach(x =>
            {
                if (x != ImageDestination.FileUploader)
                {
                    AddDestination<ImageDestination>((int)x, EDataType.Image, taskSettings);
                }
            });

            Enum.GetValues(typeof(FileDestination)).Cast<FileDestination>().ForEach(x =>
            {
                AddDestination<FileDestination>((int)x, EDataType.File, taskSettings);
            });

            flp.Controls.OfType<RadioButton>().ForEach(x =>
            {
                x.Checked = (x.Tag is ImageDestination && (ImageDestination)x.Tag == (ImageDestination)taskSettings.ImageDestination) ||
                  (x.Tag is FileDestination && (FileDestination)x.Tag == (FileDestination)taskSettings.ImageFileDestination);
            });
        }

        private void OnInitCompleted()
        {
            if (InitCompleted != null)
            {
                string currentDestination = flp.Controls.OfType<RadioButton>().First<RadioButton>(x => x.Checked).Text;
                if (!string.IsNullOrEmpty(currentDestination))
                {
                    InitCompleted(currentDestination);
                }
            }
        }

        private void UpdateUI<T>(int index)
        {
            Enum destination = (Enum)Enum.ToObject(typeof(T), index);
        }

        private void AddDestination<T>(int index, EDataType dataType, TaskSettings taskSettings)
        {
            Enum destination = (Enum)Enum.ToObject(typeof(T), index);

            if (Program.UploadersConfig.IsActive<T>(index))
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
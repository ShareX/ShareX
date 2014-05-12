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
        private TaskInfo Info;

        public delegate void EventHandler(string currentDestination);
        public event EventHandler InitCompleted;

        public BeforeUploadControl()
        {
            InitializeComponent();
        }

        public void Init(TaskInfo info)
        {
            Info = info;

            switch (info.DataType)
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

                    flp.Controls.OfType<RadioButton>().ForEach(x =>
                    {
                        x.Checked = (x.Tag is ImageDestination && (ImageDestination)x.Tag == (ImageDestination)info.TaskSettings.ImageDestination) ||
                          (x.Tag is FileDestination && (FileDestination)x.Tag == (FileDestination)info.TaskSettings.ImageFileDestination);
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

                    flp.Controls.OfType<RadioButton>().ForEach(x =>
                    {
                        x.Checked = (x.Tag is TextDestination && (TextDestination)x.Tag == (TextDestination)info.TaskSettings.ImageDestination) ||
                            (x.Tag is FileDestination && (FileDestination)x.Tag == (FileDestination)info.TaskSettings.TextFileDestination);
                    });
                    break;
                case EDataType.File:
                    Enum.GetValues(typeof(FileDestination)).Cast<FileDestination>().ForEach(x =>
                    {
                        AddDestination<FileDestination>((int)x);
                    });

                    flp.Controls.OfType<RadioButton>().ForEach(x =>
                    {
                        x.Checked = x.Tag is FileDestination && (FileDestination)x.Tag == (FileDestination)info.TaskSettings.FileDestination;
                    });
                    break;
                case EDataType.URL:
                    Enum.GetValues(typeof(UrlShortenerType)).Cast<UrlShortenerType>().ForEach(x =>
                    {
                        AddDestination<UrlShortenerType>((int)x);
                    });

                    flp.Controls.OfType<RadioButton>().ForEach(x =>
                    {
                        x.Checked = x.Tag is UrlShortenerType && (UrlShortenerType)x.Tag == (UrlShortenerType)info.TaskSettings.URLShortenerDestination;
                    });

                    break;
            }

            OnInitCompleted();
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

        private void AddDestination<T>(int index)
        {
            Enum destination = (Enum)Enum.ToObject(typeof(T), index);

            if (Program.UploadersConfig.IsActive<T>(index))
            {
                RadioButton rb = new RadioButton() { AutoSize = true };
                rb.Text = destination.GetDescription();
                rb.Tag = destination;
                rb.CheckedChanged += (sender, e) => SetDestinations(rb.Checked, rb.Tag);
                flp.Controls.Add(rb);
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
    }
}
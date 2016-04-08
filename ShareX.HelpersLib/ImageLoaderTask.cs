using System;
using System.Drawing;

namespace ShareX.HelpersLib
{
    class ImageLoaderTask
    {
        private string filePath;
        private Action<Image> callback;

        public string FilePath
        {
            get
            {
                return filePath;
            }

            set
            {
                filePath = value;
            }
        }

        public ImageLoaderTask(string filePath, Action<Image> callback)
        {
            this.filePath = filePath;
            this.callback = callback;
        }

        public void Load()
        {
            callback.Invoke(ImageHelpers.LoadImage(filePath));
        }
    }
}

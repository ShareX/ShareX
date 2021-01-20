using ShareX.HelpersLib;
using System.Windows;

namespace ShareX
{
    /// <summary>
    /// Interaction logic for ClipboardFormatView.xaml
    /// </summary>
    public partial class ClipboardFormatView : Window
    {
        public ClipboardFormat ClipboardFormat { get; private set; }

        public ClipboardFormatView() : this(new ClipboardFormat())
        {
        }
        public ClipboardFormatView(ClipboardFormat cbf)
        {
            InitializeComponent();

            ClipboardFormat = cbf;
            txtDescription.Text = cbf.Description ?? "";
            txtFormat.Text = cbf.Format ?? "";
            // TODO CodeMenu.Create<CodeMenuEntryFilename>(txtFormat);
            lblExample.Text = string.Format("Supported variables: {0} and other variables such as {1} etc.",
                "$result, $url, $shorturl, $thumbnailurl, $deletionurl, $filepath, $filename, $filenamenoext, $thumbnailfilename, $thumbnailfilenamenoext, $folderpath, $foldername, $uploadtime",
                "%y, %mo, %d");
        }
    }
}

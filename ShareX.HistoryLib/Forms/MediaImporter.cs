using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Linq;
namespace ShareX.HistoryLib.Forms
{
    public partial class MediaImporter : Form
    {
        private bool _anyChanges { get; set; }

        private string _defaultFolder { get; set; }
        private string _historyPath { get; set; }
        private HistoryItemManager _historyItemManager { get; set; }
        private List<HistoryItem> _allHistoryItems { get; set; }

        public MediaImporter()
        {
            InitializeComponent();
        }

        public MediaImporter(string defaultFolder, HistoryItemManager historyItemManager, List<HistoryItem> historyItems)
        {
            InitializeComponent();
            _historyPath = defaultFolder;
            _defaultFolder = Path.GetDirectoryName(defaultFolder);
            _historyItemManager = historyItemManager;
            _allHistoryItems = historyItems;

            ShareXResources.ApplyTheme(this, true);
        }

        private void ImageImporter_Load(object sender, EventArgs e)
        {
            this.txtFolderPath.Text = _defaultFolder;
        }

        private void MediaImporter_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_anyChanges)
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFolder(txtFolderPath);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFolderPath.Text))
            {
                MessageBox.Show("Please select a folder to import media from.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Directory.Exists(txtFolderPath.Text))
            {
                MessageBox.Show("The selected folder does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var newHistoryItems = GetNewHistoryItems(txtFolderPath.Text);
            // Update the list of all history items
            _allHistoryItems.AddRange(newHistoryItems);
            _allHistoryItems.Sort((a, b) => a.DateTime.CompareTo(b.DateTime));

            // Update the history items file
            _historyItemManager.UpdateHistoryItemsFile(_historyPath, _allHistoryItems);

            _anyChanges = true;
        }

        /// <summary>
        /// Gets the new history items from the specified folder path.
        /// </summary>
        private List<HistoryItem> GetNewHistoryItems(string folderPath)
        {
            var mediaExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".mp4", ".avi", ".mov", ".apng", ".webp", ".webm", ".wmv", ".mkv" };
            var allFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                                    .Where(file => mediaExtensions.Contains(Path.GetExtension(file).ToLower()))
                                    .ToList();

            var existingFilePaths = _allHistoryItems.Select(item => item.FilePath).ToHashSet();
            var newFiles = allFiles.Where(file => !existingFilePaths.Contains(file)).ToList();

            var newHistoryItems = newFiles.Select(file => new HistoryItem
            {
                FileName = Path.GetFileName(file),
                FilePath = file,
                DateTime = File.GetLastWriteTime(file),
                Type = DetermineMediaType(Path.GetExtension(file).ToLower())
            }).ToList();

            return newHistoryItems;
        }

        /// <summary>
        /// Determines the type of the media based on the file extension.
        /// </summary>
        private string DetermineMediaType(string fileExtension)
        {
            var videoExtensions = new[] { ".mp4", ".avi", ".mov", ".webm", ".wmv", ".mkv" };
            return videoExtensions.Contains(fileExtension) ? "Video" : "Image";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

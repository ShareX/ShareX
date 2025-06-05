using ShareX.HelpersLib;
using ShareX.HistoryLib.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ShareX.HistoryLib.Forms
{
    public partial class MediaImporter : Form
    {
        private bool _anyChanges { get; set; }

        private string _defaultFolder { get; set; }
        private string _historyPath { get; set; }
        private Progress<int> _progress { get; set; }
        private CancellationTokenSource _cancellationTokenSource { get; set; }
        private HistoryItemManager _historyItemManager { get; set; }
        private List<HistoryItem> _allHistoryItems { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaImporter"/> class.
        /// </summary>
        public MediaImporter()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaImporter"/> class with specified parameters.
        /// </summary>
        /// <param name="defaultFolder">The default folder path.</param>
        /// <param name="historyItemManager">The history item manager.</param>
        /// <param name="historyItems">The list of history items.</param>
        public MediaImporter(string defaultFolder, HistoryItemManager historyItemManager, List<HistoryItem> historyItems)
        {
            InitializeComponent();
            _historyPath = defaultFolder;
            _defaultFolder = Path.GetDirectoryName(defaultFolder);
            _historyItemManager = historyItemManager;
            _allHistoryItems = historyItems;

            var stImporting = Resources.StatusImporting; 
            _progress = new Progress<int>(percent =>
            {
                // Update progress bar and UI elements with the progress percentage
                pbImportProgress.Value = percent;
                lbStatus.Text = $"{stImporting} {percent}%";
                if (percent == 100)
                    lbStatus.Text = Resources.StatusImportDone;
            });

            ShareXResources.ApplyTheme(this, true);
        }

        /// <summary>
        /// Handles the Load event of the ImageImporter control.
        /// </summary>
        private void ImageImporter_Load(object sender, EventArgs e)
        {
            this.txtFolderPath.Text = _defaultFolder;
        }

        /// <summary>
        /// Handles the FormClosing event of the MediaImporter control.
        /// </summary>
        private void MediaImporter_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_anyChanges)
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnBrowse control.
        /// </summary>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFolder(txtFolderPath);
        }

        /// <summary>
        /// Handles the Click event of the btnImport control.
        /// </summary>
        private async void btnImport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFolderPath.Text))
            {
                MessageBox.Show(Resources.SelectImportFolder, Resources.HistoryManager_GetHistoryItems_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Directory.Exists(txtFolderPath.Text))
            {
                MessageBox.Show(Resources.FolderDoesntExist, Resources.HistoryManager_GetHistoryItems_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            EnableDisableImportControls(false);
            var defaultCloseText = btnClose.Text;
            btnClose.Text = Resources.CancelString;
            pbImportProgress.Value = 0;
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var newHistoryItems = await GetNewHistoryItems(txtFolderPath.Text, _progress, _cancellationTokenSource.Token);
                // Update the list of all history items
                _allHistoryItems.AddRange(newHistoryItems);
                _allHistoryItems.Sort((a, b) => a.DateTime.CompareTo(b.DateTime));

                // Update the history items file
                _historyItemManager.UpdateHistoryItemsFile(_historyPath, _allHistoryItems);

                _anyChanges = true;
                EnableDisableImportControls(true);
                btnClose.Text = defaultCloseText;
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation
                MessageBox.Show(Resources.OperationCancelled, Resources.CancelledString, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                MessageBox.Show($"{Resources.ErrorOccurred}: {ex.Message}", Resources.HistoryManager_GetHistoryItems_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                DebugHelper.WriteException(ex);
            }
            finally
            {
                _cancellationTokenSource = null;
            }
        }

        /// <summary>
        /// Enables or disables the import controls.
        /// </summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        private void EnableDisableImportControls(bool isEnabled)
        {
            btnBrowseFolderPath.Enabled = isEnabled;
            btnImport.Enabled = isEnabled;
        }

        /// <summary>
        /// Gets the new history items from the specified folder path.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="progress">The progress reporter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of new history items.</returns>
        private async Task<List<HistoryItem>> GetNewHistoryItems(string folderPath, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var newHistoryItems = new List<HistoryItem>();

            try
            {
                await Task.Run(() =>
                {
                    var mediaExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".mp4", ".avi", ".mov", ".apng", ".webp", ".webm", ".wmv", ".mkv" };
                    var allFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                                            .Where(file => mediaExtensions.Contains(Path.GetExtension(file).ToLower()))
                                            .ToList();

                    // Check if the operation was canceled
                    cancellationToken.ThrowIfCancellationRequested();

                    var existingFilePaths = _allHistoryItems.Select(item => item.FilePath).ToHashSet();
                    var newFiles = allFiles.Where(file => !existingFilePaths.Contains(file)).ToList();

                    int totalFiles = newFiles.Count;
                    int processedFiles = 0;

                    Parallel.ForEach(newFiles, new ParallelOptions { CancellationToken = cancellationToken }, file =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var historyItem = new HistoryItem
                        {
                            FileName = Path.GetFileName(file),
                            FilePath = file,
                            DateTime = File.GetLastWriteTime(file),
                            Type = DetermineMediaType(Path.GetExtension(file).ToLower())
                        };

                        lock (newHistoryItems)
                        {
                            newHistoryItems.Add(historyItem);
                        }

                        processedFiles++;
                        progress.Report((processedFiles * 100) / totalFiles);
                    });
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation
                MessageBox.Show(Resources.OperationCancelled, Resources.CancelledString, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                MessageBox.Show($"{Resources.ErrorOccurred}: {ex.Message}", Resources.HistoryManager_GetHistoryItems_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                DebugHelper.WriteException(ex);
            }

            return newHistoryItems;
        }

        /// <summary>
        /// Determines the type of the media based on the file extension.
        /// </summary>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns>The media type.</returns>
        private string DetermineMediaType(string fileExtension)
        {
            // TODO: Add video support to ShareX starting with enum:EDataType.
            var videoExtensions = new HashSet<string> { ".mp4", ".avi", ".mov", ".webm", ".wmv", ".mkv" };
            return videoExtensions.Contains(fileExtension) ? "Video" : "Image";
        }

        /// <summary>
        /// Handles the Click event of the btnClose control.
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (_cancellationTokenSource != null)
            {
                if (MessageBox.Show(Resources.SureCancelImport, Resources.CancelString, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _cancellationTokenSource.Cancel();
                }
                // Don't close the form if the operation is still in progress
                return;
            }

            if (_anyChanges)
            {
                DialogResult = DialogResult.OK;
            }
            Close();
        }
    }
}

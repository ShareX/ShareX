#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.ImageEditor.Localization;
using System.Collections.ObjectModel;

namespace ShareX.ImageEditor.Presentation.ViewModels
{
    public partial class StartScreenDialogViewModel : ObservableObject
    {
        public string Title => Strings.StartScreenDialogView_Title;

        public ObservableCollection<string> RecentFiles { get; }

        public bool HasRecentFiles => RecentFiles.Count > 0;

        public bool HasStatusMessage => !string.IsNullOrWhiteSpace(StatusMessage);

        [ObservableProperty]
        private string _url = string.Empty;

        [ObservableProperty]
        private bool _isUrlInputVisible;

        [ObservableProperty]
        private bool _isUrlLoading;

        [ObservableProperty]
        private string? _statusMessage;

        public IRelayCommand NewImageCommand { get; }
        public IRelayCommand OpenFileCommand { get; }
        public IRelayCommand LoadFromClipboardCommand { get; }
        public IRelayCommand LoadFromUrlCommand { get; }
        public IRelayCommand SubmitUrlCommand { get; }
        public IRelayCommand CancelUrlCommand { get; }
        public IRelayCommand CloseCommand { get; }
        public IRelayCommand ExitCommand { get; }
        public IRelayCommand<string> OpenRecentFileCommand { get; }

        private readonly Action _onNewImage;
        private readonly Action _onOpenFile;
        private readonly Action _onLoadFromClipboard;
        private readonly Action _onShowUrlInput;
        private readonly Action<string> _onSubmitUrl;
        private readonly Action _onClose;
        private readonly Action _onExit;
        private readonly Action<string> _onOpenRecentFile;

        public StartScreenDialogViewModel(
            IReadOnlyList<string> recentFiles,
            Action onNewImage,
            Action onOpenFile,
            Action onLoadFromClipboard,
            Action onShowUrlInput,
            Action<string> onSubmitUrl,
            Action onClose,
            Action onExit,
            Action<string> onOpenRecentFile)
        {
            _onNewImage = onNewImage;
            _onOpenFile = onOpenFile;
            _onLoadFromClipboard = onLoadFromClipboard;
            _onShowUrlInput = onShowUrlInput;
            _onSubmitUrl = onSubmitUrl;
            _onClose = onClose;
            _onExit = onExit;
            _onOpenRecentFile = onOpenRecentFile;

            RecentFiles = new ObservableCollection<string>(recentFiles);
            RecentFiles.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasRecentFiles));

            NewImageCommand = new RelayCommand(() => _onNewImage());
            OpenFileCommand = new RelayCommand(() => _onOpenFile());
            LoadFromClipboardCommand = new RelayCommand(() =>
            {
                ClearStatus();
                _onLoadFromClipboard();
            });
            LoadFromUrlCommand = new RelayCommand(() => _onShowUrlInput());
            SubmitUrlCommand = new RelayCommand(SubmitUrl, CanSubmitUrl);
            CancelUrlCommand = new RelayCommand(HideUrlInput);
            CloseCommand = new RelayCommand(() => _onClose());
            ExitCommand = new RelayCommand(() => _onExit());
            OpenRecentFileCommand = new RelayCommand<string>(path =>
            {
                if (!string.IsNullOrEmpty(path))
                    _onOpenRecentFile(path);
            });
        }

        partial void OnUrlChanged(string value)
        {
            ClearStatus();
            SubmitUrlCommand.NotifyCanExecuteChanged();
        }

        partial void OnIsUrlInputVisibleChanged(bool value)
        {
            SubmitUrlCommand.NotifyCanExecuteChanged();
        }

        partial void OnIsUrlLoadingChanged(bool value)
        {
            SubmitUrlCommand.NotifyCanExecuteChanged();
        }

        partial void OnStatusMessageChanged(string? value)
        {
            OnPropertyChanged(nameof(HasStatusMessage));
        }

        public void ShowUrlInput(string? initialUrl = null)
        {
            ClearStatus();
            IsUrlInputVisible = true;
            IsUrlLoading = false;

            if (!string.IsNullOrWhiteSpace(initialUrl))
            {
                Url = initialUrl;
            }
        }

        public void HideUrlInput()
        {
            IsUrlLoading = false;
            IsUrlInputVisible = false;
            ClearStatus();
        }

        public void ShowStatus(string message)
        {
            StatusMessage = message;
        }

        public void ClearStatus()
        {
            StatusMessage = null;
        }

        public void SetUrlLoading(bool value)
        {
            IsUrlLoading = value;
        }

        private bool CanSubmitUrl()
        {
            return IsUrlInputVisible && !IsUrlLoading && !string.IsNullOrWhiteSpace(Url);
        }

        private void SubmitUrl()
        {
            ClearStatus();

            string trimmedUrl = Url.Trim();

            if (!Uri.TryCreate(trimmedUrl, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                ShowStatus(Strings.StartScreenDialogView_InvalidURL);
                return;
            }

            _onSubmitUrl(trimmedUrl);
        }
    }
}

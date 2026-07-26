using CommunityToolkit.Mvvm.Input;

namespace ShareX.ImageEditor.Presentation.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public event EventHandler? OpenOptionsPanelRequested;

        public bool EditorRememberWindowState
        {
            get => Options.RememberWindowState;
            set
            {
                if (Options.RememberWindowState == value)
                {
                    return;
                }

                Options.RememberWindowState = value;
                OnPropertyChanged(nameof(EditorRememberWindowState));
            }
        }

        public bool EditorShowExitConfirmation
        {
            get => Options.ShowExitConfirmation;
            set
            {
                if (Options.ShowExitConfirmation == value)
                {
                    return;
                }

                Options.ShowExitConfirmation = value;
                OnPropertyChanged(nameof(EditorShowExitConfirmation));
            }
        }

        public bool EditorZoomToFitOnOpen
        {
            get => Options.ZoomToFitOnOpen;
            set
            {
                if (Options.ZoomToFitOnOpen == value)
                {
                    return;
                }

                Options.ZoomToFitOnOpen = value;
                OnPropertyChanged(nameof(EditorZoomToFitOnOpen));
            }
        }

        public bool EditorQuickCrop
        {
            get => Options.QuickCrop;
            set
            {
                if (Options.QuickCrop == value)
                {
                    return;
                }

                Options.QuickCrop = value;
                OnPropertyChanged(nameof(EditorQuickCrop));
            }
        }

        public bool EditorAutoCloseEditorOnTask
        {
            get => Options.AutoCloseEditorOnTask;
            set
            {
                if (Options.AutoCloseEditorOnTask == value)
                {
                    return;
                }

                Options.AutoCloseEditorOnTask = value;
                OnPropertyChanged(nameof(EditorAutoCloseEditorOnTask));
            }
        }

        public bool EditorAutoCopyImageToClipboard
        {
            get => Options.AutoCopyImageToClipboard;
            set
            {
                if (Options.AutoCopyImageToClipboard == value)
                {
                    return;
                }

                Options.AutoCopyImageToClipboard = value;
                OnPropertyChanged(nameof(EditorAutoCopyImageToClipboard));
            }
        }

        public bool EditorShowInsertImageDialog
        {
            get => Options.ShowInsertImageDialog;
            set
            {
                if (Options.ShowInsertImageDialog == value)
                {
                    return;
                }

                Options.ShowInsertImageDialog = value;
                OnPropertyChanged(nameof(EditorShowInsertImageDialog));
            }
        }

        public bool EditorShowNotifications
        {
            get => Options.ShowNotifications;
            set
            {
                if (Options.ShowNotifications == value)
                {
                    return;
                }

                Options.ShowNotifications = value;
                OnPropertyChanged(nameof(EditorShowNotifications));

                if (!value)
                {
                    HideNotification();
                }
            }
        }

        [RelayCommand]
        private void OpenOptionsPanel()
        {
            OpenOptionsPanelRequested?.Invoke(this, EventArgs.Empty);
        }

    }
}

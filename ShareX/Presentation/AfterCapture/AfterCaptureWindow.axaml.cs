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

#nullable enable

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Localization;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using DrawingBitmap = SkiaSharp.SKBitmap;
using DrawingImageFormat = SkiaSharp.SKEncodedImageFormat;

namespace ShareX;

public readonly record struct AfterCaptureWindowResult(bool Accepted, string? FileName);

public partial class AfterCaptureWindow : Window
{
    private readonly DrawingBitmap? _sourceImage;
    private readonly DrawingBitmap? _ownedSourceImage;
    private readonly bool _fileAlreadySaved;
    private Bitmap? _previewBitmap;
    private AfterCaptureGoal _goal;

    public TaskSettings TaskSettings { get; }
    public IReadOnlyList<AfterCaptureGoalOption> GoalOptions { get; }
    public ObservableCollection<string> RequiredOptions { get; } = new();
    public IReadOnlyList<AfterCaptureTaskOption> AfterCaptureOptions { get; }
    public IReadOnlyList<AfterCaptureTaskOption> AfterUploadOptions { get; }
    public IReadOnlyList<AfterCaptureDestinationOption> DestinationOptions { get; }
    public AfterCaptureDestinationOption? SelectedDestination { get; set; }
    public AfterCaptureWindowResult Result { get; private set; }

    public AfterCaptureWindow() : this(global::ShareX.TaskSettings.GetDefaultTaskSettings(), null, null)
    {
    }

    public AfterCaptureWindow(TaskSettings taskSettings, TaskMetadata? metadata, string? filePath)
    {
        TaskSettings = taskSettings;
        _fileAlreadySaved = !string.IsNullOrEmpty(filePath);
        DestinationOptions = CreateDestinationOptions(taskSettings);
        SelectedDestination = DestinationOptions.FirstOrDefault(option => option.IsSelected);

        bool canUpload = !SystemOptions.DisableUpload && !ApplicationState.Settings.DisableUpload && DestinationOptions.Count > 0;
        IReadOnlyList<AfterCaptureGoal> goals = AfterCaptureGoals.GetAvailableGoals(metadata?.Image != null, canUpload);
        _goal = AfterCaptureGoals.InferGoal(taskSettings.AfterCaptureJob, goals);
        GoalOptions = goals.Select(goal => new AfterCaptureGoalOption(goal, goal == _goal, SelectGoal)).ToArray();
        AfterCaptureOptions = CreateAfterCaptureOptions(taskSettings.AfterCaptureJob);
        AfterUploadOptions = CreateAfterUploadOptions(taskSettings.AfterUploadJob);

        InitializeComponent();
        DataContext = this;
        UpdateGoalControls();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        string fileName;
        if (!string.IsNullOrEmpty(filePath))
        {
            fileName = Path.GetFileNameWithoutExtension(filePath);
            if (FileHelpers.IsImageFile(filePath))
            {
                _ownedSourceImage = SkiaImageHelpers.LoadImage(filePath);
                _sourceImage = _ownedSourceImage;
            }
        }
        else
        {
            _sourceImage = metadata?.Image;
            fileName = TaskHelpers.GetFileName(taskSettings, null, metadata);
        }

        FileNameTextBox.Text = fileName;
        CopyButton.IsEnabled = metadata?.Image != null;
        LoadPreview(filePath);

        Opened += OnOpened;
        Closed += OnClosed;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Activate();
        FileNameTextBox.Focus();
        FileNameTextBox.SelectAll();
    }

    private void LoadPreview(string? filePath)
    {
        if (_sourceImage != null)
        {
            using MemoryStream stream = new();
            _sourceImage.Save(stream, DrawingImageFormat.Png);
            stream.Position = 0;
            _previewBitmap = new Bitmap(stream);
            PreviewImage.Source = _previewBitmap;
            PreviewImage.IsVisible = true;
            EmptyPreview.IsVisible = false;
            ImageSizeText.Text = $"{_sourceImage.Width} × {_sourceImage.Height}";
            ImageSizeBadge.IsVisible = true;
            return;
        }

        PreviewImage.IsVisible = false;
        ImageSizeBadge.IsVisible = false;
        EmptyPreview.IsVisible = true;
        EmptyPreviewText.Text = string.IsNullOrEmpty(filePath)
            ? Strings.AfterCaptureWindow_NoPreview
            : Path.GetFileName(filePath);
    }

    private void SelectGoal(AfterCaptureGoal goal)
    {
        _goal = goal;
        UpdateGoalControls();
    }

    private void UpdateGoalControls()
    {
        RequiredOptions.Clear();

        foreach (Enum task in AfterCaptureGoals.GetRequiredTasks(_goal, _fileAlreadySaved))
        {
            RequiredOptions.Add(task.GetLocalizedDescription());
        }

        foreach (AfterCaptureTaskOption option in AfterCaptureOptions)
        {
            option.IsAvailable = AfterCaptureGoals.IsExtraAvailable((AfterCaptureTasks)option.Value, _goal, _fileAlreadySaved);
        }

        RequiredPanel.IsVisible = RequiredOptions.Count > 0;
        DestinationPanel.IsVisible = _goal == AfterCaptureGoal.CopyUrl;
        AfterUploadPanel.IsVisible = _goal == AfterCaptureGoal.CopyUrl;
    }

    private static IReadOnlyList<AfterCaptureTaskOption> CreateAfterCaptureOptions(AfterCaptureTasks selected)
    {
        AfterCaptureTasks[] ignored =
        [
            AfterCaptureTasks.None,
            AfterCaptureTasks.ShowQuickTaskMenu,
            AfterCaptureTasks.ShowAfterCaptureWindow
        ];

        // Tasks that belong to a goal are chosen through the goal, not as extras.

        return Helpers.GetEnums<AfterCaptureTasks>()
            .Where(task => !ignored.Contains(task) && !AfterCaptureGoals.GoalCaptureTasks.HasFlag(task))
            .Select(task => new AfterCaptureTaskOption(task, task.GetLocalizedDescription(), selected.HasFlag(task)))
            .ToArray();
    }

    private static IReadOnlyList<AfterCaptureTaskOption> CreateAfterUploadOptions(AfterUploadTasks selected)
    {
        return Helpers.GetEnums<AfterUploadTasks>()
            .Where(task => task != AfterUploadTasks.None && task != AfterUploadTasks.CopyURLToClipboard)
            .Select(task => new AfterCaptureTaskOption(task, task.GetLocalizedDescription(), selected.HasFlag(task)))
            .ToArray();
    }

    private static IReadOnlyList<AfterCaptureDestinationOption> CreateDestinationOptions(TaskSettings taskSettings)
    {
        List<AfterCaptureDestinationOption> options = new();

        foreach (ImageDestination destination in Helpers.GetEnums<ImageDestination>())
        {
            if (destination == ImageDestination.FileUploader ||
                !UploadersConfigValidator.Validate<ImageDestination>((int)destination, ApplicationState.UploadersConfig))
            {
                continue;
            }

            string label = destination.GetLocalizedDescription();
            if (destination == ImageDestination.CustomImageUploader)
            {
                label = GetCustomDestinationLabel(ApplicationState.UploadersConfig.CustomImageUploaderSelected, taskSettings, label);
            }

            bool selected = taskSettings.ImageDestination == destination;
            options.Add(new AfterCaptureDestinationOption(label, selected, () => taskSettings.ImageDestination = destination));
        }

        foreach (FileDestination destination in Helpers.GetEnums<FileDestination>())
        {
            if (!UploadersConfigValidator.Validate<FileDestination>((int)destination, ApplicationState.UploadersConfig))
            {
                continue;
            }

            string label = destination.GetLocalizedDescription();
            if (destination == FileDestination.CustomFileUploader)
            {
                label = GetCustomDestinationLabel(ApplicationState.UploadersConfig.CustomFileUploaderSelected, taskSettings, label);
            }

            bool selected = taskSettings.ImageDestination == ImageDestination.FileUploader &&
                taskSettings.ImageFileDestination == destination;
            options.Add(new AfterCaptureDestinationOption(label, selected, () =>
            {
                taskSettings.ImageDestination = ImageDestination.FileUploader;
                taskSettings.ImageFileDestination = destination;
            }));
        }

        return options;
    }

    private static string GetCustomDestinationLabel(int index, TaskSettings taskSettings, string fallback)
    {
        if (taskSettings.OverrideCustomUploader)
        {
            index = taskSettings.CustomUploaderIndex.BetweenOrDefault(0, ApplicationState.UploadersConfig.CustomUploadersList.Count - 1);
        }

        CustomUploaderItem? uploader = ApplicationState.UploadersConfig.CustomUploadersList.ReturnIfValidIndex(index);
        return uploader == null
            ? fallback
            : string.Format(Strings.AfterCaptureWindow_CustomUploader, uploader);
    }

    private void OnContinueClick(object? sender, RoutedEventArgs e)
    {
        AfterCaptureTasks captureExtras = AfterCaptureTasks.None;
        foreach (AfterCaptureTaskOption option in AfterCaptureOptions.Where(option => option.IsChecked))
        {
            captureExtras |= (AfterCaptureTasks)option.Value;
        }

        // Copying the URL stays as saved unless the goal asks for it; it only runs after an upload.
        AfterUploadTasks uploadExtras = TaskSettings.AfterUploadJob & AfterUploadTasks.CopyURLToClipboard;
        foreach (AfterCaptureTaskOption option in AfterUploadOptions.Where(option => option.IsChecked))
        {
            uploadExtras |= (AfterUploadTasks)option.Value;
        }

        AfterCaptureJobs jobs = AfterCaptureGoals.Resolve(_goal, captureExtras, uploadExtras, _fileAlreadySaved);
        TaskSettings.AfterCaptureJob = jobs.Capture;
        TaskSettings.AfterUploadJob = jobs.Upload;

        if (_goal == AfterCaptureGoal.CopyUrl)
        {
            SelectedDestination?.Select();
        }

        AcceptAndClose();
    }

    private void OnCopyClick(object? sender, RoutedEventArgs e)
    {
        TaskSettings.AfterCaptureJob = AfterCaptureTasks.CopyImageToClipboard;
        AcceptAndClose();
    }

    private void AcceptAndClose()
    {
        Result = new AfterCaptureWindowResult(true, FileNameTextBox.Text ?? string.Empty);
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();

    private void OnPreviewPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_sourceImage != null &&
            e.GetCurrentPoint(PreviewSurface).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
        {
            ImageViewerWindowIntegration.ShowImage(_sourceImage);
            e.Handled = true;
        }
    }

    private void OnCopyPreviewClick(object? sender, RoutedEventArgs e)
    {
        if (_sourceImage != null)
        {
            ClipboardHelpers.CopyImage(_sourceImage);
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _previewBitmap?.Dispose();
        _previewBitmap = null;
        _ownedSourceImage?.Dispose();
    }
}

public sealed class AfterCaptureGoalOption
{
    private readonly Action<AfterCaptureGoal> _select;
    private bool _isSelected;

    public AfterCaptureGoal Goal { get; }
    public string Label { get; }
    public string Description { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
            {
                return;
            }

            _isSelected = value;
            if (value)
            {
                _select(Goal);
            }
        }
    }

    public AfterCaptureGoalOption(AfterCaptureGoal goal, bool isSelected, Action<AfterCaptureGoal> select)
    {
        Goal = goal;
        _isSelected = isSelected;
        _select = select;
        (Label, Description) = goal switch
        {
            AfterCaptureGoal.CopyImage => (Strings.AfterCaptureWindow_GoalCopyImage, Strings.AfterCaptureWindow_GoalCopyImageDescription),
            AfterCaptureGoal.CopyFile => (Strings.AfterCaptureWindow_GoalCopyFile, Strings.AfterCaptureWindow_GoalCopyFileDescription),
            AfterCaptureGoal.CopyFilePath => (Strings.AfterCaptureWindow_GoalCopyFilePath, Strings.AfterCaptureWindow_GoalCopyFilePathDescription),
            AfterCaptureGoal.CopyFolderPath => (Strings.AfterCaptureWindow_GoalCopyFolderPath, Strings.AfterCaptureWindow_GoalCopyFolderPathDescription),
            AfterCaptureGoal.CopyUrl => (Strings.AfterCaptureWindow_GoalCopyUrl, Strings.AfterCaptureWindow_GoalCopyUrlDescription),
            _ => throw new ArgumentOutOfRangeException(nameof(goal), goal, null)
        };
    }
}

public sealed class AfterCaptureTaskOption : INotifyPropertyChanged
{
    private bool _isAvailable = true;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Enum Value { get; }
    public string Label { get; }
    public bool IsChecked { get; set; }

    public bool IsAvailable
    {
        get => _isAvailable;
        set
        {
            if (_isAvailable != value)
            {
                _isAvailable = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAvailable)));
            }
        }
    }

    public AfterCaptureTaskOption(Enum value, string label, bool isChecked)
    {
        Value = value;
        Label = label;
        IsChecked = isChecked;
    }
}

public sealed class AfterCaptureDestinationOption
{
    private readonly Action _select;

    public string Label { get; }
    public bool IsSelected { get; }

    public AfterCaptureDestinationOption(string label, bool isSelected, Action select)
    {
        Label = label;
        IsSelected = isSelected;
        _select = select;
    }

    public void Select() => _select();
}

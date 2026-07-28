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
using Avalonia.Platform.Storage;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShareX.ScreenCaptureLib;

public partial class FFmpegOptionsWindow : Window
{
    private const string RecorderDevicesUrl = "https://github.com/ShareX/RecorderDevices/releases/tag/v0.12.10";

    private bool _settingsLoaded;
    private bool _updatingCommandPreview;
    private bool _closed;

    public ScreenRecordingOptions Options { get; }

    public FFmpegOptionsWindow()
        : this(new ScreenRecordingOptions
        {
            IsRecording = true,
            FPS = 30,
            OutputPath = "output.mp4"
        })
    {
    }

    public FFmpegOptionsWindow(ScreenRecordingOptions options)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        PopulateLists();
        WireEvents();

#if MicrosoftStore
        DownloadRecorderDevicesButton.IsVisible = false;
#endif

        Opened += OnOpened;
        Closed += (_, _) => _closed = true;
    }

    private void PopulateLists()
    {
        VideoCodecComboBox.ItemsSource = Helpers.GetEnumDescriptions<FFmpegVideoCodec>();
        AudioCodecComboBox.ItemsSource = Helpers.GetEnumDescriptions<FFmpegAudioCodec>();
        X264PresetComboBox.ItemsSource = Helpers.GetEnumDescriptions<FFmpegPreset>();
        GifStatsModeComboBox.ItemsSource = Helpers.GetEnumDescriptions<FFmpegPaletteGenStatsMode>();
        NvencPresetComboBox.ItemsSource = Helpers.GetEnumDescriptions<FFmpegNVENCPreset>();
        NvencTuneComboBox.ItemsSource = Helpers.GetEnumDescriptions<FFmpegNVENCTune>();
        GifDitherComboBox.ItemsSource = Helpers.GetEnumDescriptions<FFmpegPaletteUseDither>();
        AmfUsageComboBox.ItemsSource = Helpers.GetEnumDescriptions<FFmpegAMFUsage>();
        AmfQualityComboBox.ItemsSource = Helpers.GetEnumDescriptions<FFmpegAMFQuality>();
        QsvPresetComboBox.ItemsSource = Helpers.GetEnumDescriptions<FFmpegQSVPreset>();

        AacBitrateComboBox.ItemsSource = Enumerable.Range(2, 9).Select(x => x * 32).ToArray();
        OpusBitrateComboBox.ItemsSource = Enumerable.Range(1, 16).Select(x => x * 32).ToArray();
        VorbisQualityComboBox.ItemsSource = Enumerable.Range(0, 11).ToArray();
        Mp3QualityComboBox.ItemsSource = Enumerable.Range(0, 10).Reverse().ToArray();
    }

    private void WireEvents()
    {
        OptionsTabStrip.SelectionChanged += (_, _) => UpdateTabVisibility();

        UseCustomPathCheckBox.IsCheckedChanged += (_, _) =>
        {
            if (!_settingsLoaded) return;
            Options.FFmpeg.OverrideCLIPath = UseCustomPathCheckBox.IsChecked == true;
            UpdateUI();
        };
        FFmpegPathTextBox.TextChanged += (_, _) =>
        {
            if (!_settingsLoaded) return;
            Options.FFmpeg.CLIPath = FFmpegPathTextBox.Text ?? string.Empty;
        };
        BrowseFFmpegButton.Click += async (_, _) => await BrowseForFFmpegAsync();
        RefreshDevicesButton.Click += async (_, _) => await RefreshSourcesAsync();
        DownloadRecorderDevicesButton.Click += (_, _) => URLHelpers.OpenURL(RecorderDevicesUrl);

        VideoSourceComboBox.SelectionChanged += (_, _) =>
        {
            if (!_settingsLoaded) return;
            Options.FFmpeg.VideoSource = (VideoSourceComboBox.SelectedItem as FFmpegCaptureDevice)?.Value ?? string.Empty;
            UpdateUI();
        };
        AudioSourceComboBox.SelectionChanged += (_, _) =>
        {
            if (!_settingsLoaded) return;
            Options.FFmpeg.AudioSource = (AudioSourceComboBox.SelectedItem as FFmpegCaptureDevice)?.Value ?? string.Empty;
            UpdateUI();
        };
        VideoCodecComboBox.SelectionChanged += (_, _) =>
        {
            if (!_settingsLoaded || VideoCodecComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.VideoCodec = (FFmpegVideoCodec)VideoCodecComboBox.SelectedIndex;
            UpdateUI();
        };
        AudioCodecComboBox.SelectionChanged += (_, _) =>
        {
            if (!_settingsLoaded || AudioCodecComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.AudioCodec = (FFmpegAudioCodec)AudioCodecComboBox.SelectedIndex;
            UpdateUI();
        };

        X264PresetComboBox.SelectionChanged += (_, _) =>
        {
            if (!_settingsLoaded || X264PresetComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.x264_Preset = (FFmpegPreset)X264PresetComboBox.SelectedIndex;
            UpdateUI();
        };
        UseX264BitrateCheckBox.IsCheckedChanged += (_, _) =>
        {
            if (!_settingsLoaded) return;
            Options.FFmpeg.x264_Use_Bitrate = UseX264BitrateCheckBox.IsChecked == true;
            UpdateUI();
        };
        X264CrfNumericUpDown.ValueChanged += (_, _) => SetNumeric(value => Options.FFmpeg.x264_CRF = value, X264CrfNumericUpDown);
        X264BitrateNumericUpDown.ValueChanged += (_, _) => SetNumeric(value => Options.FFmpeg.x264_Bitrate = value, X264BitrateNumericUpDown);
        VpxBitrateNumericUpDown.ValueChanged += (_, _) => SetNumeric(value => Options.FFmpeg.VPx_Bitrate = value, VpxBitrateNumericUpDown);
        XvidQualityNumericUpDown.ValueChanged += (_, _) => SetNumeric(value => Options.FFmpeg.XviD_QScale = value, XvidQualityNumericUpDown);

        NvencPresetComboBox.SelectionChanged += (_, _) =>
        {
            if (!_settingsLoaded || NvencPresetComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.NVENC_Preset = (FFmpegNVENCPreset)NvencPresetComboBox.SelectedIndex;
            UpdateUI();
        };
        NvencTuneComboBox.SelectionChanged += (_, _) =>
        {
            if (!_settingsLoaded || NvencTuneComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.NVENC_Tune = (FFmpegNVENCTune)NvencTuneComboBox.SelectedIndex;
            UpdateUI();
        };
        NvencBitrateNumericUpDown.ValueChanged += (_, _) => SetNumeric(value => Options.FFmpeg.NVENC_Bitrate = value, NvencBitrateNumericUpDown);

        GifStatsModeComboBox.SelectionChanged += (_, _) =>
        {
            if (!_settingsLoaded || GifStatsModeComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.GIFStatsMode = (FFmpegPaletteGenStatsMode)GifStatsModeComboBox.SelectedIndex;
            UpdateUI();
        };
        GifDitherComboBox.SelectionChanged += (_, _) =>
        {
            if (!_settingsLoaded || GifDitherComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.GIFDither = (FFmpegPaletteUseDither)GifDitherComboBox.SelectedIndex;
            UpdateUI();
        };
        GifBayerScaleNumericUpDown.ValueChanged += (_, _) => SetNumeric(value => Options.FFmpeg.GIFBayerScale = value, GifBayerScaleNumericUpDown);

        AmfUsageComboBox.SelectionChanged += (_, _) =>
        {
            if (!_settingsLoaded || AmfUsageComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.AMF_Usage = (FFmpegAMFUsage)AmfUsageComboBox.SelectedIndex;
            UpdateUI();
        };
        AmfQualityComboBox.SelectionChanged += (_, _) =>
        {
            if (!_settingsLoaded || AmfQualityComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.AMF_Quality = (FFmpegAMFQuality)AmfQualityComboBox.SelectedIndex;
            UpdateUI();
        };
        AmfBitrateNumericUpDown.ValueChanged += (_, _) => SetNumeric(value => Options.FFmpeg.AMF_Bitrate = value, AmfBitrateNumericUpDown);

        QsvPresetComboBox.SelectionChanged += (_, _) =>
        {
            if (!_settingsLoaded || QsvPresetComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.QSV_Preset = (FFmpegQSVPreset)QsvPresetComboBox.SelectedIndex;
            UpdateUI();
        };
        QsvBitrateNumericUpDown.ValueChanged += (_, _) => SetNumeric(value => Options.FFmpeg.QSV_Bitrate = value, QsvBitrateNumericUpDown);

        AacBitrateComboBox.SelectionChanged += (_, _) => SetSelectedNumber(value => Options.FFmpeg.AAC_Bitrate = value, AacBitrateComboBox);
        OpusBitrateComboBox.SelectionChanged += (_, _) => SetSelectedNumber(value => Options.FFmpeg.Opus_Bitrate = value, OpusBitrateComboBox);
        VorbisQualityComboBox.SelectionChanged += (_, _) => SetSelectedNumber(value => Options.FFmpeg.Vorbis_QScale = value, VorbisQualityComboBox);
        Mp3QualityComboBox.SelectionChanged += (_, _) => SetSelectedNumber(value => Options.FFmpeg.MP3_QScale = value, Mp3QualityComboBox);

        UserArgumentsTextBox.TextChanged += (_, _) =>
        {
            if (!_settingsLoaded) return;
            Options.FFmpeg.UserArgs = UserArgumentsTextBox.Text ?? string.Empty;
            UpdateCommandPreview();
        };
        UseCustomCommandsCheckBox.IsCheckedChanged += (_, _) =>
        {
            if (!_settingsLoaded) return;

            Options.FFmpeg.UseCustomCommands = UseCustomCommandsCheckBox.IsChecked == true;
            if (Options.FFmpeg.UseCustomCommands && string.IsNullOrWhiteSpace(Options.FFmpeg.CustomCommands))
            {
                Options.FFmpeg.CustomCommands = Options.GetFFmpegArgs(true) ?? string.Empty;
            }

            UpdateUI();
        };
        CommandPreviewTextBox.TextChanged += (_, _) =>
        {
            if (!_settingsLoaded || _updatingCommandPreview || !Options.FFmpeg.UseCustomCommands) return;
            Options.FFmpeg.CustomCommands = CommandPreviewTextBox.Text ?? string.Empty;
        };

        ResetOptionsButton.Click += (_, _) => ShowResetConfirmation(true);
        CancelResetButton.Click += (_, _) => ShowResetConfirmation(false);
        ConfirmResetButton.Click += async (_, _) => await ResetOptionsAsync();
    }

    private void UpdateTabVisibility()
    {
        int selectedIndex = Math.Max(0, OptionsTabStrip.SelectedIndex);
        SourcesTabContent.IsVisible = selectedIndex == 0;
        VideoTabContent.IsVisible = selectedIndex == 1;
        AudioTabContent.IsVisible = selectedIndex == 2;
        AdvancedTabContent.IsVisible = selectedIndex == 3;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        await LoadSettingsAsync();
    }

    private async Task LoadSettingsAsync()
    {
        _settingsLoaded = false;
        ShowResetConfirmation(false);

        FFmpegOptions ffmpeg = Options.FFmpeg;
        UseCustomPathCheckBox.IsChecked = ffmpeg.OverrideCLIPath;
        FFmpegPathTextBox.Text = ffmpeg.CLIPath;

        VideoCodecComboBox.SelectedIndex = (int)ffmpeg.VideoCodec;
        AudioCodecComboBox.SelectedIndex = (int)ffmpeg.AudioCodec;

        X264CrfNumericUpDown.Value = ffmpeg.x264_CRF;
        X264BitrateNumericUpDown.Value = ffmpeg.x264_Bitrate;
        UseX264BitrateCheckBox.IsChecked = ffmpeg.x264_Use_Bitrate;
        X264PresetComboBox.SelectedIndex = (int)ffmpeg.x264_Preset;
        VpxBitrateNumericUpDown.Value = ffmpeg.VPx_Bitrate;
        XvidQualityNumericUpDown.Value = ffmpeg.XviD_QScale;

        NvencBitrateNumericUpDown.Value = ffmpeg.NVENC_Bitrate;
        NvencPresetComboBox.SelectedIndex = (int)ffmpeg.NVENC_Preset;
        NvencTuneComboBox.SelectedIndex = (int)ffmpeg.NVENC_Tune;

        GifStatsModeComboBox.SelectedIndex = (int)ffmpeg.GIFStatsMode;
        GifDitherComboBox.SelectedIndex = (int)ffmpeg.GIFDither;
        GifBayerScaleNumericUpDown.Value = ffmpeg.GIFBayerScale;

        AmfUsageComboBox.SelectedIndex = (int)ffmpeg.AMF_Usage;
        AmfQualityComboBox.SelectedIndex = (int)ffmpeg.AMF_Quality;
        AmfBitrateNumericUpDown.Value = ffmpeg.AMF_Bitrate;

        QsvPresetComboBox.SelectedIndex = (int)ffmpeg.QSV_Preset;
        QsvBitrateNumericUpDown.Value = ffmpeg.QSV_Bitrate;

        SelectNumberOrDefault(AacBitrateComboBox, ffmpeg.AAC_Bitrate, 128);
        SelectNumberOrDefault(OpusBitrateComboBox, ffmpeg.Opus_Bitrate, 128);
        SelectNumberOrDefault(VorbisQualityComboBox, ffmpeg.Vorbis_QScale, 3);
        SelectNumberOrDefault(Mp3QualityComboBox, ffmpeg.MP3_QScale, 4);

        UserArgumentsTextBox.Text = ffmpeg.UserArgs;
        UseCustomCommandsCheckBox.IsChecked = ffmpeg.UseCustomCommands;

        await RefreshSourcesAsync();

        _settingsLoaded = true;
        UpdateUI();
    }

    private async Task RefreshSourcesAsync(bool selectRecorderDevices = false)
    {
        RefreshDevicesButton.IsEnabled = false;
        DeviceStatusTextBlock.Text = "Looking for capture devices...";

        DirectShowDevices? devices = null;
        Exception? discoveryError = null;
        string ffmpegPath = Options.FFmpeg.FFmpegPath;

        if (File.Exists(ffmpegPath))
        {
            await Task.Run(() =>
            {
                try
                {
                    using FFmpegCLIManager ffmpeg = new(ffmpegPath);
                    devices = ffmpeg.GetDirectShowDevices();
                }
                catch (Exception ex)
                {
                    discoveryError = ex;
                }
            });
        }

        if (_closed)
        {
            return;
        }

        List<FFmpegCaptureDevice> videoSources =
        [
            FFmpegCaptureDevice.None,
            FFmpegCaptureDevice.GDIGrab
        ];

        if (Helpers.IsWindows10OrGreater())
        {
            videoSources.Add(FFmpegCaptureDevice.DDAGrab);
        }

        List<FFmpegCaptureDevice> audioSources = [FFmpegCaptureDevice.None];

        if (devices != null)
        {
            videoSources.AddRange(devices.VideoDevices.Select(x => new FFmpegCaptureDevice(x, $"dshow ({x})")));
            audioSources.AddRange(devices.AudioDevices.Select(x => new FFmpegCaptureDevice(x, $"dshow ({x})")));
        }

        FFmpegOptions options = Options.FFmpeg;
        if (selectRecorderDevices && videoSources.Any(x => EqualsSource(x, FFmpegCaptureDevice.ScreenCaptureRecorder.Value)))
        {
            options.VideoSource = FFmpegCaptureDevice.ScreenCaptureRecorder.Value;
        }
        else if (!videoSources.Any(x => EqualsSource(x, options.VideoSource)))
        {
            options.VideoSource = FFmpegCaptureDevice.GDIGrab.Value;
        }

        if (selectRecorderDevices && audioSources.Any(x => EqualsSource(x, FFmpegCaptureDevice.VirtualAudioCapturer.Value)))
        {
            options.AudioSource = FFmpegCaptureDevice.VirtualAudioCapturer.Value;
        }
        else if (!audioSources.Any(x => EqualsSource(x, options.AudioSource)))
        {
            options.AudioSource = FFmpegCaptureDevice.None.Value;
        }

        bool wasLoaded = _settingsLoaded;
        _settingsLoaded = false;
        VideoSourceComboBox.ItemsSource = videoSources;
        AudioSourceComboBox.ItemsSource = audioSources;
        VideoSourceComboBox.SelectedItem = videoSources.First(x => EqualsSource(x, options.VideoSource));
        AudioSourceComboBox.SelectedItem = audioSources.First(x => EqualsSource(x, options.AudioSource));
        _settingsLoaded = wasLoaded;

        if (!File.Exists(ffmpegPath))
        {
            DeviceStatusTextBlock.Text = $"FFmpeg was not found at {ffmpegPath}";
        }
        else if (discoveryError != null)
        {
            DebugHelper.WriteException(discoveryError);
            DeviceStatusTextBlock.Text = "FFmpeg could not enumerate DirectShow devices. Desktop capture is still available.";
        }
        else
        {
            int deviceCount = Math.Max(0, videoSources.Count - (Helpers.IsWindows10OrGreater() ? 3 : 2)) +
                              Math.Max(0, audioSources.Count - 1);
            DeviceStatusTextBlock.Text = deviceCount > 0
                ? $"{deviceCount} DirectShow device{(deviceCount == 1 ? string.Empty : "s")} found."
                : "No DirectShow devices found. Desktop capture is ready.";
        }

        RefreshDevicesButton.IsEnabled = true;
        UpdateUI();
    }

    private async Task BrowseForFFmpegAsync()
    {
        IStorageFolder? startFolder = null;
        string? currentFolder = Path.GetDirectoryName(Options.FFmpeg.FFmpegPath);

        if (string.IsNullOrWhiteSpace(currentFolder) || !Directory.Exists(currentFolder))
        {
            currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        }

        if (Directory.Exists(currentFolder))
        {
            startFolder = await StorageProvider.TryGetFolderFromPathAsync(new Uri(currentFolder));
        }

        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Browse for ffmpeg.exe",
            AllowMultiple = false,
            SuggestedStartLocation = startFolder,
            FileTypeFilter =
            [
                new FilePickerFileType("FFmpeg executable") { Patterns = ["ffmpeg.exe", "*.exe"] },
                FilePickerFileTypes.All
            ]
        });

        if (files.Count == 0)
        {
            return;
        }

        UseCustomPathCheckBox.IsChecked = true;
        FFmpegPathTextBox.Text = files[0].Path.LocalPath;
        Options.FFmpeg.OverrideCLIPath = true;
        Options.FFmpeg.CLIPath = files[0].Path.LocalPath;
        await RefreshSourcesAsync();
    }

    private async Task ResetOptionsAsync()
    {
        bool overrideCliPath = Options.FFmpeg.OverrideCLIPath;
        string cliPath = Options.FFmpeg.CLIPath;

        Options.FFmpeg = new FFmpegOptions
        {
            OverrideCLIPath = overrideCliPath,
            CLIPath = cliPath
        };

        await LoadSettingsAsync();
    }

    private void UpdateUI()
    {
        FFmpegOptions ffmpeg = Options.FFmpeg;

        FFmpegPathTextBox.IsEnabled = ffmpeg.OverrideCLIPath;
        BrowseFFmpegButton.IsEnabled = ffmpeg.OverrideCLIPath;

        bool x264 = ffmpeg.VideoCodec is FFmpegVideoCodec.libx264 or FFmpegVideoCodec.libx265;
        bool vpx = ffmpeg.VideoCodec is FFmpegVideoCodec.libvpx or FFmpegVideoCodec.libvpx_vp9;
        bool xvid = ffmpeg.VideoCodec == FFmpegVideoCodec.libxvid;
        bool nvenc = ffmpeg.VideoCodec is FFmpegVideoCodec.h264_nvenc or FFmpegVideoCodec.hevc_nvenc;
        bool gif = ffmpeg.VideoCodec == FFmpegVideoCodec.gif;
        bool amf = ffmpeg.VideoCodec is FFmpegVideoCodec.h264_amf or FFmpegVideoCodec.hevc_amf;
        bool qsv = ffmpeg.VideoCodec is FFmpegVideoCodec.h264_qsv or FFmpegVideoCodec.hevc_qsv;

        X264SettingsPanel.IsVisible = x264;
        VpxSettingsPanel.IsVisible = vpx;
        XvidSettingsPanel.IsVisible = xvid;
        NvencSettingsPanel.IsVisible = nvenc;
        GifSettingsPanel.IsVisible = gif;
        AmfSettingsPanel.IsVisible = amf;
        QsvSettingsPanel.IsVisible = qsv;
        NoVideoSettingsPanel.IsVisible = !x264 && !vpx && !xvid && !nvenc && !gif && !amf && !qsv;

        X264QualityPanel.IsVisible = !ffmpeg.x264_Use_Bitrate;
        X264BitratePanel.IsVisible = ffmpeg.x264_Use_Bitrate;
        X264PresetWarningBorder.IsVisible = ffmpeg.x264_Preset > FFmpegPreset.fast;
        GifBayerScalePanel.IsVisible = ffmpeg.GIFDither == FFmpegPaletteUseDither.bayer;

        AacSettingsPanel.IsVisible = ffmpeg.AudioCodec == FFmpegAudioCodec.libvoaacenc;
        OpusSettingsPanel.IsVisible = ffmpeg.AudioCodec == FFmpegAudioCodec.libopus;
        VorbisSettingsPanel.IsVisible = ffmpeg.AudioCodec == FFmpegAudioCodec.libvorbis;
        Mp3SettingsPanel.IsVisible = ffmpeg.AudioCodec == FFmpegAudioCodec.libmp3lame;

        AudioUnavailableBorder.IsVisible = ffmpeg.IsAnimatedImage;
        AudioCodecComboBox.IsEnabled = !ffmpeg.IsAnimatedImage;
        AudioSettingsPanel.IsEnabled = !ffmpeg.IsAnimatedImage;

        (VideoCodecSummaryTextBlock.Text, VideoCodecHintTextBlock.Text) = GetVideoCodecDescription(ffmpeg.VideoCodec);

        CommandPreviewTextBox.IsReadOnly = !ffmpeg.UseCustomCommands;
        UpdateCommandPreview();
    }

    private void UpdateCommandPreview()
    {
        _updatingCommandPreview = true;
        CommandPreviewTextBox.Text = Options.FFmpeg.UseCustomCommands
            ? Options.FFmpeg.CustomCommands
            : Options.GetFFmpegArgs() ?? string.Empty;
        _updatingCommandPreview = false;
    }

    private void SetNumeric(Action<int> setter, NumericUpDown control)
    {
        if (!_settingsLoaded)
        {
            return;
        }

        setter((int)(control.Value ?? 0));
        UpdateCommandPreview();
    }

    private void SetSelectedNumber(Action<int> setter, ComboBox control)
    {
        if (!_settingsLoaded || control.SelectedItem is not int value)
        {
            return;
        }

        setter(value);
        UpdateCommandPreview();
    }

    private static void SelectNumberOrDefault(ComboBox control, int value, int defaultValue)
    {
        IEnumerable<int> items = control.ItemsSource?.Cast<int>() ?? [];
        control.SelectedItem = items.Contains(value) ? value : defaultValue;
    }

    private void ShowResetConfirmation(bool show)
    {
        NormalFooterPanel.IsVisible = !show;
        ResetConfirmationPanel.IsVisible = show;
    }

    private static bool EqualsSource(FFmpegCaptureDevice device, string? value)
    {
        return device.Value.Equals(value ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private static (string Summary, string Hint) GetVideoCodecDescription(FFmpegVideoCodec codec)
    {
        return codec switch
        {
            FFmpegVideoCodec.libx264 => ("H.264 software encoding", "The best default for broad playback compatibility."),
            FFmpegVideoCodec.libx265 => ("H.265 software encoding", "Smaller files than H.264, with slower encoding and less playback support."),
            FFmpegVideoCodec.libvpx => ("VP8 software encoding", "Creates WebM files for web-focused workflows."),
            FFmpegVideoCodec.libvpx_vp9 => ("VP9 software encoding", "Efficient WebM output with higher CPU usage."),
            FFmpegVideoCodec.libxvid => ("MPEG-4 / Xvid", "A legacy encoder for AVI compatibility."),
            FFmpegVideoCodec.h264_nvenc => ("H.264 NVIDIA encoding", "Uses a supported NVIDIA GPU to reduce CPU usage."),
            FFmpegVideoCodec.hevc_nvenc => ("HEVC NVIDIA encoding", "Efficient GPU encoding with more limited playback compatibility."),
            FFmpegVideoCodec.h264_amf => ("H.264 AMD encoding", "Uses a supported AMD GPU to reduce CPU usage."),
            FFmpegVideoCodec.hevc_amf => ("HEVC AMD encoding", "Efficient GPU encoding with more limited playback compatibility."),
            FFmpegVideoCodec.h264_qsv => ("H.264 Intel Quick Sync", "Uses supported Intel graphics hardware to reduce CPU usage."),
            FFmpegVideoCodec.hevc_qsv => ("HEVC Intel Quick Sync", "Efficient Intel hardware encoding with more limited playback compatibility."),
            FFmpegVideoCodec.gif => ("Animated GIF", "Widely supported, but produces larger files and does not include audio."),
            FFmpegVideoCodec.libwebp => ("Animated WebP", "Compact animated images with no audio track."),
            FFmpegVideoCodec.apng => ("Animated PNG", "High-quality animation with large files and no audio track."),
            _ => ("Video encoding", string.Empty)
        };
    }
}

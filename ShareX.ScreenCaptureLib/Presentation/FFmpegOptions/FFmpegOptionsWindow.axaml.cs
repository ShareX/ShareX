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
using ShareX.Platform;
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
    private readonly FFmpegOptionsWindowViewModel _availability;
    private readonly bool _allowDeviceDownload = true;

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
        _availability = new FFmpegOptionsWindowViewModel(Options.IsRecording);

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        PopulateLists();
        WireEvents();
        RefreshAvailability();

#if MicrosoftStore
        _allowDeviceDownload = false;
        DownloadRecorderDevicesButton.IsVisible = false;
#endif

        Opened += OnOpened;
        Closed += (_, _) => { _closed = true; _availability.Close(); };
    }

    private void PopulateLists()
    {
        VideoCodecComboBox.ItemsSource = Helpers.GetEnumDescriptions<FFmpegVideoCodec>();
        AudioCodecComboBox.ItemsSource = Helpers.GetEnumDescriptions<FFmpegAudioCodec>();
        X264PresetComboBox.ItemsSource = new[]
        {
            Localization.Strings.FFmpegOptionsWindow_Preset_ultra_fast,
            Localization.Strings.FFmpegOptionsWindow_Preset_super_fast,
            Localization.Strings.FFmpegOptionsWindow_Preset_very_fast,
            Localization.Strings.FFmpegOptionsWindow_Preset_faster,
            Localization.Strings.FFmpegOptionsWindow_Preset_fast,
            Localization.Strings.FFmpegOptionsWindow_Preset_medium,
            Localization.Strings.FFmpegOptionsWindow_Preset_slow,
            Localization.Strings.FFmpegOptionsWindow_Preset_slower,
            Localization.Strings.FFmpegOptionsWindow_Preset_very_slow,
            Localization.Strings.FFmpegOptionsWindow_Preset_placebo
        };
        GifStatsModeComboBox.ItemsSource = Helpers.GetEnumDescriptions<FFmpegPaletteGenStatsMode>();
        NvencPresetComboBox.ItemsSource = new[]
        {
            Localization.Strings.FFmpegOptionsWindow_NVENC_fastest_lowest_quality,
            Localization.Strings.FFmpegOptionsWindow_NVENC_faster_lower_quality,
            Localization.Strings.FFmpegOptionsWindow_NVENC_fast_low_quality,
            Localization.Strings.FFmpegOptionsWindow_NVENC_medium_medium_quality,
            Localization.Strings.FFmpegOptionsWindow_NVENC_slow_good_quality,
            Localization.Strings.FFmpegOptionsWindow_NVENC_slower_better_quality,
            Localization.Strings.FFmpegOptionsWindow_NVENC_slowest_best_quality
        };
        NvencTuneComboBox.ItemsSource = new[]
        {
            Localization.Strings.FFmpegOptionsWindow_NVENC_high_quality,
            Localization.Strings.FFmpegOptionsWindow_NVENC_low_latency,
            Localization.Strings.FFmpegOptionsWindow_NVENC_ultra_low_latency,
            Localization.Strings.FFmpegOptionsWindow_NVENC_lossless
        };
        GifDitherComboBox.ItemsSource = Helpers.GetEnumDescriptions<FFmpegPaletteUseDither>();
        AmfUsageComboBox.ItemsSource = new[]
        {
            Localization.Strings.FFmpegOptionsWindow_AMF_generic_transcoding,
            Localization.Strings.FFmpegOptionsWindow_AMF_ultra_low_latency_transcoding,
            Localization.Strings.FFmpegOptionsWindow_AMF_low_latency_transcoding,
            Localization.Strings.FFmpegOptionsWindow_AMF_webcam,
            Localization.Strings.FFmpegOptionsWindow_AMF_high_quality_transcoding,
            Localization.Strings.FFmpegOptionsWindow_AMF_low_latency_high_quality_transcoding
        };
        AmfQualityComboBox.ItemsSource = new[]
        {
            Localization.Strings.FFmpegOptionsWindow_AMF_prefer_speed,
            Localization.Strings.FFmpegOptionsWindow_AMF_balanced,
            Localization.Strings.FFmpegOptionsWindow_AMF_prefer_quality
        };
        QsvPresetComboBox.ItemsSource = new[]
        {
            Localization.Strings.FFmpegOptionsWindow_Preset_very_fast,
            Localization.Strings.FFmpegOptionsWindow_Preset_faster,
            Localization.Strings.FFmpegOptionsWindow_Preset_fast,
            Localization.Strings.FFmpegOptionsWindow_Preset_medium,
            Localization.Strings.FFmpegOptionsWindow_Preset_slow,
            Localization.Strings.FFmpegOptionsWindow_Preset_slower,
            Localization.Strings.FFmpegOptionsWindow_Preset_very_slow
        };

        AacBitrateComboBox.ItemsSource = Enumerable.Range(2, 9).Select(x => x * 32).ToArray();
        OpusBitrateComboBox.ItemsSource = Enumerable.Range(1, 16).Select(x => x * 32).ToArray();
        VorbisQualityComboBox.ItemsSource = Enumerable.Range(0, 11).ToArray();
        Mp3QualityComboBox.ItemsSource = Enumerable.Range(0, 10).Reverse().ToArray();
    }

    private void WireEvents()
    {
        OptionsTabStrip.SelectionChanged += (_, _) => UpdateTabVisibility();

        UseCustomPathCheckBox.IsCheckedChanged += (_, _) => UpdateOption(() =>
        {
            Options.FFmpeg.OverrideCLIPath = UseCustomPathCheckBox.IsChecked == true;
            UpdateUI();
        });
        FFmpegPathTextBox.TextChanged += (_, _) => UpdateOption(() =>
        {
            Options.FFmpeg.CLIPath = FFmpegPathTextBox.Text ?? string.Empty;
        });
        BrowseFFmpegButton.Click += async (_, _) => await BrowseForFFmpegAsync();
        RefreshDevicesButton.Click += async (_, _) =>
        {
            if (_availability.GetDeviceActionSupport(RecordingDeviceAction.ListDirectShowDevices).IsSupported)
                await RefreshSourcesAsync();
            else RefreshAvailability();
        };
        DownloadRecorderDevicesButton.Click += (_, _) =>
        {
            if (!_allowDeviceDownload) return;
            if (!_availability.TryDeviceAction(RecordingDeviceAction.InstallRecorderDevices, () => URLHelpers.OpenURL(RecorderDevicesUrl)))
                RefreshAvailability();
        };

        VideoSourceComboBox.SelectionChanged += (_, _) => UpdateOption(() =>
        {
            Options.FFmpeg.VideoSource = (VideoSourceComboBox.SelectedItem as FFmpegCaptureDevice)?.Value ?? string.Empty;
            UpdateUI();
        });
        AudioSourceComboBox.SelectionChanged += (_, _) => UpdateOption(() =>
        {
            Options.FFmpeg.AudioSource = (AudioSourceComboBox.SelectedItem as FFmpegCaptureDevice)?.Value ?? string.Empty;
            UpdateUI();
        });
        VideoCodecComboBox.SelectionChanged += (_, _) => UpdateOption(() =>
        {
            if (VideoCodecComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.VideoCodec = (FFmpegVideoCodec)VideoCodecComboBox.SelectedIndex;
            UpdateUI();
        });
        AudioCodecComboBox.SelectionChanged += (_, _) => UpdateOption(() =>
        {
            if (AudioCodecComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.AudioCodec = (FFmpegAudioCodec)AudioCodecComboBox.SelectedIndex;
            UpdateUI();
        });

        X264PresetComboBox.SelectionChanged += (_, _) => UpdateOption(() =>
        {
            if (X264PresetComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.x264_Preset = (FFmpegPreset)X264PresetComboBox.SelectedIndex;
            UpdateUI();
        });
        UseX264BitrateCheckBox.IsCheckedChanged += (_, _) => UpdateOption(() =>
        {
            Options.FFmpeg.x264_Use_Bitrate = UseX264BitrateCheckBox.IsChecked == true;
            UpdateUI();
        });
        X264CrfNumericUpDown.ValueChanged += (_, _) => SetNumeric(value => Options.FFmpeg.x264_CRF = value, X264CrfNumericUpDown);
        X264BitrateNumericUpDown.ValueChanged += (_, _) => SetNumeric(value => Options.FFmpeg.x264_Bitrate = value, X264BitrateNumericUpDown);
        VpxBitrateNumericUpDown.ValueChanged += (_, _) => SetNumeric(value => Options.FFmpeg.VPx_Bitrate = value, VpxBitrateNumericUpDown);
        XvidQualityNumericUpDown.ValueChanged += (_, _) => SetNumeric(value => Options.FFmpeg.XviD_QScale = value, XvidQualityNumericUpDown);

        NvencPresetComboBox.SelectionChanged += (_, _) => UpdateOption(() =>
        {
            if (NvencPresetComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.NVENC_Preset = (FFmpegNVENCPreset)NvencPresetComboBox.SelectedIndex;
            UpdateUI();
        });
        NvencTuneComboBox.SelectionChanged += (_, _) => UpdateOption(() =>
        {
            if (NvencTuneComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.NVENC_Tune = (FFmpegNVENCTune)NvencTuneComboBox.SelectedIndex;
            UpdateUI();
        });
        NvencBitrateNumericUpDown.ValueChanged += (_, _) => SetNumeric(value => Options.FFmpeg.NVENC_Bitrate = value, NvencBitrateNumericUpDown);

        GifStatsModeComboBox.SelectionChanged += (_, _) => UpdateOption(() =>
        {
            if (GifStatsModeComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.GIFStatsMode = (FFmpegPaletteGenStatsMode)GifStatsModeComboBox.SelectedIndex;
            UpdateUI();
        });
        GifDitherComboBox.SelectionChanged += (_, _) => UpdateOption(() =>
        {
            if (GifDitherComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.GIFDither = (FFmpegPaletteUseDither)GifDitherComboBox.SelectedIndex;
            UpdateUI();
        });
        GifBayerScaleNumericUpDown.ValueChanged += (_, _) => SetNumeric(value => Options.FFmpeg.GIFBayerScale = value, GifBayerScaleNumericUpDown);

        AmfUsageComboBox.SelectionChanged += (_, _) => UpdateOption(() =>
        {
            if (AmfUsageComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.AMF_Usage = (FFmpegAMFUsage)AmfUsageComboBox.SelectedIndex;
            UpdateUI();
        });
        AmfQualityComboBox.SelectionChanged += (_, _) => UpdateOption(() =>
        {
            if (AmfQualityComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.AMF_Quality = (FFmpegAMFQuality)AmfQualityComboBox.SelectedIndex;
            UpdateUI();
        });
        AmfBitrateNumericUpDown.ValueChanged += (_, _) => SetNumeric(value => Options.FFmpeg.AMF_Bitrate = value, AmfBitrateNumericUpDown);

        QsvPresetComboBox.SelectionChanged += (_, _) => UpdateOption(() =>
        {
            if (QsvPresetComboBox.SelectedIndex < 0) return;
            Options.FFmpeg.QSV_Preset = (FFmpegQSVPreset)QsvPresetComboBox.SelectedIndex;
            UpdateUI();
        });
        QsvBitrateNumericUpDown.ValueChanged += (_, _) => SetNumeric(value => Options.FFmpeg.QSV_Bitrate = value, QsvBitrateNumericUpDown);

        AacBitrateComboBox.SelectionChanged += (_, _) => SetSelectedNumber(value => Options.FFmpeg.AAC_Bitrate = value, AacBitrateComboBox);
        OpusBitrateComboBox.SelectionChanged += (_, _) => SetSelectedNumber(value => Options.FFmpeg.Opus_Bitrate = value, OpusBitrateComboBox);
        VorbisQualityComboBox.SelectionChanged += (_, _) => SetSelectedNumber(value => Options.FFmpeg.Vorbis_QScale = value, VorbisQualityComboBox);
        Mp3QualityComboBox.SelectionChanged += (_, _) => SetSelectedNumber(value => Options.FFmpeg.MP3_QScale = value, Mp3QualityComboBox);

        UserArgumentsTextBox.TextChanged += (_, _) => UpdateOption(() =>
        {
            Options.FFmpeg.UserArgs = UserArgumentsTextBox.Text ?? string.Empty;
            UpdateCommandPreview();
        });
        UseCustomCommandsCheckBox.IsCheckedChanged += (_, _) => UpdateOption(() =>
        {

            Options.FFmpeg.UseCustomCommands = UseCustomCommandsCheckBox.IsChecked == true;
            if (Options.FFmpeg.UseCustomCommands && string.IsNullOrWhiteSpace(Options.FFmpeg.CustomCommands))
            {
                Options.FFmpeg.CustomCommands = Options.GetFFmpegArgs(true) ?? string.Empty;
            }

            UpdateUI();
        });
        CommandPreviewTextBox.TextChanged += (_, _) => UpdateOption(() =>
        {
            if (_updatingCommandPreview || !Options.FFmpeg.UseCustomCommands) return;
            Options.FFmpeg.CustomCommands = CommandPreviewTextBox.Text ?? string.Empty;
        });

        ResetOptionsButton.Click += (_, _) => RunSupported(() => ShowResetConfirmation(true));
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
        if (_closed || _availability.IsReadingDevices || !RefreshAvailability()) return;
        if (!_availability.GetDeviceActionSupport(RecordingDeviceAction.ListDirectShowDevices).IsSupported)
        {
            ApplyDevices(Options.FFmpeg.FFmpegPath, null, null, false);
            return;
        }
        RefreshDevicesButton.IsEnabled = false;
        DeviceStatusTextBlock.Text = Localization.Strings.FFmpegOptionsWindow_Looking_for_devices;

        bool applied = await _availability.TryReadDevicesAsync(ReadDevicesAsync, result =>
        {
            if (!_closed) ApplyDevices(result.Path, result.Devices, result.Error, selectRecorderDevices);
        });
        if (_closed) return;
        if (!applied && _availability.Support.IsSupported)
            ApplyDevices(Options.FFmpeg.FFmpegPath, null, null, false);
        RefreshAvailability();
    }

    private async Task<(string Path, DirectShowDevices? Devices, Exception? Error)> ReadDevicesAsync()
    {
        DirectShowDevices? devices = null;
        Exception? discoveryError = null;
        string ffmpegPath = Options.FFmpeg.FFmpegPath;

        if (File.Exists(ffmpegPath))
        {
            await Task.Run(() =>
            {
                if (_closed || !_availability.GetDeviceActionSupport(RecordingDeviceAction.ListDirectShowDevices).IsSupported) return;
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

        return (ffmpegPath, devices, discoveryError);
    }

    private void ApplyDevices(string ffmpegPath, DirectShowDevices? devices, Exception? discoveryError, bool selectRecorderDevices)
    {
        // Screen devices come from the platform independently of DirectShow discovery.
        List<FFmpegCaptureDevice> videoSources = [FFmpegCaptureDevice.None];
        IReadOnlyList<string> platformDevices = PlatformServices.Current.ScreenRecording.GetSupportedDevices();
        videoSources.AddRange(platformDevices.Select(FFmpegCaptureDevice.FromPlatformDevice));
        string defaultVideoSource = platformDevices.Count > 0 ? platformDevices[0] : FFmpegCaptureDevice.None.Value;

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
            options.VideoSource = _availability.ResolveSelectedSource(videoSources, options.VideoSource, defaultVideoSource).Value;
        }

        if (selectRecorderDevices && audioSources.Any(x => EqualsSource(x, FFmpegCaptureDevice.VirtualAudioCapturer.Value)))
        {
            options.AudioSource = FFmpegCaptureDevice.VirtualAudioCapturer.Value;
        }
        else if (!audioSources.Any(x => EqualsSource(x, options.AudioSource)))
        {
            options.AudioSource = _availability.ResolveSelectedSource(audioSources, options.AudioSource, FFmpegCaptureDevice.None.Value).Value;
        }

        bool wasLoaded = _settingsLoaded;
        _settingsLoaded = false;
        VideoSourceComboBox.ItemsSource = videoSources;
        AudioSourceComboBox.ItemsSource = audioSources;
        VideoSourceComboBox.SelectedItem = videoSources.First(x => EqualsSource(x, options.VideoSource));
        AudioSourceComboBox.SelectedItem = audioSources.First(x => EqualsSource(x, options.AudioSource));
        _settingsLoaded = wasLoaded;

        FeatureSupport discoverySupport = _availability.GetDeviceActionSupport(RecordingDeviceAction.ListDirectShowDevices);
        if (!discoverySupport.IsSupported)
        {
            DeviceStatusTextBlock.Text = discoverySupport.Reason;
        }
        else if (!File.Exists(ffmpegPath))
        {
            DeviceStatusTextBlock.Text = string.Format(Localization.Strings.FFmpegOptionsWindow_FFmpeg_not_found, ffmpegPath);
        }
        else if (discoveryError != null)
        {
            DebugHelper.WriteException(discoveryError);
            DeviceStatusTextBlock.Text = Localization.Strings.FFmpegOptionsWindow_Device_enumeration_failed;
        }
        else
        {
            // Discovered devices only: not None and not the platform's screen devices.
            int deviceCount = Math.Max(0, videoSources.Count - 1 - platformDevices.Count) +
                              Math.Max(0, audioSources.Count - 1);
            DeviceStatusTextBlock.Text = deviceCount switch
            {
                0 => Localization.Strings.FFmpegOptionsWindow_No_devices_found,
                1 => string.Format(Localization.Strings.FFmpegOptionsWindow_One_device_found, deviceCount),
                _ => string.Format(Localization.Strings.FFmpegOptionsWindow_Multiple_devices_found, deviceCount)
            };
        }

        UpdateUI();
    }

    private async Task BrowseForFFmpegAsync()
    {
        if (_closed || !RefreshAvailability()) return;
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

        if (_closed || !RefreshAvailability()) return;
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Localization.Strings.FFmpegOptionsWindow_Browse_for_ffmpeg,
            AllowMultiple = false,
            SuggestedStartLocation = startFolder,
            FileTypeFilter =
            [
                new FilePickerFileType(Localization.Strings.FFmpegOptionsWindow_FFmpeg_executable) { Patterns = ["*.exe"] }
            ]
        });

        if (files.Count == 0 || _closed || !RefreshAvailability())
        {
            return;
        }

        RunSupported(() =>
        {
            UseCustomPathCheckBox.IsChecked = true;
            FFmpegPathTextBox.Text = files[0].Path.LocalPath;
            Options.FFmpeg.OverrideCLIPath = true;
            Options.FFmpeg.CLIPath = files[0].Path.LocalPath;
        });
        await RefreshSourcesAsync();
    }

    private async Task ResetOptionsAsync()
    {
        if (_closed || !RefreshAvailability()) return;
        bool overrideCliPath = Options.FFmpeg.OverrideCLIPath;
        string cliPath = Options.FFmpeg.CLIPath;

        RunSupported(() => Options.FFmpeg = new FFmpegOptions
        {
            OverrideCLIPath = overrideCliPath,
            CLIPath = cliPath
        });

        await LoadSettingsAsync();
    }

    private void UpdateUI()
    {
        if (_closed || !RefreshAvailability()) return;
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
        if (_closed || !RefreshAvailability()) return;
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

        RunSupported(() =>
        {
            setter((int)(control.Value ?? 0));
            UpdateCommandPreview();
        });
    }

    private void SetSelectedNumber(Action<int> setter, ComboBox control)
    {
        if (!_settingsLoaded || control.SelectedItem is not int value)
        {
            return;
        }

        RunSupported(() =>
        {
            setter(value);
            UpdateCommandPreview();
        });
    }

    private bool RefreshAvailability()
    {
        FeatureSupport support = _availability.Support;
        OptionsContent.IsEnabled = support.IsSupported;
        ToolTip.SetTip(AvailabilitySurface, support.IsSupported ? null : support.Reason);
        FeatureSupport discovery = _availability.GetDeviceActionSupport(RecordingDeviceAction.ListDirectShowDevices);
        FeatureSupport download = _availability.GetDeviceActionSupport(RecordingDeviceAction.InstallRecorderDevices);
        RefreshDevicesButton.IsEnabled = !_availability.IsReadingDevices && discovery.IsSupported;
        DownloadRecorderDevicesButton.IsEnabled = _allowDeviceDownload && download.IsSupported;
        ToolTip.SetTip(RefreshDevicesSurface, discovery.IsSupported ? null : discovery.Reason);
        ToolTip.SetTip(RecorderDevicesSurface, download.IsSupported ? null : download.Reason);
        if (!support.IsSupported) DeviceStatusTextBlock.Text = support.Reason;
        return support.IsSupported;
    }

    private void RunSupported(Action action)
    {
        if (!_closed && !_availability.TryChange(action)) RefreshAvailability();
    }

    private void UpdateOption(Action action)
    {
        if (_settingsLoaded) RunSupported(action);
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
            FFmpegVideoCodec.libx264 => (Localization.Strings.FFmpegOptionsWindow_Codec_H264_software, Localization.Strings.FFmpegOptionsWindow_Codec_H264_software_hint),
            FFmpegVideoCodec.libx265 => (Localization.Strings.FFmpegOptionsWindow_Codec_H265_software, Localization.Strings.FFmpegOptionsWindow_Codec_H265_software_hint),
            FFmpegVideoCodec.libvpx => (Localization.Strings.FFmpegOptionsWindow_Codec_VP8_software, Localization.Strings.FFmpegOptionsWindow_Codec_VP8_software_hint),
            FFmpegVideoCodec.libvpx_vp9 => (Localization.Strings.FFmpegOptionsWindow_Codec_VP9_software, Localization.Strings.FFmpegOptionsWindow_Codec_VP9_software_hint),
            FFmpegVideoCodec.libxvid => (Localization.Strings.FFmpegOptionsWindow_Codec_MPEG4_Xvid, Localization.Strings.FFmpegOptionsWindow_Codec_MPEG4_Xvid_hint),
            FFmpegVideoCodec.h264_nvenc => (Localization.Strings.FFmpegOptionsWindow_Codec_H264_NVIDIA, Localization.Strings.FFmpegOptionsWindow_Codec_H264_NVIDIA_hint),
            FFmpegVideoCodec.hevc_nvenc => (Localization.Strings.FFmpegOptionsWindow_Codec_HEVC_NVIDIA, Localization.Strings.FFmpegOptionsWindow_Codec_HEVC_NVIDIA_hint),
            FFmpegVideoCodec.h264_amf => (Localization.Strings.FFmpegOptionsWindow_Codec_H264_AMD, Localization.Strings.FFmpegOptionsWindow_Codec_H264_AMD_hint),
            FFmpegVideoCodec.hevc_amf => (Localization.Strings.FFmpegOptionsWindow_Codec_HEVC_AMD, Localization.Strings.FFmpegOptionsWindow_Codec_HEVC_AMD_hint),
            FFmpegVideoCodec.h264_qsv => (Localization.Strings.FFmpegOptionsWindow_Codec_H264_Intel_Quick_Sync, Localization.Strings.FFmpegOptionsWindow_Codec_H264_Intel_Quick_Sync_hint),
            FFmpegVideoCodec.hevc_qsv => (Localization.Strings.FFmpegOptionsWindow_Codec_HEVC_Intel_Quick_Sync, Localization.Strings.FFmpegOptionsWindow_Codec_HEVC_Intel_Quick_Sync_hint),
            FFmpegVideoCodec.gif => (Localization.Strings.FFmpegOptionsWindow_Codec_animated_GIF, Localization.Strings.FFmpegOptionsWindow_Codec_animated_GIF_hint),
            FFmpegVideoCodec.libwebp => (Localization.Strings.FFmpegOptionsWindow_Codec_animated_WebP, Localization.Strings.FFmpegOptionsWindow_Codec_animated_WebP_hint),
            FFmpegVideoCodec.apng => (Localization.Strings.FFmpegOptionsWindow_Codec_animated_PNG, Localization.Strings.FFmpegOptionsWindow_Codec_animated_PNG_hint),
            _ => (Localization.Strings.FFmpegOptionsWindow_Codec_video_encoding, string.Empty)
        };
    }
}

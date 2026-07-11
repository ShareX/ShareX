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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using ShareX.ImageEditor.Presentation.ViewModels;
using ShareX.ImageEditor.Presentation.Views;
using SkiaSharp;

namespace ShareX.ImageEditor.Hosting
{
    public class AvaloniaApp : Application
    {
        public override void Initialize()
        {
            Uri baseUri = new Uri("avares://ShareX.ImageEditor/");
            Resources.MergedDictionaries.Add(new ResourceInclude(baseUri)
            {
                Source = new Uri("avares://ShareX.ImageEditor/Presentation/Theming/AppTheme.axaml")
            });
            Styles.Add(new FluentTheme());
            Styles.Add(new StyleInclude(baseUri)
            {
                Source = new Uri("avares://ShareX.ImageEditor/Presentation/Theming/AppStyles.axaml")
            });
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // No main window here, we manage windows manually
                desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }

    public class EditorEvents
    {
        public Action<SKBitmap>? CopyImageRequested { get; set; }
        public Func<SKBitmap, string?, string?>? SaveImageRequested { get; set; }
        public Func<SKBitmap, string?, string?>? SaveImageAsRequested { get; set; }
        public Action<SKBitmap>? PrintImageRequested { get; set; }
        public Action<SKBitmap>? PinImageRequested { get; set; }
        public Action<SKBitmap>? UploadImageRequested { get; set; }
        public string? ImageFilePath { get; set; }
    }

    public static class AvaloniaIntegration
    {
        private static bool initialized = false;

        private static readonly object initLock = new object();

        public static void Initialize()
        {
            if (!initialized)
            {
                lock (initLock)
                {
                    if (!initialized)
                    {
                        EditorServices.EnsureDefaultDesktopWallpaperService();

                        if (Application.Current == null)
                        {
                            AppBuilder builder = AppBuilder.Configure<AvaloniaApp>()
                                .UsePlatformDetect()
                                .WithInterFont();

#if DEBUG
                            builder = builder.LogToTrace();
#endif

                            builder.SetupWithoutStarting();
                        }

                        initialized = true;
                    }
                }
            }
        }

        public static SKBitmap? ShowEditorDialog(ImageEditorOptions options, EditorEvents? events = null,
            bool taskMode = false, string? imageFilePath = null)
        {
            return ShowEditorDialog(null, options, events, taskMode, imageFilePath);
        }

        public static void ShowImageComparerWindow()
        {
            Initialize();

            Dispatcher.UIThread.Post(() =>
            {
                ImageComparerWindow window = new ImageComparerWindow();
                window.Show();
            });
        }

        public static void ShowIconConverterWindow()
        {
            Initialize();

            Dispatcher.UIThread.Post(() =>
            {
                IconConverterWindow window = new IconConverterWindow();
                window.Show();
            });
        }

        public static void ShowVideoConverterWindow(
            VideoConverterSettings settings,
            VideoConversionHandler conversionHandler,
            Action<VideoConverterSettings>? settingsChanged = null,
            string? inputFilePath = null)
        {
            Initialize();

            Dispatcher.UIThread.Post(() =>
            {
                VideoConverterWindow window = new VideoConverterWindow(settings, conversionHandler, settingsChanged, inputFilePath);
                window.Show();
            });
        }

        public static void ShowBackgroundRemoverWindow(string? modelsFolder)
        {
            ShowBackgroundRemoverWindow(modelsFolder, new BackgroundRemoverOptions());
        }

        public static void ShowBackgroundRemoverWindow(string? modelsFolder, BackgroundRemoverOptions options)
        {
            Initialize();

            Dispatcher.UIThread.Post(() =>
            {
                BackgroundRemoverWindow window = new BackgroundRemoverWindow(modelsFolder, options);
                window.Show();
            });
        }

        public static SKBitmap? ShowEditorDialog(SKBitmap? imageBitmap, ImageEditorOptions options, EditorEvents? events = null,
            bool taskMode = false, string? imageFilePath = null)
        {
            return ShowEditorDialogCore(imageBitmap, options, events, taskMode, imageFilePath);
        }

        private static SKBitmap? ShowEditorDialogCore(SKBitmap? imageBitmap, ImageEditorOptions options, EditorEvents? events,
            bool taskMode, string? imageFilePath)
        {
            Initialize();

            TaskCompletionSource<SKBitmap?> tcs = new TaskCompletionSource<SKBitmap?>();

            Dispatcher.UIThread.Post(() =>
            {
                EditorWindow window = new EditorWindow(options);

                if (imageBitmap != null)
                {
                    window.LoadImage(imageBitmap);
                }

                if (window.DataContext is MainViewModel vm)
                {
                    string? filePath = imageFilePath ?? events?.ImageFilePath;

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        vm.ImageFilePath = filePath;
                    }

                    vm.ShowFileMenu = true;
                    vm.ShowOptionsButton = true;
                    vm.ShowTaskButtons = true;
                    vm.UseContinueWorkflow = taskMode;
                    vm.ShowBottomToolbar = true;
                    vm.ShowStartScreen = !taskMode;
                }

                SetupEvents(window, events);

                window.Closed += (s, a) =>
                {
                    SKBitmap? result = null;

                    if (window.DataContext is MainViewModel vm)
                    {
                        switch (vm.TaskResult)
                        {
                            case MainViewModel.EditorTaskResult.Continue:
                                result = window.GetResultBitmap();
                                break;
                            case MainViewModel.EditorTaskResult.ContinueNoSave:
                                result = window.GetSourceBitmap();
                                break;
                        }
                    }

                    tcs.SetResult(result);
                };

                window.Show();
            });

            return tcs.Task.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static void SetupEvents(EditorWindow window, EditorEvents? events)
        {
            if (events == null) return;

            MainViewModel? vm = window.DataContext as MainViewModel;
            if (vm == null) return;

            if (events.CopyImageRequested != null)
            {
                vm.HasHostCopyHandler = true;
                vm.CopyRequested += () =>
                {
                    using SKBitmap? skBitmap = window.GetResultBitmap();

                    if (skBitmap != null)
                    {
                        events.CopyImageRequested(skBitmap);
                    }

                    return Task.CompletedTask;
                };
            }

            if (events.SaveImageRequested != null)
            {
                vm.HasHostSaveHandler = true;
                vm.SaveRequested += () =>
                {
                    string? savedPath = null;

                    using SKBitmap? skBitmap = window.GetResultBitmap();

                    if (skBitmap != null)
                    {
                        savedPath = events.SaveImageRequested(skBitmap, vm.ImageFilePath);

                        if (!string.IsNullOrEmpty(savedPath))
                        {
                            vm.ImageFilePath = savedPath;
                            vm.IsDirty = false;
                        }
                    }

                    return Task.FromResult(savedPath);
                };
            }

            if (events.SaveImageAsRequested != null)
            {
                vm.HasHostSaveAsHandler = true;
                vm.SaveAsRequested += () =>
                {
                    string? savedPath = null;

                    using SKBitmap? skBitmap = window.GetResultBitmap();

                    if (skBitmap != null)
                    {
                        savedPath = events.SaveImageAsRequested(skBitmap, vm.ImageFilePath);

                        if (!string.IsNullOrEmpty(savedPath))
                        {
                            vm.ImageFilePath = savedPath;
                            vm.IsDirty = false;
                        }
                    }

                    return Task.FromResult(savedPath);
                };
            }

            if (events.PrintImageRequested != null)
            {
                vm.PrintRequested += () =>
                {
                    using SKBitmap? skBitmap = window.GetResultBitmap();

                    if (skBitmap != null)
                    {
                        events.PrintImageRequested(skBitmap);
                    }
                };
            }

            if (events.PinImageRequested != null)
            {
                vm.PinRequested += () =>
                {
                    using SKBitmap? skBitmap = window.GetResultBitmap();

                    if (skBitmap != null)
                    {
                        events.PinImageRequested(skBitmap);
                    }
                };
            }

            if (events.UploadImageRequested != null)
            {
                vm.UploadRequested += () =>
                {
                    using SKBitmap? skBitmap = window.GetResultBitmap();

                    if (skBitmap != null)
                    {
                        events.UploadImageRequested(skBitmap);
                    }
                };
            }
        }
    }
}

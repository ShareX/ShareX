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
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ShareX.AvaloniaUI.Theming;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Presentation.ViewModels;
using ShareX.ImageEditor.Presentation.Views;
using SkiaSharp;
using System;
using System.IO;

namespace ShareX.ImageEditor.App
{
    public partial class App : Application
    {
        private static readonly string[] ImageExtensions = [".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tiff", ".tif", ".webp", ".ico"];

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            ThemeManager.Configure(new ApplicationThemeOptions());

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                ImageEditorOptions options = new ImageEditorOptions();

#if DEBUG
                options.ShowExitConfirmation = false;
#endif

                EditorWindow window = new EditorWindow(options);
                desktop.MainWindow = window;

                if (window.DataContext is MainViewModel vm)
                {
                    vm.ShowFileMenu = true;
                    vm.ShowTaskButtons = false;
                    vm.UseContinueWorkflow = false;
                    vm.ShowBottomToolbar = true;
                    vm.ShowStartScreen = true;

                    string? imagePath = GetImagePathFromArgs(desktop.Args);

#if DEBUG
                    vm.ShowStartScreen = false;

                    if (!vm.ShowStartScreen && string.IsNullOrEmpty(imagePath))
                    {
                        SKBitmap debugBitmap = new SKBitmap(800, 600);
                        debugBitmap.Erase(new SKColor(35, 35, 35));
                        window.LoadImage(debugBitmap);
                    }
#endif

                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        window.LoadImage(imagePath);
                    }
                }
            }

            base.OnFrameworkInitializationCompleted();
        }

        private string? GetImagePathFromArgs(string[]? args)
        {
            if (args == null || args.Length == 0)
            {
                return null;
            }

            string filePath = args[0];

            if (File.Exists(filePath))
            {
                string extension = Path.GetExtension(filePath);

                if (Array.Exists(ImageExtensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                {
                    return filePath;
                }
            }

            return null;
        }
    }
}

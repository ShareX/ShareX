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
using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using DrawingImage = System.Drawing.Image;

namespace ShareX.HelpersLib;

public static class ImageViewerWindowIntegration
{
    public static void ShowImage() => ShowWindow(() => new ImageViewerWindow());

    public static void ShowImage(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            ShowImage();
            return;
        }

        ShowWindow(() => new ImageViewerWindow(filePath));
    }

    public static void ShowImage(IReadOnlyList<string> filePaths, int selectedIndex = 0)
    {
        if (filePaths.Count > 0)
        {
            string[] files = new string[filePaths.Count];
            for (int index = 0; index < filePaths.Count; index++)
            {
                files[index] = filePaths[index];
            }

            ShowWindow(() => new ImageViewerWindow(files, selectedIndex));
        }
    }

    public static void ShowImage(
        byte[]? imageData,
        string? displayName = null,
        Window? owner = null)
    {
        if (imageData is not { Length: > 0 })
        {
            return;
        }

        byte[] data = (byte[])imageData.Clone();
        ShowWindow(() => new ImageViewerWindow(data, displayName), owner);
    }

    public static void ShowImage(DrawingImage? image, Window? owner = null)
    {
        if (image == null)
        {
            return;
        }

        try
        {
            using DrawingImage? clonedImage = image.CloneSafe();
            if (clonedImage == null)
            {
                return;
            }

            using MemoryStream stream = new();
            clonedImage.Save(stream, ImageFormat.Png);
            ShowImage(stream.ToArray(), owner: owner);
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception, "Failed to prepare image viewer data.");
        }
    }

    private static void ShowWindow(Func<Window> windowFactory, Window? owner = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();

        void ShowCore()
        {
            try
            {
                Window window = windowFactory();
                if (owner is { IsVisible: true } visibleOwner)
                {
                    window.Closed += (_, _) => Dispatcher.UIThread.Post(() =>
                    {
                        if (visibleOwner.IsVisible)
                        {
                            visibleOwner.Activate();
                        }
                    }, DispatcherPriority.Input);

                    window.Show(visibleOwner);
                }
                else
                {
                    window.Show();
                }
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
            }
        }

        if (Dispatcher.UIThread.CheckAccess())
        {
            ShowCore();
        }
        else
        {
            Dispatcher.UIThread.Post(ShowCore);
        }
    }
}

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

using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using ShareX.Tools.HashChecker;
using ShareX.Tools.ImageCombiner;
using ShareX.Tools.QrCode;
using ShareX.Tools.VideoConverter;

namespace ShareX.Tools.Integration;

public static class ToolsIntegration
{
    public static void ShowHashCheckerWindow(HashCalculationHandler handler, Action? playSound = null, string? filePath = null)
    {
        Show(() => new HashCheckerWindow(handler, playSound, filePath));
    }

    public static void ShowImageCombinerWindow(ImageCombinerSettings settings, ImageCombinerServices services,
        Action<ImageCombinerSettings>? settingsChanged = null, IEnumerable<string>? imageFiles = null)
    {
        Show(() => new ImageCombinerWindow(settings, services, settingsChanged, imageFiles));
    }

    public static void ShowQrCodeWindow(QrCodeServices services, QrCodeWindowOptions options)
    {
        Show(() => new QrCodeWindow(services, options));
    }

    public static void ShowVideoConverterWindow(VideoConverterSettings settings, VideoConversionHandler handler,
        Action<VideoConverterSettings>? settingsChanged = null, string? inputFilePath = null)
    {
        Show(() => new VideoConverterWindow(settings, handler, settingsChanged, inputFilePath));
    }

    private static void Show(Func<Avalonia.Controls.Window> windowFactory)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() => windowFactory().Show());
    }
}

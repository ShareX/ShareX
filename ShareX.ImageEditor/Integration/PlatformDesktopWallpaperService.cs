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
using ShareX.Platform;
using PlatformWallpaperService = ShareX.Platform.IDesktopWallpaperService;
using PlatformWallpaperLayout = ShareX.Platform.DesktopWallpaperLayout;

namespace ShareX.ImageEditor.Integration;

/// <summary>Adapts the application platform service without replacing a host's custom editor service.</summary>
internal sealed class PlatformDesktopWallpaperService : IDesktopWallpaperService
{
    private readonly Func<PlatformWallpaperService> resolveService;

    public PlatformDesktopWallpaperService() : this(() => PlatformServices.Current.Wallpaper) { }

    internal PlatformDesktopWallpaperService(PlatformWallpaperService service) : this(() => service) { }

    private PlatformDesktopWallpaperService(Func<PlatformWallpaperService> resolveService) => this.resolveService = resolveService;

    public bool IsSupported => resolveService().Support.IsSupported;
    public bool RequiresDesktopWallpaperPrewarm => resolveService().RequiresPrewarm;

    public bool TryGetDesktopWallpaper(out DesktopWallpaperInfo? wallpaper)
    {
        wallpaper = null;
        PlatformWallpaperService service = resolveService();
        if (!service.Support.IsSupported || service.GetWallpaper() is not DesktopWallpaper result) return false;

        wallpaper = new DesktopWallpaperInfo
        {
            Path = result.Path,
            Layout = result.Layout switch
            {
                PlatformWallpaperLayout.Fill => DesktopWallpaperLayout.Fill,
                PlatformWallpaperLayout.Fit => DesktopWallpaperLayout.Fit,
                PlatformWallpaperLayout.Stretch => DesktopWallpaperLayout.Stretch,
                PlatformWallpaperLayout.Center => DesktopWallpaperLayout.Center,
                PlatformWallpaperLayout.Tile => DesktopWallpaperLayout.Tile,
                PlatformWallpaperLayout.Span => DesktopWallpaperLayout.Span,
                _ => DesktopWallpaperLayout.Fill
            }
        };
        return true;
    }

    public void PrewarmDesktopWallpaper()
    {
        PlatformWallpaperService service = resolveService();
        if (service.Support.IsSupported) service.Prewarm();
    }
}
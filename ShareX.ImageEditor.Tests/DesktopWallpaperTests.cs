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
using ShareX.ImageEditor.Integration;
using ShareX.Platform;
using Xunit;
using EditorWallpaperLayout = ShareX.ImageEditor.Integration.DesktopWallpaperLayout;
using PlatformWallpaperLayout = ShareX.Platform.DesktopWallpaperLayout;
using PlatformWallpaperService = ShareX.Platform.IDesktopWallpaperService;

namespace ShareX.ImageEditor.Tests;

[Collection("Editor graphics")]
public sealed class DesktopWallpaperTests
{
    [Theory]
    [InlineData(PlatformWallpaperLayout.Fill, EditorWallpaperLayout.Fill)]
    [InlineData(PlatformWallpaperLayout.Fit, EditorWallpaperLayout.Fit)]
    [InlineData(PlatformWallpaperLayout.Stretch, EditorWallpaperLayout.Stretch)]
    [InlineData(PlatformWallpaperLayout.Center, EditorWallpaperLayout.Center)]
    [InlineData(PlatformWallpaperLayout.Tile, EditorWallpaperLayout.Tile)]
    [InlineData(PlatformWallpaperLayout.Span, EditorWallpaperLayout.Span)]
    public void PlatformMetadataRetainsItsPathAndLayout(PlatformWallpaperLayout platformLayout, EditorWallpaperLayout editorLayout)
    {
        string path = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "fixture wallpaper.png"));
        FakeWallpaperService service = new() { Result = new(path, platformLayout) };
        PlatformDesktopWallpaperService adapter = new(service);
        Assert.True(adapter.IsSupported);
        Assert.True(adapter.TryGetDesktopWallpaper(out DesktopWallpaperInfo? wallpaper));
        Assert.NotNull(wallpaper);
        Assert.Equal(path, wallpaper.Path);
        Assert.Equal(editorLayout, wallpaper.Layout);
    }

    [Fact]
    public void UnavailableLookupAndPrewarmDoNotCallTheBackend()
    {
        FakeWallpaperService service = new() { Support = FeatureSupport.NotSupported("Unavailable in this session.") };
        PlatformDesktopWallpaperService adapter = new(service);
        Assert.False(adapter.IsSupported);
        Assert.False(adapter.TryGetDesktopWallpaper(out DesktopWallpaperInfo? wallpaper));
        Assert.Null(wallpaper);
        adapter.PrewarmDesktopWallpaper();
        Assert.Equal(0, service.Lookups);
        Assert.Equal(0, service.Prewarms);

        service.Support = FeatureSupport.Supported;
        Assert.True(adapter.IsSupported);
        Assert.True(adapter.RequiresDesktopWallpaperPrewarm);
        Assert.False(adapter.TryGetDesktopWallpaper(out wallpaper));
        Assert.Null(wallpaper);
        Assert.Equal(1, service.Lookups);
        adapter.PrewarmDesktopWallpaper();
        Assert.Equal(1, service.Prewarms);
        service.RequiresPrewarm = false;
        Assert.False(adapter.RequiresDesktopWallpaperPrewarm);
    }

    [Fact]
    public void DefaultInitializationPreservesTheHostsOverride()
    {
        ShareX.ImageEditor.Integration.IDesktopWallpaperService? previous = EditorServices.DesktopWallpaper;
        try
        {
            PlatformDesktopWallpaperService custom = new(new FakeWallpaperService());
            EditorServices.DesktopWallpaper = custom;
            EditorServices.EnsureDefaultDesktopWallpaperService();
            Assert.Same(custom, EditorServices.DesktopWallpaper);

            EditorServices.DesktopWallpaper = null;
            EditorServices.EnsureDefaultDesktopWallpaperService();
            PlatformDesktopWallpaperService defaultService = Assert.IsType<PlatformDesktopWallpaperService>(EditorServices.DesktopWallpaper);
            EditorServices.EnsureDefaultDesktopWallpaperService();
            Assert.Same(defaultService, EditorServices.DesktopWallpaper);
        }
        finally
        {
            EditorServices.DesktopWallpaper = previous;
        }
    }

    private sealed class FakeWallpaperService : PlatformWallpaperService
    {
        public FeatureSupport Support { get; set; } = FeatureSupport.Supported;
        public bool RequiresPrewarm { get; set; } = true;
        public DesktopWallpaper? Result { get; set; }
        public int Lookups { get; private set; }
        public int Prewarms { get; private set; }
        public DesktopWallpaper? GetWallpaper() { Lookups++; return Result; }
        public void Prewarm() => Prewarms++;
    }
}
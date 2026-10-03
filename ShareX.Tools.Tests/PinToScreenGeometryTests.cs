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
using ShareX.HelpersLib;
using ShareX.Platform;
using Xunit;
using Point = System.Drawing.Point;

namespace ShareX.Tools.Tests;

public sealed class PinToScreenGeometryTests
{
    [Theory]
    [InlineData(ImageContentAlignment.TopLeft, -1590, 50)]
    [InlineData(ImageContentAlignment.TopCenter, -950, 50)]
    [InlineData(ImageContentAlignment.TopRight, -310, 50)]
    [InlineData(ImageContentAlignment.MiddleLeft, -1590, 380)]
    [InlineData(ImageContentAlignment.MiddleCenter, -950, 380)]
    [InlineData(ImageContentAlignment.MiddleRight, -310, 380)]
    [InlineData(ImageContentAlignment.BottomLeft, -1590, 710)]
    [InlineData(ImageContentAlignment.BottomCenter, -950, 710)]
    [InlineData(ImageContentAlignment.BottomRight, -310, 710)]
    public void PlacementUsesPhysicalWindowSizeOnScaledNegativeOriginScreen(ImageContentAlignment placement, int x, int y)
    {
        ScreenInfo screen = Screen(1.5);
        PixelPoint position = PinToScreenGeometry.GetInitialPosition(screen, new Size(200, 120), null, 12, placement, 10);
        Assert.Equal(new PixelPoint(x, y), position);
    }

    [Theory]
    [InlineData(1, -270, -282, -2)]
    [InlineData(1.25, -330, -345, -5)]
    [InlineData(1.5, -390, -408, -8)]
    [InlineData(2, -510, -534, -14)]
    public void RequestedImageOriginUsesScaledBorderAndShadow(double scaling, int originX, int expectedWindowX, int expectedWindowY)
    {
        PixelPoint position = PinToScreenGeometry.GetInitialPosition(Screen(scaling), new Size(224, 144),
            new Point(originX, 10), 12, ImageContentAlignment.BottomRight, 10);
        // Physical outer inset is subtracted from the requested image position, independent of screen placement settings.
        Assert.Equal(new PixelPoint(expectedWindowX, expectedWindowY), position);
    }

    [Theory]
    [InlineData(1, -1100, -650)]
    [InlineData(1.25, -1125, -662)]
    [InlineData(1.5, -1150, -675)]
    [InlineData(2, -1200, -700)]
    public void ImageResizeKeepsItsPhysicalCenter(double scaling, int x, int y)
    {
        PixelPoint before = new(-1000, -600);
        Size oldSize = new(200, 100);
        Size newSize = new(400, 200);
        PixelPoint after = PinToScreenGeometry.KeepCenter(before, oldSize, newSize, scaling);
        Assert.Equal(new PixelPoint(x, y), after);
        Assert.InRange(Math.Abs(before.X + oldSize.Width * scaling / 2 - (after.X + newSize.Width * scaling / 2)), 0, 0.5);
        Assert.InRange(Math.Abs(before.Y + oldSize.Height * scaling / 2 - (after.Y + newSize.Height * scaling / 2)), 0, 0.5);
    }

    [Fact]
    public void PhysicalSizeRoundsUpSoFractionalWindowsStayInsideWorkingArea()
    {
        PixelPoint point = PinToScreenGeometry.GetInitialPosition(Screen(1.25), new Size(200.2, 120.3), null, 12,
            ImageContentAlignment.BottomRight, 10);
        Assert.Equal(new PixelPoint(-261, 739), point);
    }

    [Fact]
    public void UnavailableGlobalPointerUsesToolkitPrimaryScreen()
    {
        ScreenInfo secondary = Screen(1.5);
        ScreenInfo primary = new("primary", "primary", new PlatformRectangle(0, 0, 1920, 1080),
            new PlatformRectangle(0, 0, 1920, 1040), true, 1);
        Assert.Same(primary, PinToScreenGeometry.FindScreen([secondary, primary], null));
        Assert.Same(secondary, PinToScreenGeometry.FindScreen([secondary, primary], new PlatformPoint(-1, 0)));
        Assert.Same(primary, PinToScreenGeometry.FindScreen([secondary, primary], new PlatformPoint(0, 0)));
        Assert.Same(secondary, PinToScreenGeometry.FindScreen([secondary], null));
        Assert.Null(PinToScreenGeometry.FindScreen([], null));
    }

    [Fact]
    public void RequestedPositionInDesktopGapUsesNearestMonitorScaling()
    {
        ScreenInfo secondary = Screen(1.5) with
        {
            Bounds = new PlatformRectangle(-1800, -200, 1600, 1000),
            WorkingArea = new PlatformRectangle(-1800, -200, 1600, 960)
        };
        ScreenInfo primary = new("primary", "primary", new PlatformRectangle(0, 0, 1920, 1080),
            new PlatformRectangle(0, 0, 1920, 1040), true, 1);
        Assert.Same(secondary, PinToScreenGeometry.FindScreen([primary, secondary], new PlatformPoint(-190, -50)));
        Assert.Same(primary, PinToScreenGeometry.FindScreen([primary, secondary], new PlatformPoint(5000, 0)));
    }

    [Fact]
    public void OneHundredPercentDpiKeepsExistingPlacementBehavior()
    {
        foreach (ImageContentAlignment placement in Enum.GetValues<ImageContentAlignment>())
        {
            PixelPoint actual = PinToScreenGeometry.GetInitialPosition(Screen(1), new Size(200, 120), null, 12, placement, 10);
            Point expected = Helpers.GetPosition(placement, 10, new System.Drawing.Rectangle(-1600, 40, 1600, 860),
                new System.Drawing.Size(200, 120));
            Assert.Equal(new PixelPoint(expected.X, expected.Y), actual);
        }
    }

    private static ScreenInfo Screen(double scaling) => new("secondary", "secondary",
        new PlatformRectangle(-1600, 0, 1600, 900), new PlatformRectangle(-1600, 40, 1600, 860), false, scaling);
}

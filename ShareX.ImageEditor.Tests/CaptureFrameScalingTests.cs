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
using ShareX.ScreenCaptureLib;
using Xunit;

namespace ShareX.ImageEditor.Tests;

public sealed class CaptureFrameScalingTests
{
    [Theory]
    [InlineData(1.25)]
    [InlineData(1.5)]
    [InlineData(2.0)]
    [InlineData(0.5)]
    public void WideToolbarOriginDoesNotDetermineTheRenderScale(double renderScaling)
    {
        // The capture lies on a scaled monitor starting at X=1920. The toolbar
        // extends left into a monitor at scale 1, so its origin is a bad DPI anchor.
        PlatformRectangle region = new(1920, 10, 40, 30);
        RecordingFrameLayout layout = FrameWindowShape.CreateRecordingLayout(region, renderScaling, 461, 42, 1, 3);
        Assert.True(layout.Position.X < 1920);
        Assert.Equal(region.X, layout.Position.X + layout.Frame.X + 1);
        Assert.Equal(region.Y, layout.Position.Y + 1);
        Assert.Equal(42, layout.Frame.Width);
        Assert.Equal(32, layout.Frame.Height);
        var logicalSize = FrameWindowShape.GetLogicalSize(layout.Frame.Width, layout.Frame.Height, renderScaling);
        Assert.Equal(42, logicalSize.Width * renderScaling, 8);
        Assert.Equal(32, logicalSize.Height * renderScaling, 8);
        // Dividing by the origin monitor's scale 1 would produce a wrong frame.
        Assert.NotEqual(42, 42 * renderScaling);
        var shape = FrameWindowShape.Create(layout.Frame, 1, layout.Toolbar);
        Assert.Equal(5, shape.Count);
        Assert.Equal(1, shape[0].Height);
        Assert.Equal(1, shape[2].Width);
        Assert.Equal(layout.Toolbar, shape[4]);
        Assert.Equal(layout.Frame.Height + 3, layout.Toolbar.Y);
    }

    [Fact]
    public void ScalingAndToolbarChangesKeepThePhysicalRecordingAreaOnNegativeOrigins()
    {
        PlatformRectangle region = new(-1900, -650, 800, 600);
        foreach (double scaling in new[] { 1.0, 1.25, 1.5, 2.0, 1.25, 1.0 })
        {
            foreach (double toolbarWidth in new[] { 461.0, 349.0, 277.0, 165.0, 461.0 })
            {
                RecordingFrameLayout layout = FrameWindowShape.CreateRecordingLayout(region, scaling, toolbarWidth, 42, 1, 3);
                PlatformRectangle actual = new(layout.Position.X + layout.Frame.X + 1, layout.Position.Y + 1,
                    layout.Frame.Width - 2, layout.Frame.Height - 2);
                Assert.Equal(region, actual);
                Assert.True(layout.Width * scaling >= layout.Frame.Right);
                Assert.True(layout.Width * scaling >= layout.Toolbar.Right);
                Assert.Equal(42, layout.Height - layout.Toolbar.Y / scaling, 8);
                Assert.Equal((int)Math.Ceiling(toolbarWidth * scaling), layout.Toolbar.Width);
            }
        }
    }

    [Theory]
    [InlineData(1.0)]
    [InlineData(1.25)]
    [InlineData(1.5)]
    [InlineData(2.0)]
    [InlineData(0.25)]
    public void ScrollingFrameKeepsItsNativeDimensionsAtEveryValidScale(double scaling)
    {
        var size = FrameWindowShape.GetLogicalSize(1244, 756, scaling);
        Assert.Equal(1244, size.Width * scaling, 8);
        Assert.Equal(756, size.Height * scaling, 8);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void UnavailableOrInvalidScaleUsesAValidInitialLayout(double scaling)
    {
        RecordingFrameLayout layout = FrameWindowShape.CreateRecordingLayout(new(10, 20, 40, 30), scaling, 461, 42, 1, 3);
        Assert.Equal(1, layout.Scaling);
        Assert.True(double.IsFinite(layout.Width));
        Assert.True(double.IsFinite(layout.Height));
        Assert.Equal(10, layout.Position.X + layout.Frame.X + 1);
        Assert.Equal(20, layout.Position.Y + 1);
        var size = FrameWindowShape.GetLogicalSize(42, 32, scaling);
        Assert.Equal(42, size.Width);
        Assert.Equal(32, size.Height);
    }
}

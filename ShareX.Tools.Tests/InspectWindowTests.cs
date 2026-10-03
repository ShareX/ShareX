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
using ShareX.Platform.Windows;
using System.Runtime.InteropServices;
using Xunit;

namespace ShareX.Tools.Tests;

[Collection("Inspector native window")]
public sealed class InspectWindowTests
{
    [Fact]
    public void InspectorUsesPortableDetailsAndClearsClosedWindows()
    {
        Exception? failure = null;
        Thread thread = new(() => { try { VerifyInspector(); } catch (Exception exception) { failure = exception; } }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(15)), "Inspector verification did not complete.");
        if (failure != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
    }

    private static void Equal<T>(T expected, T actual) => Assert.Equal(expected, actual);

    private static void VerifyInspector()
    {
        PlatformServices.Initialize(new WindowsPlatformServices());
        // Off-screen tool window: visible to enumeration without putting a window on the user's desktop.
        IntPtr window = CreateWindowExW(0x08000080, "STATIC", "ShareX inspector verification", 0x10000000,
            -32000, -32000, 100, 80, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        if (window == IntPtr.Zero) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

        try
        {
            Equal(true, PlatformServices.Current.Windows.GetWindows().Any(item => item.Handle == window.ToInt64()));
            PlatformRectangle client = PlatformServices.Current.WindowManagement.GetDetails(window.ToInt64())!.ClientBounds!.Value;
            Equal(0, client.X);
            Equal(0, client.Y);
            Equal(false, client.IsEmpty);
            using ShareX.Tools.InspectWindowViewModel viewModel = new();
            viewModel.SelectWindow(window, true);
            Equal(true, viewModel.HasSelection);
            Equal("ShareX inspector verification", viewModel.SelectedTitle);
            Equal(true, viewModel.CanChangeTopMost);
            Equal(true, viewModel.CanChangeOpacity);
            Equal(10, viewModel.Details.Count);
            viewModel.IsTopMost = true;
            Equal(true, PlatformServices.Current.WindowManagement.GetDetails(window.ToInt64())!.IsTopMost!.Value);
            viewModel.Opacity = 50;
            Equal((byte)128, PlatformServices.Current.WindowManagement.GetDetails(window.ToInt64())!.Opacity!.Value);
            viewModel.SelectWindow(window, false);
            Equal(false, viewModel.CanChangeTopMost);
            Equal(false, viewModel.CanChangeOpacity);
            DestroyWindow(window);
            window = IntPtr.Zero;
            viewModel.RefreshCommand.Execute(null);
            Equal(false, viewModel.HasSelection);
            Equal(0, viewModel.Details.Count);
        }
        finally
        {
            if (window != IntPtr.Zero) DestroyWindow(window);
            PlatformServices.Shutdown();
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateWindowExW(uint extendedStyle, string className, string title, uint style,
        int x, int y, int width, int height, IntPtr parent, IntPtr menu, IntPtr instance, IntPtr parameter);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyWindow(IntPtr window);
}

[CollectionDefinition("Inspector native window", DisableParallelization = true)]
public sealed class InspectorNativeWindowCollection { }

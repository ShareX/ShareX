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

using ShareX.Desktop.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ShareX.Desktop.Tests
{
    public sealed class TrayWatcherCancellationTests
    {
        [Fact]
        public async Task CancellationFromTrayWatcherIsRecognised()
        {
            Exception exception = await Record.ExceptionAsync(() => Avalonia.FreeDesktop.DBusTrayIconImpl.WatchAsync());

            Assert.IsType<TaskCanceledException>(exception);
            Assert.True(DesktopHost.IsTrayWatcherCancellation(exception));
        }

        [Fact]
        public async Task CancellationFromOtherCodeIsNotRecognised()
        {
            Exception exception = await Record.ExceptionAsync(() => Task.Delay(Timeout.Infinite, new CancellationToken(true)));

            Assert.IsType<TaskCanceledException>(exception);
            Assert.False(DesktopHost.IsTrayWatcherCancellation(exception));
        }

        [Fact]
        public async Task OtherExceptionFromTrayWatcherIsNotRecognised()
        {
            Exception exception = await Record.ExceptionAsync(() => Avalonia.FreeDesktop.DBusTrayIconImpl.FailAsync());

            Assert.IsType<InvalidOperationException>(exception);
            Assert.False(DesktopHost.IsTrayWatcherCancellation(exception));
        }

        [Fact]
        public void UnthrownCancellationIsNotRecognised()
        {
            Assert.False(DesktopHost.IsTrayWatcherCancellation(new OperationCanceledException()));
        }
    }
}

namespace Avalonia.FreeDesktop
{
    /// <summary>Stands in for Avalonia's type so the exception carries the same stack frame as the real tray watcher.</summary>
    internal static class DBusTrayIconImpl
    {
        public static async Task WatchAsync()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            await Task.Delay(Timeout.Infinite, cancellation.Token);
        }

        public static async Task FailAsync()
        {
            await Task.Yield();
            throw new InvalidOperationException();
        }
    }
}

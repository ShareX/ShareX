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

using ShareX.HelpersLib;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ShareX.ImageEditor.Tests;

/// <summary>R40: the recording worker's waits end when the recording window closes and disposes the event it owns.</summary>
public sealed class OwnedWaitTests
{
    [Fact]
    public async Task SignalEndsTheWait()
    {
        using ManualResetEvent signal = new ManualResetEvent(false);
        Task<OwnedWaitResult> wait = Task.Run(() => OwnedWait.Wait(signal, () => false));
        signal.Set();

        Assert.Equal(OwnedWaitResult.Signaled, await wait.WaitAsync(TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public void TimeoutIsKept()
    {
        using ManualResetEvent signal = new ManualResetEvent(false);
        Stopwatch timer = Stopwatch.StartNew();

        Assert.Equal(OwnedWaitResult.TimedOut, OwnedWait.Wait(signal, () => false, 120));
        Assert.InRange(timer.ElapsedMilliseconds, 100, 2000);
    }

    [Fact]
    public async Task ClosingTheOwnerEndsAnUnboundedWait()
    {
        using ManualResetEvent signal = new ManualResetEvent(false);
        bool closed = false;
        Task<OwnedWaitResult> wait = Task.Run(() => OwnedWait.Wait(signal, () => Volatile.Read(ref closed)));
        await Task.Delay(100);
        Volatile.Write(ref closed, true);

        Assert.Equal(OwnedWaitResult.OwnerClosed, await wait.WaitAsync(TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public async Task DisposingTheHandleMidWaitEndsTheWait()
    {
        ManualResetEvent signal = new ManualResetEvent(false);
        Task<OwnedWaitResult> wait = Task.Run(() => OwnedWait.Wait(signal, () => false));
        await Task.Delay(100);
        signal.Dispose();

        Assert.Equal(OwnedWaitResult.OwnerClosed, await wait.WaitAsync(TimeSpan.FromSeconds(5)));
    }
}

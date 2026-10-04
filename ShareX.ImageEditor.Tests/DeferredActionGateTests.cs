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
using System.Collections.Generic;
using Xunit;

namespace ShareX.ImageEditor.Tests;

/// <summary>R36: startup holds warnings until its command line work has run, then shows them in order.</summary>
public sealed class DeferredActionGateTests
{
    [Fact]
    public void HeldActionsRunAfterReleaseInOrder()
    {
        DeferredActionGate gate = new DeferredActionGate();
        List<string> log = new List<string>();

        gate.Hold();
        gate.Run(() => log.Add("hotkey warning"));
        log.Add("command line work");
        gate.Run(() => log.Add("second warning"));
        gate.Release();

        Assert.Equal(["command line work", "hotkey warning", "second warning"], log);
        Assert.False(gate.IsHeld);
    }

    [Fact]
    public void ActionsRunAtOnceWhenNotHeldAndReleaseIsSafeToRepeat()
    {
        DeferredActionGate gate = new DeferredActionGate();
        int runs = 0;

        gate.Run(() => runs++);
        gate.Release();
        gate.Release();

        Assert.Equal(1, runs);
    }

    [Fact]
    public void ActionQueuedByAReleasedActionRunsImmediately()
    {
        DeferredActionGate gate = new DeferredActionGate();
        List<int> order = new List<int>();

        gate.Hold();
        gate.Run(() => { order.Add(1); gate.Run(() => order.Add(2)); });
        gate.Release();

        Assert.Equal([1, 2], order);
    }
}

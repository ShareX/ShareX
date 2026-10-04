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
using Xunit;
using Strings = ShareX.Tools.Localization.Strings;

namespace ShareX.Tools.Tests;

public sealed class MouseHighlighterSupportTests
{
    [Fact]
    public void UnsupportedStartAndSettingsDoNotInvokeTheirCallbacksOrExposeInstallAdvice()
    {
        FeatureSupport unavailable = FeatureSupport.NotSupported("Install a legacy mouse-hook helper.");
        MouseHighlighterWindowViewModel viewModel = new(() => unavailable, () => false, () => false);
        MouseHighlighterOptions options = new() { Radius = -1, AutoActivate = true };
        int starts = 0, changes = 0;

        Assert.False(viewModel.ToggleSupport.IsSupported);
        Assert.False(viewModel.SettingsSupport.IsSupported);
        Assert.Equal(Strings.MouseHighlighter_Unavailable, viewModel.ToggleSupport.Reason);
        Assert.NotEqual(unavailable.Reason, viewModel.SettingsSupport.Reason);
        Assert.Equal(Strings.MouseHighlighter_Start, viewModel.ToggleText);
        Assert.False(viewModel.TryToggle(_ => starts++));
        Assert.False(viewModel.TryChangeSettings(() => { options.Validate(); options.AutoActivate = false; changes++; }));
        Assert.Equal(-1, options.Radius);
        Assert.True(options.AutoActivate);
        Assert.Equal(0, starts);
        Assert.Equal(0, changes);
    }

    [Fact]
    public void CapabilityLossBlocksStaleStartButAnActiveHighlighterCanStillStopAfterRecording()
    {
        FeatureSupport support = FeatureSupport.Supported;
        bool manuallyActive = false;
        MouseHighlighterWindowViewModel viewModel = new(() => support, () => manuallyActive, () => true);
        List<bool> requestedStates = [];
        Assert.True(viewModel.ToggleSupport.IsSupported);

        support = FeatureSupport.NotSupported("The fixture overlay disappeared.");
        Assert.False(viewModel.TryToggle(requestedStates.Add));
        Assert.Empty(requestedStates);
        Assert.Equal(Strings.MouseHighlighter_KeepAfterRecording, viewModel.ToggleText);

        manuallyActive = true;
        Assert.True(viewModel.ToggleSupport.IsSupported);
        Assert.Null(viewModel.ToggleSupport.Reason);
        Assert.Equal(Strings.MouseHighlighter_StopAfterRecording, viewModel.ToggleText);
        Assert.False(viewModel.TryChangeSettings(() => Assert.Fail("Unavailable settings must remain unchanged while stopping.")));
        Assert.True(viewModel.TryToggle(active => { requestedStates.Add(active); manuallyActive = active; }));
        Assert.Equal(new[] { false }, requestedStates);
        Assert.False(manuallyActive);
        Assert.False(viewModel.ToggleSupport.IsSupported);
    }

    [Fact]
    public void SupportedStartStopAndRecordingLabelsPreserveTheirStateAndCallbackFailures()
    {
        bool manuallyActive = false, recordingActive = false;
        MouseHighlighterWindowViewModel viewModel = new(() => FeatureSupport.Supported,
            () => manuallyActive, () => recordingActive);
        List<bool> requestedStates = [];
        void SetActive(bool active) { requestedStates.Add(active); manuallyActive = active; }

        Assert.Null(viewModel.SettingsSupport.Reason);
        Assert.Equal(Strings.MouseHighlighter_Start, viewModel.ToggleText);
        Assert.True(viewModel.TryToggle(SetActive));
        Assert.Equal(Strings.MouseHighlighter_Stop, viewModel.ToggleText);
        recordingActive = true;
        Assert.Equal(Strings.MouseHighlighter_StopAfterRecording, viewModel.ToggleText);
        Assert.True(viewModel.TryToggle(SetActive));
        Assert.Equal(Strings.MouseHighlighter_KeepAfterRecording, viewModel.ToggleText);
        Assert.True(viewModel.TryToggle(SetActive));
        Assert.Equal(new[] { true, false, true }, requestedStates);

        MouseHighlighterOptions options = new() { Radius = -1 };
        int changes = 0;
        Assert.True(viewModel.TryChangeSettings(() => { options.Validate(); changes++; }));
        Assert.InRange(options.Radius, 5, 500);
        Assert.Equal(1, changes);
        InvalidOperationException error = new("Fixture native activation failed.");
        Assert.Same(error, Assert.Throws<InvalidOperationException>(() => viewModel.TryToggle(_ => throw error)));
        Assert.True(manuallyActive); // The backend retains responsibility for rolling its state back on failure.
    }

    [Fact]
    public void StartupSkipsUnsupportedDesktopAndKeepsSavedFlag()
    {
        MouseHighlighterOptions options = new() { AutoActivate = true };
        int toggles = 0;

        bool started = MouseHighlighterManager.ActivateOnStartup(options,
            () => FeatureSupport.NotSupported("Wayland does not allow it."), _ => toggles++);

        Assert.False(started);
        Assert.Equal(0, toggles);
        Assert.True(options.AutoActivate);
    }

    [Fact]
    public void StartupActivatesWhenSupportedAndRequested()
    {
        int toggles = 0;

        Assert.True(MouseHighlighterManager.ActivateOnStartup(new MouseHighlighterOptions { AutoActivate = true }, () => FeatureSupport.Supported, _ => toggles++));
        Assert.False(MouseHighlighterManager.ActivateOnStartup(new MouseHighlighterOptions { AutoActivate = false }, () => FeatureSupport.Supported, _ => toggles++));
        Assert.Equal(1, toggles);
    }
}

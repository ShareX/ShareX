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

namespace ShareX.Tools.Tests;

public sealed class BackgroundRemoverDeviceTests
{
    [Fact]
    public async Task CpuOnlySupportPreservesSavedGpuAndStopsProcessingBeforeReadingFiles()
    {
        const string reason = "GPU processing is unavailable in this fixture.";
        BackgroundRemoverOptions options = new() { SelectedDevice = BackgroundRemovalDevice.GPU };
        using BackgroundRemoverViewModel viewModel = new(null, options, device =>
            device == BackgroundRemovalDevice.GPU ? FeatureSupport.NotSupported(reason) : FeatureSupport.Supported);
        PrepareSelection(viewModel);
        List<bool> processingChanges = [];
        viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(viewModel.IsProcessing)) processingChanges.Add(viewModel.IsProcessing);
        };

        Assert.Equal(BackgroundRemovalDevice.GPU, viewModel.SelectedDeviceOption!.Device);
        Assert.False(viewModel.SelectedDeviceOption.IsSupported);
        Assert.Equal(reason, viewModel.SelectedDeviceSupportReason);
        Assert.Equal(reason, viewModel.ProcessingDeviceToolTip);
        Assert.False(viewModel.RemoveBackgroundCommand.CanExecute(null));
        await viewModel.RemoveBackgroundCommand.ExecuteAsync(null);
        Assert.Empty(processingChanges);
        Assert.Null(viewModel.ResultPreviewImage);
        Assert.Equal(BackgroundRemovalDevice.GPU, options.SelectedDevice);

        int commandChanges = 0;
        viewModel.RemoveBackgroundCommand.CanExecuteChanged += (_, _) => commandChanges++;
        foreach (BackgroundRemovalDevice device in new[] { BackgroundRemovalDevice.Auto, BackgroundRemovalDevice.CPU })
        {
            viewModel.SelectedDeviceOption = Assert.Single(viewModel.AvailableDevices, option => option.Device == device);
            Assert.True(viewModel.SelectedDeviceOption.IsSupported);
            Assert.True(viewModel.RemoveBackgroundCommand.CanExecute(null));
            Assert.Null(viewModel.SelectedDeviceSupportReason);
            Assert.Equal(device, options.SelectedDevice);
        }
        Assert.True(commandChanges >= 2);
    }

    [Fact]
    public void SupportedGpuKeepsAllDeviceChoicesAndExistingSelection()
    {
        BackgroundRemoverOptions options = new() { SelectedDevice = BackgroundRemovalDevice.GPU };
        using BackgroundRemoverViewModel viewModel = new(null, options, _ => FeatureSupport.Supported);
        PrepareSelection(viewModel);
        Assert.Equal(Enum.GetValues<BackgroundRemovalDevice>(), viewModel.AvailableDevices.Select(option => option.Device));
        Assert.All(viewModel.AvailableDevices, option => Assert.True(option.IsSupported));
        Assert.Equal(BackgroundRemovalDevice.GPU, viewModel.SelectedDeviceOption!.Device);
        Assert.Equal(BackgroundRemovalDevice.GPU, options.SelectedDevice);
        Assert.Equal(Localization.Strings.BackgroundRemoverWindow_Processing_device, viewModel.ProcessingDeviceToolTip);
        Assert.True(viewModel.RemoveBackgroundCommand.CanExecute(null));
    }

    private static void PrepareSelection(BackgroundRemoverViewModel viewModel)
    {
        // These paths deliberately do not exist: capability guards must run before decoding or inference.
        viewModel.ImagePath = "unused-fixture-image.png";
        viewModel.SelectedModel = new BackgroundRemovalModel
        {
            FilePath = "unused-fixture-model.onnx", FileName = "unused-fixture-model.onnx", FileSize = 0
        };
    }
}

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
using ShareX.Platform;
using Xunit;

namespace ShareX.ImageEditor.Tests;

public sealed class PrintWindowSupportTests
{
    [Fact]
    public void UnsupportedPrintingDoesNotInvokeTheActionOrExposeInstallAdvice()
    {
        FeatureSupport support = FeatureSupport.NotSupported("Install a legacy printing package to enable this feature.");
        PrintWindowViewModel viewModel = new(getSupport: () => support);
        int calls = 0;
        Assert.False(viewModel.CanPrint);
        Assert.Equal(ShareX.HelpersLib.Localization.Strings.PrintWindow_PrintingUnavailable, viewModel.PrintingUnavailableReason);
        Assert.NotEqual(support.Reason, viewModel.PrintingUnavailableReason);
        Assert.False(viewModel.TryPrint(() => calls++));
        Assert.Equal(0, calls);

        // An already-open window must recheck support if its backend becomes unavailable.
        support = FeatureSupport.Supported;
        Assert.True(viewModel.CanPrint);
        Assert.Null(viewModel.PrintingUnavailableReason);
        support = FeatureSupport.NotSupported("Fixture service was removed.");
        Assert.False(viewModel.TryPrint(() => calls++));
        Assert.Equal(0, calls);
    }

    [Fact]
    public void PreviewOnlyRejectsPrintingAndSupportedActionsKeepTheirOwnResult()
    {
        PrintWindowViewModel previewOnly = new(previewOnly: true, getSupport: () => FeatureSupport.Supported);
        Assert.False(previewOnly.CanPrint);
        Assert.Null(previewOnly.PrintingUnavailableReason);
        Assert.False(previewOnly.TryPrint(() => Assert.Fail("Preview-only mode must never start a print job.")));

        PrintWindowViewModel supported = new(getSupport: () => FeatureSupport.Supported);
        bool printed = true;
        int calls = 0;
        Assert.True(supported.CanPrint);
        Assert.True(supported.TryPrint(() => { calls++; printed = false; }));
        Assert.Equal(1, calls);
        Assert.False(printed); // A cancelled print dialog remains false for the preview's existing close condition.
        Assert.True(supported.TryPrint(() => { calls++; printed = true; }));
        Assert.Equal(2, calls);
        Assert.True(printed);
    }
}

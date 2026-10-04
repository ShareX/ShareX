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

using ShareX.Platform.MacOS;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace ShareX.Platform.Tests;

public class MacFinderTests
{
    private static readonly ShellMenuEntry Upload = new ShellMenuEntry("ShareX", "Upload with ShareX", "/Applications/Share X's.app/Contents/MacOS/ShareX", [], ShellMenuTarget.FilesAndFolders);
    private static readonly ShellMenuEntry Edit = new ShellMenuEntry("ShareXImageEditor", "Edit <with> ShareX & co", "/Applications/ShareX.app/Contents/MacOS/ShareX", ["-ImageEditor"], ShellMenuTarget.Images);

    [Fact]
    public void Command_QuotesPathAndArgumentsAndPassesSelection()
    {
        Assert.Equal("'/Applications/Share X'\\''s.app/Contents/MacOS/ShareX' \"$@\" >/dev/null 2>&1 &", FinderQuickAction.CreateCommand(Upload));
        Assert.Equal("'/Applications/ShareX.app/Contents/MacOS/ShareX' '-ImageEditor' \"$@\" >/dev/null 2>&1 &", FinderQuickAction.CreateCommand(Edit));
    }

    [Fact]
    public void Bundle_IsWellFormedAndTargetsFinder()
    {
        Assert.Equal("ShareX.workflow", FinderQuickAction.GetBundleName(Upload));
        Assert.Equal("a_b.workflow", FinderQuickAction.GetBundleName(Upload with { Id = "a/b" }));

        XDocument info = XDocument.Parse(FinderQuickAction.CreateInfoPlist(Edit));
        string[] infoStrings = info.Descendants("string").Select(x => x.Value).ToArray();
        Assert.Contains("Edit <with> ShareX & co", infoStrings);
        Assert.Contains("public.image", infoStrings);
        Assert.Contains("com.apple.finder", infoStrings);
        Assert.Contains("public.item", XDocument.Parse(FinderQuickAction.CreateInfoPlist(Upload)).Descendants("string").Select(x => x.Value));

        XDocument document = XDocument.Parse(FinderQuickAction.CreateDocument(Edit));
        string[] strings = document.Descendants("string").Select(x => x.Value).ToArray();
        Assert.Contains(FinderQuickAction.CreateCommand(Edit), strings);
        Assert.Contains("com.apple.Automator.servicesMenu", strings);
        Assert.Contains("com.apple.Automator.fileSystemObject.image", strings);
        Assert.Equal(FinderQuickAction.CreateDocument(Edit), FinderQuickAction.CreateDocument(Edit));
    }

    [Fact]
    public void Service_RegistersAndRemovesQuickActions()
    {
        string library = Path.Combine(Path.GetTempPath(), "sharex-library-" + Guid.NewGuid().ToString("N"));
        RecordingRunner runner = new RecordingRunner();
        MacShellIntegrationService service = new MacShellIntegrationService(library, runner);

        try
        {
            Assert.True(service.Support.IsSupported);
            Assert.False(service.IsRegistered(Upload));

            service.Register(Upload);
            Assert.True(service.IsRegistered(Upload));
            Assert.True(File.Exists(Path.Combine(library, "Services", "ShareX.workflow", "Contents", "Info.plist")));
            Assert.False(service.IsRegistered(Upload with { ExecutablePath = "/Applications/Other.app/Contents/MacOS/ShareX" }));
            Assert.False(service.IsRegistered(Edit));

            service.Unregister(Upload);
            Assert.False(Directory.Exists(Path.Combine(library, "Services", "ShareX.workflow")));
            Assert.Equal(2, runner.Calls.Count(call => call.Command == "/System/Library/CoreServices/pbs"));
        }
        finally
        {
            if (Directory.Exists(library)) Directory.Delete(library, true);
        }
    }

    [MacOSFact]
    public void Shell_KnowsCommonMimeTypes()
    {
        if (!OperatingSystem.IsMacOS()) return;
        MacShellService shell = new MacShellService(Diagnostics.CommandRunner.Default);

        Assert.Equal("image/png", shell.GetMimeType(".png"));
        Assert.Equal("image/jpeg", shell.GetMimeType("jpg"));
        Assert.Null(shell.GetMimeType(".sharex-unknown-extension"));
    }
}

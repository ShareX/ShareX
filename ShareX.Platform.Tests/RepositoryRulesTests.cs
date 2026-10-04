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

using ShareX.Platform.Linux;
using ShareX.Platform.MacOS;
using System;
using System.IO;
using Xunit;

namespace ShareX.Platform.Tests;

/// <summary>
/// McoreD and Jaex agree (2026-10-04) that Windows, Linux and macOS reach 100%, with OCR and other features through each system's
/// own support. These checks keep that agreement from being reverted by accident, for example by an agent working from older
/// instructions or resolving a merge conflict with an earlier version of AGENTS.md.
/// </summary>
public class RepositoryRulesTests
{
    private static string RepositoryFile(string name)
    {
        for (DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory); directory != null; directory = directory.Parent)
        {
            string path = Path.Combine(directory.FullName, name);
            if (File.Exists(path) && File.Exists(Path.Combine(directory.FullName, "ShareX.sln")))
            {
                return path;
            }
        }

        throw new FileNotFoundException("The repository root was not found above the test output.", name);
    }

    [Fact]
    public void AgentsFile_KeepsTheAgreedRules()
    {
        string rules = File.ReadAllText(RepositoryFile("AGENTS.md"));

        Assert.Contains("McoreD and Jaex agree", rules);
        Assert.Contains("The macOS phase is open", rules);
        Assert.Contains("OCR is implemented on Linux", rules);
        Assert.Contains("macOS uses Apple's Vision framework", rules);
        Assert.DoesNotContain("OCR remains Windows-only", rules);
        Assert.DoesNotContain("Do not work on the macOS port now", rules);
        Assert.DoesNotContain("## Linux dependencies: no extra installations", rules);
    }

    [Fact]
    public void CrossPlatformWorkflow_RunsOnThisBranch()
    {
        string workflow = File.ReadAllText(RepositoryFile(Path.Combine(".github", "workflows", "platform.yml")));

        Assert.DoesNotContain("branches-ignore", workflow);
        Assert.DoesNotContain("github.head_ref != 'cross-platform-v2'", workflow);
        Assert.Contains("macos-app:", workflow);
    }

    [Fact]
    public void Ocr_UsesEachSystemsOwnEngine()
    {
        Assert.Contains("Ocr = new TesseractOcrService(", File.ReadAllText(RepositoryFile(Path.Combine("ShareX.Platform.Linux", "LinuxPlatformServices.cs"))));
        Assert.Contains("Ocr { get; } = new VisionOcrService();", File.ReadAllText(RepositoryFile(Path.Combine("ShareX.Platform.MacOS", "MacPlatformServices.cs"))));
        Assert.True(typeof(IOcrService).IsAssignableFrom(typeof(TesseractOcrService)));
        Assert.True(typeof(IOcrService).IsAssignableFrom(typeof(VisionOcrService)));
    }
}

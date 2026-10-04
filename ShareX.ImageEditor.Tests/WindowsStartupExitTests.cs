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

using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace ShareX.ImageEditor.Tests;

[Collection("Windows application verification")]
public sealed class WindowsStartupExitTests
{
    [WindowsApplicationSmokeFact]
    public async Task InitialExitWithPendingWelcomeDoesNotOpenStartupWindows()
    {
        if (!OperatingSystem.IsWindows()) return;
        using WindowsApplicationSmokeTests.ApplicationFiles files = new();
        string settingsPath = Path.Combine(files.SettingsPath, "ApplicationConfig.json");
        JsonObject settings = Assert.IsType<JsonObject>(JsonNode.Parse(File.ReadAllText(settingsPath)));
        settings["ShowStartScreen"] = true;
        settings["ActionsToolbarRunAtStartup"] = true;
        File.WriteAllText(settingsPath, settings.ToJsonString());

        string log = await files.RunAsync("windows-application-first-start-exit.log",
            "-portable", "-multi", "-silent", "-NoHotkeys", "-ExitShareX");
        Assert.Contains("CommandLine: -ExitShareX", log);
        Assert.Contains("Hotkey host init finished.", log);
        Assert.Contains("ShareX closed.", log);
        Assert.DoesNotContain("Start screen opening.", log);
        using JsonDocument savedSettings = JsonDocument.Parse(File.ReadAllText(settingsPath));
        Assert.True(savedSettings.RootElement.GetProperty("ShowStartScreen").GetBoolean());
        Assert.True(savedSettings.RootElement.GetProperty("ActionsToolbarRunAtStartup").GetBoolean());
        Assert.True(savedSettings.RootElement.GetProperty("DisableUpload").GetBoolean());
        Assert.False(savedSettings.RootElement.GetProperty("AutoCheckUpdate").GetBoolean());
    }
}

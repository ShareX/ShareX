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
using Xunit;

namespace ShareX.ImageEditor.Tests;

/// <summary>R39: a first start that carries work does it before the welcome screen; options alone do not count as work.</summary>
public sealed class StartupActionTests
{
    private static readonly string[] Options = ["silent", "s", "portable", "NoHotkeys"];

    private static bool HasActions(params string[] args)
    {
        CLIManager cli = new CLIManager(args);
        cli.ParseCommands();
        return CLIManager.HasActions(cli.Commands, Options);
    }

    [Fact]
    public void OptionsAloneAreNotActions()
    {
        Assert.False(HasActions());
        Assert.False(HasActions("-silent"));
        Assert.False(HasActions("-s", "-portable", "-NoHotkeys"));
    }

    [Fact]
    public void JobsFilesAndBrowserInputAreActions()
    {
        Assert.True(HasActions("-silent", "-PrintScreen"));
        Assert.True(HasActions("-NativeMessagingInput", "/tmp/ShareX-native-1.json"));
        Assert.True(HasActions("/home/user/picture.png"));
        Assert.True(HasActions("-CustomUploader", "/tmp/a.sxcu"));
    }
}

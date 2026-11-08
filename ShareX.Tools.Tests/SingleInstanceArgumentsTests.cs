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
using System.IO;
using Xunit;

namespace ShareX.Tests;

public sealed class SingleInstanceArgumentsTests : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), "sharex-tests-" + Guid.NewGuid().ToString("N"));

    public SingleInstanceArgumentsTests()
    {
        Directory.CreateDirectory(Path.Combine(directory, "folder"));
        File.WriteAllText(Path.Combine(directory, "photo 1.png"), "");
    }

    public void Dispose() => Directory.Delete(directory, true);

    [Fact]
    public void FilesNamedRelativeToTheCallerReachTheFirstInstanceInFull()
    {
        string[] resolved = SingleInstanceManager.ResolveRelativePaths(["-ImageEditor", "photo 1.png", "folder"], directory);

        Assert.Equal(["-ImageEditor", Path.Combine(directory, "photo 1.png"), Path.Combine(directory, "folder")], resolved);
    }

    [Fact]
    public void OtherArgumentsAreLeftAlone()
    {
        string absolute = Path.Combine(directory, "photo 1.png");
        string[] arguments = [absolute, "missing.png", "https://example.com/a.png", "", "-task", "Upload"];

        Assert.Equal(arguments, SingleInstanceManager.ResolveRelativePaths(arguments, directory));
    }
}

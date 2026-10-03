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

using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ShareX.Platform;

/// <summary>Builds the native messaging manifest a browser reads, from the one ShareX ships.</summary>
public static class NativeMessagingManifest
{
    /// <summary>The shipped manifest with "name" and an absolute "path", as Chrome and Firefox require outside Windows.</summary>
    public static string Create(BrowserHost host)
    {
        JsonNode manifest = JsonNode.Parse(File.ReadAllText(host.ManifestPath)) ?? new JsonObject();
        manifest["name"] = host.Name;
        manifest["path"] = Path.GetFullPath(host.HostExecutablePath);
        return manifest.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Whether a manifest file already points at this host's executable.</summary>
    public static bool PointsAt(string manifestFile, BrowserHost host)
    {
        try
        {
            return File.Exists(manifestFile) &&
                JsonNode.Parse(File.ReadAllText(manifestFile))?["path"]?.GetValue<string>() == Path.GetFullPath(host.HostExecutablePath);
        }
        catch (JsonException)
        {
            return false;
        }
    }
}

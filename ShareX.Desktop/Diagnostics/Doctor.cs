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
using System.Text;

namespace ShareX.Desktop.Diagnostics;

/// <summary>Reports what works in this session and, for each thing that does not, what to install.</summary>
public static class Doctor
{
    public static string Report(IPlatformServices platform)
    {
        StringBuilder text = new StringBuilder();
        text.AppendLine(platform.Info.ToString());
        text.AppendLine();
        Line(text, "Screen capture", platform.ScreenCapture.Support);
        Line(text, "Screen recording", platform.ScreenRecording.Support);
        Line(text, "Clipboard", platform.Clipboard.Support);
        Line(text, "Notifications", platform.Notifications.Support);
        Line(text, "Global hotkeys", platform.Hotkeys.Support);
        Line(text, "Window list", platform.Windows.Support);
        Line(text, "Secret storage", platform.Credentials.Support);
        Line(text, "File thumbnails", platform.Thumbnails.Support);
        Line(text, "File manager menu", platform.ShellIntegration.Support);
        text.AppendLine();
        text.AppendLine("Config folder: " + platform.Paths.GetDefaultPersonalFolder("ShareX"));
        return text.ToString();
    }

    private static void Line(StringBuilder text, string name, FeatureSupport support)
    {
        text.Append(support.IsSupported ? "  ok       " : "  missing  ");
        text.Append(name.PadRight(20));

        if (!support.IsSupported)
        {
            text.Append(support.Reason);
        }

        text.AppendLine();
    }
}

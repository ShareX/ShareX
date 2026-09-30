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

namespace ShareX.Platform;

/// <summary>Reports whether a platform feature works in the current session, and why not when it does not.</summary>
public sealed record FeatureSupport(bool IsSupported, string? Reason = null)
{
    public static FeatureSupport Supported { get; } = new FeatureSupport(true);

    public static FeatureSupport NotSupported(string reason) => new FeatureSupport(false, reason);

    /// <summary>The feature works once the user installs the named external tool or package.</summary>
    public static FeatureSupport RequiresTool(string toolDescription) => new FeatureSupport(false, $"Install {toolDescription} to enable this feature.");

    public override string ToString() => IsSupported ? "Supported" : $"Not supported: {Reason}";
}

public enum PermissionState
{
    /// <summary>The platform has no permission gate for this feature.</summary>
    NotRequired,
    Granted,
    Denied,
    /// <summary>The user has not been asked yet, or the state cannot be queried without prompting.</summary>
    Unknown
}

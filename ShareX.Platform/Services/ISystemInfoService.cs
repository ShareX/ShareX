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

/// <summary>Facts about the operating system and the signed in user that ShareX shows in diagnostics or uses to adapt its behaviour.</summary>
public interface ISystemInfoService
{
    /// <summary>For example "Windows 11 Pro", "Omarchy" or "macOS 15.1".</summary>
    string OperatingSystemName { get; }

    /// <summary>True when ShareX runs elevated: an administrator token on Windows, root elsewhere.</summary>
    bool IsElevated { get; }

    /// <summary>True when the user may elevate: the Administrators group on Windows, admin, wheel or sudo elsewhere.</summary>
    bool IsAdministratorGroupMember { get; }

    /// <summary>Windows tablet mode. Always false elsewhere.</summary>
    bool IsTabletMode { get; }

    /// <summary>
    /// The hardware GPU for machine learning (the background remover): the one with the most dedicated memory, numbered as DirectML
    /// numbers adapters. Null where ShareX runs models on the CPU only (Linux and macOS).
    /// </summary>
    GpuAdapter? GetPreferredGpu();
}

/// <param name="Index">The adapter index DirectML expects.</param>
public sealed record GpuAdapter(int Index, string Name);

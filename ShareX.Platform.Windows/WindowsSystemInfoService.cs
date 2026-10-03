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

using Microsoft.Win32;
using System;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Principal;
using Vortice.DXGI;

namespace ShareX.Platform.Windows;

[SupportedOSPlatform("windows")]
public sealed class WindowsSystemInfoService : ISystemInfoService
{
    public string OperatingSystemName
    {
        get
        {
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            return key?.GetValue("ProductName") as string ?? Environment.OSVersion.VersionString;
        }
    }

    public bool IsElevated
    {
        get
        {
            try
            {
                using WindowsIdentity identity = WindowsIdentity.GetCurrent();
                return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public bool IsAdministratorGroupMember
    {
        get
        {
            try
            {
                using WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                string administrators = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Value;
                return principal.UserClaims.Any(claim => claim.Value.Contains(administrators));
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public bool IsTabletMode
    {
        get
        {
            try
            {
                return Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\ImmersiveShell", "TabletMode", 0) is int value && value > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public string GetSingleInstanceMutexName(string name) => name;

    /// <summary>The non-software DXGI adapter with the most dedicated video memory, as the background remover has always chosen.</summary>
    public GpuAdapter? GetPreferredGpu()
    {
        try
        {
            using IDXGIFactory1 factory = Vortice.DXGI.DXGI.CreateDXGIFactory1<IDXGIFactory1>();
            GpuAdapter? preferredAdapter = null;
            ulong largestDedicatedMemory = 0;

            for (uint index = 0; ; index++)
            {
                if (factory.EnumAdapters1(index, out IDXGIAdapter1 adapter).Failure)
                {
                    break;
                }

                using (adapter)
                {
                    AdapterDescription1 description = adapter.Description1;

                    if ((description.Flags & AdapterFlags.Software) != 0)
                    {
                        continue;
                    }

                    ulong dedicatedMemory = description.DedicatedVideoMemory;

                    if (preferredAdapter == null || dedicatedMemory > largestDedicatedMemory)
                    {
                        preferredAdapter = new GpuAdapter((int)index, description.Description);
                        largestDedicatedMemory = dedicatedMemory;
                    }
                }
            }

            return preferredAdapter ?? new GpuAdapter(0, "GPU 0");
        }
        catch
        {
            return new GpuAdapter(0, "GPU 0");
        }
    }
}

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

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ShareX.Platform.Windows;

/// <summary>Sounds with PlaySound, the API System.Media.SoundPlayer used.</summary>
[SupportedOSPlatform("windows")]
public sealed partial class WindowsSoundService : ISoundService
{
    private const uint SND_SYNC = 0x0000;
    private const uint SND_NODEFAULT = 0x0002;
    private const uint SND_MEMORY = 0x0004;
    private const uint SND_FILENAME = 0x00020000;

    public FeatureSupport Support => FeatureSupport.Supported;

    public unsafe void Play(byte[] wav)
    {
        if (wav.Length == 0)
        {
            return;
        }

        fixed (byte* data = wav)
        {
            PlaySound((IntPtr)data, IntPtr.Zero, SND_MEMORY | SND_SYNC | SND_NODEFAULT);
        }
    }

    public void PlayFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            PlaySoundFile(filePath, IntPtr.Zero, SND_FILENAME | SND_SYNC | SND_NODEFAULT);
        }
    }

    [LibraryImport("winmm.dll", EntryPoint = "PlaySoundW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool PlaySound(IntPtr sound, IntPtr module, uint flags);

    [LibraryImport("winmm.dll", EntryPoint = "PlaySoundW", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool PlaySoundFile(string sound, IntPtr module, uint flags);
}

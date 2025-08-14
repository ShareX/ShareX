#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib
{
    [ComImport, Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")]
    public class WshShell
    {
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), Guid("F935DC21-1CF0-11D0-ADB9-00C04FD58A0B")]
    public interface IWshShell
    {
        [DispId(0x3ea)]
        IWshShortcut CreateShortcut(string pathLink);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), Guid("F935DC23-1CF0-11D0-ADB9-00C04FD58A0B")]
    public interface IWshShortcut
    {
        [DispId(0)]
        string FullName { get; }

        [DispId(0x3e8)]
        string Arguments { get; set; }

        [DispId(0x3e9)]
        string Description { get; set; }

        [DispId(0x3ea)]
        string Hotkey { get; set; }

        [DispId(0x3eb)]
        string IconLocation { get; set; }

        [DispId(0x3ec)]
        string RelativePath { set; }

        [DispId(0x3ed)]
        string TargetPath { get; set; }

        [DispId(0x3ee)]
        int WindowStyle { get; set; }

        [DispId(0x3ef)]
        string WorkingDirectory { get; set; }

        [DispId(0x7d0)]
        void Load([In] string pathLink);

        [DispId(0x7d1)]
        void Save();
    }
}
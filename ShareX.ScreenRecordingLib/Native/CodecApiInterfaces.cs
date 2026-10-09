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

// SPDX-License-Identifier: GPL-3.0-or-later
using System.Runtime.InteropServices;

namespace ShareX.ScreenRecordingLib.Native;

// Vortice.MediaFoundation does not currently expose ICodecAPI.
[Guid("901db4c7-31ce-41a2-85dc-8fa0bf41b8da")]
internal unsafe struct ICodecAPI
{
    public void** Vtable;
    public HRESULT IsSupported(Guid* key)
    {
        fixed (ICodecAPI* self = &this)
            return ((delegate* unmanaged[Stdcall]<ICodecAPI*, Guid*, HRESULT>)Vtable[3])(self, key);
    }
    public HRESULT SetValue(Guid* key, VARIANT* value)
    {
        fixed (ICodecAPI* self = &this)
            return ((delegate* unmanaged[Stdcall]<ICodecAPI*, Guid*, VARIANT*, HRESULT>)Vtable[9])(self, key, value);
    }
}

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
using SharpGen.Runtime;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Vortice.MediaFoundation;

namespace ShareX.ScreenRecordingLib.Native;

/// <summary>Media Foundation notifies the pool after releasing the tracked GPU sample.</summary>
internal sealed class SampleReleaseCallback : ComObject, IMFAsyncCallback
{
    private readonly NativeCallback callback;

    public SampleReleaseCallback(Action released) : this(new NativeCallback(released)) { }

    private SampleReleaseCallback(NativeCallback callback)
        : base(Marshal.GetComInterfaceForObject(callback, typeof(INativeAsyncCallback)))
    {
        this.callback = callback;
    }

    public Result GetParameters(out AsyncCallbackFlags flags, out int queue)
    {
        flags = AsyncCallbackFlags.None;
        queue = 0;
        return new(unchecked((int)0x80004001)); // E_NOTIMPL: use the default MF work queue.
    }

    public Result Invoke(IMFAsyncResult result) => new(callback.Invoke(result.NativePointer));

    // A CLR COM callable wrapper roots the callback until its last native reference is released.
    // SharpGen's CallbackBase uses a weak handle, which is insufficient for queued callbacks
    // surviving pool disposal on a failed recording.
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    private sealed class NativeCallback(Action released) : INativeAsyncCallback
    {
        public int GetParameters(out uint flags, out uint queue)
        {
            flags = queue = 0;
            return unchecked((int)0x80004001);
        }

        public int Invoke(nint result)
        {
            try { released(); return 0; }
            catch (Exception ex) { return Marshal.GetHRForException(ex); }
        }
    }
}

[ComVisible(true)]
[Guid("a27003cf-2354-4f2a-8d6a-ab7cff15437e")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[EditorBrowsable(EditorBrowsableState.Never)]
// CLR COM marshaling requires a public interface; this is not part of the recorder's managed API.
public interface INativeAsyncCallback
{
    [PreserveSig] int GetParameters(out uint flags, out uint queue);
    [PreserveSig] int Invoke(nint result);
}

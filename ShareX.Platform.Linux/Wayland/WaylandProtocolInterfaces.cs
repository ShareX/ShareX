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

#nullable enable

using System;
using System.Runtime.InteropServices;

namespace ShareX.Platform.Linux.Wayland;

using static WaylandNative;

/// <summary>
/// Unmanaged <c>wl_interface</c> descriptors for core Wayland objects (exported by <c>libwayland-client</c>) and the
/// extension protocols used for multi-monitor overlay surfaces (<c>zwlr_layer_shell_v1</c>, <c>zxdg_output_v1</c>,
/// <c>wp_viewporter</c>, <c>wp_cursor_shape_v1</c>). Allocated once for the process lifetime.
/// </summary>
internal static unsafe class WaylandProtocolInterfaces
{
    private static readonly Lazy<InterfaceTable> Table = new(BuildTable);

    public static InterfaceTable All => Table.Value;

    internal sealed class InterfaceTable
    {
        public WlInterface* WlRegistry;
        public WlInterface* WlCallback;
        public WlInterface* WlCompositor;
        public WlInterface* WlShm;
        public WlInterface* WlShmPool;
        public WlInterface* WlBuffer;
        public WlInterface* WlSurface;
        public WlInterface* WlRegion;
        public WlInterface* WlSeat;
        public WlInterface* WlPointer;
        public WlInterface* WlKeyboard;
        public WlInterface* WlOutput;

        public WlInterface* ZwlrLayerShellV1;
        public WlInterface* ZwlrLayerSurfaceV1;
        public WlInterface* ZXdgOutputManagerV1;
        public WlInterface* ZXdgOutputV1;
        public WlInterface* WpViewporter;
        public WlInterface* WpViewport;
        public WlInterface* WpCursorShapeManagerV1;
        public WlInterface* WpCursorShapeDeviceV1;
    }

    private static InterfaceTable BuildTable()
    {
        InterfaceTable t = new()
        {
            WlRegistry = (WlInterface*)GetWaylandExport("wl_registry_interface"),
            WlCallback = (WlInterface*)GetWaylandExport("wl_callback_interface"),
            WlCompositor = (WlInterface*)GetWaylandExport("wl_compositor_interface"),
            WlShm = (WlInterface*)GetWaylandExport("wl_shm_interface"),
            WlShmPool = (WlInterface*)GetWaylandExport("wl_shm_pool_interface"),
            WlBuffer = (WlInterface*)GetWaylandExport("wl_buffer_interface"),
            WlSurface = (WlInterface*)GetWaylandExport("wl_surface_interface"),
            WlRegion = (WlInterface*)GetWaylandExport("wl_region_interface"),
            WlSeat = (WlInterface*)GetWaylandExport("wl_seat_interface"),
            WlPointer = (WlInterface*)GetWaylandExport("wl_pointer_interface"),
            WlKeyboard = (WlInterface*)GetWaylandExport("wl_keyboard_interface"),
            WlOutput = (WlInterface*)GetWaylandExport("wl_output_interface"),

            ZwlrLayerShellV1 = AllocInterface(),
            ZwlrLayerSurfaceV1 = AllocInterface(),
            ZXdgOutputManagerV1 = AllocInterface(),
            ZXdgOutputV1 = AllocInterface(),
            WpViewporter = AllocInterface(),
            WpViewport = AllocInterface(),
            WpCursorShapeManagerV1 = AllocInterface(),
            WpCursorShapeDeviceV1 = AllocInterface()
        };

        InitInterface(
            t.ZwlrLayerShellV1,
            "zwlr_layer_shell_v1",
            4,
            [
                ("get_layer_surface", "no?ous", [t.ZwlrLayerSurfaceV1, t.WlSurface, t.WlOutput, null, null]),
                ("destroy", "3", [])
            ],
            []);

        InitInterface(
            t.ZwlrLayerSurfaceV1,
            "zwlr_layer_surface_v1",
            4,
            [
                ("set_size", "uu", [null, null]),
                ("set_anchor", "u", [null]),
                ("set_exclusive_zone", "i", [null]),
                ("set_margin", "iiii", [null, null, null, null]),
                ("set_keyboard_interactivity", "u", [null]),
                ("get_popup", "o", [null]),
                ("ack_configure", "u", [null]),
                ("destroy", "", []),
                ("set_layer", "2u", [null])
            ],
            [
                ("configure", "uuu", [null, null, null]),
                ("closed", "", [])
            ]);

        InitInterface(
            t.ZXdgOutputManagerV1,
            "zxdg_output_manager_v1",
            3,
            [
                ("destroy", "", []),
                ("get_xdg_output", "no", [t.ZXdgOutputV1, t.WlOutput])
            ],
            []);

        InitInterface(
            t.ZXdgOutputV1,
            "zxdg_output_v1",
            3,
            [
                ("destroy", "", [])
            ],
            [
                ("logical_position", "ii", [null, null]),
                ("logical_size", "ii", [null, null]),
                ("done", "", []),
                ("name", "2s", [null]),
                ("description", "2s", [null])
            ]);

        InitInterface(
            t.WpViewporter,
            "wp_viewporter",
            1,
            [
                ("destroy", "", []),
                ("get_viewport", "no", [t.WpViewport, t.WlSurface])
            ],
            []);

        InitInterface(
            t.WpViewport,
            "wp_viewport",
            1,
            [
                ("destroy", "", []),
                ("set_source", "ffff", [null, null, null, null]),
                ("set_destination", "ii", [null, null])
            ],
            []);

        InitInterface(
            t.WpCursorShapeManagerV1,
            "wp_cursor_shape_manager_v1",
            1,
            [
                ("destroy", "", []),
                ("get_pointer", "no", [t.WpCursorShapeDeviceV1, t.WlPointer]),
                ("get_tablet_tool_v2", "no", [t.WpCursorShapeDeviceV1, null])
            ],
            []);

        InitInterface(
            t.WpCursorShapeDeviceV1,
            "wp_cursor_shape_device_v1",
            1,
            [
                ("destroy", "", []),
                ("set_shape", "uu", [null, null])
            ],
            []);

        return t;
    }

    private static WlInterface* AllocInterface() =>
        (WlInterface*)NativeMemory.AllocZeroed((nuint)sizeof(WlInterface));

    private static void InitInterface(
        WlInterface* iface,
        string name,
        int version,
        (string Name, string Signature, WlInterface*[] Types)[] methods,
        (string Name, string Signature, WlInterface*[] Types)[] events)
    {
        iface->Name = Marshal.StringToCoTaskMemUTF8(name);
        iface->Version = version;
        iface->MethodCount = methods.Length;
        iface->Methods = AllocMessages(methods);
        iface->EventCount = events.Length;
        iface->Events = AllocMessages(events);
    }

    private static WlMessage* AllocMessages((string Name, string Signature, WlInterface*[] Types)[] descriptors)
    {
        if (descriptors.Length == 0)
        {
            return null;
        }

        WlMessage* messages = (WlMessage*)NativeMemory.AllocZeroed((nuint)descriptors.Length, (nuint)sizeof(WlMessage));

        for (int i = 0; i < descriptors.Length; i++)
        {
            messages[i].Name = Marshal.StringToCoTaskMemUTF8(descriptors[i].Name);
            messages[i].Signature = Marshal.StringToCoTaskMemUTF8(descriptors[i].Signature);
            WlInterface*[] types = descriptors[i].Types;

            if (types.Length > 0)
            {
                WlInterface** typeArray = (WlInterface**)NativeMemory.AllocZeroed((nuint)types.Length, (nuint)sizeof(IntPtr));

                for (int j = 0; j < types.Length; j++)
                {
                    typeArray[j] = types[j];
                }

                messages[i].Types = typeArray;
            }
        }

        return messages;
    }
}

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
using System.Runtime.InteropServices;

namespace ShareX.Platform.Linux.Native;

/// <summary>
/// The parts of GLib, GObject and GStreamer 1.x that video playback and filmstrips use. GStreamer is the distribution's own,
/// loaded at run time like FFmpeg: ShareX works without it and never bundles it.
/// </summary>
internal static unsafe partial class GStreamer
{
    private const string LibGlib = "libglib-2.0.so.0";
    private const string LibGobject = "libgobject-2.0.so.0";
    private const string LibGst = "libgstreamer-1.0.so.0";
    private const string LibGstVideo = "libgstvideo-1.0.so.0";
    private const string LibGstApp = "libgstapp-1.0.so.0";

    public const int StateNull = 1, StatePaused = 3, StatePlaying = 4;
    public const int StateChangeFailure = 0, StateChangeSuccess = 1, StateChangeAsync = 2, StateChangeNoPreroll = 3;
    public const int FormatTime = 3, FormatBuffers = 4;
    public const int SeekFlagFlush = 1 << 0, SeekFlagAccurate = 1 << 1;
    public const int MessageEos = 1 << 0, MessageError = 1 << 1, MessageAsyncDone = 1 << 21;
    public const ulong ClockTimeNone = ulong.MaxValue;
    public const int MapRead = 1;

    // GType values of fundamental types: G_TYPE_MAKE_FUNDAMENTAL(n) is n << 2.
    public const nuint TypeBoolean = 5 << 2, TypeInt = 6 << 2, TypeDouble = 15 << 2, TypeString = 16 << 2, TypeObject = 20 << 2;

    /// <summary>GValue: a GType and two 8 byte data words.</summary>
    [StructLayout(LayoutKind.Sequential, Size = 24)]
    public struct GValue
    {
        public nuint Type;
        public ulong Data0;
        public ulong Data1;
    }

    /// <summary>GstMapInfo; only the data pointer and size are read.</summary>
    [StructLayout(LayoutKind.Explicit, Size = 104)]
    public struct GstMapInfo
    {
        [FieldOffset(16)] public byte* Data;
        [FieldOffset(24)] public nuint Size;
    }

    /// <summary>GError: domain, code and the message.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GError
    {
        public uint Domain;
        public int Code;
        public IntPtr Message;
    }

    private static readonly Lazy<bool> available = new Lazy<bool>(() =>
    {
        foreach (string library in new[] { LibGlib, LibGobject, LibGst, LibGstVideo, LibGstApp })
        {
            if (!NativeLibrary.TryLoad(library, out _))
            {
                return false;
            }
        }

        try
        {
            // The player embeds in an X11 window (ShareX runs under X11 or XWayland). Without this, GStreamer GL picks Wayland when
            // WAYLAND_DISPLAY is set and crashes drawing into the X11 window. GStreamer reads the native environment.
            if (Environment.GetEnvironmentVariable("GST_GL_WINDOW") == null) setenv("GST_GL_WINDOW", "x11", 0);
            return gst_init_check(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        }
        catch (Exception e) when (e is DllNotFoundException or EntryPointNotFoundException)
        {
            return false;
        }
    });

    /// <summary>True when GStreamer is installed and initialized.</summary>
    public static bool IsAvailable => available.Value;

    public static bool HasElement(string factory)
    {
        IntPtr found = gst_element_factory_find(factory);
        if (found == IntPtr.Zero) return false;
        gst_object_unref(found);
        return true;
    }

    public static string? TakeString(IntPtr text)
    {
        if (text == IntPtr.Zero) return null;
        string? value = Marshal.PtrToStringUTF8(text);
        g_free(text);
        return value;
    }

    public static void SetProperty(IntPtr target, string name, double value)
    {
        GValue gvalue = default;
        g_value_init(&gvalue, TypeDouble);
        g_value_set_double(&gvalue, value);
        g_object_set_property(target, name, &gvalue);
        g_value_unset(&gvalue);
    }

    public static void SetProperty(IntPtr target, string name, bool value)
    {
        GValue gvalue = default;
        g_value_init(&gvalue, TypeBoolean);
        g_value_set_boolean(&gvalue, value ? 1 : 0);
        g_object_set_property(target, name, &gvalue);
        g_value_unset(&gvalue);
    }

    public static void SetProperty(IntPtr target, string name, string value)
    {
        GValue gvalue = default;
        g_value_init(&gvalue, TypeString);
        g_value_set_string(&gvalue, value);
        g_object_set_property(target, name, &gvalue);
        g_value_unset(&gvalue);
    }

    public static void SetObjectProperty(IntPtr target, string name, IntPtr value)
    {
        GValue gvalue = default;
        g_value_init(&gvalue, TypeObject);
        g_value_set_object(&gvalue, value);
        g_object_set_property(target, name, &gvalue);
        g_value_unset(&gvalue);
    }

    /// <summary>The message of an error message, freeing what GStreamer allocated.</summary>
    public static string ParseError(IntPtr message)
    {
        gst_message_parse_error(message, out IntPtr error, out IntPtr debug);
        string text = error != IntPtr.Zero ? Marshal.PtrToStringUTF8(((GError*)error)->Message) ?? "GStreamer error" : "GStreamer error";
        if (error != IntPtr.Zero) g_error_free(error);
        g_free(debug);
        return text;
    }

    /// <summary>Width and height from the first structure of caps, and the pixel aspect ratio (1 when absent).</summary>
    public static bool TryGetVideoSize(IntPtr caps, out int width, out int height, out double pixelAspect)
    {
        width = height = 0;
        pixelAspect = 1;
        if (caps == IntPtr.Zero) return false;
        IntPtr structure = gst_caps_get_structure(caps, 0);
        if (structure == IntPtr.Zero || !gst_structure_get_int(structure, "width", out width) || !gst_structure_get_int(structure, "height", out height))
        {
            return false;
        }

        if (gst_structure_get_fraction(structure, "pixel-aspect-ratio", out int numerator, out int denominator) && numerator > 0 && denominator > 0)
        {
            pixelAspect = (double)numerator / denominator;
        }

        return width > 0 && height > 0;
    }

    [LibraryImport("libc", StringMarshalling = StringMarshalling.Utf8)]
    private static partial int setenv(string name, string value, int overwrite);

    [LibraryImport(LibGlib)]
    public static partial void g_free(IntPtr memory);

    [LibraryImport(LibGlib)]
    public static partial void g_error_free(IntPtr error);

    [LibraryImport(LibGobject)]
    public static partial GValue* g_value_init(GValue* value, nuint type);

    [LibraryImport(LibGobject)]
    public static partial void g_value_unset(GValue* value);

    [LibraryImport(LibGobject)]
    public static partial void g_value_set_double(GValue* value, double data);

    [LibraryImport(LibGobject)]
    public static partial void g_value_set_boolean(GValue* value, int data);

    [LibraryImport(LibGobject, StringMarshalling = StringMarshalling.Utf8)]
    public static partial void g_value_set_string(GValue* value, string data);

    [LibraryImport(LibGobject)]
    public static partial void g_value_set_object(GValue* value, IntPtr data);

    [LibraryImport(LibGobject, StringMarshalling = StringMarshalling.Utf8)]
    public static partial void g_object_set_property(IntPtr target, string name, GValue* value);

    [LibraryImport(LibGst)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool gst_init_check(IntPtr argc, IntPtr argv, IntPtr error);

    [LibraryImport(LibGst, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr gst_element_factory_find(string name);

    [LibraryImport(LibGst, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr gst_element_factory_make(string factory, string? name);

    [LibraryImport(LibGst, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr gst_parse_launch(string description, out IntPtr error);

    [LibraryImport(LibGst, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr gst_filename_to_uri(string filename, IntPtr error);

    [LibraryImport(LibGst, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr gst_bin_get_by_name(IntPtr bin, string name);

    [LibraryImport(LibGst)]
    public static partial IntPtr gst_object_ref(IntPtr target);

    [LibraryImport(LibGst)]
    public static partial IntPtr gst_object_ref_sink(IntPtr target);

    [LibraryImport(LibGst)]
    public static partial void gst_object_unref(IntPtr target);

    [LibraryImport(LibGst)]
    public static partial void gst_mini_object_unref(IntPtr target);

    [LibraryImport(LibGst)]
    public static partial int gst_element_set_state(IntPtr element, int state);

    [LibraryImport(LibGst)]
    public static partial int gst_element_get_state(IntPtr element, out int state, out int pending, ulong timeout);

    [LibraryImport(LibGst)]
    public static partial IntPtr gst_element_get_bus(IntPtr element);

    /// <summary>The next message, of any type, or zero. (gst_bus_pop_filtered discards the messages it skips.)</summary>
    [LibraryImport(LibGst)]
    public static partial IntPtr gst_bus_pop(IntPtr bus);

    /// <summary>GST_MESSAGE_TYPE: the type follows the 64 byte GstMiniObject header, part of GStreamer 1.x's stable ABI.</summary>
    public static int MessageType(IntPtr message) => *(int*)((byte*)message + 64);

    [LibraryImport(LibGst)]
    public static partial void gst_message_parse_error(IntPtr message, out IntPtr error, out IntPtr debug);

    [LibraryImport(LibGst)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool gst_element_query_position(IntPtr element, int format, out long position);

    [LibraryImport(LibGst)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool gst_element_query_duration(IntPtr element, int format, out long duration);

    [LibraryImport(LibGst)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool gst_element_seek_simple(IntPtr element, int format, int flags, long position);

    [LibraryImport(LibGst)]
    public static partial IntPtr gst_event_new_step(int format, ulong amount, double rate, [MarshalAs(UnmanagedType.Bool)] bool flush,
        [MarshalAs(UnmanagedType.Bool)] bool intermediate);

    [LibraryImport(LibGst)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool gst_element_send_event(IntPtr element, IntPtr gstEvent);

    [LibraryImport(LibGst, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr gst_element_get_static_pad(IntPtr element, string name);

    [LibraryImport(LibGst)]
    public static partial IntPtr gst_pad_get_current_caps(IntPtr pad);

    [LibraryImport(LibGst)]
    public static partial IntPtr gst_caps_get_structure(IntPtr caps, uint index);

    [LibraryImport(LibGst, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool gst_structure_get_int(IntPtr structure, string field, out int value);

    [LibraryImport(LibGst, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool gst_structure_get_fraction(IntPtr structure, string field, out int numerator, out int denominator);

    [LibraryImport(LibGst)]
    public static partial IntPtr gst_sample_get_buffer(IntPtr sample);

    [LibraryImport(LibGst)]
    public static partial IntPtr gst_sample_get_caps(IntPtr sample);

    [LibraryImport(LibGst)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool gst_buffer_map(IntPtr buffer, GstMapInfo* info, int flags);

    [LibraryImport(LibGst)]
    public static partial void gst_buffer_unmap(IntPtr buffer, GstMapInfo* info);

    [LibraryImport(LibGstVideo)]
    public static partial void gst_video_overlay_set_window_handle(IntPtr overlay, nuint handle);

    [LibraryImport(LibGstVideo)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool gst_video_overlay_set_render_rectangle(IntPtr overlay, int x, int y, int width, int height);

    [LibraryImport(LibGstVideo)]
    public static partial void gst_video_overlay_expose(IntPtr overlay);

    [LibraryImport(LibGstApp)]
    public static partial IntPtr gst_app_sink_try_pull_preroll(IntPtr appsink, ulong timeout);
}

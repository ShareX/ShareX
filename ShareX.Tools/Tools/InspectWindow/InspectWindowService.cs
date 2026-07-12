#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

using ShareX.HelpersLib;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;

namespace ShareX.Tools;

public sealed class InspectWindowListItem : IDisposable
{
    public IntPtr Handle { get; }
    public string Title { get; }
    public string ProcessName { get; }
    public AvaloniaBitmap? Icon { get; }
    public bool HasIcon => Icon != null;
    public string DisplayName => string.IsNullOrWhiteSpace(ProcessName) ? Title : $"{Title}  —  {ProcessName}";

    public InspectWindowListItem(IntPtr handle, string title, string processName, AvaloniaBitmap? icon)
    {
        Handle = handle;
        Title = title;
        ProcessName = processName;
        Icon = icon;
    }

    public void Dispose()
    {
        Icon?.Dispose();
    }

    public override string ToString() => Title;
}

public static class InspectWindowService
{
    private const uint GA_ROOT = 2;
    private static readonly string[] IgnoredClasses = ["Progman", "Button"];

    public static IntPtr GetWindowAtPoint(int x, int y, bool topLevel)
    {
        IntPtr handle = WindowFromPoint(new POINT(x, y));
        return topLevel && handle != IntPtr.Zero ? GetAncestor(handle, GA_ROOT) : handle;
    }

    public static IReadOnlyList<InspectWindowListItem> GetVisibleWindows(IntPtr ignoredHandle)
    {
        List<InspectWindowListItem> windows = [];

        NativeMethods.EnumWindows((handle, _) =>
        {
            if (handle == ignoredHandle)
            {
                return true;
            }

            try
            {
                WindowInfo window = new(handle);
                string title = window.Text;
                string className = window.ClassName;
                System.Drawing.Rectangle rectangle = window.Rectangle;

                if (window.IsVisible && !window.IsCloaked && !string.IsNullOrWhiteSpace(title) &&
                    rectangle.Width > 0 && rectangle.Height > 0 &&
                    !IgnoredClasses.Contains(className, StringComparer.OrdinalIgnoreCase))
                {
                    windows.Add(new InspectWindowListItem(
                        handle,
                        title,
                        TryGet(() => window.ProcessName),
                        GetWindowIcon(window)));
                }
            }
            catch (Exception ex)
            {
                ToolsDiagnostics.ReportWarning(nameof(InspectWindowService), "Failed to inspect a window while building the window list.", ex);
            }

            return true;
        }, IntPtr.Zero);

        return windows.OrderBy(x => x.Title, StringComparer.CurrentCultureIgnoreCase).ToArray();
    }

    private static string TryGet(Func<string> valueFactory)
    {
        try
        {
            return valueFactory() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    public static AvaloniaBitmap? GetWindowIcon(IntPtr handle)
    {
        return handle == IntPtr.Zero ? null : GetWindowIcon(new WindowInfo(handle));
    }

    private static AvaloniaBitmap? GetWindowIcon(WindowInfo window)
    {
        try
        {
            using System.Drawing.Icon? icon = window.Icon;
            if (icon == null || icon.Width <= 0 || icon.Height <= 0)
            {
                return null;
            }

            using System.Drawing.Bitmap bitmap = icon.ToBitmap();
            using MemoryStream stream = new();
            bitmap.Save(stream, ImageFormat.Png);
            stream.Position = 0;
            return new AvaloniaBitmap(stream);
        }
        catch
        {
            return null;
        }
    }

    [DllImport("user32.dll")]
    private static extern IntPtr WindowFromPoint(POINT point);

    [DllImport("user32.dll")]
    private static extern IntPtr GetAncestor(IntPtr handle, uint flags);
}

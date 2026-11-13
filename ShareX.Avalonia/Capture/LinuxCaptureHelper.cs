#region License Information (GPL v3)
/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/
#endregion License Information (GPL v3)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using SkiaSharp;

namespace ShareX.ScreenCaptureLib;

public static class LinuxCaptureHelper
{
    private static readonly object _syncLock = new();

    public static SKBitmap? CaptureFullscreen()
    {
        lock (_syncLock)
        {
            string? filePath = CaptureViaPortal(interactive: false);
            return LoadBitmapWithRetry(filePath);
        }
    }

    public static SKBitmap? CaptureInteractiveWindow()
    {
        lock (_syncLock)
        {
            string? filePath = CaptureViaPortal(interactive: true);
            return LoadBitmapWithRetry(filePath);
        }
    }

    private static SKBitmap? LoadBitmapWithRetry(string? filePath, bool deleteSourceFile = true)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            return null;
        }

        SKBitmap? result = null;

        for (int i = 0; i < 25; i++)
        {
            try
            {
                FileInfo fi = new FileInfo(filePath);
                if (fi.Exists && fi.Length > 0)
                {
                    using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        SKBitmap? bmp = SKBitmap.Decode(stream);
                        if (bmp != null)
                        {
                            result = bmp.Copy();
                            bmp.Dispose();
                            break;
                        }
                    }
                }
            }
            catch (IOException)
            {
                // File locked during portal write
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Waiting for portal screenshot file to decode: {ex.Message}");
            }

            Thread.Sleep(100);
        }

        if (deleteSourceFile && result != null && !string.IsNullOrEmpty(filePath))
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to delete temporary portal screenshot file: {ex.Message}");
            }
        }

        return result;
    }

    public static SKBitmap? CaptureRectangle(Rectangle rect)
    {
        SKBitmap? full = CaptureFullscreen();
        if (full == null) return null;
        if (rect.IsEmpty || rect.Width <= 0 || rect.Height <= 0) return full;

        Rectangle fullBounds = new Rectangle(0, 0, full.Width, full.Height);
        Rectangle intersect = Rectangle.Intersect(fullBounds, rect);
        if (intersect.Width <= 0 || intersect.Height <= 0) return full;

        try
        {
            SKBitmap cropped = new SKBitmap();
            SKRectI subset = new SKRectI(intersect.Left, intersect.Top, intersect.Right, intersect.Bottom);
            if (full.ExtractSubset(cropped, subset))
            {
                full.Dispose();
                return cropped;
            }
            cropped.Dispose();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to crop rectangle: {ex}");
        }

        return full;
    }

    private static IEnumerable<string> GetPicturesDirectories()
    {
        HashSet<string> dirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            string myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (!string.IsNullOrEmpty(myPictures) && Directory.Exists(myPictures))
            {
                dirs.Add(myPictures);
            }
        }
        catch { }

        try
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!string.IsNullOrEmpty(userProfile))
            {
                string[] candidates = new[]
                {
                    Path.Combine(userProfile, "Imagens"),
                    Path.Combine(userProfile, "Pictures"),
                    Path.Combine(userProfile, "Imagens", "Capturas de tela"),
                    Path.Combine(userProfile, "Pictures", "Screenshots")
                };

                foreach (string candidate in candidates)
                {
                    if (Directory.Exists(candidate))
                    {
                        dirs.Add(candidate);
                    }
                }
            }
        }
        catch { }

        try
        {
            using Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo("xdg-user-dir", "PICTURES")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            proc.Start();
            string? outDir = proc.StandardOutput.ReadToEnd()?.Trim();
            proc.WaitForExit(1000);
            if (!string.IsNullOrEmpty(outDir) && Directory.Exists(outDir))
            {
                dirs.Add(outDir);
            }
        }
        catch { }

        return dirs;
    }

    private static string? CaptureViaPortal(bool interactive = false)
    {
        List<FileSystemWatcher> watchers = new();
        try
        {
            DateTime startTime = DateTime.UtcNow;
            string? foundPath = null;
            using AutoResetEvent waitHandle = new AutoResetEvent(false);

            foreach (string dir in GetPicturesDirectories())
            {
                try
                {
                    FileSystemWatcher fsw = new FileSystemWatcher(dir, "*.png")
                    {
                        IncludeSubdirectories = true,
                        EnableRaisingEvents = true
                    };
                    fsw.Created += (s, e) =>
                    {
                        foundPath = e.FullPath;
                        waitHandle.Set();
                    };
                    fsw.Changed += (s, e) =>
                    {
                        foundPath = e.FullPath;
                        waitHandle.Set();
                    };
                    watchers.Add(fsw);
                }
                catch { }
            }

            using (Process caller = new Process())
            {
                string interactiveArg = interactive ? "<true>" : "<false>";
                caller.StartInfo = new ProcessStartInfo("gdbus", $"call --session --dest org.freedesktop.portal.Desktop --object-path /org/freedesktop/portal/desktop --method org.freedesktop.portal.Screenshot.Screenshot \"\" \"{{'interactive': {interactiveArg}}}\"")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                caller.Start();
                caller.WaitForExit(5000);
            }

            int timeoutMs = interactive ? 10000 : 5000;
            waitHandle.WaitOne(timeoutMs);

            foreach (var w in watchers)
            {
                try { w.EnableRaisingEvents = false; w.Dispose(); } catch { }
            }
            watchers.Clear();

            if (!string.IsNullOrEmpty(foundPath) && File.Exists(foundPath))
            {
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        using var fs = File.OpenRead(foundPath);
                        if (fs.Length > 0) return foundPath;
                    }
                    catch { }
                    Thread.Sleep(50);
                }
                return foundPath;
            }

            foreach (string dir in GetPicturesDirectories())
            {
                var latestFile = Directory.GetFiles(dir, "*.png", SearchOption.AllDirectories)
                    .Select(f => new FileInfo(f))
                    .Where(fi => fi.LastWriteTimeUtc >= startTime.AddSeconds(-2))
                    .OrderByDescending(fi => fi.LastWriteTimeUtc)
                    .FirstOrDefault();

                if (latestFile != null && latestFile.Length > 0)
                {
                    return latestFile.FullName;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Portal capture exception: {ex}");
        }
        finally
        {
            foreach (var w in watchers)
            {
                try { w.EnableRaisingEvents = false; w.Dispose(); } catch { }
            }
            watchers.Clear();
        }

        return null;
    }
}

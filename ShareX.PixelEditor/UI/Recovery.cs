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

using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace ShareX.PixelEditor.UI;

/// <summary>A sprite left behind by an editor that did not close normally.</summary>
internal sealed record RecoveredSprite(string ProjectPath, string? OriginalPath, string Title, DateTime SavedAt);

/// <summary>
/// Keeps copies of sprites with unsaved changes so they can be restored after a crash. Each editor window writes into its own
/// session folder, which holds a lock file while the window is open; a session folder without a held lock belongs to an editor
/// that ended without cleaning up.
/// </summary>
internal sealed class Recovery : IDisposable
{
    private const string LockName = "session.lock";
    private const string ProjectSuffix = ".sxpixel";
    private const string InfoSuffix = ".json";

    private sealed class Info
    {
        public string? OriginalPath { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime SavedAt { get; set; }
    }

    private sealed class Entry
    {
        public required string Name { get; init; }
        public object? State { get; set; }
    }

    private readonly string _root;
    private readonly ConditionalWeakTable<Sprite, Entry> _entries = [];
    private string? _session;
    private FileStream? _lock;
    private int _next;

    public Recovery(string? settingsFilePath)
    {
        string? folder = string.IsNullOrEmpty(settingsFilePath) ? null : Path.GetDirectoryName(settingsFilePath);
        _root = Path.Combine(folder ?? Path.Combine(Path.GetTempPath(), "ShareX"), "PixelEditorRecovery");
    }

    /// <summary>Writes a copy of every sprite whose changes are not saved and changed since its last copy, and removes copies of saved sprites.</summary>
    public void Backup(IEnumerable<Sprite> sprites)
    {
        foreach (Sprite sprite in sprites)
        {
            if (!sprite.IsDirty)
            {
                Remove(sprite);
                continue;
            }
            object? state = sprite.History.CurrentState;
            if (_entries.TryGetValue(sprite, out Entry? existing) && ReferenceEquals(existing.State, state)) continue;
            try
            {
                string session = EnsureSession();
                Entry entry = existing ?? new Entry { Name = (++_next).ToString("D3") };
                string project = Path.Combine(session, entry.Name + ProjectSuffix);
                string temp = project + ".tmp";
                SpriteFile.SaveProject(sprite, temp);
                File.Move(temp, project, true);
                Info info = new() { OriginalPath = sprite.FilePath, Title = sprite.Title, SavedAt = DateTime.Now };
                File.WriteAllText(Path.Combine(session, entry.Name + InfoSuffix), JsonConvert.SerializeObject(info, Formatting.Indented));
                entry.State = state;
                if (existing == null) _entries.Add(sprite, entry);
            }
            catch (Exception)
            {
                // A failed backup must never interrupt editing; the next one tries again.
            }
        }
    }

    /// <summary>Deletes the copy of a sprite that was saved or closed.</summary>
    public void Remove(Sprite sprite)
    {
        if (_session == null || !_entries.TryGetValue(sprite, out Entry? entry)) return;
        _entries.Remove(sprite);
        TryDelete(Path.Combine(_session, entry.Name + ProjectSuffix));
        TryDelete(Path.Combine(_session, entry.Name + InfoSuffix));
    }

    /// <summary>Finds sprites written by editors that are no longer running.</summary>
    public List<RecoveredSprite> FindAbandoned()
    {
        List<RecoveredSprite> found = [];
        foreach (string folder in AbandonedSessions())
        {
            foreach (string project in Directory.EnumerateFiles(folder, "*" + ProjectSuffix).Order())
            {
                try
                {
                    string infoPath = Path.ChangeExtension(project, InfoSuffix);
                    Info info = File.Exists(infoPath) ? JsonConvert.DeserializeObject<Info>(File.ReadAllText(infoPath)) ?? new Info() : new Info();
                    found.Add(new RecoveredSprite(project, info.OriginalPath, info.Title, info.SavedAt == default ? File.GetLastWriteTime(project) : info.SavedAt));
                }
                catch (Exception)
                {
                }
            }
        }
        return found;
    }

    /// <summary>Deletes the folders of editors that are no longer running.</summary>
    public void DiscardAbandoned()
    {
        foreach (string folder in AbandonedSessions()) TryDeleteFolder(folder);
    }

    /// <summary>Ends the session after a normal close: nothing needs recovering.</summary>
    public void Dispose()
    {
        _lock?.Dispose();
        _lock = null;
        if (_session != null) TryDeleteFolder(_session);
        _session = null;
    }

    private IEnumerable<string> AbandonedSessions()
    {
        if (!Directory.Exists(_root)) yield break;
        foreach (string folder in Directory.EnumerateDirectories(_root))
        {
            if (_session != null && string.Equals(Path.GetFullPath(folder), Path.GetFullPath(_session), StringComparison.OrdinalIgnoreCase)) continue;
            string lockPath = Path.Combine(folder, LockName);
            if (IsLocked(lockPath)) continue;
            // A folder that was just made may not hold its lock yet.
            if (!File.Exists(lockPath) && Directory.GetCreationTime(folder) > DateTime.Now.AddSeconds(-30)) continue;
            yield return folder;
        }
    }

    private static bool IsLocked(string path)
    {
        if (!File.Exists(path)) return false;
        try
        {
            using FileStream stream = new(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            return false;
        }
        catch (IOException)
        {
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return true;
        }
    }

    private string EnsureSession()
    {
        if (_session != null) return _session;
        string session = Path.Combine(_root, DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(session);
        // The lock is deleted by the system when the handle closes, so even a crash leaves the folder unlocked.
        _lock = new FileStream(Path.Combine(session, LockName), FileMode.Create, FileAccess.ReadWrite, FileShare.None, 1, FileOptions.DeleteOnClose);
        _session = session;
        return session;
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path)) File.Delete(path);
        }
        catch (Exception)
        {
        }
    }

    private static void TryDeleteFolder(string path)
    {
        try
        {
            if (Directory.Exists(path)) Directory.Delete(path, true);
        }
        catch (Exception)
        {
        }
    }
}

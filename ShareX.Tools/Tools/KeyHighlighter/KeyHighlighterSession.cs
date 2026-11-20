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

using System.Diagnostics;

namespace ShareX.Tools;

/// <summary>
/// Collects key presses for one recording and gives the image to draw on each frame. Start it when the recording
/// starts and dispose it when the recording ends. Nothing is shown on screen and no key is stored or logged.
/// </summary>
public sealed class KeyHighlighterSession : IDisposable
{
    private readonly KeyHighlighterSettings settings;
    private readonly KeyStrokeFormatter formatter;
    private readonly KeyHighlighterRenderer renderer;
    private readonly KeyHighlighterKeyboardHook hook;
    private readonly Lock sync = new();

    public KeyHighlighterSession(KeyHighlighterSettings settings)
    {
        this.settings = settings;
        formatter = new KeyStrokeFormatter(settings.ShowAllKeys, settings.DisplaySeconds);
        renderer = new KeyHighlighterRenderer(settings);
        hook = new KeyHighlighterKeyboardHook(OnKey);
    }

    private static long Now => Stopwatch.GetTimestamp() * 1000 / Stopwatch.Frequency;

    private void OnKey(int virtualKey, bool pressed, IntPtr data)
    {
        lock (sync)
        {
            if (!pressed)
            {
                formatter.KeyUp(virtualKey, Now);
                return;
            }
            string? text = null;
            bool password = false;
            bool altGr = (GetAsyncKeyState(0xA5) & 0x8000) != 0 && (GetAsyncKeyState(0x11) & 0x8000) != 0;
            if (altGr && KeyStrokeFormatter.IsTextKey(virtualKey))
            {
                password = settings.ShowAllKeys && KeyHighlighterKeyboardHook.IsPasswordFieldFocused();
                text = KeyHighlighterKeyboardHook.GetAltGrText(virtualKey, (GetAsyncKeyState(0x10) & 0x8000) != 0,
                    (GetKeyState(0x14) & 1) != 0);
            }
            else if (settings.ShowAllKeys && KeyStrokeFormatter.IsTextKey(virtualKey))
            {
                password = KeyHighlighterKeyboardHook.IsPasswordFieldFocused();
                if (!password)
                {
                    bool shift = (GetAsyncKeyState(0x10) & 0x8000) != 0;
                    bool capsLock = (GetKeyState(0x14) & 1) != 0;
                    text = KeyHighlighterKeyboardHook.GetTypedText(virtualKey, shift, capsLock);
                }
            }
            else if (KeyStrokeFormatter.IsTextKey(virtualKey) && virtualKey is >= 0xBA and <= 0xE2)
            {
                text = KeyHighlighterKeyboardHook.GetTypedText(virtualKey, false, false);
            }
            formatter.KeyDown(virtualKey, text, password, Now);
        }
    }

    /// <summary>The image for a frame of the given size, or null when no keys are shown. Safe to call from any thread.</summary>
    public KeyHighlighterImage? GetImage(int frameWidth, int frameHeight)
    {
        lock (sync)
        {
            return renderer.Render(formatter.GetItems(Now), frameWidth, frameHeight);
        }
    }

    public void Dispose()
    {
        hook.Dispose();
        lock (sync) renderer.Dispose();
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int key);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern short GetKeyState(int key);
}

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

using ShareX.ScreenRecordingLib;
using ShareX.Tools;
using System;
using System.Threading;

namespace ShareX
{
    /// <summary>Draws pressed keys into a recording. Keys are collected only between <see cref="Begin"/> and disposing its result.</summary>
    internal sealed class KeyHighlighterRecordingOverlay : IRecordingOverlay
    {
        private readonly KeyHighlighterSettings settings;
        private KeyHighlighterSession session;

        public KeyHighlighterRecordingOverlay(TaskSettingsCapture capture)
        {
            settings = new KeyHighlighterSettings(capture.ScreenRecordKeysShowAll, capture.ScreenRecordKeysPosition,
                capture.ScreenRecordKeysFontSize, capture.ScreenRecordKeysTextColor, capture.ScreenRecordKeysDuration);
        }

        public IDisposable Begin()
        {
            KeyHighlighterSession started = new KeyHighlighterSession(settings);
            Volatile.Write(ref session, started);
            return new Lease(() =>
            {
                Interlocked.CompareExchange(ref session, null, started);
                started.Dispose();
            });
        }

        public RecordingOverlayImage GetImage(int frameWidth, int frameHeight)
        {
            KeyHighlighterImage image = Volatile.Read(ref session)?.GetImage(frameWidth, frameHeight);
            return image == null ? null : new RecordingOverlayImage(image.Pixels, image.Width, image.Height, image.X, image.Y, image.Version);
        }

        private sealed class Lease(Action dispose) : IDisposable
        {
            private int disposed;

            public void Dispose()
            {
                if (Interlocked.Exchange(ref disposed, 1) == 0) dispose();
            }
        }
    }
}

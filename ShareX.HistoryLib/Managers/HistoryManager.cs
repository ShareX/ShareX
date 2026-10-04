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

using ShareX.HelpersLib;
using ShareX.HistoryLib.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessageBox = ShareX.AvaloniaUI.MessageBox;
using MessageBoxButtons = ShareX.AvaloniaUI.MessageBoxButtons;
using MessageBoxIcon = ShareX.AvaloniaUI.MessageBoxIcon;

namespace ShareX.HistoryLib
{
    public abstract class HistoryManager
    {
        public string FilePath { get; private set; }
        public string BackupFolder { get; set; }
        public bool CreateBackup { get; set; }
        public bool CreateWeeklyBackup { get; set; }

        public HistoryManager(string filePath)
        {
            FilePath = filePath;
        }

        public List<HistoryItem> GetHistoryItems()
        {
            try
            {
                return Load();
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);

                MessageBox.Show(Strings.HistoryManager_ErrorOccurredWhileReadingHistoryFile + " " + FilePath + "\r\n\r\n" + e,
                    "ShareX - " + Strings.HistoryManager_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return new List<HistoryItem>();
        }

        public async Task<List<HistoryItem>> GetHistoryItemsAsync()
        {
            return await Task.Run(GetHistoryItems);
        }

        public bool AppendHistoryItem(HistoryItem historyItem)
        {
            return AppendHistoryItems(new HistoryItem[] { historyItem });
        }

        private readonly object backgroundWritesLock = new object();
        private Task backgroundWrites = Task.CompletedTask;
        private bool backgroundWritesClosed;

        /// <summary>
        /// Saves the item on a background thread, after any item queued before it, so uploads finishing together are written in
        /// order and one at a time. <see cref="FlushBackgroundWrites"/> waits for them; closing the history does that first.
        /// </summary>
        public Task<bool> AppendHistoryItemInBackground(HistoryItem historyItem)
        {
            lock (backgroundWritesLock)
            {
                if (backgroundWritesClosed)
                {
                    DebugHelper.WriteLine("History is closing; the item was not saved: " + historyItem?.FileName);
                    return Task.FromResult(false);
                }

                Task<bool> write = backgroundWrites.ContinueWith(_ => AppendHistoryItem(historyItem), CancellationToken.None,
                    TaskContinuationOptions.None, TaskScheduler.Default);
                backgroundWrites = write;
                return write;
            }
        }

        /// <summary>Waits for the queued background writes. Returns false when they did not finish within <paramref name="timeout"/>.</summary>
        public bool FlushBackgroundWrites(TimeSpan timeout)
        {
            Task pending;

            lock (backgroundWritesLock)
            {
                pending = backgroundWrites;
            }

            return WaitForWrites(pending, timeout);
        }

        /// <summary>
        /// Stops accepting background writes and returns the task that completes when every accepted write has finished. Nothing can
        /// be queued behind it afterwards, so it is a stable point to wait for before closing the history.
        /// </summary>
        protected Task CloseBackgroundWrites()
        {
            lock (backgroundWritesLock)
            {
                backgroundWritesClosed = true;
                return backgroundWrites;
            }
        }

        private static readonly object exitWorkLock = new object();
        private static readonly List<Task> exitWork = new List<Task>();
        private static bool exitHooked;

        /// <summary>How long the process waits at exit for history work that outlived closing the history.</summary>
        internal static TimeSpan ProcessExitBudget { get; set; } = TimeSpan.FromMinutes(2);

        /// <summary>
        /// Keeps the process alive at exit until <paramref name="work"/> has finished (at most <see cref="ProcessExitBudget"/>),
        /// so accepted history writes are not abandoned when the application closes while a write is slow.
        /// </summary>
        protected static void FinishBeforeProcessExit(Task work)
        {
            lock (exitWorkLock)
            {
                exitWork.RemoveAll(task => task.IsCompleted);
                exitWork.Add(work);

                if (!exitHooked)
                {
                    exitHooked = true;
                    AppDomain.CurrentDomain.ProcessExit += (_, _) => WaitForExitWork();
                }
            }
        }

        private static void WaitForExitWork()
        {
            Task[] pending;

            lock (exitWorkLock)
            {
                pending = exitWork.Where(task => !task.IsCompleted).ToArray();
            }

            if (pending.Length > 0 && !WaitForWrites(Task.WhenAll(pending), ProcessExitBudget))
            {
                DebugHelper.WriteLine($"History writes did not finish within {ProcessExitBudget.TotalSeconds:0} seconds of exit; the last items may be missing from the history.");
            }
        }

        protected static bool WaitForWrites(Task pending, TimeSpan timeout)
        {
            try
            {
                return pending.Wait(timeout);
            }
            catch (AggregateException e)
            {
                DebugHelper.WriteException(e);
                return true;
            }
        }

        public bool AppendHistoryItems(IEnumerable<HistoryItem> historyItems)
        {
            try
            {
                return Append(historyItems.Where(IsValidHistoryItem));
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }

        private bool IsValidHistoryItem(HistoryItem historyItem)
        {
            return historyItem != null && !string.IsNullOrEmpty(historyItem.FileName) && historyItem.DateTime != DateTime.MinValue &&
                (!string.IsNullOrEmpty(historyItem.URL) || !string.IsNullOrEmpty(historyItem.FilePath));
        }

        internal List<HistoryItem> Load()
        {
            return Load(FilePath);
        }

        internal abstract List<HistoryItem> Load(string filePath);

        protected bool Append(IEnumerable<HistoryItem> historyItems)
        {
            return Append(FilePath, historyItems);
        }

        protected abstract bool Append(string filePath, IEnumerable<HistoryItem> historyItems);

        protected void Backup(string filePath)
        {
            if (!string.IsNullOrEmpty(BackupFolder))
            {
                if (CreateBackup)
                {
                    FileHelpers.CopyFile(filePath, BackupFolder);
                }

                if (CreateWeeklyBackup)
                {
                    FileHelpers.BackupFileWeekly(filePath, BackupFolder);
                }
            }
        }

        public void Test(int itemCount)
        {
            Test(FilePath, itemCount);
        }

        public void Test(string filePath, int itemCount)
        {
            HistoryItem historyItem = new HistoryItem()
            {
                FileName = "Example.png",
                FilePath = @"C:\ShareX\Screenshots\Example.png",
                DateTime = DateTime.Now,
                Type = "Image",
                Host = "Imgur",
                URL = "https://example.com/Example.png",
                ThumbnailURL = "https://example.com/Example.png",
                DeletionURL = "https://example.com/Example.png",
                ShortenedURL = "https://example.com/Example.png"
            };

            HistoryItem[] historyItems = new HistoryItem[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                historyItems[i] = historyItem;
            }

            Thread.Sleep(1000);

            DebugTimer saveTimer = new DebugTimer($"Saved {itemCount} items");
            Append(filePath, historyItems);
            saveTimer.WriteElapsedMilliseconds();

            Thread.Sleep(1000);

            DebugTimer loadTimer = new DebugTimer($"Loaded {itemCount} items");
            Load(filePath);
            loadTimer.WriteElapsedMilliseconds();
        }
    }
}

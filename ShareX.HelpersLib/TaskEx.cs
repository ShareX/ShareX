#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    public class TaskEx<T>
    {
        public delegate void ProgressChangedEventHandler(T progress);
        public event ProgressChangedEventHandler ProgressChanged;

        public bool IsRunning { get; private set; }
        public bool IsCanceled { get; private set; }

        private Progress<T> p;
        private CancellationTokenSource cts;

        public async Task Run(Action action)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException();
            }

            IsRunning = true;
            IsCanceled = false;

            p = new Progress<T>(OnProgressChanged);

            using (cts = new CancellationTokenSource())
            {
                try
                {
                    await Task.Run(action, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    IsCanceled = true;
                }
                finally
                {
                    IsRunning = false;
                }
            }
        }

        public void Report(T progress)
        {
            if (p != null)
            {
                ((IProgress<T>)p).Report(progress);
            }
        }

        public void Cancel()
        {
            cts?.Cancel();
        }

        public void ThrowIfCancellationRequested()
        {
            cts?.Token.ThrowIfCancellationRequested();
        }

        private void OnProgressChanged(T progress)
        {
            ProgressChanged?.Invoke(progress);
        }
    }
}
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

using Avalonia.Threading;
using ShareX.HelpersLib;
using System;
using System.Threading.Tasks;

namespace ShareX;

internal sealed record MultiUploadConfirmationResult(bool IsConfirmed, bool DontShowAgain);

internal static class MultiUploadConfirmationWindowIntegration
{
    public static MultiUploadConfirmationResult Show(int fileCount)
    {

        if (Dispatcher.UIThread.CheckAccess())
        {
            MultiUploadConfirmationResult result = new(false, false);
            DispatcherFrame frame = new();
            ShowCore(fileCount, value =>
            {
                result = value;
                frame.Continue = false;
            });
            Dispatcher.UIThread.PushFrame(frame);
            return result;
        }

        TaskCompletionSource<MultiUploadConfirmationResult> completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() =>
            ShowCore(fileCount, value => completion.TrySetResult(value)));
        return completion.Task.GetAwaiter().GetResult();
    }

    private static void ShowCore(int fileCount, Action<MultiUploadConfirmationResult> completed)
    {
        try
        {
            MultiUploadConfirmationWindow window = new(fileCount);
            window.Closed += (_, _) =>
                completed(new MultiUploadConfirmationResult(window.IsConfirmed, window.DontShowAgain));
            window.Show();
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            completed(new MultiUploadConfirmationResult(false, false));
        }
    }
}

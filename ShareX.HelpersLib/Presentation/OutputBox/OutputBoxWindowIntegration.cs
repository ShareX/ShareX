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
using ShareX.AvaloniaUI.Integration;
using System;
using System.Threading.Tasks;

namespace ShareX.HelpersLib;

public static class OutputBoxWindowIntegration
{
    public static void Show(string text, string title, bool scrollToEnd = false)
    {
        AvaloniaBootstrapper.EnsureInitialized();

        if (Dispatcher.UIThread.CheckAccess())
        {
            DispatcherFrame frame = new();
            ShowCore(text, title, scrollToEnd, () => frame.Continue = false);
            Dispatcher.UIThread.PushFrame(frame);
            return;
        }

        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() =>
            ShowCore(text, title, scrollToEnd, () => completion.TrySetResult()));
        completion.Task.GetAwaiter().GetResult();
    }

    private static void ShowCore(string text, string title, bool scrollToEnd, Action completed)
    {
        try
        {
            OutputBoxWindow window = new(text, title, scrollToEnd);
            window.Closed += (_, _) => completed();
            window.Show();
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            completed();
        }
    }
}

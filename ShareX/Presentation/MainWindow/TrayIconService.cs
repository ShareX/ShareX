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

using ShareX.HelpersLib;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX;

internal interface ITrayIconService : IDisposable
{
    event Action? RightButtonDown;
    event Action? RightButtonUp;

    bool Visible { get; set; }
    string ToolTipText { get; set; }

    void SetIcon(byte[] iconBytes);
    void SetIcon(Icon icon);
}

/// <summary>
/// WinForms tray icon adapter. The visible tray menu remains an Avalonia menu;
/// this class only owns the notification icon and its mouse interaction.
/// </summary>
internal sealed class WinFormsTrayIconService : ITrayIconService
{
    private readonly NotifyIcon _notifyIcon;
    private readonly Timer _singleClickTimer;
    private Icon? _ownedIcon;
    private int _leftClickCount;
    private bool _disposed;

    public event Action? RightButtonDown;
    public event Action? RightButtonUp;

    public bool Visible
    {
        get => _notifyIcon.Visible;
        set => _notifyIcon.Visible = value;
    }

    public string ToolTipText
    {
        get => _notifyIcon.Text;
        set => _notifyIcon.Text = value.Truncate(63);
    }

    public WinFormsTrayIconService(Icon icon, string toolTipText, bool visible)
    {
        _notifyIcon = new NotifyIcon
        {
            ContextMenuStrip = null,
            Text = toolTipText.Truncate(63)
        };
        _notifyIcon.MouseDown += OnMouseDown;
        _notifyIcon.MouseUp += OnMouseUp;

        _singleClickTimer = new Timer
        {
            Interval = SystemInformation.DoubleClickTime
        };
        _singleClickTimer.Tick += OnSingleClickTimerTick;

        SetIcon(icon);
        _notifyIcon.Visible = visible;
    }

    public void SetIcon(byte[] iconBytes)
    {
        using MemoryStream stream = new(iconBytes, writable: false);
        using Icon icon = new(stream);
        SetIcon(icon);
    }

    public void SetIcon(Icon icon)
    {
        Icon replacement = (Icon)icon.Clone();
        _notifyIcon.Icon = replacement;
        _ownedIcon?.Dispose();
        _ownedIcon = replacement;
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            RightButtonDown?.Invoke();
        }
    }

    private async void OnMouseUp(object? sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Left:
                if (ApplicationState.Settings.TrayLeftDoubleClickAction == HotkeyType.None)
                {
                    await TaskHelpers.ExecuteJob(ApplicationState.Settings.TrayLeftClickAction);
                }
                else
                {
                    _leftClickCount++;

                    if (_leftClickCount == 1)
                    {
                        _singleClickTimer.Start();
                    }
                    else
                    {
                        _leftClickCount = 0;
                        _singleClickTimer.Stop();
                        await TaskHelpers.ExecuteJob(ApplicationState.Settings.TrayLeftDoubleClickAction);
                    }
                }
                break;
            case MouseButtons.Middle:
                await TaskHelpers.ExecuteJob(ApplicationState.Settings.TrayMiddleClickAction);
                break;
            case MouseButtons.Right:
                RightButtonUp?.Invoke();
                break;
        }
    }

    private async void OnSingleClickTimerTick(object? sender, EventArgs e)
    {
        _singleClickTimer.Stop();

        if (_leftClickCount == 1)
        {
            _leftClickCount = 0;
            await TaskHelpers.ExecuteJob(ApplicationState.Settings.TrayLeftClickAction);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _singleClickTimer.Stop();
        _singleClickTimer.Tick -= OnSingleClickTimerTick;
        _singleClickTimer.Dispose();

        _notifyIcon.Visible = false;
        _notifyIcon.MouseDown -= OnMouseDown;
        _notifyIcon.MouseUp -= OnMouseUp;
        _notifyIcon.Icon = null;
        _notifyIcon.Dispose();

        _ownedIcon?.Dispose();
        _ownedIcon = null;
    }
}

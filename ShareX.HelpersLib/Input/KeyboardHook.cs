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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class KeyboardHook : IDisposable
    {
        public event KeyEventHandler KeyDown, KeyUp;

        private HookProc keyboardHookProc;
        private IntPtr keyboardHookHandle = IntPtr.Zero;

        public KeyboardHook()
        {
            keyboardHookProc = KeyboardHookProc;
            keyboardHookHandle = SetHook(NativeConstants.WH_KEYBOARD_LL, keyboardHookProc);
        }

        ~KeyboardHook()
        {
            Dispose();
        }

        private static IntPtr SetHook(int hookType, HookProc hookProc)
        {
            using (Process currentProcess = Process.GetCurrentProcess())
            using (ProcessModule currentModule = currentProcess.MainModule)
            {
                return NativeMethods.SetWindowsHookEx(hookType, hookProc, NativeMethods.GetModuleHandle(currentModule.ModuleName), 0);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                bool handled = false;

                switch ((KeyEvent)wParam)
                {
                    case KeyEvent.WM_KEYDOWN:
                    case KeyEvent.WM_SYSKEYDOWN:
                        handled = OnKeyDown(lParam);
                        break;
                    case KeyEvent.WM_KEYUP:
                    case KeyEvent.WM_SYSKEYUP:
                        handled = OnKeyUp(lParam);
                        break;
                }

                if (handled)
                {
                    return keyboardHookHandle;
                }
            }

            return NativeMethods.CallNextHookEx(keyboardHookHandle, nCode, wParam, lParam);
        }

        private bool OnKeyDown(IntPtr key)
        {
            if (KeyDown != null)
            {
                KeyEventArgs keyEventArgs = GetKeyEventArgs(key);
                KeyDown(this, keyEventArgs);
                return keyEventArgs.Handled || keyEventArgs.SuppressKeyPress;
            }

            return false;
        }

        private bool OnKeyUp(IntPtr key)
        {
            if (KeyUp != null)
            {
                KeyEventArgs keyEventArgs = GetKeyEventArgs(key);
                KeyUp(this, keyEventArgs);
                return keyEventArgs.Handled || keyEventArgs.SuppressKeyPress;
            }

            return false;
        }

        private KeyEventArgs GetKeyEventArgs(IntPtr key)
        {
            Keys keyData = (Keys)Marshal.ReadInt32(key) | Control.ModifierKeys;
            return new KeyEventArgs(keyData);
        }

        public void Dispose()
        {
            NativeMethods.UnhookWindowsHookEx(keyboardHookHandle);
        }
    }
}
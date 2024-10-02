using System;

namespace ShareX.HelpersLib
{
    public class MouseHook : IDisposable
    {
        private HookProc _proc;
        private static IntPtr _hookID = IntPtr.Zero;

        public delegate void MouseEventHandler(MouseEventInfo eventInfo);
        public event MouseEventHandler OnMouseEvent;

        public MouseHook()
        {
            _proc = HookCallback;
            _hookID = SetHook(_proc); // Set up global mouse hook
        }

        ~MouseHook()
        {
            Dispose();
        }

        private static IntPtr SetHook(HookProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return NativeMethods.SetWindowsHookEx(NativeConstants.WH_MOUSE_LL, proc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                switch ((WindowsMessages)wParam)
                {
                    case WindowsMessages.LBUTTONDOWN:
                        OnMouseEvent(new MouseEventInfo
                        {
                            ButtonState = ButtonState.LeftButtonDown,
                            CursorPosition = CaptureHelpers.GetCursorPosition()
                        });
                        break;

                    case WindowsMessages.LBUTTONUP:
                        OnMouseEvent(new MouseEventInfo
                        {
                            ButtonState = ButtonState.ButtonUp,
                            CursorPosition = CaptureHelpers.GetCursorPosition()
                        });
                        break;
                }
            }
            
            return NativeMethods.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            NativeMethods.UnhookWindowsHookEx(_hookID); // Clean up hook on exit
        }
    }
}
using System;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class MouseClickEffectManager : IDisposable
    {
        private MouseHook mouseHook;
        private MouseClickEffectForm clickEffectForm;
        private bool _disposed;

        // start click mouse effects
        public void Start()
        {
            var action = new Action(() =>
            {
                clickEffectForm = new MouseClickEffectForm();
                mouseHook = new MouseHook();
                mouseHook.OnMouseEvent += OnMouseEvent;
                clickEffectForm.Show();
            });

            // form needs to be running on main UI thread otherwise it will not show up
            if (Application.OpenForms[0].InvokeRequired)
            {
                Application.OpenForms[0].Invoke(action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Mouse event handler
        /// </summary>
        private void OnMouseEvent(MouseEventInfo eventInfo)
        {
            switch (eventInfo.ButtonState)
            {
                case ButtonState.LeftButtonDown:
                    clickEffectForm.DrawMouseEffect(eventInfo.CursorPosition);
                    break;
                default:
                    clickEffectForm.ClearMouseEffect();
                    break;
            }
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                mouseHook.OnMouseEvent -= OnMouseEvent;
                mouseHook.Dispose();
                clickEffectForm.Close();
                clickEffectForm.Dispose();
            }

            mouseHook = null;
            clickEffectForm = null;

            _disposed = true;
        }
    }
}
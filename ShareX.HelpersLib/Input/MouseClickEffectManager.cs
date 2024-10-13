using System;

namespace ShareX.HelpersLib
{
    public class MouseClickEffectManager : IDisposable
    {
        private bool disposed;
        private bool showEffect;
        private MouseHook mouseHook;
        private MouseClickEffectForm clickEffectForm;


        // start click mouse effects
        public void Start()
        {
            showEffect = true;
            clickEffectForm = new MouseClickEffectForm();
            mouseHook = new MouseHook();
            mouseHook.OnMouseEvent += OnMouseEvent;
            clickEffectForm.Show();
        }

        /// <summary>
        /// Mouse event handler
        /// </summary>
        private void OnMouseEvent(MouseEventInfo eventInfo)
        {
            if (!showEffect)
                return;

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

        public void Pause()
        {
            showEffect = false;
        }

        public void Resume()
        {
            showEffect = true;
        }

        public void Stop()
        {
            showEffect = false;
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
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

            disposed = true;
        }
    }
}
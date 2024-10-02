using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    // transparent form where mouse effect will be drawn
    public class MouseClickEffectForm : Form
    {
        private bool _drawEffect = false;
        public MouseClickEffectForm()
        {
            Size = new Size(40, 40);
            FormBorderStyle = FormBorderStyle.None;
            ShowIcon = false;
            ShowInTaskbar = false;
            TopMost = true;
            AllowTransparency = true;
            BackColor = Color.White; // Set a key color for transparency
            TransparencyKey = Color.White; // Make that color transparent
            Opacity = 0.3;
        }

        /// <summary>
        /// Draw mouse effect on given mouse position
        /// </summary>
        public void DrawMouseEffect(Point cursorPosition)
        {
            _drawEffect = true;
            CenterFormToCursorPosition(cursorPosition);
            Invalidate(); // redraw form
        }

        /// <summary>
        /// Clear mouse effect
        /// </summary>
        public void ClearMouseEffect()
        {
            _drawEffect = false;
            Invalidate(); // redraw form
        }

        /// <summary>
        /// Draw a circle as an mouse effect
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_drawEffect)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                // Define a brush to draw circle
                Brush brush = new SolidBrush(Color.Red);
                int diameter = 20;

                // Calculate the top-left corner to center the circle
                int x = (ClientSize.Width - diameter) / 2;
                int y = (ClientSize.Height - diameter) / 2;

                // .NET GDI+ is not precise when drawign circles this would be better off with WPF/vector-based drawing
                g.FillEllipse(brush, x, y, diameter, diameter);
            }

            base.OnPaint(e);
        }

        private void CenterFormToCursorPosition(Point cursorPosition)
        {
            var x = cursorPosition.X - (Width / 2);
            var y = cursorPosition.Y - (Height / 2);
            var flags = SetWindowPosFlags.SWP_NOSIZE |
                        SetWindowPosFlags.SWP_NOOWNERZORDER |
                        SetWindowPosFlags.SWP_NOACTIVATE;

            // Center the form/circle at the current mouse cursor position
            NativeMethods.SetWindowPos(Handle, (IntPtr)NativeConstants.HWND_TOPMOST, x, y, 0, 0, flags);
        }

    }
}
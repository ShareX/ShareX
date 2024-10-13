using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    // transparent form where mouse click effect will be drawn
    public class MouseClickEffectForm : LayeredForm
    {
        protected override CreateParams CreateParams
        {
            get
            {
                // set style to layered topmost transparent
                var createParams = base.CreateParams;
                createParams.ExStyle |= (int)(WindowStyles.WS_EX_TOPMOST | WindowStyles.WS_EX_TRANSPARENT);
                return createParams;
            }
        }

        /// <summary>
        /// Draw mouse effect on given mouse position
        /// </summary>
        public void DrawMouseEffect(Point cursorPosition)
        {
            CenterFormToCursorPosition(cursorPosition);
            SelectBitmap(GetCircleImage(Color.Red), 100);
        }

        /// <summary>
        /// Clear mouse effect
        /// </summary>
        public void ClearMouseEffect()
        {
            SelectBitmap(GetEmptyImage(), 1);
        }

        private Bitmap GetCircleImage(Color color)
        {
            var bmp = new Bitmap(ClientSize.Width, ClientSize.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                Brush brush = new SolidBrush(Color.FromArgb(100, color));
                var diameter = 20;
                // Calculate the top-left corner to center the circle
                var x = (ClientSize.Width - diameter) / 2;
                var y = (ClientSize.Height - diameter) / 2;

                // .NET GDI+ is not precise when drawign circles this would be better off with WPF/vector-based drawing
                g.FillEllipse(brush, x, y, diameter, diameter);
            }

            return bmp;
        }

        private Bitmap GetEmptyImage()
        {
            var backgroundImage = new Bitmap(ClientSize.Width, ClientSize.Height);
            var gBackgroundImage = Graphics.FromImage(backgroundImage);

            gBackgroundImage.InterpolationMode = InterpolationMode.NearestNeighbor;
            gBackgroundImage.SmoothingMode = SmoothingMode.HighSpeed;
            gBackgroundImage.CompositingMode = CompositingMode.SourceCopy;
            gBackgroundImage.CompositingQuality = CompositingQuality.HighSpeed;
            gBackgroundImage.Clear(Color.FromArgb(0, 0, 0, 0));

            return backgroundImage;
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
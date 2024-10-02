

using System.Drawing;

namespace ShareX.HelpersLib
{
    public class MouseEventInfo
    {
        public ButtonState ButtonState { get; set; }
        public Point CursorPosition { get; set; }
    }

    public enum ButtonState
    {
        LeftButtonDown,
        RightButtonDown,
        ButtonUp
    }
}
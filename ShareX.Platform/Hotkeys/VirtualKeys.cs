namespace ShareX.Platform;

/// <summary>Windows virtual key codes used as the portable key identity for <see cref="PlatformHotkey"/>.</summary>
public static class VirtualKeys
{
    public const int Back = 0x08;
    public const int Tab = 0x09;
    public const int Return = 0x0D;
    public const int Pause = 0x13;
    public const int CapsLock = 0x14;
    public const int Escape = 0x1B;
    public const int Space = 0x20;
    public const int PageUp = 0x21;
    public const int PageDown = 0x22;
    public const int End = 0x23;
    public const int Home = 0x24;
    public const int Left = 0x25;
    public const int Up = 0x26;
    public const int Right = 0x27;
    public const int Down = 0x28;
    public const int PrintScreen = 0x2C;
    public const int Insert = 0x2D;
    public const int Delete = 0x2E;
    public const int D0 = 0x30;
    public const int D9 = 0x39;
    public const int A = 0x41;
    public const int Z = 0x5A;
    public const int NumPad0 = 0x60;
    public const int NumPad9 = 0x69;
    public const int Multiply = 0x6A;
    public const int Add = 0x6B;
    public const int Subtract = 0x6D;
    public const int Decimal = 0x6E;
    public const int Divide = 0x6F;
    public const int F1 = 0x70;
    public const int F24 = 0x87;
    public const int NumLock = 0x90;
    public const int ScrollLock = 0x91;
    public const int OemSemicolon = 0xBA;
    public const int OemPlus = 0xBB;
    public const int OemComma = 0xBC;
    public const int OemMinus = 0xBD;
    public const int OemPeriod = 0xBE;
    public const int OemQuestion = 0xBF;
    public const int OemTilde = 0xC0;
    public const int OemOpenBrackets = 0xDB;
    public const int OemPipe = 0xDC;
    public const int OemCloseBrackets = 0xDD;
    public const int OemQuotes = 0xDE;

    public static bool IsLetter(int keyCode) => keyCode >= A && keyCode <= Z;

    public static bool IsDigit(int keyCode) => keyCode >= D0 && keyCode <= D9;

    public static bool IsNumPadDigit(int keyCode) => keyCode >= NumPad0 && keyCode <= NumPad9;

    public static bool IsFunctionKey(int keyCode) => keyCode >= F1 && keyCode <= F24;

    public static string GetName(int keyCode)
    {
        if (IsLetter(keyCode) || IsDigit(keyCode)) return ((char)keyCode).ToString();
        if (IsNumPadDigit(keyCode)) return "NumPad" + (keyCode - NumPad0);
        if (IsFunctionKey(keyCode)) return "F" + (keyCode - F1 + 1);

        return keyCode switch
        {
            Back => "Backspace",
            Tab => "Tab",
            Return => "Enter",
            Pause => "Pause",
            CapsLock => "CapsLock",
            Escape => "Escape",
            Space => "Space",
            PageUp => "PageUp",
            PageDown => "PageDown",
            End => "End",
            Home => "Home",
            Left => "Left",
            Up => "Up",
            Right => "Right",
            Down => "Down",
            PrintScreen => "PrintScreen",
            Insert => "Insert",
            Delete => "Delete",
            Multiply => "NumPad*",
            Add => "NumPad+",
            Subtract => "NumPad-",
            Decimal => "NumPad.",
            Divide => "NumPad/",
            NumLock => "NumLock",
            ScrollLock => "ScrollLock",
            OemSemicolon => ";",
            OemPlus => "=",
            OemComma => ",",
            OemMinus => "-",
            OemPeriod => ".",
            OemQuestion => "/",
            OemTilde => "`",
            OemOpenBrackets => "[",
            OemPipe => "\\",
            OemCloseBrackets => "]",
            OemQuotes => "'",
            _ => $"0x{keyCode:X2}"
        };
    }
}

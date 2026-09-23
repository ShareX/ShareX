#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using System.Drawing;

namespace ShareX.Tools;

public enum MouseHighlightMode
{
    Circle,
    Ripple
}

public sealed class MouseHighlighterOptions
{
    public bool AutoActivate { get; set; }
    public MouseHighlightMode Mode { get; set; } = MouseHighlightMode.Ripple;
    public bool HighlightPrimaryClicks { get; set; } = true;
    public Color PrimaryColor { get; set; } = Color.FromArgb(166, 54, 169, 225);
    public bool ShowPrimaryReleaseCrosshairs { get; set; }
    public bool HighlightSecondaryClicks { get; set; } = true;
    public Color SecondaryColor { get; set; } = Color.FromArgb(166, 89, 184, 106);
    public bool ShowSecondaryReleaseCrosshairs { get; set; }
    public bool HighlightMiddleClicks { get; set; } = true;
    public Color MiddleColor { get; set; } = Color.FromArgb(166, 242, 142, 43);
    public bool ShowMiddleReleaseCrosshairs { get; set; }
    public Color AlwaysColor { get; set; } = Color.Transparent;
    public int Radius { get; set; } = 20;
    public int FadeDelay { get; set; } = 500;
    public int FadeDuration { get; set; } = 250;
    public int RippleSize { get; set; } = 60;
    public double RippleIntensity { get; set; } = 0.7;
    public int RippleDuration { get; set; } = 500;
    public bool FollowCursorWhileHeld { get; set; } = true;

    internal bool IsButtonEnabled(MouseHighlightButton button) => button switch
    {
        MouseHighlightButton.Primary => HighlightPrimaryClicks,
        MouseHighlightButton.Secondary => HighlightSecondaryClicks,
        MouseHighlightButton.Middle => HighlightMiddleClicks,
        _ => false
    };

    internal Color GetColor(MouseHighlightButton button) => button switch
    {
        MouseHighlightButton.Primary => PrimaryColor,
        MouseHighlightButton.Secondary => SecondaryColor,
        MouseHighlightButton.Middle => MiddleColor,
        _ => Color.Transparent
    };

    internal bool ShowReleaseCrosshairs(MouseHighlightButton button) => button switch
    {
        MouseHighlightButton.Primary => HighlightPrimaryClicks && ShowPrimaryReleaseCrosshairs,
        MouseHighlightButton.Secondary => HighlightSecondaryClicks && ShowSecondaryReleaseCrosshairs,
        MouseHighlightButton.Middle => HighlightMiddleClicks && ShowMiddleReleaseCrosshairs,
        _ => false
    };

    // Also validate settings loaded from disk, which may bypass the UI limits.
    public void Validate()
    {
        if (!Enum.IsDefined(Mode)) Mode = MouseHighlightMode.Ripple;
        Radius = Math.Clamp(Radius, 5, 500);
        FadeDelay = Math.Clamp(FadeDelay, 0, 10000);
        FadeDuration = Math.Clamp(FadeDuration, 0, 10000);
        RippleSize = Math.Clamp(RippleSize, 10, 300);
        RippleIntensity = double.IsFinite(RippleIntensity) ? Math.Clamp(RippleIntensity, 0.15, 1.35) : 0.7;
        RippleDuration = Math.Clamp(RippleDuration, 60, 2000);
    }
}

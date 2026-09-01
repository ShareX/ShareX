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

using ShareX.AvaloniaUI.Theming;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Adjustments;

public enum ChannelSwapMode
{
    RedGreen,
    RedBlue,
    GreenBlue,
    RotateRGB,
    RotateBGR
}

public sealed class ChannelSwapImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "channel_swap";
    public override string Name => "Channel swap";
    public override string IconKey => LucideIcons.shuffle;
    public override string Description => "Swaps color channels for creative color effects.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Enum<ChannelSwapImageEffect, ChannelSwapMode>(
            "mode", "Swap mode", ChannelSwapMode.RedGreen, (e, v) => e.Mode = v,
            new (string, ChannelSwapMode)[]
            {
                ("Red \u2194 Green", ChannelSwapMode.RedGreen),
                ("Red \u2194 Blue", ChannelSwapMode.RedBlue),
                ("Green \u2194 Blue", ChannelSwapMode.GreenBlue),
                ("Rotate R\u2192G\u2192B", ChannelSwapMode.RotateRGB),
                ("Rotate B\u2192G\u2192R", ChannelSwapMode.RotateBGR)
            })
    ];

    public ChannelSwapMode Mode { get; set; } = ChannelSwapMode.RedGreen;

    public override SKBitmap Apply(SKBitmap source)
    {
        return ApplyPixelOperation(source, c => Mode switch
        {
            ChannelSwapMode.RedGreen => new SKColor(c.Green, c.Red, c.Blue, c.Alpha),
            ChannelSwapMode.RedBlue => new SKColor(c.Blue, c.Green, c.Red, c.Alpha),
            ChannelSwapMode.GreenBlue => new SKColor(c.Red, c.Blue, c.Green, c.Alpha),
            ChannelSwapMode.RotateRGB => new SKColor(c.Blue, c.Red, c.Green, c.Alpha),
            ChannelSwapMode.RotateBGR => new SKColor(c.Green, c.Blue, c.Red, c.Alpha),
            _ => c
        });
    }
}
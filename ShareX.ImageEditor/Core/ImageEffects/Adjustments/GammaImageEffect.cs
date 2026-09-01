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

public sealed class GammaImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "gamma";
    public override string Name => "Gamma";
    public override string IconKey => LucideIcons.gauge;
    public override string Description => "Adjusts the gamma level.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<GammaImageEffect>("amount", "Amount", 0.1, 5, 1, (effect, value) => effect.Amount = value, tickFrequency: 0.1, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0.0}")
    ];

    public float Amount { get; set; } = 1f;

    private float _cachedAmount = float.NaN;
    private byte[]? _cachedTable;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (_cachedTable is null || _cachedAmount != Amount)
        {
            _cachedAmount = Amount;
            _cachedTable = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                float val = i / 255f;
                float corrected = (float)Math.Pow(val, 1.0 / Amount);
                _cachedTable[i] = (byte)(Math.Max(0, Math.Min(1, corrected)) * 255);
            }
        }

        using var filter = SKColorFilter.CreateTable(null, _cachedTable, _cachedTable, _cachedTable);
        return ApplyColorFilter(source, filter);
    }
}
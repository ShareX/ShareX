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

using ShareX.HelpersLib;
using SharpGen.Runtime;
using Vortice.DXGI;

namespace ShareX.ScreenRecordingLib.Video;

/// <param name="SdrWhiteScale">SDR white in scRGB units (1.0 = 80 nits), from the Windows "SDR content brightness" setting.</param>
/// <param name="PeakNits">Peak luminance reported by the display.</param>
internal readonly record struct MonitorColorInfo(bool IsHdr, float SdrWhiteScale, float PeakNits)
{
    private const float ScRgbNits = 80f;
    private const float DefaultPeakNits = 1000f;

    public static readonly MonitorColorInfo Sdr = new(false, 1f, ScRgbNits);

    /// <summary>The display's peak relative to SDR white: how much highlight headroom the tone mapper compresses.</summary>
    public float RelativePeak => Math.Clamp(PeakNits / (ScRgbNits * SdrWhiteScale), 1.25f, 125f);

    public static MonitorColorInfo Get(nint monitor)
    {
        try
        {
            using IDXGIFactory1 factory = Vortice.DXGI.DXGI.CreateDXGIFactory1<IDXGIFactory1>();
            for (uint i = 0; factory.EnumAdapters1(i, out IDXGIAdapter1 adapter).Success; i++)
            {
                using (adapter)
                {
                    for (uint j = 0; adapter.EnumOutputs(j, out IDXGIOutput output).Success; j++)
                    {
                        using (output)
                        {
                            if (output.Description.Monitor != monitor) continue;
                            using IDXGIOutput6 output6 = output.QueryInterface<IDXGIOutput6>();
                            OutputDescription1 description = output6.Description1;
                            // Windows composes an HDR desktop in scRGB and scans it out as BT.2020 PQ.
                            if (description.ColorSpace != ColorSpaceType.RgbFullG2084NoneP2020) return Sdr;
                            float peak = float.IsFinite(description.MaxLuminance) && description.MaxLuminance >= ScRgbNits
                                ? description.MaxLuminance : DefaultPeakNits;
                            return new(true, DisplayConfigHelper.GetSdrWhiteScale(description.DeviceName), peak);
                        }
                    }
                }
            }
        }
        catch (SharpGenException e)
        {
            // Detection is best effort: the 8-bit capture path remains correct for SDR desktops.
            DebugHelper.WriteException(e, "Could not query the display's HDR state.");
        }
        return Sdr;
    }
}

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
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;
using Vortice.WIC;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ShareX.ScreenCaptureLib
{
    internal static class HDRScreenCapture
    {
        private const uint FrameAcquireTimeout = 500;
        private const int MaxFrameAcquireAttempts = 3;
        private const float SceneReferredSdrWhiteNits = 80f;
        private const float DefaultHdrPeakNits = 1000f;
        private const float ToneMapKnee = 0.75f;
        private const int SdrReferenceTolerance = 4;

        private static readonly Format[] supportedFormats =
        {
            Format.R16G16B16A16_Float,
            Format.R10G10B10A2_UNorm,
            Format.B8G8R8A8_UNorm
        };

        public static bool ApplyColorCorrection(Bitmap destination, Rectangle captureRectangle)
        {
            using IDXGIFactory1 factory = Vortice.DXGI.DXGI.CreateDXGIFactory1<IDXGIFactory1>();
            bool corrected = false;

            for (uint adapterIndex = 0; ; adapterIndex++)
            {
                if (factory.EnumAdapters1(adapterIndex, out IDXGIAdapter1 adapter).Failure)
                {
                    break;
                }

                using (adapter)
                {
                    corrected |= ApplyAdapterColorCorrection(adapter, destination, captureRectangle);
                }
            }

            return corrected;
        }

        private static bool ApplyAdapterColorCorrection(IDXGIAdapter1 adapter, Bitmap destination, Rectangle captureRectangle)
        {
            ID3D11Device device = null;
            ID3D11DeviceContext deviceContext = null;
            bool corrected = false;

            try
            {
                for (uint outputIndex = 0; ; outputIndex++)
                {
                    if (adapter.EnumOutputs(outputIndex, out IDXGIOutput output).Failure)
                    {
                        break;
                    }

                    using (output)
                    {
                        try
                        {
                            using IDXGIOutput6 output6 = output.QueryInterface<IDXGIOutput6>();
                            OutputDescription1 outputDescription = output6.Description1;
                            Rectangle outputBounds = GetOutputBounds(outputDescription);

                            if (!IsHdrOutput(outputDescription) || !captureRectangle.IntersectsWith(outputBounds))
                            {
                                continue;
                            }

                            if (device == null)
                            {
                                if (D3D11.D3D11CreateDevice(adapter, DriverType.Unknown, DeviceCreationFlags.BgraSupport,
                                    [FeatureLevel.Level_11_1, FeatureLevel.Level_11_0], out device, out deviceContext).Failure)
                                {
                                    return corrected;
                                }
                            }

                            using CapturedOutput capturedOutput = CaptureOutput(output, outputDescription, device, deviceContext);

                            if (capturedOutput != null)
                            {
                                CopyOutputIntersection(destination, captureRectangle, capturedOutput, outputBounds);
                                corrected = true;
                            }
                        }
                        catch (Exception e)
                        {
                            DebugHelper.WriteException(e, "HDR capture failed for a display output.");
                        }
                    }
                }
            }
            finally
            {
                deviceContext?.Dispose();
                device?.Dispose();
            }

            return corrected;
        }

        private static CapturedOutput CaptureOutput(IDXGIOutput output, OutputDescription1 outputDescription,
            ID3D11Device device, ID3D11DeviceContext deviceContext)
        {
            using IDXGIOutput5 output5 = output.QueryInterface<IDXGIOutput5>();
            using IDXGIOutputDuplication duplication = output5.DuplicateOutput1(device, supportedFormats);

            IDXGIResource desktopResource = null;
            bool frameAcquired = false;

            try
            {
                if (!TryAcquireDesktopFrame(duplication, out desktopResource))
                {
                    return null;
                }

                frameAcquired = true;

                using (ID3D11Texture2D desktopTexture = desktopResource.QueryInterface<ID3D11Texture2D>())
                {
                    Texture2DDescription textureDescription = desktopTexture.Description;

                    if (textureDescription.Format != Format.R16G16B16A16_Float &&
                        textureDescription.Format != Format.R10G10B10A2_UNorm)
                    {
                        return null;
                    }

                    Texture2DDescription stagingDescription = textureDescription;
                    stagingDescription.Usage = ResourceUsage.Staging;
                    stagingDescription.BindFlags = BindFlags.None;
                    stagingDescription.CPUAccessFlags = CpuAccessFlags.Read;
                    stagingDescription.MiscFlags = ResourceOptionFlags.None;

                    using ID3D11Texture2D stagingTexture = device.CreateTexture2D(stagingDescription);
                    deviceContext.CopyResource(stagingTexture, desktopTexture);

                    if (deviceContext.Map(stagingTexture, 0, MapMode.Read, Vortice.Direct3D11.MapFlags.None,
                        out MappedSubresource mapped).Failure)
                    {
                        return null;
                    }

                    try
                    {
                        float sdrWhiteScale = DisplayConfigHelper.GetSdrWhiteScale(outputDescription.DeviceName);
                        float peakNits = GetPeakNits(outputDescription);
                        Bitmap toneMappedBitmap = null;
                        Bitmap sdrReferenceBitmap = null;

                        try
                        {
                            toneMappedBitmap = ConvertWithWindowsToneMapper(mapped, textureDescription);
                            sdrReferenceBitmap = ConvertToSdrReference(mapped, textureDescription, sdrWhiteScale);
                        }
                        catch
                        {
                            toneMappedBitmap?.Dispose();
                            sdrReferenceBitmap?.Dispose();
                            toneMappedBitmap = ConvertToSrgb(mapped, textureDescription, sdrWhiteScale, peakNits);
                        }

                        CapturedOutput capturedOutput = new CapturedOutput(toneMappedBitmap, sdrReferenceBitmap);
                        RotateOutput(capturedOutput.ToneMappedBitmap, outputDescription.Rotation);

                        if (capturedOutput.SdrReferenceBitmap != null)
                        {
                            RotateOutput(capturedOutput.SdrReferenceBitmap, outputDescription.Rotation);
                        }

                        Rectangle outputBounds = GetOutputBounds(outputDescription);

                        if (capturedOutput.ToneMappedBitmap.Width != outputBounds.Width ||
                            capturedOutput.ToneMappedBitmap.Height != outputBounds.Height)
                        {
                            capturedOutput.Dispose();
                            return null;
                        }

                        return capturedOutput;
                    }
                    finally
                    {
                        deviceContext.Unmap(stagingTexture, 0);
                    }
                }
            }
            finally
            {
                if (frameAcquired)
                {
                    duplication.ReleaseFrame();
                }

                desktopResource?.Dispose();
            }
        }

        private static bool TryAcquireDesktopFrame(IDXGIOutputDuplication duplication, out IDXGIResource desktopResource)
        {
            desktopResource = null;

            for (int attempt = 0; attempt < MaxFrameAcquireAttempts; attempt++)
            {
                // A newly created duplication can initially report a pointer-only update. Its texture does not
                // contain a desktop image yet, so presenting it would make the region capture entirely black.
                DwmFlush();

                if (duplication.AcquireNextFrame(FrameAcquireTimeout, out OutduplFrameInfo frameInfo,
                    out desktopResource).Failure)
                {
                    return false;
                }

                if (frameInfo.LastPresentTime != 0)
                {
                    return true;
                }

                duplication.ReleaseFrame();
                desktopResource.Dispose();
                desktopResource = null;
            }

            return false;
        }

        private static unsafe Bitmap ConvertWithWindowsToneMapper(MappedSubresource mapped,
            Texture2DDescription description)
        {
            Guid sourcePixelFormat = description.Format switch
            {
                Format.R16G16B16A16_Float => Vortice.WIC.PixelFormat.Format64bppRGBAHalf,
                Format.R10G10B10A2_UNorm => Vortice.WIC.PixelFormat.Format32bppR10G10B10A2HDR10,
                _ => throw new NotSupportedException($"Unsupported HDR capture format: {description.Format}")
            };

            uint width = description.Width;
            uint height = description.Height;
            uint sourceBufferSize = checked(mapped.RowPitch * height);

            using IWICImagingFactory factory = new IWICImagingFactory();
            using IWICImagingFactory3 factory3 = factory.QueryInterface<IWICImagingFactory3>();
            using IWICBitmap sourceBitmap = factory.CreateBitmapFromMemory(width, height, sourcePixelFormat,
                mapped.RowPitch, sourceBufferSize, mapped.DataPointer.ToPointer());
            using IWICBitmapToneMapper toneMapper = factory3.CreateBitmapToneMapper();

            toneMapper.InitializeForSdrTarget(sourceBitmap, Vortice.WIC.PixelFormat.Format32bppBGRA,
                BitmapToneMappingMode.ToneMappingMode_Default);

            Bitmap bitmap = new Bitmap((int)width, (int)height, PixelFormat.Format32bppArgb);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            bool converted = false;

            try
            {
                int destinationStride = Math.Abs(bitmapData.Stride);
                uint destinationBufferSize = checked((uint)(destinationStride * bitmap.Height));
                toneMapper.CopyPixels(new RectI(0, 0, bitmap.Width, bitmap.Height),
                    (uint)destinationStride, destinationBufferSize, bitmapData.Scan0);
                converted = true;
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);

                if (!converted)
                {
                    bitmap.Dispose();
                }
            }

            return bitmap;
        }

        private static unsafe Bitmap ConvertToSdrReference(MappedSubresource mapped,
            Texture2DDescription description, float sdrWhiteScale)
        {
            int width = (int)description.Width;
            int height = (int)description.Height;
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            bool converted = false;

            try
            {
                for (int y = 0; y < height; y++)
                {
                    byte* sourceRow = (byte*)mapped.DataPointer + y * mapped.RowPitch;
                    byte* destinationRow = (byte*)bitmapData.Scan0 + y * bitmapData.Stride;

                    for (int x = 0; x < width; x++)
                    {
                        float red;
                        float green;
                        float blue;

                        if (description.Format == Format.R16G16B16A16_Float)
                        {
                            ushort* sourcePixel = (ushort*)(sourceRow + x * 8);
                            red = (float)BitConverter.UInt16BitsToHalf(sourcePixel[0]) / sdrWhiteScale;
                            green = (float)BitConverter.UInt16BitsToHalf(sourcePixel[1]) / sdrWhiteScale;
                            blue = (float)BitConverter.UInt16BitsToHalf(sourcePixel[2]) / sdrWhiteScale;
                        }
                        else
                        {
                            uint packedPixel = *(uint*)(sourceRow + x * 4);
                            red = PqToLinearSrgb((packedPixel & 0x3FF) / 1023f, sdrWhiteScale);
                            green = PqToLinearSrgb(((packedPixel >> 10) & 0x3FF) / 1023f, sdrWhiteScale);
                            blue = PqToLinearSrgb(((packedPixel >> 20) & 0x3FF) / 1023f, sdrWhiteScale);
                            ConvertRec2020ToSrgb(ref red, ref green, ref blue);
                        }

                        bool extendedRange = !IsSdrChannel(red) || !IsSdrChannel(green) || !IsSdrChannel(blue);
                        byte* destinationPixel = destinationRow + x * 4;
                        destinationPixel[0] = LinearToSrgbByte(blue);
                        destinationPixel[1] = LinearToSrgbByte(green);
                        destinationPixel[2] = LinearToSrgbByte(red);

                        // Alpha is used only as an internal mask: extended-range pixels must use the
                        // Windows tone-mapped result even if the legacy capture happened to clip to white.
                        destinationPixel[3] = extendedRange ? (byte)255 : (byte)0;
                    }
                }

                converted = true;
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);

                if (!converted)
                {
                    bitmap.Dispose();
                }
            }

            return bitmap;
        }

        private static bool IsSdrChannel(float value)
        {
            return float.IsFinite(value) && value >= 0f && value <= 1f;
        }

        private static unsafe Bitmap ConvertToSrgb(MappedSubresource mapped, Texture2DDescription description,
            float sdrWhiteScale, float peakNits)
        {
            int width = (int)description.Width;
            int height = (int)description.Height;
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            bool converted = false;

            try
            {
                float relativePeak = Math.Clamp(peakNits / (SceneReferredSdrWhiteNits * sdrWhiteScale), 1.25f, 125f);

                for (int y = 0; y < height; y++)
                {
                    byte* sourceRow = (byte*)mapped.DataPointer + y * mapped.RowPitch;
                    byte* destinationRow = (byte*)bitmapData.Scan0 + y * bitmapData.Stride;

                    for (int x = 0; x < width; x++)
                    {
                        float red;
                        float green;
                        float blue;

                        if (description.Format == Format.R16G16B16A16_Float)
                        {
                            ushort* sourcePixel = (ushort*)(sourceRow + x * 8);
                            red = (float)BitConverter.UInt16BitsToHalf(sourcePixel[0]) / sdrWhiteScale;
                            green = (float)BitConverter.UInt16BitsToHalf(sourcePixel[1]) / sdrWhiteScale;
                            blue = (float)BitConverter.UInt16BitsToHalf(sourcePixel[2]) / sdrWhiteScale;
                        }
                        else
                        {
                            uint packedPixel = *(uint*)(sourceRow + x * 4);
                            red = PqToLinearSrgb((packedPixel & 0x3FF) / 1023f, sdrWhiteScale);
                            green = PqToLinearSrgb(((packedPixel >> 10) & 0x3FF) / 1023f, sdrWhiteScale);
                            blue = PqToLinearSrgb(((packedPixel >> 20) & 0x3FF) / 1023f, sdrWhiteScale);
                            ConvertRec2020ToSrgb(ref red, ref green, ref blue);
                        }

                        ToneMapAndCompressGamut(ref red, ref green, ref blue, relativePeak);

                        byte* destinationPixel = destinationRow + x * 4;
                        destinationPixel[0] = LinearToSrgbByte(blue);
                        destinationPixel[1] = LinearToSrgbByte(green);
                        destinationPixel[2] = LinearToSrgbByte(red);
                        destinationPixel[3] = 255;
                    }
                }

                converted = true;
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);

                if (!converted)
                {
                    bitmap.Dispose();
                }
            }

            return bitmap;
        }

        private static void ToneMapAndCompressGamut(ref float red, ref float green, ref float blue, float relativePeak)
        {
            red = Math.Max(red, 0f);
            green = Math.Max(green, 0f);
            blue = Math.Max(blue, 0f);

            float maximum = Math.Max(red, Math.Max(green, blue));

            // Values entirely inside the SDR/sRGB range are deliberately left untouched.
            if (maximum <= 1f)
            {
                return;
            }

            float luminance = red * 0.2126f + green * 0.7152f + blue * 0.0722f;
            float mappedLuminance = ToneMapShoulder(luminance, relativePeak);

            if (luminance > 0.00001f)
            {
                float luminanceScale = mappedLuminance / luminance;
                red *= luminanceScale;
                green *= luminanceScale;
                blue *= luminanceScale;
            }

            maximum = Math.Max(red, Math.Max(green, blue));

            if (maximum > 1f)
            {
                // Bring wide-gamut colors into sRGB while retaining their luminance and hue direction.
                float gray = Math.Clamp(mappedLuminance, 0f, 1f);
                float chromaScale = maximum > gray ? (1f - gray) / (maximum - gray) : 0f;
                red = gray + (red - gray) * chromaScale;
                green = gray + (green - gray) * chromaScale;
                blue = gray + (blue - gray) * chromaScale;
            }
        }

        private static float ToneMapShoulder(float value, float relativePeak)
        {
            if (value <= ToneMapKnee)
            {
                return Math.Max(value, 0f);
            }

            float peak = Math.Max(relativePeak, ToneMapKnee + 0.001f);
            float normalized = Math.Clamp((value - ToneMapKnee) / (peak - ToneMapKnee), 0f, 1f);
            float curveStrength = (peak - ToneMapKnee) / (1f - ToneMapKnee);
            float shoulder = curveStrength * normalized / (1f + (curveStrength - 1f) * normalized);

            return ToneMapKnee + (1f - ToneMapKnee) * shoulder;
        }

        private static float PqToLinearSrgb(float encoded, float sdrWhiteScale)
        {
            const float m1 = 2610f / 16384f;
            const float m2 = 2523f / 32f;
            const float c1 = 3424f / 4096f;
            const float c2 = 2413f / 128f;
            const float c3 = 2392f / 128f;

            float power = MathF.Pow(Math.Clamp(encoded, 0f, 1f), 1f / m2);
            float normalizedNits = MathF.Pow(Math.Max(power - c1, 0f) / Math.Max(c2 - c3 * power, 0.00001f), 1f / m1);
            float nits = normalizedNits * 10000f;

            return nits / (SceneReferredSdrWhiteNits * sdrWhiteScale);
        }

        private static void ConvertRec2020ToSrgb(ref float red, ref float green, ref float blue)
        {
            float sourceRed = red;
            float sourceGreen = green;
            float sourceBlue = blue;

            red = 1.660491f * sourceRed - 0.587641f * sourceGreen - 0.072850f * sourceBlue;
            green = -0.124550f * sourceRed + 1.132900f * sourceGreen - 0.008349f * sourceBlue;
            blue = -0.018151f * sourceRed - 0.100579f * sourceGreen + 1.118730f * sourceBlue;
        }

        private static byte LinearToSrgbByte(float value)
        {
            value = Math.Clamp(value, 0f, 1f);
            float encoded = value <= 0.0031308f
                ? value * 12.92f
                : 1.055f * MathF.Pow(value, 1f / 2.4f) - 0.055f;

            return (byte)Math.Clamp((int)MathF.Round(encoded * 255f), 0, 255);
        }

        private static void RotateOutput(Bitmap bitmap, ModeRotation rotation)
        {
            switch (rotation)
            {
                case ModeRotation.Rotate90:
                    bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case ModeRotation.Rotate180:
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case ModeRotation.Rotate270:
                    bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
        }

        private static void CopyOutputIntersection(Bitmap destination, Rectangle captureRectangle,
            CapturedOutput capturedOutput, Rectangle outputBounds)
        {
            Rectangle intersection = Rectangle.Intersect(captureRectangle, outputBounds);

            if (intersection.IsEmpty)
            {
                return;
            }

            Rectangle sourceRectangle = new Rectangle(
                intersection.X - outputBounds.X,
                intersection.Y - outputBounds.Y,
                intersection.Width,
                intersection.Height);
            Rectangle destinationRectangle = new Rectangle(
                intersection.X - captureRectangle.X,
                intersection.Y - captureRectangle.Y,
                intersection.Width,
                intersection.Height);

            if (capturedOutput.SdrReferenceBitmap != null &&
                Image.GetPixelFormatSize(destination.PixelFormat) == 32)
            {
                CopyToneMappedPixels(destination, destinationRectangle, capturedOutput.ToneMappedBitmap,
                    capturedOutput.SdrReferenceBitmap, sourceRectangle);
                return;
            }

            using Graphics graphics = Graphics.FromImage(destination);
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.DrawImage(capturedOutput.ToneMappedBitmap, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);
        }

        private static unsafe void CopyToneMappedPixels(Bitmap destination, Rectangle destinationRectangle,
            Bitmap toneMappedBitmap, Bitmap sdrReferenceBitmap, Rectangle sourceRectangle)
        {
            BitmapData destinationData = null;
            BitmapData toneMappedData = null;
            BitmapData referenceData = null;

            try
            {
                destinationData = destination.LockBits(destinationRectangle, ImageLockMode.ReadWrite,
                    destination.PixelFormat);
                toneMappedData = toneMappedBitmap.LockBits(sourceRectangle, ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);
                referenceData = sdrReferenceBitmap.LockBits(sourceRectangle, ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

                for (int y = 0; y < sourceRectangle.Height; y++)
                {
                    byte* destinationRow = (byte*)destinationData.Scan0 + y * destinationData.Stride;
                    byte* toneMappedRow = (byte*)toneMappedData.Scan0 + y * toneMappedData.Stride;
                    byte* referenceRow = (byte*)referenceData.Scan0 + y * referenceData.Stride;

                    for (int x = 0; x < sourceRectangle.Width; x++)
                    {
                        byte* destinationPixel = destinationRow + x * 4;
                        byte* toneMappedPixel = toneMappedRow + x * 4;
                        byte* referencePixel = referenceRow + x * 4;

                        bool extendedRange = referencePixel[3] != 0;
                        bool legacyCaptureMatchesSdr =
                            Math.Abs(destinationPixel[0] - referencePixel[0]) <= SdrReferenceTolerance &&
                            Math.Abs(destinationPixel[1] - referencePixel[1]) <= SdrReferenceTolerance &&
                            Math.Abs(destinationPixel[2] - referencePixel[2]) <= SdrReferenceTolerance;

                        if (extendedRange || !legacyCaptureMatchesSdr)
                        {
                            destinationPixel[0] = toneMappedPixel[0];
                            destinationPixel[1] = toneMappedPixel[1];
                            destinationPixel[2] = toneMappedPixel[2];
                            destinationPixel[3] = 255;
                        }
                    }
                }
            }
            finally
            {
                if (referenceData != null)
                {
                    sdrReferenceBitmap.UnlockBits(referenceData);
                }

                if (toneMappedData != null)
                {
                    toneMappedBitmap.UnlockBits(toneMappedData);
                }

                if (destinationData != null)
                {
                    destination.UnlockBits(destinationData);
                }
            }
        }

        private sealed class CapturedOutput : IDisposable
        {
            public Bitmap ToneMappedBitmap { get; }
            public Bitmap SdrReferenceBitmap { get; }

            public CapturedOutput(Bitmap toneMappedBitmap, Bitmap sdrReferenceBitmap)
            {
                ToneMappedBitmap = toneMappedBitmap;
                SdrReferenceBitmap = sdrReferenceBitmap;
            }

            public void Dispose()
            {
                ToneMappedBitmap?.Dispose();
                SdrReferenceBitmap?.Dispose();
            }
        }

        private static bool IsHdrOutput(OutputDescription1 description)
        {
            return description.AttachedToDesktop &&
                description.ColorSpace == ColorSpaceType.RgbFullG2084NoneP2020;
        }

        private static Rectangle GetOutputBounds(OutputDescription1 description)
        {
            return Rectangle.FromLTRB(
                description.DesktopCoordinates.Left,
                description.DesktopCoordinates.Top,
                description.DesktopCoordinates.Right,
                description.DesktopCoordinates.Bottom);
        }

        private static float GetPeakNits(OutputDescription1 description)
        {
            float peakNits = description.MaxLuminance;

            if (!float.IsFinite(peakNits) || peakNits < SceneReferredSdrWhiteNits)
            {
                peakNits = DefaultHdrPeakNits;
            }

            return peakNits;
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmFlush();
    }

    internal static class DisplayConfigHelper
    {
        private const uint QueryDisplayConfigOnlyActivePaths = 0x00000002;
        private const uint DisplayConfigGetSourceName = 1;
        private const uint DisplayConfigGetSdrWhiteLevel = 11;
        private const int DisplayConfigModeInfoSize = 64;

        public static float GetSdrWhiteScale(string deviceName)
        {
            try
            {
                if (GetDisplayConfigBufferSizes(QueryDisplayConfigOnlyActivePaths, out uint pathCount, out uint modeCount) != 0)
                {
                    return 1f;
                }

                DisplayConfigPathInfo[] paths = new DisplayConfigPathInfo[pathCount];
                IntPtr modes = Marshal.AllocHGlobal(checked((int)modeCount * DisplayConfigModeInfoSize));

                try
                {
                    if (QueryDisplayConfig(QueryDisplayConfigOnlyActivePaths, ref pathCount, paths,
                        ref modeCount, modes, IntPtr.Zero) != 0)
                    {
                        return 1f;
                    }

                    for (int i = 0; i < pathCount; i++)
                    {
                        DisplayConfigSourceDeviceName sourceName = new DisplayConfigSourceDeviceName
                        {
                            Header = new DisplayConfigDeviceInfoHeader
                            {
                                Type = DisplayConfigGetSourceName,
                                Size = (uint)Marshal.SizeOf<DisplayConfigSourceDeviceName>(),
                                AdapterId = paths[i].SourceInfo.AdapterId,
                                Id = paths[i].SourceInfo.Id
                            },
                            ViewGdiDeviceName = string.Empty
                        };

                        if (DisplayConfigGetDeviceInfo(ref sourceName) != 0 ||
                            !string.Equals(sourceName.ViewGdiDeviceName, deviceName, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        DisplayConfigSdrWhiteLevel whiteLevel = new DisplayConfigSdrWhiteLevel
                        {
                            Header = new DisplayConfigDeviceInfoHeader
                            {
                                Type = DisplayConfigGetSdrWhiteLevel,
                                Size = (uint)Marshal.SizeOf<DisplayConfigSdrWhiteLevel>(),
                                AdapterId = paths[i].TargetInfo.AdapterId,
                                Id = paths[i].TargetInfo.Id
                            }
                        };

                        if (DisplayConfigGetDeviceInfo(ref whiteLevel) == 0 && whiteLevel.SdrWhiteLevel > 0)
                        {
                            return Math.Clamp(whiteLevel.SdrWhiteLevel / 1000f, 1f, 125f);
                        }
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(modes);
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e, "Failed to query the HDR display SDR white level.");
            }

            return 1f;
        }

        [DllImport("user32.dll")]
        private static extern int GetDisplayConfigBufferSizes(uint flags, out uint pathCount, out uint modeCount);

        [DllImport("user32.dll")]
        private static extern int QueryDisplayConfig(uint flags, ref uint pathCount,
            [Out] DisplayConfigPathInfo[] paths, ref uint modeCount, IntPtr modes, IntPtr currentTopologyId);

        [DllImport("user32.dll", EntryPoint = "DisplayConfigGetDeviceInfo", CharSet = CharSet.Unicode)]
        private static extern int DisplayConfigGetDeviceInfo(ref DisplayConfigSourceDeviceName requestPacket);

        [DllImport("user32.dll", EntryPoint = "DisplayConfigGetDeviceInfo")]
        private static extern int DisplayConfigGetDeviceInfo(ref DisplayConfigSdrWhiteLevel requestPacket);

        [StructLayout(LayoutKind.Sequential)]
        private struct Luid
        {
            public uint LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigPathSourceInfo
        {
            public Luid AdapterId;
            public uint Id;
            public uint ModeInfoIdx;
            public uint StatusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigRational
        {
            public uint Numerator;
            public uint Denominator;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigPathTargetInfo
        {
            public Luid AdapterId;
            public uint Id;
            public uint ModeInfoIdx;
            public uint OutputTechnology;
            public uint Rotation;
            public uint Scaling;
            public DisplayConfigRational RefreshRate;
            public uint ScanLineOrdering;
            public int TargetAvailable;
            public uint StatusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigPathInfo
        {
            public DisplayConfigPathSourceInfo SourceInfo;
            public DisplayConfigPathTargetInfo TargetInfo;
            public uint Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigDeviceInfoHeader
        {
            public uint Type;
            public uint Size;
            public Luid AdapterId;
            public uint Id;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DisplayConfigSourceDeviceName
        {
            public DisplayConfigDeviceInfoHeader Header;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string ViewGdiDeviceName;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigSdrWhiteLevel
        {
            public DisplayConfigDeviceInfoHeader Header;
            public uint SdrWhiteLevel;
        }
    }
}

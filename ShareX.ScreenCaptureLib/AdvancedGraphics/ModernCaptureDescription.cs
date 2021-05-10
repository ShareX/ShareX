using System;
using System.Collections.Generic;
using System.Drawing;
using ShareX.ScreenCaptureLib.AdvancedGraphics.GDI;

namespace ShareX.ScreenCaptureLib.AdvancedGraphics
{
    public class ModernCaptureMonitorDescription
    {
        // For GDI use
        public Rectangle DestGdiRect { get; set; }
        public MonitorInfo MonitorInfo { get; set; }

        // For DX use
        public ShaderHdrMetadata HdrMetadata { get; set; }
        public SharpDX.Vector2 DestD3DVsTopLeft { get; set; }
        public SharpDX.Vector2 DestD3DVsBottomRight { get; set; }
        public SharpDX.Vector2 DestD3DPsSamplerTopLeft { get; set; }
        public SharpDX.Vector2 DestD3DPsSamplerBottomRight { get; set; }

        // For WinRT use
        public bool CaptureCursor { get; set; }

        public ModernCaptureMonitorDescription()
        {

        }

        public ModernCaptureMonitorDescription(ModernCaptureMonitorDescription d)
        {
            this.DestGdiRect = d.DestGdiRect;
            this.MonitorInfo = d.MonitorInfo;
            this.HdrMetadata = d.HdrMetadata;
            this.DestD3DVsTopLeft = d.DestD3DVsTopLeft;
            this.DestD3DVsBottomRight = d.DestD3DVsBottomRight;
            this.DestD3DPsSamplerTopLeft = d.DestD3DPsSamplerTopLeft;
            this.DestD3DPsSamplerBottomRight = d.DestD3DPsSamplerBottomRight;
            this.CaptureCursor = d.CaptureCursor;
        }
    }

    public class ModernCaptureItemDescription
    {
        public List<ModernCaptureMonitorDescription> Regions { get; private set; }
        public Rectangle CanvasRect { get; private set; }
        // Maps to (0,0) in DirectX coordinate system
        public double CanvasMidPointX { get; set; }
        public double CanvasMidPointY { get; set; }

        private static void SamplerBoundCheck(float input)
        {
            if (input < 0 || input > 1) throw new InvalidCastException("Sampler region out of bound");
        }

        public ModernCaptureItemDescription(Rectangle canvas, List<ModernCaptureMonitorDescription> monRegions)
        {
            CanvasRect = canvas;
            Regions = monRegions;

            // Calculate (0, 0) location
            CanvasMidPointX = CanvasRect.X + (CanvasRect.Width / 2.0);
            CanvasMidPointY = CanvasRect.Y + (CanvasRect.Height / 2.0);

            var widthHalf = CanvasRect.Width / 2.0;
            var heightHalf = CanvasRect.Height / 2.0;

            foreach (var region in Regions)
            {
                // Calculate Vertex Shader Location, reference coordinate system is the DX canvas center (0, 0)
                var vtlX = (region.DestGdiRect.X - CanvasMidPointX) / widthHalf;
                var vtlY = (CanvasMidPointY - region.DestGdiRect.Y) / heightHalf;
                var vbrX = (region.DestGdiRect.X + region.DestGdiRect.Width - CanvasMidPointX) / widthHalf;
                var vbrY = (CanvasMidPointY - region.DestGdiRect.Y - region.DestGdiRect.Height) / heightHalf;

                region.DestD3DVsTopLeft = new SharpDX.Vector2((float) vtlX, (float) vtlY);
                region.DestD3DVsBottomRight = new SharpDX.Vector2((float)vbrX, (float)vbrY);

                // Calculate Pixel Shader Location, reference coordinate system is the top-left (X/Y) of this screen as (0, 0)
                var ptlX = ((double) (region.DestGdiRect.X - region.MonitorInfo.MonitorArea.X)) / region.MonitorInfo.MonitorArea.Width;
                var ptlY = ((double) (region.DestGdiRect.Y - region.MonitorInfo.MonitorArea.Y)) / region.MonitorInfo.MonitorArea.Height;
                var pbrX = ((double) (region.DestGdiRect.X + region.DestGdiRect.Width - region.MonitorInfo.MonitorArea.X)) / region.MonitorInfo.MonitorArea.Width;
                var pbrY = ((double) (region.DestGdiRect.Y + region.DestGdiRect.Height - region.MonitorInfo.MonitorArea.Y)) / region.MonitorInfo.MonitorArea.Height;

                region.DestD3DPsSamplerTopLeft = new SharpDX.Vector2((float) ptlX, (float) ptlY);
                region.DestD3DPsSamplerBottomRight = new SharpDX.Vector2((float) pbrX, (float) pbrY);

                // Make sure they are not outbound
                SamplerBoundCheck(region.DestD3DPsSamplerTopLeft.X);
                SamplerBoundCheck(region.DestD3DPsSamplerTopLeft.Y);
                SamplerBoundCheck(region.DestD3DPsSamplerBottomRight.X);
                SamplerBoundCheck(region.DestD3DPsSamplerBottomRight.Y);
            }
        }
    }
}

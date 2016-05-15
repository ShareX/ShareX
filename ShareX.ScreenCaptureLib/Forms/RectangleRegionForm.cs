#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using ShareX.ScreenCaptureLib.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class RectangleRegionForm : BaseRegionForm
    {
        public RectangleRegionMode Mode { get; private set; }

        public ShapeManager ShapeManager { get; private set; }

        public Point CurrentPosition { get; private set; }

        public Color CurrentColor
        {
            get
            {
                if (bmpBackgroundImage != null && !CurrentPosition.IsEmpty)
                {
                    Point position = CaptureHelpers.ScreenToClient(CurrentPosition);

                    return bmpBackgroundImage.GetPixel(position.X, position.Y);
                }

                return Color.Empty;
            }
        }

        public SimpleWindowInfo SelectedWindow { get; private set; }

        private ColorBlinkAnimation colorBlinkAnimation = new ColorBlinkAnimation();
        private Bitmap bmpBackgroundImage;

        public RectangleRegionForm(RectangleRegionMode mode)
        {
            Mode = mode;

            KeyDown += RectangleRegion_KeyDown;
            MouseDown += RectangleRegion_MouseDown;
        }

        private void RectangleRegion_MouseDown(object sender, MouseEventArgs e)
        {
            if ((Mode == RectangleRegionMode.OneClick || Mode == RectangleRegionMode.ScreenColorPicker) && e.Button == MouseButtons.Left)
            {
                CurrentPosition = InputManager.MousePosition;

                if (Mode == RectangleRegionMode.OneClick)
                {
                    SelectedWindow = ShapeManager.FindSelectedWindow();
                }

                Close(RegionResult.Region);
            }
        }

        private void RectangleRegion_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F1:
                    Config.ShowTips = !Config.ShowTips;
                    break;
                case Keys.Control | Keys.C:
                    CopyAreaInfo();
                    break;
            }
        }

        private void CopyAreaInfo()
        {
            string clipboardText;

            if (ShapeManager.IsCurrentRegionValid)
            {
                clipboardText = GetAreaText(ShapeManager.CurrentRectangle);
            }
            else
            {
                CurrentPosition = InputManager.MousePosition;
                clipboardText = GetInfoText();
            }

            ClipboardHelpers.CopyText(clipboardText);
        }

        public override void Prepare()
        {
            base.Prepare();

            if (Config != null)
            {
                ShapeManager = new ShapeManager(this);
                ShapeManager.WindowCaptureMode = Config.DetectWindows;
                ShapeManager.IncludeControls = Config.DetectControls;

                if (Mode == RectangleRegionMode.OneClick || ShapeManager.WindowCaptureMode)
                {
                    IntPtr handle = Handle;

                    TaskEx.Run(() =>
                    {
                        WindowsRectangleList wla = new WindowsRectangleList();
                        wla.IgnoreHandle = handle;
                        wla.IncludeChildWindows = ShapeManager.IncludeControls;
                        ShapeManager.Windows = wla.GetWindowInfoListAsync(5000);
                    });
                }

                if (Config.UseCustomInfoText || Mode == RectangleRegionMode.ScreenColorPicker)
                {
                    bmpBackgroundImage = new Bitmap(backgroundImage);
                }
            }
        }

        public override WindowInfo GetWindowInfo()
        {
            return ShapeManager.FindSelectedWindowInfo(CurrentPosition);
        }

        protected override void Update()
        {
            base.Update();
            ShapeManager.Update();
        }

        protected override void Draw(Graphics g)
        {
            // Draw snap rectangles
            if (ShapeManager.IsCreating && ShapeManager.IsSnapResizing)
            {
                foreach (Size size in Config.SnapSizes)
                {
                    Rectangle snapRect = CaptureHelpers.CalculateNewRectangle(ShapeManager.PositionOnClick, ShapeManager.CurrentPosition, size);
                    g.DrawRectangleProper(markerPen, snapRect);
                }
            }

            List<BaseShape> areas = ShapeManager.ValidRegions.ToList();

            if (areas.Count > 0)
            {
                // Create graphics path from all regions
                UpdateRegionPath();

                // If background is dimmed then draw non dimmed background to region selections
                if (Config.UseDimming)
                {
                    using (Region region = new Region(regionDrawPath))
                    {
                        g.Clip = region;
                        g.FillRectangle(lightBackgroundBrush, ScreenRectangle0Based);
                        g.ResetClip();
                    }
                }

                // Blink borders of all regions slightly to make non active regions to be visible in both dark and light backgrounds
                using (Pen blinkBorderPen = new Pen(colorBlinkAnimation.GetColor()))
                {
                    g.DrawPath(blinkBorderPen, regionDrawPath);
                }
            }

            // Draw effect shapes
            foreach (BaseEffectShape effectShape in ShapeManager.EffectShapes)
            {
                effectShape.Draw(g);
            }

            // Draw drawing shapes
            foreach (BaseDrawingShape drawingShape in ShapeManager.DrawingShapes)
            {
                drawingShape.Draw(g);
            }

            // Draw animated rectangle on hover area
            if (ShapeManager.IsCurrentHoverAreaValid)
            {
                using (GraphicsPath hoverDrawPath = new GraphicsPath { FillMode = FillMode.Winding })
                {
                    ShapeManager.CreateRegionShape(ShapeManager.CurrentHoverRectangle).AddShapePath(hoverDrawPath, -1);

                    g.DrawPath(borderPen, hoverDrawPath);
                    g.DrawPath(borderDotPen, hoverDrawPath);
                }
            }

            // Draw animated rectangle on selection area
            if (ShapeManager.IsCurrentShapeTypeRegion && ShapeManager.IsCurrentRegionValid)
            {
                g.DrawRectangleProper(borderPen, ShapeManager.CurrentRectangle);
                g.DrawRectangleProper(borderDotPen, ShapeManager.CurrentRectangle);

                if (Mode == RectangleRegionMode.Ruler)
                {
                    DrawRuler(g, ShapeManager.CurrentRectangle, borderPen, 5, 10);
                    DrawRuler(g, ShapeManager.CurrentRectangle, borderPen, 15, 100);

                    Point centerPos = new Point(ShapeManager.CurrentRectangle.X + ShapeManager.CurrentRectangle.Width / 2, ShapeManager.CurrentRectangle.Y + ShapeManager.CurrentRectangle.Height / 2);
                    int markSize = 10;
                    g.DrawLine(borderPen, centerPos.X, centerPos.Y - markSize, centerPos.X, centerPos.Y + markSize);
                    g.DrawLine(borderPen, centerPos.X - markSize, centerPos.Y, centerPos.X + markSize, centerPos.Y);
                }
            }

            // Draw all regions rectangle info
            if (Config.ShowInfo)
            {
                // Add hover area to list so rectangle info can be shown
                if (ShapeManager.IsCurrentShapeTypeRegion && ShapeManager.IsCurrentHoverAreaValid && areas.All(area => area.Rectangle != ShapeManager.CurrentHoverRectangle))
                {
                    BaseShape shape = ShapeManager.CreateRegionShape(ShapeManager.CurrentHoverRectangle);
                    areas.Add(shape);
                }

                foreach (BaseShape regionInfo in areas)
                {
                    if (regionInfo.Rectangle.IsValid())
                    {
                        string areaText = GetAreaText(regionInfo.Rectangle);
                        DrawAreaText(g, areaText, regionInfo.Rectangle);
                    }
                }
            }

            // Draw resize nodes
            DrawObjects(g);

            // Draw F1 tips
            if (Config.ShowTips)
            {
                DrawTips(g);
            }

            // Draw right click menu tip
            if (Mode == RectangleRegionMode.Annotation && Config.ShowMenuTip)
            {
                DrawMenuTip(g);
            }

            // Draw magnifier
            if (Config.ShowMagnifier)
            {
                DrawMagnifier(g);
            }

            // Draw screen wide crosshair
            if (Config.ShowCrosshair)
            {
                DrawCrosshair(g);
            }
        }

        private void DrawInfoText(Graphics g, string text, Rectangle rect, Font font, int padding)
        {
            g.FillRectangle(textBackgroundBrush, rect.Offset(-2));
            g.DrawRectangleProper(textBackgroundPenBlack, rect.Offset(-1));
            g.DrawRectangleProper(textBackgroundPenWhite, rect);

            ImageHelpers.DrawTextWithShadow(g, text, rect.Offset(-padding).Location, font, Brushes.White, Brushes.Black);
        }

        private void DrawAreaText(Graphics g, string text, Rectangle area)
        {
            int offset = 5;
            int backgroundPadding = 3;
            Size textSize = g.MeasureString(text, infoFont).ToSize();
            Point textPos;

            if (area.Y - offset - textSize.Height - backgroundPadding * 2 < ScreenRectangle0Based.Y)
            {
                textPos = new Point(area.X + offset + backgroundPadding, area.Y + offset + backgroundPadding);
            }
            else
            {
                textPos = new Point(area.X + backgroundPadding, area.Y - offset - backgroundPadding - textSize.Height);
            }

            if (textPos.X + textSize.Width + backgroundPadding >= ScreenRectangle0Based.Width)
            {
                textPos.X = ScreenRectangle0Based.Width - textSize.Width - backgroundPadding;
            }

            Rectangle backgroundRect = new Rectangle(textPos.X - backgroundPadding, textPos.Y - backgroundPadding, textSize.Width + backgroundPadding * 2, textSize.Height + backgroundPadding * 2);

            DrawInfoText(g, text, backgroundRect, infoFont, backgroundPadding);
        }

        private void DrawTips(Graphics g)
        {
            StringBuilder sb = new StringBuilder();
            WriteTips(sb);
            string tipText = sb.ToString().Trim();

            Size textSize = g.MeasureString(tipText, infoFont).ToSize();
            int offset = 10;
            int padding = 10;
            int rectWidth = textSize.Width + padding * 2 + 2;
            int rectHeight = textSize.Height + padding * 2;
            Rectangle screenBounds = CaptureHelpers.GetActiveScreenBounds0Based();
            Rectangle textRectangle = new Rectangle(screenBounds.X + screenBounds.Width - rectWidth - offset, screenBounds.Y + offset, rectWidth, rectHeight);

            if (textRectangle.Offset(10).Contains(InputManager.MousePosition0Based))
            {
                textRectangle.Y = screenBounds.Height - rectHeight - offset;
            }

            DrawInfoText(g, tipText, textRectangle, infoFont, padding);
        }

        private void DrawMenuTip(Graphics g)
        {
            // TODO: Translate
            string tipText = "Tip: Right click to open options menu";
            Size textSize = g.MeasureString(tipText, infoFontMedium).ToSize();
            int offset = 10;
            int padding = 3;
            int rectWidth = textSize.Width + padding * 2;
            int rectHeight = textSize.Height + padding * 2;
            Rectangle screenBounds = CaptureHelpers.GetActiveScreenBounds0Based();
            Rectangle textRectangle = new Rectangle(screenBounds.X + (screenBounds.Width / 2) - (rectWidth / 2), screenBounds.Y + offset, rectWidth, rectHeight);
            DrawInfoText(g, tipText, textRectangle, infoFontMedium, padding);
        }

        protected virtual void WriteTips(StringBuilder sb)
        {
            sb.AppendLine(Resources.RectangleRegion_WriteTips__F1__Hide_tips);
            sb.AppendLine();

            if (ShapeManager.IsCreating)
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Insert__Stop_region_selection);
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Right_click__Cancel_region_selection);
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Esc__Cancel_capture);
            }
            else
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Hold_Left_click__Start_region_selection);
                // TODO: Translate
                sb.AppendLine("[Right click] Open options menu");
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Right_click___Esc__Cancel_capture);
            }

            if (!Config.QuickCrop && ShapeManager.Regions.Length > 0)
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Double_Left_click___Enter__Capture_regions);
            }

            sb.AppendLine();

            if ((!Config.QuickCrop || !ShapeManager.IsCurrentShapeTypeRegion) && ShapeManager.CurrentShape != null && !ShapeManager.IsCreating)
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Right_click_on_selection___Delete__Remove_region);
                sb.AppendLine(string.Format(Resources.RectangleRegion_WriteTips__Arrow_keys__Resize_selected_region_from__0_, ShapeManager.ResizeManager.IsBottomRightResizing ?
                    Resources.RectangleRegion_WriteTips_bottom_right : Resources.RectangleRegion_WriteTips_top_left));
                sb.AppendLine(string.Format(Resources.RectangleRegion_WriteTips__Tab__Swap_resize_anchor_to__0_, ShapeManager.ResizeManager.IsBottomRightResizing ?
                    Resources.RectangleRegion_WriteTips_top_left : Resources.RectangleRegion_WriteTips_bottom_right));
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Hold_Shift__Move_selected_region_instead_of_resizing);
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Hold_Ctrl__Resize___Move_faster);
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Hold_Left_click_on_selection__Move_region);
            }
            else
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Arrow_keys__Move_cursor_position);
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Ctrl___Arrow_keys__Move_cursor_position_faster);
            }

            if (ShapeManager.IsCreating)
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Hold_Shift__Proportional_resizing);
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Hold_Alt__Snap_resizing_to_preset_sizes);
            }

            if (ShapeManager.IsCurrentRegionValid)
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Ctrl___C__Copy_position_and_size);
            }
            else if (Config.UseCustomInfoText)
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Ctrl___C__Copy_info);
            }
            else
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Ctrl___C__Copy_position);
            }

            sb.AppendLine();

            sb.AppendLine(Resources.RectangleRegion_WriteTips__Space__Fullscreen_capture);
            sb.AppendLine(Resources.RectangleRegion_WriteTips__1__2__3_____0__Monitor_capture);
            sb.AppendLine(Resources.RectangleRegion_WriteTips_____Active_monitor_capture);

            sb.AppendLine();

            // TODO: Translate
            sb.AppendLine("[Mouse wheel] Change current tool");
            if (ShapeManager.CurrentShapeType == ShapeType.RegionRectangle) sb.Append("-> ");
            sb.AppendLine(string.Format("[{0}] {1}", "Numpad 0", ShapeType.RegionRectangle.GetLocalizedDescription()));
            if (ShapeManager.CurrentShapeType == ShapeType.RegionRoundedRectangle) sb.Append("-> ");
            sb.AppendLine(ShapeType.RegionRoundedRectangle.GetLocalizedDescription());
            if (ShapeManager.CurrentShapeType == ShapeType.RegionEllipse) sb.Append("-> ");
            sb.AppendLine(ShapeType.RegionEllipse.GetLocalizedDescription());
            if (ShapeManager.CurrentShapeType == ShapeType.DrawingRectangle) sb.Append("-> ");
            sb.AppendLine(string.Format("[{0}] {1}", "Numpad 1", ShapeType.DrawingRectangle.GetLocalizedDescription()));
            if (ShapeManager.CurrentShapeType == ShapeType.DrawingRoundedRectangle) sb.Append("-> ");
            sb.AppendLine(string.Format("[{0}] {1}", "Numpad 2", ShapeType.DrawingRoundedRectangle.GetLocalizedDescription()));
            if (ShapeManager.CurrentShapeType == ShapeType.DrawingEllipse) sb.Append("-> ");
            sb.AppendLine(string.Format("[{0}] {1}", "Numpad 3", ShapeType.DrawingEllipse.GetLocalizedDescription()));
            if (ShapeManager.CurrentShapeType == ShapeType.DrawingLine) sb.Append("-> ");
            sb.AppendLine(string.Format("[{0}] {1}", "Numpad 4", ShapeType.DrawingLine.GetLocalizedDescription()));
            if (ShapeManager.CurrentShapeType == ShapeType.DrawingArrow) sb.Append("-> ");
            sb.AppendLine(string.Format("[{0}] {1}", "Numpad 5", ShapeType.DrawingArrow.GetLocalizedDescription()));
            if (ShapeManager.CurrentShapeType == ShapeType.DrawingBlur) sb.Append("-> ");
            sb.AppendLine(string.Format("[{0}] {1}", "Numpad 6", ShapeType.DrawingBlur.GetLocalizedDescription()));
            if (ShapeManager.CurrentShapeType == ShapeType.DrawingPixelate) sb.Append("-> ");
            sb.AppendLine(string.Format("[{0}] {1}", "Numpad 7", ShapeType.DrawingPixelate.GetLocalizedDescription()));
            if (ShapeManager.CurrentShapeType == ShapeType.DrawingHighlight) sb.Append("-> ");
            sb.AppendLine(string.Format("[{0}] {1}", "Numpad 8", ShapeType.DrawingHighlight.GetLocalizedDescription()));
        }

        private string GetAreaText(Rectangle area)
        {
            if (Mode == RectangleRegionMode.Ruler)
            {
                Point endPos = new Point(area.Right - 1, area.Bottom - 1);
                return string.Format(Resources.RectangleRegion_GetRulerText_Ruler_info, area.X, area.Y, endPos.X, endPos.Y,
                    area.Width, area.Height, MathHelpers.Distance(area.Location, endPos), MathHelpers.LookAtDegree(area.Location, endPos));
            }

            return string.Format(Resources.RectangleRegion_GetAreaText_Area, area.X, area.Y, area.Width, area.Height);
        }

        private string GetInfoText()
        {
            if (Mode == RectangleRegionMode.ScreenColorPicker || Config.UseCustomInfoText)
            {
                Color color = CurrentColor;

                if (Mode != RectangleRegionMode.ScreenColorPicker && !string.IsNullOrEmpty(Config.CustomInfoText))
                {
                    return Config.CustomInfoText.Replace("$r", color.R.ToString(), StringComparison.InvariantCultureIgnoreCase).
                        Replace("$g", color.G.ToString(), StringComparison.InvariantCultureIgnoreCase).
                        Replace("$b", color.B.ToString(), StringComparison.InvariantCultureIgnoreCase).
                        Replace("$hex", ColorHelpers.ColorToHex(color), StringComparison.InvariantCultureIgnoreCase).
                        Replace("$x", CurrentPosition.X.ToString(), StringComparison.InvariantCultureIgnoreCase).
                        Replace("$y", CurrentPosition.Y.ToString(), StringComparison.InvariantCultureIgnoreCase);
                }

                return string.Format(Resources.RectangleRegion_GetColorPickerText, color.R, color.G, color.B, ColorHelpers.ColorToHex(color), CurrentPosition.X, CurrentPosition.Y);
            }

            return string.Format("X: {0} Y: {1}", CurrentPosition.X, CurrentPosition.Y);
        }

        private void DrawCrosshair(Graphics g)
        {
            int offset = 5;
            Point mousePos = InputManager.MousePosition0Based;
            Point left = new Point(mousePos.X - offset, mousePos.Y), left2 = new Point(0, mousePos.Y);
            Point right = new Point(mousePos.X + offset, mousePos.Y), right2 = new Point(ScreenRectangle0Based.Width - 1, mousePos.Y);
            Point top = new Point(mousePos.X, mousePos.Y - offset), top2 = new Point(mousePos.X, 0);
            Point bottom = new Point(mousePos.X, mousePos.Y + offset), bottom2 = new Point(mousePos.X, ScreenRectangle0Based.Height - 1);

            if (left.X - left2.X > 10)
            {
                g.DrawLine(borderPen, left, left2);
                g.DrawLine(borderDotPen, left, left2);
            }

            if (right2.X - right.X > 10)
            {
                g.DrawLine(borderPen, right, right2);
                g.DrawLine(borderDotPen, right, right2);
            }

            if (top.Y - top2.Y > 10)
            {
                g.DrawLine(borderPen, top, top2);
                g.DrawLine(borderDotPen, top, top2);
            }

            if (bottom2.Y - bottom.Y > 10)
            {
                g.DrawLine(borderPen, bottom, bottom2);
                g.DrawLine(borderDotPen, bottom, bottom2);
            }
        }

        private void DrawMagnifier(Graphics g)
        {
            Point mousePos = InputManager.MousePosition0Based;
            Rectangle currentScreenRect0Based = CaptureHelpers.ScreenToClient(Screen.FromPoint(InputManager.MousePosition).Bounds);
            int offsetX = 10, offsetY = 10, infoTextOffset = 0, infoTextPadding = 3;
            Rectangle infoTextRect = Rectangle.Empty;
            string infoText = string.Empty;

            if (Config.ShowInfo)
            {
                infoTextOffset = 10;

                CurrentPosition = InputManager.MousePosition;

                infoText = GetInfoText();
                Size textSize = g.MeasureString(infoText, infoFont).ToSize();
                infoTextRect.Size = new Size(textSize.Width + infoTextPadding * 2, textSize.Height + infoTextPadding * 2);
            }

            using (Bitmap magnifier = Magnifier(backgroundImage, mousePos, Config.MagnifierPixelCount, Config.MagnifierPixelCount, Config.MagnifierPixelSize))
            {
                int x = mousePos.X + offsetX;

                if (x + magnifier.Width > currentScreenRect0Based.Right)
                {
                    x = mousePos.X - offsetX - magnifier.Width;
                }

                int y = mousePos.Y + offsetY;

                if (y + magnifier.Height + infoTextOffset + infoTextRect.Height > currentScreenRect0Based.Bottom)
                {
                    y = mousePos.Y - offsetY - magnifier.Height - infoTextOffset - infoTextRect.Height;
                }

                if (Config.ShowInfo)
                {
                    infoTextRect.Location = new Point(x + (magnifier.Width / 2) - (infoTextRect.Width / 2), y + magnifier.Height + infoTextOffset);
                    DrawInfoText(g, infoText, infoTextRect, infoFont, infoTextPadding);
                }

                g.SetHighQuality();

                using (TextureBrush brush = new TextureBrush(magnifier))
                {
                    brush.TranslateTransform(x, y);

                    if (Config.UseSquareMagnifier)
                    {
                        g.FillRectangle(brush, x, y, magnifier.Width, magnifier.Height);
                        g.DrawRectangleProper(Pens.White, x - 1, y - 1, magnifier.Width + 2, magnifier.Height + 2);
                        g.DrawRectangleProper(Pens.Black, x, y, magnifier.Width, magnifier.Height);
                    }
                    else
                    {
                        g.FillEllipse(brush, x, y, magnifier.Width, magnifier.Height);
                        g.DrawEllipse(Pens.White, x - 1, y - 1, magnifier.Width + 2, magnifier.Height + 2);
                        g.DrawEllipse(Pens.Black, x, y, magnifier.Width, magnifier.Height);
                    }
                }
            }
        }

        private Bitmap Magnifier(Image img, Point position, int horizontalPixelCount, int verticalPixelCount, int pixelSize)
        {
            horizontalPixelCount = (horizontalPixelCount | 1).Between(1, 101);
            verticalPixelCount = (verticalPixelCount | 1).Between(1, 101);
            pixelSize = pixelSize.Between(1, 1000);

            if (horizontalPixelCount * pixelSize > ScreenRectangle0Based.Width || verticalPixelCount * pixelSize > ScreenRectangle0Based.Height)
            {
                horizontalPixelCount = verticalPixelCount = 15;
                pixelSize = 10;
            }

            int width = horizontalPixelCount * pixelSize;
            int height = verticalPixelCount * pixelSize;
            Bitmap bmp = new Bitmap(width - 1, height - 1);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;

                g.DrawImage(img, new Rectangle(0, 0, width, height), new Rectangle(position.X - horizontalPixelCount / 2, position.Y - verticalPixelCount / 2,
                    horizontalPixelCount, verticalPixelCount), GraphicsUnit.Pixel);

                g.PixelOffsetMode = PixelOffsetMode.None;

                using (SolidBrush crosshairBrush = new SolidBrush(Color.FromArgb(125, Color.LightBlue)))
                {
                    g.FillRectangle(crosshairBrush, new Rectangle(0, (height - pixelSize) / 2, (width - pixelSize) / 2, pixelSize)); // Left
                    g.FillRectangle(crosshairBrush, new Rectangle((width + pixelSize) / 2, (height - pixelSize) / 2, (width - pixelSize) / 2, pixelSize)); // Right
                    g.FillRectangle(crosshairBrush, new Rectangle((width - pixelSize) / 2, 0, pixelSize, (height - pixelSize) / 2)); // Top
                    g.FillRectangle(crosshairBrush, new Rectangle((width - pixelSize) / 2, (height + pixelSize) / 2, pixelSize, (height - pixelSize) / 2)); // Bottom
                }

                using (Pen pen = new Pen(Color.FromArgb(75, Color.Black)))
                {
                    for (int x = 1; x < horizontalPixelCount; x++)
                    {
                        g.DrawLine(pen, new Point(x * pixelSize - 1, 0), new Point(x * pixelSize - 1, height - 1));
                    }

                    for (int y = 1; y < verticalPixelCount; y++)
                    {
                        g.DrawLine(pen, new Point(0, y * pixelSize - 1), new Point(width - 1, y * pixelSize - 1));
                    }
                }

                g.DrawRectangle(Pens.Black, (width - pixelSize) / 2 - 1, (height - pixelSize) / 2 - 1, pixelSize, pixelSize);
                g.DrawRectangle(Pens.White, (width - pixelSize) / 2, (height - pixelSize) / 2, pixelSize - 2, pixelSize - 2);
            }

            return bmp;
        }

        private void DrawRuler(Graphics g, Rectangle rect, Pen pen, int rulerSize, int rulerWidth)
        {
            if (rect.Width >= rulerSize && rect.Height >= rulerSize)
            {
                for (int x = 1; x <= rect.Width / rulerWidth; x++)
                {
                    g.DrawLine(pen, new Point(rect.X + x * rulerWidth, rect.Y), new Point(rect.X + x * rulerWidth, rect.Y + rulerSize));
                    g.DrawLine(pen, new Point(rect.X + x * rulerWidth, rect.Bottom), new Point(rect.X + x * rulerWidth, rect.Bottom - rulerSize));
                }

                for (int y = 1; y <= rect.Height / rulerWidth; y++)
                {
                    g.DrawLine(pen, new Point(rect.X, rect.Y + y * rulerWidth), new Point(rect.X + rulerSize, rect.Y + y * rulerWidth));
                    g.DrawLine(pen, new Point(rect.Right, rect.Y + y * rulerWidth), new Point(rect.Right - rulerSize, rect.Y + y * rulerWidth));
                }
            }
        }

        public void UpdateRegionPath()
        {
            if (regionFillPath != null)
            {
                regionFillPath.Dispose();
                regionFillPath = null;
            }

            if (regionDrawPath != null)
            {
                regionDrawPath.Dispose();
                regionDrawPath = null;
            }

            BaseShape[] areas = ShapeManager.ValidRegions;

            if (areas != null && areas.Length > 0)
            {
                regionFillPath = new GraphicsPath { FillMode = FillMode.Winding };
                regionDrawPath = new GraphicsPath { FillMode = FillMode.Winding };

                foreach (BaseShape regionShape in ShapeManager.ValidRegions)
                {
                    regionShape.AddShapePath(regionFillPath);
                    regionShape.AddShapePath(regionDrawPath, -1);
                }
            }
        }

        protected override Image GetOutputImage()
        {
            return ShapeManager.RenderOutputImage(backgroundImage);
        }

        protected override void Dispose(bool disposing)
        {
            if (bmpBackgroundImage != null)
            {
                bmpBackgroundImage.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
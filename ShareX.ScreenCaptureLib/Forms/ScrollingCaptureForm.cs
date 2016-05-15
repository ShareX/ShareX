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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class ScrollingCaptureForm : Form
    {
        public event Action<Image> ImageProcessRequested;

        public ScrollingCaptureOptions Options { get; set; }
        public Image Result { get; set; }

        private WindowInfo selectedWindow;
        private Rectangle selectedRectangle;
        private List<Image> images = new List<Image>();
        private int currentScrollCount;
        private bool isBusy, isCapturing, firstCapture, detectingScrollMethod;
        private ScrollingCaptureScrollMethod currentScrollMethod;

        public ScrollingCaptureForm(ScrollingCaptureOptions options, bool forceSelection = false)
        {
            Options = options;

            InitializeComponent();
            Icon = ShareXResources.Icon;

            cbScrollMethod.Items.AddRange(Helpers.GetEnumDescriptions<ScrollingCaptureScrollMethod>());
            cbScrollMethod.SelectedIndex = (int)Options.ScrollMethod;
            cbScrollTopMethodBeforeCapture.Items.AddRange(Helpers.GetEnumDescriptions<ScrollingCaptureScrollTopMethod>());
            cbScrollTopMethodBeforeCapture.SelectedIndex = (int)Options.ScrollTopMethodBeforeCapture;
            nudStartDelay.SetValue(Options.StartDelay);
            nudScrollDelay.SetValue(Options.ScrollDelay);
            nudMaximumScrollCount.SetValue(Options.MaximumScrollCount);
            cbStartSelectionAutomatically.Checked = Options.StartSelectionAutomatically;
            cbStartCaptureAutomatically.Checked = Options.StartCaptureAutomatically;
            cbAutoDetectScrollEnd.Checked = Options.AutoDetectScrollEnd;
            cbRemoveDuplicates.Checked = Options.RemoveDuplicates;
            cbAutoCombine.Checked = Options.AfterCaptureAutomaticallyCombine;
            chkAutoUpload.Checked = Options.AutoUpload;

            if (forceSelection || Options.StartSelectionAutomatically)
            {
                if (Options.StartCaptureAutomatically)
                {
                    WindowState = FormWindowState.Minimized;
                }

                SelectHandle();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                Clean();
            }

            base.Dispose(disposing);
        }

        protected void OnImageProcessRequested(Image img)
        {
            if (ImageProcessRequested != null)
            {
                ImageProcessRequested(img);
            }
        }

        private void btnSelectHandle_Click(object sender, EventArgs e)
        {
            SelectHandle();
        }

        private void btnSelectRectangle_Click(object sender, EventArgs e)
        {
            SelectRectangle();
        }

        private void SelectHandle()
        {
            WindowState = FormWindowState.Minimized;

            bool capturing = false;

            try
            {
                Thread.Sleep(250);

                SimpleWindowInfo simpleWindowInfo = RegionCaptureHelpers.GetWindowInfo();

                if (simpleWindowInfo != null)
                {
                    selectedWindow = new WindowInfo(simpleWindowInfo.Handle);
                    lblControlText.Text = selectedWindow.ClassName ?? string.Empty;
                    selectedRectangle = simpleWindowInfo.Rectangle;
                    lblSelectedRectangle.Text = selectedRectangle.ToString();
                    btnSelectRectangle.Enabled = btnCapture.Enabled = true;

                    if (Options.StartCaptureAutomatically)
                    {
                        capturing = true;
                        StartCapture();
                    }
                }
                else
                {
                    btnCapture.Enabled = false;
                }
            }
            finally
            {
                if (!capturing) this.ForceActivate();
            }
        }

        private void SelectRectangle()
        {
            WindowState = FormWindowState.Minimized;

            try
            {
                Thread.Sleep(250);

                Rectangle rect;

                if (RegionCaptureHelpers.GetRectangleRegion(out rect))
                {
                    selectedRectangle = rect;
                    lblSelectedRectangle.Text = selectedRectangle.ToString();
                }
            }
            finally
            {
                this.ForceActivate();
            }
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            if (isCapturing)
            {
                StopCapture();
            }
            else
            {
                StartCapture();
            }
        }

        private void StartCapture()
        {
            isCapturing = true;
            btnCapture.Text = Resources.ScrollingCaptureForm_StartCapture_Stop_capture;
            WindowState = FormWindowState.Minimized;
            firstCapture = true;

            if (Options.ScrollMethod == ScrollingCaptureScrollMethod.Automatic)
            {
                currentScrollMethod = ScrollingCaptureScrollMethod.SendMessageScroll;
            }
            else
            {
                currentScrollMethod = Options.ScrollMethod;
            }

            detectingScrollMethod = true;
            Clean();
            selectedWindow.Activate();
            captureTimer.Interval = Options.StartDelay;
            captureTimer.Start();
        }

        private void StopCapture()
        {
            captureTimer.Stop();
            btnCapture.Text = Resources.ScrollingCaptureForm_StopCapture_Start_capture;
            if (!Options.AutoUpload) this.ForceActivate();
            tcScrollingCapture.SelectedTab = tpOutput;
            StartingProcess();
            if (Options.RemoveDuplicates) RemoveDuplicates();
            txtImagesCount.Text = images.Count.ToString();
            btnGuessEdges.Enabled = btnGuessCombineAdjustments.Enabled = images.Count > 1;
            btnStartTask.Enabled = btnResetCombine.Enabled = images.Count > 0;
            nudIgnoreLast.Maximum = images.Count > 1 ? images.Count - 1 : 0;
            ResetOffsets();
            if (Options.AfterCaptureAutomaticallyCombine)
            {
                GuessEdges();
                GuessCombineAdjustments();
            }
            CombineAndPreviewImages();

            EndingProcess();
            isCapturing = false;

            if (Options.AutoUpload) StartProcess();
            if (Options.AutoClose) Close();
        }

        private void Clean()
        {
            currentScrollCount = 0;

            CleanPictureBox();

            if (images != null)
            {
                foreach (Image image in images)
                {
                    if (image != null)
                    {
                        image.Dispose();
                    }
                }

                images.Clear();
            }

            if (Result != null)
            {
                Result.Dispose();
            }
        }

        private void CleanPictureBox(Image img = null)
        {
            Image temp = pbOutput.Image;

            pbOutput.Image = img;

            if (temp != null)
            {
                temp.Dispose();
            }
        }

        private void RemoveDuplicates()
        {
            if (images.Count > 1)
            {
                for (int i = images.Count - 1; i > 0; i--)
                {
                    bool result = ImageHelpers.IsImagesEqual((Bitmap)images[i], (Bitmap)images[i - 1]);

                    if (result)
                    {
                        Image img = images[i];
                        images.Remove(img);
                        img.Dispose();
                    }
                }
            }
        }

        private void captureTimer_Tick(object sender, EventArgs e)
        {
            if (firstCapture)
            {
                firstCapture = false;
                captureTimer.Interval = Options.ScrollDelay;

                if (Options.ScrollTopMethodBeforeCapture == ScrollingCaptureScrollTopMethod.All || Options.ScrollTopMethodBeforeCapture == ScrollingCaptureScrollTopMethod.KeyPressHome)
                {
                    InputHelpers.SendKeyPress(VirtualKeyCode.HOME);
                }

                if (Options.ScrollTopMethodBeforeCapture == ScrollingCaptureScrollTopMethod.All || Options.ScrollTopMethodBeforeCapture == ScrollingCaptureScrollTopMethod.SendMessageTop)
                {
                    NativeMethods.SendMessage(selectedWindow.Handle, (int)WindowsMessages.VSCROLL, (int)ScrollBarCommands.SB_TOP, 0);
                }

                if (Options.ScrollTopMethodBeforeCapture != ScrollingCaptureScrollTopMethod.None)
                {
                    return;
                }
            }

            Screenshot.CaptureCursor = false;
            Image image = Screenshot.CaptureRectangle(selectedRectangle);

            if (image != null)
            {
                images.Add(image);
            }

            if (Options.ScrollMethod == ScrollingCaptureScrollMethod.Automatic && detectingScrollMethod && images.Count > 1 && IsLastTwoImagesSame())
            {
                if (currentScrollMethod == ScrollingCaptureScrollMethod.SendMessageScroll)
                {
                    currentScrollMethod = ScrollingCaptureScrollMethod.KeyPressPageDown;
                }
                else if (currentScrollMethod == ScrollingCaptureScrollMethod.KeyPressPageDown)
                {
                    currentScrollMethod = ScrollingCaptureScrollMethod.MouseWheel;
                }
                else
                {
                    StopCapture();
                }
            }
            else if (currentScrollCount == Options.MaximumScrollCount || (Options.AutoDetectScrollEnd && IsScrollReachedBottom(selectedWindow.Handle)))
            {
                StopCapture();
            }
            else if (images.Count > 1)
            {
                detectingScrollMethod = false;
            }

            switch (currentScrollMethod)
            {
                case ScrollingCaptureScrollMethod.Automatic:
                case ScrollingCaptureScrollMethod.SendMessageScroll:
                    NativeMethods.SendMessage(selectedWindow.Handle, (int)WindowsMessages.VSCROLL, (int)ScrollBarCommands.SB_PAGEDOWN, 0);
                    break;
                case ScrollingCaptureScrollMethod.KeyPressPageDown:
                    InputHelpers.SendKeyPress(VirtualKeyCode.NEXT);
                    break;
                case ScrollingCaptureScrollMethod.MouseWheel:
                    InputHelpers.SendMouseWheel(-120);
                    break;
            }

            currentScrollCount++;
        }

        private void cbScrollMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.ScrollMethod = (ScrollingCaptureScrollMethod)cbScrollMethod.SelectedIndex;
        }

        private void cbScrollTopMethodBeforeCapture_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.ScrollTopMethodBeforeCapture = (ScrollingCaptureScrollTopMethod)cbScrollTopMethodBeforeCapture.SelectedIndex;
        }

        private void nudStartDelay_ValueChanged(object sender, EventArgs e)
        {
            Options.StartDelay = (int)nudStartDelay.Value;
        }

        private void nudScrollDelay_ValueChanged(object sender, EventArgs e)
        {
            Options.ScrollDelay = (int)nudScrollDelay.Value;
        }

        private void nudMaximumScrollCount_ValueChanged(object sender, EventArgs e)
        {
            Options.MaximumScrollCount = (int)nudMaximumScrollCount.Value;
        }

        private void cbStartSelectionAutomatically_CheckedChanged(object sender, EventArgs e)
        {
            Options.StartSelectionAutomatically = cbStartSelectionAutomatically.Checked;
        }

        private void cbStartCaptureAutomatically_CheckedChanged(object sender, EventArgs e)
        {
            Options.StartCaptureAutomatically = cbStartCaptureAutomatically.Checked;
        }

        private void cbAutoDetectScrollEnd_CheckedChanged(object sender, EventArgs e)
        {
            Options.AutoDetectScrollEnd = cbAutoDetectScrollEnd.Checked;
        }

        private void cbRemoveDuplicates_CheckedChanged(object sender, EventArgs e)
        {
            Options.RemoveDuplicates = cbRemoveDuplicates.Checked;
        }

        private void cbAutoCombine_CheckedChanged(object sender, EventArgs e)
        {
            Options.AfterCaptureAutomaticallyCombine = cbAutoCombine.Checked;
        }

        private bool IsScrollReachedBottom(IntPtr handle)
        {
            SCROLLINFO scrollInfo = new SCROLLINFO();
            scrollInfo.cbSize = (uint)Marshal.SizeOf(scrollInfo);
            scrollInfo.fMask = (uint)(ScrollInfoMask.SIF_RANGE | ScrollInfoMask.SIF_PAGE | ScrollInfoMask.SIF_TRACKPOS);

            if (NativeMethods.GetScrollInfo(handle, (int)SBOrientation.SB_VERT, ref scrollInfo))
            {
                return scrollInfo.nMax == scrollInfo.nTrackPos + scrollInfo.nPage - 1;
            }

            return IsLastTwoImagesSame();
        }

        private bool IsLastTwoImagesSame()
        {
            bool result = false;

            if (images.Count > 1)
            {
                result = ImageHelpers.IsImagesEqual((Bitmap)images[images.Count - 1], (Bitmap)images[images.Count - 2]);

                if (result)
                {
                    Image last = images[images.Count - 1];
                    images.Remove(last);
                    last.Dispose();
                }
            }

            return result;
        }

        private void nudTrimLeft_ValueChanged(object sender, EventArgs e)
        {
            Options.TrimLeftEdge = (int)nudTrimLeft.Value;
            CombineAndPreviewImagesFromControl();
        }

        private void nudTrimTop_ValueChanged(object sender, EventArgs e)
        {
            Options.TrimTopEdge = (int)nudTrimTop.Value;
            CombineAndPreviewImagesFromControl();
        }

        private void nudTrimRight_ValueChanged(object sender, EventArgs e)
        {
            Options.TrimRightEdge = (int)nudTrimRight.Value;
            CombineAndPreviewImagesFromControl();
        }

        private void nudTrimBottom_ValueChanged(object sender, EventArgs e)
        {
            Options.TrimBottomEdge = (int)nudTrimBottom.Value;
            CombineAndPreviewImagesFromControl();
        }

        private void nudCombineVertical_ValueChanged(object sender, EventArgs e)
        {
            Options.CombineAdjustmentVertical = (int)nudCombineVertical.Value;
            CombineAndPreviewImagesFromControl();
        }

        private void nudCombineLastVertical_ValueChanged(object sender, EventArgs e)
        {
            Options.CombineAdjustmentLastVertical = (int)nudCombineLastVertical.Value;
            CombineAndPreviewImagesFromControl();
        }

        private void nudIgnoreLast_ValueChanged(object sender, EventArgs e)
        {
            Options.IgnoreLast = (int)nudIgnoreLast.Value;
            txtImagesCount.Text = (images.Count - Options.IgnoreLast).ToString();
            CombineAndPreviewImagesFromControl();
        }

        private void btnGuessEdges_Click(object sender, EventArgs e)
        {
            StartingProcess();
            GuessEdges();
            CombineAndPreviewImages();
            EndingProcess();
        }

        private void btnGuessCombineAdjustments_Click(object sender, EventArgs e)
        {
            StartingProcess();
            GuessCombineAdjustments();
            CombineAndPreviewImages();
            EndingProcess();
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            StartProcess();
        }

        private void StartingProcess()
        {
            isBusy = true;
            CleanPictureBox();
            lblProcessing.Visible = true;
            Application.DoEvents();
        }

        private void StartProcess()
        {
            if (Result != null)
            {
                OnImageProcessRequested((Image)Result.Clone());
            }
        }

        private void EndingProcess()
        {
            lblProcessing.Visible = false;
            isBusy = false;
        }

        private void btnResetCombine_Click(object sender, EventArgs e)
        {
            StartingProcess();
            ResetOffsets();
            CombineAndPreviewImages();
            EndingProcess();
        }

        private void ResetOffsets()
        {
            nudTrimLeft.Value = nudTrimTop.Value = nudTrimRight.Value = nudTrimBottom.Value = nudCombineVertical.Value = nudCombineLastVertical.Value = nudIgnoreLast.Value = 0;
            Options.TrimLeftEdge = Options.TrimTopEdge = Options.TrimRightEdge = Options.TrimBottomEdge =
                Options.CombineAdjustmentVertical = Options.CombineAdjustmentLastVertical = Options.IgnoreLast = 0;
        }

        private void CombineAndPreviewImagesFromControl()
        {
            if (!isBusy)
            {
                Result = CombineImages();
                CleanPictureBox(Result);
            }
        }

        private void CombineAndPreviewImages()
        {
            Result = CombineImages();
            pbOutput.Image = Result;
        }

        private Image CombineImages()
        {
            if (images == null || images.Count == 0)
            {
                return null;
            }

            if (images.Count == 1)
            {
                return (Image)images[0].Clone();
            }

            List<Image> output = new List<Image>();

            for (int i = 0; i < images.Count - Options.IgnoreLast; i++)
            {
                Image newImage;
                Image image = images[i];

                if (Options.TrimLeftEdge > 0 || Options.TrimTopEdge > 0 || Options.TrimTopEdge > 0 || Options.TrimBottomEdge > 0 ||
                    Options.CombineAdjustmentVertical > 0 || Options.CombineAdjustmentLastVertical > 0)
                {
                    Rectangle rect = new Rectangle(Options.TrimLeftEdge, Options.TrimTopEdge, image.Width - Options.TrimLeftEdge - Options.TrimRightEdge,
                        image.Height - Options.TrimTopEdge - Options.TrimBottomEdge);

                    if (i == images.Count - 1)
                    {
                        rect.Y += Options.CombineAdjustmentLastVertical;
                        rect.Height -= Options.CombineAdjustmentLastVertical;
                    }
                    else if (i > 0)
                    {
                        rect.Y += Options.CombineAdjustmentVertical;
                        rect.Height -= Options.CombineAdjustmentVertical;
                    }

                    newImage = ImageHelpers.CropImage(image, rect);

                    if (newImage == null)
                    {
                        continue;
                    }
                }
                else
                {
                    newImage = (Image)image.Clone();
                }

                output.Add(newImage);
            }

            Image result = ImageHelpers.CombineImages(output);

            foreach (Image image in output)
            {
                if (image != null)
                {
                    image.Dispose();
                }
            }

            output.Clear();

            return result;
        }

        private void GuessEdges()
        {
            if (images.Count < 2) return;

            nudTrimLeft.Value = nudTrimTop.Value = nudTrimRight.Value = nudTrimBottom.Value = 0;

            Padding result = new Padding();

            for (int i = 0; i < images.Count - 1; i++)
            {
                Padding edges = GuessEdges(images[i], images[i + 1]);

                if (i == 0)
                {
                    result = edges;
                }
                else
                {
                    result.Left = Math.Min(result.Left, edges.Left);
                    result.Top = Math.Min(result.Top, edges.Top);
                    result.Right = Math.Min(result.Right, edges.Right);
                    result.Bottom = Math.Min(result.Bottom, edges.Bottom);
                }
            }

            nudTrimLeft.SetValue(result.Left);
            nudTrimTop.SetValue(result.Top);
            nudTrimRight.SetValue(result.Right);
            nudTrimBottom.SetValue(result.Bottom);
        }

        private void chkAutoUpload_CheckedChanged(object sender, EventArgs e)
        {
            Options.AutoUpload = chkAutoUpload.Checked;
        }

        private void cbAutoClose_CheckedChanged(object sender, EventArgs e)
        {
            Options.AutoClose = cbAutoClose.Checked;
        }

        private Padding GuessEdges(Image img1, Image img2)
        {
            Padding result = new Padding();
            Rectangle rect = new Rectangle(0, 0, img1.Width, img1.Height);

            using (UnsafeBitmap bmp1 = new UnsafeBitmap((Bitmap)img1, true, ImageLockMode.ReadOnly))
            using (UnsafeBitmap bmp2 = new UnsafeBitmap((Bitmap)img2, true, ImageLockMode.ReadOnly))
            {
                bool valueFound = false;

                // Left edge
                for (int x = rect.X; !valueFound && x < rect.Width; x++)
                {
                    for (int y = rect.Y; y < rect.Height; y++)
                    {
                        if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                        {
                            valueFound = true;
                            result.Left = x;
                            rect.X = x;
                            break;
                        }
                    }
                }

                valueFound = false;

                // Top edge
                for (int y = rect.Y; !valueFound && y < rect.Height; y++)
                {
                    for (int x = rect.X; x < rect.Width; x++)
                    {
                        if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                        {
                            valueFound = true;
                            result.Top = y;
                            rect.Y = y;
                            break;
                        }
                    }
                }

                valueFound = false;

                // Right edge
                for (int x = rect.Width - 1; !valueFound && x >= rect.X; x--)
                {
                    for (int y = rect.Y; y < rect.Height; y++)
                    {
                        if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                        {
                            valueFound = true;
                            result.Right = rect.Width - x - 1;
                            rect.Width = x + 1;
                            break;
                        }
                    }
                }

                valueFound = false;

                // Bottom edge
                for (int y = rect.Height - 1; !valueFound && y >= rect.X; y--)
                {
                    for (int x = rect.X; x < rect.Width; x++)
                    {
                        if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                        {
                            valueFound = true;
                            result.Bottom = rect.Height - y - 1;
                            rect.Height = y + 1;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        private void GuessCombineAdjustments()
        {
            if (images.Count > 1)
            {
                int vertical = 0;

                for (int i = 0; i < images.Count - 2; i++)
                {
                    int temp = CalculateVerticalOffset(images[i], images[i + 1]);
                    vertical = Math.Max(vertical, temp);
                }

                nudCombineVertical.SetValue(vertical);
                nudCombineLastVertical.SetValue(CalculateVerticalOffset(images[images.Count - 2], images[images.Count - 1]));
            }
        }

        private int CalculateVerticalOffset(Image img1, Image img2, int ignoreRightOffset = 50)
        {
            int lastMatchCount = 0;
            int lastMatchOffset = 0;

            Rectangle rect = new Rectangle(Options.TrimLeftEdge, Options.TrimTopEdge,
                img1.Width - Options.TrimLeftEdge - Options.TrimRightEdge - (img1.Width > ignoreRightOffset ? ignoreRightOffset : 0),
                img1.Height - Options.TrimTopEdge - Options.TrimBottomEdge);

            using (UnsafeBitmap bmp1 = new UnsafeBitmap((Bitmap)img1, true, ImageLockMode.ReadOnly))
            using (UnsafeBitmap bmp2 = new UnsafeBitmap((Bitmap)img2, true, ImageLockMode.ReadOnly))
            {
                for (int y = rect.Y; y < rect.Bottom; y++)
                {
                    bool isLineMatches = true;

                    for (int x = rect.X; x < rect.Right; x++)
                    {
                        if (bmp2.GetPixel(x, y) != bmp1.GetPixel(x, rect.Bottom - 1))
                        {
                            isLineMatches = false;
                            break;
                        }
                    }

                    if (isLineMatches)
                    {
                        int lineMatchesCount = 1;
                        int y3 = 2;

                        for (int y2 = y - 1; y2 >= rect.Y; y2--)
                        {
                            bool isLineMatches2 = true;

                            for (int x2 = rect.X; x2 < rect.Right; x2++)
                            {
                                if (bmp2.GetPixel(x2, y2) != bmp1.GetPixel(x2, rect.Bottom - y3))
                                {
                                    isLineMatches2 = false;
                                    break;
                                }
                            }

                            if (isLineMatches2)
                            {
                                lineMatchesCount++;
                                y3++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (lineMatchesCount > lastMatchCount)
                        {
                            lastMatchCount = lineMatchesCount;
                            lastMatchOffset = y - rect.Y + 1;
                        }
                    }
                }
            }

            return lastMatchOffset;
        }
    }
}
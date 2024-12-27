#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2024 ShareX Team

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
using ShareX.HelpersLib.Extensions;
using ShareX.HelpersLib.Helpers;
using ShareX.HelpersLib.Native;
using ShareX.Properties;
using ShareX.Tools.PinToScreen;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ShareX;

public partial class PinToScreenForm : Form
{
    private static readonly Lock syncLock = new();
    private static readonly List<PinToScreenForm> forms = [];

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Image Image { get; private set; }

    private int imageScale = 100;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ImageScale
    {
        get => imageScale;
        set
        {
            int newImageScale = value.Clamp(20, 500);

            if (imageScale != newImageScale)
            {
                imageScale = newImageScale;
                if (loaded)
                {
                    AutoSizeForm();
                }

                UpdateControls();
            }
        }
    }

    public Size ImageSize => new((int)Math.Round(Image.Width * (ImageScale / 100f)), (int)Math.Round(Image.Height * (ImageScale / 100f)));

    public Size FormSize
    {
        get
        {
            Size size = Minimized ? Options.MinimizeSize : ImageSize;
            if (Options.Border)
            {
                size = size.Offset(Options.BorderSize * 2);
            }

            return size;
        }
    }

    private int imageOpacity = 100;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ImageOpacity
    {
        get => imageOpacity;
        set
        {
            int newImageOpacity = value.Clamp(10, 100);

            if (imageOpacity != newImageOpacity)
            {
                imageOpacity = newImageOpacity;

                Opacity = imageOpacity / 100f;
            }
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Minimized { get; private set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PinToScreenOptions Options { get; private set; }

    private bool isDWMEnabled, loaded, dragging;
    private Point initialLocation;
    private Cursor openHandCursor, closedHandCursor;

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams createParams = base.CreateParams;
            createParams.ExStyle |= (int)WindowStyles.WS_EX_TOOLWINDOW;
            return createParams;
        }
    }

    private PinToScreenForm(PinToScreenOptions options)
    {
        Options = options;

        ImageScale = Options.InitialScale;
        ImageOpacity = Options.InitialOpacity;

        InitializeComponent();
        ShareXResources.ApplyTheme(this, true);
        TopMost = Options.TopMost;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

        tsMain.Cursor = Cursors.Arrow;
        openHandCursor = Helpers.CreateCursor(Resources.openhand);
        closedHandCursor = Helpers.CreateCursor(Resources.closedhand);
        SetHandCursor(false);

        isDWMEnabled = NativeMethods.IsDWMEnabled();

        UpdateControls();

        loaded = true;
    }

    private PinToScreenForm(Image image, PinToScreenOptions options, Point? location = null) : this(options)
    {
        Image = image;
        AutoSizeForm();

        if (location == null)
        {
            location = Helpers.GetPosition(Options.Placement, Options.PlacementOffset, HelpersLib.Helpers.CaptureHelpers.GetActiveScreenWorkingArea(), FormSize);
        } else if (Options.Border)
        {
            location = location.Value.Add(-Options.BorderSize);
        }

        Location = location.Value;
    }

    public static void PinToScreenAsync(Image image, PinToScreenOptions options = null, Point? location = null)
    {
        if (image != null)
        {
            options ??= new PinToScreenOptions();

            Thread thread = new(() =>
            {
                using PinToScreenForm form = new(image, options, location);
                lock (syncLock)
                {
                    forms.Add(form);
                }

                form.ShowDialog();

                lock (syncLock)
                {
                    forms.Remove(form);
                }
            });

            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }

    public static void CloseAll()
    {
        lock (syncLock)
        {
            foreach (PinToScreenForm form in forms)
            {
                form.InvokeSafe(form.Close);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            Image?.Dispose();
            openHandCursor?.Dispose();
            closedHandCursor?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void UpdateControls()
    {
        int toolbarMargin = 20;
        tsMain.Visible = ClientRectangle.Contains(PointToClient(MousePosition)) &&
            ClientRectangle.Contains(new Rectangle(0, 0, (Options.Border ? Options.BorderSize * 2 : 0) + tsMain.Width + toolbarMargin,
            (Options.Border ? Options.BorderSize * 2 : 0) + tsMain.Height + toolbarMargin));
        tslScale.Text = ImageScale + "%";
    }

    private void AutoSizeForm()
    {
        Size previousSize = Size;
        Size newSize = FormSize;

        Point newLocation = Location;
        IntPtr insertAfter = Options.TopMost ? (IntPtr)NativeConstants.HWND_TOPMOST : (IntPtr)NativeConstants.HWND_TOP;
        SetWindowPosFlags flags = SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOSENDCHANGING;

        if (Options.KeepCenterLocation)
        {
            Point locationOffset = new((previousSize.Width - newSize.Width) / 2, (previousSize.Height - newSize.Height) / 2);
            newLocation = new Point(newLocation.X + locationOffset.X, newLocation.Y + locationOffset.Y);
        } else
        {
            flags |= SetWindowPosFlags.SWP_NOMOVE;
        }

        NativeMethods.SetWindowPos(Handle, insertAfter, newLocation.X, newLocation.Y, newSize.Width, newSize.Height, flags);

        tsMain.Location = new Point(Width / 2 - tsMain.Width / 2, Options.Border ? Options.BorderSize : 0);

        UpdateControls();

        Refresh();
    }

    private void SetHandCursor(bool grabbing)
    {
        if (grabbing)
        {
            if (Cursor != closedHandCursor)
            {
                Cursor = closedHandCursor;
            }
        } else
        {
            if (Cursor != openHandCursor)
            {
                Cursor = openHandCursor;
            }
        }
    }

    private void ResetImage()
    {
        ImageScale = 100;
        ImageOpacity = 100;
    }

    private void ToggleMinimize()
    {
        Minimized = !Minimized;

        if (ImageOpacity < 100)
        {
            Opacity = Minimized ? (double)1f : (double)(ImageOpacity / 100f);
        }

        AutoSizeForm();
    }

    private void TsbCopy_Click(object sender, EventArgs e)
    {
        ClipboardHelpers.CopyImage(Image);
    }

    private void TslScale_Click(object sender, EventArgs e)
    {
        if (!Minimized)
        {
            ImageScale = 100;
        }
    }

    private void tsbOptions_Click(object sender, EventArgs e)
    {
        tsMain.Visible = false;

        using PinToScreenOptionsForm pinToScreenOptionsForm = new(Options);
        if (pinToScreenOptionsForm.ShowDialog(this) == DialogResult.OK)
        {
            if (TopMost != Options.TopMost)
            {
                TopMost = Options.TopMost;
            }

            AutoSizeForm();
        }
    }

    private void tsbClose_Click(object sender, EventArgs e)
    {
        Close();
    }

    protected override void WndProc(ref Message m)
    {
        if (Options.Shadow && m.Msg == (int)WindowsMessages.NCPAINT && isDWMEnabled)
        {
            NativeMethods.SetNCRenderingPolicy(Handle, DWMNCRENDERINGPOLICY.DWMNCRP_ENABLED);

            if (Helpers.IsWindows11OrGreater())
            {
                NativeMethods.SetWindowCornerPreference(Handle, DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND);
            }

            MARGINS margins = new()
            {
                bottomHeight = 1,
                leftWidth = 1,
                rightWidth = 1,
                topHeight = 1
            };

            _ = NativeMethods.DwmExtendFrameIntoClientArea(Handle, ref margins);
        }

        base.WndProc(ref m);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        //base.OnPaintBackground(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;

        g.Clear(Options.BackgroundColor);

        if (Image != null)
        {
            Point position = new(0, 0);

            if (Options.Border)
            {
                using (Pen pen = new(Options.BorderColor, Options.BorderSize) { Alignment = PenAlignment.Inset })
                {
                    g.DrawRectangleProper(pen, new Rectangle(position, FormSize));
                }

                position = position.Add(Options.BorderSize);
            }

            if (Minimized)
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(Image, new Rectangle(position, Options.MinimizeSize), 0, 0, Options.MinimizeSize.Width, Options.MinimizeSize.Height, GraphicsUnit.Pixel);
            } else
            {
                if (ImageScale == 100)
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(Image, new Rectangle(position, Image.Size));
                } else
                {
                    g.InterpolationMode = Options.HighQualityScale ? InterpolationMode.HighQualityBicubic : InterpolationMode.NearestNeighbor;

                    using ImageAttributes ia = new();
                    ia.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(Image, new Rectangle(position, ImageSize), 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel, ia);
                }
            }
        }
    }

    private void PinToScreenForm_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            if (e.Clicks > 1)
            {
                ToggleMinimize();
            } else
            {
                dragging = true;
                initialLocation = e.Location;
                SetHandCursor(true);
            }
        }
    }

    private void PinToScreenForm_MouseMove(object sender, MouseEventArgs e)
    {
        if (dragging)
        {
            Location = new(Location.X + e.X - initialLocation.X, Location.Y + e.Y - initialLocation.Y);
            Update();
        }
    }

    private void PinToScreenForm_MouseUp(object sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Left:
                dragging = false;
                SetHandCursor(false);
                break;
            case MouseButtons.Right:
                Close();
                break;
        }

        if (!Minimized)
        {
            switch (e.Button)
            {
                case MouseButtons.Middle:
                    ResetImage();
                    break;
            }
        }
    }

    private void PinToScreenForm_MouseWheel(object sender, MouseEventArgs e)
    {
        if (Minimized) return;

        if (ModifierKeys == Keys.None)
            ImageScale += e.Delta > 0 ? Options.ScaleStep : -Options.ScaleStep;

        if (ModifierKeys == Keys.Control)
            ImageOpacity += e.Delta > 0 ? Options.OpacityStep : -Options.OpacityStep;
    }

    private void PinToScreenForm_MouseEnter(object sender, EventArgs e)
    {
        UpdateControls();
    }

    private void PinToScreenForm_MouseLeave(object sender, EventArgs e)
    {
        UpdateControls();
    }

    private void TsMain_MouseLeave(object sender, EventArgs e)
    {
        UpdateControls();
    }

    private void PinToScreenForm_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyData == (Keys.Control | Keys.C)) ClipboardHelpers.CopyImage(Image);

        if (!Minimized)
        {
            _ = e.KeyData switch
            {
                Keys.Oemplus or Keys.Add => ImageScale += Options.ScaleStep,
                Keys.OemMinus or Keys.Subtract => ImageScale -= Options.ScaleStep,
                Keys.Control | Keys.Oemplus or Keys.Control | Keys.Add => ImageOpacity += Options.OpacityStep,
                Keys.Control | Keys.OemMinus or Keys.Control | Keys.Subtract => ImageOpacity -= Options.OpacityStep,
                _ => 0
            };
        }
    }

    private static readonly string PathSave = TaskHelpers.GetScreenshotsFolder() + Path.DirectorySeparatorChar + DateTime.Now.Second + DateTime.Now.Microsecond+".png";
    private void TsbSave_Click(object sender, EventArgs e)
    {
        Image.Save(PathSave, ImageFormat.Png);
    }
}
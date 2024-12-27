// ImageListView - A listview control for image files
// Copyright (C) 2009 Ozgur Ozcitak
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Ozgur Ozcitak (ozcitak@yahoo.com)

using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace ShareX.ImageListView;

#region DefaultRenderer
/// <summary>
/// The default renderer.
/// </summary>
public class DefaultRenderer : ImageListView.ImageListViewRenderer
{
    /// <summary>
    /// Initializes a new instance of the DefaultRenderer class.
    /// </summary>
    public DefaultRenderer()
    {
        ;
    }
}
#endregion

#region DebugRenderer
#if DEBUG
/// <summary>
/// Represents a renderer meant to be used for debugging purposes.
/// Included in the debug build only.
/// </summary>
public class DebugRenderer : ImageListView.ImageListViewRenderer
{
    private readonly long baseMem;

    /// <summary>
    /// Initializes a new instance of the DebugRenderer class.
    /// </summary>
    public DebugRenderer()
    {
        Process p = Process.GetCurrentProcess();
        p.Refresh();
        baseMem = p.PrivateMemorySize64;
    }
    /// <summary>
    /// Draws the specified item on the given graphics.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="state">The current view state of item.</param>
    /// <param name="bounds">The bounding rectangle of item in client coordinates.</param>
    public override void DrawItem(Graphics g, ImageListViewItem item, ItemState state, Rectangle bounds)
    {
        if (item.Index == ImageListView.layoutManager.FirstPartiallyVisible ||
            item.Index == ImageListView.layoutManager.LastPartiallyVisible)
        {
            using Brush b = new HatchBrush(HatchStyle.BackwardDiagonal, Color.Green, Color.Transparent);
            g.FillRectangle(b, bounds);
        }
        if (item.Index == ImageListView.layoutManager.FirstVisible ||
            item.Index == ImageListView.layoutManager.LastVisible)
        {
            using Brush b = new HatchBrush(HatchStyle.ForwardDiagonal, Color.Red, Color.Transparent);
            g.FillRectangle(b, bounds);
        }

        base.DrawItem(g, item, state, bounds);

        if ((state & ItemState.Selected) != ItemState.None)
        {
            using Pen p = new(Color.Blue);
            g.DrawRectangle(p, bounds);
        }
    }
    /// <summary>
    /// Draws an overlay image over the client area.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="bounds">The bounding rectangle of the client area.</param>
    public override void DrawOverlay(Graphics g, Rectangle bounds)
    {
        // Refresh process info
        Process p = Process.GetCurrentProcess();
        p.Refresh();
        long mem = Math.Max(0, p.PrivateMemorySize64 - baseMem);

        // Display memory stats
        string s = string.Format("Total: {0}\r\nCache: {1}\r\nCache*: {2}", Utility.FormatSize(baseMem), Utility.FormatSize(mem), Utility.FormatSize(ImageListView.thumbnailCache.MemoryUsed));
        SizeF sz = g.MeasureString(s, ImageListView.Font);
        Rectangle r = new(ItemAreaBounds.Right - 120, ItemAreaBounds.Top + 5, 115, (int)sz.Height);
        using (Brush b = new SolidBrush(Color.FromArgb(220, Color.LightGray)))
        {
            g.FillRectangle(b, r);
        }
        using (Pen pen = new(Color.FromArgb(128, Color.Gray)))
        {
            g.DrawRectangle(pen, r);
        }
        g.DrawString(s, ImageListView.Font, Brushes.Black, r.Location);

        // Display navigation parameters
        r = new Rectangle(ItemAreaBounds.Right - 120, ItemAreaBounds.Top + 5 + (int)sz.Height + 10, 115, 125);
        using (Brush b = new SolidBrush(Color.FromArgb(220, Color.LightGray)))
        {
            g.FillRectangle(b, r);
        }
        using (Pen pen = new(Color.FromArgb(128, Color.Gray)))
        {
            g.DrawRectangle(pen, r);
        }

        // Is left button down?
        r = new Rectangle(r.Left + 5, r.Top + 5, 15, 15);
        if (ImageListView.navigationManager.LeftButton)
        {
            g.FillRectangle(Brushes.DarkGray, r);
        }
        g.DrawRectangle(Pens.Black, r);
        r.Offset(15, 0);
        r.Offset(15, 0);

        // Is right button down?
        if (ImageListView.navigationManager.RightButton)
        {
            g.FillRectangle(Brushes.DarkGray, r);
        }
        g.DrawRectangle(Pens.Black, r);
        r.Offset(-30, 22);

        // Is shift key down?
        Color tColor = Color.Gray;
        if (ImageListView.navigationManager.ShiftKey)
            tColor = Color.Black;
        using (Brush b = new SolidBrush(tColor))
        {
            g.DrawString("Shift", ImageListView.Font, b, r.Location);
        }
        r.Offset(0, 12);

        // Is control key down?
        tColor = Color.Gray;
        if (ImageListView.navigationManager.ControlKey)
            tColor = Color.Black;
        using (Brush b = new SolidBrush(tColor))
        {
            g.DrawString("Control", ImageListView.Font, b, r.Location);
        }
        r.Offset(0, 20);

        // Display hit test details for item area
        ImageListView.HitTest(ImageListView.PointToClient(Control.MousePosition), out ImageListView.HitInfo h);

        tColor = Color.Gray;
        if (h.InItemArea)
            tColor = Color.Black;
        using (Brush b = new SolidBrush(tColor))
        {
            g.DrawString("InItemArea (" + h.ItemIndex.ToString() + ")", ImageListView.Font, b, r.Location);
        }
        r.Offset(0, 12);

        // Display hit test details for column header area
        tColor = Color.Gray;
        if (h.InHeaderArea)
            tColor = Color.Black;
        using (Brush b = new SolidBrush(tColor))
        {
            if (h.Column != null)
            {
                g.DrawString("InHeaderArea (" + h.Column.ToString() + ")", ImageListView.Font, b, r.Location);
            }
        }
        r.Offset(0, 12);

        // Display hit test details for pane area
        tColor = Color.Gray;
        if (h.InPaneArea)
            tColor = Color.Black;
        using (Brush b = new SolidBrush(tColor))
        {
            g.DrawString("InPaneArea " + (h.PaneBorder ? " (Border)" : ""), ImageListView.Font, b, r.Location);
        }
        r.Offset(0, 12);
    }
}
#endif
#endregion

#region NewYear2010Renderer
/// <summary>
/// A renderer to celebrate the new year of 2010.
/// </summary>
/// <remarks>Compile with conditional compilation symbol BONUSPACK to
/// include this renderer in the assembly.</remarks>
public class NewYear2010Renderer : ImageListView.ImageListViewRenderer
{
    /// <summary>
    /// Represents a snow flake
    /// </summary>
    private class SnowFlake
    {
        /// <summary>
        /// Gets or sets the client coordinates of the snow flake.
        /// </summary>
        public Point Location { get; set; }
        /// <summary>
        /// Gets or sets the rotation angle of the snow flake in degrees.
        /// </summary>
        public double Rotation { get; set; }
        /// <summary>
        /// Gets or sets the size of the snow flake.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Initializes a new instance of the SnowFlake class.
        /// </summary>
        /// <param name="newSize">The size of the snow flake.</param>
        public SnowFlake(int newSize)
        {
            Size = newSize;
            Location = new Point(0, 0);
            Rotation = 0.0;
        }
    }

    private readonly object lockObject = new();

    private readonly int maxFlakeCount = 100;
    private readonly int minFlakeSize = 4;
    private readonly int maxFlakeSize = 12;
    private readonly int flakeGeneration = 3;
    private int currentGeneration = 0;
    private readonly long refreshPeriod = 50;
    private readonly int fallSpeed = 3;
    private DateTime lastRedraw = DateTime.Now;
    private volatile bool inTimer = false;

    private List<SnowFlake>? flakes = null;
    private System.Threading.Timer timer;
    private Random random = new();

    private Rectangle displayBounds = Rectangle.Empty;

    private GraphicsPath flake;
    private GraphicsPath terrain;

    /// <summary>
    /// Initializes a new instance of the NewYear2010Renderer class.
    /// </summary>
    public NewYear2010Renderer()
    {
        flake = CreateFlake(10, 3);
        terrain = CreateTerrain();
        timer = new System.Threading.Timer(UpdateTimerCallback, null, 0, refreshPeriod);
    }

    /// <summary>
    /// Generates a snowflake from a Koch curve.
    /// http://en.wikipedia.org/wiki/Koch_snowflake
    /// </summary>
    /// <param name="size">The size of the snow flake.</param>
    /// <param name="iterations">Number of iterations. Higher values 
    /// produce more complex curves.</param>
    private GraphicsPath CreateFlake(int size, int iterations)
    {
        Queue<PointF> segments = new();
        float h = (float)Math.Sin(Math.PI / 3.0) * size;
        PointF v1 = new(-1.0f * size / 2.0f, -h / 3.0f);
        PointF v2 = new(size / 2f, -h / 3.0f);
        PointF v3 = new(0.0f, h * 2.0f / 3.0f);
        segments.Enqueue(v1); segments.Enqueue(v2);
        segments.Enqueue(v2); segments.Enqueue(v3);
        segments.Enqueue(v3); segments.Enqueue(v1);

        for (int k = 0; k < iterations - 1; k++)
        {
            int todivide = segments.Count / 2;
            for (int i = 0; i < todivide; i++)
            {
                PointF p1 = segments.Dequeue();
                PointF p2 = segments.Dequeue();

                // Trisect the segment
                PointF pi1 = new((p2.X - p1.X) / 3.0f + p1.X,
                    (p2.Y - p1.Y) / 3.0f + p1.Y);
                PointF pi2 = new((p2.X - p1.X) * 2.0f / 3.0f + p1.X,
                    (p2.Y - p1.Y) * 2.0f / 3.0f + p1.Y);
                double dist = Math.Sqrt((pi1.X - pi2.X) * (pi1.X - pi2.X) + (pi1.Y - pi2.Y) * (pi1.Y - pi2.Y));
                double angle = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X) - Math.PI / 3.0;
                PointF pn = new(pi1.X + (float)(Math.Cos(angle) * dist),
                    pi1.Y + (float)(Math.Sin(angle) * dist));

                segments.Enqueue(p1); segments.Enqueue(pi1);
                segments.Enqueue(pi1); segments.Enqueue(pn);
                segments.Enqueue(pn); segments.Enqueue(pi2);
                segments.Enqueue(pi2); segments.Enqueue(p2);
            }
        }

        GraphicsPath path = new();
        while (segments.Count != 0)
        {
            PointF p1 = segments.Dequeue();
            PointF p2 = segments.Dequeue();
            path.AddLine(p1, p2);
        }

        path.CloseFigure();
        return path;
    }

    /// <summary>
    /// Generates a random snowy terrain.
    /// </summary>
    private GraphicsPath CreateTerrain()
    {
        Random rnd = new();
        GraphicsPath path = new();
        int width = 100;
        int height = 10;

        int count = 20;
        int step = width / count;
        int lastx = 0, lasty = 0;
        Point[] points = new Point[count];
        for (int i = 0; i < count; i++)
        {
            int x = i * (width + 2 * step) / count - step;
            int y = rnd.Next(-height / 2, height / 2);
            points[i] = new Point(x, y);
            lastx = x;
            lasty = y;
        }
        path.AddCurve(points);

        path.AddLine(lastx, lasty, width + step, 0);
        path.AddLine(width + step, 0, width + step, 200);
        path.AddLine(width + step, 200, -step, 200);
        path.CloseFigure();

        return path;
    }

    /// <summary>
    /// Updates snow flakes at each timer tick.
    /// </summary>
    /// <param name="state">Not used, null.</param>
    private void UpdateTimerCallback(object state)
    {
        if (inTimer) return;
        inTimer = true;

        bool redraw = false;

        lock (lockObject)
        {
            if (displayBounds.IsEmpty)
            {
                inTimer = false;
                return;
            }

            if (flakes == null)
                flakes = new List<SnowFlake>();

            // Add new snow flakes
            currentGeneration++;
            if (currentGeneration == flakeGeneration)
            {
                currentGeneration = 0;
                if (flakes.Count < maxFlakeCount)
                {
                    SnowFlake snowFlake = new(random.Next(minFlakeSize, maxFlakeSize));
                    snowFlake.Rotation = 360.0 * random.NextDouble();
                    snowFlake.Location = new Point(random.Next(displayBounds.Left, displayBounds.Right), -20);
                    flakes.Add(snowFlake);
                }
            }

            // Move snow flakes
            for (int i = flakes.Count - 1; i >= 0; i--)
            {
                SnowFlake snowFlake = flakes[i];
                if (snowFlake.Location.Y > displayBounds.Height)
                    flakes.Remove(snowFlake);
                else
                {
                    snowFlake.Location = new Point(snowFlake.Location.X, snowFlake.Location.Y + snowFlake.Size * fallSpeed / 10);
                    snowFlake.Rotation += 360.0 / 40.0;
                    if (snowFlake.Rotation > 360.0) snowFlake.Rotation -= 360.0;
                }
            }

            // Do we need a refresh?
            if ((DateTime.Now - lastRedraw).Milliseconds > refreshPeriod)
                redraw = true;
        }

        if (redraw)
        {
            try
            {
                if (ImageListView != null && ImageListView.IsHandleCreated && !ImageListView.IsDisposed)
                {
                    if (ImageListView.InvokeRequired)
                        ImageListView.BeginInvoke((MethodInvoker)delegate { ImageListView.Refresh(); });
                    else
                        ImageListView.Refresh();
                }
            } catch
            {
                ;
            }
        }

        inTimer = false;
    }

    /// <summary>
    /// Initializes the System.Drawing.Graphics used to draw
    /// control elements.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    public override void InitializeGraphics(Graphics g)
    {
        base.InitializeGraphics(g);
        g.SmoothingMode = SmoothingMode.HighQuality;
    }

    /// <summary>
    /// Sets the layout of the control.
    /// </summary>
    /// <param name="e">A LayoutEventArgs that contains event data.</param>
    public override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        lock (lockObject)
        {
            displayBounds = ImageListView.DisplayRectangle;
        }
    }

    /// <summary>
    /// Draws an overlay image over the client area.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="bounds">The bounding rectangle of the client area.</param>
    public override void DrawOverlay(Graphics g, Rectangle bounds)
    {
        lock (lockObject)
        {
            lastRedraw = DateTime.Now;
        }

        // Draw the terrain
        DrawTerrain(g);

        lock (lockObject)
        {
            // Draw the snow flakes
            if (flakes != null)
            {
                foreach (SnowFlake snowFlake in flakes)
                    DrawSnowFlake(g, snowFlake);
            }
        }

        // Redraw some of the terrain over slow flakes
        DrawTerrainOutline(g);
    }

    /// <summary>
    /// Draws a snow flake.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="snowFlake">The snowflake to draw.</param>
    private void DrawSnowFlake(Graphics g, SnowFlake snowFlake)
    {
        g.ResetTransform();
        // Tranform to upper left corner before rotating.
        // This produces a nice wobbling effect.
        g.TranslateTransform(-snowFlake.Size / 2, -snowFlake.Size / 2, MatrixOrder.Append);
        g.ScaleTransform(snowFlake.Size / 6.0f, snowFlake.Size / 6.0f);
        g.RotateTransform((float)snowFlake.Rotation, MatrixOrder.Append);
        g.TranslateTransform(snowFlake.Location.X, snowFlake.Location.Y, MatrixOrder.Append);
        using (SolidBrush brush = new(Color.White))
        using (Pen pen = new(Color.Gray))
        using (Pen glowPen = new(Color.FromArgb(96, Color.White), 2.0f))
        {
            g.DrawPath(glowPen, flake);
            g.FillPath(brush, flake);
            g.DrawPath(pen, flake);
        }

        g.ResetTransform();
    }

    /// <summary>
    /// Draws the terrain.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    private void DrawTerrain(Graphics g)
    {
        g.ResetTransform();
        using (SolidBrush brush = new(Color.White))
        using (Pen pen = new(Color.Gray))
        {
            Rectangle rec = ImageListView.DisplayRectangle;
            g.ScaleTransform(rec.Width / 100.0f, 3.0f, MatrixOrder.Append);
            g.TranslateTransform(0, rec.Height - 30, MatrixOrder.Append);
            g.FillPath(brush, terrain);
            g.DrawPath(pen, terrain);
        }
        g.ResetTransform();
    }

    /// <summary>
    /// Draws the terrain outline.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    private void DrawTerrainOutline(Graphics g)
    {
        g.ResetTransform();
        using (SolidBrush brush = new(Color.White))
        {
            Rectangle rec = ImageListView.DisplayRectangle;
            g.ScaleTransform(rec.Width / 100.0f, 3.0f, MatrixOrder.Append);
            g.TranslateTransform(0, rec.Height - 20, MatrixOrder.Append);
            g.FillPath(brush, terrain);
        }
        g.ResetTransform();
    }

    /// <summary>
    /// Releases managed resources.
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();

        flake.Dispose();
        terrain.Dispose();
        timer.Dispose();
    }
}
#endregion

#region NoirRenderer
/// <summary>
/// A renderer with a dark theme.
/// This renderer cannot be themed.
/// </summary>
public class NoirRenderer : ImageListView.ImageListViewRenderer
{
    private readonly int padding;
    private int mReflectionSize;

    /// <summary>
    /// Gets or sets the size of image reflections.
    /// </summary>
    public int ReflectionSize { get { return mReflectionSize; } set { mReflectionSize = value; } }

    /// <summary>
    /// Initializes a new instance of the NoirRenderer class.
    /// </summary>
    public NoirRenderer()
        : this(20)
    {
        ;
    }
    /// <summary>
    /// Initializes a new instance of the NoirRenderer class.
    /// </summary>
    /// <param name="reflectionSize">Size of image reflections.</param>
    public NoirRenderer(int reflectionSize)
    {
        mReflectionSize = reflectionSize;
        padding = 4;
    }

    /// <summary>
    /// Gets a value indicating whether this renderer can apply custom colors.
    /// </summary>
    /// <value></value>
    public override bool CanApplyColors { get { return false; } }

    /// <summary>
    /// Draws the background of the control.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="bounds">The client coordinates of the item area.</param>
    public override void DrawBackground(Graphics g, Rectangle bounds)
    {
        g.Clear(Color.Black);
    }
    /// <summary>
    /// Returns item size for the given view mode.
    /// </summary>
    /// <param name="view">The view mode for which the measurement should be made.</param>
    /// <returns>The item size.</returns>
    public override Size MeasureItem(View view)
    {
        return view == View.Details
            ? base.MeasureItem(view)
            : new Size(ImageListView.ThumbnailSize.Width + 2 * padding,
                ImageListView.ThumbnailSize.Height + 2 * padding + mReflectionSize);
    }
    /// <summary>
    /// Draws the specified item on the given graphics.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="state">The current view state of item.</param>
    /// <param name="bounds">The bounding rectangle of item in client coordinates.</param>
    public override void DrawItem(Graphics g, ImageListViewItem item, ItemState state, Rectangle bounds)
    {
        // Item background color
        using (Brush brush = new SolidBrush(Color.Black))
        {
            g.FillRectangle(brush, bounds);
        }

        if (ImageListView.View == View.Details)
        {
            // Item background
            if ((state & ItemState.Selected) == ItemState.Selected)
            {
                using Brush brush = new LinearGradientBrush(bounds,
                    Color.FromArgb(64, 96, 160), Color.FromArgb(64, 64, 96, 160), LinearGradientMode.Horizontal);
                g.FillRectangle(brush, bounds);
            } else if ((state & ItemState.Hovered) == ItemState.Hovered)
            {
                using Brush brush = new LinearGradientBrush(bounds,
                    Color.FromArgb(64, Color.White), Color.FromArgb(16, Color.White), LinearGradientMode.Horizontal);
                g.FillRectangle(brush, bounds);
            }

            // Shade sort column
            List<ImageListView.ImageListViewColumnHeader> uicolumns = ImageListView.Columns.GetDisplayedColumns();
            int x = bounds.Left - 1;
            foreach (ImageListView.ImageListViewColumnHeader column in uicolumns)
            {
                if (ImageListView.SortColumn >= 0 && ImageListView.SortColumn < ImageListView.Columns.Count &&
                    ImageListView.Columns[ImageListView.SortColumn].Guid == column.Guid &&
                    ImageListView.SortOrder != SortOrder.None &&
                    (state & ItemState.Hovered) == ItemState.None && (state & ItemState.Selected) == ItemState.None)
                {
                    Rectangle subItemBounds = bounds;
                    subItemBounds.X = x;
                    subItemBounds.Width = column.Width;
                    using Brush brush = new SolidBrush(Color.FromArgb(32, 128, 128, 128));
                    g.FillRectangle(brush, subItemBounds);
                    break;
                }
                x += column.Width;
            }
            // Separators 
            x = ImageListView.layoutManager.ColumnHeaderBounds.Left;
            foreach (ImageListView.ImageListViewColumnHeader column in uicolumns)
            {
                x += column.Width;
                if (!ReferenceEquals(column, uicolumns[uicolumns.Count - 1]))
                {
                    using Pen pen = new(Color.FromArgb(64, 128, 128, 128));
                    g.DrawLine(pen, x, bounds.Top, x, bounds.Bottom);
                }
            }

            // Item texts
            Size offset = new(2, (bounds.Height - ImageListView.Font.Height) / 2);
            using (StringFormat sf = new())
            {
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                // Sub text
                RectangleF rt = new(bounds.Left + offset.Width, bounds.Top + offset.Height, uicolumns[0].Width - 2 * offset.Width, bounds.Height - 2 * offset.Height);
                foreach (ImageListView.ImageListViewColumnHeader column in uicolumns)
                {
                    rt.Width = column.Width - 2 * offset.Width;
                    Color foreColor = Color.White;
                    if (!item.Enabled) foreColor = Color.FromArgb(128, 128, 128);
                    using (Brush bItemFore = new SolidBrush(foreColor))
                    {
                        if (column.Type == ColumnType.Rating && ImageListView.RatingImage != null && ImageListView.EmptyRatingImage != null)
                        {
                            string srating = item.GetSubItemText(ColumnType.Rating);
                            if (!string.IsNullOrEmpty(srating))
                            {
                                int w = ImageListView.RatingImage.Width;
                                int y = (int)(rt.Top + (rt.Height - ImageListView.RatingImage.Height) / 2.0f);
                                int rating = item.StarRating;
                                if (rating < 0) rating = 0;
                                if (rating > 5) rating = 5;
                                for (int i = 1; i <= rating; i++)
                                    g.DrawImage(ImageListView.RatingImage, rt.Left + (i - 1) * w, y);
                                for (int i = rating + 1; i <= 5; i++)
                                    g.DrawImage(ImageListView.EmptyRatingImage, rt.Left + (i - 1) * w, y);
                            }
                        } else
                            g.DrawString(item.SubItems[column].Text, ImageListView.Font, bItemFore, rt, sf);
                    }
                    rt.X += column.Width;
                }
            }

            // Border
            if ((state & ItemState.Hovered) == ItemState.Hovered)
            {
                using Pen pen = new(Color.FromArgb(128, Color.White));
                g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            } else if ((state & ItemState.Selected) == ItemState.Hovered)
            {
                using Pen pen = new(Color.FromArgb(96, 144, 240));
                g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
        } else // if (ImageListView.View != View.Details)
        {
            // Align images to bottom of bounds
            Image img = item.GetCachedImage(CachedImageType.Thumbnail);
            if (img != null)
            {
                Rectangle pos = Utility.GetSizedImageBounds(img,
                    new Rectangle(bounds.X + padding, bounds.Y + padding, bounds.Width - 2 * padding, bounds.Height - 2 * padding - mReflectionSize),
                    50.0f, 100.0f);

                int x = pos.X;
                int y = pos.Y;

                // Item background
                if ((state & ItemState.Selected) == ItemState.Selected)
                {
                    using Brush brush = new LinearGradientBrush(
                        new Point(x - padding, y - padding), new Point(x - padding, y + pos.Height + 2 * padding),
                        Color.FromArgb(64, 96, 160), Color.FromArgb(16, 16, 16));
                    g.FillRectangle(brush, x - padding, y - padding, pos.Width + 2 * padding, pos.Height + 2 * padding);
                } else if ((state & ItemState.Hovered) == ItemState.Hovered)
                {
                    using Brush brush = new LinearGradientBrush(
                        new Point(x - padding, y - padding), new Point(x - padding, y + pos.Height + 2 * padding),
                        Color.FromArgb(64, Color.White), Color.FromArgb(16, 16, 16));
                    g.FillRectangle(brush, x - padding, y - padding, pos.Width + 2 * padding, pos.Height + 2 * padding);
                }

                // Border
                if ((state & ItemState.Hovered) == ItemState.Hovered)
                {
                    using Brush brush = new LinearGradientBrush(
                        new Point(x - padding, y - padding), new Point(x - padding, y + pos.Height + 2 * padding),
                        Color.FromArgb(128, Color.White), Color.FromArgb(16, 16, 16));
                    using Pen pen = new(brush);
                    g.DrawRectangle(pen, x - padding, y - padding + 1, pos.Width + 2 * padding - 1, pos.Height + 2 * padding - 1);
                } else if ((state & ItemState.Selected) == ItemState.Selected)
                {
                    using Brush brush = new LinearGradientBrush(
                        new Point(x - padding, y - padding), new Point(x - padding, y + pos.Height + 2 * padding),
                        Color.FromArgb(96, 144, 240), Color.FromArgb(16, 16, 16));
                    using Pen pen = new(brush);
                    g.DrawRectangle(pen, x - padding, y - padding + 1, pos.Width + 2 * padding - 1, pos.Height + 2 * padding - 1);
                }

                // Draw item image
                DrawImageWithReflection(g, img, pos, mReflectionSize);

                // Shade over disabled item image
                if (!item.Enabled)
                {
                    pos.Inflate(4, 4);
                    using Brush brush = new LinearGradientBrush(pos,
                        Color.FromArgb(64, 0, 0, 0), Color.FromArgb(196, 0, 0, 0), LinearGradientMode.Vertical);
                    g.FillRectangle(brush, pos);
                }

                // Highlight
                if (item.Enabled)
                {
                    using Pen pen = new(Color.FromArgb(160, Color.White));
                    g.DrawLine(pen, pos.X, pos.Y + 1, pos.X + pos.Width - 1, pos.Y + 1);
                    g.DrawLine(pen, pos.X, pos.Y + 1, pos.X, pos.Y + pos.Height);
                }
            }
        }
    }
    /// <summary>
    /// Draws the checkbox icon for the specified item on the given graphics.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="bounds">The bounding rectangle of the checkbox in client coordinates.</param>
    public override void DrawCheckBox(Graphics g, ImageListViewItem item, Rectangle bounds)
    {
        ;
    }
    /// <summary>
    /// Draws the file icon for the specified item on the given graphics.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="bounds">The bounding rectangle of the file icon in client coordinates.</param>
    public override void DrawFileIcon(Graphics g, ImageListViewItem item, Rectangle bounds)
    {
        ;
    }
    /// <summary>
    /// Draws the column headers.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="column">The ImageListViewColumnHeader to draw.</param>
    /// <param name="state">The current view state of column.</param>
    /// <param name="bounds">The bounding rectangle of column in client coordinates.</param>
    public override void DrawColumnHeader(Graphics g, ImageListView.ImageListViewColumnHeader column, ColumnState state, Rectangle bounds)
    {
        // Paint background
        if (ImageListView.Focused && (state & ColumnState.Hovered) == ColumnState.Hovered)
        {
            using Brush bHovered = new LinearGradientBrush(bounds,
                Color.FromArgb(64, 96, 144, 240), Color.FromArgb(196, 96, 144, 240), LinearGradientMode.Vertical);
            g.FillRectangle(bHovered, bounds);
        } else
        {
            using Brush bNormal = new LinearGradientBrush(bounds,
                Color.FromArgb(32, 128, 128, 128), Color.FromArgb(196, 128, 128, 128), LinearGradientMode.Vertical);
            g.FillRectangle(bNormal, bounds);
        }
        using (Brush bBorder = new LinearGradientBrush(bounds,
            Color.FromArgb(96, 128, 128, 128), Color.FromArgb(128, 128, 128), LinearGradientMode.Vertical))
        using (Pen pBorder = new(bBorder))
        {
            g.DrawLine(pBorder, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
            g.DrawLine(pBorder, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
        }
        using (Pen pen = new(Color.FromArgb(16, Color.White)))
        {
            g.DrawLine(pen, bounds.Left + 1, bounds.Top + 1, bounds.Left + 1, bounds.Bottom - 2);
            g.DrawLine(pen, bounds.Right - 1, bounds.Top + 1, bounds.Right - 1, bounds.Bottom - 2);
        }

        // Draw the sort arrow
        int textOffset = 4;
        if (ImageListView.SortOrder != SortOrder.None && (state & ColumnState.SortColumn) != ColumnState.None)
        {
            Image? img = null;
            if (ImageListView.SortOrder == SortOrder.Ascending || ImageListView.SortOrder == SortOrder.AscendingNatural)
                img = ImageListViewResources.SortAscending;
            else if (ImageListView.SortOrder == SortOrder.Descending || ImageListView.SortOrder == SortOrder.DescendingNatural)
                img = ImageListViewResources.SortDescending;
            g.DrawImageUnscaled(img, bounds.X + 4, bounds.Top + (bounds.Height - img.Height) / 2);
            textOffset += img.Width;
        }

        // Text
        bounds.X += textOffset;
        bounds.Width -= textOffset;
        if (bounds.Width > 4)
        {
            using StringFormat sf = new();
            sf.FormatFlags = StringFormatFlags.NoWrap;
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            sf.Trimming = StringTrimming.EllipsisCharacter;
            using Brush brush = new SolidBrush(Color.White);
            g.DrawString(column.Text,
                ImageListView.ColumnHeaderFont == null ? ImageListView.Font : ImageListView.ColumnHeaderFont,
                brush, bounds, sf);
        }
    }
    /// <summary>
    /// Draws the extender after the last column.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="bounds">The bounding rectangle of extender column in client coordinates.</param>
    public override void DrawColumnExtender(Graphics g, Rectangle bounds)
    {
        using (Brush bNormal = new LinearGradientBrush(bounds,
            Color.FromArgb(32, 128, 128, 128), Color.FromArgb(196, 128, 128, 128), LinearGradientMode.Vertical))
        {
            g.FillRectangle(bNormal, bounds);
        }
        using (Brush bBorder = new LinearGradientBrush(bounds,
            Color.FromArgb(96, 128, 128, 128), Color.FromArgb(128, 128, 128), LinearGradientMode.Vertical))
        using (Pen pBorder = new(bBorder))
        {
            g.DrawLine(pBorder, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
            g.DrawLine(pBorder, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
        }
        using Pen pen = new(Color.FromArgb(16, Color.White));
        g.DrawLine(pen, bounds.Left + 1, bounds.Top + 1, bounds.Left + 1, bounds.Bottom - 2);
        g.DrawLine(pen, bounds.Right - 1, bounds.Top + 1, bounds.Right - 1, bounds.Bottom - 2);
    }
    /// <summary>
    /// Draws the large preview image of the focused item in Gallery mode.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="image">The image to draw.</param>
    /// <param name="bounds">The bounding rectangle of the preview area.</param>
    public override void DrawGalleryImage(Graphics g, ImageListViewItem item, Image image, Rectangle bounds)
    {
        if (item != null && image != null)
        {
            Size itemMargin = MeasureItemMargin(ImageListView.View);
            Rectangle pos = Utility.GetSizedImageBounds(image, new Rectangle(bounds.X + itemMargin.Width, bounds.Y + itemMargin.Height, bounds.Width - 2 * itemMargin.Width, bounds.Height - 2 * itemMargin.Height - mReflectionSize), 50.0f, 100.0f);
            DrawImageWithReflection(g, image, pos, mReflectionSize);
        }
    }
    /// <summary>
    /// Draws the left pane in Pane view mode.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="image">The image to draw.</param>
    /// <param name="bounds">The bounding rectangle of the pane.</param>
    public override void DrawPane(Graphics g, ImageListViewItem item, Image image, Rectangle bounds)
    {
        // Draw resize handle
        using (Brush bBorder = new SolidBrush(Color.FromArgb(64, 64, 64)))
        {
            g.FillRectangle(bBorder, bounds.Right - 2, bounds.Top, 2, bounds.Height);
        }
        bounds.Width -= 2;

        if (item != null && image != null)
        {
            // Calculate image bounds
            Size itemMargin = MeasureItemMargin(ImageListView.View);
            Rectangle pos = Utility.GetSizedImageBounds(image, new Rectangle(bounds.Location + itemMargin, bounds.Size - itemMargin - itemMargin), 50.0f, 0.0f);
            // Draw image
            g.DrawImage(image, pos);

            bounds.X += itemMargin.Width;
            bounds.Width -= 2 * itemMargin.Width;
            bounds.Y = pos.Height + 16;
            bounds.Height -= pos.Height + 16;

            // Item text
            if (ImageListView.Columns.HasType(ColumnType.Name) && ImageListView.Columns[ColumnType.Name].Visible && bounds.Height > 0)
            {
                string text = item.GetSubItemText(ColumnType.Name);
                int y = Utility.DrawStringPair(g, bounds, "", text, ImageListView.Font,
                    Brushes.White, Brushes.White);
                bounds.Y += 2 * y;
                bounds.Height -= 2 * y;
            }

            // File type
            string fileType = item.GetSubItemText(ColumnType.FileType);
            if (ImageListView.Columns.HasType(ColumnType.FileType) && ImageListView.Columns[ColumnType.FileType].Visible && bounds.Height > 0 && !string.IsNullOrEmpty(fileType))
            {
                using Brush bCaption = new SolidBrush(Color.FromArgb(196, 196, 196));
                using Brush bText = new SolidBrush(Color.White);
                int y = Utility.DrawStringPair(g, bounds, ImageListView.Columns[ColumnType.FileType].Text + ": ",
                        fileType, ImageListView.Font, bCaption, bText);
                bounds.Y += y;
                bounds.Height -= y;
            }

            // Metatada
            foreach (ImageListView.ImageListViewColumnHeader column in ImageListView.Columns)
            {
                if (column.Type == ColumnType.ImageDescription)
                {
                    bounds.Y += 8;
                    bounds.Height -= 8;
                }

                if (bounds.Height <= 0) break;

                if (column.Visible &&
                    column.Type != ColumnType.Custom &&
                    column.Type != ColumnType.FileType &&
                    column.Type != ColumnType.DateAccessed &&
                    column.Type != ColumnType.FileName &&
                    column.Type != ColumnType.FilePath &&
                    column.Type != ColumnType.Name)
                {
                    string caption = column.Text;
                    string text = item.GetSubItemText(column.Type);
                    if (!string.IsNullOrEmpty(text))
                    {
                        using Brush bCaption = new SolidBrush(Color.FromArgb(196, 196, 196));
                        using Brush bText = new SolidBrush(Color.White);
                        int y = Utility.DrawStringPair(g, bounds, caption + ": ", text,
                                ImageListView.Font, bCaption, bText);
                        bounds.Y += y;
                        bounds.Height -= y;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Draws the selection rectangle.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="selection">The client coordinates of the selection rectangle.</param>
    public override void DrawSelectionRectangle(Graphics g, Rectangle selection)
    {
        using (Brush brush = new LinearGradientBrush(selection,
            Color.FromArgb(160, 96, 144, 240), Color.FromArgb(32, 96, 144, 240),
            LinearGradientMode.ForwardDiagonal))
        {
            g.FillRectangle(brush, selection);
        }
        using (Brush brush = new LinearGradientBrush(selection,
            Color.FromArgb(96, 144, 240), Color.FromArgb(128, 96, 144, 240),
            LinearGradientMode.ForwardDiagonal))
        using (Pen pen = new(brush))
        {
            g.DrawRectangle(pen, selection);
        }
    }
    /// <summary>
    /// Draws the insertion caret for drag and drop operations.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="bounds">The bounding rectangle of the insertion caret.</param>
    public override void DrawInsertionCaret(Graphics g, Rectangle bounds)
    {
        using Brush b = new SolidBrush(Color.FromArgb(96, 144, 240));
        g.FillRectangle(b, bounds);
    }
    /// <summary>
    /// Draws the group headers.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="name">The name of the group to draw.</param>
    /// <param name="bounds">The bounding rectangle of group in client coordinates.</param>
    public override void DrawGroupHeader(Graphics g, string name, Rectangle bounds)
    {
        // Bottom border
        bounds.Inflate(0, -4);
        using (Pen pSpep = new(Color.FromArgb(64, 64, 64)))
        {
            g.DrawLine(pSpep, bounds.Left + 1, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
        }

        // Text
        if (bounds.Width > 4)
        {
            using StringFormat sf = new();
            sf.FormatFlags = StringFormatFlags.NoWrap;
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            sf.Trimming = StringTrimming.EllipsisCharacter;
            using SolidBrush bText = new(Color.White);
            g.DrawString(name, ImageListView.GroupHeaderFont == null ? ImageListView.Font : ImageListView.GroupHeaderFont, bText, bounds, sf);
        }
    }

    /// <summary>
    /// Draws an image with a reflection effect at the bottom.
    /// </summary>
    /// <param name="g">The graphics to draw on.</param>
    /// <param name="img">The image to draw.</param>
    /// <param name="x">The x coordinate of the upper left corner of the image.</param>
    /// <param name="y">The y coordinate of the upper left corner of the image.</param>
    /// <param name="width">Width of the drawn image.</param>
    /// <param name="height">Height of the drawn image.</param>
    /// <param name="reflection">Height of the reflection.</param>
    private void DrawImageWithReflection(Graphics g, Image img, int x, int y, int width, int height, int reflection)
    {
        // Draw the image
        g.DrawImage(img, x, y + 1, width, height);

        // Draw the reflection
        if (img.Width > 32 && img.Height > 32)
        {
            int reflectionHeight = height / 2;
            if (reflectionHeight > reflection) reflectionHeight = reflection;

            Region prevClip = g.Clip;
            g.SetClip(new Rectangle(x, y + height + 1, width, reflectionHeight));
            g.DrawImage(img, x, y + height + height / 2 + 1, width, -height / 2);
            g.Clip = prevClip;

            using Brush brush = new LinearGradientBrush(
                new Point(x, y + height + 1), new Point(x, y + height + reflectionHeight + 1),
                Color.FromArgb(128, 0, 0, 0), Color.Black);
            g.FillRectangle(brush, x, y + height + 1, width, reflectionHeight);
        }
    }
    /// <summary>
    /// Draws an image with a reflection effect at the bottom.
    /// </summary>
    /// <param name="g">The graphics to draw on.</param>
    /// <param name="img">The image to draw.</param>
    /// <param name="bounds">The target bounding rectangle.</param>
    /// <param name="reflection">Height of the reflection.</param>
    private void DrawImageWithReflection(Graphics g, Image img, Rectangle bounds, int reflection)
    {
        DrawImageWithReflection(g, img, bounds.X, bounds.Y, bounds.Width, bounds.Height, reflection);
    }
}
#endregion

#region TilesRenderer
/// <summary>
/// Displays items with large tiles.
/// </summary>
public class TilesRenderer : ImageListView.ImageListViewRenderer
{
    private Font mCaptionFont;
    private int mTileWidth;
    private int mTextHeight;

    /// <summary>
    /// Gets or sets the width of the tile.
    /// </summary>
    public int TileWidth { get { return mTileWidth; } set { mTileWidth = value; } }

    private Font CaptionFont
    {
        get
        {
            if (mCaptionFont == null)
                mCaptionFont = new Font(ImageListView.Font, FontStyle.Bold);
            return mCaptionFont;
        }
    }

    /// <summary>
    /// Initializes a new instance of the TilesRenderer class.
    /// </summary>
    public TilesRenderer()
        : this(150)
    {
        ;
    }

    /// <summary>
    /// Initializes a new instance of the TilesRenderer class.
    /// </summary>
    /// <param name="tileWidth">Width of tiles in pixels.</param>
    public TilesRenderer(int tileWidth)
    {
        mTileWidth = tileWidth;
    }

    /// <summary>
    /// Releases managed resources.
    /// </summary>
    public override void Dispose()
    {
        if (mCaptionFont != null)
            mCaptionFont.Dispose();

        base.Dispose();
    }
    /// <summary>
    /// Returns item size for the given view mode.
    /// </summary>
    /// <param name="view">The view mode for which the item measurement should be made.</param>
    /// <returns>The item size.</returns>
    public override Size MeasureItem(View view)
    {
        if (view == View.Thumbnails)
        {
            Size itemSize = new();
            mTextHeight = (int)(5.8f * CaptionFont.Height);

            // Calculate item size
            Size itemPadding = new(4, 4);
            itemSize.Width = ImageListView.ThumbnailSize.Width + 4 * itemPadding.Width + mTileWidth;
            itemSize.Height = Math.Max(mTextHeight, ImageListView.ThumbnailSize.Height) + 2 * itemPadding.Height;
            return itemSize;
        } else
            return base.MeasureItem(view);
    }
    /// <summary>
    /// Draws the specified item on the given graphics.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="state">The current view state of item.</param>
    /// <param name="bounds">The bounding rectangle of item in client coordinates.</param>
    public override void DrawItem(Graphics g, ImageListViewItem item, ItemState state, Rectangle bounds)
    {
        if (ImageListView.View == View.Thumbnails)
        {
            Size itemPadding = new(4, 4);

            // Paint background
            if (ImageListView.Enabled)
            {
                using Brush bItemBack = new SolidBrush(ImageListView.Colors.BackColor);
                g.FillRectangle(bItemBack, bounds);
            } else
            {
                using Brush bItemBack = new SolidBrush(ImageListView.Colors.DisabledBackColor);
                g.FillRectangle(bItemBack, bounds);
            }

            // Paint background Disabled
            if ((state & ItemState.Disabled) != ItemState.None)
            {
                using Brush bDisabled = new LinearGradientBrush(bounds, ImageListView.Colors.DisabledColor1, ImageListView.Colors.DisabledColor2, LinearGradientMode.Vertical);
                Utility.FillRoundedRectangle(g, bDisabled, bounds, 4);
            } else if (ImageListView.Focused && (state & ItemState.Selected) != ItemState.None ||
                  !ImageListView.Focused && (state & ItemState.Selected) != ItemState.None && (state & ItemState.Hovered) != ItemState.None)
            {
                using Brush bSelected = new LinearGradientBrush(bounds, ImageListView.Colors.SelectedColor1, ImageListView.Colors.SelectedColor2, LinearGradientMode.Vertical);
                Utility.FillRoundedRectangle(g, bSelected, bounds, 4);
            } else if (!ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
            {
                using Brush bGray64 = new LinearGradientBrush(bounds, ImageListView.Colors.UnFocusedColor1, ImageListView.Colors.UnFocusedColor2, LinearGradientMode.Vertical);
                Utility.FillRoundedRectangle(g, bGray64, bounds, 4);
            }
            if ((state & ItemState.Hovered) != ItemState.None)
            {
                using Brush bHovered = new LinearGradientBrush(bounds, ImageListView.Colors.HoverColor1, ImageListView.Colors.HoverColor2, LinearGradientMode.Vertical);
                Utility.FillRoundedRectangle(g, bHovered, bounds, 4);
            }

            // Draw the image
            Image img = item.GetCachedImage(CachedImageType.Thumbnail);
            if (img != null)
            {
                Rectangle pos = Utility.GetSizedImageBounds(img, new Rectangle(bounds.Location + itemPadding, ImageListView.ThumbnailSize), 0.0f, 50.0f);
                g.DrawImage(img, pos);
                // Draw image border
                if (Math.Min(pos.Width, pos.Height) > 32)
                {
                    using (Pen pOuterBorder = new(ImageListView.Colors.ImageOuterBorderColor))
                    {
                        g.DrawRectangle(pOuterBorder, pos);
                    }
                    if (Math.Min(ImageListView.ThumbnailSize.Width, ImageListView.ThumbnailSize.Height) > 32)
                    {
                        using Pen pInnerBorder = new(ImageListView.Colors.ImageInnerBorderColor);
                        g.DrawRectangle(pInnerBorder, Rectangle.Inflate(pos, -1, -1));
                    }
                }
            }

            // Draw item text
            int lineHeight = CaptionFont.Height;
            RectangleF rt;
            using (StringFormat sf = new())
            {
                rt = new RectangleF(bounds.Left + 2 * itemPadding.Width + ImageListView.ThumbnailSize.Width,
                    bounds.Top + itemPadding.Height + (Math.Max(ImageListView.ThumbnailSize.Height, mTextHeight) - mTextHeight) / 2,
                    mTileWidth, lineHeight);
                sf.Alignment = StringAlignment.Near;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                Color foreColor = ImageListView.Colors.ForeColor;
                if ((state & ItemState.Disabled) != ItemState.None)
                {
                    foreColor = ImageListView.Colors.DisabledForeColor;
                }
                using (Brush bItemFore = new SolidBrush(foreColor))
                {
                    g.DrawString(item.Text, CaptionFont, bItemFore, rt, sf);
                }
                using Brush bItemDetails = new SolidBrush(ImageListView.Colors.PaneLabelColor);
                rt.Offset(0, 1.5f * lineHeight);
                string fileType = item.GetSubItemText(ColumnType.FileType);
                if (!string.IsNullOrEmpty(fileType))
                {
                    g.DrawString(fileType, ImageListView.Font, bItemDetails, rt, sf);
                    rt.Offset(0, 1.1f * lineHeight);
                }
                string dimensions = item.GetSubItemText(ColumnType.Dimensions);
                string resolution = item.GetSubItemText(ColumnType.Resolution);
                if (!string.IsNullOrEmpty(dimensions) || !string.IsNullOrEmpty(resolution))
                {
                    string text = "";
                    if (!string.IsNullOrEmpty(dimensions))
                        text += dimensions + " pixels ";
                    if (!string.IsNullOrEmpty(resolution))
                        text += resolution.Split(new char[] { ' ', 'x' }, StringSplitOptions.RemoveEmptyEntries)[0] + " dpi";
                    g.DrawString(text, ImageListView.Font, bItemDetails, rt, sf);
                    rt.Offset(0, 1.1f * lineHeight);
                }
                string fileSize = item.GetSubItemText(ColumnType.FileSize);
                if (!string.IsNullOrEmpty(fileSize))
                {
                    g.DrawString(fileSize, ImageListView.Font, bItemDetails, rt, sf);
                    rt.Offset(0, 1.1f * lineHeight);
                }
                string dateModified = item.GetSubItemText(ColumnType.DateModified);
                if (!string.IsNullOrEmpty(dateModified))
                {
                    g.DrawString(dateModified, ImageListView.Font, bItemDetails, rt, sf);
                }
            }

            // Item border
            using (Pen pWhite128 = new(Color.FromArgb(128, ImageListView.Colors.ControlBackColor)))
            {
                Utility.DrawRoundedRectangle(g, pWhite128, bounds.Left + 1, bounds.Top + 1, bounds.Width - 3, bounds.Height - 3, 4);
            }
            if ((state & ItemState.Disabled) != ItemState.None)
            {
                using Pen pHighlight128 = new(ImageListView.Colors.DisabledBorderColor);
                Utility.DrawRoundedRectangle(g, pHighlight128, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, 4);
            } else if (ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
            {
                using Pen pHighlight128 = new(ImageListView.Colors.SelectedBorderColor);
                Utility.DrawRoundedRectangle(g, pHighlight128, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, 4);
            } else if (!ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
            {
                using Pen pGray128 = new(ImageListView.Colors.UnFocusedBorderColor);
                Utility.DrawRoundedRectangle(g, pGray128, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, 4);
            } else if ((state & ItemState.Selected) == ItemState.None)
            {
                using Pen pGray64 = new(ImageListView.Colors.BorderColor);
                Utility.DrawRoundedRectangle(g, pGray64, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, 4);
            }

            if (ImageListView.Focused && (state & ItemState.Hovered) != ItemState.None)
            {
                using Pen pHighlight64 = new(ImageListView.Colors.HoverBorderColor);
                Utility.DrawRoundedRectangle(g, pHighlight64, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, 4);
            }

            // Focus rectangle
            if (ImageListView.Focused && (state & ItemState.Focused) != ItemState.None)
            {
                ControlPaint.DrawFocusRectangle(g, bounds);
            }
        } else
            base.DrawItem(g, item, state, bounds);
    }
}
#endregion

#region XPRenderer
/// <summary>
/// Mimics Windows XP appearance.
/// This renderer cannot be themed.
/// </summary>
public class XPRenderer : ImageListView.ImageListViewRenderer
{
    /// <summary>
    /// Gets a value indicating whether this renderer can apply custom colors.
    /// </summary>
    /// <value></value>
    public override bool CanApplyColors { get { return false; } }

    /// <summary>
    /// Returns item size for the given view mode.
    /// </summary>
    /// <param name="view">The view mode for which the item measurement should be made.</param>
    /// <returns>The item size.</returns>
    public override Size MeasureItem(View view)
    {
        Size itemSize = new();

        // Reference text height
        int textHeight = ImageListView.Font.Height;

        if (view == View.Details)
            return base.MeasureItem(view);
        else
        {
            // Calculate item size
            Size itemPadding = new(4, 4);
            itemSize = ImageListView.ThumbnailSize + itemPadding + itemPadding;
            itemSize.Height += textHeight + Math.Max(4, textHeight / 3) + itemPadding.Height; // textHeight / 3 = vertical space between thumbnail and text
            return itemSize;
        }
    }
    /// <summary>
    /// Draws the background of the control.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="bounds">The client coordinates of the item area.</param>
    public override void DrawBackground(Graphics g, Rectangle bounds)
    {
        if (ImageListView.Enabled)
            g.Clear(SystemColors.Window);
        else
            g.Clear(SystemColors.Control);
    }
    /// <summary>
    /// Draws the specified item on the given graphics.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="state">The current view state of item.</param>
    /// <param name="bounds">The bounding rectangle of item in client coordinates.</param>
    public override void DrawItem(Graphics g, ImageListViewItem item, ItemState state, Rectangle bounds)
    {
        // Paint background
        if (ImageListView.Enabled || !item.Enabled)
            g.FillRectangle(SystemBrushes.Window, bounds);
        else
            g.FillRectangle(SystemBrushes.Control, bounds);

        if (ImageListView.View != View.Details)
        {
            Size itemPadding = new(4, 4);

            // Draw the image
            Image img = item.GetCachedImage(CachedImageType.Thumbnail);
            if (img != null)
            {
                Rectangle border = new(bounds.Location + itemPadding, ImageListView.ThumbnailSize);
                Rectangle pos = Utility.GetSizedImageBounds(img, border);
                g.DrawImage(img, pos);

                // Draw image border
                if (ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
                {
                    using Pen pen = new(SystemColors.Highlight, 3);
                    g.DrawRectangle(pen, border);
                } else if (!ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
                {
                    using Pen pen = new(SystemColors.GrayText, 3);
                    pen.Alignment = PenAlignment.Center;
                    g.DrawRectangle(pen, border);
                } else
                {
                    using Pen pGray128 = new(Color.FromArgb(128, SystemColors.GrayText));
                    g.DrawRectangle(pGray128, border);
                }
            }

            // Draw item text
            SizeF szt = TextRenderer.MeasureText(g, item.Text, ImageListView.Font);
            RectangleF rt;
            using (StringFormat sf = new())
            {
                rt = new RectangleF(bounds.Left + itemPadding.Width, bounds.Top + 3 * itemPadding.Height + ImageListView.ThumbnailSize.Height, ImageListView.ThumbnailSize.Width, szt.Height);
                sf.Alignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                rt.Width += 1;
                rt.Inflate(1, 2);
                if (ImageListView.Focused && (state & ItemState.Focused) != ItemState.None)
                    rt.Inflate(-1, -1);
                if (ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
                {
                    g.FillRectangle(SystemBrushes.Highlight, rt);
                } else if (!ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
                {
                    g.FillRectangle(SystemBrushes.GrayText, rt);
                }
                if ((state & ItemState.Disabled) != ItemState.None)
                {
                    g.DrawString(item.Text, ImageListView.Font, SystemBrushes.GrayText, rt, sf);
                } else if ((state & ItemState.Selected) != ItemState.None)
                {
                    g.DrawString(item.Text, ImageListView.Font, SystemBrushes.HighlightText, rt, sf);
                } else
                {
                    g.DrawString(item.Text, ImageListView.Font, SystemBrushes.WindowText, rt, sf);
                }
            }

            if (ImageListView.Focused && (state & ItemState.Focused) != ItemState.None)
            {
                Rectangle fRect = Rectangle.Round(rt);
                fRect.Inflate(1, 1);
                ControlPaint.DrawFocusRectangle(g, fRect);
            }
        } else // if (ImageListView.View == Manina.Windows.Forms.View.Details)
        {
            if (ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
            {
                g.FillRectangle(SystemBrushes.Highlight, bounds);
            } else if (!ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
            {
                g.FillRectangle(SystemBrushes.GrayText, bounds);
            }

            Size offset = new(2, (bounds.Height - ImageListView.Font.Height) / 2);
            using (StringFormat sf = new())
            {
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                // Sub text
                List<ImageListView.ImageListViewColumnHeader> uicolumns = ImageListView.Columns.GetDisplayedColumns();
                RectangleF rt = new(bounds.Left + offset.Width, bounds.Top + offset.Height, uicolumns[0].Width - 2 * offset.Width, bounds.Height - 2 * offset.Height);
                foreach (ImageListView.ImageListViewColumnHeader column in uicolumns)
                {
                    rt.Width = column.Width - 2 * offset.Width;
                    int iconOffset = 0;
                    if (column.Type == ColumnType.Name)
                    {
                        // Allocate space for checkbox and file icon
                        if (ImageListView.ShowCheckBoxes && ImageListView.ShowFileIcons)
                            iconOffset += 2 * 16 + 3 * 2;
                        else if (ImageListView.ShowCheckBoxes)
                            iconOffset += 16 + 2 * 2;
                        else if (ImageListView.ShowFileIcons)
                            iconOffset += 16 + 2 * 2;
                    }
                    rt.X += iconOffset;
                    rt.Width -= iconOffset;

                    Brush forecolor = SystemBrushes.WindowText;
                    if ((state & ItemState.Disabled) != ItemState.None)
                        forecolor = SystemBrushes.GrayText;
                    else if ((state & ItemState.Selected) != ItemState.None)
                        forecolor = SystemBrushes.HighlightText;

                    if (column.Type == ColumnType.Rating && ImageListView.RatingImage != null && ImageListView.EmptyRatingImage != null)
                    {
                        int w = ImageListView.RatingImage.Width;
                        int y = (int)(rt.Top + (rt.Height - ImageListView.RatingImage.Height) / 2.0f);
                        int rating = item.StarRating;
                        if (rating < 0) rating = 0;
                        if (rating > 5) rating = 5;
                        for (int i = 1; i <= rating; i++)
                            g.DrawImage(ImageListView.RatingImage, rt.Left + (i - 1) * w, y);
                        for (int i = rating + 1; i <= 5; i++)
                            g.DrawImage(ImageListView.EmptyRatingImage, rt.Left + (i - 1) * w, y);
                    } else
                        g.DrawString(item.SubItems[column].Text, ImageListView.Font, forecolor, rt, sf);

                    rt.X -= iconOffset;
                    rt.X += column.Width;
                }
            }

            if (ImageListView.Focused && (state & ItemState.Focused) != ItemState.None)
                ControlPaint.DrawFocusRectangle(g, bounds);
        }
    }
    /// <summary>
    /// Draws the large preview image of the focused item in Gallery mode.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="image">The image to draw.</param>
    /// <param name="bounds">The bounding rectangle of the preview area.</param>
    public override void DrawGalleryImage(Graphics g, ImageListViewItem item, Image image, Rectangle bounds)
    {
        if (item != null && image != null)
        {
            // Calculate image bounds
            Size itemMargin = MeasureItemMargin(ImageListView.View);
            Rectangle pos = Utility.GetSizedImageBounds(image, new Rectangle(bounds.Location + itemMargin, bounds.Size - itemMargin - itemMargin));
            // Draw image
            g.DrawImage(image, pos);

            // Draw image border
            if (Math.Min(pos.Width, pos.Height) > 32)
            {
                g.DrawRectangle(SystemPens.WindowText, pos);
            }
        }
    }
    /// <summary>
    /// Draws the left pane in Pane view mode.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="image">The image to draw.</param>
    /// <param name="bounds">The bounding rectangle of the pane.</param>
    public override void DrawPane(Graphics g, ImageListViewItem item, Image image, Rectangle bounds)
    {
        // Draw pane background
        if (ImageListView.Enabled)
            g.FillRectangle(SystemBrushes.Window, bounds);
        else
            g.FillRectangle(SystemBrushes.Control, bounds);

        using (Brush bBorder = new SolidBrush(Color.FromArgb(128, SystemColors.GrayText)))
        {
            g.FillRectangle(bBorder, bounds.Right - 2, bounds.Top, 2, bounds.Height);
        }
        bounds.Width -= 2;

        if (item != null && image != null)
        {
            // Calculate image bounds
            Size itemMargin = MeasureItemMargin(ImageListView.View);
            Rectangle pos = Utility.GetSizedImageBounds(image, new Rectangle(bounds.Location + itemMargin, bounds.Size - itemMargin - itemMargin), 50.0f, 0.0f);
            // Draw image
            g.DrawImage(image, pos);
            // Draw image border
            if (Math.Min(pos.Width, pos.Height) > 32)
            {
                using Pen pBorder = new(SystemColors.WindowText);
                g.DrawRectangle(pBorder, pos);
            }
            bounds.X += itemMargin.Width;
            bounds.Width -= 2 * itemMargin.Width;
            bounds.Y = pos.Height + 16;
            bounds.Height -= pos.Height + 16;

            // Item text
            if (ImageListView.Columns.HasType(ColumnType.Name) && ImageListView.Columns[ColumnType.Name].Visible && bounds.Height > 0)
            {
                string text = item.GetSubItemText(ColumnType.Name);
                int y = Utility.DrawStringPair(g, bounds, "", text, ImageListView.Font, SystemBrushes.GrayText, SystemBrushes.WindowText);
                bounds.Y += 2 * y;
                bounds.Height -= 2 * y;
            }

            // File type
            string fileType = item.GetSubItemText(ColumnType.FileType);
            if (ImageListView.Columns.HasType(ColumnType.FileType) && ImageListView.Columns[ColumnType.FileType].Visible && bounds.Height > 0 && !string.IsNullOrEmpty(fileType))
            {
                int y = Utility.DrawStringPair(g, bounds, ImageListView.Columns[ColumnType.FileType].Text + ": ",
                    fileType, ImageListView.Font, SystemBrushes.GrayText, SystemBrushes.WindowText);
                bounds.Y += y;
                bounds.Height -= y;
            }

            // Metatada
            foreach (ImageListView.ImageListViewColumnHeader column in ImageListView.Columns)
            {
                if (column.Type == ColumnType.ImageDescription)
                {
                    bounds.Y += 8;
                    bounds.Height -= 8;
                }

                if (bounds.Height <= 0) break;

                if (column.Visible &&
                    column.Type != ColumnType.Custom &&
                    column.Type != ColumnType.FileType &&
                    column.Type != ColumnType.DateAccessed &&
                    column.Type != ColumnType.FileName &&
                    column.Type != ColumnType.FilePath &&
                    column.Type != ColumnType.Name)
                {
                    string caption = column.Text;
                    string text = item.GetSubItemText(column.Type);
                    if (!string.IsNullOrEmpty(text))
                    {
                        int y = Utility.DrawStringPair(g, bounds, caption + ": ", text,
                            ImageListView.Font, SystemBrushes.GrayText, SystemBrushes.WindowText);
                        bounds.Y += y;
                        bounds.Height -= y;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Draws the column headers.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="column">The ImageListViewColumnHeader to draw.</param>
    /// <param name="state">The current view state of column.</param>
    /// <param name="bounds">The bounding rectangle of column in client coordinates.</param>
    public override void DrawColumnHeader(Graphics g, ImageListView.ImageListViewColumnHeader column, ColumnState state, Rectangle bounds)
    {
        // Paint background
        if (ImageListView.Focused && (state & ColumnState.Hovered) == ColumnState.Hovered)
        {
            using Brush bHovered = new LinearGradientBrush(bounds, Color.FromArgb(16, SystemColors.Highlight), Color.FromArgb(64, SystemColors.Highlight), LinearGradientMode.Vertical);
            g.FillRectangle(bHovered, bounds);
        } else
        {
            using Brush bNormal = new LinearGradientBrush(bounds, Color.FromArgb(32, SystemColors.Control), Color.FromArgb(196, SystemColors.Control), LinearGradientMode.Vertical);
            g.FillRectangle(bNormal, bounds);
        }
        using (Brush bBorder = new LinearGradientBrush(bounds, Color.FromArgb(196, SystemColors.Control), Color.FromArgb(32, SystemColors.Control), LinearGradientMode.Vertical))
        using (Pen pBorder = new(bBorder))
        {
            g.DrawLine(pBorder, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
            g.DrawLine(pBorder, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
        }
        using (Pen pSpep = new(Color.FromArgb(32, SystemColors.Control)))
        {
            g.DrawLine(pSpep, bounds.Left + 1, bounds.Top + 1, bounds.Left + 1, bounds.Bottom - 2);
            g.DrawLine(pSpep, bounds.Right - 1, bounds.Top + 1, bounds.Right - 1, bounds.Bottom - 2);
        }

        // Draw the sort arrow
        int textOffset = 4;
        if (ImageListView.SortOrder != SortOrder.None && (state & ColumnState.SortColumn) != ColumnState.None)
        {
            Image img = null;
            if (ImageListView.SortOrder == SortOrder.Ascending || ImageListView.SortOrder == SortOrder.AscendingNatural)
                img = ImageListViewResources.SortAscending;
            else if (ImageListView.SortOrder == SortOrder.Descending || ImageListView.SortOrder == SortOrder.DescendingNatural)
                img = ImageListViewResources.SortDescending;
            if (img != null)
            {
                g.DrawImageUnscaled(img, bounds.X + 4, bounds.Top + (bounds.Height - img.Height) / 2);
                textOffset += img.Width;
            }
        }

        // Text
        bounds.X += textOffset;
        bounds.Width -= textOffset;
        if (bounds.Width > 4)
        {
            using StringFormat sf = new();
            sf.FormatFlags = StringFormatFlags.NoWrap;
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            sf.Trimming = StringTrimming.EllipsisCharacter;
            g.DrawString(column.Text, ImageListView.ColumnHeaderFont == null ? ImageListView.Font : ImageListView.ColumnHeaderFont,
                SystemBrushes.WindowText, bounds, sf);
        }
    }
    /// <summary>
    /// Draws the extender after the last column.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="bounds">The bounding rectangle of extender column in client coordinates.</param>
    public override void DrawColumnExtender(Graphics g, Rectangle bounds)
    {
        // Paint background
        using (Brush bBack = new LinearGradientBrush(bounds, Color.FromArgb(32, SystemColors.Control), Color.FromArgb(196, SystemColors.Control), LinearGradientMode.Vertical))
        {
            g.FillRectangle(bBack, bounds);
        }
        using (Brush bBorder = new LinearGradientBrush(bounds, Color.FromArgb(196, SystemColors.Control), Color.FromArgb(32, SystemColors.Control), LinearGradientMode.Vertical))
        using (Pen pBorder = new(bBorder))
        {
            g.DrawLine(pBorder, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
            g.DrawLine(pBorder, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
        }
        using Pen pSpep = new(Color.FromArgb(32, SystemColors.Control));
        g.DrawLine(pSpep, bounds.Left + 1, bounds.Top + 1, bounds.Left + 1, bounds.Bottom - 2);
    }
    /// <summary>
    /// Draws the selection rectangle.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="selection">The client coordinates of the selection rectangle.</param>
    public override void DrawSelectionRectangle(Graphics g, Rectangle selection)
    {
        using SolidBrush brush = new(Color.FromArgb(128, SystemColors.Highlight));
        using Pen pen = new(SystemColors.Highlight);
        g.FillRectangle(brush, selection);
        g.DrawRectangle(pen, selection);
    }
    /// <summary>
    /// Draws the insertion caret for drag and drop operations.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="bounds">The bounding rectangle of the insertion caret.</param>
    public override void DrawInsertionCaret(Graphics g, Rectangle bounds)
    {
        g.FillRectangle(SystemBrushes.Highlight, bounds);
    }
    /// <summary>
    /// Draws the group headers.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="name">The name of the group to draw.</param>
    /// <param name="bounds">The bounding rectangle of group in client coordinates.</param>
    public override void DrawGroupHeader(Graphics g, string name, Rectangle bounds)
    {
        // Bottom border
        bounds.Inflate(0, -4);
        using (Pen pSpep = new(Color.FromArgb(128, SystemColors.GrayText)))
        {
            g.DrawLine(pSpep, bounds.Left + 1, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
        }

        // Text
        if (bounds.Width > 4)
        {
            using StringFormat sf = new();
            sf.FormatFlags = StringFormatFlags.NoWrap;
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            sf.Trimming = StringTrimming.EllipsisCharacter;
            g.DrawString(name, ImageListView.GroupHeaderFont == null ? ImageListView.Font : ImageListView.GroupHeaderFont,
                SystemBrushes.WindowText, bounds, sf);
        }
    }
}
#endregion

#region ZoomingRenderer
/// <summary>
/// Zooms items on mouse over.
/// </summary>
public class ZoomingRenderer : ImageListView.ImageListViewRenderer
{
    private float mZoomRatio;

    /// <summary>
    /// Gets or sets the relative zoom ratio.
    /// </summary>
    public float ZoomRatio
    {
        get
        {
            return mZoomRatio;
        }
        set
        {
            mZoomRatio = value;
            if (mZoomRatio < 0.0f) mZoomRatio = 0.0f;
        }
    }

    /// <summary>
    /// Initializes a new instance of the ZoomingRenderer class.
    /// </summary>
    public ZoomingRenderer()
        : this(0.5f)
    {
        ;
    }

    /// <summary>
    /// Initializes a new instance of the ZoomingRenderer class.
    /// </summary>
    /// <param name="zoomRatio">Relative zoom ratio.</param>
    public ZoomingRenderer(float zoomRatio)
    {
        if (zoomRatio < 0.0f) zoomRatio = 0.0f;
        mZoomRatio = zoomRatio;
    }

    /// <summary>
    /// Initializes the System.Drawing.Graphics used to draw
    /// control elements.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    public override void InitializeGraphics(Graphics g)
    {
        base.InitializeGraphics(g);

        ItemDrawOrder = ItemDrawOrder.NormalSelectedHovered;
    }
    /// <summary>
    /// Returns item size for the given view mode.
    /// </summary>
    /// <param name="view">The view mode for which the item measurement should be made.</param>
    /// <returns>The item size.</returns>
    public override Size MeasureItem(View view)
    {
        return view == View.Thumbnails ? ImageListView.ThumbnailSize + new Size(8, 8) : base.MeasureItem(view);
    }
    /// <summary>
    /// Draws the specified item on the given graphics.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="state">The current view state of item.</param>
    /// <param name="bounds">The bounding rectangle of item in client coordinates.</param>
    public override void DrawItem(Graphics g, ImageListViewItem item, ItemState state, Rectangle bounds)
    {
        Clip = ImageListView.View == View.Details;

        if (ImageListView.View != View.Details)
        {
            Rectangle controlBounds = ClientBounds;

            // Zoom on mouse over
            if ((state & ItemState.Hovered) != ItemState.None)
            {
                bounds.Inflate((int)(bounds.Width * mZoomRatio), (int)(bounds.Height * mZoomRatio));
                if (bounds.Bottom > controlBounds.Bottom)
                    bounds.Y = controlBounds.Bottom - bounds.Height;
                if (bounds.Top < controlBounds.Top)
                    bounds.Y = controlBounds.Top;
                if (bounds.Right > controlBounds.Right)
                    bounds.X = controlBounds.Right - bounds.Width;
                if (bounds.Left < controlBounds.Left)
                    bounds.X = controlBounds.Left;
            }

            // Get item image
            Image img = null;
            if ((state & ItemState.Hovered) != ItemState.None)
                img = GetImageAsync(item, new Size(bounds.Width - 8, bounds.Height - 8));
            if (img == null) img = item.GetCachedImage(CachedImageType.Thumbnail);

            int imageWidth = 0;
            int imageHeight = 0;
            if (img != null)
            {
                // Calculate image bounds
                Rectangle pos = Utility.GetSizedImageBounds(img, Rectangle.Inflate(bounds, -4, -4));
                imageWidth = pos.Width;
                imageHeight = pos.Height;
                int imageX = pos.X;
                int imageY = pos.Y;

                // Allocate space for item text
                if ((state & ItemState.Hovered) != ItemState.None &&
                    (bounds.Height - imageHeight) / 2 < ImageListView.Font.Height + 8)
                {
                    int delta = ImageListView.Font.Height + 8 - (bounds.Height - imageHeight) / 2;
                    bounds.Height += 2 * delta;
                    imageY += delta;

                    delta = 0;
                    if (bounds.Bottom > controlBounds.Bottom)
                        delta = bounds.Y - (controlBounds.Bottom - bounds.Height);
                    if (bounds.Top < controlBounds.Top)
                        delta = bounds.Y - controlBounds.Top;

                    bounds.Y -= delta;
                    imageY -= delta;
                }

                // Paint background
                if (ImageListView.Enabled)
                {
                    using Brush bItemBack = new SolidBrush(ImageListView.Colors.BackColor);
                    g.FillRectangle(bItemBack, bounds);
                } else
                {
                    using Brush bItemBack = new SolidBrush(ImageListView.Colors.DisabledBackColor);
                    g.FillRectangle(bItemBack, bounds);
                }

                if ((state & ItemState.Disabled) != ItemState.None)
                {
                    using Brush bDisabled = new LinearGradientBrush(bounds, ImageListView.Colors.DisabledColor1, ImageListView.Colors.DisabledColor2, LinearGradientMode.Vertical);
                    Utility.FillRoundedRectangle(g, bDisabled, bounds, 5);
                } else if (ImageListView.Focused && (state & ItemState.Selected) != ItemState.None ||
                      !ImageListView.Focused && (state & ItemState.Selected) != ItemState.None && (state & ItemState.Hovered) != ItemState.None)
                {
                    using Brush bSelected = new LinearGradientBrush(bounds, ImageListView.Colors.SelectedColor1, ImageListView.Colors.SelectedColor2, LinearGradientMode.Vertical);
                    Utility.FillRoundedRectangle(g, bSelected, bounds, 5);
                } else if (!ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
                {
                    using Brush bGray64 = new LinearGradientBrush(bounds, ImageListView.Colors.UnFocusedColor1, ImageListView.Colors.UnFocusedColor2, LinearGradientMode.Vertical);
                    Utility.FillRoundedRectangle(g, bGray64, bounds, 5);
                }
                if ((state & ItemState.Hovered) != ItemState.None)
                {
                    using Brush bHovered = new LinearGradientBrush(bounds, ImageListView.Colors.HoverColor1, ImageListView.Colors.HoverColor2, LinearGradientMode.Vertical);
                    Utility.FillRoundedRectangle(g, bHovered, bounds, 5);
                }

                // Draw the image
                g.DrawImage(img, imageX, imageY, imageWidth, imageHeight);

                // Draw image border
                if (Math.Min(imageWidth, imageHeight) > 32)
                {
                    using (Pen pOuterBorder = new(ImageListView.Colors.ImageOuterBorderColor))
                    {
                        g.DrawRectangle(pOuterBorder, imageX, imageY, imageWidth, imageHeight);
                    }
                    if (Math.Min(imageWidth, imageHeight) > 32)
                    {
                        using Pen pInnerBorder = new(ImageListView.Colors.ImageInnerBorderColor);
                        g.DrawRectangle(pInnerBorder, imageX + 1, imageY + 1, imageWidth - 2, imageHeight - 2);
                    }
                }
            } else
            {
                // Paint background
                if (ImageListView.Enabled)
                {
                    using Brush bItemBack = new SolidBrush(ImageListView.Colors.BackColor);
                    g.FillRectangle(bItemBack, bounds);
                } else
                {
                    using Brush bItemBack = new SolidBrush(ImageListView.Colors.DisabledBackColor);
                    g.FillRectangle(bItemBack, bounds);
                }
            }

            // Draw item text
            if ((state & ItemState.Hovered) != ItemState.None)
            {
                RectangleF rt;
                using StringFormat sf = new();
                rt = new RectangleF(bounds.Left + 4, bounds.Top + 4, bounds.Width - 8, (bounds.Height - imageHeight) / 2 - 8);
                sf.Alignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                using (Brush bItemFore = new SolidBrush(ImageListView.Colors.ForeColor))
                {
                    g.DrawString(item.Text, ImageListView.Font, bItemFore, rt, sf);
                }
                rt.Y = bounds.Bottom - (bounds.Height - imageHeight) / 2 + 4;
                string details = "";
                string dimensions = item.GetSubItemText(ColumnType.Dimensions);
                if (!string.IsNullOrEmpty(dimensions))
                    details += dimensions + " pixels ";
                string fileSize = item.GetSubItemText(ColumnType.FileSize);
                if (!string.IsNullOrEmpty(fileSize))
                    details += item.GetSubItemText(ColumnType.FileSize);
                using Brush bGrayText = new SolidBrush(ImageListView.Colors.PaneLabelColor);
                g.DrawString(details, ImageListView.Font, bGrayText, rt, sf);
            }

            // Item border
            using (Pen pWhite128 = new(Color.FromArgb(128, ImageListView.Colors.ControlBackColor)))
            {
                Utility.DrawRoundedRectangle(g, pWhite128, bounds.Left + 1, bounds.Top + 1, bounds.Width - 3, bounds.Height - 3, 4);
            }
            if ((state & ItemState.Disabled) != ItemState.None)
            {
                using Pen pHighlight128 = new(ImageListView.Colors.DisabledBorderColor);
                Utility.DrawRoundedRectangle(g, pHighlight128, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, 4);
            } else if (ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
            {
                using Pen pHighlight128 = new(ImageListView.Colors.SelectedBorderColor);
                Utility.DrawRoundedRectangle(g, pHighlight128, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, 4);
            } else if (!ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
            {
                using Pen pGray128 = new(ImageListView.Colors.UnFocusedBorderColor);
                Utility.DrawRoundedRectangle(g, pGray128, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, 4);
            } else if ((state & ItemState.Selected) == ItemState.None)
            {
                using Pen pGray64 = new(ImageListView.Colors.BorderColor);
                Utility.DrawRoundedRectangle(g, pGray64, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, 4);
            }

            if (ImageListView.Focused && (state & ItemState.Hovered) != ItemState.None)
            {
                using Pen pHighlight64 = new(ImageListView.Colors.HoverBorderColor);
                Utility.DrawRoundedRectangle(g, pHighlight64, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, 4);
            }
        } else
            base.DrawItem(g, item, state, bounds);
    }
    /// <summary>
    /// Draws the checkbox icon for the specified item on the given graphics.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="bounds">The bounding rectangle of the checkbox in client coordinates.</param>
    public override void DrawCheckBox(Graphics g, ImageListViewItem item, Rectangle bounds)
    {
        if (ImageListView.View == View.Details)
            base.DrawCheckBox(g, item, bounds);
    }
    /// <summary>
    /// Draws the file icon for the specified item on the given graphics.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="bounds">The bounding rectangle of the file icon in client coordinates.</param>
    public override void DrawFileIcon(Graphics g, ImageListViewItem item, Rectangle bounds)
    {
        if (ImageListView.View == View.Details)
            base.DrawFileIcon(g, item, bounds);
    }
}
#endregion

#region ThemeRenderer
/// <summary>
/// Displays the control in the current system theme.
/// This renderer cannot be themed.
/// </summary>
public class ThemeRenderer : ImageListView.ImageListViewRenderer
{
    // Check boxes
    private readonly VisualStyleRenderer rCheckedNormal = null;
    private readonly VisualStyleRenderer rUncheckedNormal = null;
    private readonly VisualStyleRenderer rCheckedDisabled = null;
    private readonly VisualStyleRenderer rUncheckedDisabled = null;
    // File icons
    private VisualStyleRenderer rFileIcon = null;
    // Column headers
    private VisualStyleRenderer rColumnNormal = null;
    private readonly VisualStyleRenderer rColumnHovered = null;
    private readonly VisualStyleRenderer rColumnSorted = null;
    private readonly VisualStyleRenderer rColumnSortedHovered = null;
    private readonly VisualStyleRenderer rSortAscending = null;
    private readonly VisualStyleRenderer rSortDescending = null;
    // Items
    private readonly VisualStyleRenderer rItemNormal = null;
    private readonly VisualStyleRenderer rItemHovered = null;
    private readonly VisualStyleRenderer rItemSelected = null;
    private readonly VisualStyleRenderer rItemHoveredSelected = null;
    private readonly VisualStyleRenderer rItemSelectedHidden = null;
    private readonly VisualStyleRenderer rItemDisabled = null;
    // Group headers
    private VisualStyleRenderer rGroupNormal = null;
    private VisualStyleRenderer rGroupLine = null;

    /// <summary>
    /// Gets whether visual styles are supported.
    /// </summary>
    public bool VisualStylesEnabled { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this renderer can apply custom colors.
    /// </summary>
    /// <value></value>
    public override bool CanApplyColors { get { return false; } }

    /// <summary>
    /// Initializes a new instance of the ThemeRenderer class.
    /// </summary>
    public ThemeRenderer()
    {
        VisualStylesEnabled = Application.RenderWithVisualStyles;

        // Create renderers
        if (VisualStylesEnabled)
        {
            // See http://msdn.microsoft.com/en-us/library/bb773210(VS.85).aspx
            // for part and state codes used below.

            // Check boxes
            rCheckedNormal = GetRenderer(VisualStyleElement.Button.CheckBox.CheckedNormal);
            rUncheckedNormal = GetRenderer(VisualStyleElement.Button.CheckBox.UncheckedNormal);
            rCheckedDisabled = GetRenderer(VisualStyleElement.Button.CheckBox.CheckedDisabled);
            rUncheckedDisabled = GetRenderer(VisualStyleElement.Button.CheckBox.UncheckedDisabled);
            // File icons
            rFileIcon = GetRenderer(VisualStyleElement.Button.PushButton.Normal);
            // Column headers
            rColumnNormal = GetRenderer("Header", 1, 1);
            rColumnHovered = GetRenderer("Header", 1, 2);
            rColumnSorted = GetRenderer("Header", 1, 4);
            rColumnSortedHovered = GetRenderer("Header", 1, 5);
            rSortAscending = GetRenderer(VisualStyleElement.Header.SortArrow.SortedUp);
            rSortDescending = GetRenderer(VisualStyleElement.Header.SortArrow.SortedDown);
            // Items
            rItemNormal = GetRenderer("Explorer::ListView", 1, 1);
            rItemHovered = GetRenderer("Explorer::ListView", 1, 2);
            rItemSelected = GetRenderer("Explorer::ListView", 1, 3);
            rItemHoveredSelected = GetRenderer("Explorer::ListView", 1, 6);
            rItemSelectedHidden = GetRenderer("Explorer::ListView", 1, 5);
            rItemDisabled = GetRenderer("Explorer::ListView", 1, 4);
            // Groups
            rGroupNormal = GetRenderer("Explorer::ListView", 6, 1);
            rGroupLine = GetRenderer("Explorer::ListView", 7, 1);
        }
    }

    /// <summary>
    /// Returns a renderer for the given element.
    /// </summary>
    private VisualStyleRenderer GetRenderer(VisualStyleElement e)
    {
        return VisualStyleRenderer.IsElementDefined(e) ? new VisualStyleRenderer(e) : null;
    }

    /// <summary>
    /// Returns a renderer for the given element.
    /// </summary>
    private VisualStyleRenderer GetRenderer(string className, int part, int state)
    {
        VisualStyleElement e = VisualStyleElement.CreateElement(className, part, state);
        return VisualStyleRenderer.IsElementDefined(e) ? new VisualStyleRenderer(e) : null;
    }

    /// <summary>
    /// Draws the checkbox icon for the specified item on the given graphics.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="bounds">The bounding rectangle of the checkbox in client coordinates.</param>
    public override void DrawCheckBox(Graphics g, ImageListViewItem item, Rectangle bounds)
    {
        VisualStyleRenderer renderer = item.Enabled ? item.Checked ? rCheckedNormal : rUncheckedNormal : item.Checked ? rCheckedDisabled : rUncheckedDisabled;
        if (VisualStylesEnabled && renderer != null)
            renderer.DrawBackground(g, bounds, bounds);
        else
            base.DrawCheckBox(g, item, bounds);
    }

    /// <summary>
    /// Draws the file icon for the specified item on the given graphics.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="bounds">The bounding rectangle of the file icon in client coordinates.</param>
    public override void DrawFileIcon(Graphics g, ImageListViewItem item, Rectangle bounds)
    {
        Image icon = item.GetCachedImage(CachedImageType.SmallIcon);

        if (icon != null && VisualStylesEnabled && rFileIcon != null)
            rFileIcon.DrawImage(g, bounds, icon);
        else
            base.DrawFileIcon(g, item, bounds);
    }

    /// <summary>
    /// Draws the column headers.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="column">The ImageListViewColumnHeader to draw.</param>
    /// <param name="state">The current view state of column.</param>
    /// <param name="bounds">The bounding rectangle of column in client coordinates.</param>
    public override void DrawColumnHeader(Graphics g, ImageListView.ImageListViewColumnHeader column, ColumnState state, Rectangle bounds)
    {
        SortOrder order = SortOrder.None;
        if (ImageListView.SortOrder != SortOrder.None &&
            (state & ColumnState.SortColumn) != ColumnState.None)
            order = ImageListView.SortOrder;

        VisualStyleRenderer rBack = (state & ColumnState.Hovered) == ColumnState.Hovered && order != SortOrder.None
            ? rColumnSortedHovered
            : (state & ColumnState.Hovered) == ColumnState.Hovered && order == SortOrder.None
            ? rColumnHovered
            : (state & ColumnState.Hovered) == ColumnState.None && order != SortOrder.None ? rColumnSorted : rColumnNormal;
        VisualStyleRenderer rSort = order == SortOrder.Ascending || order == SortOrder.AscendingNatural ? rSortAscending : rSortDescending;

        // Background
        if (VisualStylesEnabled && rBack != null && rSort != null)
        {
            // Background
            rBack.DrawBackground(g, bounds, bounds);
            // Sort arrow
            if (order != SortOrder.None)
            {
                Size sz = rSort.GetPartSize(g, ThemeSizeType.True);
                Rectangle sortBounds = new(new Point(0, 0), sz);
                sortBounds.Offset(bounds.X + (bounds.Width - sz.Width) / 2, 0);
                rSort.DrawBackground(g, sortBounds, sortBounds);
            }

            // Text
            if (bounds.Width > 4)
            {
                Rectangle textBounds = bounds;
                textBounds.Inflate(-3, 0);
                TextRenderer.DrawText(g, column.Text,
                    SystemFonts.MenuFont, textBounds, SystemColors.ControlText,
                    TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.PreserveGraphicsClipping);
            }
        } else
            base.DrawColumnHeader(g, column, state, bounds);
    }

    /// <summary>
    /// Draws the extender after the last column.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="bounds">The bounding rectangle of extender column in client coordinates.</param>
    public override void DrawColumnExtender(Graphics g, Rectangle bounds)
    {
        if (VisualStylesEnabled && rColumnNormal != null)
            rColumnNormal.DrawBackground(g, bounds, bounds);
        else
            base.DrawColumnExtender(g, bounds);
    }

    /// <summary>
    /// Returns item size for the given view mode.
    /// </summary>
    /// <param name="view">The view mode for which the measurement should be made.</param>
    /// <returns>The item size.</returns>
    public override Size MeasureItem(View view)
    {
        Size sz = base.MeasureItem(view);
        if (VisualStylesEnabled && view != View.Details)
        {
            sz.Width += 6;
            sz.Height += 6;
        }
        return sz;
    }

    /// <summary>
    /// Draws the specified item on the given graphics.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="state">The current view state of item.</param>
    /// <param name="bounds">The bounding rectangle of item in client coordinates.</param>
    public override void DrawItem(Graphics g, ImageListViewItem item, ItemState state, Rectangle bounds)
    {
        VisualStyleRenderer rBack;

        if (!ImageListView.Enabled)
            rBack = rItemSelectedHidden;
        rBack = (state & ItemState.Disabled) != ItemState.None
            ? rItemDisabled
            : !ImageListView.Focused && (state & ItemState.Selected) != ItemState.None
            ? rItemSelectedHidden
            : (state & ItemState.Selected) != ItemState.None && (state & ItemState.Hovered) != ItemState.None
            ? rItemHoveredSelected
            : (state & ItemState.Selected) != ItemState.None
            ? rItemSelected
            : (state & ItemState.Hovered) != ItemState.None ? rItemHovered : rItemNormal;

        if (VisualStylesEnabled && rBack != null)
        {
            // Do not draw the background of normal items
            if ((state & ItemState.Hovered) != ItemState.None || (state & ItemState.Selected) != ItemState.None)
                rBack.DrawBackground(g, bounds, bounds);

            Size itemPadding = new(7, 7);

            // Draw the image
            if (ImageListView.View != View.Details)
            {
                Image img = item.GetCachedImage(CachedImageType.Thumbnail);
                if (img != null)
                {
                    Rectangle pos = Utility.GetSizedImageBounds(img, new Rectangle(bounds.Location + itemPadding, ImageListView.ThumbnailSize));
                    // Image background
                    Rectangle imgback = pos;
                    imgback.Inflate(3, 3);
                    g.FillRectangle(SystemBrushes.Window, imgback);
                    // Image border
                    if (img.Width > 32 && img.Height > 32)
                    {
                        using Pen pen = new(Color.FromArgb(224, 224, 244));
                        g.DrawRectangle(pen, imgback.X, imgback.Y, imgback.Width - 1, imgback.Height - 1);
                    }
                    // Image
                    g.DrawImage(img, pos);
                }

                // Draw item text
                Color foreColor = SystemColors.ControlText;
                if ((state & ItemState.Disabled) != ItemState.None)
                    foreColor = SystemColors.GrayText;
                Size szt = TextRenderer.MeasureText(item.Text, ImageListView.Font);
                Rectangle rt = new(
                    bounds.Left + itemPadding.Width, bounds.Top + 2 * itemPadding.Height + ImageListView.ThumbnailSize.Height,
                    ImageListView.ThumbnailSize.Width, szt.Height);
                TextRenderer.DrawText(g, item.Text, ImageListView.Font, rt, foreColor,
                    TextFormatFlags.EndEllipsis | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.NoPrefix);
            } else // if (ImageListView.View == View.Details)
            {
                List<ImageListView.ImageListViewColumnHeader> uicolumns = ImageListView.Columns.GetDisplayedColumns();

                // Separators 
                int x = bounds.Left - 2;
                foreach (ImageListView.ImageListViewColumnHeader column in uicolumns)
                {
                    x += column.Width;
                    if (!ReferenceEquals(column, uicolumns[uicolumns.Count - 1]))
                    {
                        using Pen pGray32 = new(Color.FromArgb(32, 128, 128, 128));
                        g.DrawLine(pGray32, x, bounds.Top, x, bounds.Bottom);
                    }
                }
                Size offset = new(2, (bounds.Height - ImageListView.Font.Height) / 2);
                // Sub text
                int firstWidth = 0;
                if (uicolumns.Count > 0)
                    firstWidth = uicolumns[0].Width;
                Rectangle rt = new(bounds.Left + offset.Width, bounds.Top + offset.Height, firstWidth - 2 * offset.Width, bounds.Height - 2 * offset.Height);
                Color foreColor = SystemColors.ControlText;
                if ((state & ItemState.Disabled) != ItemState.None)
                    foreColor = SystemColors.GrayText;
                foreach (ImageListView.ImageListViewColumnHeader column in uicolumns)
                {
                    rt.Width = column.Width - 2 * offset.Width;
                    using (Brush bItemFore = new SolidBrush(SystemColors.ControlText))
                    {
                        int iconOffset = 0;
                        if (column.Type == ColumnType.Name)
                        {
                            // Allocate space for checkbox and file icon
                            if (ImageListView.ShowCheckBoxes && ImageListView.ShowFileIcons)
                                iconOffset += 2 * 16 + 3 * 2;
                            else if (ImageListView.ShowCheckBoxes)
                                iconOffset += 16 + 2 * 2;
                            else if (ImageListView.ShowFileIcons)
                                iconOffset += 16 + 2 * 2;
                        }
                        rt.X += iconOffset;
                        rt.Width -= iconOffset;
                        // Rating stars
                        if (column.Type == ColumnType.Rating && ImageListView.RatingImage != null && ImageListView.EmptyRatingImage != null)
                        {
                            int rating = item.GetSimpleRating();
                            if (rating > 0)
                            {
                                int w = ImageListView.RatingImage.Width;
                                int y = (int)(rt.Top + (rt.Height - ImageListView.RatingImage.Height) / 2.0f);

                                for (int i = 1; i <= 5; i++)
                                {
                                    if (rating >= i)
                                        g.DrawImage(ImageListView.RatingImage, rt.Left + (i - 1) * w, y);
                                    else
                                        g.DrawImage(ImageListView.EmptyRatingImage, rt.Left + (i - 1) * w, y);
                                }
                            }
                        } else
                            TextRenderer.DrawText(g, item.SubItems[column].Text, ImageListView.Font, rt, foreColor,
                                TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.NoPrefix);

                        rt.X -= iconOffset;
                    }
                    rt.X += column.Width;
                }
            }

            // Focus rectangle
            if (ImageListView.Focused && (state & ItemState.Focused) != ItemState.None)
            {
                Rectangle focusBounds = bounds;
                focusBounds.Inflate(-2, -2);
                ControlPaint.DrawFocusRectangle(g, focusBounds);
            }

        } else
            base.DrawItem(g, item, state, bounds);
    }
    /// <summary>
    /// Draws the group headers.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="name">The name of the group to draw.</param>
    /// <param name="bounds">The bounding rectangle of group in client coordinates.</param>
    public override void DrawGroupHeader(Graphics g, string name, Rectangle bounds)
    {
        if (VisualStylesEnabled && rGroupNormal != null && rGroupLine != null)
        {
            bounds.Inflate(-3, 0);

            // Background
            rGroupNormal.DrawBackground(g, bounds, bounds);

            // Text
            TextRenderer.DrawText(g, name,
                SystemFonts.MenuFont, bounds, SystemColors.ControlText,
                TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.PreserveGraphicsClipping);

            // Border
            Rectangle lineBounds = new(bounds.Left, bounds.Bottom - 1, bounds.Width, 1);
            rGroupLine.DrawBackground(g, lineBounds, lineBounds);
        } else
            base.DrawGroupHeader(g, name, bounds);
    }
}
#endregion

#region MeerkatRenderer
/// <summary>
/// A renderer to celebrate the release of Ubuntu 10.10 Maverick Meerkat.
/// </summary>
public class MeerkatRenderer : ImageListView.ImageListViewRenderer
{
    /// <summary>
    /// Gets a list of color themes preferred by this renderer.
    /// </summary>
    /// <value></value>
    public override ImageListViewColor[] PreferredColors
    {
        get { return new ImageListViewColor[] { ImageListViewColor.Mandarin }; }
    }

    /// <summary>
    /// Initializes the System.Drawing.Graphics used to draw
    /// control elements.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    public override void InitializeGraphics(Graphics g)
    {
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
    }
    /// <summary>
    /// Returns item size for the given view mode.
    /// </summary>
    /// <param name="view">The view mode for which the measurement should be made.</param>
    /// <returns>The item size.</returns>
    public override Size MeasureItem(View view)
    {
        if (view == View.Details)
            return base.MeasureItem(view);
        else
        {
            // Reference text height
            int textHeight = ImageListView.Font.Height;

            Size itemSize = new();

            itemSize.Height = ImageListView.ThumbnailSize.Height + textHeight + 4 * 3;
            itemSize.Width = ImageListView.ThumbnailSize.Width + 2 * 3;

            return itemSize;
        }
    }
    /// <summary>
    /// Draws the column headers.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="column">The ImageListViewColumnHeader to draw.</param>
    /// <param name="state">The current view state of column.</param>
    /// <param name="bounds">The bounding rectangle of column in client coordinates.</param>
    public override void DrawColumnHeader(Graphics g, ImageListView.ImageListViewColumnHeader column, ColumnState state, Rectangle bounds)
    {
        // Paint background
        if ((state & ColumnState.Hovered) != ColumnState.None)
        {
            using Brush bHovered = new LinearGradientBrush(bounds, ImageListView.Colors.ColumnHeaderHoverColor1, ImageListView.Colors.ColumnHeaderHoverColor2, LinearGradientMode.Vertical);
            g.FillRectangle(bHovered, bounds);
        } else
        {
            using Brush bNormal = new LinearGradientBrush(bounds, ImageListView.Colors.ColumnHeaderBackColor1, ImageListView.Colors.ColumnHeaderBackColor2, LinearGradientMode.Vertical);
            g.FillRectangle(bNormal, bounds);
        }
        using (Pen pBorder = new(ImageListView.Colors.ColumnSeparatorColor))
        {
            g.DrawLine(pBorder, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
            g.DrawLine(pBorder, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
        }
        using (Pen pBorder = new(Color.FromArgb(252, 252, 252)))
        {
            g.DrawRectangle(pBorder, bounds.Left + 1, bounds.Top, bounds.Width - 2, bounds.Height - 2);
        }

        // Draw the sort arrow
        int offset = 4;
        int width = bounds.Width - 2 * offset;
        if (ImageListView.SortOrder != SortOrder.None && (state & ColumnState.SortColumn) != ColumnState.None)
        {
            Image img = null;
            if (ImageListView.SortOrder == SortOrder.Ascending)
                img = ImageListViewResources.SortAscending;
            else if (ImageListView.SortOrder == SortOrder.Descending)
                img = ImageListViewResources.SortDescending;
            if (img != null)
            {
                g.DrawImageUnscaled(img, bounds.Right - offset - img.Width, bounds.Top + (bounds.Height - img.Height) / 2);
                width -= img.Width + offset;
            }
        }

        // Text
        bounds.X += offset;
        bounds.Width = width;
        if (bounds.Width > 4)
        {
            using StringFormat sf = new();
            sf.FormatFlags = StringFormatFlags.NoWrap;
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            sf.Trimming = StringTrimming.EllipsisCharacter;
            using SolidBrush bText = new(ImageListView.Colors.ColumnHeaderForeColor);
            g.DrawString(column.Text, ImageListView.ColumnHeaderFont == null ? ImageListView.Font : ImageListView.ColumnHeaderFont, bText, bounds, sf);
        }
    }
    /// <summary>
    /// Draws the extender after the last column.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="bounds">The bounding rectangle of extender column in client coordinates.</param>
    public override void DrawColumnExtender(Graphics g, Rectangle bounds)
    {
        // Paint background
        using (Brush bNormal = new LinearGradientBrush(bounds, ImageListView.Colors.ColumnHeaderBackColor1, ImageListView.Colors.ColumnHeaderBackColor2, LinearGradientMode.Vertical))
        {
            g.FillRectangle(bNormal, bounds);
        }
        using (Pen pBorder = new(ImageListView.Colors.ColumnSeparatorColor))
        {
            g.DrawLine(pBorder, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
            g.DrawLine(pBorder, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
        }
        using (Pen pBorder = new(Color.FromArgb(252, 252, 252)))
        {
            g.DrawRectangle(pBorder, bounds.Left + 1, bounds.Top, bounds.Width - 2, bounds.Height - 2);
        }
    }
    /// <summary>
    /// Draws the specified item on the given graphics.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="state">The current view state of item.</param>
    /// <param name="bounds">The bounding rectangle of item in client coordinates.</param>
    public override void DrawItem(Graphics g, ImageListViewItem item, ItemState state, Rectangle bounds)
    {
        if (ImageListView.View == View.Details)
        {
            bool alternate = item.Index % 2 == 1;
            List<ImageListView.ImageListViewColumnHeader> uicolumns = ImageListView.Columns.GetDisplayedColumns();

            // Paint background
            if ((state & ItemState.Disabled) != ItemState.None)
            {
                // Disabled
                using Brush bItemBack = new LinearGradientBrush(bounds, ImageListView.Colors.DisabledColor1,
                    ImageListView.Colors.DisabledColor2, LinearGradientMode.Vertical);
                g.FillRectangle(bItemBack, bounds);
            } else if (ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
            {
                // Focused and selected
                using Brush bItemBack = new LinearGradientBrush(bounds, ImageListView.Colors.SelectedColor1,
                    ImageListView.Colors.SelectedColor2, LinearGradientMode.Vertical);
                g.FillRectangle(bItemBack, bounds);
            } else if (!ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
            {
                // Not focused and selected
                using Brush bItemBack = new LinearGradientBrush(bounds, ImageListView.Colors.UnFocusedColor1,
                    ImageListView.Colors.UnFocusedColor2, LinearGradientMode.Vertical);
                g.FillRectangle(bItemBack, bounds);
            } else
            {
                // Not selected
                using (Brush bItemBack = new SolidBrush(alternate ?
                    ImageListView.Colors.AlternateBackColor : ImageListView.Colors.BackColor))
                {
                    g.FillRectangle(bItemBack, bounds);
                }

                // Shade sort column
                int x = bounds.Left - 1;
                foreach (ImageListView.ImageListViewColumnHeader column in uicolumns)
                {
                    if (ImageListView.SortOrder != SortOrder.None &&
                        ImageListView.SortColumn >= 0 && ImageListView.SortColumn < ImageListView.Columns.Count &&
                        ImageListView.Columns[ImageListView.SortColumn].Guid == column.Guid)
                    {
                        Rectangle subItemBounds = bounds;
                        subItemBounds.X = x;
                        subItemBounds.Width = column.Width;
                        using Brush bSort = new SolidBrush(ImageListView.Colors.ColumnSelectColor);
                        g.FillRectangle(bSort, subItemBounds);
                        break;
                    }
                    x += column.Width;
                }

            }

            // Separators 
            int xs = bounds.Left - 1;
            foreach (ImageListView.ImageListViewColumnHeader column in uicolumns)
            {
                xs += column.Width;
                if (!ReferenceEquals(column, uicolumns[uicolumns.Count - 1]))
                {
                    using Pen pSep = new(ImageListView.Colors.ColumnSeparatorColor);
                    g.DrawLine(pSep, xs, bounds.Top, xs, bounds.Bottom);
                }
            }

            // Sub items
            Color foreColor = ImageListView.Colors.CellForeColor;
            if ((state & ItemState.Disabled) != ItemState.None)
                foreColor = ImageListView.Colors.DisabledForeColor;
            else if (ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
                foreColor = ImageListView.Colors.SelectedForeColor;
            else if (!ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
                foreColor = ImageListView.Colors.UnFocusedForeColor;
            else if (alternate)
                foreColor = ImageListView.Colors.AlternateCellForeColor;

            int offset = 2;
            int firstWidth = 0;
            if (uicolumns.Count > 0)
                firstWidth = uicolumns[0].Width;
            Rectangle rt = new(bounds.Left + offset, bounds.Top, firstWidth - 2 * offset, bounds.Height);
            foreach (ImageListView.ImageListViewColumnHeader column in uicolumns)
            {
                rt.Width = column.Width - 2 * offset;
                int iconOffset = 0;
                if (column.Type == ColumnType.Name)
                {
                    // Allocate space for checkbox and file icon
                    if (ImageListView.ShowCheckBoxes && ImageListView.ShowFileIcons)
                        iconOffset += 2 * 16 + 3 * 2;
                    else if (ImageListView.ShowCheckBoxes)
                        iconOffset += 16 + 2 * 2;
                    else if (ImageListView.ShowFileIcons)
                        iconOffset += 16 + 2 * 2;
                }
                rt.X += iconOffset;
                rt.Width -= iconOffset;
                // Rating stars
                if (column.Type == ColumnType.Rating && ImageListView.RatingImage != null && ImageListView.EmptyRatingImage != null)
                {
                    int rating = item.GetSimpleRating();
                    if (rating > 0)
                    {
                        int w = ImageListView.RatingImage.Width;
                        int y = (int)(rt.Top + (rt.Height - ImageListView.RatingImage.Height) / 2.0f);

                        for (int i = 1; i <= 5; i++)
                        {
                            if (rating >= i)
                                g.DrawImage(ImageListView.RatingImage, rt.Left + (i - 1) * w, y);
                            else
                                g.DrawImage(ImageListView.EmptyRatingImage, rt.Left + (i - 1) * w, y);
                        }
                    }
                } else
                    TextRenderer.DrawText(g, item.SubItems[column].Text, ImageListView.Font, rt, foreColor,
                        TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.NoPrefix);

                rt.X -= iconOffset;
                rt.X += column.Width;
            }

            // Focus rectangle
            if (ImageListView.Focused && (state & ItemState.Focused) != ItemState.None)
                ControlPaint.DrawFocusRectangle(g, bounds);
        } else // if (ImageListView.View != View.Details)
        {
            // Paint background
            if (ImageListView.Enabled)
            {
                using Brush bItemBack = new SolidBrush(ImageListView.Colors.BackColor);
                g.FillRectangle(bItemBack, bounds);
            } else
            {
                using Brush bItemBack = new SolidBrush(ImageListView.Colors.DisabledBackColor);
                g.FillRectangle(bItemBack, bounds);
            }

            // Get thumbnail
            Image img = item.GetCachedImage(CachedImageType.Thumbnail);

            // Reference text height
            int textHeight = ImageListView.Font.Height;

            // Calculate bounds
            Rectangle textBounds = new(bounds.Left + 3, bounds.Bottom - (textHeight + 3), bounds.Width - 2 * 3, textHeight);
            Rectangle imgBounds = img != null
                ? new Rectangle(bounds.Left + (bounds.Width - img.Width) / 2,
                    bounds.Bottom - (img.Height + textHeight + 3 * 3), img.Width, img.Height)
                : new Rectangle(bounds.Left + 3, bounds.Top + 3, ImageListView.ThumbnailSize.Width, ImageListView.ThumbnailSize.Height);
            Rectangle textOutline = Rectangle.Inflate(textBounds, 3, 3);
            Rectangle imgOutline = Rectangle.Inflate(imgBounds, 3, 3);
            textOutline.Width -= 1;
            textOutline.Height -= 1;

            // Paint background
            if ((state & ItemState.Disabled) != ItemState.None)
            {
                // Disabled
                using Brush bBack = new SolidBrush(ImageListView.Colors.DisabledColor1);
                Utility.FillRoundedRectangle(g, bBack, textOutline, 4);
                Utility.FillRoundedRectangle(g, bBack, imgOutline, 4);
            } else if (ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
            {
                // Focused and selected
                using Brush bBack = new SolidBrush(ImageListView.Colors.SelectedColor1);
                Utility.FillRoundedRectangle(g, bBack, textOutline, 4);
                Utility.FillRoundedRectangle(g, bBack, imgOutline, 4);
            } else if (!ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
            {
                // Not focused and selected
                using Brush bBack = new SolidBrush(ImageListView.Colors.UnFocusedColor1);
                Utility.FillRoundedRectangle(g, bBack, textOutline, 4);
                Utility.FillRoundedRectangle(g, bBack, imgOutline, 4);
            }

            // Draw image
            if (img != null)
            {
                g.DrawImage(img, imgBounds.Location);
            }

            // Image border
            using (Pen pBorder = new(ImageListView.Colors.BorderColor))
            {
                Utility.DrawRoundedRectangle(g, pBorder, imgOutline.Left, imgOutline.Top, imgOutline.Width - 1, imgOutline.Height - 1, 3);
            }

            // Hovered state
            if ((state & ItemState.Hovered) != ItemState.None)
            {
                using Brush bGlow = new SolidBrush(Color.FromArgb(24, Color.White));
                Utility.FillRoundedRectangle(g, bGlow, imgOutline, 4);
            }

            // Item text
            Color foreColor = ImageListView.Colors.ForeColor;
            if ((state & ItemState.Disabled) != ItemState.None)
                foreColor = ImageListView.Colors.DisabledForeColor;
            else if (ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
                foreColor = ImageListView.Colors.SelectedForeColor;
            else if (!ImageListView.Focused && (state & ItemState.Selected) != ItemState.None)
                foreColor = ImageListView.Colors.UnFocusedForeColor;
            TextRenderer.DrawText(g, item.Text, ImageListView.Font, textBounds, foreColor,
                TextFormatFlags.EndEllipsis | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.NoPrefix);

            // Focus rectangle
            if (ImageListView.Focused && (state & ItemState.Focused) != ItemState.None)
            {
                textOutline.Offset(1, 1);
                textOutline.Width -= 1;
                textOutline.Height -= 1;
                ControlPaint.DrawFocusRectangle(g, textOutline);
            }
        }
    }
    /// <summary>
    /// Draws the large preview image of the focused item in Gallery mode.
    /// </summary>
    /// <param name="g">The System.Drawing.Graphics to draw on.</param>
    /// <param name="item">The ImageListViewItem to draw.</param>
    /// <param name="image">The image to draw.</param>
    /// <param name="bounds">The bounding rectangle of the preview area.</param>
    public override void DrawGalleryImage(Graphics g, ImageListViewItem item, Image image, Rectangle bounds)
    {
        if (item != null && image != null)
        {
            // Calculate image bounds
            Size itemMargin = MeasureItemMargin(ImageListView.View);
            Rectangle pos = Utility.GetSizedImageBounds(image, new Rectangle(bounds.Location + itemMargin, bounds.Size - itemMargin - itemMargin));
            // Draw image
            g.DrawImage(image, pos);
            // Draw image border
            if (pos.Width > 32 && pos.Height > 32)
            {
                using Pen pBorder = new(ImageListView.Colors.BorderColor);
                g.DrawRectangle(pBorder, pos);
            }
        }
    }
}
#endregion

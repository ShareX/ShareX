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
//

using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace ShareX.ImageListView;

public partial class ImageListView
{
    /// <summary>
    /// Represents an overridable class for image list view renderers.
    /// </summary>
    public class ImageListViewRenderer : IDisposable
    {
        #region Constants
        /// <summary>
        /// Represents the time in milliseconds after which the control deems to be needing a refresh.
        /// </summary>
        internal const int LazyRefreshInterval = 100;
        #endregion

        #region Member Variables
        private BufferedGraphics bufferGraphics;
        private bool disposed;
        private bool creatingGraphics;
        private DateTime lastRenderTime;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the ImageListView owning this item.
        /// </summary>
        public ImageListView ImageListView { get; internal set; }
        /// <summary>
        /// Gets or sets whether the graphics is clipped to the bounds of 
        /// drawing elements.
        /// </summary>
        public bool Clip { get; set; }
        /// <summary>
        /// Gets or sets the order by which items are drawn.
        /// </summary>
        public ItemDrawOrder ItemDrawOrder { get; set; }
        /// <summary>
        /// Gets or sets whether items are drawn before of after headers and the gallery images.
        /// </summary>
        public bool ItemsDrawnFirst { get; set; }
        /// <summary>
        /// Gets the rectangle bounding the client area of the control without the scroll bars.
        /// </summary>
        public Rectangle ClientBounds { get { return ImageListView.layoutManager.ClientArea; } }
        /// <summary>
        /// Gets the rectangle bounding the item display area.
        /// </summary>
        public Rectangle ItemAreaBounds { get { return ImageListView.layoutManager.ItemAreaBounds; } }
        /// <summary>
        /// Gets the rectangle bounding the column headers.
        /// </summary>
        public Rectangle ColumnHeaderBounds { get { return ImageListView.layoutManager.ColumnHeaderBounds; } }
        /// <summary>
        /// Gets a value indicating whether this renderer can apply custom colors.
        /// </summary>
        public virtual bool CanApplyColors { get { return true; } }
        /// <summary>
        /// Gets whether the lazy refresh interval is exceeded.
        /// </summary>
        internal bool LazyRefreshIntervalExceeded { get { return ((int)(DateTime.Now - lastRenderTime).TotalMilliseconds > LazyRefreshInterval); } }
        /// <summary>
        /// Gets a list of color themes preferred by this renderer.
        /// </summary>
        public virtual ImageListViewColor[] PreferredColors { get { return null; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ImageListViewRenderer class.
        /// </summary>
        public ImageListViewRenderer()
        {
            creatingGraphics = false;
            disposed = false;
            Clip = true;
            ItemsDrawnFirst = false;
            ItemDrawOrder = ItemDrawOrder.ItemIndex;
            lastRenderTime = DateTime.MinValue;
        }
        #endregion

        #region DrawItemParams
        /// <summary>
        /// Represents the paramaters required to draw an item.
        /// </summary>
        private struct DrawItemParams
        {
            public ImageListViewItem Item;
            public ItemState State;
            public Rectangle Bounds;

            public DrawItemParams(ImageListViewItem item, ItemState state, Rectangle bounds)
            {
                Item = item;
                State = state;
                Bounds = bounds;
            }
        }
        #endregion

        #region ItemDrawOrderComparer
        /// <summary>
        /// Compares items by the draw order.
        /// </summary>
        private class ItemDrawOrderComparer : IComparer<DrawItemParams>
        {
            private readonly ItemDrawOrder mDrawOrder;

            public ItemDrawOrderComparer(ItemDrawOrder drawOrder)
            {
                mDrawOrder = drawOrder;
            }

            /// <summary>
            /// Compares items by the draw order.
            /// </summary>
            /// <param name="param1">First item to compare.</param>
            /// <param name="param2">Second item to compare.</param>
            /// <returns>1 if the first item should be drawn first, 
            /// -1 if the second item should be drawn first,
            /// 0 if the two items can be drawn in any order.</returns>
            public int Compare(DrawItemParams param1, DrawItemParams param2)
            {
                if (ReferenceEquals(param1, param2))
                    return 0;
                if (ReferenceEquals(param1.Item, param2.Item))
                    return 0;

                int comparison = 0;

                if (mDrawOrder == ItemDrawOrder.ItemIndex)
                {
                    return CompareByIndex(param1, param2);
                } else if (mDrawOrder == ItemDrawOrder.ZOrder)
                {
                    return CompareByZOrder(param1, param2);
                } else if (mDrawOrder == ItemDrawOrder.NormalSelectedHovered)
                {
                    comparison = -CompareByHovered(param1, param2);
                    if (comparison != 0) return comparison;
                    comparison = -CompareBySelected(param1, param2);
                    if (comparison != 0) return comparison;
                    comparison = -CompareByNormal(param1, param2);
                    if (comparison != 0) return comparison;
                } else if (mDrawOrder == ItemDrawOrder.NormalHoveredSelected)
                {
                    comparison = -CompareBySelected(param1, param2);
                    if (comparison != 0) return comparison;
                    comparison = -CompareByHovered(param1, param2);
                    if (comparison != 0) return comparison;
                    comparison = -CompareByNormal(param1, param2);
                    if (comparison != 0) return comparison;
                } else if (mDrawOrder == ItemDrawOrder.SelectedNormalHovered)
                {
                    comparison = -CompareByHovered(param1, param2);
                    if (comparison != 0) return comparison;
                    comparison = -CompareByNormal(param1, param2);
                    if (comparison != 0) return comparison;
                    comparison = -CompareBySelected(param1, param2);
                    if (comparison != 0) return comparison;
                } else if (mDrawOrder == ItemDrawOrder.SelectedHoveredNormal)
                {
                    comparison = -CompareByNormal(param1, param2);
                    if (comparison != 0) return comparison;
                    comparison = -CompareByHovered(param1, param2);
                    if (comparison != 0) return comparison;
                    comparison = -CompareBySelected(param1, param2);
                    if (comparison != 0) return comparison;
                } else if (mDrawOrder == ItemDrawOrder.HoveredNormalSelected)
                {
                    comparison = -CompareBySelected(param1, param2);
                    if (comparison != 0) return comparison;
                    comparison = -CompareByNormal(param1, param2);
                    if (comparison != 0) return comparison;
                    comparison = -CompareByHovered(param1, param2);
                    if (comparison != 0) return comparison;
                } else if (mDrawOrder == ItemDrawOrder.HoveredSelectedNormal)
                {
                    comparison = -CompareByNormal(param1, param2);
                    if (comparison != 0) return comparison;
                    comparison = -CompareBySelected(param1, param2);
                    if (comparison != 0) return comparison;
                    comparison = -CompareByHovered(param1, param2);
                    if (comparison != 0) return comparison;
                }

                // Compare by zorder
                comparison = CompareByZOrder(param1, param2);
                if (comparison != 0) return comparison;

                // Finally compare by index
                comparison = CompareByIndex(param1, param2);
                return comparison;
            }

            /// <summary>
            /// Compares items by their index property.
            /// </summary>
            private int CompareByIndex(DrawItemParams param1, DrawItemParams param2)
            {
                return param1.Item.Index < param2.Item.Index ? -1 : param1.Item.Index > param2.Item.Index ? 1 : 0;
            }
            /// <summary>
            /// Compares items by their zorder property.
            /// </summary>
            private int CompareByZOrder(DrawItemParams param1, DrawItemParams param2)
            {
                return param1.Item.ZOrder < param2.Item.ZOrder ? -1 : param1.Item.ZOrder > param2.Item.ZOrder ? 1 : 0;
            }
            /// <summary>
            /// Compares items by their neutral state.
            /// </summary>
            private int CompareByNormal(DrawItemParams param1, DrawItemParams param2)
            {
                return param1.State == ItemState.None && param2.State != ItemState.None
                    ? -1
                    : param1.State != ItemState.None && param2.State == ItemState.None ? 1 : 0;
            }
            /// <summary>
            /// Compares items by their selected state.
            /// </summary>
            private int CompareBySelected(DrawItemParams param1, DrawItemParams param2)
            {
                return (param1.State & ItemState.Selected) == ItemState.Selected &&
                    (param2.State & ItemState.Selected) != ItemState.Selected
                    ? -1
                    : (param1.State & ItemState.Selected) != ItemState.Selected &&
                    (param2.State & ItemState.Selected) == ItemState.Selected
                    ? 1
                    : 0;
            }
            /// <summary>
            /// Compares items by their hovered state.
            /// </summary>
            private int CompareByHovered(DrawItemParams param1, DrawItemParams param2)
            {
                return (param1.State & ItemState.Hovered) == ItemState.Hovered ? -1 : (param2.State & ItemState.Hovered) == ItemState.Hovered ? 1 : 0;
            }
            /// <summary>
            /// Compares items by their focused state.
            /// </summary>
            private int CompareByFocused(DrawItemParams param1, DrawItemParams param2)
            {
                return (param1.State & ItemState.Focused) == ItemState.Focused ? -1 : (param2.State & ItemState.Focused) == ItemState.Focused ? 1 : 0;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Reads and returns the image for the given item.
        /// </summary>
        /// <param name="item">The item to read.</param>
        /// <param name="size">The size of the requested image..</param>
        /// <returns>Item thumbnail of requested size.</returns>
        public Image GetImageAsync(ImageListViewItem item, Size size)
        {
            Image img = ImageListView.thumbnailCache.GetRendererImage(item.Guid, size, ImageListView.UseEmbeddedThumbnails,
                ImageListView.AutoRotateThumbnails);

            if (img == null)
            {
                ImageListView.thumbnailCache.AddToRendererCache(item.Guid, item.mAdaptor, item.VirtualItemKey,
                    size, ImageListView.UseEmbeddedThumbnails, ImageListView.AutoRotateThumbnails);
            }

            return img;
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Renders the border of the control.
        /// </summary>
        /// <param name="g">The graphics to draw on.</param>
        private void RenderBorder(Graphics g)
        {
            // Background
            g.ResetClip();
            DrawBorder(g, new Rectangle(0, 0, ImageListView.Width, ImageListView.Height));
        }
        /// <summary>
        /// Renders the background of the control.
        /// </summary>
        /// <param name="g">The graphics to draw on.</param>
        private void RenderBackground(Graphics g)
        {
            // Background
            g.SetClip(ImageListView.layoutManager.ClientArea);
            DrawBackground(g, ImageListView.layoutManager.ClientArea);
        }
        /// <summary>
        /// Renders the group header.
        /// </summary>
        /// <param name="g">The graphics to draw on.</param>
        private void RenderGroupHeaders(Graphics g)
        {
            if (!ImageListView.showGroups)
                return;

            foreach (ImageListViewGroup group in ImageListView.groups.GetDisplayedGroups())
            {
                if (Clip)
                {
                    Rectangle clip = Rectangle.Intersect(group.headerBounds, ImageListView.layoutManager.ItemAreaBounds);
                    g.SetClip(clip);
                } else
                    g.SetClip(ImageListView.layoutManager.ClientArea);

                if (ImageListView.View == View.Gallery || ImageListView.View == View.HorizontalStrip)
                {
                    g.TranslateTransform(group.headerBounds.Left, group.headerBounds.Bottom);
                    g.RotateTransform(270);
                    DrawGroupHeader(g, group.Name, new Rectangle(0, 0, group.headerBounds.Height, group.headerBounds.Width));
                    g.ResetTransform();
                } else
                    DrawGroupHeader(g, group.Name, group.headerBounds);
            }
        }
        /// <summary>
        /// Renders the column header.
        /// </summary>
        /// <param name="g">The graphics to draw on.</param>
        private void RenderColumnHeaders(Graphics g)
        {
            if (ImageListView.View != View.Details)
                return;

            int x = ImageListView.layoutManager.ColumnHeaderBounds.Left;
            int y = ImageListView.layoutManager.ColumnHeaderBounds.Top;
            int h = MeasureColumnHeaderHeight();
            int lastX = 0;
            foreach (ImageListViewColumnHeader column in ImageListView.Columns.GetDisplayedColumns())
            {
                ColumnState state = ColumnState.None;
                if (ReferenceEquals(ImageListView.navigationManager.HoveredColumn, column))
                    state |= ColumnState.Hovered;
                if (ReferenceEquals(ImageListView.navigationManager.HoveredSeparator, column))
                    state |= ColumnState.SeparatorHovered;
                if (ReferenceEquals(ImageListView.navigationManager.SelectedSeparator, column))
                    state |= ColumnState.SeparatorSelected;
                if (ImageListView.SortColumn >= 0 && ImageListView.SortColumn < ImageListView.Columns.Count &&
                    ImageListView.Columns[ImageListView.SortColumn].Guid == column.Guid)
                    state |= ColumnState.SortColumn;

                Rectangle bounds = new(x, y, column.Width, h);
                if (Clip)
                {
                    Rectangle clip = Rectangle.Intersect(bounds, ImageListView.layoutManager.ClientArea);
                    g.SetClip(clip);
                } else
                    g.SetClip(ImageListView.layoutManager.ClientArea);

                DrawColumnHeader(g, column, state, bounds);

                x += column.Width;
                lastX = bounds.Right;
            }

            // Extender column
            if (ImageListView.Columns.Count != 0)
            {
                if (lastX < ImageListView.layoutManager.ClientArea.Right)
                {
                    Rectangle extender = new(
                        lastX,
                        ImageListView.layoutManager.ColumnHeaderBounds.Top,
                        ImageListView.layoutManager.ClientArea.Right - lastX,
                        ImageListView.layoutManager.ColumnHeaderBounds.Height);
                    if (Clip)
                        g.SetClip(extender);
                    else
                        g.SetClip(ImageListView.layoutManager.ClientArea);
                    DrawColumnExtender(g, extender);
                }
            } else
            {
                Rectangle extender = ImageListView.layoutManager.ColumnHeaderBounds;
                if (Clip)
                    g.SetClip(extender);
                else
                    g.SetClip(ImageListView.layoutManager.ClientArea);
                DrawColumnExtender(g, extender);
            }
        }
        /// <summary>
        /// Renders the large gallery image.
        /// </summary>
        /// <param name="g">The graphics to draw on.</param>
        private void RenderGalleryImage(Graphics g)
        {
            if (ImageListView.View != View.Gallery)
                return;

            Rectangle bounds = ImageListView.layoutManager.ClientArea;
            bounds.Height -= ImageListView.layoutManager.ItemAreaBounds.Height;

            ImageListViewItem item = null;
            if (ImageListView.Items.FocusedItem != null)
                item = ImageListView.Items.FocusedItem;
            else if (ImageListView.SelectedItems.Count != 0)
                item = ImageListView.SelectedItems[0];
            else if (ImageListView.Items.Count != 0)
                item = ImageListView.Items[0];

            Image image = null;
            if (item != null && bounds.Width > 4 && bounds.Height > 4)
            {
                image = GetGalleryImageAsync(item, bounds.Size);
                if (image == null) image = item.GetCachedImage(CachedImageType.Thumbnail);
            }

            if (Clip)
                g.SetClip(bounds);
            else
                g.SetClip(ImageListView.layoutManager.ClientArea);

            DrawGalleryImage(g, item, image, bounds);
        }
        /// <summary>
        /// Renders the pane.
        /// </summary>
        /// <param name="g">The graphics to draw on.</param>
        private void RenderPane(Graphics g)
        {
            if (ImageListView.View != View.Pane)
                return;

            Rectangle bounds = ImageListView.layoutManager.ClientArea;
            bounds.Width = ImageListView.mPaneWidth;

            ImageListViewItem item = null;
            if (ImageListView.Items.FocusedItem != null)
                item = ImageListView.Items.FocusedItem;
            else if (ImageListView.SelectedItems.Count != 0)
                item = ImageListView.SelectedItems[0];
            else if (ImageListView.Items.Count != 0)
                item = ImageListView.Items[0];

            Image image = null;
            if (item != null && bounds.Width > 4 && bounds.Height > 4)
            {
                image = GetGalleryImageAsync(item, new Size(bounds.Width, 65535));
                if (image == null) image = item.GetCachedImage(CachedImageType.Thumbnail);
            }

            if (Clip)
                g.SetClip(bounds);
            else
                g.SetClip(ImageListView.layoutManager.ClientArea);

            DrawPane(g, item, image, bounds);
        }
        /// <summary>
        /// Renders the items.
        /// </summary>
        /// <param name="g">The graphics to draw on.</param>
        private void RenderItems(Graphics g)
        {
            // Is the control empty?
            if (ImageListView.Items.Count == 0)
                return;
            // No items visible?
            if (ImageListView.layoutManager.FirstPartiallyVisible == -1 ||
                ImageListView.layoutManager.LastPartiallyVisible == -1)
                return;
            // No columns displayed?
            if (ImageListView.View == View.Details && ImageListView.Columns.GetDisplayedColumns().Count == 0)
                return;

            List<DrawItemParams> drawItemParams = new();
            for (int i = ImageListView.layoutManager.FirstPartiallyVisible; i <= ImageListView.layoutManager.LastPartiallyVisible; i++)
            {
                ImageListViewItem item = ImageListView.Items[i];

                // Determine item state
                ItemState state = ItemState.None;
                ItemHighlightState highlightState = ImageListView.navigationManager.HighlightState(item);
                if (highlightState == ItemHighlightState.HighlightedAndSelected ||
                    (highlightState == ItemHighlightState.NotHighlighted && item.Selected))
                    state |= ItemState.Selected;

                if (ReferenceEquals(ImageListView.navigationManager.HoveredItem, item) &&
                    ImageListView.navigationManager.MouseSelecting == false)
                    state |= ItemState.Hovered;

                if (item.Focused)
                    state |= ItemState.Focused;

                if (!item.Enabled)
                    state |= ItemState.Disabled;

                // Get item bounds
                Rectangle bounds = ImageListView.layoutManager.GetItemBounds(i);

                // Add to params to be sorted and drawn
                drawItemParams.Add(new DrawItemParams(item, state, bounds));
            }

            // Sort items by draw order
            drawItemParams.Sort(new ItemDrawOrderComparer(ItemDrawOrder));

            // Draw items
            foreach (DrawItemParams param in drawItemParams)
            {
                if (Clip)
                {
                    Rectangle clip = Rectangle.Intersect(param.Bounds, ImageListView.layoutManager.ItemAreaBounds);
                    g.SetClip(clip);
                } else
                    g.SetClip(ImageListView.layoutManager.ClientArea);

                // Draw the item
                DrawItem(g, param.Item, param.State, param.Bounds);

                // Draw sub item overlays
                if (ImageListView.View == View.Details)
                {
                    int xc1 = ImageListView.layoutManager.ColumnHeaderBounds.Left;
                    int colIndex = 0;
                    foreach (ImageListViewColumnHeader column in ImageListView.Columns.GetDisplayedColumns())
                    {
                        Rectangle subBounds = new(xc1, param.Bounds.Y, column.Width, param.Bounds.Height);
                        if (Clip)
                        {
                            Rectangle clip = Rectangle.Intersect(subBounds, ImageListView.layoutManager.ItemAreaBounds);
                            g.SetClip(clip);
                        }

                        // Check if the mouse is over the sub item
                        bool subItemHovered = ((param.State & ItemState.Hovered) != ItemState.None) &&
                            (ImageListView.navigationManager.HoveredSubItem == colIndex);

                        DrawSubItemItemOverlay(g, param.Item, param.State, colIndex, subItemHovered, subBounds);

                        colIndex++;
                        xc1 += column.Width;
                    }
                }

                // Draw the checkbox and file icon
                if (ImageListView.ShowCheckBoxes)
                {
                    Rectangle cBounds = ImageListView.layoutManager.GetCheckBoxBounds(param.Item.Index);
                    if (Clip)
                    {
                        Rectangle clip = Rectangle.Intersect(cBounds, ImageListView.layoutManager.ItemAreaBounds);
                        g.SetClip(clip);
                    } else
                        g.SetClip(ImageListView.layoutManager.ClientArea);

                    DrawCheckBox(g, param.Item, cBounds);
                }
                if (ImageListView.ShowFileIcons)
                {
                    Rectangle cBounds = ImageListView.layoutManager.GetIconBounds(param.Item.Index);
                    if (Clip)
                    {
                        Rectangle clip = Rectangle.Intersect(cBounds, ImageListView.layoutManager.ItemAreaBounds);
                        g.SetClip(clip);
                    } else
                        g.SetClip(ImageListView.layoutManager.ClientArea);

                    DrawFileIcon(g, param.Item, cBounds);
                }
            }
        }
        /// <summary>
        /// Renders the overlay.
        /// </summary>
        /// <param name="g">The graphics to draw on.</param>
        private void RenderOverlay(Graphics g)
        {
            g.SetClip(ImageListView.layoutManager.ClientArea);
            DrawOverlay(g, ImageListView.layoutManager.ClientArea);
        }
        /// <summary>
        /// Renders the drag-drop insertion caret.
        /// </summary>
        /// <param name="g">The graphics to draw on.</param>
        private void RenderInsertionCaret(Graphics g)
        {
            if (ImageListView.navigationManager.DropTarget == null)
                return;

            Rectangle bounds = ImageListView.layoutManager.GetItemBounds(ImageListView.navigationManager.DropTarget.Index);
            if (ImageListView.View == View.Details)
            {
                if (ImageListView.navigationManager.DropToRight)
                    bounds.Offset(0, ImageListView.layoutManager.ItemSizeWithMargin.Height);
                bounds.Offset(0, -1);
                bounds.Height = 2;
            } else if (ImageListView.View == View.VerticalStrip)
            {
                if (ImageListView.navigationManager.DropToRight)
                    bounds.Offset(0, ImageListView.layoutManager.ItemSizeWithMargin.Height);
                Size itemMargin = MeasureItemMargin(ImageListView.View);
                bounds.Offset(0, -(itemMargin.Height - 2) / 2 - 2);
                bounds.Height = 2;
            } else
            {
                if (ImageListView.navigationManager.DropToRight)
                    bounds.Offset(ImageListView.layoutManager.ItemSizeWithMargin.Width, 0);
                Size itemMargin = MeasureItemMargin(ImageListView.View);
                bounds.Offset(-(itemMargin.Width - 2) / 2 - 2, 0);
                bounds.Width = 2;
            }
            if (Clip)
                g.SetClip(bounds);
            else
                g.SetClip(ImageListView.layoutManager.ClientArea);
            DrawInsertionCaret(g, bounds);
        }
        /// <summary>
        /// Renders the selection rectangle.
        /// </summary>
        /// <param name="g">The graphics to draw on.</param>
        private void RenderSelectionRectangle(Graphics g)
        {
            if (!ImageListView.navigationManager.MouseSelecting)
                return;

            Rectangle sel = ImageListView.navigationManager.SelectionRectangle;
            if (sel.Height > 0 && sel.Width > 0)
            {
                g.SetClip(ImageListView.layoutManager.ClientArea);
                if (Clip)
                {
                    Rectangle selclip = new(sel.Left, sel.Top, sel.Width + 1, sel.Height + 1);
                    g.IntersectClip(selclip);
                }
                g.ExcludeClip(ImageListView.layoutManager.ColumnHeaderBounds);
                DrawSelectionRectangle(g, sel);
            }
        }
        /// <summary>
        /// Renders the area between scrollbars.
        /// </summary>
        /// <param name="g">The graphics to draw on.</param>
        private void RenderScrollbarFiller(Graphics g)
        {
            if (!ImageListView.hScrollBar.Visible || !ImageListView.vScrollBar.Visible)
                return;

            Rectangle bounds = ImageListView.layoutManager.ClientArea;
            Rectangle filler = new(bounds.Right, bounds.Bottom, ImageListView.vScrollBar.Width, ImageListView.hScrollBar.Height);
            g.SetClip(filler);
            g.FillRectangle(SystemBrushes.Control, filler);
        }
        /// <summary>
        /// Renders the control.
        /// </summary>
        /// <param name="graphics">The graphics to draw on.</param>
        internal void Render(Graphics graphics)
        {
            if (disposed) return;

            if (bufferGraphics == null)
            {
                if (!RecreateBuffer(graphics)) return;
            }

            // Save the timne of this render for lazy refreshes
            lastRenderTime = DateTime.Now;

            // Update the layout
            ImageListView.layoutManager.Update();

            // Set drawing area
            Graphics g = bufferGraphics.Graphics;
            g.ResetClip();

            // Draw control border
            RenderBorder(g);

            // Draw background
            RenderBackground(g);

            // Draw group headers if visible
            RenderGroupHeaders(g);

            // Draw items if they should be drawn first
            bool itemsDrawn = false;
            if (ItemsDrawnFirst)
            {
                RenderItems(g);
                itemsDrawn = true;
            }

            // Draw the large preview image in Gallery mode
            RenderGalleryImage(g);

            // Draw the left-pane
            RenderPane(g);

            // Draw column headers
            RenderColumnHeaders(g);

            // Draw items if they should be drawn last.
            if (!itemsDrawn)
                RenderItems(g);

            // Draw the overlay image
            RenderOverlay(g);

            // Draw the selection rectangle
            RenderSelectionRectangle(g);

            // Draw the insertion caret
            RenderInsertionCaret(g);

            // Scrollbar filler
            RenderScrollbarFiller(g);

            // Draw on to the control
            bufferGraphics.Render(graphics);
        }
        /// <summary>
        /// Loads and returns the large gallery image for the given item.
        /// </summary>
        private Image GetGalleryImageAsync(ImageListViewItem item, Size size)
        {
            Image img = ImageListView.thumbnailCache.GetGalleryImage(item.Guid, size, ImageListView.UseEmbeddedThumbnails,
                ImageListView.AutoRotateThumbnails);

            if (img == null)
            {
                ImageListView.thumbnailCache.AddToGalleryCache(item.Guid, item.mAdaptor, item.VirtualItemKey,
                    size, ImageListView.UseEmbeddedThumbnails, ImageListView.AutoRotateThumbnails);
            }

            return img;
        }
        /// <summary>
        /// Clears the graphics buffer objects.
        /// </summary>
        internal void ClearBuffer()
        {
            if (bufferGraphics != null)
                bufferGraphics.Dispose();
            bufferGraphics = null;
        }
        /// <summary>
        /// Destroys the current buffer and creates a new buffered graphics 
        /// sized to the client area of the owner control.
        /// </summary>
        /// <param name="graphics">The Graphics to match the pixel format to.</param>
        internal bool RecreateBuffer(Graphics graphics)
        {
            if (creatingGraphics) return false;

            creatingGraphics = true;

            BufferedGraphicsContext bufferContext = BufferedGraphicsManager.Current;

            if (disposed)
                throw (new ObjectDisposedException("bufferContext"));

            int width = System.Math.Max(ImageListView.Width, 1);
            int height = System.Math.Max(ImageListView.Height, 1);

            bufferContext.MaximumBuffer = new Size(width, height);

            ClearBuffer();

            bufferGraphics = bufferContext.Allocate(graphics, new Rectangle(0, 0, width, height));

            creatingGraphics = false;

            InitializeGraphics(bufferGraphics.Graphics);

            return true;
        }
        /// <summary>
        /// Releases buffered graphics objects.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (!disposed)
            {
                ClearBuffer();

                disposed = true;
                GC.SuppressFinalize(this);
            }
        }
#if DEBUG
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// ImageListViewRenderer is reclaimed by garbage collection.
        /// </summary>
        ~ImageListViewRenderer()
        {
            System.Diagnostics.Debug.Print("Finalizer of {0} called.", GetType());
            Dispose();
        }
#endif
        #endregion

        #region Virtual Methods
        /// <summary>
        /// Initializes the System.Drawing.Graphics used to draw
        /// control elements.
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics to draw on.</param>
        public virtual void InitializeGraphics(Graphics g)
        {
            g.PixelOffsetMode = PixelOffsetMode.None;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }
        /// <summary>
        /// Returns the height of group headers.
        /// </summary>
        public virtual int MeasureGroupHeaderHeight()
        {
            return ImageListView.disposed || ImageListView.GroupHeaderFont == null
                ? 24
                : System.Math.Max(ImageListView.GroupHeaderFont.Height + 8, 24);
        }
        /// <summary>
        /// Returns the height of column headers.
        /// </summary>
        /// <returns>The height of column headers.</returns>
        public virtual int MeasureColumnHeaderHeight()
        {
            return ImageListView.disposed || ImageListView.ColumnHeaderFont == null
                ? 24
                : System.Math.Max(ImageListView.ColumnHeaderFont.Height + 4, 24);
        }
        /// <summary>
        /// Returns the spacing between items for the given view mode.
        /// </summary>
        /// <param name="view">The view mode for which the measurement should be made.</param>
        /// <returns>The spacing between items.</returns>
        public virtual Size MeasureItemMargin(View view)
        {
            return view == View.Details ? new Size(2, 0) : new Size(4, 4);
        }
        /// <summary>
        /// Returns item size for the given view mode.
        /// </summary>
        /// <param name="view">The view mode for which the measurement should be made.</param>
        /// <returns>The item size.</returns>
        public virtual Size MeasureItem(View view)
        {
            Size itemSize = new();

            // Reference text height
            int textHeight = ImageListView.Font.Height;

            if (view == View.Details)
            {
                // Calculate total column width
                int colWidth = 0;
                foreach (ImageListViewColumnHeader column in ImageListView.Columns)
                    if (column.Visible) colWidth += column.Width;

                itemSize = new Size(colWidth, textHeight + 2 * textHeight / 6); // textHeight / 6 = vertical space between item border and text
            } else
            {
                Size itemPadding = new(4, 4);
                itemSize = ImageListView.ThumbnailSize + itemPadding + itemPadding;
                itemSize.Height += textHeight + System.Math.Max(4, textHeight / 3); // textHeight / 3 = vertical space between thumbnail and text
            }

            return itemSize;
        }
        /// <summary>
        /// Draws the border of the control.
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics to draw on.</param>
        /// <param name="bounds">The coordinates of the border.</param>
        public virtual void DrawBorder(Graphics g, Rectangle bounds)
        {
            if (ImageListView.BorderStyle != BorderStyle.None)
            {
                Border3DStyle style = (ImageListView.BorderStyle == BorderStyle.FixedSingle) ? Border3DStyle.Flat : Border3DStyle.SunkenInner;
                ControlPaint.DrawBorder3D(g, bounds, style);
            }
        }
        /// <summary>
        /// Draws the background of the control.
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics to draw on.</param>
        /// <param name="bounds">The client coordinates of the item area.</param>
        public virtual void DrawBackground(Graphics g, Rectangle bounds)
        {
            // Clear the background
            if (ImageListView.Enabled)
                g.Clear(ImageListView.Colors.ControlBackColor);
            else
                g.Clear(ImageListView.Colors.DisabledBackColor);

            // Draw the background image
            if (ImageListView.BackgroundImage != null)
            {
                Image img = ImageListView.BackgroundImage;

                if (ImageListView.BackgroundImageLayout == ImageLayout.None)
                {
                    g.DrawImageUnscaled(img, ImageListView.layoutManager.ItemAreaBounds.Location);
                } else if (ImageListView.BackgroundImageLayout == ImageLayout.Center)
                {
                    int x = bounds.Left + (bounds.Width - img.Width) / 2;
                    int y = bounds.Top + (bounds.Height - img.Height) / 2;
                    g.DrawImageUnscaled(img, x, y);
                } else if (ImageListView.BackgroundImageLayout == ImageLayout.Stretch)
                {
                    g.DrawImage(img, bounds);
                } else if (ImageListView.BackgroundImageLayout == ImageLayout.Tile)
                {
                    using Brush imgBrush = new TextureBrush(img, WrapMode.Tile);
                    g.FillRectangle(imgBrush, bounds);
                } else if (ImageListView.BackgroundImageLayout == ImageLayout.Zoom)
                {
                    float xscale = bounds.Width / (float)img.Width;
                    float yscale = bounds.Height / (float)img.Height;
                    float scale = Math.Min(xscale, yscale);
                    int width = (int)(img.Width * scale);
                    int height = (int)(img.Height * scale);
                    int x = bounds.Left + (bounds.Width - width) / 2;
                    int y = bounds.Top + (bounds.Height - height) / 2;
                    g.DrawImage(img, x, y, width, height);
                }
            }
        }
        /// <summary>
        /// Draws the selection rectangle.
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics to draw on.</param>
        /// <param name="selection">The client coordinates of the selection rectangle.</param>
        public virtual void DrawSelectionRectangle(Graphics g, Rectangle selection)
        {
            using SolidBrush brush = new(ImageListView.Colors.SelectionRectangleColor1);
            using Pen pen = new(ImageListView.Colors.SelectionRectangleBorderColor);
            g.FillRectangle(brush, selection);
            g.DrawRectangle(pen, selection);
        }
        /// <summary>
        /// Draws the specified item on the given graphics.
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics to draw on.</param>
        /// <param name="item">The ImageListViewItem to draw.</param>
        /// <param name="state">The current view state of item.</param>
        /// <param name="bounds">The bounding rectangle of item in client coordinates.</param>
        public virtual void DrawItem(Graphics g, ImageListViewItem item, ItemState state, Rectangle bounds)
        {
            Size itemPadding = new(4, 4);
            bool alternate = (item.Index % 2 == 1);

            // Paint background
            if (ImageListView.Enabled)
            {
                using Brush bItemBack = new SolidBrush(alternate && ImageListView.View == View.Details ?
                    ImageListView.Colors.AlternateBackColor : ImageListView.Colors.BackColor);
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
                Utility.FillRoundedRectangle(g, bDisabled, bounds, (ImageListView.View == View.Details ? 2 : 4));
            }

            // Paint background Selected
            else if ((ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None)) ||
                (!ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None) && ((state & ItemState.Hovered) != ItemState.None)))
            {
                using Brush bSelected = new LinearGradientBrush(bounds, ImageListView.Colors.SelectedColor1, ImageListView.Colors.SelectedColor2, LinearGradientMode.Vertical);
                Utility.FillRoundedRectangle(g, bSelected, bounds, (ImageListView.View == View.Details ? 2 : 4));
            }

            // Paint background unfocused
            else if (!ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None))
            {
                using Brush bGray64 = new LinearGradientBrush(bounds, ImageListView.Colors.UnFocusedColor1, ImageListView.Colors.UnFocusedColor2, LinearGradientMode.Vertical);
                Utility.FillRoundedRectangle(g, bGray64, bounds, (ImageListView.View == View.Details ? 2 : 4));
            }

            // Paint background Hovered
            if ((state & ItemState.Hovered) != ItemState.None)
            {
                using Brush bHovered = new LinearGradientBrush(bounds, ImageListView.Colors.HoverColor1, ImageListView.Colors.HoverColor2, LinearGradientMode.Vertical);
                Utility.FillRoundedRectangle(g, bHovered, bounds, (ImageListView.View == View.Details ? 2 : 4));
            }

            if (ImageListView.View != View.Details)
            {
                // Draw the image
                Image img = item.GetCachedImage(CachedImageType.Thumbnail);
                if (img != null)
                {
                    Rectangle pos = Utility.GetSizedImageBounds(img, new Rectangle(bounds.Location + itemPadding, ImageListView.ThumbnailSize));
                    g.DrawImage(img, pos);
                    // Draw image border
                    if (Math.Min(pos.Width, pos.Height) > 32)
                    {
                        using (Pen pOuterBorder = new(ImageListView.Colors.ImageOuterBorderColor))
                        {
                            g.DrawRectangle(pOuterBorder, pos);
                        }
                        if (System.Math.Min(ImageListView.ThumbnailSize.Width, ImageListView.ThumbnailSize.Height) > 32)
                        {
                            using Pen pInnerBorder = new(ImageListView.Colors.ImageInnerBorderColor);
                            g.DrawRectangle(pInnerBorder, Rectangle.Inflate(pos, -1, -1));
                        }
                    }
                }

                // Draw item text
                Color foreColor = ImageListView.Colors.ForeColor;
                if ((state & ItemState.Disabled) != ItemState.None)
                {
                    foreColor = ImageListView.Colors.DisabledForeColor;
                } else if ((state & ItemState.Selected) != ItemState.None)
                {
                    foreColor = ImageListView.Focused ? ImageListView.Colors.SelectedForeColor : ImageListView.Colors.UnFocusedForeColor;
                }
                Size szt = TextRenderer.MeasureText(item.Text, ImageListView.Font);
                Rectangle rt = new(bounds.Left + itemPadding.Width, bounds.Top + 2 * itemPadding.Height + ImageListView.ThumbnailSize.Height, ImageListView.ThumbnailSize.Width, szt.Height);
                TextRenderer.DrawText(g, item.Text, ImageListView.Font, rt, foreColor,
                    TextFormatFlags.EndEllipsis | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
            } else // if (ImageListView.View == View.Details)
            {
                List<ImageListViewColumnHeader> uicolumns = ImageListView.Columns.GetDisplayedColumns();

                // Shade sort column
                int x = bounds.Left - 1;
                foreach (ImageListViewColumnHeader column in uicolumns)
                {
                    if (ImageListView.SortOrder != SortOrder.None &&
                        ImageListView.SortColumn >= 0 && ImageListView.SortColumn < ImageListView.Columns.Count &&
                        (state & ItemState.Hovered) == ItemState.None && (state & ItemState.Selected) == ItemState.None &&
                        ImageListView.Columns[ImageListView.SortColumn].Guid == column.Guid)
                    {
                        Rectangle subItemBounds = bounds;
                        subItemBounds.X = x;
                        subItemBounds.Width = column.Width;
                        using Brush bGray16 = new SolidBrush(ImageListView.Colors.ColumnSelectColor);
                        g.FillRectangle(bGray16, subItemBounds);
                        break;
                    }
                    x += column.Width;
                }

                // Separators 
                if (!ImageListView.GroupsVisible)
                {
                    x = bounds.Left - 1;
                    foreach (ImageListViewColumnHeader column in uicolumns)
                    {
                        x += column.Width;
                        if (!ReferenceEquals(column, uicolumns[uicolumns.Count - 1]))
                        {
                            using Pen pGray32 = new(ImageListView.Colors.ColumnSeparatorColor);
                            g.DrawLine(pGray32, x, bounds.Top, x, bounds.Bottom);
                        }
                    }
                }

                Size offset = new(2, (bounds.Height - ImageListView.Font.Height) / 2);
                using StringFormat sf = new();
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                // Sub text
                int firstWidth = 0;
                if (uicolumns.Count > 0)
                    firstWidth = uicolumns[0].Width;
                RectangleF rt = new(bounds.Left + offset.Width, bounds.Top + offset.Height, firstWidth - 2 * offset.Width, bounds.Height - 2 * offset.Height);
                foreach (ImageListViewColumnHeader column in uicolumns)
                {
                    rt.Width = column.Width - 2 * offset.Width;
                    Color foreColor = ImageListView.Colors.CellForeColor;
                    if ((state & ItemState.Disabled) != ItemState.None)
                    {
                        foreColor = ImageListView.Colors.DisabledForeColor;
                    } else if ((state & ItemState.Selected) != ItemState.None)
                    {
                        foreColor = ImageListView.Focused ? ImageListView.Colors.SelectedForeColor : ImageListView.Colors.UnFocusedForeColor;
                    } else if (alternate)
                        foreColor = ImageListView.Colors.AlternateCellForeColor;
                    using (Brush bItemFore = new SolidBrush(foreColor))
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
                            g.DrawString(item.SubItems[column].Text, ImageListView.Font, bItemFore, rt, sf);

                        rt.X -= iconOffset;
                    }
                    rt.X += column.Width;
                }
            }

            // Item border
            if (ImageListView.View != View.Details)
            {
                using Pen pWhite128 = new(Color.FromArgb(128, ImageListView.Colors.ControlBackColor));
                Utility.DrawRoundedRectangle(g, pWhite128, bounds.Left + 1, bounds.Top + 1, bounds.Width - 3, bounds.Height - 3, (ImageListView.View == View.Details ? 2 : 4));
            }
            if (((state & ItemState.Disabled) != ItemState.None))
            {
                using Pen pHighlight128 = new(ImageListView.Colors.DisabledBorderColor);
                Utility.DrawRoundedRectangle(g, pHighlight128, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, (ImageListView.View == View.Details ? 2 : 4));
            } else if (ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None))
            {
                using Pen pHighlight128 = new(ImageListView.Colors.SelectedBorderColor);
                Utility.DrawRoundedRectangle(g, pHighlight128, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, (ImageListView.View == View.Details ? 2 : 4));
            } else if (!ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None))
            {
                using Pen pGray128 = new(ImageListView.Colors.UnFocusedBorderColor);
                Utility.DrawRoundedRectangle(g, pGray128, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, (ImageListView.View == View.Details ? 2 : 4));
            } else if (ImageListView.View != View.Details && (state & ItemState.Selected) == ItemState.None)
            {
                using Pen pGray64 = new(ImageListView.Colors.BorderColor);
                Utility.DrawRoundedRectangle(g, pGray64, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, (ImageListView.View == View.Details ? 2 : 4));
            }

            if (ImageListView.Focused && ((state & ItemState.Hovered) != ItemState.None))
            {
                using Pen pHighlight64 = new(ImageListView.Colors.HoverBorderColor);
                Utility.DrawRoundedRectangle(g, pHighlight64, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, (ImageListView.View == View.Details ? 2 : 4));
            }

            // Focus rectangle
            if (ImageListView.Focused && ((state & ItemState.Focused) != ItemState.None))
            {
                ControlPaint.DrawFocusRectangle(g, bounds);
            }
        }
        /// <summary>
        /// Draws the overlay graphics for the specified sub item on the given graphics.
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics to draw on.</param>
        /// <param name="item">The ImageListViewItem to draw.</param>
        /// <param name="state">The current view state of item.</param>
        /// <param name="subItemIndex">The index of the sub item. The index returned is the 0-based index of the 
        /// column as displayed on the screen, considering column visibility and display indices.
        /// Returns -1 if the hit point is not over a sub item.</param>
        /// <param name="subItemHovered">true if the mouse cursor is over the sub item; otherwise false.</param>
        /// <param name="bounds">The bounding rectangle of the sub item in client coordinates.</param>
        public virtual void DrawSubItemItemOverlay(Graphics g, ImageListViewItem item, ItemState state, int subItemIndex, bool subItemHovered, Rectangle bounds)
        {
            ;
        }
        /// <summary>
        /// Draws the checkbox icon for the specified item on the given graphics.
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics to draw on.</param>
        /// <param name="item">The ImageListViewItem to draw.</param>
        /// <param name="bounds">The bounding rectangle of the checkbox in client coordinates.</param>
        public virtual void DrawCheckBox(Graphics g, ImageListViewItem item, Rectangle bounds)
        {
            Size size = CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.CheckedNormal);
            PointF pt = new(bounds.X + (bounds.Width - (float)size.Width) / 2.0f,
                bounds.Y + (bounds.Height - (float)size.Height) / 2.0f);
            CheckBoxState state = CheckBoxState.UncheckedNormal;
            state = item.Enabled
                ? item.Checked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal
                : item.Checked ? CheckBoxState.CheckedDisabled : CheckBoxState.UncheckedDisabled;
            CheckBoxRenderer.DrawCheckBox(g, Point.Round(pt), state);
        }
        /// <summary>
        /// Draws the file icon for the specified item on the given graphics.
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics to draw on.</param>
        /// <param name="item">The ImageListViewItem to draw.</param>
        /// <param name="bounds">The bounding rectangle of the file icon in client coordinates.</param>
        public virtual void DrawFileIcon(Graphics g, ImageListViewItem item, Rectangle bounds)
        {
            Image icon = item.GetCachedImage(CachedImageType.SmallIcon);
            if (icon != null)
            {
                Size size = icon.Size;
                PointF ptf = new(bounds.X + (bounds.Width - (float)size.Width) / 2.0f,
                    bounds.Y + (bounds.Height - (float)size.Height) / 2.0f);
                Point pt = Point.Round(ptf);
                g.DrawImage(icon, pt.X, pt.Y);
            }
        }
        /// <summary>
        /// Draws the group headers.
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics to draw on.</param>
        /// <param name="name">The name of the group to draw.</param>
        /// <param name="bounds">The bounding rectangle of group in client coordinates.</param>
        public virtual void DrawGroupHeader(Graphics g, string name, Rectangle bounds)
        {
            // Bottom border
            bounds.Inflate(0, -4);
            using (Pen pSpep = new(new LinearGradientBrush(bounds, ImageListView.Colors.ColumnSeparatorColor, Color.Transparent, LinearGradientMode.Horizontal)))
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
                using SolidBrush bText = new(ImageListView.Colors.ColumnHeaderForeColor);
                g.DrawString(name, (ImageListView.GroupHeaderFont == null ? ImageListView.Font : ImageListView.GroupHeaderFont), bText, bounds, sf);
            }
        }
        /// <summary>
        /// Draws the column headers.
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics to draw on.</param>
        /// <param name="column">The ImageListViewColumnHeader to draw.</param>
        /// <param name="state">The current view state of column.</param>
        /// <param name="bounds">The bounding rectangle of column in client coordinates.</param>
        public virtual void DrawColumnHeader(Graphics g, ImageListViewColumnHeader column, ColumnState state, Rectangle bounds)
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
            using (Brush bBorder = new LinearGradientBrush(bounds, ImageListView.Colors.ColumnHeaderBackColor2, ImageListView.Colors.ColumnHeaderBackColor1, LinearGradientMode.Vertical))
            using (Pen pBorder = new(bBorder))
            {
                g.DrawLine(pBorder, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
                g.DrawLine(pBorder, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }
            using (Pen pSpep = new(ImageListView.Colors.ColumnHeaderBackColor1))
            {
                g.DrawLine(pSpep, bounds.Left + 1, bounds.Top + 1, bounds.Left + 1, bounds.Bottom - 2);
                g.DrawLine(pSpep, bounds.Right - 1, bounds.Top + 1, bounds.Right - 1, bounds.Bottom - 2);
            }

            // Draw the sort arrow
            int textOffset = 4;
            if (ImageListView.SortOrder != SortOrder.None && ((state & ColumnState.SortColumn) != ColumnState.None))
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
                using SolidBrush bText = new(ImageListView.Colors.ColumnHeaderForeColor);
                g.DrawString(column.Text, (ImageListView.ColumnHeaderFont == null ? ImageListView.Font : ImageListView.ColumnHeaderFont), bText, bounds, sf);
            }
        }
        /// <summary>
        /// Draws the left pane in Pane view mode.
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics to draw on.</param>
        /// <param name="item">The ImageListViewItem to draw.</param>
        /// <param name="image">The image to draw.</param>
        /// <param name="bounds">The bounding rectangle of the pane.</param>
        public virtual void DrawPane(Graphics g, ImageListViewItem item, Image image, Rectangle bounds)
        {
            // Draw pane background
            using (Brush bGray16 = new SolidBrush(ImageListView.Colors.PaneBackColor))
            {
                g.FillRectangle(bGray16, bounds);
            }
            using (Brush bBorder = new SolidBrush(ImageListView.Colors.PaneSeparatorColor))
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
                    using (Pen pGray128 = new(ImageListView.Colors.ImageOuterBorderColor))
                    {
                        g.DrawRectangle(pGray128, pos);
                    }
                    using Pen pWhite128 = new(ImageListView.Colors.ImageInnerBorderColor);
                    g.DrawRectangle(pWhite128, Rectangle.Inflate(pos, -1, -1));
                }
                bounds.X += itemMargin.Width;
                bounds.Width -= 2 * itemMargin.Width;
                bounds.Y = pos.Height + 16;
                bounds.Height -= pos.Height + 16;

                // Item text
                if (ImageListView.Columns.HasType(ColumnType.Name) && ImageListView.Columns[ColumnType.Name].Visible && bounds.Height > 0)
                {
                    string itemText = item.GetSubItemText(ColumnType.Name);
                    using SolidBrush bLabel = new(ImageListView.Colors.PaneLabelColor);
                    using SolidBrush bText = new(ImageListView.Colors.ForeColor);
                    int y = Utility.DrawStringPair(g, bounds, "", itemText, ImageListView.Font, bLabel, bText);
                    bounds.Y += 2 * y;
                    bounds.Height -= 2 * y;
                }

                // File type
                string fileType = item.GetSubItemText(ColumnType.FileType);
                if (ImageListView.Columns.HasType(ColumnType.FileType) && ImageListView.Columns[ColumnType.FileType].Visible && bounds.Height > 0 && !string.IsNullOrEmpty(fileType))
                {
                    using SolidBrush bLabel = new(ImageListView.Colors.PaneLabelColor);
                    using SolidBrush bText = new(ImageListView.Colors.ForeColor);
                    int y = Utility.DrawStringPair(g, bounds, ImageListView.Columns[ColumnType.FileType].Text + ": ",
                            fileType, ImageListView.Font, bLabel, bText);
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
                            using SolidBrush bLabel = new(ImageListView.Colors.PaneLabelColor);
                            using SolidBrush bText = new(ImageListView.Colors.ForeColor);
                            int y = Utility.DrawStringPair(g, bounds, caption + ": ", text,
                                    ImageListView.Font, bLabel, bText);
                            bounds.Y += y;
                            bounds.Height -= y;
                        }
                    }
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
        public virtual void DrawGalleryImage(Graphics g, ImageListViewItem item, Image image, Rectangle bounds)
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
                    using Pen pOuterBorder = new(ImageListView.Colors.ImageOuterBorderColor);
                    using Pen pInnerBorder = new(ImageListView.Colors.ImageInnerBorderColor);
                    g.DrawRectangle(pOuterBorder, pos);
                    g.DrawRectangle(pInnerBorder, Rectangle.Inflate(pos, -1, -1));
                }
            }
        }
        /// <summary>
        /// Draws the extender after the last column.
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics to draw on.</param>
        /// <param name="bounds">The bounding rectangle of extender column in client coordinates.</param>
        public virtual void DrawColumnExtender(Graphics g, Rectangle bounds)
        {
            // Paint background
            using (Brush bBack = new LinearGradientBrush(bounds, ImageListView.Colors.ColumnHeaderBackColor1, ImageListView.Colors.ColumnHeaderBackColor2, LinearGradientMode.Vertical))
            {
                g.FillRectangle(bBack, bounds);
            }
            using (Brush bBorder = new LinearGradientBrush(bounds, ImageListView.Colors.ColumnHeaderBackColor2, ImageListView.Colors.ColumnHeaderBackColor1, LinearGradientMode.Vertical))
            using (Pen pBorder = new(bBorder))
            {
                g.DrawLine(pBorder, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
                g.DrawLine(pBorder, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }
            using Pen pSpep = new(ImageListView.Colors.ColumnHeaderBackColor1);
            g.DrawLine(pSpep, bounds.Left + 1, bounds.Top + 1, bounds.Left + 1, bounds.Bottom - 2);
        }
        /// <summary>
        /// Draws the insertion caret for drag and drop operations.
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics to draw on.</param>
        /// <param name="bounds">The bounding rectangle of the insertion caret.</param>
        public virtual void DrawInsertionCaret(Graphics g, Rectangle bounds)
        {
            using Brush b = new SolidBrush(ImageListView.Colors.InsertionCaretColor);
            g.FillRectangle(b, bounds);
        }
        /// <summary>
        /// Draws an overlay image over the client area.
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics to draw on.</param>
        /// <param name="bounds">The bounding rectangle of the client area.</param>
        public virtual void DrawOverlay(Graphics g, Rectangle bounds)
        {
            ;
        }
        /// <summary>
        /// Releases managed resources.
        /// </summary>
        public virtual void Dispose()
        {
            ((IDisposable)this).Dispose();
        }
        /// <summary>
        /// Sets the layout of the control.
        /// </summary>
        /// <param name="e">A LayoutEventArgs that contains event data.</param>
        public virtual void OnLayout(LayoutEventArgs e)
        {
            ;
        }
        #endregion
    }
}

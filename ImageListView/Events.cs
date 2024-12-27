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

using System.ComponentModel;
using System.Runtime.InteropServices;

using static ShareX.ImageListView.ImageListView;

namespace ShareX.ImageListView;

#region Event Delegates
/// <summary>
/// Represents the method that will handle the CacheError event.
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A CacheErrorEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void CacheErrorEventHandler(object sender, CacheErrorEventArgs e);
/// <summary>
/// Represents the method that will handle the DropFiles event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A DropFileEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void DropFilesEventHandler(object sender, DropFileEventArgs e);
/// <summary>
/// Represents the method that will handle the DropItems event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A DropItemEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void DropItemsEventHandler(object sender, DropItemEventArgs e);
/// <summary>
/// Represents the method that will handle the DropComplete event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A DropCompleteEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void DropCompleteEventHandler(object sender, DropCompleteEventArgs e);
/// <summary>
/// Represents the method that will handle the ColumnClick event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A ColumnClickEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void ColumnClickEventHandler(object sender, ColumnClickEventArgs e);
/// <summary>
/// Represents the method that will handle the ColumnHover event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A ColumnHoverEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void ColumnHoverEventHandler(object sender, ColumnHoverEventArgs e);
/// <summary>
/// Represents the method that will handle the ColumnWidthChanged event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A ColumnEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void ColumnWidthChangedEventHandler(object sender, ColumnEventArgs e);
/// <summary>
/// Represents the method that will handle the ItemClick event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A ItemClickEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void ItemClickEventHandler(object sender, ItemClickEventArgs e);
/// <summary>
/// Represents the method that will handle the ItemCheckBoxClick event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A ItemEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void ItemCheckBoxClickEventHandler(object sender, ItemEventArgs e);
/// <summary>
/// Represents the method that will handle the ItemHover event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A ItemHoverEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void ItemHoverEventHandler(object sender, ItemHoverEventArgs e);
/// <summary>
/// Represents the method that will handle the ItemDoubleClick event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">An ItemClickEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void ItemDoubleClickEventHandler(object sender, ItemClickEventArgs e);
/// <summary>
/// Represents the method that will handle the ThumbnailCaching event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A ThumbnailCachingEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void ThumbnailCachingEventHandler(object sender, ThumbnailCachingEventArgs e);
/// <summary>
/// Represents the method that will handle the ThumbnailCached event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A ThumbnailCachedEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void ThumbnailCachedEventHandler(object sender, ThumbnailCachedEventArgs e);
/// <summary>
/// Represents the method that will handle the DetailsCaching event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">An ItemEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void DetailsCachingEventHandler(object sender, ItemEventArgs e);
/// <summary>
/// Represents the method that will handle the DetailsCached event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">An ItemEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void DetailsCachedEventHandler(object sender, ItemEventArgs e);
/// <summary>
/// Represents the method that will handle the ShellInfoCachingEventHandler event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A ShellInfoCachingEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void ShellInfoCachingEventHandler(object sender, ShellInfoCachingEventArgs e);
/// <summary>
/// Represents the method that will handle the ShellInfoCachedEventHandler event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A ShellInfoCachedEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void ShellInfoCachedEventHandler(object sender, ShellInfoCachedEventArgs e);
/// <summary>
/// Refreshes the owner control.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
internal delegate void RefreshDelegateInternal();
/// <summary>
/// Represents the method that will handle the ItemCollectionChanged event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A ItemCollectionChangedEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void ItemCollectionChangedEventHandler(object sender, ItemCollectionChangedEventArgs e);
/// <summary>
/// Represents the method that will handle the PaneResized event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A PaneResizedEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void PaneResizedEventHandler(object sender, PaneResizedEventArgs e);
/// <summary>
/// Represents the method that will handle the PaneResizing event. 
/// </summary>
/// <param name="sender">The ImageListView object that is the source of the event.</param>
/// <param name="e">A PaneResizingEventArgs that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void PaneResizingEventHandler(object sender, PaneResizingEventArgs e);
#endregion

#region Event Arguments
/// <summary>
/// Represents the event arguments for errors during cache operations.
/// </summary>
[Serializable, ComVisible(true)]
public class CacheErrorEventArgs : EventArgs
{
    /// <summary>
    /// Gets the ImageListViewItem that is associated with this error.
    /// This parameter can be null.
    /// </summary>
    public ImageListViewItem Item { get; private set; }
    /// <summary>
    /// Gets a value indicating which error occurred during an asynchronous operation.
    /// </summary>
    public Exception Error { get; private set; }
    /// <summary>
    /// Gets the thread raising the error.
    /// </summary>
    public CacheThread CacheThread { get; private set; }

    /// <summary>
    /// Initializes a new instance of the CacheErrorEventArgs class.
    /// </summary>
    /// <param name="item">The ImageListViewItem that is associated with this error.</param>
    /// <param name="error">The error that occurred during an asynchronous operation.</param>
    /// <param name="cacheThread">The thread raising the error.</param>
    public CacheErrorEventArgs(ImageListViewItem item, Exception error, CacheThread cacheThread)
    {
        Item = item;
        Error = error;
        CacheThread = cacheThread;
    }
}
/// <summary>
/// Represents the event arguments for external drag drop events.
/// </summary>
[Serializable, ComVisible(true)]
public class DropFileEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets whether default event code will be processed.
    /// When set to true, the control will automatically insert the new items.
    /// Otherwise, the control will not process the dropped files.
    /// </summary>
    public bool Cancel { get; set; }
    /// <summary>
    /// Gets the position of the insertion caret.
    /// This determines where the new items will be inserted.
    /// </summary>
    public int Index { get; private set; }
    /// <summary>
    /// Gets the array of filenames droppped on the control.
    /// </summary>
    public string[] FileNames { get; private set; }

    /// <summary>
    /// Initializes a new instance of the DropFileEventArgs class.
    /// </summary>
    /// <param name="index">The position of the insertion caret.</param>
    /// <param name="fileNames">The array of filenames droppped on the control.</param>
    public DropFileEventArgs(int index, string[] fileNames)
    {
        Cancel = false;
        Index = index;
        FileNames = fileNames;
    }
}
/// <summary>
/// Represents the event arguments for internal drag drop events.
/// </summary>
[Serializable, ComVisible(true)]
public class DropItemEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets whether default event code will be processed.
    /// When set to true, the control will automatically insert the new items.
    /// Otherwise, the control will not process the dropped items.
    /// </summary>
    public bool Cancel { get; set; }
    /// <summary>
    /// Gets the position of the insertion caret.
    /// This determines where the new items will be inserted.
    /// </summary>
    public int Index { get; private set; }
    /// <summary>
    /// Gets the array of items droppped on the control.
    /// </summary>
    public ImageListViewItem[] Items { get; private set; }

    /// <summary>
    /// Initializes a new instance of the DropItemEventArgs class.
    /// </summary>
    /// <param name="index">The position of the insertion caret.</param>
    /// <param name="items">The array of items droppped on the control.</param>
    public DropItemEventArgs(int index, ImageListViewItem[] items)
    {
        Cancel = false;
        Index = index;
        Items = items;
    }
}
/// <summary>
/// Represents the event arguments for drag drop event completion.
/// </summary>
[Serializable, ComVisible(true)]
public class DropCompleteEventArgs : EventArgs
{
    /// <summary>
    /// Gets the array of items droppped on the control.
    /// </summary>
    public ImageListViewItem[] Items { get; private set; }
    /// <summary>
    /// Gets if the drag operation is internal or external to the control.
    /// In an internal drag operation, own items of the control are reordered.
    /// </summary>
    public bool InternalDrag { get; private set; }

    /// <summary>
    /// Initializes a new instance of the DropCompleteEventArgs class.
    /// </summary>
    /// <param name="items">The array of items droppped on the control.</param>
    /// <param name="internalDrag">true if a drop event occurred after an internal reordering of items,
    /// otherwise false if image files were externally dropped onto the control.</param>
    public DropCompleteEventArgs(ImageListViewItem[] items, bool internalDrag)
    {
        Items = items;
        InternalDrag = internalDrag;
    }
}
/// <summary>
/// Represents the event arguments for column related events.
/// </summary>
[Serializable, ComVisible(true)]
public class ColumnEventArgs : EventArgs
{
    /// <summary>
    /// Gets the ImageListViewColumnHeader that is the target of the event.
    /// </summary>
    public ImageListViewColumnHeader Column { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ColumnEventArgs class.
    /// </summary>
    /// <param name="column">The column that is the target of this event.</param>
    public ColumnEventArgs(ImageListViewColumnHeader column)
    {
        Column = column;
    }
}
/// <summary>
/// Represents the event arguments for column click related events.
/// </summary>
[Serializable, ComVisible(true)]
public class ColumnClickEventArgs : EventArgs
{
    /// <summary>
    /// Gets the ImageListViewColumnHeader that is the target of the event.
    /// </summary>
    public ImageListView.ImageListViewColumnHeader Column { get; private set; }
    /// <summary>
    /// Gets the coordinates of the cursor.
    /// </summary>
    public Point Location { get; private set; }
    /// <summary>
    /// Gets the x-coordinates of the cursor.
    /// </summary>
    public int X { get { return Location.X; } }
    /// <summary>
    /// Gets the y-coordinates of the cursor.
    /// </summary>
    public int Y { get { return Location.Y; } }
    /// <summary>
    /// Gets the state of the mouse buttons.
    /// </summary>
    public MouseButtons Buttons { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ColumnClickEventArgs class.
    /// </summary>
    /// <param name="column">The column that is the target of this event.</param>
    /// <param name="location">The location of the mouse.</param>
    /// <param name="buttons">One of the System.Windows.Forms.MouseButtons values 
    /// indicating which mouse button was pressed.</param>
    public ColumnClickEventArgs(ImageListView.ImageListViewColumnHeader column, Point location, MouseButtons buttons)
    {
        Column = column;
        Location = location;
        Buttons = buttons;
    }
}
/// <summary>
/// Represents the event arguments for column hover related events.
/// </summary>
[Serializable, ComVisible(true)]
public class ColumnHoverEventArgs : EventArgs
{
    /// <summary>
    /// Gets the ImageListViewColumnHeader that was previously hovered.
    /// Returns null if there was no previously hovered column.
    /// </summary>
    public ImageListViewColumnHeader PreviousColumn { get; private set; }
    /// <summary>
    /// Gets the currently hovered ImageListViewColumnHeader.
    /// Returns null if there is no hovered column.
    /// </summary>
    public ImageListView.ImageListViewColumnHeader Column { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ColumnHoverEventArgs class.
    /// </summary>
    /// <param name="column">The currently hovered column.</param>
    /// <param name="previousColumn">The previously hovered column.</param>
    public ColumnHoverEventArgs(ImageListView.ImageListViewColumnHeader column, ImageListView.ImageListViewColumnHeader previousColumn)
    {
        Column = column;
        PreviousColumn = previousColumn;
    }
}
/// <summary>
/// Represents the event arguments for item related events.
/// </summary>
[Serializable, ComVisible(true)]
public class ItemEventArgs : EventArgs
{
    /// <summary>
    /// Gets the ImageListViewItem that is the target of the event.
    /// </summary>
    public ImageListViewItem Item { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ItemEventArgs class.
    /// </summary>
    /// <param name="item">The item that is the target of this event.</param>
    public ItemEventArgs(ImageListViewItem item)
    {
        Item = item;
    }
}
/// <summary>
/// Represents the event arguments for item click related events.
/// </summary>
[Serializable, ComVisible(true)]
public class ItemClickEventArgs : EventArgs
{
    /// <summary>
    /// Gets the ImageListViewItem that is the target of the event.
    /// </summary>
    public ImageListViewItem Item { get; private set; }
    /// <summary>
    /// Gets the index of the sub item under the hit point.
    /// The index returned is the 0-based index of the column
    /// as displayed on the screen, considering column visibility
    /// and display indices.
    /// Returns -1 if the hit point is not over a sub item.
    /// </summary>
    public int SubItemIndex { get; private set; }
    /// <summary>
    /// Gets the coordinates of the cursor.
    /// </summary>
    public Point Location { get; private set; }
    /// <summary>
    /// Gets the x-coordinates of the cursor.
    /// </summary>
    public int X { get { return Location.X; } }
    /// <summary>
    /// Gets the y-coordinates of the cursor.
    /// </summary>
    public int Y { get { return Location.Y; } }
    /// <summary>
    /// Gets the state of the mouse buttons.
    /// </summary>
    public MouseButtons Buttons { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ItemClickEventArgs class.
    /// </summary>
    /// <param name="item">The item that is the target of this event.</param>
    /// <param name="subItemIndex">Gets the index of the sub item under the hit point.</param>
    /// <param name="location">The location of the mouse.</param>
    /// <param name="buttons">One of the System.Windows.Forms.MouseButtons values 
    /// indicating which mouse button was pressed.</param>
    public ItemClickEventArgs(ImageListViewItem item, int subItemIndex, Point location, MouseButtons buttons)
    {
        Item = item;
        SubItemIndex = subItemIndex;
        Location = location;
        Buttons = buttons;
    }
}
/// <summary>
/// Represents the event arguments for item hover related events.
/// </summary>
[Serializable, ComVisible(true)]
public class ItemHoverEventArgs : EventArgs
{
    /// <summary>
    /// Gets the ImageListViewItem that was previously hovered.
    /// Returns null if there was no previously hovered item.
    /// </summary>
    public ImageListViewItem PreviousItem { get; private set; }
    /// <summary>
    /// Gets the currently hovered ImageListViewItem.
    /// Returns null if there is no hovered item.
    /// </summary>
    public ImageListViewItem Item { get; private set; }
    /// <summary>
    /// Gets the index of the sub item that was previously hovered.
    /// The index returned is the 0-based index of the column
    /// as displayed on the screen, considering column visibility
    /// and display indices.
    /// Returns -1 if the hit point is not over a sub item.
    /// </summary>
    public int PreviousSubItemIndex { get; private set; }
    /// <summary>
    /// Gets the index of the hovered sub item.
    /// The index returned is the 0-based index of the column
    /// as displayed on the screen, considering column visibility
    /// and display indices.
    /// Returns -1 if the hit point is not over a sub item.
    /// </summary>
    public int SubItemIndex { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ItemEventArgs class.
    /// </summary>
    /// <param name="item">The currently hovered item.</param>
    /// <param name="subItemIndex">The index of the hovered sub item.</param>
    /// <param name="previousItem">The previously hovered item.</param>
    /// <param name="previousSubItemIndex">The index of the sub item that was previously hovered.</param>
    public ItemHoverEventArgs(ImageListViewItem item, int subItemIndex, ImageListViewItem previousItem, int previousSubItemIndex)
    {
        Item = item;
        SubItemIndex = subItemIndex;

        PreviousItem = previousItem;
        PreviousSubItemIndex = previousSubItemIndex;
    }
}
/// <summary>
/// Represents the event arguments related to control layout.
/// </summary>
[Serializable, ComVisible(true)]
public class LayoutEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the rectangle bounding the item area.
    /// </summary>
    public Rectangle ItemAreaBounds { get; set; }

    /// <summary>
    /// Initializes a new instance of the LayoutEventArgs class.
    /// </summary>
    /// <param name="itemAreaBounds">The rectangle bounding the item area.</param>
    public LayoutEventArgs(Rectangle itemAreaBounds)
    {
        ItemAreaBounds = itemAreaBounds;
    }
}
/// <summary>
/// Represents the event arguments for the thumbnail caching event.
/// </summary>
[Serializable, ComVisible(true)]
public class ThumbnailCachingEventArgs : EventArgs
{
    /// <summary>
    /// Gets the ImageListViewItem that is the target of the event.
    /// </summary>
    public ImageListViewItem Item { get; private set; }
    /// <summary>
    /// Gets the size of the thumbnail request.
    /// </summary>
    public Size Size { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ThumbnailCachingEventArgs class.
    /// </summary>
    /// <param name="item">The item that is the target of this event.</param>
    /// <param name="size">The size of the thumbnail request.</param>
    public ThumbnailCachingEventArgs(ImageListViewItem item, Size size)
    {
        Item = item;
        Size = size;
    }
}
/// <summary>
/// Represents the event arguments for the thumbnail cached event.
/// </summary>
[Serializable, ComVisible(true)]
public class ThumbnailCachedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the ImageListViewItem that is the target of the event.
    /// </summary>
    public ImageListViewItem Item { get; private set; }
    /// <summary>
    /// Gets the size of the thumbnail request.
    /// </summary>
    public Size Size { get; private set; }
    /// <summary>
    /// Gets the cached thumbnail image.
    /// </summary>
    public Image Thumbnail { get; private set; }
    /// <summary>
    /// Gets whether the cached image is a thumbnail image or
    /// a large image for gallery or pane views.
    /// </summary>
    public bool IsThumbnail { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ThumbnailCachedEventArgs class.
    /// </summary>
    /// <param name="item">The item that is the target of this event.</param>
    /// <param name="thumbnail">The cached thumbnail image.</param>
    /// <param name="size">The size of the thumbnail request.</param>
    /// <param name="thumbnailImage">true if the cached image is a thumbnail image; otherwise false
    /// if the image is a large image for gallery or pane views.</param>
    public ThumbnailCachedEventArgs(ImageListViewItem item, Image thumbnail, Size size, bool thumbnailImage)
    {
        Item = item;
        Thumbnail = thumbnail;
        Size = size;
        IsThumbnail = thumbnailImage;
    }
}
/// <summary>
/// Represents the event arguments for the shell info caching event.
/// </summary>
[Serializable, ComVisible(true)]
public class ShellInfoCachingEventArgs : EventArgs
{
    /// <summary>
    /// Gets the file extension for which the shell info is requested.
    /// </summary>
    public string Extension { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ShellInfoCachingEventArgs class.
    /// </summary>
    /// <param name="extension">The file extension for which the shell info is requested.</param>
    public ShellInfoCachingEventArgs(string extension)
    {
        Extension = extension;
    }
}
/// <summary>
/// Represents the event arguments for the shell info cached event.
/// </summary>
[Serializable, ComVisible(true)]
public class ShellInfoCachedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the file extension for which the shell info is requested.
    /// </summary>
    public string Extension { get; private set; }
    /// <summary>
    /// Gets the small shell icon.
    /// </summary>
    public Image SmallIcon { get; private set; }
    /// <summary>
    /// Gets the large shell icon.
    /// </summary>
    public Image LargeIcon { get; private set; }
    /// <summary>
    /// Gets the shell file type.
    /// </summary>
    public string FileType { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ShellInfoCachedEventArgs class.
    /// </summary>
    /// <param name="extension">The file extension for which the shell info is requested.</param>
    /// <param name="smallIcon">The small shell icon.</param>
    /// <param name="largeIcon">The large shell icon.</param>
    /// <param name="filetype">The shell file type.</param>
    public ShellInfoCachedEventArgs(string extension, Image smallIcon, Image largeIcon, string filetype)
    {
        Extension = extension;
        SmallIcon = smallIcon;
        LargeIcon = largeIcon;
        FileType = filetype;
    }
}
/// <summary>
/// Represents the event arguments for the pane resized event.
/// </summary>
[Serializable, ComVisible(true)]
public class PaneResizedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the width of the pane.
    /// </summary>
    public int PaneWidth { get; private set; }

    /// <summary>
    /// Initializes a new instance of the PaneResizedEventArgs class.
    /// </summary>
    /// <param name="paneWidth">The width of the pane.</param>
    public PaneResizedEventArgs(int paneWidth)
    {
        PaneWidth = paneWidth;
    }
}

/// <summary>
/// Represents the event arguments for the pane resizing event.
/// </summary>
[Serializable, ComVisible(true)]
public class PaneResizingEventArgs : EventArgs
{
    /// <summary>
    /// Gets the width of the pane.
    /// </summary>
    public int PaneWidth { get; private set; }

    /// <summary>
    /// Initializes a new instance of the PaneResizingEventArgs class.
    /// </summary>
    /// <param name="paneWidth">The width of the pane.</param>
    public PaneResizingEventArgs(int paneWidth)
    {
        PaneWidth = paneWidth;
    }
}
/// <summary>
/// Represents the event arguments for item collection related events.
/// </summary>
[Serializable, ComVisible(true)]
public class ItemCollectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the type of action causing the change.
    /// </summary>
    public CollectionChangeAction Action { get; private set; }
    /// <summary>
    /// Gets the ImageListViewItem that is the target of the event.
    /// </summary>
    public ImageListViewItem Item { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ItemCollectionChangedEventArgs class.
    /// </summary>
    /// <param name="action">The type of action causing the change.</param>
    /// <param name="item">The item that is the target of this event. This parameter will be null
    /// if the collection is cleared.</param>
    public ItemCollectionChangedEventArgs(CollectionChangeAction action, ImageListViewItem item)
    {
        Action = action;
        Item = item;
    }
}
#endregion

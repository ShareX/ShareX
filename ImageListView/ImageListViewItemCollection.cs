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

using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ShareX.ImageListView;

public partial class ImageListView
{
    /// <summary>
    /// Represents the collection of items in the image list view.
    /// </summary>
    public class ImageListViewItemCollection : IList<ImageListViewItem>, ICollection, IList, IEnumerable
    {
        #region Member Variables
        private List<ImageListViewItem> mItems;
        internal ImageListView mImageListView;
        private ImageListViewItem mFocused;
        private Dictionary<Guid, ImageListViewItem> lookUp;
        internal bool collectionModified;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageListViewItemCollection"/>  class.
        /// </summary>
        /// <param name="owner">The <see cref="ImageListView"/> owning this collection.</param>
        internal ImageListViewItemCollection(ImageListView owner)
        {
            mItems = new List<ImageListViewItem>();
            lookUp = new Dictionary<Guid, ImageListViewItem>();
            mFocused = null;
            mImageListView = owner;
            collectionModified = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of elements contained in the <see cref="ImageListViewItemCollection"/>.
        /// </summary>
        public int Count
        {
            get { return mItems.Count; }
        }
        /// <summary>
        /// Gets a value indicating whether the <see cref="ImageListViewItemCollection"/> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        /// <summary>
        /// Gets or sets the focused item.
        /// </summary>
        public ImageListViewItem FocusedItem
        {
            get
            {
                return mFocused;
            }
            set
            {
                ImageListViewItem oldFocusedItem = mFocused;
                mFocused = value;
                // Refresh items
                if (oldFocusedItem != mFocused && mImageListView != null)
                    mImageListView.Refresh();
            }
        }
        /// <summary>
        /// Gets the <see cref="ImageListView"/> owning this collection.
        /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets the ImageListView owning this collection.")]
        public ImageListView ImageListView { get { return mImageListView; } }
        /// <summary>
        /// Gets or sets the <see cref="ImageListViewItem"/> at the specified index.
        /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets or sets the item at the specified index.")]
        public ImageListViewItem this[int index]
        {
            get
            {
                return mItems[index];
            }
            set
            {
                ImageListViewItem item = value;
                ImageListViewItem oldItem = mItems[index];

                if (mItems[index] == mFocused)
                    mFocused = item;
                bool oldSelected = mItems[index].Selected;
                item.mIndex = index;
                if (mImageListView != null)
                    item.mImageListView = mImageListView;
                item.owner = this;
                mItems[index] = item;
                lookUp.Remove(oldItem.Guid);
                lookUp.Add(item.Guid, item);
                collectionModified = true;

                if (mImageListView != null)
                {
                    mImageListView.thumbnailCache.Remove(oldItem.Guid);
                    mImageListView.metadataCache.Remove(oldItem.Guid);
                    if (mImageListView.CacheMode == CacheMode.Continuous)
                    {
                        mImageListView.thumbnailCache.Add(item.Guid, item.Adaptor, item.VirtualItemKey,
                            mImageListView.ThumbnailSize, mImageListView.UseEmbeddedThumbnails,
                            mImageListView.AutoRotateThumbnails);
                    }
                    mImageListView.metadataCache.Add(item.Guid, item.Adaptor, item.VirtualItemKey);
                    if (item.Selected != oldSelected)
                        mImageListView.OnSelectionChanged(new EventArgs());
                }
            }
        }
        /// <summary>
        /// Gets the <see cref="ImageListViewItem"/> with the specified Guid.
        /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets or sets the item with the specified Guid.")]
        internal ImageListViewItem this[Guid guid]
        {
            get
            {
                return lookUp[guid];
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Adds an item to the <see cref="ImageListViewItemCollection"/>.
        /// </summary>
        /// <param name="item">The <see cref="ImageListViewItem"/> to add to the <see cref="ImageListViewItemCollection"/>.</param>
        /// <param name="adaptor">The adaptor associated with this item.</param>
        public void Add(ImageListViewItem item, ImageListView.ImageListViewItemAdaptor adaptor)
        {
            AddInternal(item, adaptor);

            if (mImageListView != null)
            {
                if (item.Selected)
                    mImageListView.OnSelectionChangedInternal();
                mImageListView.Refresh();
            }
        }
        /// <summary>
        /// Adds an item to the <see cref="ImageListViewItemCollection"/>.
        /// </summary>
        /// <param name="item">The <see cref="ImageListViewItem"/> to add to the <see cref="ImageListViewItemCollection"/>.</param>
        public void Add(ImageListViewItem item)
        {
            Add(item, mImageListView.defaultAdaptor);
        }
        /// <summary>
        /// Adds an item to the <see cref="ImageListViewItemCollection"/>.
        /// </summary>
        /// <param name="item">The <see cref="ImageListViewItem"/> to add to the <see cref="ImageListViewItemCollection"/>.</param>
        /// <param name="initialThumbnail">The initial thumbnail image for the item.</param>
        /// <param name="adaptor">The adaptor associated with this item.</param>
        public void Add(ImageListViewItem item, Image initialThumbnail, ImageListView.ImageListViewItemAdaptor adaptor)
        {
            item.clonedThumbnail = initialThumbnail;
            Add(item, adaptor);
        }
        /// <summary>
        /// Adds an item to the <see cref="ImageListViewItemCollection"/>.
        /// </summary>
        /// <param name="item">The <see cref="ImageListViewItem"/> to add to the <see cref="ImageListViewItemCollection"/>.</param>
        /// <param name="initialThumbnail">The initial thumbnail image for the item.</param>
        public void Add(ImageListViewItem item, Image initialThumbnail)
        {
            Add(item, initialThumbnail, mImageListView.defaultAdaptor);
        }
        /// <summary>
        /// Adds an item to the <see cref="ImageListViewItemCollection"/>.
        /// </summary>
        /// <param name="filename">The name of the image file.</param>
        public void Add(string filename)
        {
            Add(filename, null);
        }
        /// <summary>
        /// Adds an item to the <see cref="ImageListViewItemCollection"/>.
        /// </summary>
        /// <param name="filename">The name of the image file.</param>
        /// <param name="initialThumbnail">The initial thumbnail image for the item.</param>
        public void Add(string filename, Image initialThumbnail)
        {
            ImageListViewItem item = new(filename);
            item.mAdaptor = mImageListView.defaultAdaptor;
            item.clonedThumbnail = initialThumbnail;
            Add(item);
        }
        /// <summary>
        /// Adds a virtual item to the <see cref="ImageListViewItemCollection"/>.
        /// </summary>
        /// <param name="key">The key identifying the item.</param>
        /// <param name="text">Text of the item.</param>
        /// <param name="adaptor">The adaptor associated with this item.</param>
        public void Add(object key, string text, ImageListView.ImageListViewItemAdaptor adaptor)
        {
            Add(key, text, null, adaptor);
        }
        /// <summary>
        /// Adds a virtual item to the <see cref="ImageListViewItemCollection"/>.
        /// </summary>
        /// <param name="key">The key identifying the item.</param>
        /// <param name="text">Text of the item.</param>
        public void Add(object key, string text)
        {
            Add(key, text, mImageListView.defaultAdaptor);
        }
        /// <summary>
        /// Adds a virtual item to the <see cref="ImageListViewItemCollection"/>.
        /// </summary>
        /// <param name="key">The key identifying the item.</param>
        /// <param name="text">Text of the item.</param>
        /// <param name="initialThumbnail">The initial thumbnail image for the item.</param>
        /// <param name="adaptor">The adaptor associated with this item.</param>
        public void Add(object key, string text, Image initialThumbnail, ImageListView.ImageListViewItemAdaptor adaptor)
        {
            ImageListViewItem item = new(key, text);
            item.clonedThumbnail = initialThumbnail;
            Add(item, adaptor);
        }
        /// <summary>
        /// Adds a virtual item to the <see cref="ImageListViewItemCollection"/>.
        /// </summary>
        /// <param name="key">The key identifying the item.</param>
        /// <param name="text">Text of the item.</param>
        /// <param name="initialThumbnail">The initial thumbnail image for the item.</param>
        public void Add(object key, string text, Image initialThumbnail)
        {
            Add(key, text, initialThumbnail, mImageListView.defaultAdaptor);
        }
        /// <summary>
        /// Adds a range of items to the <see cref="ImageListViewItemCollection"/>.
        /// </summary>
        /// <param name="items">An array of <see cref="ImageListViewItem"/> 
        /// to add to the <see cref="ImageListViewItemCollection"/>.</param>
        /// <param name="adaptor">The adaptor associated with this item.</param>
        public void AddRange(ImageListViewItem[] items, ImageListView.ImageListViewItemAdaptor adaptor)
        {
            if (mImageListView != null)
                mImageListView.SuspendPaint();

            foreach (ImageListViewItem item in items)
                Add(item, adaptor);

            if (mImageListView != null)
            {
                mImageListView.Refresh();
                mImageListView.ResumePaint();
            }
        }
        /// <summary>
        /// Adds a range of items to the <see cref="ImageListViewItemCollection"/>.
        /// </summary>
        /// <param name="items">An array of <see cref="ImageListViewItem"/> 
        /// to add to the <see cref="ImageListViewItemCollection"/>.</param>
        public void AddRange(ImageListViewItem[] items)
        {
            AddRange(items, mImageListView.defaultAdaptor);
        }
        /// <summary>
        /// Adds a range of items to the <see cref="ImageListViewItemCollection"/>.
        /// </summary>
        /// <param name="filenames">The names or the image files.</param>
        public void AddRange(string[] filenames)
        {
            ImageListViewItem[] items = new ImageListViewItem[filenames.Length];

            for (int i = 0; i < filenames.Length; i++)
            {
                items[i] = new ImageListViewItem(filenames[i]);
            }

            AddRange(items);
        }
        /// <summary>
        /// Removes all items from the <see cref="ImageListViewItemCollection"/>.
        /// </summary>
        public void Clear()
        {
            mItems.Clear();

            mFocused = null;
            lookUp.Clear();
            collectionModified = true;

            if (mImageListView != null)
            {
                mImageListView.metadataCache.Clear();
                mImageListView.thumbnailCache.Clear();
                mImageListView.SelectedItems.Clear();

                if (mImageListView.showGroups)
                    Sort();

                mImageListView.Refresh();
            }

            // Raise the clear event
            mImageListView.OnItemCollectionChanged(new ItemCollectionChangedEventArgs(CollectionChangeAction.Refresh, null));
        }
        /// <summary>
        /// Determines whether the <see cref="ImageListViewItemCollection"/> 
        /// contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the 
        /// <see cref="ImageListViewItemCollection"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the 
        /// <see cref="ImageListViewItemCollection"/>; otherwise, false.
        /// </returns>
        public bool Contains(ImageListViewItem item)
        {
            return mItems.Contains(item);
        }
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> 
        /// that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<ImageListViewItem> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }
        /// <summary>
        /// Inserts an item to the <see cref="ImageListViewItemCollection"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The <see cref="ImageListViewItem"/> to 
        /// insert into the <see cref="ImageListViewItemCollection"/>.</param>
        /// <param name="adaptor">The adaptor associated with this item.</param>
        public void Insert(int index, ImageListViewItem item, ImageListView.ImageListViewItemAdaptor adaptor)
        {
            InsertInternal(index, item, adaptor);

            if (mImageListView != null)
            {
                if (item.Selected)
                    mImageListView.OnSelectionChangedInternal();
                mImageListView.Refresh();
            }
        }
        /// <summary>
        /// Inserts an item to the <see cref="ImageListViewItemCollection"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The <see cref="ImageListViewItem"/> to 
        /// insert into the <see cref="ImageListViewItemCollection"/>.</param>
        public void Insert(int index, ImageListViewItem item)
        {
            Insert(index, item, mImageListView.defaultAdaptor);
        }
        /// <summary>
        /// Inserts an item to the <see cref="ImageListViewItemCollection"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new item should be inserted.</param>
        /// <param name="filename">The name of the image file.</param>
        public void Insert(int index, string filename)
        {
            Insert(index, new ImageListViewItem(filename));
        }
        /// <summary>
        /// Inserts an item to the <see cref="ImageListViewItemCollection"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new item should be inserted.</param>
        /// <param name="filename">The name of the image file.</param>
        /// <param name="initialThumbnail">The initial thumbnail image for the item.</param>
        public void Insert(int index, string filename, Image initialThumbnail)
        {
            ImageListViewItem item = new(filename);
            item.clonedThumbnail = initialThumbnail;
            Insert(index, item);
        }
        /// <summary>
        /// Inserts a virtual item to the <see cref="ImageListViewItemCollection"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new item should be inserted.</param>
        /// <param name="key">The key identifying the item.</param>
        /// <param name="text">Text of the item.</param>
        /// <param name="adaptor">The adaptor associated with this item.</param>
        public void Insert(int index, object key, string text, ImageListView.ImageListViewItemAdaptor adaptor)
        {
            Insert(index, key, text, null, adaptor);
        }
        /// <summary>
        /// Inserts a virtual item to the <see cref="ImageListViewItemCollection"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new item should be inserted.</param>
        /// <param name="key">The key identifying the item.</param>
        /// <param name="text">Text of the item.</param>
        public void Insert(int index, object key, string text)
        {
            Insert(index, key, text, mImageListView.defaultAdaptor);
        }
        /// <summary>
        /// Inserts a virtual item to the <see cref="ImageListViewItemCollection"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new item should be inserted.</param>
        /// <param name="key">The key identifying the item.</param>
        /// <param name="text">Text of the item.</param>
        /// <param name="initialThumbnail">The initial thumbnail image for the item.</param>
        /// <param name="adaptor">The adaptor associated with this item.</param>
        public void Insert(int index, object key, string text, Image initialThumbnail, ImageListView.ImageListViewItemAdaptor adaptor)
        {
            ImageListViewItem item = new(key, text);
            item.clonedThumbnail = initialThumbnail;
            Insert(index, item, adaptor);
        }
        /// <summary>
        /// Inserts a virtual item to the <see cref="ImageListViewItemCollection"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new item should be inserted.</param>
        /// <param name="key">The key identifying the item.</param>
        /// <param name="text">Text of the item.</param>
        /// <param name="initialThumbnail">The initial thumbnail image for the item.</param>
        public void Insert(int index, object key, string text, Image initialThumbnail)
        {
            Insert(index, key, text, initialThumbnail, mImageListView.defaultAdaptor);
        }
        /// <summary>
        /// Removes the first occurrence of a specific object 
        /// from the <see cref="ImageListViewItemCollection"/>.
        /// </summary>
        /// <param name="item">The <see cref="ImageListViewItem"/> to remove 
        /// from the <see cref="ImageListViewItemCollection"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the 
        /// <see cref="ImageListViewItemCollection"/>; otherwise, false. This method also 
        /// returns false if <paramref name="item"/> is not found in the original 
        /// <see cref="ImageListViewItemCollection"/>.
        /// </returns>
        public bool Remove(ImageListViewItem item)
        {
            bool ret = RemoveInternal(item, true);

            if (mImageListView != null)
            {
                if (item.Selected)
                    mImageListView.OnSelectionChangedInternal();
                mImageListView.Refresh();
            }

            return ret;
        }
        /// <summary>
        /// Removes the <see cref="ImageListViewItem"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            Remove(mItems[index]);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Determines whether the collection contains the given key.
        /// </summary>
        /// <param name="guid">The key of the item.</param>
        /// <returns>true if the collection contains the given key; otherwise false.</returns>
        internal bool ContainsKey(Guid guid)
        {
            return lookUp.ContainsKey(guid);
        }
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="guid">The key of the item.</param>
        /// <param name="item">the value associated with the specified key, 
        /// if the key is found; otherwise, the default value for the type 
        /// of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the collection contains the given key; otherwise false.</returns>
        internal bool TryGetValue(Guid guid, out ImageListViewItem item)
        {
            return lookUp.TryGetValue(guid, out item);
        }
        /// <summary>
        /// Adds the given item without raising a selection changed event.
        /// </summary>
        /// <param name="item">The <see cref="ImageListViewItem"/> to add.</param>
        /// <param name="adaptor">The adaptor associated with this item.</param>
        /// <returns>true if the item was added; otherwise false.</returns>
        internal bool AddInternal(ImageListViewItem item, ImageListView.ImageListViewItemAdaptor adaptor)
        {
            return InsertInternal(-1, item, adaptor);
        }
        /// <summary>
        /// Inserts the given item without raising a selection changed event.
        /// </summary>
        /// <param name="index">Insertion index. If index is -1 the item is added to the end of the list.</param>
        /// <param name="item">The <see cref="ImageListViewItem"/> to add.</param>
        /// <param name="adaptor">The adaptor associated with this item.</param>
        /// <returns>true if the item was added; otherwise false.</returns>
        internal bool InsertInternal(int index, ImageListViewItem item, ImageListView.ImageListViewItemAdaptor adaptor)
        {
            if (mImageListView == null)
                return false;

            // Check if the file already exists
            if (!string.IsNullOrEmpty(item.FileName) && !mImageListView.AllowDuplicateFileNames)
            {
                if (mItems.Exists(a => string.Compare(a.FileName, item.FileName, StringComparison.OrdinalIgnoreCase) == 0))
                    return false;
            }
            item.owner = this;
            item.mAdaptor = adaptor;
            if (index == -1)
            {
                item.mIndex = mItems.Count;
                mItems.Add(item);
            } else
            {
                item.mIndex = index;
                for (int i = index; i < mItems.Count; i++)
                    mItems[i].mIndex++;
                mItems.Insert(index, item);
            }
            lookUp.Add(item.Guid, item);
            collectionModified = true;

            item.mImageListView = mImageListView;

            // Add current thumbnail to cache
            if (item.clonedThumbnail != null)
            {
                mImageListView.thumbnailCache.Add(item.Guid, item.Adaptor, item.VirtualItemKey, mImageListView.ThumbnailSize,
                    item.clonedThumbnail, mImageListView.UseEmbeddedThumbnails, mImageListView.AutoRotateThumbnails);
                item.clonedThumbnail = null;
            }

            // Add to thumbnail cache
            if (mImageListView.CacheMode == CacheMode.Continuous)
            {
                mImageListView.thumbnailCache.Add(item.Guid, item.Adaptor, item.VirtualItemKey,
                    mImageListView.ThumbnailSize, mImageListView.UseEmbeddedThumbnails, mImageListView.AutoRotateThumbnails);
            }

            // Add to details cache
            mImageListView.metadataCache.Add(item.Guid, item.Adaptor, item.VirtualItemKey);

            // Add to shell info cache
            string extension = item.extension;
            if (!string.IsNullOrEmpty(extension))
            {
                CacheState state = mImageListView.shellInfoCache.GetCacheState(extension);
                if (state == CacheState.Error && mImageListView.RetryOnError == true)
                {
                    mImageListView.shellInfoCache.Remove(extension);
                    mImageListView.shellInfoCache.Add(extension);
                } else if (state == CacheState.Unknown)
                    mImageListView.shellInfoCache.Add(extension);
            }

            // Update groups
            if (mImageListView.showGroups)
                AddRemoveGroupItem(item.Index, true);

            // Raise the add event
            mImageListView.OnItemCollectionChanged(new ItemCollectionChangedEventArgs(CollectionChangeAction.Add, item));

            return true;
        }
        /// <summary>
        /// Removes the given item without raising a selection changed event.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        internal void RemoveInternal(ImageListViewItem item)
        {
            RemoveInternal(item, true);
        }
        /// <summary>
        /// Removes the given item without raising a selection changed event.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="removeFromCache">true to remove item image from cache; otherwise false.</param>
        internal bool RemoveInternal(ImageListViewItem item, bool removeFromCache)
        {
            for (int i = item.mIndex + 1; i < mItems.Count; i++)
                mItems[i].mIndex--;
            if (item == mFocused) mFocused = null;
            if (removeFromCache && mImageListView != null)
            {
                mImageListView.thumbnailCache.Remove(item.Guid);
                mImageListView.metadataCache.Remove(item.Guid);
            }
            bool ret = mItems.Remove(item);
            lookUp.Remove(item.Guid);
            collectionModified = true;

            if (mImageListView != null)
            {
                // Raise the remove event
                mImageListView.OnItemCollectionChanged(new ItemCollectionChangedEventArgs(CollectionChangeAction.Remove, item));

                if (mImageListView.showGroups)
                    AddRemoveGroupItem(item.Index, false);
            }

            return ret;
        }
        /// <summary>
        /// Returns the index of the specified item.
        /// </summary>
        internal int IndexOf(ImageListViewItem item)
        {
            return item.Index;
        }
        /// <summary>
        /// Returns the index of the item with the specified Guid.
        /// </summary>
        internal int IndexOf(Guid guid)
        {
            return lookUp.TryGetValue(guid, out ImageListViewItem item) ? item.Index : -1;
        }
        /// <summary>
        /// Sorts the items by the sort order and sort column of the owner.
        /// </summary>
        internal void Sort()
        {
            if (mImageListView == null)
                return;

            mImageListView.showGroups = false;
            mImageListView.groups.Clear();

            if ((mImageListView.GroupOrder == SortOrder.None || mImageListView.GroupColumn < 0 || mImageListView.GroupColumn >= mImageListView.Columns.Count) &&
               (mImageListView.SortOrder == SortOrder.None || mImageListView.SortColumn < 0 || mImageListView.SortColumn >= mImageListView.Columns.Count))
                return;

            // Display wait cursor while sorting
            Cursor cursor = mImageListView.Cursor;
            mImageListView.Cursor = Cursors.WaitCursor;

            // Sort and group items
            ImageListViewColumnHeader sortColumn = null;
            ImageListViewColumnHeader groupColumn = null;
            if (mImageListView.GroupColumn >= 0 && mImageListView.GroupColumn < mImageListView.Columns.Count)
                groupColumn = mImageListView.Columns[mImageListView.GroupColumn];
            if (mImageListView.SortColumn >= 0 || mImageListView.SortColumn < mImageListView.Columns.Count)
                sortColumn = mImageListView.Columns[mImageListView.SortColumn];
            if (mItems.Count == 1 && groupColumn != null)
                mItems[0].UpdateGroup(groupColumn);
            mItems.Sort(new ImageListViewItemComparer(groupColumn, mImageListView.GroupOrder, sortColumn, mImageListView.SortOrder));
            if (mImageListView.GroupOrder != SortOrder.None && groupColumn != null)
                mImageListView.showGroups = true;

            // Update item indices and create groups
            string lastGroup = string.Empty;
            for (int i = 0; i < mItems.Count; i++)
            {
                ImageListViewItem item = mItems[i];
                item.mIndex = i;
                string group = item.group;

                if (string.Compare(lastGroup, group, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    lastGroup = group;
                    mImageListView.groups.Add(group, i, i);
                } else if (mImageListView.groups.HasName(lastGroup))
                {
                    mImageListView.groups[lastGroup].LastItemIndex = i;
                }
            }

            // Restore previous cursor
            mImageListView.Cursor = cursor;
            collectionModified = true;
        }
        /// <summary>
        /// Updates groups after adding or removing an item. This just updates
        /// the count of items in groups, it DOES NOT re-sort the items.
        /// </summary>
        /// <param name="index">The index of the new or removed item.</param>
        /// <param name="add">true to add an item; false to remove a item.</param>
        private void AddRemoveGroupItem(int index, bool add)
        {
            if (mImageListView == null || !mImageListView.showGroups)
                return;
            if (mImageListView.groups.Count == 0)
            {
                Sort();
                return;
            }

            // Special case of adding an item to the end
            ImageListView.ImageListViewGroup lastGroup = mImageListView.groups[mImageListView.groups.Count - 1];
            if (add && index == lastGroup.LastItemIndex + 1)
            {
                lastGroup.LastItemIndex++;
                return;
            }

            // Insert into a group
            List<ImageListView.ImageListViewGroup> emptyGroups = new();
            foreach (ImageListView.ImageListViewGroup group in mImageListView.groups)
            {
                if (group.LastItemIndex < index)
                    continue;
                else if (group.FirstItemIndex <= index && group.LastItemIndex >= index)
                {
                    if (add)
                    {
                        group.LastItemIndex++;
                    } else
                    {
                        group.LastItemIndex--;
                    }
                } else // if (group.FirstItemIndex > index)
                {
                    if (add)
                    {
                        group.FirstItemIndex++;
                        group.LastItemIndex++;
                    } else
                    {
                        group.FirstItemIndex--;
                        group.LastItemIndex--;
                    }
                }

                if (group.ItemCount == 0)
                    emptyGroups.Add(group);
            }

            // Purge empty groups
            foreach (ImageListView.ImageListViewGroup group in emptyGroups)
                mImageListView.groups.Remove(group);
        }
        #endregion

        #region ImageListViewItemComparer
        /// <summary>
        /// Compares items by the sort order and sort column of the owner.
        /// </summary>
        private class ImageListViewItemComparer : IComparer<ImageListViewItem>
        {
            private readonly ImageListViewColumnHeader mGroupColumn;
            private ImageListViewColumnHeader mSortColumn;
            private readonly SortOrder mGroupOrder;
            private readonly SortOrder mSortOrder;

            public ImageListViewItemComparer(ImageListViewColumnHeader groupColumn, SortOrder groupOrder, ImageListViewColumnHeader sortColumn, SortOrder sortOrder)
            {
                mGroupColumn = groupColumn;
                mSortColumn = sortColumn;
                mGroupOrder = groupOrder;
                mSortOrder = sortOrder;
            }

            /// <summary>
            /// Compares two strings and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            private int CompareStrings(string x, string y, bool natural)
            {
                if (!natural)
                    return string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase);

                // Following natural sort algorithm is taken from:
                // http://www.interact-sw.co.uk/iangblog/2007/12/13/natural-sorting
                string[] xparts = Regex.Split(x.Replace(" ", ""), "([0-9]+)");
                string[] yparts = Regex.Split(y.Replace(" ", ""), "([0-9]+)");
                for (int i = 0; i < Math.Max(xparts.Length, yparts.Length); i++)
                {
                    bool hasx = (i < xparts.Length);
                    bool hasy = (i < yparts.Length);

                    if (!(hasx || hasy)) return 0;

                    if (!hasx) return -1;
                    if (!hasy) return 1;

                    string xpart = xparts[i];
                    string ypart = yparts[i];
                    int res = int.TryParse(xpart, out int xi) && int.TryParse(ypart, out int yi)
                        ? xi < yi ? -1 : (xi > yi ? 1 : 0)
                        : string.Compare(xpart, ypart, StringComparison.InvariantCultureIgnoreCase);
                    if (res != 0) return res;
                }
                return 0;
            }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            public int Compare(ImageListViewItem? x, ImageListViewItem? y)
            {
                int result = 0,sign = 0;
                if (mGroupOrder != SortOrder.None)
                {
                    result = 0;
                    sign = ((mGroupOrder == SortOrder.Ascending || mGroupOrder == SortOrder.AscendingNatural) ? 1 : -1);

                    x?.UpdateGroup(mGroupColumn);
                    y?.UpdateGroup(mGroupColumn);
                    result = (x?.groupOrder < y?.groupOrder ? -1 : (x?.groupOrder > y?.groupOrder ? 1 : 0));
                    if (result != 0)
                        return sign * result;
                }

                if (mSortOrder != SortOrder.None)
                {
                    result = 0;
                    sign = ((mSortOrder == SortOrder.Ascending || mSortOrder == SortOrder.AscendingNatural) ? 1 : -1);
                    bool natural = (mSortOrder == SortOrder.AscendingNatural || mSortOrder == SortOrder.DescendingNatural);
                    if (mSortColumn != null)
                    {
                        switch (mSortColumn.Type)
                        {
                            case ColumnType.DateAccessed:
                                result = DateTime.Compare(x.DateAccessed, y.DateAccessed);
                                break;
                            case ColumnType.DateCreated:
                                result = DateTime.Compare(x.DateCreated, y.DateCreated);
                                break;
                            case ColumnType.DateModified:
                                result = DateTime.Compare(x.DateModified, y.DateModified);
                                break;
                            case ColumnType.Dimensions:
                                long ax = x.Dimensions.Width * x.Dimensions.Height;
                                long ay = y.Dimensions.Width * y.Dimensions.Height;
                                result = (ax < ay ? -1 : (ax > ay ? 1 : 0));
                                break;
                            case ColumnType.FileName:
                                result = CompareStrings(x.FileName, y.FileName, natural);
                                break;
                            case ColumnType.FilePath:
                                result = CompareStrings(x.FilePath, y.FilePath, natural);
                                break;
                            case ColumnType.FileSize:
                                result = (x.FileSize < y.FileSize ? -1 : (x.FileSize > y.FileSize ? 1 : 0));
                                break;
                            case ColumnType.FileType:
                                result = CompareStrings(x.FileType, y.FileType, natural);
                                break;
                            case ColumnType.Name:
                                result = CompareStrings(x.Text, y.Text, natural);
                                break;
                            case ColumnType.Resolution:
                                float rx = x.Resolution.Width * x.Resolution.Height;
                                float ry = y.Resolution.Width * y.Resolution.Height;
                                result = (rx < ry ? -1 : (rx > ry ? 1 : 0));
                                break;
                            case ColumnType.ImageDescription:
                                result = CompareStrings(x.ImageDescription, y.ImageDescription, natural);
                                break;
                            case ColumnType.EquipmentModel:
                                result = CompareStrings(x.EquipmentModel, y.EquipmentModel, natural);
                                break;
                            case ColumnType.DateTaken:
                                result = DateTime.Compare(x.DateTaken, y.DateTaken);
                                break;
                            case ColumnType.Artist:
                                result = CompareStrings(x.Artist, y.Artist, natural);
                                break;
                            case ColumnType.Copyright:
                                result = CompareStrings(x.Copyright, y.Copyright, natural);
                                break;
                            case ColumnType.ExposureTime:
                                result = (x.ExposureTime < y.ExposureTime ? -1 : (x.ExposureTime > y.ExposureTime ? 1 : 0));
                                break;
                            case ColumnType.FNumber:
                                result = (x.FNumber < y.FNumber ? -1 : (x.FNumber > y.FNumber ? 1 : 0));
                                break;
                            case ColumnType.ISOSpeed:
                                result = (x.ISOSpeed < y.ISOSpeed ? -1 : (x.ISOSpeed > y.ISOSpeed ? 1 : 0));
                                break;
                            case ColumnType.UserComment:
                                result = CompareStrings(x.UserComment, y.UserComment, natural);
                                break;
                            case ColumnType.Rating:
                                result = (x.Rating < y.Rating ? -1 : (x.Rating > y.Rating ? 1 : 0));
                                break;
                            case ColumnType.Software:
                                result = CompareStrings(x.Software, y.Software, natural);
                                break;
                            case ColumnType.FocalLength:
                                result = (x.FocalLength < y.FocalLength ? -1 : (x.FocalLength > y.FocalLength ? 1 : 0));
                                break;
                            case ColumnType.Custom:
                                result = mSortColumn.Comparer != null
                                    ? mSortColumn.Comparer.Compare(x, y)
                                    : CompareStrings(x.SubItems[mSortColumn].Text, y.SubItems[mSortColumn].Text, natural);
                                break;
                            default:
                                result = 0;
                                break;
                        }
                    }
                }

                return sign * result;
            }
        }
        #endregion

        #region Unsupported Interface
        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        void ICollection<ImageListViewItem>.CopyTo(ImageListViewItem[] array, int arrayIndex)
        {
            mItems.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        [Obsolete("Use ImageListViewItem.Index property instead.")]
        int IList<ImageListViewItem>.IndexOf(ImageListViewItem item)
        {
            return mItems.IndexOf(item);
        }
        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        void ICollection.CopyTo(Array array, int index)
        {
            if (array is not ImageListViewItem[])
                throw new ArgumentException("An array of ImageListViewItem is required.", "array");
            mItems.CopyTo((ImageListViewItem[])array, index);
        }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        int ICollection.Count
        {
            get { return mItems.Count; }
        }
        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }
        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException(); }
        }
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        int IList.Add(object value)
        {
            if (value is not ImageListViewItem)
                throw new ArgumentException("An object of type ImageListViewItem is required.", "value");
            ImageListViewItem item = (ImageListViewItem)value;
            Add(item);
            return mItems.IndexOf(item);
        }
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IList"/> contains a specific value.
        /// </summary>
        bool IList.Contains(object? value) => value is not ImageListViewItem
                ? throw new ArgumentException("An object of type ImageListViewItem is required.", "value")
                : mItems.Contains((ImageListViewItem)value);
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => mItems.GetEnumerator();
        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        int IList.IndexOf(object? value) => value is not ImageListViewItem
                ? throw new ArgumentException("An object of type ImageListViewItem is required.", "value")
                : IndexOf((ImageListViewItem)value);
        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList"/> at the specified index.
        /// </summary>
        void IList.Insert(int index, object? value)
        {
            if (value is not ImageListViewItem)
                throw new ArgumentException("An object of type ImageListViewItem is required.", "value");
            Insert(index, (ImageListViewItem)value);
        }
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        bool IList.IsFixedSize => false;
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        void IList.Remove(object? value)
        {
            if (value is not ImageListViewItem)
                throw new ArgumentException("An object of type ImageListViewItem is required.", "value");
            ImageListViewItem item = (ImageListViewItem)value;
            Remove(item);
        }
        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        object IList.this[int index]
        {
            get => this[index];
            set
            {
                if (value is not ImageListViewItem)
                    throw new ArgumentException("An object of type ImageListViewItem is required.", "value");
                this[index] = (ImageListViewItem)value;
            }
        }
        #endregion
    }
}
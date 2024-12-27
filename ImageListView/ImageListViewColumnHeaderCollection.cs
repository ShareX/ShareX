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

namespace ShareX.ImageListView;

public partial class ImageListView
{
    /// <summary>
    /// Represents the collection of columns in an ImageListView control.
    /// </summary>
    public class ImageListViewColumnHeaderCollection : IList<ImageListViewColumnHeader>, ICollection, IList, IEnumerable
    {
        #region Member Variables
        private ImageListView mImageListView;
        private List<ImageListViewColumnHeader> mItems;
        private List<ImageListViewColumnHeader> mDisplayedItems;
        internal bool updateDisplayList;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of columns in the collection.
        /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets the number of columns in the collection.")]
        public int Count { get { return mItems.Count; } }
        /// <summary>
        /// Gets the ImageListView owning this collection.
        /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets the ImageListView owning this collection.")]
        public ImageListView ImageListView { get { return mImageListView; } }
        /// <summary>
        /// Gets the column at the specified index within the collection.
        /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets or item at the specified index within the collection.")]
        public ImageListViewColumnHeader this[int index]
        {
            get
            {
                return mItems[index];
            }
            set
            {
                mItems[index] = value;

                updateDisplayList = true;
            }
        }
        /// <summary>
        /// Gets the column with the specified type within the collection.
        /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets or sets the item with the specified type within the collection.")]
        public ImageListViewColumnHeader this[ColumnType type]
        {
            get
            {
                if (type == ColumnType.Custom)
                    throw new ArgumentException("Column type is ambiguous. You must access custom columns by index.", "type");

                foreach (ImageListViewColumnHeader column in this)
                    if (column.Type == type) return column;
                throw new ArgumentException("Unknown column type.", "type");
            }
        }
        /// <summary>
        /// Gets a value indicating whether the Collection is read-only.
        /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets a value indicating whether the Collection is read-only.")]
        public bool IsReadOnly
        {
            get { return false; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ImageListViewColumnHeaderCollection class.
        /// </summary>
        /// <param name="owner">The owner control.</param>
        internal ImageListViewColumnHeaderCollection(ImageListView owner)
        {
            mImageListView = owner;
            mItems = [];
            updateDisplayList = true;
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public void Add(ImageListViewColumnHeader item)
        {
            if (mImageListView == null)
                throw new InvalidOperationException("Owner control is null.");

            item.mImageListView = mImageListView;
            item.owner = this;
            if (item.DisplayIndex == -1)
                item.DisplayIndex = mItems.Count;

            mItems.Add(item);

            updateDisplayList = true;
        }
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="key">The key to associate this column with sub items.</param>
        /// <param name="text">Text of the column header.</param>
        /// <param name="width">Width in pixels of the column header.</param>
        public void Add(string key, string text, int width)
        {
            Add(new ImageListViewColumnHeader(ColumnType.Custom, key, text, width));
        }
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="key">The key to associate this column with sub items.</param>
        /// <param name="text">Text of the column header.</param>
        public void Add(string key, string text)
        {
            Add(new ImageListViewColumnHeader(ColumnType.Custom, key, text));
        }
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="type">The type of data to display in this column.</param>
        /// <param name="text">Text of the column header.</param>
        /// <param name="width">Width in pixels of the column header.</param>
        public void Add(ColumnType type, string text, int width)
        {
            Add(new ImageListViewColumnHeader(type, text, width));
        }
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="type">The type of data to display in this column.</param>
        /// <param name="text">Text of the column header.</param>
        public void Add(ColumnType type, string text)
        {
            Add(new ImageListViewColumnHeader(type, text));
        }
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="type">The type of data to display in this column.</param>
        /// <param name="width">Width in pixels of the column header.</param>
        public void Add(ColumnType type, int width)
        {
            Add(new ImageListViewColumnHeader(type, width));
        }
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="type">The type of data to display in this column.</param>
        public void Add(ColumnType type)
        {
            Add(new ImageListViewColumnHeader(type));
        }
        /// <summary>
        /// Adds a range of items to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="items">The items to add to the collection.</param>
        public void AddRange(ImageListViewColumnHeader[] items)
        {
            if (mImageListView != null)
                mImageListView.SuspendPaint();

            foreach (ImageListViewColumnHeader item in items)
                Add(item);

            if (mImageListView != null)
            {
                mImageListView.Refresh();
                mImageListView.ResumePaint();
            }
        }
        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        public void Clear()
        {
            mItems.Clear();
            updateDisplayList = true;
        }
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        public bool Contains(ImageListViewColumnHeader item)
        {
            return mItems.Contains(item);
        }
        /// <summary>
        /// Returns an enumerator to use to iterate through columns.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{ImageListViewColumnHeader}"/> that represents the item collection.</returns>
        public IEnumerator<ImageListViewColumnHeader> GetEnumerator()
        {
            foreach (ImageListViewColumnHeader column in mItems)
                yield return column;
            yield break;
        }
        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(ImageListViewColumnHeader item)
        {
            return mItems.IndexOf(item);
        }
        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public void Insert(int index, ImageListViewColumnHeader item)
        {
            if (mImageListView == null)
                throw new InvalidOperationException("Owner control is null.");

            item.mImageListView = mImageListView;
            item.owner = this;
            if (item.DisplayIndex == -1)
            {
                foreach (ImageListViewColumnHeader col in mItems)
                    if (col.DisplayIndex >= index)
                        col.DisplayIndex++;
                item.DisplayIndex = index;
            }

            mItems.Insert(index, item);

            updateDisplayList = true;
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public bool Remove(ImageListViewColumnHeader item)
        {
            bool exists = mItems.Remove(item);

            updateDisplayList = true;
            return exists;
        }
        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            mItems.RemoveAt(index);
            ImageListViewColumnHeader item = mItems[index];

            updateDisplayList = true;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Determines whether the collection has the given column type.
        /// </summary>
        /// <param name="type">The type of column.</param>
        internal bool HasType(ColumnType type)
        {
            foreach (ImageListViewColumnHeader column in this)
                if (column.Type == type) return true;

            return false;
        }
        /// <summary>
        /// Gets the columns as diplayed on the UI.
        /// </summary>
        /// <returns>The list of of visible columns.</returns>
        public List<ImageListViewColumnHeader> GetDisplayedColumns()
        {
            if (mDisplayedItems != null && !updateDisplayList)
                return mDisplayedItems;

            mDisplayedItems = new List<ImageListViewColumnHeader>();
            foreach (ImageListViewColumnHeader column in mItems)
            {
                if (column.Visible)
                    mDisplayedItems.Add(column);
            }
            mDisplayedItems.Sort(ColumnCompare);

            updateDisplayList = false;
            return mDisplayedItems;
        }
        /// <summary>
        /// Compares the columns by their display index.
        /// </summary>
        internal static int ColumnCompare(ImageListViewColumnHeader a, ImageListViewColumnHeader b)
        {
            return a.DisplayIndex < b.DisplayIndex ? -1 : a.DisplayIndex > b.DisplayIndex ? 1 : 0;
        }
        #endregion

        #region Unsupported Interface
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        void ICollection.CopyTo(Array array, int index)
        {
            if (!(array is ImageListViewColumnHeader[]))
                throw new ArgumentException("An array of ImageListViewColumnHeader is required.", "array");
            mItems.CopyTo((ImageListViewColumnHeader[])array, index);
        }
        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        void ICollection<ImageListViewColumnHeader>.CopyTo(ImageListViewColumnHeader[] array, int arrayIndex)
        {
            mItems.CopyTo(array, arrayIndex);
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
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        int IList.Add(object value)
        {
            if (!(value is ImageListViewColumnHeader))
                throw new ArgumentException("An object of type ImageListViewColumnHeader is required.", "value");
            ImageListViewColumnHeader item = (ImageListViewColumnHeader)value;
            Add(item);
            return mItems.IndexOf(item);
        }
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IList"/> contains a specific value.
        /// </summary>
        bool IList.Contains(object value)
        {
            return !(value is ImageListViewColumnHeader)
                ? throw new ArgumentException("An object of type ImageListViewColumnHeader is required.", "value")
                : mItems.Contains((ImageListViewColumnHeader)value);
        }
        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        int IList.IndexOf(object value) => !(value is ImageListViewColumnHeader)
                ? throw new ArgumentException("An object of type ImageListViewColumnHeader is required.", "value")
                : IndexOf((ImageListViewColumnHeader)value);
        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList"/> at the specified index.
        /// </summary>
        void IList.Insert(int index, object value)
        {
            if (!(value is ImageListViewColumnHeader))
                throw new ArgumentException("An object of type ImageListViewColumnHeader is required.", "value");
            Insert(index, (ImageListViewColumnHeader)value);
        }
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        bool IList.IsFixedSize => false;
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        void IList.Remove(object value)
        {
            if (value is not ImageListViewColumnHeader)
                throw new ArgumentException("An object of type ImageListViewColumnHeader is required.", "value");
            Remove((ImageListViewColumnHeader)value);
        }
        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        object IList.this[int index]
        {
            get => this[index];
            set
            {
                if (value is not ImageListViewColumnHeader)
                    throw new ArgumentException("An object of type ImageListViewColumnHeader is required.", "value");
                this[index] = (ImageListViewColumnHeader)value;
                updateDisplayList = true;
            }
        }
        #endregion
    }
}
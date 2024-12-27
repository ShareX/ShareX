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
    /// Represents the collection of groups in an ImageListView control.
    /// </summary>
    internal class ImageListViewGroupCollection : IList<ImageListViewGroup>, ICollection, IList, IEnumerable
    {
        #region Member Variables
        private ImageListView mImageListView;
        private List<ImageListViewGroup> mItems;
        internal bool collectionModified;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of groups in the collection.
        /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets the number of groups in the collection.")]
        public int Count { get { return mItems.Count; } }
        /// <summary>
        /// Gets the ImageListView owning this collection.
        /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets the ImageListView owning this collection.")]
        public ImageListView ImageListView { get { return mImageListView; } }
        /// <summary>
        /// Gets or sets the group at the specified index within the collection.
        /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets or sets the group at the specified index within the collection.")]
        public ImageListViewGroup this[int index]
        {
            get
            {
                return mItems[index];
            }
            set
            {
                mItems[index] = value;
                collectionModified = true;
            }
        }
        /// <summary>
        /// Gets the group with the specified name within the collection.
        /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets the group with the specified name within the collection.")]
        public ImageListViewGroup this[string name]
        {
            get
            {
                foreach (ImageListViewGroup group in this)
                    if (string.Compare(group.Name, name) == 0) return group;
                throw new ArgumentException("Unknown group name.", "name");
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

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ImageListViewGroup class.
        /// </summary>
        /// <param name="owner">The owner control.</param>
        internal ImageListViewGroupCollection(ImageListView owner)
        {
            mImageListView = owner;
            mItems = new List<ImageListViewGroup>();
            collectionModified = true;
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public void Add(ImageListViewGroup item)
        {
            if (mImageListView == null)
                throw new InvalidOperationException("Owner control is null.");

            item.mImageListView = mImageListView;
            item.owner = this;

            mItems.Add(item);

            collectionModified = true;
        }
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="name">The name of the group.</param>
        /// <param name="firstItemIndex">The index of the first item.</param>
        /// <param name="lastItemIndex">The index of the last item.</param>
        public void Add(string name, int firstItemIndex, int lastItemIndex)
        {
            Add(new ImageListViewGroup(name, firstItemIndex, lastItemIndex));
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        public void Clear()
        {
            mItems.Clear();

            collectionModified = true;
        }
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        public bool Contains(ImageListViewGroup item)
        {
            return mItems.Contains(item);
        }
        /// <summary>
        /// Returns an enumerator to use to iterate through columns.
        /// </summary>
        /// <returns>An IEnumerator&lt;ImageListViewColumn&gt; that represents the item collection.</returns>
        public IEnumerator<ImageListViewGroup> GetEnumerator()
        {
            foreach (ImageListViewGroup group in mItems)
                yield return group;
            yield break;
        }
        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(ImageListViewGroup item)
        {
            return mItems.IndexOf(item);
        }
        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public void Insert(int index, ImageListViewGroup item)
        {
            if (mImageListView == null)
                throw new InvalidOperationException("Owner control is null.");

            item.mImageListView = mImageListView;
            item.owner = this;

            mItems.Insert(index, item);

            collectionModified = true;
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public bool Remove(ImageListViewGroup item)
        {
            bool ret = mItems.Remove(item);
            collectionModified = true;
            return ret;
        }
        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            mItems.RemoveAt(index);
            collectionModified = true;
        }
        /// <summary>
        /// Determines whether the collection has the group with the given name.
        /// </summary>
        /// <param name="name">The name of the group.</param>
        internal bool HasName(string name)
        {
            foreach (ImageListViewGroup group in this)
                if (string.Compare(group.Name, name) == 0) return true;

            return false;
        }
        /// <summary>
        /// Gets the list of visible groups.
        /// </summary>
        internal List<ImageListViewGroup> GetDisplayedGroups()
        {
            List<ImageListViewGroup> visible = new();
            foreach (ImageListViewGroup group in this)
            {
                if (group.isVisible)
                    visible.Add(group);
            }
            return visible;
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
            if (!(array is ImageListViewGroup[]))
                throw new ArgumentException("An array of ImageListViewGroup is required.", "array");
            mItems.CopyTo((ImageListViewGroup[])array, index);
        }
        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        void ICollection<ImageListViewGroup>.CopyTo(ImageListViewGroup[] array, int arrayIndex)
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
            if (!(value is ImageListViewGroup))
                throw new ArgumentException("An object of type ImageListViewGroup is required.", "value");
            ImageListViewGroup item = (ImageListViewGroup)value;
            Add(item);
            return mItems.IndexOf(item);
        }
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IList"/> contains a specific value.
        /// </summary>
        bool IList.Contains(object value)
        {
            return !(value is ImageListViewGroup)
                ? throw new ArgumentException("An object of type ImageListViewGroup is required.", "value")
                : mItems.Contains((ImageListViewGroup)value);
        }
        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        int IList.IndexOf(object value)
        {
            return !(value is ImageListViewGroup)
                ? throw new ArgumentException("An object of type ImageListViewGroup is required.", "value")
                : IndexOf((ImageListViewGroup)value);
        }
        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList"/> at the specified index.
        /// </summary>
        void IList.Insert(int index, object value)
        {
            if (!(value is ImageListViewGroup))
                throw new ArgumentException("An object of type ImageListViewGroup is required.", "value");
            Insert(index, (ImageListViewGroup)value);
        }
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        bool IList.IsFixedSize
        {
            get { return false; }
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        void IList.Remove(object value)
        {
            if (value is not ImageListViewGroup)
                throw new ArgumentException("An object of type ImageListViewGroup is required.", "value");
            Remove((ImageListViewGroup)value);
        }
        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        object IList.this[int index]
        {
            get => this[index];
            set
            {
                if (!(value is ImageListViewGroup))
                    throw new ArgumentException("An object of type ImageListViewGroup is required.", "value");
                this[index] = (ImageListViewGroup)value;
                collectionModified = true;
            }
        }
        #endregion
    }
}
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
    /// Represents the collection of selected items in the image list view.
    /// </summary>
    public class ImageListViewSelectedItemCollection : IList<ImageListViewItem>
    {
        #region Member Variables
        internal ImageListView mImageListView;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageListViewSelectedItemCollection"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="ImageListView"/> owning this collection.</param>
        internal ImageListViewSelectedItemCollection(ImageListView owner)
        {
            mImageListView = owner;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of elements contained in the <see cref="ImageListViewSelectedItemCollection"/>.
        /// </summary>
        [Category("Behavior"), Browsable(true), Description("Gets the number of elements contained in the collection.")]
        public int Count
        {
            get
            {
                int count = 0;
                foreach (ImageListViewItem item in mImageListView.mItems)
                    if (item.Selected && item.Enabled) count++;
                return count;
            }
        }            /// <summary>
                     /// Gets a value indicating whether the <see cref="ImageListViewSelectedItemCollection"/> is read-only.
                     /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets a value indicating whether the collection is read-only.")]
        public bool IsReadOnly { get { return true; } }
        /// <summary>
        /// Gets the <see cref="ImageListView"/> owning this collection.
        /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets the ImageListView owning this collection.")]
        public ImageListView ImageListView { get { return mImageListView; } }
        /// <summary>
        /// Gets or sets the <see cref="ImageListViewItem"/> at the specified index.
        /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets or sets the item at the specified index")]
        public ImageListViewItem this[int index]
        {
            get
            {
                int i = 0;
                foreach (ImageListViewItem item in this)
                {
                    if (i == index)
                        return item;
                    i++;
                }
                throw new ArgumentException("No item with the given index exists.", "index");
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Determines whether the <see cref="ImageListViewSelectedItemCollection"/> contains a specific value.
        /// </summary>
        /// <param name="item">The <see cref="ImageListViewItem"/> to locate in the <see cref="ImageListViewSelectedItemCollection"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="ImageListViewSelectedItemCollection"/>; otherwise, false.
        /// </returns>
        public bool Contains(ImageListViewItem item)
        {
            return (item.Selected && item.Enabled && mImageListView.Items.Contains(item));
        }
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<ImageListViewItem> GetEnumerator()
        {
            return new ImageListViewSelectedItemEnumerator(mImageListView.mItems);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        internal void Clear()
        {
            Clear(true);
        }
        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        internal void Clear(bool raiseEvent)
        {
            foreach (ImageListViewItem item in this)
                item.mSelected = false;
            if (raiseEvent && mImageListView != null)
                mImageListView.OnSelectionChangedInternal();
        }
        #endregion

        #region Unsupported Interface
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        void ICollection<ImageListViewItem>.Add(ImageListViewItem item)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        void ICollection<ImageListViewItem>.Clear()
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        void ICollection<ImageListViewItem>.CopyTo(ImageListViewItem[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        [Obsolete("Use ImageListViewItem.Index property instead.")]
        int IList<ImageListViewItem>.IndexOf(ImageListViewItem item)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        void IList<ImageListViewItem>.Insert(int index, ImageListViewItem item)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        bool ICollection<ImageListViewItem>.Remove(ImageListViewItem item)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        void IList<ImageListViewItem>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        ImageListViewItem IList<ImageListViewItem>.this[int index]
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Internal Classes
        /// <summary>
        /// Represents an enumerator to walk though the selected items.
        /// </summary>
        internal class ImageListViewSelectedItemEnumerator : IEnumerator<ImageListViewItem>
        {
            #region Member Variables
            private ImageListViewItemCollection owner;
            private int current;
            private Guid lastItem;
            #endregion

            #region Constructor
            public ImageListViewSelectedItemEnumerator(ImageListViewItemCollection collection)
            {
                owner = collection;
                current = -1;
                lastItem = Guid.Empty;
            }
            #endregion

            #region Properties
            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            public ImageListViewItem Current
            {
                get
                {
                    return current == -1 || current > owner.Count - 1 ? throw new InvalidOperationException() : owner[current];
                }
            }
            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            object IEnumerator.Current
            {
                get { return Current; }
            }
            #endregion

            #region Instance Methods
            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                ;
            }
            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            public bool MoveNext()
            {
                // Did we reach the end?
                if (current > owner.Count - 1)
                {
                    lastItem = Guid.Empty;
                    return false;
                }

                // Move to the next item if:
                // 1. We are before the first item. - OR -
                // 2. The current item is the same as the one we enumerated before. 
                //    The current item may have differed if the user for example 
                //    removed the current item between MoveNext calls. - OR -
                // 3. The current item is not selected.
                // 3. The current item is not enabled.
                while (current == -1 ||
                    owner[current].Guid == lastItem ||
                    owner[current].Selected == false ||
                    owner[current].Enabled == false)
                {
                    current++;
                    if (current > owner.Count - 1)
                    {
                        lastItem = Guid.Empty;
                        return false;
                    }
                }

                // Cache the last item
                lastItem = owner[current].Guid;
                return true;
            }
            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                current = -1;
                lastItem = Guid.Empty;
            }
            #endregion
        }
        #endregion
    }
}
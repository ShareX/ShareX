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

namespace ShareX.ImageListView;

public partial class ImageListView
{
    /// <summary>
    /// Represents the collection of items in a group in an ImageListView control.
    /// </summary>
    public class ImageListViewGroup : IEnumerable<ImageListViewItem>, IEnumerable, IComparable<ImageListViewGroup>
    {
        #region Member Variables
        internal ImageListView mImageListView;
        internal ImageListViewGroupCollection owner;
        private bool mCollapsed;
        // Layout variables
        internal int itemCols;
        internal int itemRows;
        internal Rectangle itemBounds;
        internal Rectangle headerBounds;
        internal bool isVisible;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the group.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the index of the first item.
        /// </summary>
        public int FirstItemIndex { get; internal set; }
        /// <summary>
        /// Gets the index of the last item.
        /// </summary>
        public int LastItemIndex { get; internal set; }
        /// <summary>
        /// Gets or sets whether the group is collapsed.
        /// </summary>
        public bool Collapsed
        {
            get
            {
                return mCollapsed;
            }
            set
            {
                if (value != mCollapsed)
                {
                    mCollapsed = value;
                    if (owner != null)
                        owner.collectionModified = true;
                    if (mImageListView != null)
                        mImageListView.Refresh();
                }
            }
        }
        /// <summary>
        /// Gets the item count.
        /// </summary>
        public int ItemCount { get { return LastItemIndex - FirstItemIndex + 1; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageListViewGroup"/>  class.
        /// </summary>
        /// <param name="name">The name of the group.</param>
        /// <param name="firstItemIndex">The index of the first item.</param>
        /// <param name="lastItemIndex">The index of the last item.</param>
        internal ImageListViewGroup(string name, int firstItemIndex, int lastItemIndex)
        {
            mImageListView = null;
            owner = null;
            Name = name;
            mCollapsed = false;
            FirstItemIndex = firstItemIndex;
            LastItemIndex = lastItemIndex;
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<ImageListViewItem> GetEnumerator()
        {
            for (int i = FirstItemIndex; i <= LastItemIndex; i++)
                yield return mImageListView.Items[i];
            yield break;
        }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This object is less than the <paramref name="other"/> parameter.
        /// Zero
        /// This object is equal to <paramref name="other"/>.
        /// Greater than zero
        /// This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(ImageListViewGroup? other) => string.Compare(Name, other?.Name);
        #endregion

        #region Helper Methods
        /// <summary>
        /// Determines which items in the group intersect with the given
        /// selection rectangle.
        /// </summary>
        /// <param name="rec">The selection rectangle.</param>
        /// <param name="orientation">Scroll orientation of the owner control.</param>
        /// <param name="itemSize">The size of one item including margins.</param>
        /// <returns>List of item indices.</returns>
        public List<int> ItemIndicesInRectangle(Rectangle rec, ScrollOrientation orientation, Size itemSize)
        {
            List<int> items = new();
            if (rec.Top <= itemBounds.Bottom && rec.Bottom >= itemBounds.Top &&
                rec.Left <= itemBounds.Right && rec.Right >= itemBounds.Left)
            {
                if (orientation == ScrollOrientation.HorizontalScroll)
                {
                    int startCol = (int)Math.Floor((float)(rec.Left - itemBounds.Left) / (float)itemSize.Width);
                    int endCol = (int)Math.Floor((float)(rec.Right - itemBounds.Left) / (float)itemSize.Width);

                    startCol = Math.Min(itemCols, Math.Max(0, startCol));
                    endCol = Math.Min(itemCols, Math.Max(0, endCol));

                    for (int i = FirstItemIndex + startCol; i <= FirstItemIndex + endCol; i++)
                    {
                        items.Add(i);
                    }
                } else if (orientation == ScrollOrientation.VerticalScroll)
                {
                    int startRow = (int)Math.Floor((float)(rec.Top - itemBounds.Top) / (float)itemSize.Height);
                    int endRow = (int)Math.Floor((float)(rec.Bottom - itemBounds.Top) / (float)itemSize.Height);
                    int startCol = (int)Math.Floor((float)(rec.Left - itemBounds.Left) / (float)itemSize.Width);
                    int endCol = (int)Math.Floor((float)(rec.Right - itemBounds.Left) / (float)itemSize.Width);

                    startRow = Math.Min(itemRows - 1, Math.Max(0, startRow));
                    endRow = Math.Min(itemRows - 1, Math.Max(0, endRow));
                    startCol = Math.Min(itemCols - 1, Math.Max(0, startCol));
                    endCol = Math.Min(itemCols - 1, Math.Max(0, endCol));

                    for (int row = startRow; row <= endRow; row++)
                    {
                        for (int col = startCol; col <= endCol; col++)
                        {
                            int i = FirstItemIndex + row * itemCols + col;
                            items.Add(i);
                        }
                    }
                }
            }
            return items;
        }
        #endregion
    }
}
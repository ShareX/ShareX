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

namespace ShareX.ImageListView;

public partial class ImageListView
{
    /// <summary>
    /// Represents the details of a mouse hit test.
    /// </summary>
    public class HitInfo
    {
        #region Properties
        /// <summary>
        /// Gets whether an item is under the hit point.
        /// </summary>
        public bool ItemHit { get { return ItemIndex != -1; } }
        /// <summary>
        /// Gets whether an item checkbox is under the hit point.
        /// </summary>
        public bool CheckBoxHit { get; private set; }
        /// <summary>
        /// Gets whether the file icon is under the hit point.
        /// </summary>
        public bool FileIconHit { get; private set; }
        /// <summary>
        /// Gets whether a column is under the hit point.
        /// </summary>
        public bool ColumnHit { get { return Column != null; } }
        /// <summary>
        /// Gets whether a column separator is under the hit point.
        /// </summary>
        public bool ColumnSeparatorHit { get { return ColumnSeparator != null; } }

        /// <summary>
        /// Gets the index of the item under the hit point.
        /// </summary>
        public int ItemIndex { get; private set; }
        /// <summary>
        /// Gets the index of the group header under the hit point.
        /// </summary>
        public ImageListViewGroup Group { get; private set; }
        /// <summary>
        /// Gets the index of the column under the hit point.
        /// </summary>
        public ImageListViewColumnHeader Column { get; private set; }
        /// <summary>
        /// Gets the index of the column separator under the hit point.
        /// </summary>
        public ImageListViewColumnHeader ColumnSeparator { get; private set; }
        /// <summary>
        /// Gets whether the hit point is over the pane border.
        /// </summary>
        public bool PaneBorder { get; private set; }

        /// <summary>
        /// Gets the index of the sub item under the hit point.
        /// The index returned is the 0-based index of the column
        /// as displayed on the screen, considering column visibility
        /// and display indices.
        /// Returns -1 if the hit point is not over a sub item.
        /// </summary>
        public int SubItemIndex { get; private set; }

        /// <summary>
        /// Gets whether the hit point is inside the item area.
        /// </summary>
        public bool InItemArea { get; private set; }
        /// <summary>
        /// Gets whether the hit point is inside the column header area.
        /// </summary>
        public bool InHeaderArea { get; private set; }
        /// <summary>
        /// Gets whether the hit point is inside the left-pane area.
        /// </summary>
        public bool InPaneArea { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the HitInfo class.
        /// </summary>
        /// <param name="itemIndex">Index of the item.</param>
        /// <param name="checkBoxHit">if set to true the mouse cursor is over a checkbox.</param>
        /// <param name="fileIconHit">if set to true the mouse cursor is over a file icon.</param>
        /// <param name="group">The group header hit.</param>
        /// <param name="column">The column header hit</param>
        /// <param name="columnSeparator">The column separator.</param>
        /// <param name="subItemIndex">Index of the sub item.</param>
        /// <param name="paneBorder">if set to true the mouse cursor is over the left-pane border.</param>
        /// <param name="inItemArea">if set to true the mouse is in the item area.</param>
        /// <param name="inHeaderArea">if set to true the mouse cursor is in the column header area.</param>
        /// <param name="inPaneArea">if set to true the mouse cursor is in the left-pane area.</param>
        private HitInfo(int itemIndex, bool checkBoxHit, bool fileIconHit, ImageListViewGroup group, ImageListViewColumnHeader column,
            ImageListViewColumnHeader columnSeparator, int subItemIndex,
            bool paneBorder, bool inItemArea, bool inHeaderArea, bool inPaneArea)
        {
            ItemIndex = itemIndex;
            CheckBoxHit = checkBoxHit;
            FileIconHit = fileIconHit;
            Group = group;
            Column = column;
            ColumnSeparator = columnSeparator;
            SubItemIndex = subItemIndex;

            InItemArea = inItemArea;
            InHeaderArea = inHeaderArea;

            InPaneArea = inPaneArea;
            PaneBorder = paneBorder;
        }
        /// <summary>
        /// Initializes a new instance of the HitInfo class.
        /// Used when the control registered an item hit.
        /// </summary>
        /// <param name="itemIndex">Index of the item.</param>
        /// <param name="subItemIndex">Index of the sub item.</param>
        /// <param name="checkBoxHit">if set to true the mouse cursor is over a checkbox.</param>
        /// <param name="fileIconHit">if set to true the mouse cursor is over a file icon.</param>
        internal HitInfo(int itemIndex, int subItemIndex, bool checkBoxHit, bool fileIconHit)
            : this(itemIndex, checkBoxHit, fileIconHit, null, null, null, subItemIndex, false, true, false, false)
        {
            ;
        }
        /// <summary>
        /// Initializes a new instance of the HitInfo class.
        /// Used when the control registered a column hit.
        /// </summary>
        /// <param name="group">The group header hit.</param>
        internal HitInfo(ImageListViewGroup group)
            : this(-1, false, false, group, null, null, -1, false, false, true, false)
        {
            ;
        }
        /// <summary>
        /// Initializes a new instance of the HitInfo class.
        /// Used when the control registered a column hit.
        /// </summary>
        /// <param name="column">Type column hit.</param>
        /// <param name="columnSeparator">The column separator.</param>
        internal HitInfo(ImageListViewColumnHeader column, ImageListViewColumnHeader columnSeparator)
            : this(-1, false, false, null, column, columnSeparator, -1, false, false, true, false)
        {
            ;
        }
        /// <summary>
        /// Initializes a new instance of the HitInfo class.
        /// Used when the control registered a hit in pane area.
        /// </summary>
        /// <param name="paneBorder">True if the hit point is over the left-pane 
        /// border, false otherwise.</param>
        internal HitInfo(bool paneBorder)
            : this(-1, false, false, null, null, null, -1, paneBorder, false, false, true)
        {
            ;
        }
        #endregion
    }
}
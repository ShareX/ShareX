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
using System.Resources;

namespace ShareX.ImageListView;

public partial class ImageListView
{
    /// <summary>
    /// Represents a column header displayed in details view mode.
    /// </summary>
    [TypeConverter(typeof(ImageListViewColumnHeaderTypeConverter))]
    public class ImageListViewColumnHeader : ICloneable
    {
        #region Member Variables
        private Guid mGuid;
        private string mKey;
        private int mDisplayIndex;
        internal ImageListView? mImageListView;
        private string mText;
        private ColumnType mType;
        private bool mVisible;
        private int mWidth;

        internal ImageListViewColumnHeaderCollection? owner;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the unique identifier for this item.
        /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets the unique identifier for this item.")]
        internal Guid Guid { get { return mGuid; } private set { mGuid = value; } }
        /// <summary>
        /// Gets a unique string to identify for this item.
        /// </summary>
        [Category("Behavior"), Browsable(true), Description("Gets a unique string to identify for this item.")]
        public string Key
        {
            get
            {
                return mKey;
            }
            set
            {
                mKey = value;
            }
        }
        /// <summary>
        /// Gets the default header text for this column type.
        /// </summary>
        [Category("Appearance"), Browsable(false), Description("Gets the default header text for this column type."), Localizable(true)]
        public virtual string DefaultText
        {
            get
            {
                return GetDefaultText(mType);
            }
        }
        /// <summary>
        /// Gets or sets the display order of the column.
        /// </summary>
        [Category("Appearance"), Browsable(true), Description("Gets or sets the display order of the column.")]
        public int DisplayIndex
        {
            get
            {
                return mDisplayIndex;
            }
            set
            {
                if (mDisplayIndex != value)
                {
                    mDisplayIndex = value;

                    if (owner != null)
                        owner.updateDisplayList = true;

                    if (mImageListView != null)
                        mImageListView.Refresh();
                }
            }
        }
        /// <summary>
        /// Gets the ImageListView owning this item.
        /// </summary>
        [Category("Behavior"), Browsable(false), Description("Gets the ImageListView owning this item.")]
        public ImageListView ImageListView => mImageListView;
        /// <summary>
        /// Gets or sets the column header text.
        /// </summary>
        [Category("Appearance"), Browsable(true), Description("Gets or sets the column header text."), Localizable(true)]
        public string Text
        {
            get
            {
                return !string.IsNullOrEmpty(mText) ? mText : DefaultText;
            }
            set
            {
                mText = value;
                if (mImageListView != null)
                    mImageListView.Refresh();
            }
        }
        /// <summary>
        /// Gets or sets the type of information displayed by the column.
        /// </summary>
        [Category("Appearance"), Browsable(true), Description("Gets or sets the type of information displayed by the column.")]
        public ColumnType Type
        {
            get
            {
                return mType;
            }
            set
            {
                mType = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the control is displayed.
        /// </summary>
        [Category("Appearance"), Browsable(true), Description("Gets or sets a value indicating whether the control is displayed."), DefaultValue(true)]
        public bool Visible
        {
            get
            {
                return mVisible;
            }
            set
            {
                mVisible = value;

                if (owner != null)
                    owner.updateDisplayList = true;

                if (mImageListView != null)
                    mImageListView.Refresh();
            }
        }
        /// <summary>
        /// Gets or sets the column width.
        /// </summary>
        [Category("Appearance"), Browsable(true), Description("Gets or sets the column width."), DefaultValue(ImageListView.DefaultColumnWidth), Localizable(true)]
        public int Width
        {
            get
            {
                return mWidth;
            }
            set
            {
                mWidth = System.Math.Max(12, value);
                if (mImageListView != null)
                    mImageListView.Refresh();
            }
        }
        /// <summary>
        /// Gets or sets the comparer used while sorting items with this custom column.
        /// </summary>
        [Browsable(false)]
        public IComparer<ImageListViewItem>? Comparer { get; set; }

        /// <summary>
        /// Gets or sets the grouper used while grouping items with this column.
        /// </summary>
        [Browsable(false)]
        public IGrouper? Grouper { get; set; }
        #endregion

        #region Custom Property Serializers
        /// <summary>
        /// Determines if the column text should be serialized.
        /// </summary>
        /// <returns>true if the designer should serialize 
        /// the property; otherwise false.</returns>
        public bool ShouldSerializeText()
        {
            return Text != DefaultText;
        }
        /// <summary>
        /// Resets the column text to its default value.
        /// </summary>
        public void ResetText()
        {
            Text = DefaultText;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ImageListViewColumnHeader class.
        /// </summary>
        /// <param name="type">The type of data to display in this column.</param>
        /// <param name="key">A unique key to associate this column with sub items.</param>
        /// <param name="text">Text of the column header.</param>
        /// <param name="width">Width in pixels of the column header.</param>
        /// <param name="displayIndex">Display order of the column.</param>
        /// <param name="visible">Whether the column is initially visible.</param>
        public ImageListViewColumnHeader(ColumnType type, string key, string text, int width = ImageListView.DefaultColumnWidth, int displayIndex = -1, bool visible = true)
        {
            mImageListView = null;
            owner = null;
            mGuid = Guid.NewGuid();
            Comparer = null;
            Grouper = null;

            mType = type;
            mKey = key;
            mText = text;
            mWidth = width;
            mDisplayIndex = displayIndex;
            mVisible = visible;
        }
        /// <summary>
        /// Initializes a new instance of the ImageListViewColumnHeader class.
        /// </summary>
        /// <param name="type">The type of data to display in this column.</param>
        /// <param name="text">Text of the column header.</param>
        /// <param name="width">Width in pixels of the column header.</param>
        /// <param name="displayIndex">Display order of the column.</param>
        /// <param name="visible">Whether the column is initially visible.</param>
        public ImageListViewColumnHeader(ColumnType type, string text, int width = ImageListView.DefaultColumnWidth, int displayIndex = -1, bool visible = true)
        : this(type, "", text, width, displayIndex, visible)
        {
        }
        /// <summary>
        /// Initializes a new instance of the ImageListViewColumnHeader class.
        /// </summary>
        /// <param name="type">The type of data to display in this column.</param>
        /// <param name="width">Width in pixels of the column header.</param>
        /// <param name="displayIndex">Display order of the column.</param>
        /// <param name="visible">Whether the column is initially visible.</param>
        public ImageListViewColumnHeader(ColumnType type, int width = ImageListView.DefaultColumnWidth, int displayIndex = -1, bool visible = true)
        : this(type, "", "", width, displayIndex, visible)
        {

        }
        /// <summary>
        /// Initializes a new instance of the ImageListViewColumnHeader class.
        /// </summary>
        public ImageListViewColumnHeader()
            : this(ColumnType.Name)
        {
            ;
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Resizes the width of the column based on the length of the column content.
        /// </summary>
        public void AutoFit()
        {
            if (mImageListView == null)
                throw new InvalidOperationException("Cannot calculate column width. Owner image list view is null.");

            int width = 0;
            if (mType == ColumnType.Rating)
            {
                if (mImageListView.RatingImage != null)
                    width = mImageListView.RatingImage.Width * 5;
            } else if (mType == ColumnType.Custom)
            {
                foreach (ImageListViewItem item in mImageListView.Items)
                {
                    int itemwidth = TextRenderer.MeasureText(item.SubItems[this].Text, mImageListView.Font).Width;
                    width = System.Math.Max(width, itemwidth);
                }
            } else
            {
                foreach (ImageListViewItem item in mImageListView.Items)
                {
                    int itemwidth = TextRenderer.MeasureText(item.GetSubItemText(Type), mImageListView.Font).Width;
                    width = System.Math.Max(width, itemwidth);
                }
            }

            // Add space for checkboxes and file icon
            if (mType == ColumnType.Name)
            {
                if (ImageListView.ShowCheckBoxes && ImageListView.ShowFileIcons)
                    width += 2 * 16 + 3 * 2;
                else if (ImageListView.ShowCheckBoxes)
                    width += 16 + 2 * 2;
                else if (ImageListView.ShowFileIcons)
                    width += 16 + 2 * 2;
            }

            this.Width = width + 8;
            mImageListView.Refresh();
        }
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return mType.ToString();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the default column header text for the given column type.
        /// </summary>
        [Localizable(true)]
        private string GetDefaultText(ColumnType type)
        {
            ResourceManager manager = new("Manina.Windows.Forms.ImageListViewResources", GetType().Assembly);
            return manager.GetString(type.ToString());
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            ImageListViewColumnHeader column = new();

            column.mDisplayIndex = mDisplayIndex;
            column.mText = mText;
            column.mType = mType;
            column.mVisible = mVisible;
            column.mWidth = mWidth;

            return column;
        }
        #endregion
    }
}

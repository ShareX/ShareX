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

/// <summary>
/// Represents a listview control for image files.
/// </summary>
[ToolboxBitmap(typeof(ImageListView))]
[Description("Represents a listview control for image files.")]
[DefaultEvent("ItemClick")]
[DefaultProperty("Items")]
[Designer(typeof(ImageListViewDesigner))]
[Docking(DockingBehavior.Ask)]
public partial class ImageListView : Control, IComponent
{
    #region Constants
    /// <summary>
    /// Default width of column headers in pixels.
    /// </summary>
    internal const int DefaultColumnWidth = 100;
    /// <summary>
    /// Selection tolerance for column separators.
    /// </summary>
    internal const int SeparatorSize = 12;
    /// <summary>
    /// Selection tolerance for left-pane border.
    /// </summary>
    internal const int PaneBorderSize = 4;
    #endregion

    #region Member Variables
    // Set when properties change
    private bool mDefaultImageChanged = false;
    private bool mErrorImageChanged = false;
    private bool mRatingImageChanged = false;
    private bool mEmptyRatingImageChanged = false;
    // Properties
    private BorderStyle mBorderStyle;
    private CacheMode mCacheMode;
    private int mCacheLimitAsItemCount;
    private long mCacheLimitAsMemory;
    private ImageListViewColor mColors;
    private ImageListViewColumnHeaderCollection mColumns;
    private Image mDefaultImage;
    private Image? mErrorImage;
    private Image mRatingImage;
    private Image mEmptyRatingImage;
    private Font mGroupHeaderFont;
    private Font mColumnHeaderFont;
    private bool mIntegralScroll;
    private ImageListViewItemCollection mItems;
    private int mPaneWidth;
    private bool mRetryOnError;
    internal ImageListViewSelectedItemCollection mSelectedItems;
    internal ImageListViewCheckedItemCollection mCheckedItems;
    private int mSortColumn;
    private int mGroupColumn;
    private SortOrder mSortOrder;
    private SortOrder mGroupOrder;
    private bool mShowFileIcons;
    private bool mShowCheckBoxes;
    private ContentAlignment mIconAlignment;
    private Size mIconPadding;
    private ContentAlignment mCheckBoxAlignment;
    private Size mCheckBoxPadding;
    private Size mThumbnailSize;
    private UseEmbeddedThumbnails mUseEmbeddedThumbnails;
    private View mView;
    private Point mViewOffset;
    private bool mShowScrollBars;

    // Groups
    internal ImageListViewGroupCollection groups;
    internal bool showGroups;

    // Renderer variables
    internal ImageListViewRenderer mRenderer;
    private bool controlSuspended;
    private int rendererSuspendCount;
    private bool rendererNeedsPaint;
    private System.Timers.Timer lazyRefreshTimer;
    private RefreshDelegateInternal lazyRefreshCallback;

    // Layout variables
    internal HScrollBar hScrollBar;
    internal VScrollBar vScrollBar;
    internal ImageListViewLayoutManager layoutManager;
    private bool disposed;

    // Interaction variables
    internal ImageListViewNavigationManager navigationManager;

    // Cache threads
    internal ImageListViewCacheThumbnail thumbnailCache;
    internal ImageListViewCacheShellInfo shellInfoCache;
    internal ImageListViewCacheMetadata metadataCache;
    internal FileSystemAdaptor defaultAdaptor;

    // Resource manager
    private ResourceManager resources;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets whether thumbnail images are automatically rotated.
    /// </summary>
    [Category("Behavior"), Description("Gets or sets whether thumbnail images are automatically rotated."), DefaultValue(true)]
    public bool AutoRotateThumbnails { get; set; }
    /// <summary>
    /// Gets or sets whether checkboxes respond to mouse clicks.
    /// </summary>
    [Category("Behavior"), Description("Gets or sets whether checkboxes respond to mouse clicks."), DefaultValue(true)]
    public bool AllowCheckBoxClick { get; set; }
    /// <summary>
    /// Gets or sets whether column headers respond to mouse clicks.
    /// </summary>
    [Category("Behavior"), Description("Gets or sets whether column headers respond to mouse clicks."), DefaultValue(true)]
    public bool AllowColumnClick { get; set; }
    /// <summary>
    /// Gets or sets whether column headers can be resized with the mouse.
    /// </summary>
    [Category("Behavior"), Description("Gets or sets whether column headers can be resized with the mouse."), DefaultValue(true)]
    public bool AllowColumnResize { get; set; }
    /// <summary>
    /// Gets or sets whether the user can reorder items by moving them.
    /// </summary>
    [Category("Behavior"), Description("Gets or sets whether the user can reorder items by moving them."), DefaultValue(true)]
    public bool AllowItemReorder { get; set; }
    /// <summary>
    /// Gets or sets whether items can be dragged out of the control to start a drag and drop operation.
    /// </summary>
    [Category("Behavior"), Description("Gets or sets whether items can be dragged out of the control to start a drag and drop operation."), DefaultValue(false)]
    public bool AllowDrag { get; set; }
    /// <summary>
    /// Gets or sets whether duplicate items (image files pointing to the same path 
    /// on the file system) are allowed.
    /// </summary>
    [Category("Behavior"), Description("Gets or sets whether duplicate items (image files pointing to the same path on the file system) are allowed."), DefaultValue(false)]
    public bool AllowDuplicateFileNames { get; set; }
    /// <summary>
    /// Gets or sets whether the left-pane can be resized with the mouse.
    /// </summary>
    [Category("Behavior"), Description("Gets or sets whether the left-pane can be resized with the mouse."), DefaultValue(true)]
    public bool AllowPaneResize { get; set; }
    /// <summary>
    /// Gets or sets the background color of the control.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the background color of the control."), DefaultValue(typeof(Color), "Window")]
    public override Color BackColor
    {
        get { return mColors.ControlBackColor; }
        set
        {
            mColors.ControlBackColor = value;
            Refresh();
        }
    }
    /// <summary>
    /// Gets or sets the border style of the control.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the border style of the control."), DefaultValue(typeof(BorderStyle), "Fixed3D")]
    public BorderStyle BorderStyle
    {
        get { return mBorderStyle; }
        set
        {
            mBorderStyle = value;
            UpdateStyles();
        }
    }
    /// <summary>
    /// Gets or sets the cache mode. Setting the the CacheMode to Continuous disables the CacheLimit.
    /// </summary>
    [Category("Behavior"), Description("Gets or sets the cache mode."), DefaultValue(typeof(CacheMode), "OnDemand"), RefreshProperties(RefreshProperties.All)]
    public CacheMode CacheMode
    {
        get { return mCacheMode; }
        set
        {
            if (mCacheMode != value)
            {
                mCacheMode = value;

                if (thumbnailCache != null)
                    thumbnailCache.CacheMode = mCacheMode;

                if (mCacheMode == CacheMode.Continuous)
                {
                    mCacheLimitAsItemCount = 0;
                    mCacheLimitAsMemory = 0;
                    if (thumbnailCache != null)
                    {
                        thumbnailCache.CacheLimitAsItemCount = 0;
                        thumbnailCache.CacheLimitAsMemory = 0;
                        thumbnailCache.diskCache.Size = 0;
                    }
                    // Rebuild the cache
                    ClearThumbnailCache();
                }
            }
        }
    }
    /// <summary>
    /// Gets or sets the cache limit as either the count of thumbnail images or the memory allocated for cache (e.g. 10MB).
    /// </summary>
    [Category("Behavior"), Description("Gets or sets the cache limit as either the count of thumbnail images or the memory allocated for cache (e.g. 10MB)."), DefaultValue("20MB"), RefreshProperties(RefreshProperties.All)]
    public string CacheLimit
    {
        get
        {
            return mCacheLimitAsMemory != 0 ? (mCacheLimitAsMemory / 1024 / 1024).ToString() + "MB" : mCacheLimitAsItemCount.ToString();
        }
        set
        {
            string slimit = value;
            mCacheMode = CacheMode.OnDemand;
            if ((slimit.EndsWith("MB", StringComparison.OrdinalIgnoreCase) && int.TryParse(slimit.Substring(0, slimit.Length - 2).Trim(), out int limit)) || (slimit.EndsWith("MiB", StringComparison.OrdinalIgnoreCase) && int.TryParse(slimit.Substring(0, slimit.Length - 3).Trim(), out limit)))
            {
                mCacheLimitAsItemCount = 0;
                mCacheLimitAsMemory = limit * 1024 * 1024;
                if (thumbnailCache != null)
                {
                    thumbnailCache.CacheLimitAsItemCount = 0;
                    thumbnailCache.CacheLimitAsMemory = mCacheLimitAsMemory;
                }
            } else if (int.TryParse(slimit, out limit))
            {
                mCacheLimitAsMemory = 0;
                mCacheLimitAsItemCount = limit;
                if (thumbnailCache != null)
                {
                    thumbnailCache.CacheLimitAsMemory = 0;
                    thumbnailCache.CacheLimitAsItemCount = mCacheLimitAsItemCount;
                }
            } else
                throw new ArgumentException("Cache limit must be specified as either the count of thumbnail images or the memory allocated for cache (eg 10MB)", "value");
        }
    }
    /// <summary>
    /// Gets or sets the path to the persistent cache file.
    /// </summary>
    [Category("Behavior"), Description("Gets or sets the path to the persistent cache file."), Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PersistentCacheDirectory
    {
        get
        {
            return thumbnailCache.diskCache.DirectoryName;
        }
        set
        {
            thumbnailCache.diskCache.DirectoryName = value;
        }
    }
    /// <summary>
    /// Gets or sets the size of the persistent cache file in MB.
    /// </summary>
    [Category("Behavior"), Description("Gets or sets the size of the persistent cache file in MB."), Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public long PersistentCacheSize
    {
        get
        {
            return mCacheMode == CacheMode.Continuous ? 0 : thumbnailCache != null ? thumbnailCache.diskCache.Size / 1024 / 1024 : 0;
        }
        set
        {
            if (thumbnailCache != null)
                thumbnailCache.diskCache.Size = value * 1024 * 1024;
        }
    }
    /// <summary>
    /// Gets or sets the color palette of the ImageListView.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the color palette of the ImageListView.")]
    public ImageListViewColor Colors
    {
        get
        {
            mColors ??= ImageListViewColor.Default;

            return mColors;
        }
        set
        {
            mColors = value;
            Refresh();
        }
    }

    /// <summary>
    /// Gets or sets the collection of columns of the image list view.
    /// </summary>
    [Category("Appearance"), Description("Gets the collection of columns of the image list view.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ImageListViewColumnHeaderCollection Columns
    {
        get { return mColumns; }
        internal set
        {
            mColumns = value;
            Refresh();
        }
    }
    /// <summary>
    /// Gets or sets the placeholder image.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the placeholder image.")]
    public Image DefaultImage
    {
        get
        {
            return mDefaultImage == null ? resources.GetObject("DefaultImage") as Image : mDefaultImage;
        }
        set
        {
            mDefaultImageChanged = true;
            mDefaultImage = value;
            Refresh();
        }
    }
    /// <summary>
    /// Gets the rectangle that represents the display area of the control.
    /// </summary>
    [Category("Appearance"), Browsable(false), Description("Gets the rectangle that represents the display area of the control.")]
    public override Rectangle DisplayRectangle
    {
        get
        {
            return layoutManager == null ? base.DisplayRectangle : layoutManager.ClientArea;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether the control can respond to user interaction.
    /// Cache threads are paused while the control is disabled and resumed when the control is
    /// enabled.
    /// </summary>
    [Category("Behavior"), Browsable(true), Description("Gets or sets a value indicating whether the control can respond to user interaction."), DefaultValue(true)]
    public new bool Enabled
    {
        get { return base.Enabled; }
        set
        {
            base.Enabled = value;
            if (value)
            {
                thumbnailCache.Resume();
                shellInfoCache.Resume();
                metadataCache.Resume();
            } else
            {
                thumbnailCache.Pause();
                shellInfoCache.Pause();
                metadataCache.Pause();
            }
        }
    }
    /// <summary>
    /// Gets or sets the error image.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the error image.")]
    public Image? ErrorImage
    {
        get => mErrorImage ?? resources.GetObject("ErrorImage") as Image;
        set
        {
            mErrorImageChanged = true;
            mErrorImage = value == null ? null : value;
            Refresh();
        }
    }
    /// <summary>
    /// Gets or sets the font of the group headers.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the font of the group headers."), DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt, style=Bold")]
    public Font GroupHeaderFont
    {
        get
        {
            if (mGroupHeaderFont == null)
            {
                mGroupHeaderFont = Font != null ? (Font)Font.Clone() : (Font)Control.DefaultFont.Clone();
            }

            return mGroupHeaderFont;
        }
        set
        {
            mGroupHeaderFont = value;
            Refresh();
        }
    }
    /// <summary>
    /// Gets or sets the font of the column headers.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the font of the column headers."), DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
    public Font ColumnHeaderFont
    {
        get
        {
            if (mColumnHeaderFont == null)
            {
                mColumnHeaderFont = Font != null ? (Font)Font.Clone() : (Font)Control.DefaultFont.Clone();
            }

            return mColumnHeaderFont;
        }
        set
        {
            mColumnHeaderFont = value;
            Refresh();
        }
    }
    /// <summary>
    /// Gets or sets whether scrollbars scroll by an amount which is a multiple of item height.
    /// </summary>
    [Browsable(true), Category("Behavior"), Description("Gets or sets whether scrollbars scroll by an amount which is a multiple of item height."), DefaultValue(false)]
    public bool IntegralScroll
    {
        get { return mIntegralScroll; }
        set
        {
            if (mIntegralScroll != value)
            {
                mIntegralScroll = value;
                Refresh();
            }
        }
    }
    /// <summary>
    /// Gets the collection of items contained in the image list view.
    /// </summary>
    [Category("Behavior"), Description("Gets the collection of items contained in the image list view.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ImageListViewItemCollection Items
    {
        get { return mItems; }
        internal set
        {
            mItems = value;
            Refresh();
        }
    }
    /// <summary>
    /// Gets or sets whether multiple items can be selected.
    /// </summary>
    [Category("Behavior"), Description("Gets or sets whether multiple items can be selected."), DefaultValue(true)]
    public bool MultiSelect { get; set; }
    /// <summary>
    /// Gets or sets the width of the left pane.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the width of the left pane."), DefaultValue(240)]
    public int PaneWidth
    {
        get { return mPaneWidth; }
        set
        {
            if (mPaneWidth != value)
            {
                if (mPaneWidth < 2)
                    mPaneWidth = 2;
                mPaneWidth = value;
                Refresh();
            }
        }
    }
    /// <summary>
    /// Gets or sets the rating image.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the rating image.")]
    public Image RatingImage
    {
        get
        {
            return mRatingImage == null ? resources.GetObject("RatingImage") as Image : mRatingImage;
        }
        set
        {
            mRatingImageChanged = true;
            mRatingImage = value;
            Refresh();
        }
    }
    /// <summary>
    /// Gets or sets the empty rating image.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the empty rating image.")]
    public Image EmptyRatingImage
    {
        get
        {
            return mEmptyRatingImage == null ? resources.GetObject("EmptyRatingImage") as Image : mEmptyRatingImage;
        }
        set
        {
            mEmptyRatingImageChanged = true;
            mEmptyRatingImage = value;
            Refresh();
        }
    }
    /// <summary>
    /// Gets or sets whether the control will retry loading thumbnails on an error.
    /// </summary>
    [Category("Behavior"), Description("Gets or sets whether the control will retry loading thumbnails on an error."), DefaultValue(true)]
    public bool RetryOnError
    {
        get { return mRetryOnError; }
        set
        {
            mRetryOnError = value;
            if (thumbnailCache != null)
                thumbnailCache.RetryOnError = mRetryOnError;
            if (shellInfoCache != null)
                shellInfoCache.RetryOnError = mRetryOnError;
            if (metadataCache != null)
                metadataCache.RetryOnError = mRetryOnError;
        }
    }
    /// <summary>
    /// Gets or sets whether the scrollbars should be shown.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets whether the scrollbars should be shown."), DefaultValue(true)]
    public bool ScrollBars
    {
        get { return mShowScrollBars; }
        set
        {
            mShowScrollBars = value;
            Refresh();
        }
    }
    /// <summary>
    /// Gets the collection of selected items contained in the image list view.
    /// </summary>
    [Browsable(false), Category("Behavior"), Description("Gets the collection of selected items contained in the image list view.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ImageListViewSelectedItemCollection SelectedItems
    {
        get { return mSelectedItems; }
    }
    /// <summary>
    /// Gets the collection of checked items contained in the image list view.
    /// </summary>
    [Browsable(false), Category("Behavior"), Description("Gets the collection of checked items contained in the image list view.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ImageListViewCheckedItemCollection CheckedItems
    {
        get { return mCheckedItems; }
    }
    /// <summary>
    /// Gets or sets whether shell icons are displayed for non-image files.
    /// </summary>
    [Browsable(false), Category("Behavior"), Description("Gets or sets whether shell icons are displayed for non-image files."), DefaultValue(true)]
    public bool ShellIconFallback { get; set; }
    /// <summary>
    /// Gets or sets whether shell icons are extracted from the contents of icon and executable files.
    /// When set to false, the generic shell icon for the filename extension is extracted.
    /// </summary>
    [Browsable(false), Category("Behavior"), Description("Gets or sets whether shell icons are extracted from the contents of icon and executable files."), DefaultValue(true)]
    public bool ShellIconFromFileContent { get; set; }
    /// <summary>
    /// Gets or sets whether to display the file icons.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets whether to display the file icons."), DefaultValue(false)]
    public bool ShowFileIcons
    {
        get { return mShowFileIcons; }
        set
        {
            mShowFileIcons = value;
            Refresh();
        }
    }
    /// <summary>
    /// Gets or sets whether to display the item checkboxes.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets whether to display the item checkboxes."), DefaultValue(false)]
    public bool ShowCheckBoxes
    {
        get { return mShowCheckBoxes; }
        set
        {
            mShowCheckBoxes = value;
            Refresh();
        }
    }
    /// <summary>
    /// Gets or sets alignment of file icons.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets alignment of file icons."), DefaultValue(ContentAlignment.TopRight)]
    public ContentAlignment IconAlignment
    {
        get { return mIconAlignment; }
        set
        {
            mIconAlignment = value;
            Refresh();
        }
    }
    /// <summary>
    /// Gets or sets file icon padding.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets file icon padding."), DefaultValue(typeof(Size), "2,2")]
    public Size IconPadding
    {
        get { return mIconPadding; }
        set
        {
            mIconPadding = value;
            Refresh();
        }
    }
    /// <summary>
    /// Gets or sets alignment of item checkboxes.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets alignment of item checkboxes."), DefaultValue(ContentAlignment.BottomRight)]
    public ContentAlignment CheckBoxAlignment
    {
        get { return mCheckBoxAlignment; }
        set
        {
            mCheckBoxAlignment = value;
            Refresh();
        }
    }
    /// <summary>
    /// Gets or sets item checkbox padding.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets item checkbox padding."), DefaultValue(typeof(Size), "2,2")]
    public Size CheckBoxPadding
    {
        get { return mCheckBoxPadding; }
        set
        {
            mCheckBoxPadding = value;
            Refresh();
        }
    }
    /// <summary>
    /// Gets or sets the index of the sort column.
    /// </summary>
    [Category("Appearance"), DefaultValue(0), Description("Gets or sets the index of the sort column.")]
    public int SortColumn
    {
        get { return mSortColumn; }
        set
        {
            if (value != mSortColumn)
            {
                mSortColumn = value;
                Sort();
            }
        }
    }
    /// <summary>
    /// Gets or sets the sort order.
    /// </summary>
    [Category("Appearance"), DefaultValue(typeof(SortOrder), "None"), Description("Gets or sets the sort order.")]
    public SortOrder SortOrder
    {
        get { return mSortOrder; }
        set
        {
            if (value != mSortOrder)
            {
                mSortOrder = value;
                Sort();
            }
        }
    }
    /// <summary>
    /// Gets or sets the index of the group column.
    /// </summary>
    [Category("Appearance"), DefaultValue(0), Description("Gets or sets the index of the group column.")]
    public int GroupColumn
    {
        get { return mGroupColumn; }
        set
        {
            if (value != mGroupColumn)
            {
                mGroupColumn = value;
                Sort();
            }
        }
    }
    /// <summary>
    /// Gets or sets the group order.
    /// </summary>
    [Category("Appearance"), DefaultValue(typeof(SortOrder), "None"), Description("Gets or sets the group order.")]
    public SortOrder GroupOrder
    {
        get { return mGroupOrder; }
        set
        {
            if (value != mGroupOrder)
            {
                mGroupOrder = value;
                Sort();
            }
        }
    }
    /// <summary>
    /// This property is not relevant for this class.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Bindable(false), DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override string Text { get; set; }
    /// <summary>
    /// Gets or sets the size of image thumbnails.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the size of image thumbnails."), DefaultValue(typeof(Size), "96,96")]
    public Size ThumbnailSize
    {
        get => mThumbnailSize;
        set
        {
            if (mThumbnailSize != value)
            {
                mThumbnailSize = value;
                thumbnailCache.Rebuild();
                Refresh();
            }
        }
    }
    /// <summary>
    /// Gets or sets the embedded thumbnails extraction behavior.
    /// </summary>
    [Category("Behavior"), Description("Gets or sets the embedded thumbnails extraction behavior."), DefaultValue(typeof(UseEmbeddedThumbnails), "Auto")]
    public UseEmbeddedThumbnails UseEmbeddedThumbnails
    {
        get => mUseEmbeddedThumbnails;
        set
        {
            if (mUseEmbeddedThumbnails != value)
            {
                mUseEmbeddedThumbnails = value;
                Refresh();
            }
        }
    }
    /// <summary>
    /// Gets or sets the view mode of the image list view.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the view mode of the image list view."), DefaultValue(typeof(View), "Thumbnails")]
    public View View
    {
        get => mView;
        set
        {
            if (mView != value)
            {
                int current = layoutManager.FirstPartiallyVisible;
                mView = value;
                Refresh();
                EnsureVisible(current);
            }
        }
    }
    /// <summary>
    /// Gets whether groups are displayed.
    /// </summary>
    [Browsable(false), Category("Appearance"), Description("Gets whether groups are displayed.")]
    public bool GroupsVisible => showGroups;
    /// <summary>
    /// Gets or sets the scroll offset.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    /// <summary>
    /// Gets or sets the scroll offset.
    /// </summary>
    internal Point ViewOffset
    {
        get => mViewOffset;
        set => mViewOffset = value;
    }
    /// <summary>
    /// Gets the scroll orientation.
    /// </summary>
    internal ScrollOrientation ScrollOrientation
        => (mView == View.Gallery || mView == View.HorizontalStrip ? ScrollOrientation.HorizontalScroll : ScrollOrientation.VerticalScroll);
    /// <summary>
    /// Gets or sets whether the control uses WPF/WIC for thumbnail extraction.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets whether the control uses for thumbnail extraction.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseWIC { get { return Extractor.UseWIC; } set { Extractor.UseWIC = value; } }

    /// <summary>
    /// Gets the thumbnail extractor.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
    public IExtractor ThumbnailExtractor { get { return Extractor.Instance; } }
    #endregion

    #region Custom Property Serializers
    /// <summary>
    /// Determines if the header font should be serialized.
    /// </summary>
    /// <returns>true if the designer should serialize 
    /// the property; otherwise false.</returns>
    public bool ShouldSerializeHeaderFont()
        => !mColumnHeaderFont.Equals(new Font("Microsoft Sans Serif", 8.25f));
    /// <summary>
    /// Resets the header font to its default value.
    /// </summary>
    public void ResetHeaderFont()
        => ColumnHeaderFont = new Font("Microsoft Sans Serif", 8.25f);

    /// <summary>
    /// Determines if the colors should be serialized.
    /// </summary>
    /// <returns>true if the designer should serialize 
    /// the property; otherwise false.</returns>
    public bool ShouldSerializeColors()
    {
        ImageListViewColor defaultColors = ImageListViewColor.Default;
        return !mColors.Equals(defaultColors);
    }
    /// <summary>
    /// Resets the colors to their default value.
    /// </summary>
    public void ResetColors()
    {
        Colors = ImageListViewColor.Default;
    }

    /// <summary>
    /// Determines if the default image should be serialized.
    /// </summary>
    /// <returns>true if the designer should serialize 
    /// the property; otherwise false.</returns>
    public bool ShouldSerializeDefaultImage()
    {
        return mDefaultImageChanged;
    }
    /// <summary>
    /// Resets the default image to its default value.
    /// </summary>
    public void ResetDefaultImage()
    {
        DefaultImage = resources.GetObject("DefaultImage") as Image;
        mDefaultImageChanged = false;
    }

    /// <summary>
    /// Determines if the error image should be serialized.
    /// </summary>
    /// <returns>true if the designer should serialize 
    /// the property; otherwise false.</returns>
    public bool ShouldSerializeErrorImage()
    {
        return mErrorImageChanged;
    }
    /// <summary>
    /// Resets the error image to its default value.
    /// </summary>
    public void ResetErrorImage()
    {
        ErrorImage = resources.GetObject("ErrorImage") as Image;
        mErrorImageChanged = false;
    }

    /// <summary>
    /// Determines if the rating image should be serialized.
    /// </summary>
    /// <returns>true if the designer should serialize 
    /// the property; otherwise false.</returns>
    public bool ShouldSerializeRatingImage()
    {
        return mRatingImageChanged;
    }
    /// <summary>
    /// Resets the rating image to its default value.
    /// </summary>
    public void ResetRatingImage()
    {
        RatingImage = resources.GetObject("RatingImage") as Image;
        mRatingImageChanged = false;
    }

    /// <summary>
    /// Determines if the empty rating image should be serialized.
    /// </summary>
    /// <returns>true if the designer should serialize 
    /// the property; otherwise false.</returns>
    public bool ShouldSerializeEmptyRatingImage()
    {
        return mEmptyRatingImageChanged;
    }
    /// <summary>
    /// Resets the empty rating image to its default value.
    /// </summary>
    public void ResetEmptyRatingImage()
    {
        EmptyRatingImage = resources.GetObject("EmptyRatingImage") as Image;
        mEmptyRatingImageChanged = false;
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the ImageListView class.
    /// </summary>
    public ImageListView()
    {
        // Renderer parameters
        controlSuspended = false;
        rendererSuspendCount = 0;
        rendererNeedsPaint = true;

        mColors = ImageListViewColor.Default;
        SetRenderer(new ImageListViewRenderer());

        // Property defaults
        AutoRotateThumbnails = true;
        AllowCheckBoxClick = true;
        AllowColumnClick = true;
        AllowColumnResize = true;
        AllowDrag = false;
        AllowItemReorder = true;
        AllowDuplicateFileNames = false;
        AllowPaneResize = true;
        mBorderStyle = BorderStyle.Fixed3D;
        mCacheMode = CacheMode.OnDemand;
        mCacheLimitAsItemCount = 0;
        mCacheLimitAsMemory = 20 * 1024 * 1024;
        mColumns = new ImageListViewColumnHeaderCollection(this);
        resources = new ResourceManager("Manina.Windows.Forms.ImageListViewResources", typeof(ImageListView).Assembly);
        mDefaultImage = resources.GetObject("DefaultImage") as Image;
        mErrorImage = resources.GetObject("ErrorImage") as Image;
        mRatingImage = resources.GetObject("RatingImage") as Image;
        mEmptyRatingImage = resources.GetObject("EmptyRatingImage") as Image;
        GroupHeaderFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
        ColumnHeaderFont = new Font("Microsoft Sans Serif", 8.25f);
        mIntegralScroll = false;
        mItems = new ImageListViewItemCollection(this);
        MultiSelect = true;
        mPaneWidth = 240;
        mRetryOnError = true;
        mSelectedItems = new ImageListViewSelectedItemCollection(this);
        mCheckedItems = new ImageListViewCheckedItemCollection(this);
        mSortColumn = 0;
        mGroupColumn = 0;
        mSortOrder = SortOrder.None;
        mGroupOrder = SortOrder.None;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.Opaque | ControlStyles.Selectable | ControlStyles.UserMouse, true);
        ScrollBars = true;
        ShellIconFallback = true;
        ShellIconFromFileContent = true;
        Size = new Size(120, 100);
        mShowCheckBoxes = false;
        mCheckBoxAlignment = ContentAlignment.BottomRight;
        mCheckBoxPadding = new Size(2, 2);
        mShowFileIcons = false;
        mIconAlignment = ContentAlignment.TopRight;
        mIconPadding = new Size(2, 2);
        Text = string.Empty;
        mThumbnailSize = new Size(96, 96);
        mUseEmbeddedThumbnails = UseEmbeddedThumbnails.Auto;
        mView = View.Thumbnails;
        mViewOffset = new Point(0, 0);
        mShowScrollBars = true;

        // Child controls
        hScrollBar = new HScrollBar();
        vScrollBar = new VScrollBar();
        hScrollBar.Visible = false;
        vScrollBar.Visible = false;
        hScrollBar.Scroll += hScrollBar_Scroll;
        vScrollBar.Scroll += vScrollBar_Scroll;
        Controls.Add(hScrollBar);
        Controls.Add(vScrollBar);

        // Groups
        groups = new ImageListViewGroupCollection(this);
        showGroups = false;

        // Lazy refresh timer
        lazyRefreshTimer = new System.Timers.Timer();
        lazyRefreshTimer.Interval = ImageListViewRenderer.LazyRefreshInterval;
        lazyRefreshTimer.Enabled = false;
        lazyRefreshTimer.Elapsed += lazyRefreshTimer_Tick;
        lazyRefreshCallback = new RefreshDelegateInternal(Refresh);

        // Helpers
        layoutManager = new ImageListViewLayoutManager(this);
        navigationManager = new ImageListViewNavigationManager(this);

        // Cache nabagers
        defaultAdaptor = new FileSystemAdaptor();
        thumbnailCache = new ImageListViewCacheThumbnail(this);
        shellInfoCache = new ImageListViewCacheShellInfo(this);
        metadataCache = new ImageListViewCacheMetadata(this);

        disposed = false;
    }
    #endregion

    #region Select/Check
    /// <summary>
    /// Marks all items as selected.
    /// </summary>
    public void SelectAll()
    {
        SuspendPaint();

        foreach (ImageListViewItem item in Items)
            item.mSelected = true;

        OnSelectionChangedInternal();

        Refresh();
        ResumePaint();
    }
    /// <summary>
    /// Marks all items as unselected.
    /// </summary>
    public void ClearSelection()
    {
        SuspendPaint();
        mSelectedItems.Clear();
        Refresh();
        ResumePaint();
    }
    /// <summary>
    /// Reverses the selection state of all items.
    /// </summary>
    public void InvertSelection()
    {
        SuspendPaint();

        foreach (ImageListViewItem item in Items)
            item.mSelected = !item.mSelected;

        OnSelectionChangedInternal();

        Refresh();
        ResumePaint();
    }
    /// <summary>
    /// Marks all items that satisfy a condition as selected.
    /// </summary>
    public void SelectWhere(Func<ImageListViewItem, bool> predicate)
    {
        SuspendPaint();

        foreach (ImageListViewItem item in Items.Where(predicate))
            item.mSelected = true;

        OnSelectionChangedInternal();

        Refresh();
        ResumePaint();
    }
    /// <summary>
    /// Marks all items that satisfy a condition as unselected.
    /// </summary>
    public void UnselectWhere(Func<ImageListViewItem, bool> predicate)
    {
        SuspendPaint();

        foreach (ImageListViewItem item in Items.Where(predicate))
            item.mSelected = false;

        OnSelectionChangedInternal();

        Refresh();
        ResumePaint();
    }
    /// <summary>
    /// Marks all items as checked.
    /// </summary>
    public void CheckAll()
    {
        SuspendPaint();

        foreach (ImageListViewItem item in Items)
        {
            item.mChecked = true;
            OnItemCheckBoxClickInternal(item);
        }

        Refresh();
        ResumePaint();
    }
    /// <summary>
    /// Marks all items as unchecked.
    /// </summary>
    public void UncheckAll()
    {
        SuspendPaint();
        mCheckedItems.Clear();
        Refresh();
        ResumePaint();
    }
    /// <summary>
    /// Reverses the check state of all items.
    /// </summary>
    public void InvertCheckState()
    {
        SuspendPaint();

        foreach (ImageListViewItem item in Items)
        {
            item.mChecked = !item.mChecked;
            OnItemCheckBoxClickInternal(item);
        }

        Refresh();
        ResumePaint();
    }
    /// <summary>
    /// Marks all items that satisfy a condition as checked.
    /// </summary>
    public void CheckWhere(Func<ImageListViewItem, bool> predicate)
    {
        SuspendPaint();

        foreach (ImageListViewItem item in Items.Where(predicate))
        {
            item.mChecked = true;
            OnItemCheckBoxClickInternal(item);
        }

        Refresh();
        ResumePaint();
    }
    /// <summary>
    /// Marks all items that satisfy a condition as unchecked.
    /// </summary>
    public void UncheckWhere(Func<ImageListViewItem, bool> predicate)
    {
        SuspendPaint();

        foreach (ImageListViewItem item in Items.Where(predicate))
        {
            item.mChecked = false;
            OnItemCheckBoxClickInternal(item);
        }

        Refresh();
        ResumePaint();
    }
    /// <summary>
    /// Marks all items as enabled.
    /// </summary>
    public void EnableAll()
    {
        SuspendPaint();

        foreach (ImageListViewItem item in Items)
            item.mEnabled = true;

        Refresh();
        ResumePaint();
    }
    /// <summary>
    /// Marks all items as disabled.
    /// </summary>
    public void DisableAll()
    {
        SuspendPaint();

        foreach (ImageListViewItem item in Items)
            item.mEnabled = false;

        Refresh();
        ResumePaint();
    }
    /// <summary>
    /// Marks all items that satisfy a condition as enabled.
    /// </summary>
    public void EnableWhere(Func<ImageListViewItem, bool> predicate)
    {
        SuspendPaint();

        foreach (ImageListViewItem item in Items.Where(predicate))
            item.mEnabled = true;

        Refresh();
        ResumePaint();
    }
    /// <summary>
    /// Marks all items that satisfy a condition as disabled.
    /// </summary>
    public void DisableWhere(Func<ImageListViewItem, bool> predicate)
    {
        SuspendPaint();

        foreach (ImageListViewItem item in Items.Where(predicate))
            item.mEnabled = false;

        Refresh();
        ResumePaint();
    }
    #endregion

    #region Instance Methods
    /// <summary>
    /// Clears the thumbnail cache.
    /// </summary>
    public void ClearThumbnailCache()
    {
        if (thumbnailCache != null)
        {
            thumbnailCache.Clear();
            if (CacheMode == CacheMode.Continuous)
            {
                foreach (ImageListViewItem item in mItems)
                {
                    thumbnailCache.Add(item.Guid, item.Adaptor, item.VirtualItemKey, mThumbnailSize, mUseEmbeddedThumbnails, AutoRotateThumbnails);
                }
            }
            Refresh();
        }
    }
    /// <summary>
    /// Temporarily suspends the layout logic for the control.
    /// </summary>
    public new void SuspendLayout()
    {
        if (controlSuspended)
            return;

        controlSuspended = true;
        base.SuspendLayout();
        SuspendPaint();
    }
    /// <summary>
    /// Resumes usual layout logic.
    /// </summary>
    public new void ResumeLayout()
    {
        ResumeLayout(false);
    }
    /// <summary>
    /// Resumes usual layout logic, optionally forcing an immediate layout of pending layout requests.
    /// </summary>
    /// <param name="performLayout">true to execute pending layout requests; otherwise, false.</param>
    public new void ResumeLayout(bool performLayout)
    {
        if (!controlSuspended)
            return;

        controlSuspended = false;
        base.ResumeLayout(performLayout);
        if (performLayout)
            Refresh();
        ResumePaint();
    }
    /// <summary>
    /// Sets the renderer for this instance.
    /// </summary>
    /// <param name="renderer">An <see cref="ImageListViewRenderer"/> to assign to the control.</param>
    /// <param name="keepColors">true to keep current colors; otherwise false.</param>
    public void SetRenderer(ImageListViewRenderer renderer, bool keepColors)
    {
        if (renderer == null)
            throw new ArgumentNullException("renderer");

        ImageListViewRenderer oldRenderer = mRenderer;

        mRenderer = renderer;
        mRenderer.ImageListView = this;
        if (!keepColors)
        {
            ImageListViewColor[] preferredColors = mRenderer.PreferredColors;
            mColors = preferredColors != null ? preferredColors[0] : ImageListViewColor.Default;
        }

        if (oldRenderer != null)
            oldRenderer.Dispose();

        if (layoutManager != null)
            layoutManager.Update(true);

        Refresh();
    }
    /// <summary>
    /// Sets the renderer for this instance.
    /// </summary>
    /// <param name="renderer">An <see cref="ImageListViewRenderer"/> to assign to the control.</param>
    public void SetRenderer(ImageListViewRenderer renderer)
    {
        SetRenderer(renderer, false);
    }
    /// <summary>
    /// Sorts the items.
    /// </summary>
    public void Sort()
    {
        mItems.Sort();
        Refresh();
    }
    /// <summary>
    /// Determines the image list view element under the specified coordinates.
    /// </summary>
    /// <param name="pt">The client coordinates of the point to be tested.</param>
    /// <param name="hitInfo">Details of the hit test.</param>
    public void HitTest(Point pt, out HitInfo hitInfo)
    {
        if (View == View.Details && pt.Y <= mRenderer.MeasureColumnHeaderHeight())
        {
            int i = 0;
            int x = layoutManager.ColumnHeaderBounds.Left;
            ImageListViewColumnHeader colIndex = null;
            ImageListViewColumnHeader sepIndex = null;
            if (AllowColumnClick || AllowColumnResize)
            {
                foreach (ImageListViewColumnHeader col in Columns.GetDisplayedColumns())
                {
                    // Over a column?
                    if (pt.X >= x && pt.X < x + col.Width + SeparatorSize / 2)
                        colIndex = col;

                    // Over a colummn separator?
                    if (pt.X > x + col.Width - SeparatorSize / 2 && pt.X < x + col.Width + SeparatorSize / 2)
                        sepIndex = col;

                    if (colIndex != null)
                        break;
                    x += col.Width;
                    i++;
                }
            }
            hitInfo = new HitInfo(colIndex, sepIndex);
        } else if (View == View.Pane && pt.X <= mPaneWidth)
        {
            bool overBorder = (pt.X >= mPaneWidth - PaneBorderSize);
            hitInfo = new HitInfo(overBorder);
        } else
        {
            if (showGroups)
            {
                foreach (ImageListViewGroup @group in groups)
                {
                    if (@group.headerBounds.Contains(pt))
                    {
                        hitInfo = new HitInfo(@group);
                        return;
                    }
                }
            }

            int itemIndex = -1;
            bool checkBoxHit = false;
            bool fileIconHit = false;
            int subItemIndex = -1;

            if (showGroups)
            {
                foreach (ImageListViewGroup @group in groups)
                {
                    if (@group.itemBounds.Contains(pt))
                    {
                        // Normalize to group item area coordinates
                        pt.X -= @group.itemBounds.Left;
                        pt.Y -= @group.itemBounds.Top;

                        if (pt.X > 0 && pt.Y > 0)
                        {
                            int col = pt.X / layoutManager.ItemSizeWithMargin.Width;
                            int row = pt.Y / layoutManager.ItemSizeWithMargin.Height;

                            int index = @group.FirstItemIndex + row * @group.itemCols + col;
                            if (index >= 0 && index <= Items.Count - 1)
                            {
                                Rectangle bounds = layoutManager.GetItemBounds(index);
                                if (bounds.Contains(pt.X + @group.itemBounds.Left, pt.Y + @group.itemBounds.Top))
                                    itemIndex = index;
                                if (ShowCheckBoxes)
                                {
                                    Rectangle checkBoxBounds = layoutManager.GetCheckBoxBounds(index);
                                    if (checkBoxBounds.Contains(pt.X + @group.itemBounds.Left, pt.Y + @group.itemBounds.Top))
                                        checkBoxHit = true;
                                }
                                if (ShowFileIcons)
                                {
                                    Rectangle fileIconBounds = layoutManager.GetIconBounds(index);
                                    if (fileIconBounds.Contains(pt.X + layoutManager.ItemAreaBounds.Left, pt.Y + layoutManager.ItemAreaBounds.Top))
                                        fileIconHit = true;
                                }
                            }

                            // Calculate sub item index
                            if (itemIndex != -1 && View == View.Details)
                            {
                                int xc1 = layoutManager.ColumnHeaderBounds.Left;
                                int colIndex = 0;
                                foreach (ImageListViewColumnHeader column in mColumns.GetDisplayedColumns())
                                {
                                    int xc2 = xc1 + column.Width;
                                    if (pt.X >= xc1 && pt.X < xc2)
                                    {
                                        subItemIndex = colIndex;
                                        break;
                                    }
                                    colIndex++;
                                    xc1 = xc2;
                                }
                            }
                        }

                        break;
                    }
                }
            } else
            {
                // Normalize to item area coordinates
                pt.X -= layoutManager.ItemAreaBounds.Left;
                pt.Y -= layoutManager.ItemAreaBounds.Top;

                if (pt.X > 0 && pt.Y > 0)
                {
                    int col = (pt.X + mViewOffset.X) / layoutManager.ItemSizeWithMargin.Width;
                    int row = (pt.Y + mViewOffset.Y) / layoutManager.ItemSizeWithMargin.Height;

                    if (ScrollOrientation == ScrollOrientation.HorizontalScroll || (ScrollOrientation == ScrollOrientation.VerticalScroll && col <= layoutManager.Cols))
                    {
                        int index = row * layoutManager.Cols + col;
                        if (index >= 0 && index <= Items.Count - 1)
                        {
                            Rectangle bounds = layoutManager.GetItemBounds(index);
                            if (bounds.Contains(pt.X + layoutManager.ItemAreaBounds.Left, pt.Y + layoutManager.ItemAreaBounds.Top))
                                itemIndex = index;
                            if (ShowCheckBoxes)
                            {
                                Rectangle checkBoxBounds = layoutManager.GetCheckBoxBounds(index);
                                if (checkBoxBounds.Contains(pt.X + layoutManager.ItemAreaBounds.Left, pt.Y + layoutManager.ItemAreaBounds.Top))
                                    checkBoxHit = true;
                            }
                            if (ShowFileIcons)
                            {
                                Rectangle fileIconBounds = layoutManager.GetIconBounds(index);
                                if (fileIconBounds.Contains(pt.X + layoutManager.ItemAreaBounds.Left, pt.Y + layoutManager.ItemAreaBounds.Top))
                                    fileIconHit = true;
                            }
                        }
                    }

                    // Calculate sub item index
                    if (itemIndex != -1 && View == View.Details)
                    {
                        int xc1 = layoutManager.ColumnHeaderBounds.Left;
                        int colIndex = 0;
                        foreach (ImageListViewColumnHeader column in mColumns.GetDisplayedColumns())
                        {
                            int xc2 = xc1 + column.Width;
                            if (pt.X >= xc1 && pt.X < xc2)
                            {
                                subItemIndex = colIndex;
                                break;
                            }
                            colIndex++;
                            xc1 = xc2;
                        }
                    }
                }
            }

            hitInfo = new HitInfo(itemIndex, subItemIndex, checkBoxHit, fileIconHit);
        }
    }
    /// <summary>
    /// Scrolls the image list view to ensure that the item with the specified 
    /// index is visible on the screen.
    /// </summary>
    /// <param name="itemIndex">The index of the item to make visible.</param>
    /// <returns>true if the item was made visible; otherwise false (item is already visible or the image list view is empty).</returns>
    public bool EnsureVisible(int itemIndex)
    {
        if (Items.Count == 0 || itemIndex < 0 || itemIndex > Items.Count - 1)
            return false;

        // Already visible?
        Rectangle bounds = layoutManager.ItemAreaBounds;
        Rectangle itemBounds = layoutManager.GetItemBounds(itemIndex);
        if (bounds.Contains(itemBounds))
            return false;

        // Scroll to item
        if (ScrollOrientation == ScrollOrientation.HorizontalScroll)
        {
            int delta = 0;
            delta = bounds.Left - itemBounds.Left;
            int newXOffset = mViewOffset.X - delta;
            if (newXOffset > hScrollBar.Maximum - hScrollBar.LargeChange + 1)
                newXOffset = hScrollBar.Maximum - hScrollBar.LargeChange + 1;
            if (newXOffset < hScrollBar.Minimum)
                newXOffset = hScrollBar.Minimum;
            mViewOffset.X = newXOffset;
            mViewOffset.Y = 0;
            hScrollBar.Value = newXOffset;
            vScrollBar.Value = 0;
        } else
        {
            int delta = 0;
            delta = bounds.Top - itemBounds.Top;
            int newYOffset = mViewOffset.Y - delta;
            if (newYOffset > vScrollBar.Maximum - vScrollBar.LargeChange + 1)
                newYOffset = vScrollBar.Maximum - vScrollBar.LargeChange + 1;
            if (newYOffset < vScrollBar.Minimum)
                newYOffset = vScrollBar.Minimum;
            mViewOffset.X = 0;
            mViewOffset.Y = newYOffset;
            hScrollBar.Value = 0;
            vScrollBar.Value = newYOffset;
        }

        Refresh();
        return true;
    }
    /// <summary>
    /// Determines whether the specified item is visible on the screen.
    /// </summary>
    /// <param name="item">The item to test.</param>
    /// <returns>An ItemVisibility value.</returns>
    public ItemVisibility IsItemVisible(ImageListViewItem item)
    {
        return IsItemVisible(item.Index);
    }
    /// <summary>
    /// Finds the first item that starts with the specified string.
    /// </summary>
    /// <param name="s">The text to search for.</param>
    /// <returns>
    /// The zero-based index of the first item found; or -1 if no match is found.
    /// </returns>
    public int FindString(string s)
    {
        return FindString(s, 0);
    }
    /// <summary>
    /// Finds the first item that starts with the specified string.
    /// </summary>
    /// <param name="s">The text to search for.</param>
    /// <param name="startIndex">The zero-based index of the first 
    /// item to be searched. Set to zero to search from the
    /// beginning of the control.</param>
    /// <returns>
    /// The zero-based index of the first item found; or -1 if no match is found.
    /// </returns>
    public int FindString(string s, int startIndex)
    {
        for (int i = startIndex; i < mItems.Count; i++)
        {
            ImageListViewItem item = mItems[i];
            if (item.Text.StartsWith(s, StringComparison.InvariantCultureIgnoreCase))
            {
                return item.Index;
            }
        }

        return -1;
    }
    #endregion

    #region Rendering Methods
    /// <summary>
    /// Refreshes the control.
    /// </summary>
    /// <param name="force">Forces a refresh even if the renderer is suspended.</param>
    /// <param name="lazy">Refreshes the control only if a set amount of time
    /// has passed since the last refresh.</param>
    internal void Refresh(bool force, bool lazy)
    {
        if (force)
            base.Refresh();
        else if (lazy)
        {
            rendererNeedsPaint = true;
            lazyRefreshTimer.Start();
        } else if (CanPaint())
            base.Refresh();
        else
            rendererNeedsPaint = true;
    }
    /// <summary>
    /// Redraws the owner control.
    /// </summary>
    /// <param name="force">If true, forces an immediate update, even if
    /// the renderer is suspended by a SuspendPaint call.</param>
    private void Refresh(bool force)
    {
        Refresh(force, false);
    }
    /// <summary>
    /// Redraws the owner control.
    /// </summary>
    private new void Refresh()
    {
        Refresh(false, false);
    }
    /// <summary>
    /// Suspends painting until a matching ResumePaint call is made.
    /// </summary>
    private void SuspendPaint()
    {
        if (rendererSuspendCount == 0)
            rendererNeedsPaint = false;
        rendererSuspendCount++;
    }
    /// <summary>
    /// Resumes painting. This call must be matched by a prior SuspendPaint call.
    /// </summary>
    private void ResumePaint()
    {
        System.Diagnostics.Debug.Assert(rendererSuspendCount > 0, "Suspend count does not match resume count.", "ResumePaint() must be matched by a prior SuspendPaint() call.");

        rendererSuspendCount--;
        if (rendererNeedsPaint)
            Refresh();
    }
    /// <summary>
    /// Determines if the control can be painted.
    /// </summary>
    private bool CanPaint()
    {
        return mRenderer == null ? false : !controlSuspended && rendererSuspendCount == 0;
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Determines whether the specified item is visible on the screen.
    /// </summary>
    /// <param name="guid">The Guid of the item to test.</param>
    /// <returns>true if the item is visible or partially visible; otherwise false.</returns>
    internal bool IsItemVisible(Guid guid)
    {
        return layoutManager.IsItemVisible(guid);
    }
    /// <summary>
    /// Determines whether the specified item is modified.
    /// </summary>
    /// <param name="guid">The Guid of the item to test.</param>
    /// <returns>true if the item is modified; otherwise false.</returns>
    internal bool IsItemDirty(Guid guid)
    {
        return mItems.TryGetValue(guid, out ImageListViewItem item) ? item.isDirty : false;
    }
    /// <summary>
    /// Determines whether the specified item is visible on the screen.
    /// </summary>
    /// <param name="itemIndex">The index of the item to test.</param>
    /// <returns>An ItemVisibility value.</returns>
    internal ItemVisibility IsItemVisible(int itemIndex)
    {
        return mItems.Count == 0
            ? ItemVisibility.NotVisible
            : itemIndex < 0 || itemIndex > mItems.Count - 1
            ? ItemVisibility.NotVisible
            : itemIndex < layoutManager.FirstPartiallyVisible || itemIndex > layoutManager.LastPartiallyVisible
            ? ItemVisibility.NotVisible
            : itemIndex >= layoutManager.FirstVisible && itemIndex <= layoutManager.LastVisible
            ? ItemVisibility.Visible
            : ItemVisibility.PartiallyVisible;
    }
    /// <summary>
    /// Gets the guids of visible items.
    /// </summary>
    internal Dictionary<Guid, bool> GetVisibleItems()
    {
        Dictionary<Guid, bool> visible = new();
        if (layoutManager.FirstPartiallyVisible != -1 && layoutManager.LastPartiallyVisible != -1)
        {
            int start = layoutManager.FirstPartiallyVisible;
            int end = layoutManager.LastPartiallyVisible;

            start -= layoutManager.Cols * layoutManager.Rows;
            end += layoutManager.Cols * layoutManager.Rows;

            start = Math.Min(mItems.Count - 1, Math.Max(0, start));
            end = Math.Min(mItems.Count - 1, Math.Max(0, end));

            for (int i = start; i <= end; i++)
                visible.Add(mItems[i].Guid, false);
        }
        return visible;
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Handles the DragOver event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that contains the event data.</param>
    protected override void OnDragOver(DragEventArgs e)
    {
        navigationManager.DragOver(e);
        base.OnDragOver(e);
    }
    /// <summary>
    /// Handles the DragEnter event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that contains the event data.</param>
    protected override void OnDragEnter(DragEventArgs e)
    {
        navigationManager.DragEnter(e);
        base.OnDragEnter(e);
    }
    /// <summary>
    /// Handles the DragLeave event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
    protected override void OnDragLeave(EventArgs e)
    {
        navigationManager.DragLeave();
        base.OnDragLeave(e);
    }
    /// <summary>
    /// Handles the DragDrop event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that contains the event data.</param>
    protected override void OnDragDrop(DragEventArgs e)
    {
        navigationManager.DragDrop(e);
        base.OnDragDrop(e);
    }
    /// <summary>
    /// Handles the Scroll event of the vScrollBar control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Forms.ScrollEventArgs"/> instance containing the event data.</param>
    private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
    {
        mViewOffset.Y = e.NewValue;
        Refresh();
    }
    /// <summary>
    /// Handles the Scroll event of the hScrollBar control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Forms.ScrollEventArgs"/> instance containing the event data.</param>
    private void hScrollBar_Scroll(object sender, ScrollEventArgs e)
    {
        mViewOffset.X = e.NewValue;
        Refresh();
    }

    /// <summary>
    /// Handles the Tick event of the lazyRefreshTimer control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void lazyRefreshTimer_Tick(object sender, EventArgs e)
    {
        try
        {
            if (IsHandleCreated && !IsDisposed)
                BeginInvoke(lazyRefreshCallback);
            lazyRefreshTimer.Stop();
        } finally
        {
            ;
        }
    }
    /// <summary>
    /// Handles the Resize event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        if (!disposed && mRenderer != null)
            mRenderer.ClearBuffer();

        if (hScrollBar != null && layoutManager != null)
            layoutManager.Update();

        Refresh();
    }
    /// <summary>
    /// Handles the Paint event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
    protected override void OnPaint(PaintEventArgs e)
    {
        if (!disposed && mRenderer != null)
            mRenderer.Render(e.Graphics);
        rendererNeedsPaint = false;
    }
    /// <summary>
    /// Handles the MouseDown event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
    protected override void OnMouseDown(MouseEventArgs e)
    {
        // Capture focus if right clicked
        if (!Focused && (e.Button & MouseButtons.Right) == MouseButtons.Right)
            Focus();

        navigationManager.MouseDown(e);
        base.OnMouseDown(e);
    }
    /// <summary>
    /// Handles the MouseUp event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
    protected override void OnMouseUp(MouseEventArgs e)
    {
        navigationManager.MouseUp(e);
        base.OnMouseUp(e);
    }
    /// <summary>
    /// Handles the MouseMove event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        navigationManager.MouseMove(e);
        base.OnMouseMove(e);
    }
    /// <summary>
    /// Handles the MouseWheel event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
    protected override void OnMouseWheel(MouseEventArgs e)
    {
        SuspendPaint();

        if (ScrollOrientation == ScrollOrientation.VerticalScroll)
        {
            int newYOffset = mViewOffset.Y - (e.Delta / SystemInformation.MouseWheelScrollDelta) * vScrollBar.SmallChange;
            if (newYOffset > vScrollBar.Maximum - vScrollBar.LargeChange + 1)
                newYOffset = vScrollBar.Maximum - vScrollBar.LargeChange + 1;
            if (newYOffset < 0)
                newYOffset = 0;
            if (newYOffset < vScrollBar.Minimum)
                newYOffset = vScrollBar.Minimum;
            if (newYOffset > vScrollBar.Maximum)
                newYOffset = vScrollBar.Maximum;
            mViewOffset.Y = newYOffset;
            vScrollBar.Value = newYOffset;
        } else
        {
            int newXOffset = mViewOffset.X - (e.Delta / SystemInformation.MouseWheelScrollDelta) * hScrollBar.SmallChange;
            if (newXOffset > hScrollBar.Maximum - hScrollBar.LargeChange + 1)
                newXOffset = hScrollBar.Maximum - hScrollBar.LargeChange + 1;
            if (newXOffset < 0)
                newXOffset = 0;
            if (newXOffset < hScrollBar.Minimum)
                newXOffset = hScrollBar.Minimum;
            if (newXOffset > hScrollBar.Maximum)
                newXOffset = hScrollBar.Maximum;
            mViewOffset.X = newXOffset;
            hScrollBar.Value = newXOffset;
        }

        OnMouseMove(e);
        Refresh(true);
        ResumePaint();

        base.OnMouseWheel(e);
    }
    /// <summary>
    /// Handles the MouseLeave event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
    protected override void OnMouseLeave(EventArgs e)
    {
        navigationManager.MouseLeave();
        base.OnMouseLeave(e);
    }
    /// <summary>
    /// Handles the MouseDoubleClick event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        navigationManager.MouseDoubleClick(e);
        base.OnMouseDoubleClick(e);
    }
    /// <summary>
    /// Handles the IsInputKey event.
    /// </summary>
    /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values.</param>
    /// <returns>
    /// true if the specified key is a regular input key; otherwise, false.
    /// </returns>
    protected override bool IsInputKey(Keys keyData)
    {
        return (keyData & Keys.Left) == Keys.Left || (keyData & Keys.Right) == Keys.Right || (keyData & Keys.Up) == Keys.Up || (keyData & Keys.Down) == Keys.Down
            ? true
            : base.IsInputKey(keyData);
    }
    /// <summary>
    /// Handles the KeyDown event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data.</param>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        navigationManager.KeyDown(e);
        base.OnKeyDown(e);
    }
    /// <summary>
    /// Handles the KeyUp event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data.</param>
    protected override void OnKeyUp(KeyEventArgs e)
    {
        navigationManager.KeyUp(e);
        base.OnKeyUp(e);
    }
    /// <summary>
    /// Handles the GotFocus event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        Refresh();
    }
    /// <summary>
    /// Handles the LostFocus event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        Refresh();
    }
    /// <summary>
    /// Releases the unmanaged resources used by the control and its child controls
    /// and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources;
    /// false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Events
                hScrollBar.Scroll -= hScrollBar_Scroll;
                vScrollBar.Scroll -= vScrollBar_Scroll;
                lazyRefreshTimer.Elapsed -= lazyRefreshTimer_Tick;

                // Resources
                if (mDefaultImage != null)
                    mDefaultImage.Dispose();
                if (mErrorImage != null)
                    mErrorImage.Dispose();
                if (mRatingImage != null)
                    mRatingImage.Dispose();
                if (mEmptyRatingImage != null)
                    mEmptyRatingImage.Dispose();

                // Child controls
                if (hScrollBar != null && hScrollBar.IsHandleCreated && !hScrollBar.IsDisposed)
                    hScrollBar.Dispose();
                if (vScrollBar != null && vScrollBar.IsHandleCreated && !vScrollBar.IsDisposed)
                    vScrollBar.Dispose();
                if (lazyRefreshTimer != null)
                    lazyRefreshTimer.Dispose();

                // internal classes
                defaultAdaptor.Dispose();
                thumbnailCache.Dispose();
                shellInfoCache.Dispose();
                metadataCache.Dispose();
                navigationManager.Dispose();
                if (mRenderer != null)
                    mRenderer.Dispose();
            }

            disposed = true;
        }

        if (IsHandleCreated && !IsDisposed && !InvokeRequired)
            base.Dispose(disposing);
    }
    #endregion

    #region Virtual Functions
    /// <summary>
    /// Raises the CacheError event.
    /// </summary>
    /// <param name="e">A CacheErrorEventArgs that contains event data.</param>
    protected virtual void OnCacheError(CacheErrorEventArgs e)
    {
        CacheError?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the DropFiles event.
    /// </summary>
    /// <param name="e">A DropFileEventArgs that contains event data.</param>
    protected virtual void OnDropFiles(DropFileEventArgs e)
    {
        DropFiles?.Invoke(this, e);

        if (e.Cancel)
            return;

        int index = e.Index;
        int firstItemIndex = 0;
        mSelectedItems.Clear(false);

        // Add items
        bool first = true;
        List<ImageListViewItem> items = new();
        foreach (string filename in e.FileNames)
        {
            ImageListViewItem item = new(filename);
            if (first || MultiSelect)
            {
                item.mSelected = true;
                first = false;
            }

            if (mItems.InsertInternal(index, item, defaultAdaptor))
            {
                if (firstItemIndex == 0)
                    firstItemIndex = item.Index;

                items.Add(item);
                index++;
            }
        }

        EnsureVisible(firstItemIndex);
        OnSelectionChangedInternal();

        OnDropComplete(new DropCompleteEventArgs(items.ToArray(), false));
    }
    /// <summary>
    /// Raises the DropItems event.
    /// </summary>
    /// <param name="e">A DropItemEventArgs that contains event data.</param>
    protected virtual void OnDropItems(DropItemEventArgs e)
    {
        DropItems?.Invoke(this, e);

        if (e.Cancel)
            return;

        int index = e.Index;
        mSelectedItems.Clear(false);

        foreach (ImageListViewItem item in e.Items)
        {
            if (item.Index < index) index--;
            Items.RemoveInternal(item, false);
        }
        if (index < 0) index = 0;
        if (index > Items.Count) index = Items.Count;

        // Add items
        bool first = true;
        List<ImageListViewItem> items = new();
        foreach (ImageListViewItem item in e.Items)
        {
            if (first || MultiSelect)
            {
                item.mSelected = true;
                first = false;
            }

            if (Items.InsertInternal(index, item, item.Adaptor))
            {
                items.Add(item);
                index++;
            }
        }

        OnSelectionChangedInternal();

        OnDropComplete(new DropCompleteEventArgs(items.ToArray(), true));
    }
    /// <summary>
    /// Raises the DropComplete event.
    /// </summary>
    /// <param name="e">A DropCompleteEventArgs that contains event data.</param>
    protected virtual void OnDropComplete(DropCompleteEventArgs e)
    {
        DropComplete?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the ColumnWidthChanged event.
    /// </summary>
    /// <param name="e">A ColumnEventArgs that contains event data.</param>
    protected virtual void OnColumnWidthChanged(ColumnEventArgs e)
    {
        ColumnWidthChanged?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the ColumnClick event.
    /// </summary>
    /// <param name="e">A ColumnClickEventArgs that contains event data.</param>
    protected virtual void OnColumnClick(ColumnClickEventArgs e)
    {
        ColumnClick?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the ColumnHover event.
    /// </summary>
    /// <param name="e">A ColumnClickEventArgs that contains event data.</param>
    protected virtual void OnColumnHover(ColumnHoverEventArgs e)
    {
        ColumnHover?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the ItemClick event.
    /// </summary>
    /// <param name="e">A ItemClickEventArgs that contains event data.</param>
    protected virtual void OnItemClick(ItemClickEventArgs e)
    {
        ItemClick?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the ItemCheckBoxClick event.
    /// </summary>
    /// <param name="e">A ItemEventArgs that contains event data.</param>
    protected virtual void OnItemCheckBoxClick(ItemEventArgs e)
    {
        ItemCheckBoxClick?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the ItemCheckBoxClick event.
    /// </summary>
    /// <param name="item">The checked item.</param>
    internal virtual void OnItemCheckBoxClickInternal(ImageListViewItem item)
    {
        OnItemCheckBoxClick(new ItemEventArgs(item));
    }
    /// <summary>
    /// Raises the ItemHover event.
    /// </summary>
    /// <param name="e">A ItemClickEventArgs that contains event data.</param>
    protected virtual void OnItemHover(ItemHoverEventArgs e)
    {
        ItemHover?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the ItemDoubleClick event.
    /// </summary>
    /// <param name="e">A ItemClickEventArgs that contains event data.</param>
    protected virtual void OnItemDoubleClick(ItemClickEventArgs e)
    {
        ItemDoubleClick?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the SelectionChanged event.
    /// </summary>
    /// <param name="e">A EventArgs that contains event data.</param>
    protected virtual void OnSelectionChanged(EventArgs e)
    {
        SelectionChanged?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the SelectionChanged event.
    /// </summary>
    internal void OnSelectionChangedInternal()
    {
        OnSelectionChanged(new EventArgs());
    }
    /// <summary>
    /// Raises the ThumbnailCached event.
    /// </summary>
    /// <param name="e">A ThumbnailCachedEventArgs that contains event data.</param>
    protected virtual void OnThumbnailCached(ThumbnailCachedEventArgs e)
    {
        ThumbnailCached?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the PaneReszied event.
    /// </summary>
    /// <param name="e">A PaneResizedEventArgs that contains event data.</param>
    protected virtual void OnPaneResized(PaneResizedEventArgs e)
    {
        PaneResized?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the PaneResizing event.
    /// </summary>
    /// <param name="e">A PaneResizingEventArgs that contains event data.</param>
    protected virtual void OnPaneResizing(PaneResizingEventArgs e)
    {
        PaneResizing?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the DetailsCaching event.
    /// </summary>
    /// <param name="e">An ItemEventArgs that contains event data.</param>
    protected virtual void OnDetailsCaching(ItemEventArgs e)
    {
        DetailsCaching?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the DetailsCached event.
    /// </summary>
    /// <param name="e">An ItemEventArgs that contains event data.</param>
    protected virtual void OnDetailsCached(ItemEventArgs e)
    {
        DetailsCached?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the DetailsCaching event.
    /// This method is invoked from the thumbnail thread.
    /// </summary>
    internal virtual void OnDetailsCachingInternal(Guid guid)
    {
        if (mItems.TryGetValue(guid, out ImageListViewItem item))
            OnDetailsCaching(new ItemEventArgs(item));
    }
    /// <summary>
    /// Raises the DetailsCached event.
    /// This method is invoked from the thumbnail thread.
    /// </summary>
    internal virtual void OnDetailsCachedInternal(Guid guid)
    {
        if (mItems.TryGetValue(guid, out ImageListViewItem item))
            OnDetailsCached(new ItemEventArgs(item));
    }
    /// <summary>
    /// Raises the ShellInfoCaching event.
    /// </summary>
    /// <param name="e">A ShellInfoCachingEventArgs that contains event data.</param>
    protected internal virtual void OnShellInfoCaching(ShellInfoCachingEventArgs e)
    {
        ShellInfoCaching?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the ShellInfoCached event.
    /// </summary>
    /// <param name="e">A ShellInfoCachedEventArgs that contains event data.</param>
    protected internal virtual void OnShellInfoCached(ShellInfoCachedEventArgs e)
    {
        ShellInfoCached?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the CacheError event.
    /// This method is invoked from the thumbnail thread.
    /// </summary>
    /// <param name="guid">The Guid of the ImageListViewItem that is associated with this error.
    /// This parameter can be null.</param>
    /// <param name="error">The error that occurred during an asynchronous operation.</param>
    /// <param name="cacheThread">The thread raising the error.</param>
    internal void OnCacheErrorInternal(Guid guid, Exception error, CacheThread cacheThread)
    {
        mItems.TryGetValue(guid, out ImageListViewItem item);
        OnCacheError(new CacheErrorEventArgs(item, error, cacheThread));
    }
    /// <summary>
    /// Raises the CacheError event.
    /// This method is invoked from the thumbnail thread.
    /// </summary>
    /// <param name="error">The error that occurred during an asynchronous operation.</param>
    /// <param name="cacheThread">The thread raising the error.</param>
    internal void OnCacheErrorInternal(Exception error, CacheThread cacheThread)
    {
        OnCacheError(new CacheErrorEventArgs(null, error, cacheThread));
    }
    /// <summary>
    /// Raises the ThumbnailCached event.
    /// This method is invoked from the thumbnail thread.
    /// </summary>
    /// <param name="guid">The guid of the item whose thumbnail is cached.</param>
    /// <param name="thumbnail">The cached image.</param>
    /// <param name="size">Requested thumbnail size.</param>
    /// <param name="thumbnailImage">true if the cached image is a thumbnail image; otherwise false
    /// if the image is a large image for gallery or pane views.</param>
    internal void OnThumbnailCachedInternal(Guid guid, Image thumbnail, Size size, bool thumbnailImage)
    {
        if (mItems.TryGetValue(guid, out ImageListViewItem item))
            OnThumbnailCached(new ThumbnailCachedEventArgs(item, thumbnail, size, thumbnailImage));
    }
    /// <summary>
    /// Updates item details.
    /// This method is invoked from the item cache thread.
    /// </summary>
    /// <param name="guid">Item guid.</param>
    /// <param name="details">Array of item details.</param>
    internal void UpdateItemDetailsInternal(Guid guid, Utility.Tuple<ColumnType, string, object>[] details)
    {
        if (mItems.TryGetValue(guid, out ImageListViewItem item))
            item.UpdateDetailsInternal(details);
    }
    /// <summary>
    /// Raises the ThumbnailCaching event.
    /// This method is invoked from the thumbnail thread.
    /// </summary>
    /// <param name="guid">The guid of the item whose thumbnail is cached.</param>
    /// <param name="size">Requested thumbnail size.</param>
    internal void OnThumbnailCachingInternal(Guid guid, Size size)
    {
        if (mItems.TryGetValue(guid, out ImageListViewItem item))
            OnThumbnailCaching(new ThumbnailCachingEventArgs(item, size));
    }
    /// <summary>
    /// Raises the ThumbnailCaching event.
    /// </summary>
    /// <param name="e">A ThumbnailCachingEventArgs that contains event data.</param>
    protected virtual void OnThumbnailCaching(ThumbnailCachingEventArgs e)
    {
        ThumbnailCaching?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the ItemCollectionChanged event.
    /// </summary>
    /// <param name="e">A ItemCollectionChangedEventArgs that contains event data.</param>
    protected virtual void OnItemCollectionChanged(ItemCollectionChangedEventArgs e)
    {
        ItemCollectionChanged?.Invoke(this, e);
    }
    #endregion

    #region Public Events
    /// <summary>
    /// Occurs when an error occurs during an asynchronous cache operation.
    /// </summary>
    [Category("Behavior"), Browsable(true), Description("Occurs when an error occurs during an asynchronous cache operation.")]
    public event CacheErrorEventHandler CacheError;
    /// <summary>
    /// Occurs after the user drops files on to the control.
    /// </summary>
    [Category("Drag Drop"), Browsable(true), Description("Occurs after the user drops files on to the control.")]
    public event DropFilesEventHandler DropFiles;
    /// <summary>
    /// Occurs after the user drops items on to the control.
    /// </summary>
    [Category("Drag Drop"), Browsable(true), Description("Occurs after the user drops items on to the control.")]
    public event DropItemsEventHandler DropItems;
    /// <summary>
    /// Occurs after items are dropped successfully.
    /// </summary>
    [Category("Drag Drop"), Browsable(true), Description("Occurs after items are dropped successfully.")]
    public event DropCompleteEventHandler DropComplete;
    /// <summary>
    /// Occurs after the user successfully resized a column header.
    /// </summary>
    [Category("Action"), Browsable(true), Description("Occurs after the user successfully resized a column header.")]
    public event ColumnWidthChangedEventHandler ColumnWidthChanged;
    /// <summary>
    /// Occurs when the user clicks a column header.
    /// </summary>
    [Category("Action"), Browsable(true), Description("Occurs when the user clicks a column header.")]
    public event ColumnClickEventHandler ColumnClick;
    /// <summary>
    /// Occurs when the user moves the mouse over (and out of) a column header.
    /// </summary>
    [Category("Action"), Browsable(true), Description("Occurs when the user moves the mouse over (and out of) a column header.")]
    public event ColumnHoverEventHandler ColumnHover;
    /// <summary>
    /// Occurs when the user clicks an item.
    /// </summary>
    [Category("Action"), Browsable(true), Description("Occurs when the user clicks an item.")]
    public event ItemClickEventHandler ItemClick;
    /// <summary>
    /// Occurs when the user clicks an item checkbox.
    /// </summary>
    [Category("Action"), Browsable(true), Description("Occurs when the user clicks an item checkbox.")]
    public event ItemCheckBoxClickEventHandler ItemCheckBoxClick;
    /// <summary>
    /// Occurs when the user moves the mouse over (and out of) an item.
    /// </summary>
    [Category("Action"), Browsable(true), Description("Occurs when the user moves the mouse over (and out of) an item.")]
    public event ItemHoverEventHandler ItemHover;
    /// <summary>
    /// Occurs when the user double-clicks an item.
    /// </summary>
    [Category("Action"), Browsable(true), Description("Occurs when the user double-clicks an item.")]
    public event ItemDoubleClickEventHandler ItemDoubleClick;
    /// <summary>
    /// Occurs when the selected items collection changes.
    /// </summary>
    [Category("Behavior"), Browsable(true), Description("Occurs when the selected items collection changes.")]
    public event EventHandler SelectionChanged;
    /// <summary>
    /// Occurs after an item thumbnail is cached.
    /// </summary>
    [Category("Behavior"), Browsable(true), Description("Occurs after an item thumbnail is cached.")]
    public event ThumbnailCachedEventHandler ThumbnailCached;
    /// <summary>
    /// Occurs before an item thumbnail is cached.
    /// </summary>
    [Category("Behavior"), Browsable(true), Description("Occurs before an item thumbnail is cached.")]
    public event ThumbnailCachingEventHandler ThumbnailCaching;
    /// <summary>
    /// Occurs after the item collection is changed.
    /// </summary>
    [Category("Behavior"), Browsable(true), Description("Occurs after the item collection is changed.")]
    public event ItemCollectionChangedEventHandler ItemCollectionChanged;
    /// <summary>
    /// Occurs after the pane is resized.
    /// </summary>
    [Category("Action"), Browsable(true), Description("Occurs after the pane is resized.")]
    public event PaneResizedEventHandler PaneResized;
    /// <summary>
    /// Occurs while the pane is being resized.
    /// </summary>
    [Category("Action"), Browsable(true), Description("Occurs while the pane is being resized.")]
    public event PaneResizingEventHandler PaneResizing;
    /// <summary>
    /// Occurs before an item details is cached.
    /// </summary>
    [Category("Behavior"), Browsable(true), Description("Occurs before an item details is cached.")]
    public event DetailsCachingEventHandler DetailsCaching;
    /// <summary>
    /// Occurs after an item details is cached.
    /// </summary>
    [Category("Behavior"), Browsable(true), Description("Occurs after an item details is cached.")]
    public event DetailsCachedEventHandler DetailsCached;
    /// <summary>
    /// Occurs before shell info for a file extension is cached.
    /// </summary>
    [Category("Behavior"), Browsable(true), Description("Occurs before shell info for a file extension is cached.")]
    public event ShellInfoCachingEventHandler ShellInfoCaching;
    /// <summary>
    /// Occurs after shell info for a file extension is cached.
    /// </summary>
    [Category("Behavior"), Browsable(true), Description("Occurs after shell info for a file extension is cached.")]
    public event ShellInfoCachedEventHandler ShellInfoCached;
    #endregion
}

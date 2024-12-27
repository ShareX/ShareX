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
using System.Drawing.Design;

namespace ShareX.ImageListView;

/// <summary>
/// Represents an item in the image list view.
/// </summary>
[TypeConverter(typeof(ImageListViewItemTypeConverter))]
public class ImageListViewItem : ICloneable
{
    #region Member Variables
    // Property backing fields
    internal int mIndex;
    private Guid mGuid;
    internal ImageListView mImageListView;
    internal bool mChecked;
    internal bool mSelected;
    internal bool mEnabled;
    private string mText;
    private int mZOrder;
    // File info
    internal string extension;
    private DateTime mDateAccessed;
    private DateTime mDateCreated;
    private DateTime mDateModified;
    private string mFileName;
    private string mFilePath;
    private string mFolderName;
    private long mFileSize;
    private Size mDimensions;
    private SizeF mResolution;
    // Exif tags
    private string mImageDescription;
    private string mEquipmentModel;
    private DateTime mDateTaken;
    private string mArtist;
    private string mCopyright;
    private float mExposureTime;
    private float mFNumber;
    private ushort mISOSpeed;
    private string mUserComment;
    private ushort mRating;
    private ushort mStarRating;
    private string mSoftware;
    private float mFocalLength;
    // Adaptor
    internal object mVirtualItemKey;
    internal ImageListView.ImageListViewItemAdaptor mAdaptor;
    // Used for custom columns
    private ImageListViewSubItemCollection mSubItems;
    // Used for cloned items
    internal Image clonedThumbnail;
    // Group info
    internal string group;
    internal int groupOrder;

    internal ImageListView.ImageListViewItemCollection owner;
    internal bool isDirty;
    private bool editing;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the cache state of the item thumbnail.
    /// </summary>
    [Category("Behavior"), Browsable(false), Description("Gets the cache state of the item thumbnail.")]
    public CacheState ThumbnailCacheState
    {
        get
        {
            return mImageListView.thumbnailCache.GetCacheState(mGuid, mImageListView.ThumbnailSize, mImageListView.UseEmbeddedThumbnails,
                mImageListView.AutoRotateThumbnails);
        }
    }
    /// <summary>
    /// Gets a value determining if the item is focused.
    /// </summary>
    [Category("Appearance"), Browsable(false), Description("Gets a value determining if the item is focused."), DefaultValue(false)]
    public bool Focused
    {
        get
        {
            return owner == null || owner.FocusedItem == null ? false : this == owner.FocusedItem;
        }
        set
        {
            if (owner != null)
                owner.FocusedItem = this;
        }
    }
    /// <summary>
    /// Gets a value determining if the item is enabled.
    /// </summary>
    [Category("Appearance"), Browsable(true), Description("Gets a value determining if the item is enabled."), DefaultValue(true)]
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
            if (!mEnabled && mSelected)
            {
                mSelected = false;
                if (mImageListView != null)
                    mImageListView.OnSelectionChangedInternal();
            }
            if (mImageListView != null && mImageListView.IsItemVisible(mGuid))
                mImageListView.Refresh();
        }
    }
    /// <summary>
    /// Gets the unique identifier for this item.
    /// </summary>
    [Category("Behavior"), Browsable(false), Description("Gets the unique identifier for this item.")]
    internal Guid Guid { get { return mGuid; } private set { mGuid = value; } }
    /// <summary>
    /// Gets the adaptor of this item.
    /// </summary>
    [Category("Behavior"), Browsable(false), Description("Gets the adaptor of this item.")]
    public ImageListView.ImageListViewItemAdaptor Adaptor { get { return mAdaptor; } }
    /// <summary>
    /// Gets the virtual item key associated with this item.
    /// Returns null if the item is not a virtual item.
    /// </summary>
    [Category("Behavior"), Browsable(false), Description("Gets the virtual item key associated with this item.")]
    public object VirtualItemKey { get { return mVirtualItemKey; } }
    /// <summary>
    /// Gets the ImageListView owning this item.
    /// </summary>
    [Category("Behavior"), Browsable(false), Description("Gets the ImageListView owning this item.")]
    public ImageListView ImageListView { get { return mImageListView; } private set { mImageListView = value; } }
    /// <summary>
    /// Gets the index of the item.
    /// </summary>
    [Category("Behavior"), Browsable(false), Description("Gets the index of the item."), EditorBrowsable(EditorBrowsableState.Advanced)]
    public int Index { get { return mIndex; } }
    /// <summary>
    /// Gets or sets a value determining if the item is checked.
    /// </summary>
    [Category("Appearance"), Browsable(true), Description("Gets or sets a value determining if the item is checked."), DefaultValue(false)]
    public bool Checked
    {
        get
        {
            return mChecked;
        }
        set
        {
            if (value != mChecked)
            {
                mChecked = value;
                if (mImageListView != null)
                    mImageListView.OnItemCheckBoxClickInternal(this);
            }
        }
    }
    /// <summary>
    /// Gets or sets a value determining if the item is selected.
    /// </summary>
    [Category("Appearance"), Browsable(false), Description("Gets or sets a value determining if the item is selected."), DefaultValue(false)]
    public bool Selected
    {
        get
        {
            return mSelected;
        }
        set
        {
            if (value != mSelected && mEnabled)
            {
                mSelected = value;
                if (mImageListView != null)
                {
                    mImageListView.OnSelectionChangedInternal();
                    if (mImageListView.IsItemVisible(mGuid))
                        mImageListView.Refresh();
                }
            }
        }
    }
    /// <summary>
    /// Gets or sets the user-defined data associated with the item.
    /// </summary>
    [Category("Data"), Browsable(true), Description("Gets or sets the user-defined data associated with the item."), TypeConverter(typeof(StringConverter))]
    public object Tag { get; set; }
    /// <summary>
    /// Gets or sets the text associated with this item. If left blank, item Text 
    /// reverts to the name of the image file.
    /// </summary>
    [Category("Appearance"), Browsable(true), Description("Gets or sets the text associated with this item. If left blank, item Text reverts to the name of the image file.")]
    public string Text
    {
        get
        {
            return mText;
        }
        set
        {
            mText = value;
            if (mImageListView != null && mImageListView.IsItemVisible(mGuid))
                mImageListView.Refresh();
        }
    }
    /// <summary>
    /// Gets the collection of sub items.
    /// </summary>
    [Category("Appearance"), Browsable(true), Description("Gets the collection of sub items.")]
    public ImageListViewSubItemCollection SubItems
    {
        get
        {
            return mSubItems;
        }
    }
    /// <summary>
    /// Gets or sets the name of the image file represented by this item.
    /// </summary>        
    [Category("File Properties"), Browsable(true), Description("Gets or sets the name of the image file represented by this item.")]
    [Editor(typeof(OpenFileDialogEditor), typeof(UITypeEditor))]
    public string FileName
    {
        get
        {
            return mFileName;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("FileName cannot be null");

            if (mFileName != value)
            {
                mFileName = value;
                mVirtualItemKey = mFileName;

                if (string.IsNullOrEmpty(mText))
                    mText = Path.GetFileName(mFileName);
                extension = Path.GetExtension(mFileName);

                isDirty = true;
                if (mImageListView != null)
                {
                    mImageListView.thumbnailCache.Remove(mGuid, true);
                    mImageListView.metadataCache.Remove(mGuid);
                    mImageListView.metadataCache.Add(mGuid, Adaptor, mFileName);
                    if (mImageListView.IsItemVisible(mGuid))
                        mImageListView.Refresh();
                }
            }
        }
    }
    /// <summary>
    /// Gets the thumbnail image. If the thumbnail image is not cached, it will be 
    /// added to the cache queue and null will be returned. The returned image needs
    /// to be disposed by the caller.
    /// </summary>
    [Category("Appearance"), Browsable(false), Description("Gets the thumbnail image.")]
    public Image ThumbnailImage
    {
        get
        {
            if (mImageListView == null)
                throw new InvalidOperationException("Owner control is null.");

            if (ThumbnailCacheState != CacheState.Cached)
            {
                mImageListView.thumbnailCache.Add(Guid, mAdaptor, mVirtualItemKey, mImageListView.ThumbnailSize,
                    mImageListView.UseEmbeddedThumbnails, mImageListView.AutoRotateThumbnails);
            }

            return mImageListView.thumbnailCache.GetImage(Guid, mAdaptor, mVirtualItemKey, mImageListView.ThumbnailSize, mImageListView.UseEmbeddedThumbnails,
                mImageListView.AutoRotateThumbnails, true);
        }
    }
    /// <summary>
    /// Gets or sets the draw order of the item.
    /// </summary>
    [Category("Appearance"), Browsable(true), Description("Gets or sets the draw order of the item."), DefaultValue(0)]
    public int ZOrder { get { return mZOrder; } set { mZOrder = value; } }
    #endregion

    #region Shell Properties
    /// <summary>
    /// Gets the small shell icon of the image file represented by this item.
    /// If the icon image is not cached, it will be added to the cache queue and null will be returned.
    /// </summary>
    [Category("Appearance"), Browsable(false), Description("Gets the small shell icon of the image file represented by this item.")]
    public Image SmallIcon
    {
        get
        {
            if (mImageListView == null)
                throw new InvalidOperationException("Owner control is null.");

            string iconPath = PathForShellIcon();
            CacheState state = mImageListView.shellInfoCache.GetCacheState(iconPath);
            if (state == CacheState.Cached)
            {
                return mImageListView.shellInfoCache.GetSmallIcon(iconPath);
            } else if (state == CacheState.Error)
            {
                if (mImageListView.RetryOnError)
                {
                    mImageListView.shellInfoCache.Remove(iconPath);
                    mImageListView.shellInfoCache.Add(iconPath);
                }
                return null;
            } else
            {
                mImageListView.shellInfoCache.Add(iconPath);
                return null;
            }
        }
    }
    /// <summary>
    /// Gets the large shell icon of the image file represented by this item.
    /// If the icon image is not cached, it will be added to the cache queue and null will be returned.
    /// </summary>
    [Category("Appearance"), Browsable(false), Description("Gets the large shell icon of the image file represented by this item.")]
    public Image LargeIcon
    {
        get
        {
            if (mImageListView == null)
                throw new InvalidOperationException("Owner control is null.");

            string iconPath = PathForShellIcon();
            CacheState state = mImageListView.shellInfoCache.GetCacheState(iconPath);
            if (state == CacheState.Cached)
            {
                return mImageListView.shellInfoCache.GetLargeIcon(iconPath);
            } else if (state == CacheState.Error)
            {
                if (mImageListView.RetryOnError)
                {
                    mImageListView.shellInfoCache.Remove(iconPath);
                    mImageListView.shellInfoCache.Add(iconPath);
                }
                return null;
            } else
            {
                mImageListView.shellInfoCache.Add(iconPath);
                return null;
            }
        }
    }
    /// <summary>
    /// Gets the last access date of the image file represented by this item.
    /// </summary>
    [Category("File Properties"), Browsable(true), Description("Gets the last access date of the image file represented by this item.")]
    public DateTime DateAccessed { get { UpdateFileInfo(); return mDateAccessed; } }
    /// <summary>
    /// Gets the creation date of the image file represented by this item.
    /// </summary>
    [Category("File Properties"), Browsable(true), Description("Gets the creation date of the image file represented by this item.")]
    public DateTime DateCreated { get { UpdateFileInfo(); return mDateCreated; } }
    /// <summary>
    /// Gets the modification date of the image file represented by this item.
    /// </summary>
    [Category("File Properties"), Browsable(true), Description("Gets the modification date of the image file represented by this item.")]
    public DateTime DateModified { get { UpdateFileInfo(); return mDateModified; } }
    /// <summary>
    /// Gets the shell type of the image file represented by this item.
    /// </summary>
    [Category("File Properties"), Browsable(true), Description("Gets the shell type of the image file represented by this item.")]
    public string FileType
    {
        get
        {
            if (mImageListView == null)
                throw new InvalidOperationException("Owner control is null.");

            string iconPath = PathForShellIcon();
            CacheState state = mImageListView.shellInfoCache.GetCacheState(iconPath);
            if (state == CacheState.Cached)
            {
                return mImageListView.shellInfoCache.GetFileType(iconPath);
            } else if (state == CacheState.Error)
            {
                if (mImageListView.RetryOnError)
                {
                    mImageListView.shellInfoCache.Remove(iconPath);
                    mImageListView.shellInfoCache.Add(iconPath);
                }
                return null;
            } else
            {
                mImageListView.shellInfoCache.Add(iconPath);
                return null;
            }
        }
    }
    /// <summary>
    /// Gets the path of the image file represented by this item.
    /// </summary>        
    [Category("File Properties"), Browsable(true), Description("Gets the path of the image file represented by this item.")]
    public string FilePath { get { UpdateFileInfo(); return mFilePath; } }
    /// <summary>
    /// Gets the name of the folder represented by this item.
    /// </summary>        
    [Category("File Properties"), Browsable(true), Description(" Gets the name of the folder represented by this item.")]
    public string FolderName { get { UpdateFileInfo(); return mFolderName; } }
    /// <summary>
    /// Gets file size in bytes.
    /// </summary>
    [Category("File Properties"), Browsable(true), Description("Gets file size in bytes.")]
    public long FileSize { get { UpdateFileInfo(); return mFileSize; } }
    #endregion

    #region Exif Properties
    /// <summary>
    /// Gets image dimensions.
    /// </summary>
    [Category("Image Properties"), Browsable(true), Description("Gets image dimensions.")]
    public Size Dimensions { get { UpdateFileInfo(); return mDimensions; } }
    /// <summary>
    /// Gets image resolution in pixels per inch.
    /// </summary>
    [Category("Image Properties"), Browsable(true), Description("Gets image resolution in pixels per inch.")]
    public SizeF Resolution { get { UpdateFileInfo(); return mResolution; } }
    /// <summary>
    /// Gets image description.
    /// </summary>
    [Category("Image Properties"), Browsable(true), Description("Gets image description.")]
    public string ImageDescription { get { UpdateFileInfo(); return mImageDescription; } }
    /// <summary>
    /// Gets the camera model.
    /// </summary>
    [Category("Camera Properties"), Browsable(true), Description("Gets the camera model.")]
    public string EquipmentModel { get { UpdateFileInfo(); return mEquipmentModel; } }
    /// <summary>
    /// Gets the date and time the image was taken.
    /// </summary>
    [Category("Image Properties"), Browsable(true), Description("Gets the date and time the image was taken.")]
    public DateTime DateTaken { get { UpdateFileInfo(); return mDateTaken; } }
    /// <summary>
    /// Gets the name of the artist.
    /// </summary>
    [Category("Image Properties"), Browsable(true), Description("Gets the name of the artist.")]
    public string Artist { get { UpdateFileInfo(); return mArtist; } }
    /// <summary>
    /// Gets image copyright information.
    /// </summary>
    [Category("Image Properties"), Browsable(true), Description("Gets image copyright information.")]
    public string Copyright { get { UpdateFileInfo(); return mCopyright; } }
    /// <summary>
    /// Gets the exposure time in seconds.
    /// </summary>
    [Category("Camera Properties"), Browsable(true), Description("Gets the exposure time in seconds.")]
    public float ExposureTime { get { UpdateFileInfo(); return mExposureTime; } }
    /// <summary>
    /// Gets the F number.
    /// </summary>
    [Category("Camera Properties"), Browsable(true), Description("Gets the F number.")]
    public float FNumber { get { UpdateFileInfo(); return mFNumber; } }
    /// <summary>
    /// Gets the ISO speed.
    /// </summary>
    [Category("Camera Properties"), Browsable(true), Description("Gets the ISO speed.")]
    public ushort ISOSpeed { get { UpdateFileInfo(); return mISOSpeed; } }
    /// <summary>
    /// Gets user comments.
    /// </summary>
    [Category("Image Properties"), Browsable(true), Description("Gets user comments.")]
    public string UserComment { get { UpdateFileInfo(); return mUserComment; } }
    /// <summary>
    /// Gets rating in percent between 0-99 (Windows specific).
    /// </summary>
    [Category("Image Properties"), Browsable(true), Description("Gets rating in percent between 0-99.")]
    public ushort Rating { get { UpdateFileInfo(); return mRating; } }
    /// <summary>
    /// Gets the star rating between 0-5 (Windows specific).
    /// </summary>
    [Category("Image Properties"), Browsable(true), Description("Gets the star rating between 0-5.")]
    public ushort StarRating { get { UpdateFileInfo(); return mStarRating; } }
    /// <summary>
    /// Gets the name of the application that created this file.
    /// </summary>
    [Category("Image Properties"), Browsable(true), Description("Gets the name of the application that created this file.")]
    public string Software { get { UpdateFileInfo(); return mSoftware; } }
    /// <summary>
    /// Gets focal length of the lens in millimeters.
    /// </summary>
    [Category("Camera Properties"), Browsable(true), Description("Gets focal length of the lens in millimeters.")]
    public float FocalLength { get { UpdateFileInfo(); return mFocalLength; } }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageListViewItem"/> class.
    /// </summary>
    public ImageListViewItem()
    {
        mIndex = -1;
        owner = null;

        mZOrder = 0;

        Guid = Guid.NewGuid();
        ImageListView = null;
        Checked = false;
        Selected = false;
        Enabled = true;

        isDirty = true;
        editing = false;

        mVirtualItemKey = null;

        Tag = null;

        mSubItems = new ImageListViewSubItemCollection(this);

        groupOrder = 0;
        group = string.Empty;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageListViewItem"/> class.
    /// </summary>
    /// <param name="filename">The image filename representing the item.</param>
    /// <param name="text">Item text</param>
    public ImageListViewItem(string filename, string text)
        : this()
    {
        if (File.Exists(filename))
        {
            mFileName = filename;
            extension = Path.GetExtension(filename);
            // if text parameter is empty then get file name for item text
            mText = string.IsNullOrEmpty(text) ? Path.GetFileName(filename) : text;
        } else if (string.IsNullOrEmpty(text))
        {
            mFileName = filename;
            mText = filename;
        } else
        {
            mFileName = filename;
            mText = text;
        }
        mVirtualItemKey = mFileName;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageListViewItem"/> class.
    /// </summary>
    /// <param name="filename">The image filename representing the item.</param>
    public ImageListViewItem(string filename)
        : this(filename, string.Empty)
    {
        ;
    }
    /// <summary>
    /// Initializes a new instance of a virtual <see cref="ImageListViewItem"/> class.
    /// </summary>
    /// <param name="key">The key identifying this item.</param>
    /// <param name="text">Text of this item.</param>
    public ImageListViewItem(object key, string text)
        : this()
    {
        mVirtualItemKey = key;
        mText = text;
    }
    /// <summary>
    /// Initializes a new instance of a virtual <see cref="ImageListViewItem"/> class.
    /// </summary>
    /// <param name="key">The key identifying this item.</param>
    public ImageListViewItem(object key)
        : this(key, string.Empty)
    {
        ;
    }
    #endregion

    #region Instance Methods
    /// <summary>
    /// Begins editing the item.
    /// This method must be used while editing the item
    /// to prevent collisions with the cache manager.
    /// </summary>
    public void BeginEdit()
    {
        if (editing == true)
            throw new InvalidOperationException("Already editing this item.");

        if (mImageListView == null)
            throw new InvalidOperationException("Owner control is null.");

        mImageListView.thumbnailCache.BeginItemEdit(mGuid);
        mImageListView.metadataCache.BeginItemEdit(mGuid);

        editing = true;
    }
    /// <summary>
    /// Ends editing and updates the item.
    /// </summary>
    /// <param name="update">If set to true, the item will be immediately updated.</param>
    public void EndEdit(bool update)
    {
        if (editing == false)
            throw new InvalidOperationException("This item is not being edited.");

        if (mImageListView == null)
            throw new InvalidOperationException("Owner control is null.");

        mImageListView.thumbnailCache.EndItemEdit(mGuid);
        mImageListView.metadataCache.EndItemEdit(mGuid);

        editing = false;
        if (update) Update();
    }
    /// <summary>
    /// Ends editing and updates the item.
    /// </summary>
    public void EndEdit()
    {
        EndEdit(true);
    }
    /// <summary>
    /// Updates item thumbnail and item details.
    /// </summary>
    public void Update()
    {
        isDirty = true;
        if (mImageListView != null)
        {
            mImageListView.thumbnailCache.Remove(mGuid, true);
            mImageListView.metadataCache.Remove(mGuid);
            mImageListView.metadataCache.Add(mGuid, mAdaptor, mVirtualItemKey);
            mImageListView.Refresh();
        }
    }
    /// <summary>
    /// Returns the sub item item text corresponding to the specified column type.
    /// </summary>
    /// <param name="type">The type of information to return.</param>
    /// <returns>Formatted text for the given column type.</returns>
    public string GetSubItemText(ColumnType type)
    {
        switch (type)
        {
            case ColumnType.Custom:
                throw new ArgumentException("Column type is ambiguous. You must access custom columns by key.", "type");
            case ColumnType.Name:
                return Text;
            case ColumnType.FileName:
                return FileName;
            case ColumnType.DateAccessed:
                return mDateAccessed == DateTime.MinValue ? "" : mDateAccessed.ToString("g");
            case ColumnType.DateCreated:
                return mDateCreated == DateTime.MinValue ? "" : mDateCreated.ToString("g");
            case ColumnType.DateModified:
                return mDateModified == DateTime.MinValue ? "" : mDateModified.ToString("g");
            case ColumnType.FilePath:
                return mFilePath;
            case ColumnType.FolderName:
                return mFolderName;
            case ColumnType.FileSize:
                return Utility.FormatSize(mFileSize);
            case ColumnType.FileType:
                return FileType;
            case ColumnType.Dimensions:
                return mDimensions == Size.Empty ? "" : string.Format("{0} x {1}", mDimensions.Width, mDimensions.Height);
            case ColumnType.Resolution:
                return mResolution == SizeF.Empty ? "" : string.Format("{0} x {1}", mResolution.Width, mResolution.Height);
            case ColumnType.ImageDescription:
                return mImageDescription;
            case ColumnType.EquipmentModel:
                return mEquipmentModel;
            case ColumnType.DateTaken:
                return mDateTaken == DateTime.MinValue ? "" : mDateTaken.ToString("g");
            case ColumnType.Artist:
                return mArtist;
            case ColumnType.Copyright:
                return mCopyright;
            case ColumnType.ExposureTime:
                return mExposureTime < double.Epsilon
                    ? ""
                    : mExposureTime >= 1.0f ? mExposureTime.ToString("f1") : string.Format("1/{0:f0}", 1.0f / mExposureTime);
            case ColumnType.FNumber:
                return mFNumber < double.Epsilon ? "" : mFNumber.ToString("f1");
            case ColumnType.ISOSpeed:
                return mISOSpeed == 0 ? "" : mISOSpeed.ToString();
            case ColumnType.UserComment:
                return mUserComment;
            case ColumnType.Rating:
                return mRating == 0 ? "" : mRating.ToString();
            case ColumnType.Software:
                return mSoftware;
            case ColumnType.FocalLength:
                return mFocalLength < double.Epsilon ? "" : mFocalLength.ToString("f1");
            default:
                throw new ArgumentException("Unknown column type", "type");
        }
    }
    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return !string.IsNullOrEmpty(mText)
            ? mText
            : !string.IsNullOrEmpty(mFileName) ? Path.GetFileName(mFileName) : string.Format("Item {0}", mIndex);
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Gets the simpel rating (0-5)
    /// </summary>
    /// <returns></returns>
    internal ushort GetSimpleRating()
    {
        return mStarRating;
    }
    /// <summary>
    /// Sets the simple rating (0-5) from rating (0-99).
    /// </summary>
    private void UpdateRating()
    {
        mStarRating = mRating >= 1 && mRating <= 12
            ? (ushort)1
            : mRating >= 13 && mRating <= 37
            ? (ushort)2
            : mRating >= 38 && mRating <= 62
            ? (ushort)3
            : mRating >= 63 && mRating <= 87 ? (ushort)4 : mRating >= 88 && mRating <= 99 ? (ushort)5 : (ushort)0;
    }
    /// <summary>
    /// Gets an image from the cache manager.
    /// If the thumbnail image is not cached, it will be 
    /// added to the cache queue and DefaultImage of the owner image list view will
    /// be returned. If the thumbnail could not be cached ErrorImage of the owner
    /// image list view will be returned.
    /// </summary>
    /// <param name="imageType">Type of cached image to return.</param>
    /// <returns>Requested thumbnail or icon.</returns>
    public Image GetCachedImage(CachedImageType imageType)
    {
        if (mImageListView == null)
            throw new InvalidOperationException("Owner control is null.");

        string iconPath = PathForShellIcon();

        if (imageType == CachedImageType.SmallIcon || imageType == CachedImageType.LargeIcon)
        {
            if (string.IsNullOrEmpty(iconPath))
                return mImageListView.DefaultImage;

            CacheState state = mImageListView.shellInfoCache.GetCacheState(iconPath);
            if (state == CacheState.Cached)
            {
                return imageType == CachedImageType.SmallIcon
                    ? mImageListView.shellInfoCache.GetSmallIcon(iconPath)
                    : mImageListView.shellInfoCache.GetLargeIcon(iconPath);
            } else if (state == CacheState.Error)
            {
                if (mImageListView.RetryOnError)
                {
                    mImageListView.shellInfoCache.Remove(iconPath);
                    mImageListView.shellInfoCache.Add(iconPath);
                }
                return mImageListView.ErrorImage;
            } else
            {
                mImageListView.shellInfoCache.Add(iconPath);
                return mImageListView.DefaultImage;
            }
        } else
        {
            Image img = null;
            CacheState state = ThumbnailCacheState;

            if (state == CacheState.Error)
            {
                if (mImageListView.ShellIconFallback && !string.IsNullOrEmpty(iconPath))
                {
                    CacheState iconstate = mImageListView.shellInfoCache.GetCacheState(iconPath);
                    if (iconstate == CacheState.Cached)
                    {
                        img = mImageListView.ThumbnailSize.Width > 32 && mImageListView.ThumbnailSize.Height > 32
                            ? mImageListView.shellInfoCache.GetLargeIcon(iconPath)
                            : mImageListView.shellInfoCache.GetSmallIcon(iconPath);
                    } else if (iconstate == CacheState.Error)
                    {
                        if (mImageListView.RetryOnError)
                        {
                            mImageListView.shellInfoCache.Remove(iconPath);
                            mImageListView.shellInfoCache.Add(iconPath);
                        }
                    } else
                    {
                        mImageListView.shellInfoCache.Add(iconPath);
                    }
                }

                if (img == null)
                    img = mImageListView.ErrorImage;
                return img;
            }

            img = mImageListView.thumbnailCache.GetImage(Guid, mAdaptor, mVirtualItemKey, mImageListView.ThumbnailSize, mImageListView.UseEmbeddedThumbnails,
                mImageListView.AutoRotateThumbnails, false);

            if (state == CacheState.Cached)
                return img;

            mImageListView.thumbnailCache.Add(Guid, mAdaptor, mVirtualItemKey, mImageListView.ThumbnailSize,
                mImageListView.UseEmbeddedThumbnails, mImageListView.AutoRotateThumbnails);

            if (img == null && string.IsNullOrEmpty(iconPath))
                return mImageListView.DefaultImage;

            if (img == null && mImageListView.ShellIconFallback && mImageListView.ThumbnailSize.Width > 16 && mImageListView.ThumbnailSize.Height > 16)
                img = mImageListView.shellInfoCache.GetLargeIcon(iconPath);
            if (img == null && mImageListView.ShellIconFallback)
                img = mImageListView.shellInfoCache.GetSmallIcon(iconPath);
            if (img == null)
                img = mImageListView.DefaultImage;

            return img;
        }
    }
    /// <summary>
    /// Updates file info for the image file represented by this item.
    /// Item details will be updated synchronously without waiting for the
    /// cache thread.
    /// </summary>
    private void UpdateFileInfo()
    {
        if (!isDirty) return;

        if (mImageListView != null)
        {
            UpdateDetailsInternal(Adaptor.GetDetails(mVirtualItemKey));
        }
    }
    /// <summary>
    /// Invoked by the worker thread to update item details.
    /// </summary>
    /// <param name="info">Item details.</param>
    internal void UpdateDetailsInternal(Utility.Tuple<ColumnType, string, object>[] info)
    {
        if (!isDirty) return;

        // File info
        foreach (Utility.Tuple<ColumnType, string, object> item in info)
        {
            switch (item.Item1)
            {
                case ColumnType.DateAccessed:
                    mDateAccessed = (DateTime)item.Item3;
                    break;
                case ColumnType.DateCreated:
                    mDateCreated = (DateTime)item.Item3;
                    break;
                case ColumnType.DateModified:
                    mDateModified = (DateTime)item.Item3;
                    break;
                case ColumnType.FileSize:
                    mFileSize = (long)item.Item3;
                    break;
                case ColumnType.FilePath:
                    mFilePath = (string)item.Item3;
                    break;
                case ColumnType.FolderName:
                    mFolderName = (string)item.Item3;
                    break;
                case ColumnType.Dimensions:
                    mDimensions = (Size)item.Item3;
                    break;
                case ColumnType.Resolution:
                    mResolution = (SizeF)item.Item3;
                    break;
                case ColumnType.ImageDescription:
                    mImageDescription = (string)item.Item3;
                    break;
                case ColumnType.EquipmentModel:
                    mEquipmentModel = (string)item.Item3;
                    break;
                case ColumnType.DateTaken:
                    mDateTaken = (DateTime)item.Item3;
                    break;
                case ColumnType.Artist:
                    mArtist = (string)item.Item3;
                    break;
                case ColumnType.Copyright:
                    mCopyright = (string)item.Item3;
                    break;
                case ColumnType.ExposureTime:
                    mExposureTime = (float)item.Item3;
                    break;
                case ColumnType.FNumber:
                    mFNumber = (float)item.Item3;
                    break;
                case ColumnType.ISOSpeed:
                    mISOSpeed = (ushort)item.Item3;
                    break;
                case ColumnType.UserComment:
                    mUserComment = (string)item.Item3;
                    break;
                case ColumnType.Rating:
                    mRating = (ushort)item.Item3;
                    break;
                case ColumnType.Software:
                    mSoftware = (string)item.Item3;
                    break;
                case ColumnType.FocalLength:
                    mFocalLength = (float)item.Item3;
                    break;
                case ColumnType.Custom:
                    string key = item.Item2;
                    string value = (string)item.Item3;
                    mSubItems[key] = new ImageListViewSubItem(this, value);
                    break;
                default:
                    throw new Exception("Unknown column type.");
            }
        }

        UpdateRating();

        isDirty = false;
    }
    /// <summary>
    /// Updates group order and name of the item.
    /// </summary>
    /// <param name="column">The group column.</param>
    internal void UpdateGroup(ImageListView.ImageListViewColumnHeader column)
    {
        if (column == null)
        {
            groupOrder = 0;
            group = string.Empty;
            return;
        } else if (column.Grouper != null)
        {
            ImageListView.GroupInfo info = column.Grouper.GetGroupInfo(this);
            groupOrder = info.Order;
            group = info.Name;
            return;
        }

        Utility.Tuple<int, string> groupInfo = new(0, string.Empty);

        switch (column.Type)
        {
            case ColumnType.DateAccessed:
                groupInfo = Utility.GroupTextDate(DateAccessed);
                break;
            case ColumnType.DateCreated:
                groupInfo = Utility.GroupTextDate(DateCreated);
                break;
            case ColumnType.DateModified:
                groupInfo = Utility.GroupTextDate(DateModified);
                break;
            case ColumnType.Dimensions:
                groupInfo = Utility.GroupTextDimension(Dimensions);
                break;
            case ColumnType.FileName:
                groupInfo = Utility.GroupTextAlpha(FileName);
                break;
            case ColumnType.FilePath:
                groupInfo = Utility.GroupTextAlpha(FilePath);
                break;
            case ColumnType.FolderName:
                groupInfo = Utility.GroupTextAlpha(FolderName);
                break;
            case ColumnType.FileSize:
                groupInfo = Utility.GroupTextFileSize(FileSize);
                break;
            case ColumnType.FileType:
                groupInfo = Utility.GroupTextAlpha(FileType);
                break;
            case ColumnType.Name:
                groupInfo = Utility.GroupTextAlpha(Text);
                break;
            case ColumnType.ImageDescription:
                groupInfo = Utility.GroupTextAlpha(ImageDescription);
                break;
            case ColumnType.EquipmentModel:
                groupInfo = Utility.GroupTextAlpha(EquipmentModel);
                break;
            case ColumnType.DateTaken:
                groupInfo = Utility.GroupTextDate(DateTaken);
                break;
            case ColumnType.Artist:
                groupInfo = Utility.GroupTextAlpha(Artist);
                break;
            case ColumnType.Copyright:
                groupInfo = Utility.GroupTextAlpha(Copyright);
                break;
            case ColumnType.UserComment:
                groupInfo = Utility.GroupTextAlpha(UserComment);
                break;
            case ColumnType.Software:
                groupInfo = Utility.GroupTextAlpha(Software);
                break;
            case ColumnType.Custom:
                groupInfo = Utility.GroupTextAlpha(SubItems[column].Text);
                break;
            case ColumnType.ISOSpeed:
                groupInfo = new Utility.Tuple<int, string>(ISOSpeed, ISOSpeed.ToString());
                break;
            case ColumnType.Rating:
                groupInfo = new Utility.Tuple<int, string>(Rating / 5, (Rating / 5).ToString());
                break;
            case ColumnType.FocalLength:
                groupInfo = new Utility.Tuple<int, string>((int)FocalLength, FocalLength.ToString());
                break;
            case ColumnType.ExposureTime:
                groupInfo = new Utility.Tuple<int, string>((int)ExposureTime, ExposureTime.ToString());
                break;
            case ColumnType.FNumber:
                groupInfo = new Utility.Tuple<int, string>((int)FNumber, FNumber.ToString());
                break;
            case ColumnType.Resolution:
                groupInfo = new Utility.Tuple<int, string>((int)Resolution.Width, Resolution.Width.ToString());
                break;
            default:
                groupInfo = new Utility.Tuple<int, string>(0, "Unknown");
                break;
        }

        groupOrder = groupInfo.Item1;
        group = groupInfo.Item2;
    }
    /// <summary>
    /// Returns a path string to be used for extracting the shell icon
    /// of the item. Returns the filename for icon files and executables,
    /// file extension for other files.
    /// </summary>
    private string PathForShellIcon()
    {
        return mImageListView != null && mImageListView.ShellIconFromFileContent &&
            (string.Compare(extension, ".ico", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(extension, ".exe", StringComparison.OrdinalIgnoreCase) == 0)
            ? mFileName
            : extension;
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
        ImageListViewItem item = new();

        item.mText = mText;

        // File info
        item.extension = extension;
        item.mDateAccessed = mDateAccessed;
        item.mDateCreated = mDateCreated;
        item.mDateModified = mDateModified;
        item.mFileName = mFileName;
        item.mFilePath = mFilePath;
        item.mFileSize = mFileSize;

        // Image info
        item.mDimensions = mDimensions;
        item.mResolution = mResolution;

        // Exif tags
        item.mImageDescription = mImageDescription;
        item.mEquipmentModel = mEquipmentModel;
        item.mDateTaken = mDateTaken;
        item.mArtist = mArtist;
        item.mCopyright = mCopyright;
        item.mExposureTime = mExposureTime;
        item.mFNumber = mFNumber;
        item.mISOSpeed = mISOSpeed;
        item.mUserComment = mUserComment;
        item.mRating = mRating;
        item.mStarRating = mStarRating;
        item.mSoftware = mSoftware;
        item.mFocalLength = mFocalLength;

        // Virtual item properties
        item.mAdaptor = mAdaptor;
        item.mVirtualItemKey = mVirtualItemKey;

        // Sub items
        foreach (KeyValuePair<string, ImageListViewSubItem> kv in mSubItems)
            ((IDictionary<string, ImageListViewSubItem>)item.mSubItems).Add(kv.Key, kv.Value);

        // Current thumbnail
        if (mImageListView != null)
        {
            item.clonedThumbnail = mImageListView.thumbnailCache.GetImage(Guid, mAdaptor, mVirtualItemKey, mImageListView.ThumbnailSize,
                mImageListView.UseEmbeddedThumbnails, mImageListView.AutoRotateThumbnails, true);
        }

        return item;
    }
    #endregion
}

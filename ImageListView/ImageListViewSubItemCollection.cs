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

/// <summary>
/// Represents a collection of sub items.
/// </summary>
public class ImageListViewSubItemCollection : IDictionary<string, ImageListViewSubItem>
{
    #region Member Variables
    // Property backing fields
    private readonly Dictionary<string, ImageListViewSubItem> mItems;
    private ImageListViewItem mParent;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the collection of keys in the dictionary.
    /// </summary>
    [Category("Behavior"), Browsable(false), Description("Gets the collection of keys in the dictionary.")]
    public ICollection<string> Keys => ((IDictionary<string, ImageListViewSubItem>)mItems).Keys;
    /// <summary>
    /// Gets the collection of values in the dictionary.
    /// </summary>
    [Category("Behavior"), Browsable(false), Description("Gets the collection of values in the dictionary.")]
    public ICollection<ImageListViewSubItem> Values => ((IDictionary<string, ImageListViewSubItem>)mItems).Values;
    /// <summary>
    /// Gets the number of columns in the collection.
    /// </summary>
    [Category("Behavior"), Browsable(false), Description("Gets the number of columns in the collection.")]
    public int Count => mItems.Count;
    /// <summary>
    /// Gets a value indicating whether the Collection is read-only.
    /// </summary>
    [Category("Behavior"), Browsable(false), Description("Gets a value indicating whether the Collection is read-only.")]
    public bool IsReadOnly => ((IDictionary<string, ImageListViewSubItem>)mItems).IsReadOnly;
    /// <summary>
    /// Gets or sets a sub item.
    /// </summary>
    [Category("Appearance"), Browsable(true), Description("Gets or sets a sub item.")]
    public ImageListViewSubItem this[string key]
    {
        get
        {
            return mItems.TryGetValue(key, out ImageListViewSubItem subItem) ? subItem : GetDefaultItem();
        }
        set
        {
            mItems[key] = value;
            RefreshControl();
        }
    }
    /// <summary>
    /// Gets the sub item corresponding to the specified column type.
    /// </summary>
    [Category("Appearance"), Browsable(true), Description("Gets a sub item.")]
    public ImageListViewSubItem this[ColumnType type]
    {
        get
        {
            return new ImageListViewSubItem(mParent, mParent.GetSubItemText(type));
        }
    }
    /// <summary>
    /// Gets the sub item corresponding to the specified column.
    /// </summary>
    [Category("Appearance"), Browsable(true), Description("Gets a sub item.")]
    public ImageListViewSubItem this[ImageListView.ImageListViewColumnHeader column]
    {
        get
        {
            return column.Type == ColumnType.Custom ? this[column.Key] : this[column.Type];
        }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageListViewSubItemCollection"/> class.
    /// </summary>
    /// <param name="parent">The parent item.</param>
    public ImageListViewSubItemCollection(ImageListViewItem parent)
    {
        mParent = parent;
        mItems = new Dictionary<string, ImageListViewSubItem>();
    }
    #endregion

    #region Instance Methods
    /// <summary>
    /// Determines whether the collection contains an item with the given key.
    /// </summary>
    /// <param name="key">Item key.</param>
    /// <returns>true if the collection contains an element with the specified key; otherwise, false.</returns>
    public bool ContainsKey(string key)
    {
        return mItems.ContainsKey(key);
    }
    /// <summary>
    /// Adds an item to the collection.
    /// </summary>
    /// <param name="key">Item key.</param>
    /// <param name="text">The text of the sub item.</param>
    public void Add(string key, string text)
    {
        mItems.Add(key, new ImageListViewSubItem(mParent, text));
        RefreshControl();
    }
    /// <summary>
    /// Removes all items from the collection.
    /// </summary>
    public void Clear()
    {
        mItems.Clear();
        RefreshControl();
    }
    /// <summary>
    /// Returns an enumerator to use to iterate through columns.
    /// </summary>
    /// <returns>An enumerator that represents the item collection.</returns>
    public IEnumerator<KeyValuePair<string, ImageListViewSubItem>> GetEnumerator()
    {
        return ((IDictionary<string, ImageListViewSubItem>)mItems).GetEnumerator();
    }
    /// <summary>
    /// Returns an enumerator to use to iterate through columns.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> that represents the item collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IDictionary<string, ImageListViewSubItem>)mItems).GetEnumerator();
    }
    /// <summary>
    /// Removes the item with the given key from the collection.
    /// </summary>
    /// <param name="key">Item key.</param>
    public bool Remove(string key)
    {
        return mItems.Remove(key);
    }
    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">Item key.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key.</param>
    /// <returns>true if the collection contains an element with the specified key; otherwise, false.</returns>
    public bool TryGetValue(string key, out ImageListViewSubItem value)
    {
        return mItems.TryGetValue(key, out value);
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Refreshes the parent control.
    /// </summary>
    private void RefreshControl()
    {
        if (mParent != null && mParent.ImageListView != null && mParent.ImageListView.IsItemVisible(mParent.Guid))
            mParent.ImageListView.Refresh();
    }
    /// <summary>
    /// Returns a default item.
    /// </summary>
    /// <returns>A default item.</returns>
    private ImageListViewSubItem GetDefaultItem()
    {
        return new ImageListViewSubItem(mParent, "");
    }
    #endregion

    #region Unsupported Interface
    /// <summary>
    /// Adds an item to the collection.
    /// </summary>
    /// <param name="key">Item key.</param>
    /// <param name="value">The object to add to the collection.</param>
    void IDictionary<string, ImageListViewSubItem>.Add(string key, ImageListViewSubItem value)
    {
        mItems.Add(key, value);
        RefreshControl();
    }
    /// <summary>
    /// Adds an item to the collection.
    /// </summary>
    /// <param name="item">The item to add to the collection.</param>
    void ICollection<KeyValuePair<string, ImageListViewSubItem>>.Add(KeyValuePair<string, ImageListViewSubItem> item)
    {
        ((IDictionary<string, ImageListViewSubItem>)mItems).Add(item);
        RefreshControl();
    }
    /// <summary>
    /// Determines whether the collection contains the given item.
    /// </summary>
    /// <param name="item">The item to search for.</param>
    /// <returns>true if the collection contains the gvien element ; otherwise, false.</returns>
    bool ICollection<KeyValuePair<string, ImageListViewSubItem>>.Contains(KeyValuePair<string, ImageListViewSubItem> item)
    {
        return ((IDictionary<string, ImageListViewSubItem>)mItems).Contains(item);
    }
    /// <summary>
    /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
    void ICollection<KeyValuePair<string, ImageListViewSubItem>>.CopyTo(KeyValuePair<string, ImageListViewSubItem>[] array, int arrayIndex)
    {
        ((IDictionary<string, ImageListViewSubItem>)mItems).CopyTo(array, arrayIndex);
    }
    /// <summary>
    /// Removes the given item from the collection.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    bool ICollection<KeyValuePair<string, ImageListViewSubItem>>.Remove(KeyValuePair<string, ImageListViewSubItem> item)
    {
        return ((IDictionary<string, ImageListViewSubItem>)mItems).Remove(item);
    }
    #endregion
}

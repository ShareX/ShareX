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

namespace ShareX.ImageListView;

/// <summary>
/// Represents a sub item in the image list view.
/// </summary>
public class ImageListViewSubItem
{
    #region Member Variables
    // Property backing fields
    private string mText;
    private ImageListViewItem mParent;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the text associated with this sub item.
    /// </summary>
    [Category("Appearance"), Browsable(true), Description("Gets or sets the text associated with this sub item.")]
    public string Text
    {
        get
        {
            return mText;
        }
        set
        {
            mText = value;
            if (mParent != null && mParent.ImageListView != null && mParent.ImageListView.IsItemVisible(mParent.Guid))
                mParent.ImageListView.Refresh();
        }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageListViewSubItem"/> class.
    /// </summary>
    /// <param name="parent">The parent item.</param>
    /// <param name="text">Sub item text.</param>
    public ImageListViewSubItem(ImageListViewItem parent, string text)
    {
        mParent = parent;
        mText = text;
    }
    #endregion
}

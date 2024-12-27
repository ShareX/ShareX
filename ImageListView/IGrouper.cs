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
    /// Represents a class that supplies group information for items.
    /// </summary>
    public interface IGrouper
    {
        #region Instance Methods
        /// <summary>
        /// Supplies grouping information for the given item.
        /// </summary>
        /// <param name="item">The item that requesting grouping information.</param>
        GroupInfo GetGroupInfo(ImageListViewItem item);
        #endregion
    }

    /// <summary>
    /// Represents ordering and name information for a group.
    /// </summary>
    public struct GroupInfo
    {
        #region Properties
        /// <summary>
        /// The order of group. A group with a lower order number will be displayed above other groups
        /// when sorted in ascending order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The display name of the group. Items with the same group name will be collected and displayed 
        /// under the same group.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the GroupInfo class.
        /// </summary>
        /// <param name="name">Display name of the group.</param>
        /// <param name="order">Order of the group.</param>
        public GroupInfo(string name, int order)
        {
            Name = name;
            Order = order;
        }
        #endregion
    }
}

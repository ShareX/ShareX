#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using Newtonsoft.Json;
using ShareX.HelpersLib;
using System.ComponentModel;
using System.Drawing.Design;

namespace ShareX.IndexerLib
{
    public class IndexerSettings
    {
        [Category("Indexer"), DefaultValue(IndexerOutput.Html), Description("Indexer output type."), TypeConverter(typeof(EnumDescriptionConverter))]
        public IndexerOutput Output { get; set; }

        [Category("Indexer"), DefaultValue(true), Description("Don't index hidden folders.")]
        public bool SkipHiddenFolders { get; set; }

        [Category("Indexer"), DefaultValue(true), Description("Don't index hidden files.")]
        public bool SkipHiddenFiles { get; set; }

        [Category("Indexer"), DefaultValue(0), Description("Maximum folder depth level for indexing. 0 means unlimited.")]
        public int MaxDepthLevel { get; set; }

        [Category("Indexer"), DefaultValue(true), Description("Write folder and file size.")]
        public bool ShowSizeInfo { get; set; }

        [Category("Indexer"), DefaultValue(true), Description("Add footer information to show application and generated time.")]
        public bool AddFooter { get; set; }

        [Category("Indexer / Text"), DefaultValue("|___"), Description("Padding text to show indentation in the folder hierarchy.")]
        public string IndentationText { get; set; }

        [Category("Indexer / Text"), DefaultValue(false), Description("Adds empty line after folders.")]
        public bool AddEmptyLineAfterFolders { get; set; }

        [Category("Indexer / HTML"), DefaultValue(false), Description("Use custom Cascading Style Sheet file.")]
        public bool UseCustomCSSFile { get; set; }

        [Category("Indexer / HTML"), DefaultValue(false), Description("Display the path for each subfolder.")]
        public bool DisplayPath { get; set; }

        [Category("Indexer / HTML"), DefaultValue(false), Description("Limit the display path to the selected root folder. Must have DisplayPath enabled.")]
        public bool DisplayPathLimited { get; set; }

        [Category("Indexer / HTML"), DefaultValue(""), Description("Custom Cascading Style Sheet file path."), Editor(typeof(CssFileNameEditor), typeof(UITypeEditor))]
        public string CustomCSSFilePath { get; set; }

        [Category("Indexer / XML"), DefaultValue(true), Description("Folder/File information (name, size etc.) will be written as attribute.")]
        public bool UseAttribute { get; set; }

        [Category("Indexer / JSON"), DefaultValue(true), Description("Creates parseable but longer json output.")]
        public bool CreateParseableJson { get; set; }

        [JsonIgnore]
        public bool BinaryUnits;

        public IndexerSettings()
        {
            this.ApplyDefaultPropertyValues();
        }
    }
}
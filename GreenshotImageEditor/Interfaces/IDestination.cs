/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Greenshot.Plugin
{
    public class ExportInformation
    {
        private string uri = null;
        private string filepath = null;

        private bool exportMade = false;
        private string destinationDesignation = null;
        private string destinationDescription = null;

        private string errorMessage = null;

        public ExportInformation(string destinationDesignation, string destinationDescription)
        {
            this.destinationDesignation = destinationDesignation;
            this.destinationDescription = destinationDescription;
        }

        public ExportInformation(string destinationDesignation, string destinationDescription, bool exportMade)
            : this(destinationDesignation, destinationDescription)
        {
            this.exportMade = exportMade;
        }

        public string DestinationDesignation
        {
            get
            {
                return destinationDesignation;
            }
        }
        public string DestinationDescription
        {
            get
            {
                return destinationDescription;
            }
            set
            {
                destinationDescription = value;
            }
        }

        public bool ExportMade
        {
            get
            {
                return exportMade;
            }
            set
            {
                exportMade = value;
            }
        }

        public string Uri
        {
            get
            {
                return uri;
            }
            set
            {
                uri = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
            set
            {
                errorMessage = value;
            }
        }

        public string Filepath
        {
            get
            {
                return filepath;
            }
            set
            {
                filepath = value;
            }
        }
    }

    /// <summary>
    /// Description of IDestination.
    /// </summary>
    public interface IDestination : IDisposable, IComparable
    {
        /// <summary>
        /// Simple "designation" like "File", "Editor" etc, used to store the configuration
        /// </summary>
        string Designation
        {
            get;
        }

        /// <summary>
        /// Description which will be shown in the settings form, destination picker etc
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// Priority, used for sorting
        /// </summary>
        int Priority
        {
            get;
        }

        /// <summary>
        /// Gets an icon for the destination
        /// </summary>
        Image DisplayIcon
        {
            get;
        }

        /// <summary>
        /// Returns if the destination is active
        /// </summary>
        bool isActive
        {
            get;
        }

        /// <summary>
        /// Return a menu item
        /// </summary>
        /// <param name="addDynamics">Resolve the dynamic destinations too?</param>
        /// <param name="ContextMenuStrip">The menu for which the item is created</param>
        /// <param name="destinationClickHandler">Handler which is called when clicked</param>
        /// <returns>ToolStripMenuItem</returns>
        ToolStripMenuItem GetMenuItem(bool addDynamics, ContextMenuStrip menu, EventHandler destinationClickHandler);

        /// <summary>
        /// Gets the ShortcutKeys for the Editor
        /// </summary>
        Keys EditorShortcutKeys
        {
            get;
        }

        /// <summary>
        /// Gets the dynamic destinations
        /// </summary>
        IEnumerable<IDestination> DynamicDestinations();

        /// <summary>
        /// Returns true if this destination can be dynamic
        /// </summary>
        bool isDynamic
        {
            get;
        }

        /// <summary>
        /// Returns if the destination is active
        /// </summary>
        bool useDynamicsOnly
        {
            get;
        }

        /// <summary>
        /// Returns true if this destination returns a link
        /// </summary>
        bool isLinkable
        {
            get;
        }

        /// <summary>
        /// If a capture is made, and the destination is enabled, this method is called.
        /// </summary>
        /// <param name="manuallyInitiated">true if the user selected this destination from a GUI, false if it was called as part of a process</param>
        /// <param name="surface"></param>
        /// <param name="captureDetails"></param>
        /// <returns>DestinationExportInformation with information, like if the destination has "exported" the capture</returns>
        ExportInformation ExportCapture(bool manuallyInitiated, ISurface surface, ICaptureDetails captureDetails);
    }
}
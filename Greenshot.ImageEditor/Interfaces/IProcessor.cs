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

namespace Greenshot.Plugin
{
    /// <summary>
    /// Description of IProcessor.
    /// </summary>
    public interface IProcessor : IDisposable, IComparable
    {
        /// <summary>
        /// Simple "designation" like "FixTitle"
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
        /// Returns if the destination is active
        /// </summary>
        bool isActive
        {
            get;
        }

        /// <summary>
        /// If a capture is made, and the destination is enabled, this method is called.
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="captureDetails"></param>
        /// <returns>true if the processor has "processed" the capture</returns>
        bool ProcessCapture(ISurface surface, ICaptureDetails captureDetails);
    }
}
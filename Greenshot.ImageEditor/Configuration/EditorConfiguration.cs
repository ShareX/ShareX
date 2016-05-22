/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
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

using Greenshot.Core;
using Greenshot.Drawing.Fields;
using Greenshot.IniFile;
using GreenshotPlugin.UnmanagedHelpers;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Greenshot.Configuration
{
    /// <summary>
    /// Description of CoreConfiguration.
    /// </summary>
    [IniSection("Editor", Description = "Greenshot editor configuration")]
    public class EditorConfiguration : IniSection
    {
        [IniProperty("RecentColors", Separator = "|", Description = "Last used colors")]
        public List<Color> RecentColors;

        [IniProperty("LastFieldValue", Separator = "|", Description = "Field values, make sure the last used settings are re-used")]
        public Dictionary<string, object> LastUsedFieldValues;

        [IniProperty("MatchSizeToCapture", Description = "Match the editor window size to the capture", DefaultValue = "True")]
        public bool MatchSizeToCapture;
        [IniProperty("WindowPlacementFlags", Description = "Placement flags", DefaultValue = "0")]
        public WindowPlacementFlags WindowPlacementFlags;
        [IniProperty("WindowShowCommand", Description = "Show command", DefaultValue = "Normal")]
        public ShowWindowCommand ShowWindowCommand;
        [IniProperty("WindowMinPosition", Description = "Position of minimized window", DefaultValue = "-1,-1")]
        public Point WindowMinPosition;
        [IniProperty("WindowMaxPosition", Description = "Position of maximized window", DefaultValue = "-1,-1")]
        public Point WindowMaxPosition;
        [IniProperty("WindowNormalPosition", Description = "Position of normal window", DefaultValue = "100,100,400,400")]
        public Rectangle WindowNormalPosition;
        [IniProperty("ReuseEditor", Description = "Reuse already open editor", DefaultValue = "false")]
        public bool ReuseEditor;
        [IniProperty("FreehandSensitivity", Description = "The smaller this number, the less smoothing is used. Decrease for detailed drawing, e.g. when using a pen. Increase for smoother lines. e.g. when you want to draw a smooth line.", DefaultValue = "3")]
        public int FreehandSensitivity;
        [IniProperty("SuppressSaveDialogAtClose", Description = "Suppressed the 'do you want to save' dialog when closing the editor.", DefaultValue = "False")]
        public bool SuppressSaveDialogAtClose;

        [IniProperty("DropShadowEffectSettings", Description = "Settings for the drop shadow effect.")]
        public DropShadowEffect DropShadowEffectSettings;

        [IniProperty("TornEdgeEffectSettings", Description = "Settings for the torn edge effect.")]
        public TornEdgeEffect TornEdgeEffectSettings;

        public override void AfterLoad()
        {
            base.AfterLoad();
            if (RecentColors == null)
            {
                RecentColors = new List<Color>();
            }
        }

        /// <param name="requestingType">Type of the class for which to create the field</param>
        /// <param name="fieldType">FieldType of the field to construct</param>
        /// <param name="preferredDefaultValue">FieldType of the field to construct</param>
        /// <returns>a new Field of the given fieldType, with the scope of it's value being restricted to the Type scope</returns>
        public Field CreateField(Type requestingType, FieldType fieldType, object preferredDefaultValue)
        {
            string requestingTypeName = requestingType.Name;
            string requestedField = requestingTypeName + "." + fieldType.Name;
            object fieldValue = preferredDefaultValue;

            // Check if the configuration exists
            if (LastUsedFieldValues == null)
            {
                LastUsedFieldValues = new Dictionary<string, object>();
            }

            // Check if settings for the requesting type exist, if not create!
            if (LastUsedFieldValues.ContainsKey(requestedField))
            {
                // Check if a value is set (not null)!
                if (LastUsedFieldValues[requestedField] != null)
                {
                    fieldValue = LastUsedFieldValues[requestedField];
                }
                else
                {
                    // Overwrite null value
                    LastUsedFieldValues[requestedField] = fieldValue;
                }
            }
            else
            {
                LastUsedFieldValues.Add(requestedField, fieldValue);
            }
            Field returnField = new Field(fieldType, requestingType);
            returnField.Value = fieldValue;
            return returnField;
        }

        public void UpdateLastFieldValue(Field field)
        {
            string requestedField = field.Scope + "." + field.FieldType.Name;
            // Check if the configuration exists
            if (LastUsedFieldValues == null)
            {
                LastUsedFieldValues = new Dictionary<string, object>();
            }
            // check if settings for the requesting type exist, if not create!
            if (LastUsedFieldValues.ContainsKey(requestedField))
            {
                LastUsedFieldValues[requestedField] = field.myValue;
            }
            else
            {
                LastUsedFieldValues.Add(requestedField, field.myValue);
            }
        }

        public WindowPlacement GetEditorPlacement()
        {
            WindowPlacement placement = WindowPlacement.Default;
            placement.NormalPosition = new RECT(WindowNormalPosition);
            placement.MaxPosition = new POINT(WindowMaxPosition);
            placement.MinPosition = new POINT(WindowMinPosition);
            placement.ShowCmd = ShowWindowCommand;
            placement.Flags = WindowPlacementFlags;
            return placement;
        }

        public void SetEditorPlacement(WindowPlacement placement)
        {
            WindowNormalPosition = placement.NormalPosition.ToRectangle();
            WindowMaxPosition = placement.MaxPosition.ToPoint();
            WindowMinPosition = placement.MinPosition.ToPoint();
            ShowWindowCommand = placement.ShowCmd;
            WindowPlacementFlags = placement.Flags;
        }
    }
}
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

using System;
using System.ComponentModel;

namespace Greenshot.Drawing.Fields
{
    /// <summary>
    /// Represents a single field of a drawable element, i.e.
    /// line thickness of a rectangle.
    /// </summary>
    [Serializable]
    public class Field : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public object myValue;
        public object Value
        {
            get
            {
                return myValue;
            }
            set
            {
                if (!Equals(myValue, value))
                {
                    myValue = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                    }
                }
            }
        }
        public FieldType FieldType;
        public string Scope;

        /// <summary>
        /// Constructs a new Field instance, usually you should be using FieldFactory
        /// to create Fields.
        /// </summary>
        /// <param name="fieldType">FieldType of the Field to be created</param>
        /// <param name="scope">The scope to which the value of this Field is relevant.
        /// Depending on the scope the Field's value may be shared for other elements
        /// containing the same FieldType for defaulting to the last used value.
        /// When scope is set to a Type (e.g. typeof(RectangleContainer)), its value
        /// should not be reused for FieldHolders of another Type (e.g. typeof(EllipseContainer))
        /// </param>
        public Field(FieldType fieldType, Type scope)
        {
            FieldType = fieldType;
            Scope = scope.Name;
        }

        public Field(FieldType fieldType, string scope)
        {
            FieldType = fieldType;
            Scope = scope;
        }

        public Field(FieldType fieldType)
        {
            FieldType = fieldType;
        }

        /// <summary>
        /// Returns true if this field holds a value other than null.
        /// </summary>
        public bool HasValue
        {
            get { return Value != null; }
        }

        /// <summary>
        /// Creates a flat clone of this Field. The fields value itself is not cloned.
        /// </summary>
        /// <returns></returns>
        public Field Clone()
        {
            Field ret = new Field(FieldType, Scope);
            ret.Value = Value;
            return ret;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            unchecked
            {
                hashCode += 1000000009 * FieldType.GetHashCode();
                if (Scope != null)
                    hashCode += 1000000021 * Scope.GetHashCode();
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            Field other = obj as Field;
            if (other == null)
            {
                return false;
            }
            return FieldType == other.FieldType && Equals(Scope, other.Scope);
        }

        public override string ToString()
        {
            return string.Format("[Field FieldType={1} Value={0} Scope={2}]", myValue, FieldType, Scope);
        }
    }

    /// <summary>
    /// EventHandler to be used when a field value changes
    /// </summary>
    public delegate void FieldChangedEventHandler(object sender, FieldChangedEventArgs e);

    /// <summary>
    /// EventArgs to be used with FieldChangedEventHandler
    /// </summary>
    public class FieldChangedEventArgs : EventArgs
    {
        public readonly Field Field;

        public FieldChangedEventArgs(Field field)
        {
            Field = field;
        }
    }
}
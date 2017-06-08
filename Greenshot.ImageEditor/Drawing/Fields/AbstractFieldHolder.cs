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

using Greenshot.Configuration;
using Greenshot.IniFile;
using GreenshotPlugin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Greenshot.Drawing.Fields
{
    /// <summary>
    /// Basic IFieldHolder implementation, providing access to a set of fields
    /// </summary>
    [Serializable]
    public abstract class AbstractFieldHolder : IFieldHolder
    {
        private static readonly EditorConfiguration editorConfiguration = IniConfig.GetIniSection<EditorConfiguration>();

        /// <summary>
        /// called when a field's value has changed
        /// </summary>
        [NonSerialized]
        private FieldChangedEventHandler fieldChanged;
        public event FieldChangedEventHandler FieldChanged
        {
            add { fieldChanged += value; }
            remove { fieldChanged -= value; }
        }

        // we keep two Collections of our fields, dictionary for quick access, list for serialization
        // this allows us to use default serialization
        [NonSerialized]
        private Dictionary<FieldType, Field> fieldsByType = new Dictionary<FieldType, Field>();
        private readonly List<Field> fields = new List<Field>();
        
        [NonSerialized]
        private Dictionary<Field, PropertyChangedEventHandler> _handlers = new Dictionary<Field, PropertyChangedEventHandler>();

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            fieldsByType = new Dictionary<FieldType, Field>();
            // listen to changing properties
            foreach (Field field in fields)
            {
                PropertyChangedEventHandler handler = delegate
                {
                    if (fieldChanged != null)
                    {
                        fieldChanged(this, new FieldChangedEventArgs(field));
                    }
                };

                _handlers.Add(field, handler);
                field.PropertyChanged += handler;
                fieldsByType[field.FieldType] = field;
            }
        }

        public void AddField(Type requestingType, FieldType fieldType, object fieldValue)
        {
            AddField(editorConfiguration.CreateField(requestingType, fieldType, fieldValue));
        }

        public virtual void AddField(Field field)
        {
            if (fieldsByType != null && fieldsByType.ContainsKey(field.FieldType))
            {
                if (LOG.IsDebugEnabled)
                {
                    LOG.DebugFormat("A field with of type '{0}' already exists in this {1}, will overwrite.", field.FieldType, GetType());
                }
            }

            fields.Add(field);
            fieldsByType[field.FieldType] = field;

            PropertyChangedEventHandler handler = delegate { if (fieldChanged != null) fieldChanged(this, new FieldChangedEventArgs(field)); };
            _handlers.Add(field, handler);
            field.PropertyChanged += handler;
        }

        public void RemoveField(Field field)
        {
            fields.Remove(field);
            fieldsByType.Remove(field.FieldType);

            PropertyChangedEventHandler handler;
            if (_handlers.TryGetValue(field, out handler))
            {
                field.PropertyChanged -= handler;
                _handlers.Remove(field);
            }
        }

        public List<Field> GetFields()
        {
            return fields;
        }

        public Field GetField(FieldType fieldType)
        {
            try
            {
                return fieldsByType[fieldType];
            }
            catch (KeyNotFoundException e)
            {
                throw new ArgumentException("Field '" + fieldType + "' does not exist in " + GetType(), e);
            }
        }

        public object GetFieldValue(FieldType fieldType)
        {
            return GetField(fieldType).Value;
        }

        #region convenience methods to save us some casts outside

        public string GetFieldValueAsString(FieldType fieldType)
        {
            return Convert.ToString(GetFieldValue(fieldType));
        }

        public int GetFieldValueAsInt(FieldType fieldType)
        {
            return Convert.ToInt32(GetFieldValue(fieldType));
        }

        public decimal GetFieldValueAsDecimal(FieldType fieldType)
        {
            return Convert.ToDecimal(GetFieldValue(fieldType));
        }

        public double GetFieldValueAsDouble(FieldType fieldType)
        {
            return Convert.ToDouble(GetFieldValue(fieldType));
        }

        public float GetFieldValueAsFloat(FieldType fieldType)
        {
            return Convert.ToSingle(GetFieldValue(fieldType));
        }

        public bool GetFieldValueAsBool(FieldType fieldType)
        {
            return Convert.ToBoolean(GetFieldValue(fieldType));
        }

        public Color GetFieldValueAsColor(FieldType fieldType)
        {
            return (Color)GetFieldValue(fieldType);
        }

        #endregion convenience methods to save us some casts outside

        public bool HasField(FieldType fieldType)
        {
            return fieldsByType.ContainsKey(fieldType);
        }

        public bool HasFieldValue(FieldType fieldType)
        {
            return HasField(fieldType) && fieldsByType[fieldType].HasValue;
        }

        public void SetFieldValue(FieldType fieldType, object value)
        {
            try
            {
                fieldsByType[fieldType].Value = value;
            }
            catch (KeyNotFoundException e)
            {
                throw new ArgumentException("Field '" + fieldType + "' does not exist in " + GetType(), e);
            }
        }

        protected void OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            if (fieldChanged != null)
            {
                fieldChanged(sender, e);
            }
        }
    }
}
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
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace ShareX.ImageListView;

/// <summary>
/// Represents the type converter for the column headers of the image list view.
/// </summary>
internal class ImageListViewColumnHeaderTypeConverter : TypeConverter
{
    #region TypeConverter Overrides
    /// <summary>
    /// Returns whether this converter can convert the 
    /// object to the specified type, using the specified context.
    /// </summary>
    /// <param name="context">Format context.</param>
    /// <param name="destinationType">The type you want to convert to.</param>
    /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
    /// <summary>
    /// Converts the given value object to the specified type, 
    /// using the specified context and culture information.
    /// </summary>
    /// <param name="context">Format context.</param>
    /// <param name="culture">The culture info. If null is passed, the current culture is assumed.</param>
    /// <param name="value">The objct to convert.</param>
    /// <param name="destinationType">The type to convert to.</param>
    /// <returns>An object that represents the converted value.</returns>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(InstanceDescriptor) && value is ImageListView.ImageListViewColumnHeader col)
        {
            Type ? t = TypeDescriptor.GetReflectionType(value);
            ConstructorInfo ? ctor;
            InstanceDescriptor ? id = null;

            if (col.Type == ColumnType.Custom)
            {
                ctor = t.GetConstructor([
                        typeof(ColumnType), typeof(string), typeof(string), typeof(int), typeof(int), typeof(bool)
                    ]);

                if (ctor != null)
                {
                    id = new InstanceDescriptor(ctor, new object[] {
                        col.Type, col.Key, col.Text, col.Width, col.DisplayIndex, col.Visible
                    }, false);
                }
            }

            if (id == null && col.Type != ColumnType.Custom)
            {
                ctor = t.GetConstructor([
                        typeof(ColumnType), typeof(string), typeof(int), typeof(int), typeof(bool)
                    ]);

                if (ctor != null)
                {
                    id = new InstanceDescriptor(ctor, new object[] {
                        col.Type, col.Text, col.Width, col.DisplayIndex, col.Visible
                    }, false);
                }
            }

            if (id == null)
            {
                ctor = t.GetConstructor([]);
                return ctor != null
                    ? (object)new InstanceDescriptor(ctor, new object[0], false)
                    : throw new ArgumentException("No default parameterless constructor exists for ImageListViewColumnHeader.");
            }
            return id;
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
    #endregion
}

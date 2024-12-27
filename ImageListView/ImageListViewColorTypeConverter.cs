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
/// Represents type converter for the color palette of the image list view.
/// </summary>
internal class ImageListViewColorTypeConverter : ExpandableObjectConverter
{
    #region TypeConverter Overrides
    /// <summary>
    /// Returns whether this converter can convert the 
    /// object to the specified type, using the specified context.
    /// </summary>
    /// <param name="context">Format context.</param>
    /// <param name="destinationType">The type you want to convert to.</param>
    /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        if (destinationType == typeof(string))
            return true;
        else if (destinationType == typeof(InstanceDescriptor))
            return true;

        return base.CanConvertTo(context, destinationType);
    }
    /// <summary>
    /// Returns whether this converter can convert an object of the given type 
    /// to the type of this converter, using the specified context.
    /// </summary>
    /// <param name="context">Format context.</param>
    /// <param name="sourceType">The type you want to convert from.</param>
    /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    /// <summary>
    /// Converts the given value object to the specified type, 
    /// using the specified context and culture information.
    /// </summary>
    /// <param name="context">Format context.</param>
    /// <param name="culture">The culture info. If null is passed, the current culture is assumed.</param>
    /// <param name="value">The object to convert.</param>
    /// <param name="destinationType">The type to convert to.</param>
    /// <returns>An object that represents the converted value.</returns>
    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value != null && value is ImageListViewColor color)
        {
            ImageListViewColor colors = color;

            if (destinationType == typeof(string))
            {
                return colors.ToString();
            } else if (destinationType == typeof(InstanceDescriptor))
            {
                // Used by the designer serializer
                ConstructorInfo consInfo = typeof(ImageListViewColor).GetConstructor([typeof(string)]);
                return new InstanceDescriptor(consInfo, new object[] { colors.ToString() });
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
    /// <summary>
    /// Converts the given object to the type of this converter, 
    /// using the specified context and culture information.
    /// </summary>
    /// <param name="context">Format context.</param>
    /// <param name="culture">The culture info to use as the current culture.</param>
    /// <param name="value">The object to convert.</param>
    /// <returns>An object that represents the converted value.</returns>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) => value is not null and string ? new ImageListViewColor((string)value) : base.ConvertFrom(context, culture, value ?? new());
    #endregion
}

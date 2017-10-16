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

using GreenshotPlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace Greenshot.IniFile
{
    /// <summary>
    /// A container to be able to pass the value from a IniSection around.
    /// </summary>
    public class IniValue
    {
        private readonly PropertyInfo propertyInfo;
        private readonly FieldInfo fieldInfo;
        private readonly IniSection containingIniSection;
        private readonly IniPropertyAttribute attributes;

        public IniValue(IniSection containingIniSection, PropertyInfo propertyInfo, IniPropertyAttribute iniPropertyAttribute)
        {
            this.containingIniSection = containingIniSection;
            this.propertyInfo = propertyInfo;
            attributes = iniPropertyAttribute;
        }

        public IniValue(IniSection containingIniSection, FieldInfo fieldInfo, IniPropertyAttribute iniPropertyAttribute)
        {
            this.containingIniSection = containingIniSection;
            this.fieldInfo = fieldInfo;
            attributes = iniPropertyAttribute;
        }

        /// <summary>
        /// Return true when the value is fixed
        /// </summary>
        public bool IsFixed
        {
            get
            {
                if (attributes != null)
                {
                    return attributes.FixedValue;
                }
                return false;
            }
            set
            {
                if (attributes != null)
                {
                    attributes.FixedValue = value;
                }
            }
        }

        public MemberInfo MemberInfo
        {
            get
            {
                if (propertyInfo == null)
                {
                    return fieldInfo;
                }
                else
                {
                    return propertyInfo;
                }
            }
        }

        /// <summary>
        /// Returns the IniSection this value is contained in
        /// </summary>
        public IniSection ContainingIniSection
        {
            get
            {
                return containingIniSection;
            }
        }

        /// <summary>
        /// Get the in the ini file defined attributes
        /// </summary>
        public IniPropertyAttribute Attributes
        {
            get
            {
                return attributes;
            }
        }

        /// <summary>
        /// Get the value for this IniValue
        /// </summary>
        public object Value
        {
            get
            {
                if (propertyInfo == null)
                {
                    return fieldInfo.GetValue(containingIniSection);
                }
                else
                {
                    return propertyInfo.GetValue(containingIniSection, null);
                }
            }
            set
            {
                if (propertyInfo == null)
                {
                    fieldInfo.SetValue(containingIniSection, value);
                }
                else
                {
                    propertyInfo.SetValue(containingIniSection, value, null);
                }
            }
        }

        /// <summary>
        /// Get the Type of the value
        /// </summary>
        public Type ValueType
        {
            get
            {
                Type valueType = null;
                if (propertyInfo == null)
                {
                    valueType = fieldInfo.FieldType;
                }
                else
                {
                    valueType = propertyInfo.PropertyType;
                }
                if (valueType.IsGenericType && valueType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    // We are dealing with a generic type that is nullable
                    valueType = Nullable.GetUnderlyingType(valueType);
                }
                return valueType;
            }
        }

        /// <summary>
        /// Write the value to the text writer
        /// </summary>
        /// <param name="writer">TextWriter to write to</param>
        /// <param name="onlyProperties">true if we do not want the comment</param>
        public void Write(TextWriter writer, bool onlyProperties)
        {
            object myValue = Value;
            Type valueType = ValueType;
            if (myValue == null)
            {
                if (attributes.ExcludeIfNull)
                {
                    return;
                }
                if (attributes.DefaultValue != null)
                {
                    myValue = attributes.DefaultValue;
                    valueType = typeof(string);
                }
                else
                {
                    myValue = containingIniSection.GetDefault(attributes.Name);
                    if (myValue != null)
                    {
                        valueType = myValue.GetType();
                    }
                }
            }
            if (myValue == null)
            {
                if (attributes.ExcludeIfNull)
                {
                    return;
                }
            }
            if (!onlyProperties)
            {
                writer.WriteLine("; {0}", attributes.Description);
            }
            if (myValue == null)
            {
                writer.WriteLine("{0}=", attributes.Name);
                return;
            }
            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                // Handle dictionaries.
                Type valueType1 = valueType.GetGenericArguments()[0];
                Type valueType2 = valueType.GetGenericArguments()[1];
                // Get the methods we need to deal with dictionaries.
                var keys = valueType.GetProperty("Keys").GetValue(myValue, null);
                var item = valueType.GetProperty("Item");
                var enumerator = keys.GetType().GetMethod("GetEnumerator").Invoke(keys, null);
                var moveNext = enumerator.GetType().GetMethod("MoveNext");
                var current = enumerator.GetType().GetProperty("Current").GetGetMethod();
                // Get all the values.
                while ((bool)moveNext.Invoke(enumerator, null))
                {
                    var key = current.Invoke(enumerator, null);
                    var valueObject = item.GetValue(myValue, new[] { key });
                    // Write to ini file!
                    writer.WriteLine("{0}.{1}={2}", attributes.Name, ConvertValueToString(valueType1, key, attributes.Separator), ConvertValueToString(valueType2, valueObject, attributes.Separator));
                }
            }
            else
            {
                writer.WriteLine("{0}={1}", attributes.Name, ConvertValueToString(valueType, myValue, attributes.Separator));
            }
        }

        /// <summary>
        /// Set the value to the value in the ini file, or default
        /// </summary>
        /// <returns></returns>
        public void SetValueFromProperties(Dictionary<string, string> properties)
        {
            string propertyName = attributes.Name;
            string defaultValue = attributes.DefaultValue;
            string propertyValue = null;
            if (properties.ContainsKey(propertyName) && properties[propertyName] != null)
            {
                propertyValue = containingIniSection.PreCheckValue(propertyName, properties[propertyName]);
            }
            UseValueOrDefault(propertyValue);
        }

        /// <summary>
        /// This method will set the ini value to the supplied value or use the default if non supplied
        /// </summary>
        /// <param name="propertyValue"></param>
        public void UseValueOrDefault(string propertyValue)
        {
            Type valueType = ValueType;
            string propertyName = attributes.Name;
            string defaultValue = attributes.DefaultValue;
            bool defaultUsed = false;
            object defaultValueFromConfig = containingIniSection.GetDefault(propertyName);

            if (string.IsNullOrEmpty(propertyValue))
            {
                if (defaultValue != null && defaultValue.Trim().Length != 0)
                {
                    propertyValue = defaultValue;
                    defaultUsed = true;
                }
                else if (defaultValueFromConfig != null)
                {
                    LOG.DebugFormat("Default for Property {0} implemented!", propertyName);
                }
                else
                {
                    if (attributes.ExcludeIfNull)
                    {
                        Value = null;
                        return;
                    }
                    LOG.DebugFormat("Property {0} has no value or default value!", propertyName);
                }
            }
            // Now set the value
            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                // Logic for Dictionary<,>
                Type type1 = valueType.GetGenericArguments()[0];
                Type type2 = valueType.GetGenericArguments()[1];
                //LOG.Info(String.Format("Found Dictionary<{0},{1}>", type1.Name, type2.Name));
                object dictionary = Activator.CreateInstance(valueType);
                MethodInfo addMethodInfo = valueType.GetMethod("Add");
                bool addedElements = false;
                Dictionary<string, string> properties = IniConfig.PropertiesForSection(containingIniSection);
                foreach (string key in properties.Keys)
                {
                    if (key != null && key.StartsWith(propertyName + "."))
                    {
                        // What "key" do we need to store it under?
                        string subPropertyName = key.Substring(propertyName.Length + 1);
                        string stringValue = properties[key];
                        object newValue1 = null;
                        object newValue2 = null;
                        try
                        {
                            newValue1 = ConvertStringToValueType(type1, subPropertyName, attributes.Separator);
                        }
                        catch (Exception ex)
                        {
                            LOG.Warn(ex);
                            //LOG.Error("Problem converting " + subPropertyName + " to type " + type1.FullName, e);
                        }
                        try
                        {
                            newValue2 = ConvertStringToValueType(type2, stringValue, attributes.Separator);
                        }
                        catch (Exception ex)
                        {
                            LOG.Warn(ex);
                            //LOG.Error("Problem converting " + stringValue + " to type " + type2.FullName, e);
                        }
                        addMethodInfo.Invoke(dictionary, new[] { newValue1, newValue2 });
                        addedElements = true;
                    }
                }
                // No need to return something that isn't filled!
                if (addedElements)
                {
                    Value = dictionary;
                    return;
                }
                else if (defaultValueFromConfig != null)
                {
                    Value = defaultValueFromConfig;
                    return;
                }
            }
            else if (!string.IsNullOrEmpty(propertyValue))
            {
                if (valueType.IsGenericType && valueType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    // We are dealing with a generic type that is nullable
                    valueType = Nullable.GetUnderlyingType(valueType);
                }
                object newValue = null;
                try
                {
                    newValue = ConvertStringToValueType(valueType, propertyValue, attributes.Separator);
                }
                catch (Exception ex1)
                {
                    newValue = null;
                    if (!defaultUsed)
                    {
                        try
                        {
                            LOG.WarnFormat("Problem '{0}' while converting {1} to type {2} trying fallback...", ex1.Message, propertyValue, valueType.FullName);
                            newValue = ConvertStringToValueType(valueType, defaultValue, attributes.Separator);
                            ContainingIniSection.IsDirty = true;
                            LOG.InfoFormat("Used default value {0} for property {1}", defaultValue, propertyName);
                        }
                        catch (Exception ex2)
                        {
                            LOG.Warn("Problem converting fallback value " + defaultValue + " to type " + valueType.FullName, ex2);
                        }
                    }
                    else
                    {
                        LOG.Warn("Problem converting " + propertyValue + " to type " + valueType.FullName, ex1);
                    }
                }
                Value = newValue;
                return;
            }

            // If nothing is set, we can use the default value from the config (if we habe one)
            if (defaultValueFromConfig != null)
            {
                Value = defaultValueFromConfig;
                return;
            }
            if (ValueType != typeof(string))
            {
                try
                {
                    Value = Activator.CreateInstance(ValueType);
                }
                catch (Exception)
                {
                    LOG.WarnFormat("Couldn't create instance of {0} for {1}, using default value.", ValueType.FullName, attributes.Name);
                    Value = default(ValueType);
                }
            }
            else
            {
                Value = default(ValueType);
            }
        }

        /// <summary>
        /// Convert a string to a value of type "valueType"
        /// </summary>
        /// <param name="valueType">Type to convert tp</param>
        /// <param name="valueString">string to convert from</param>
        /// <returns>Value</returns>
        private static object ConvertStringToValueType(Type valueType, string valueString, string separator)
        {
            if (valueString == null)
            {
                return null;
            }
            if (valueType == typeof(string))
            {
                return valueString;
            }
            if (string.IsNullOrEmpty(valueString))
            {
                return null;
            }

            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(List<>))
            {
                string arraySeparator = separator;
                object list = Activator.CreateInstance(valueType);
                // Logic for List<>
                string[] arrayValues = valueString.Split(new[] { arraySeparator }, StringSplitOptions.None);
                if (arrayValues == null || arrayValues.Length == 0)
                {
                    return list;
                }
                MethodInfo addMethodInfo = valueType.GetMethod("Add");

                foreach (string arrayValue in arrayValues)
                {
                    if (arrayValue != null && arrayValue.Length > 0)
                    {
                        object newValue = null;
                        try
                        {
                            newValue = ConvertStringToValueType(valueType.GetGenericArguments()[0], arrayValue, separator);
                        }
                        catch (Exception ex)
                        {
                            LOG.Warn("Problem converting " + arrayValue + " to type " + valueType.FullName, ex);
                        }
                        if (newValue != null)
                        {
                            addMethodInfo.Invoke(list, new[] { newValue });
                        }
                    }
                }
                return list;
            }
            //LOG.Debug("No convertor for " + fieldType.ToString());
            if (valueType == typeof(object) && valueString.Length > 0)
            {
                //LOG.Debug("Parsing: " + valueString);
                string[] values = valueString.Split(new[] { ':' });
                //LOG.Debug("Type: " + values[0]);
                //LOG.Debug("Value: " + values[1]);
                Type fieldTypeForValue = Type.GetType(values[0], true);
                //LOG.Debug("Type after GetType: " + fieldTypeForValue);
                return ConvertStringToValueType(fieldTypeForValue, values[1], separator);
            }
            TypeConverter converter = TypeDescriptor.GetConverter(valueType);
            if (converter != null)
            {
                return converter.ConvertFromInvariantString(valueString);
            }
            else if (valueType.IsEnum)
            {
                if (valueString.Length > 0)
                {
                    try
                    {
                        return Enum.Parse(valueType, valueString);
                    }
                    catch (ArgumentException)
                    {
                        //LOG.InfoFormat("Couldn't match {0} to {1}, trying case-insentive match", valueString, fieldType);
                        foreach (Enum enumValue in Enum.GetValues(valueType))
                        {
                            if (enumValue.ToString().Equals(valueString, StringComparison.InvariantCultureIgnoreCase))
                            {
                                //LOG.Info("Match found...");
                                return enumValue;
                            }
                        }
                        throw;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Override of ToString which calls the ConvertValueToString
        /// </summary>
        /// <returns>string representation of this</returns>
        public override string ToString()
        {
            return ConvertValueToString(ValueType, Value, attributes.Separator);
        }

        /// <summary>
        /// Convert the supplied value to a string
        /// </summary>
        /// <param name="valueType">Type to convert</param>
        /// <param name="valueObject">Value to convert</param>
        /// <param name="separator">separator for lists</param>
        /// <returns>string representation of the value</returns>
        private static string ConvertValueToString(Type valueType, object valueObject, string separator)
        {
            if (valueObject == null)
            {
                // If there is nothing, deliver nothing!
                return "";
            }

            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(List<>))
            {
                StringBuilder stringBuilder = new StringBuilder();
                Type specificValueType = valueType.GetGenericArguments()[0];
                int listCount = (int)valueType.GetProperty("Count").GetValue(valueObject, null);
                // Loop though generic list
                for (int index = 0; index < listCount; index++)
                {
                    object item = valueType.GetMethod("get_Item").Invoke(valueObject, new object[] { index });

                    // Now you have an instance of the item in the generic list
                    if (index < listCount - 1)
                    {
                        stringBuilder.AppendFormat("{0}{1}", ConvertValueToString(specificValueType, item, separator), separator);
                    }
                    else
                    {
                        stringBuilder.AppendFormat("{0}", ConvertValueToString(specificValueType, item, separator));
                    }
                }
                return stringBuilder.ToString();
            }
            else if (valueType == typeof(object))
            {
                // object to String, this is the hardest
                // Format will be "FQTypename[,Assemblyname]:Value"

                // Get the type so we can call ourselves recursive
                Type objectType = valueObject.GetType();

                // Get the value as string
                string ourValue = ConvertValueToString(objectType, valueObject, separator);

                // Get the valuetype as string
                string valueTypeName = objectType.FullName;
                // Find the assembly name and only append it if it's not already in the fqtypename (like System.Drawing)
                string assemblyName = objectType.Assembly.FullName;
                // correct assemblyName, this also has version information etc.
                if (assemblyName.StartsWith("Green"))
                {
                    assemblyName = assemblyName.Substring(0, assemblyName.IndexOf(','));
                }
                return String.Format("{0},{1}:{2}", valueTypeName, assemblyName, ourValue);
            }
            else
            {
                TypeConverter converter = TypeDescriptor.GetConverter(valueType);
                if (converter != null)
                {
                    return converter.ConvertToInvariantString(valueObject);
                }
            }
            // All other types
            return valueObject.ToString();
        }
    }
}
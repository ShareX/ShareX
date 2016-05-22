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
using System.Reflection;

namespace Greenshot.Drawing.Fields.Binding
{
    /// <summary>
    /// Bidirectional binding of properties of two INotifyPropertyChanged instances.
    /// This implementation synchronizes null values, too. If you do not want this
    /// behavior (e.g. when binding to a
    /// </summary>
    public class BidirectionalBinding
    {
        private readonly INotifyPropertyChanged _controlObject;
        private readonly INotifyPropertyChanged _fieldObject;
        private readonly string _controlPropertyName;
        private readonly string _fieldPropertyName;
        private bool _updatingControl = false;
        private bool _updatingField = false;
        private IBindingConverter _converter;
        private readonly IBindingValidator _validator;

        /// <summary>
        /// Whether or not null values are passed on to the other object.
        /// </summary>
        protected bool AllowSynchronizeNull = true;

        /// <summary>
        /// Bind properties of two objects bidirectionally
        /// </summary>
        /// <param name="controlObject">Object containing 1st property to bind</param>
        /// <param name="controlPropertyName">Property of 1st object to bind</param>
        /// <param name="fieldObject">Object containing 2nd property to bind</param>
        /// <param name="fieldPropertyName">Property of 2nd object to bind</param>
        public BidirectionalBinding(INotifyPropertyChanged controlObject, string controlPropertyName, INotifyPropertyChanged fieldObject, string fieldPropertyName)
        {
            _controlObject = controlObject;
            _fieldObject = fieldObject;
            _controlPropertyName = controlPropertyName;
            _fieldPropertyName = fieldPropertyName;

            _controlObject.PropertyChanged += ControlPropertyChanged;
            _fieldObject.PropertyChanged += FieldPropertyChanged;
        }

        /// <summary>
        /// Bind properties of two objects bidirectionally, converting the values using a converter
        /// </summary>
        /// <param name="controlObject">Object containing 1st property to bind</param>
        /// <param name="controlPropertyName">Property of 1st object to bind</param>
        /// <param name="fieldObject">Object containing 2nd property to bind</param>
        /// <param name="fieldPropertyName">Property of 2nd object to bind</param>
        /// <param name="converter">taking care of converting the synchronized value to the correct target format and back</param>
        public BidirectionalBinding(INotifyPropertyChanged controlObject, string controlPropertyName, INotifyPropertyChanged fieldObject, string fieldPropertyName, IBindingConverter converter) : this(controlObject, controlPropertyName, fieldObject, fieldPropertyName)
        {
            _converter = converter;
        }

        /// <summary>
        /// Bind properties of two objects bidirectionally, converting the values using a converter.
        /// Synchronization can be intercepted by adding a validator.
        /// </summary>
        /// <param name="controlObject">Object containing 1st property to bind</param>
        /// <param name="controlPropertyName">Property of 1st object to bind</param>
        /// <param name="fieldObject">Object containing 2nd property to bind</param>
        /// <param name="fieldPropertyName">Property of 2nd object to bind</param>
        /// <param name="validator">validator to intercept synchronization if the value does not match certain criteria</param>
        public BidirectionalBinding(INotifyPropertyChanged controlObject, string controlPropertyName, INotifyPropertyChanged fieldObject, string fieldPropertyName, IBindingValidator validator) : this(controlObject, controlPropertyName, fieldObject, fieldPropertyName)
        {
            _validator = validator;
        }

        /// <summary>
        /// Bind properties of two objects bidirectionally, converting the values using a converter.
        /// Synchronization can be intercepted by adding a validator.
        /// </summary>
        /// <param name="controlObject">Object containing 1st property to bind</param>
        /// <param name="controlPropertyName">Property of 1st object to bind</param>
        /// <param name="fieldObject">Object containing 2nd property to bind</param>
        /// <param name="fieldPropertyName">Property of 2nd object to bind</param>
        /// <param name="converter">taking care of converting the synchronized value to the correct target format and back</param>
        /// <param name="validator">validator to intercept synchronization if the value does not match certain criteria</param>
        public BidirectionalBinding(INotifyPropertyChanged controlObject, string controlPropertyName, INotifyPropertyChanged fieldObject, string fieldPropertyName, IBindingConverter converter, IBindingValidator validator) : this(controlObject, controlPropertyName, fieldObject, fieldPropertyName, converter)
        {
            _validator = validator;
        }

        public void ControlPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_updatingControl && e.PropertyName.Equals(_controlPropertyName))
            {
                _updatingField = true;
                Synchronize(_controlObject, _controlPropertyName, _fieldObject, _fieldPropertyName);
                _updatingField = false;
            }
        }

        public void FieldPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_updatingField && e.PropertyName.Equals(_fieldPropertyName))
            {
                _updatingControl = true;
                Synchronize(_fieldObject, _fieldPropertyName, _controlObject, _controlPropertyName);
                _updatingControl = false;
            }
        }

        private void Synchronize(INotifyPropertyChanged sourceObject, string sourceProperty, INotifyPropertyChanged targetObject, string targetProperty)
        {
            PropertyInfo targetPropertyInfo = ResolvePropertyInfo(targetObject, targetProperty);
            PropertyInfo sourcePropertyInfo = ResolvePropertyInfo(sourceObject, sourceProperty);

            if (sourcePropertyInfo != null && targetPropertyInfo != null && targetPropertyInfo.CanWrite)
            {
                object bValue = sourcePropertyInfo.GetValue(sourceObject, null);
                if (_converter != null && bValue != null)
                {
                    bValue = _converter.convert(bValue);
                }
                try
                {
                    if (_validator == null || _validator.validate(bValue))
                    {
                        targetPropertyInfo.SetValue(targetObject, bValue, null);
                    }
                }
                catch (Exception e)
                {
                    throw new MemberAccessException("Could not set property '" + targetProperty + "' to '" + bValue + "' [" + (bValue != null ? bValue.GetType().Name : "") + "] on " + targetObject + ". Probably other type than expected, IBindingCoverter to the rescue.", e);
                }
            }
        }

        private static PropertyInfo ResolvePropertyInfo(object obj, string property)
        {
            PropertyInfo ret = null;
            string[] properties = property.Split(".".ToCharArray());
            for (int i = 0; i < properties.Length; i++)
            {
                string prop = properties[i];
                ret = obj.GetType().GetProperty(prop);
                if (ret != null && ret.CanRead && i < prop.Length - 1)
                {
                    obj = ret.GetValue(obj, null);
                }
            }
            return ret;
        }

        public IBindingConverter Converter
        {
            get { return _converter; }
            set { _converter = value; }
        }
    }
}
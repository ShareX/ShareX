using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;

namespace Localization
{
    public static class Localizer
    {
        #region non-public fields
        private static List<LocalizationItem> _items = new List<LocalizationItem>();
        private static ResourceManager _defaultResourceManager;
        private static bool _exceptionOnDuplicate;
        private static CultureInfo _cultureInfo;
        private static bool _tracingEnabled;
        private static object _sync;
        #endregion

        #region public fields
        /// <summary>
        /// Gets or sets default resource manager
        /// </summary>
        public static ResourceManager DefaultResourceManager
        {
            get
            {
                lock (_sync)
                {
                    return _defaultResourceManager;
                }
            }
            set
            {
                lock (_sync)
                {
                    _defaultResourceManager = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets current culture
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                lock (_sync)
                {
                    return _cultureInfo;
                }
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                lock (_sync)
                {
                    _cultureInfo = value;
                    Thread.CurrentThread.CurrentCulture = value;
                    CurrentUICulture = value;
                }
                OnCultureChanged();
            }
        }

        /// <summary>
        /// Gets current UI culture
        /// </summary>
        public static CultureInfo CurrentUICulture
        {
            get
            {
                lock (_sync)
                {
                    return Thread.CurrentThread.CurrentUICulture;
                }
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                lock (_sync)
                {
                    Thread.CurrentThread.CurrentUICulture = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether to throw exception when trying to add existing instance and property
        /// </summary>
        public static bool ThrowExceptionOnDuplicate
        {
            get
            {
                return _exceptionOnDuplicate;
            }
            set
            {
                _exceptionOnDuplicate = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether tracing is enabled
        /// </summary>
        public static bool TracingEnabled
        {
            get
            {
                return _tracingEnabled;
            }
            set
            {
                _tracingEnabled = value;
            }
        }
        #endregion

        #region events
        /// <summary>
        /// Occurs when the culture is changed through Localizer
        /// </summary>
        public static event EventHandler CultureChanged;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes base settings
        /// </summary>
        static Localizer()
        {
            _sync = new object();
            _cultureInfo = Thread.CurrentThread.CurrentCulture;
        }
        #endregion

        #region public methods
        #region add
        /// <summary>
        /// Adds instance and property for localizing
        /// </summary>
        /// <param name="instance">Instance to object</param>
        /// <param name="propertyName">Name of property</param>
        /// <param name="resourceKey">Resource key</param>
        public static void Add(object instance, string propertyName, string resourceKey)
        {
            Add(instance, propertyName, resourceKey, null, DefaultResourceManager);
        }

        /// <summary>
        /// Adds instance and property for localizing
        /// </summary>
        /// <param name="instance">Instance to object</param>
        /// <param name="propertyName">Name of property</param>
        /// <param name="resourceKey">Resource key</param>
        /// <param name="format">Custom string format</param>
        public static void Add(object instance, string propertyName, string resourceKey, string format)
        {
            Add(instance, propertyName, resourceKey, format, DefaultResourceManager);
        }

        /// <summary>
        /// Adds instance and property for localizing
        /// </summary>
        /// <param name="instance">Instance to object</param>
        /// <param name="propertyName">Name of property</param>
        /// <param name="resourceKey">Resource key</param>
        /// <param name="resourceManager">Resource manager</param>
        public static void Add(object instance, string propertyName, string resourceKey, ResourceManager resourceManager)
        {
            Add(instance, propertyName, resourceKey, null, resourceManager);
        }

        /// <summary>
        /// Adds instance and property for localizing
        /// </summary>
        /// <param name="instance">Instance to object</param>
        /// <param name="propertyName">Name of property</param>
        /// <param name="resourceKey">Resource key</param>
        /// <param name="format">Custom string format</param>
        /// <param name="resourceManager">Resource manager</param>
        public static void Add(object instance, string propertyName, string resourceKey, string format, ResourceManager resourceManager)
        {
            // check
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (String.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }
            resourceKey = resourceKey ?? String.Empty;
            if (String.IsNullOrEmpty(resourceKey.Trim()))
            {
                throw new ArgumentNullException("resourceKey");
            }
            if (resourceManager == null)
            {
                throw new ArgumentNullException("resourceManager");
            }

            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(instance)[propertyName];
            if (descriptor == null)
            {
                throw new ArgumentException("property does not belongs to instance", "propertyName");
            }

            // trace
            if (_tracingEnabled)
            {
                Trace.WriteLine(String.Format("Added item [Instance:\"{0}\" Property:\"{1}\" ResourceKey:\"{2}\" Format:\"{3}\"]", instance.ToString(), propertyName, resourceKey, format ?? String.Empty, resourceManager));
            }

            // create localization item
            LocalizationItem item = new LocalizationItem();
            item.Instance = instance;
            item.Manager = resourceManager;
            item.Property = descriptor;
            item.ResourceKey = resourceKey;
            item.Format = format;

            lock (_sync)
            {
                // check whether exists
                IEnumerable<LocalizationItem> existing = GetItems(instance, propertyName);
                if (existing.Count() > 0)
                {
                    if (_exceptionOnDuplicate)
                    {
                        throw new LocalizerException("This instance and property is already defined.");
                    }

                    Remove(instance, propertyName);
                }

                // add item
                _items.Add(item);
            }

            // localize item
            SetValue(item);
        }
        #endregion

        #region get string
        /// <summary>
        /// Returns localized string resource
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        /// <returns>Localized string resource</returns>
        public static string GetString(string resourceKey)
        {
            return GetString(resourceKey, DefaultResourceManager);
        }

        /// <summary>
        /// Returns localized string resource
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        /// <param name="format">Custom string format</param>
        /// <returns>Localized string resource</returns>
        public static string GetString(string resourceKey, string format)
        {
            return GetString(resourceKey, format, DefaultResourceManager);
        }

        /// <summary>
        /// Returns localized string resource
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        /// <param name="resourceManager">Resource manager</param>
        /// <returns>Localized string resource</returns>
        public static string GetString(string resourceKey, ResourceManager resourceManager)
        {
            return GetString(resourceKey, null, resourceManager);
        }

        /// <summary>
        /// Returns localized string resource
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        /// <param name="cultureInfo">Culture info</param>
        /// <returns>Localized string resource</returns>
        public static string GetString(string resourceKey, CultureInfo cultureInfo)
        {
            return GetString(resourceKey, null, DefaultResourceManager, cultureInfo);
        }

        /// <summary>
        /// Returns localized string resource
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        /// <param name="format">Custom string format</param>
        /// <param name="cultureInfo">Culture info</param>
        /// <returns>Localized string resource</returns>
        public static string GetString(string resourceKey, string format, CultureInfo cultureInfo)
        {
            return GetString(resourceKey, format, DefaultResourceManager, cultureInfo);
        }

        /// <summary>
        /// Returns localized string resource
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        /// <param name="format">Custom string format</param>
        /// <param name="resourceManager">Resource manager</param>
        /// <returns>Localized string resource</returns>
        public static string GetString(string resourceKey, string format, ResourceManager resourceManager)
        {
            return GetString(resourceKey, format, resourceManager, CurrentCulture);
        }

        /// <summary>
        /// Returns localized string resource
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        /// <param name="format">Custom string format</param>
        /// <param name="resourceManager">Resource manager</param>
        /// <param name="cultureInfo">Culture info</param>
        /// <returns>Localized string resource</returns>
        public static string GetString(string resourceKey, string format, ResourceManager resourceManager, CultureInfo cultureInfo)
        {
            // check
            resourceKey = resourceKey ?? String.Empty;
            if (String.IsNullOrEmpty(resourceKey.Trim()))
            {
                throw new ArgumentNullException("resourceKey");
            }
            if (resourceManager == null)
            {
                throw new ArgumentNullException("resourceManager");
            }

            // trace
            if (_tracingEnabled)
            {
                Trace.WriteLine(String.Format("GetString [ResourceKey:\"{0}\" Format:\"{1}\" ResourceManager:\"{2}\" CultureInfo:\"{3}\"]", resourceKey, format, resourceManager ?? (object)String.Empty, cultureInfo));
            }

            // get and format if required
            CultureInfo culture = cultureInfo != null ? cultureInfo : CurrentCulture;
            try
            {
                string res = resourceManager.GetString(resourceKey, culture);
                if (!String.IsNullOrEmpty(format))
                {
                    res = String.Format(culture, format, res);
                }
                return res;
            }
            catch (Exception ex)
            {
                throw new LocalizerException(ex.Message, ex);
            }
        }
        #endregion

        #region get object
        /// <summary>
        /// Returns localized object resource
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        /// <returns>Localized object resource</returns>
        public static object GetObject(string resourceKey)
        {
            return GetObject(resourceKey, Localizer.DefaultResourceManager, CurrentCulture);
        }

        /// <summary>
        /// Returns localized object resource
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        /// <param name="resourceManager">Resource manager</param>
        /// <returns>Localized object resource</returns>
        public static object GetObject(string resourceKey, ResourceManager resourceManager)
        {
            return GetObject(resourceKey, resourceManager, CurrentCulture);
        }

        /// <summary>
        /// Returns localized object resource
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        /// <param name="cultureInfo">Culture info</param>
        /// <returns>Localized object resource</returns>
        public static object GetObject(string resourceKey, CultureInfo cultureInfo)
        {
            return GetObject(resourceKey, Localizer.DefaultResourceManager, cultureInfo);
        }

        /// <summary>
        /// Returns localized object resource
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        /// <param name="resourceManager">Resource manager</param>
        /// <param name="cultureInfo">Culture info</param>
        /// <returns>Localized object resource</returns>
        public static object GetObject(string resourceKey, ResourceManager resourceManager, CultureInfo cultureInfo)
        {
            // check
            resourceKey = resourceKey ?? String.Empty;
            if (String.IsNullOrEmpty(resourceKey.Trim()))
            {
                throw new ArgumentNullException("resourceKey");
            }
            if (resourceManager == null)
            {
                throw new ArgumentNullException("resourceManager");
            }

            // trace
            if (_tracingEnabled)
            {
                Trace.WriteLine(String.Format("GetObject [ResourceKey:\"{0}\" ResourceManager:\"{1}\" CultureInfo:\"{2}\"]", resourceKey, resourceManager ?? (object)String.Empty, cultureInfo));
            }

            // get
            CultureInfo culture = cultureInfo != null ? cultureInfo : CurrentCulture;
            try
            {
                object res = resourceManager.GetObject(resourceKey, culture);
                return res;
            }
            catch (Exception ex)
            {
                throw new LocalizerException(ex.Message, ex);
            }
        }
        #endregion

        #region get stream
        /// <summary>
        /// Returns localized stream resource
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        /// <returns>Localized stream resource</returns>
        public static Stream GetStream(string resourceKey)
        {
            return GetStream(resourceKey, Localizer.DefaultResourceManager, CurrentCulture);
        }

        /// <summary>
        /// Returns localized stream resource
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        /// <param name="resourceManager">Resource manager</param>
        /// <returns>Localized stream resource</returns>
        public static Stream GetStream(string resourceKey, ResourceManager resourceManager)
        {
            return GetStream(resourceKey, resourceManager, CurrentCulture);
        }

        /// <summary>
        /// Returns localized stream resource
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        /// <param name="cultureInfo">Culture info</param>
        /// <returns>Localized stream resource</returns>
        public static Stream GetStream(string resourceKey, CultureInfo cultureInfo)
        {
            return GetStream(resourceKey, Localizer.DefaultResourceManager, cultureInfo);
        }

        /// <summary>
        /// Returns localized stream resource
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        /// <param name="resourceManager">Resource manager</param>
        /// <param name="cultureInfo">Culture info</param>
        /// <returns>Localized stream resource</returns>
        public static Stream GetStream(string resourceKey, ResourceManager resourceManager, CultureInfo cultureInfo)
        {
            // check
            resourceKey = resourceKey ?? String.Empty;
            if (String.IsNullOrEmpty(resourceKey.Trim()))
            {
                throw new ArgumentNullException("resourceKey");
            }
            if (resourceManager == null)
            {
                throw new ArgumentNullException("resourceManager");
            }

            // trace
            if (_tracingEnabled)
            {
                Trace.WriteLine(String.Format("GetStream [ResourceKey:\"{0}\" ResourceManager:\"{1}\" CultureInfo:\"{2}\"]", resourceKey, resourceManager ?? (object)String.Empty, cultureInfo));
            }

            // get
            CultureInfo culture = cultureInfo != null ? cultureInfo : CurrentCulture;
            try
            {
                Stream res = resourceManager.GetStream(resourceKey, culture);
                return res;
            }
            catch (Exception ex)
            {
                throw new LocalizerException(ex.Message, ex);
            }
        }
        #endregion

        #region others
        /// <summary>
        /// Refreshes localized items by current culture
        /// </summary>
        public static void Refresh()
        {
            Refresh(CurrentCulture);
        }

        /// <summary>
        /// Refreshes localized items by specific culture without changing current culture
        /// </summary>
        public static void Refresh(CultureInfo cultureInfo)
        {
            // check
            if (cultureInfo == null)
            {
                throw new ArgumentNullException("cultureInfo");
            }

            // trace
            if (_tracingEnabled)
            {
                Trace.WriteLine(String.Format("Refresh [Culture:\"{0}\"]", cultureInfo.DisplayName));
            }

            // refresh
            lock (_sync)
            {
                foreach (LocalizationItem item in _items)
                {
                    SetValue(item, cultureInfo);
                }
            }
        }

        /// <summary>
        /// Removes instance and property from Localizer
        /// </summary>
        /// <param name="instance">Instance to object</param>
        /// <param name="propertyName">Property name</param>
        public static void Remove(object instance, string propertyName)
        {
            // check
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            propertyName = propertyName ?? String.Empty;
            if (String.IsNullOrEmpty(propertyName.Trim()))
            {
                throw new ArgumentNullException("propertyName");
            }

            lock (_sync)
            {
                // check if exists
                IEnumerable<LocalizationItem> itemsToRemove = GetItems(instance, propertyName);
                if (itemsToRemove.Count() == 0)
                {
                    throw new LocalizerException("This instance and property is not added.");
                }

                // remove
                foreach (LocalizationItem item in itemsToRemove)
                {
                    _items.Remove(item);
                }
            }
        }
        #endregion
        #endregion

        #region non-public methods
        /// <summary>
        /// Raises the CultureChanged event
        /// </summary>
        private static void OnCultureChanged()
        {
            // trace
            if (_tracingEnabled)
            {
                Trace.WriteLine(String.Format("CultureChanged [CultureInfo:\"{0}\"]", CurrentCulture));
            }

            // fire event
            if (CultureChanged != null)
            {
                CultureChanged(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Returns items by instance and property name
        /// </summary>
        /// <param name="instance">Instance to object</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>Localization items</returns>
        private static IEnumerable<LocalizationItem> GetItems(object instance, string propertyName)
        {
            // check
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (String.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            // get by instance and property name
            List<LocalizationItem> res = new List<LocalizationItem>();
            lock (_sync)
            {
                foreach (LocalizationItem item in _items)
                {
                    if (item.Instance == instance && item.Property.Name == propertyName)
                    {
                        res.Add(item);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Applies localization to item
        /// </summary>
        /// <param name="item">Localization item</param>
        private static void SetValue(LocalizationItem item)
        {
            SetValue(item, null);
        }

        /// <summary>
        /// Applies localization to item by specified culture
        /// </summary>
        /// <param name="item">Localization item</param>
        /// <param name="cultureInfo">Culture info</param>
        private static void SetValue(LocalizationItem item, CultureInfo cultureInfo)
        {
            // check
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            object value = null;
            CultureInfo culture = cultureInfo != null ? cultureInfo : CurrentCulture;
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(item.Property.PropertyType);

                // resource is string
                if (converter.CanConvertFrom(typeof(string)))
                {
                    string formatedValue = item.Manager.GetString(item.ResourceKey, culture);
                    if (!String.IsNullOrEmpty(item.Format))
                    {
                        formatedValue = String.Format(culture, item.Format, formatedValue);
                    }

                    if (item.Property.PropertyType != typeof(string))
                    {
                        value = converter.ConvertFromString(formatedValue);
                    }
                    else
                    {
                        value = formatedValue;
                    }
                }
                else
                {
                    // resource is stream
                    if (item.Property.PropertyType == typeof(Stream))
                    {
                        value = item.Manager.GetStream(item.ResourceKey, culture);
                    }
                    else
                    {
                        value = item.Manager.GetObject(item.ResourceKey, culture);
                    }
                }
                lock (_sync)
                {
                    if (_items.Contains(item))
                    {
                        item.Property.SetValue(item.Instance, value);
                    }
                }
            }
            catch (Exception e)
            {
                throw new LocalizerException(e.Message, e);
            }
        }
        #endregion

        #region non-public classes
        /// <summary>
        /// Class for storing information about instance and property for localizing
        /// </summary>
        internal class LocalizationItem
        {
            /// <summary>
            /// Key from resource file
            /// </summary>
            public string ResourceKey;

            /// <summary>
            /// Instance to object
            /// </summary>
            public object Instance;

            /// <summary>
            /// Property for localizing
            /// </summary>
            public PropertyDescriptor Property;

            /// <summary>
            /// Resource manager
            /// </summary>
            public ResourceManager Manager;

            /// <summary>
            /// String format for custom result formatting
            /// </summary>
            public string Format;
        }
        #endregion
    }
}
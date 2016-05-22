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
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Greenshot.IniFile
{
    public class IniConfig
    {
        private const string INI_EXTENSION = ".ini";
        private const string DEFAULTS_POSTFIX = "-defaults";
        private const string FIXED_POSTFIX = "-fixed";

        /// <summary>
        /// A lock object for the ini file saving
        /// </summary>
        private static readonly object iniLock = new object();

        /// <summary>
        /// As the ini implementation is kept someone generic, for reusing, this holds the name of the application
        /// </summary>
        private static string applicationName;

        /// <summary>
        /// As the ini implementation is kept someone generic, for reusing, this holds the name of the configuration
        /// </summary>
        private static string configName;

        private static string configFolderPath = null;

        /// <summary>
        /// A Dictionary with all the sections stored by section name
        /// </summary>
        private static readonly Dictionary<string, IniSection> sectionMap = new Dictionary<string, IniSection>();

        /// <summary>
        /// A Dictionary with the properties for a section stored by section name
        /// </summary>
        private static Dictionary<string, Dictionary<string, string>> sections = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// A Dictionary with the fixed-properties for a section stored by section name
        /// </summary>
        private static Dictionary<string, Dictionary<string, string>> fixedProperties;

        /// <summary>
        /// Is the configuration portable (meaning we don't store it in the AppData directory)
        /// </summary>
        private static bool portable = false;
        public static bool IsPortable
        {
            get
            {
                return portable;
            }
        }

        /// <summary>
        /// Config directory when set from external
        /// </summary>
        public static string IniDirectory
        {
            get;
            set;
        }

        public static bool AllowSave = true;

        /// <summary>
        /// Initialize the ini config
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="configName"></param>
        public static void Init(string appName, string confName, string configFolder)
        {
            applicationName = appName;
            configName = confName;
            configFolderPath = configFolder;
            if (AllowSave)
            {
                Reload();
            }
        }

        /// <summary>
        /// Checks if we initialized the ini
        /// </summary>
        public static bool isInitialized
        {
            get
            {
                return applicationName != null && configName != null && sectionMap.Count > 0;
            }
        }

        /// <summary>
        /// Default init
        /// </summary>
        public static void Init(string configFolder)
        {
            AssemblyProductAttribute[] assemblyProductAttributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false) as AssemblyProductAttribute[];
            if (assemblyProductAttributes.Length > 0)
            {
                string productName = assemblyProductAttributes[0].Product;
                LOG.InfoFormat("Using ProductName {0}", productName);
                Init(productName, "GreenshotImageEditor", configFolder);
            }
            else
            {
                throw new InvalidOperationException("Assembly ProductName not set.");
            }
        }

        /// <summary>
        /// Create the location of the configuration file
        /// </summary>
        private static string CreateIniLocation(string configFilename, bool isReadOnly)
        {
            if (applicationName == null || configName == null)
            {
                throw new InvalidOperationException("IniConfig.Init not called!");
            }

            /*string iniFilePath = null;

            // Check if a Ini-Directory was supplied, and it's valid, use this before any others.
            try
            {
                if (IniDirectory != null && Directory.Exists(IniDirectory))
                {
                    // If the greenshot.ini is requested, use the supplied directory even if empty
                    if (!isReadOnly)
                    {
                        return Path.Combine(IniDirectory, configFilename);
                    }
                    iniFilePath = Path.Combine(IniDirectory, configFilename);
                    if (File.Exists(iniFilePath))
                    {
                        return iniFilePath;
                    }
                    iniFilePath = null;
                }
            }
            catch (Exception exception)
            {
                LOG.WarnFormat("The ini-directory {0} can't be used due to: {1}", IniDirectory, exception.Message);
            }

            string applicationStartupPath = "";
            try
            {
                applicationStartupPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            catch (Exception exception)
            {
                LOG.WarnFormat("Problem retrieving the AssemblyLocation: {0} (Designer mode?)", exception.Message);
                applicationStartupPath = @".";
            }
            string pafPath = Path.Combine(applicationStartupPath, @"App\" + applicationName);

            if (portable || !portableCheckMade)
            {
                if (!portable)
                {
                    LOG.Info("Checking for portable mode.");
                    portableCheckMade = true;
                    if (Directory.Exists(pafPath))
                    {
                        portable = true;
                        LOG.Info("Portable mode active!");
                    }
                }
                if (portable)
                {
                    string pafConfigPath = Path.Combine(applicationStartupPath, @"Data\Settings");
                    try
                    {
                        if (!Directory.Exists(pafConfigPath))
                        {
                            Directory.CreateDirectory(pafConfigPath);
                        }
                        iniFilePath = Path.Combine(pafConfigPath, configFilename);
                    }
                    catch (Exception e)
                    {
                        LOG.InfoFormat("Portable mode NOT possible, couldn't create directory '{0}'! Reason: {1}", pafConfigPath, e.Message);
                    }
                }
            }
            if (iniFilePath == null)
            {
                // check if file is in the same location as started from, if this is the case
                // we will use this file instead of the ApplicationData folder
                // Done for Feature Request #2741508
                iniFilePath = Path.Combine(applicationStartupPath, configFilename);
                if (!File.Exists(iniFilePath))
                {
                    string iniDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationName);
                    if (!Directory.Exists(iniDirectory))
                    {
                        Directory.CreateDirectory(iniDirectory);
                    }
                    iniFilePath = Path.Combine(iniDirectory, configFilename);
                }
            }
            LOG.InfoFormat("Using ini file {0}", iniFilePath);
            return iniFilePath;*/

            if (AllowSave)
            {
                if (!Directory.Exists(configFolderPath))
                {
                    Directory.CreateDirectory(configFolderPath);
                }
                return Path.Combine(configFolderPath, configFilename);
            }

            return null;
        }

        /// <summary>
        /// Reload the Ini file
        /// </summary>
        public static void Reload()
        {
            // Clear the current properties
            sections = new Dictionary<string, Dictionary<string, string>>();
            // Load the defaults
            Read(CreateIniLocation(configName + DEFAULTS_POSTFIX + INI_EXTENSION, true));
            // Load the normal
            Read(CreateIniLocation(configName + INI_EXTENSION, false));
            // Load the fixed settings
            fixedProperties = Read(CreateIniLocation(configName + FIXED_POSTFIX + INI_EXTENSION, true));

            foreach (IniSection section in sectionMap.Values)
            {
                try
                {
                    section.Fill(PropertiesForSection(section));
                    FixProperties(section);
                }
                catch (Exception ex)
                {
                    string sectionName = "unknown";
                    if (section != null && section.IniSectionAttribute != null && section.IniSectionAttribute.Name != null)
                    {
                        sectionName = section.IniSectionAttribute.Name;
                    }
                    LOG.WarnFormat("Problem reading the ini section {0}", sectionName);
                    LOG.Warn("Exception", ex);
                }
            }
        }

        /// <summary>
        /// This "fixes" the properties of the section, meaning any properties in the fixed file can't be changed.
        /// </summary>
        /// <param name="section">IniSection</param>
        private static void FixProperties(IniSection section)
        {
            // Make properties unchangeable
            if (fixedProperties != null)
            {
                Dictionary<string, string> fixedPropertiesForSection = null;
                if (fixedProperties.TryGetValue(section.IniSectionAttribute.Name, out fixedPropertiesForSection))
                {
                    foreach (string fixedPropertyKey in fixedPropertiesForSection.Keys)
                    {
                        if (section.Values.ContainsKey(fixedPropertyKey))
                        {
                            section.Values[fixedPropertyKey].IsFixed = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Read the ini file into the Dictionary
        /// </summary>
        /// <param name="iniLocation">Path & Filename of ini file</param>
        private static Dictionary<string, Dictionary<string, string>> Read(string iniLocation)
        {
            if (!File.Exists(iniLocation))
            {
                LOG.Info("Can't find file: " + iniLocation);
                return null;
            }
            LOG.InfoFormat("Loading ini-file: {0}", iniLocation);
            //LOG.Info("Reading ini-properties from file: " + iniLocation);
            Dictionary<string, Dictionary<string, string>> newSections = IniReader.read(iniLocation, Encoding.UTF8);
            // Merge the newly loaded properties to the already available
            foreach (string section in newSections.Keys)
            {
                Dictionary<string, string> newProperties = newSections[section];
                if (!sections.ContainsKey(section))
                {
                    // This section is not yet loaded, simply add the complete section
                    sections.Add(section, newProperties);
                }
                else
                {
                    // Overwrite or add every property from the newly loaded section to the available one
                    Dictionary<string, string> currentProperties = sections[section];
                    foreach (string propertyName in newProperties.Keys)
                    {
                        string propertyValue = newProperties[propertyName];
                        if (currentProperties.ContainsKey(propertyName))
                        {
                            // Override current value as we are loading in a certain order which insures the default, current and fixed
                            currentProperties[propertyName] = propertyValue;
                        }
                        else
                        {
                            // Add "new" value
                            currentProperties.Add(propertyName, propertyValue);
                        }
                    }
                }
            }
            return newSections;
        }

        public static IEnumerable<string> IniSectionNames
        {
            get
            {
                foreach (string sectionName in sectionMap.Keys)
                {
                    yield return sectionName;
                }
            }
        }

        /// <summary>
        /// Method used for internal tricks...
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static IniSection GetIniSection(string sectionName)
        {
            IniSection returnValue = null;
            sectionMap.TryGetValue(sectionName, out returnValue);
            return returnValue;
        }

        /// <summary>
        /// A generic method which returns an instance of the supplied type, filled with it's configuration
        /// </summary>
        /// <typeparam name="T">IniSection Type to get the configuration for</typeparam>
        /// <returns>Filled instance of IniSection type which was supplied</returns>
        public static T GetIniSection<T>() where T : IniSection
        {
            return GetIniSection<T>(true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T">IniSection Type to get the configuration for</typeparam>
        /// <param name="allowSave">false to skip saving</param>
        /// <returns>IniSection</returns>
        public static T GetIniSection<T>(bool allowSave) where T : IniSection
        {
            T section;

            Type iniSectionType = typeof(T);
            string sectionName = IniSection.GetIniSectionAttribute(iniSectionType).Name;
            if (sectionMap.ContainsKey(sectionName))
            {
                //LOG.Debug("Returning pre-mapped section " + sectionName);
                section = (T)sectionMap[sectionName];
            }
            else
            {
                // Create instance of this type
                section = (T)Activator.CreateInstance(iniSectionType);

                // Store for later save & retrieval
                sectionMap.Add(sectionName, section);
                section.Fill(PropertiesForSection(section));
                FixProperties(section);
            }
            if (section.IsDirty)
            {
                LOG.DebugFormat("Section {0} is marked dirty, saving!", sectionName);
                Save();
            }
            return section;
        }

        /// <summary>
        /// Get the raw properties for a section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static Dictionary<string, string> PropertiesForSection(IniSection section)
        {
            Type iniSectionType = section.GetType();
            string sectionName = section.IniSectionAttribute.Name;
            // Get the properties for the section
            Dictionary<string, string> properties = null;
            if (sections.ContainsKey(sectionName))
            {
                properties = sections[sectionName];
            }
            else
            {
                sections.Add(sectionName, new Dictionary<string, string>());
                properties = sections[sectionName];
            }
            return properties;
        }

        /// <summary>
        /// Save the ini file
        /// </summary>
        public static void Save()
        {
            if (AllowSave)
            {
                bool acquiredLock = false;
                try
                {
                    acquiredLock = Monitor.TryEnter(iniLock, TimeSpan.FromMilliseconds(200));
                    if (acquiredLock)
                    {
                        // Code that accesses resources that are protected by the lock.
                        string iniLocation = CreateIniLocation(configName + INI_EXTENSION, false);
                        try
                        {
                            SaveInternally(iniLocation);
                        }
                        catch (Exception ex)
                        {
                            LOG.Error("A problem occured while writing the configuration file to: " + iniLocation);
                            LOG.Error(ex);
                        }
                    }
                    else
                    {
                        // Code to deal with the fact that the lock was not acquired.
                        LOG.Warn("A second thread tried to save the ini-file, we blocked as the first took too long.");
                    }
                }
                finally
                {
                    if (acquiredLock)
                    {
                        Monitor.Exit(iniLock);
                    }
                }
            }
        }

        /// <summary>
        /// The real save implementation
        /// </summary>
        /// <param name="iniLocation"></param>
        private static void SaveInternally(string iniLocation)
        {
            LOG.Info("Saving configuration to: " + iniLocation);
            if (!Directory.Exists(Path.GetDirectoryName(iniLocation)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(iniLocation));
            }
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    foreach (IniSection section in sectionMap.Values)
                    {
                        section.Write(writer, false);
                        // Add empty line after section
                        writer.WriteLine();
                        section.IsDirty = false;
                    }
                    writer.WriteLine();
                    // Write left over properties
                    foreach (string sectionName in sections.Keys)
                    {
                        // Check if the section is one that is "registered", if so skip it!
                        if (!sectionMap.ContainsKey(sectionName))
                        {
                            writer.WriteLine("; The section {0} hasn't been 'claimed' since the last start of Greenshot, therefor it doesn't have additional information here!", sectionName);
                            writer.WriteLine("; The reason could be that the section {0} just hasn't been used, a plugin has an error and can't claim it or maybe the whole section {0} is obsolete.", sectionName);
                            // Write section name
                            writer.WriteLine("[{0}]", sectionName);
                            Dictionary<string, string> properties = sections[sectionName];
                            // Loop and write properties
                            foreach (string propertyName in properties.Keys)
                            {
                                writer.WriteLine("{0}={1}", propertyName, properties[propertyName]);
                            }
                            writer.WriteLine();
                        }
                    }
                    // Don't forget to flush the buffer
                    writer.Flush();
                    // Now write the created .ini string to the real file
                    using (FileStream fileStream = new FileStream(iniLocation, FileMode.Create, FileAccess.Write))
                    {
                        memoryStream.WriteTo(fileStream);
                    }
                }
            }
        }
    }
}
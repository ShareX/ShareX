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
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace GreenshotPlugin.Core
{
    public delegate void LanguageChangedHandler(object sender, EventArgs e);

    /// <summary>
    /// This class supplies the GUI with translations, based upon keys.
    /// The language resources are loaded from the language files found on fixed or supplied paths
    /// </summary>
    public class Language
    {
        private static List<string> languagePaths = new List<string>();
        private static IDictionary<string, List<LanguageFile>> languageFiles = new Dictionary<string, List<LanguageFile>>();
        private static IDictionary<string, string> helpFiles = new Dictionary<string, string>();
        private const string DEFAULT_LANGUAGE = "en-US";
        private const string HELP_FILENAME_PATTERN = @"help-*.html";
        private const string LANGUAGE_FILENAME_PATTERN = @"language*.xml";
        private static Regex PREFIX_REGEXP = new Regex(@"language_([a-zA-Z0-9]+).*");
        private static Regex IETF_CLEAN_REGEXP = new Regex(@"[^a-zA-Z]+");
        private static Regex IETF_REGEXP = new Regex(@"^.*([a-zA-Z]{2}-[a-zA-Z]{2})\.xml$");
        private const string LANGUAGE_GROUPS_KEY = @"SYSTEM\CurrentControlSet\Control\Nls\Language Groups";
        private static List<string> unsupportedLanguageGroups = new List<string>();
        private static IDictionary<string, string> resources = new Dictionary<string, string>();
        private static string currentLanguage = null;

        public static event LanguageChangedHandler LanguageChanged;

        /// <summary>
        /// Static initializer for the language code
        /// </summary>
        static Language()
        {
            /*try
            {
                string applicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string applicationFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                // PAF Path
                AddPath(Path.Combine(applicationFolder, @"App\Greenshot\Languages"));

                // Application data path
                AddPath(Path.Combine(applicationDataFolder, @"Greenshot\Languages\"));

                // Startup path
                AddPath(Path.Combine(applicationFolder, @"Languages"));
            }
            catch (Exception pathException)
            {
                LOG.Error(pathException);
            }

            try
            {
                using (RegistryKey languageGroupsKey = Registry.LocalMachine.OpenSubKey(LANGUAGE_GROUPS_KEY, false))
                {
                    if (languageGroupsKey != null)
                    {
                        string[] groups = languageGroupsKey.GetValueNames();
                        foreach (string group in groups)
                        {
                            string groupValue = (string)languageGroupsKey.GetValue(group);
                            bool isGroupNotInstalled = "0".Equals(groupValue);
                            if (isGroupNotInstalled)
                            {
                                unsupportedLanguageGroups.Add(group.ToLower());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LOG.Warn("Couldn't read the installed language groups.", e);
            }

            coreConfig = IniConfig.GetIniSection<CoreConfiguration>();
            ScanFiles();
            if (!string.IsNullOrEmpty(coreConfig.Language))
            {
                CurrentLanguage = coreConfig.Language;
                if (CurrentLanguage != null && CurrentLanguage != coreConfig.Language)
                {
                    coreConfig.Language = CurrentLanguage;
                    IniConfig.Save();
                }
            }

            if (CurrentLanguage == null)
            {
                LOG.Warn("Couldn't set language from configuration, changing to default. Installation problem?");
                CurrentLanguage = DEFAULT_LANGUAGE;
                if (CurrentLanguage != null)
                {
                    coreConfig.Language = CurrentLanguage;
                    IniConfig.Save();
                }
            }

            if (CurrentLanguage == null)
            {
                LOG.Error("Couldn't set language, installation problem?");
            }*/
        }

        /// <summary>
        /// Internal method to add a path to the paths that will be scanned for language files!
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true if the path exists and is added</returns>
        private static bool AddPath(string path)
        {
            if (!languagePaths.Contains(path))
            {
                if (Directory.Exists(path))
                {
                    LOG.DebugFormat("Adding language path {0}", path);
                    languagePaths.Add(path);
                    return true;
                }
                else
                {
                    LOG.InfoFormat("Not adding non existing language path {0}", path);
                }
            }
            return false;
        }

        /// <summary>
        /// Add a new path to the paths that will be scanned for language files!
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true if the path exists and is added</returns>
        public static bool AddLanguageFilePath(string path)
        {
            if (!languagePaths.Contains(path))
            {
                LOG.DebugFormat("New language path {0}", path);
                if (AddPath(path))
                {
                    ScanFiles();
                    Reload();
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Load the files for the specified ietf
        /// </summary>
        /// <param name="ietf"></param>
        private static void LoadFiles(string ietf)
        {
            ietf = ReformatIETF(ietf);
            if (!languageFiles.ContainsKey(ietf))
            {
                LOG.ErrorFormat("No language {0} available.", ietf);
                return;
            }
            List<LanguageFile> filesToLoad = languageFiles[ietf];
            foreach (LanguageFile fileToLoad in filesToLoad)
            {
                LoadResources(fileToLoad);
            }
        }

        /// <summary>
        /// Load the language resources from the scanned files
        /// </summary>
        private static void Reload()
        {
            resources.Clear();
            LoadFiles(DEFAULT_LANGUAGE);
            if (currentLanguage != null && !currentLanguage.Equals(DEFAULT_LANGUAGE))
            {
                LoadFiles(currentLanguage);
            }
        }

        /// <summary>
        /// Get or set the current language
        /// </summary>
        public static string CurrentLanguage
        {
            get
            {
                return currentLanguage;
            }
            set
            {
                string ietf = FindBestIETFMatch(value);
                if (!languageFiles.ContainsKey(ietf))
                {
                    LOG.WarnFormat("No match for language {0} found!", ietf);
                }
                else
                {
                    if (currentLanguage == null || !currentLanguage.Equals(ietf))
                    {
                        currentLanguage = ietf;
                        Reload();
                        if (LanguageChanged != null)
                        {
                            try
                            {
                                LanguageChanged(null, null);
                            }
                            catch
                            {
                            }
                        }
                        return;
                    }
                }
                LOG.Debug("CurrentLanguage not changed!");
            }
        }

        /// <summary>
        /// Try to find the best match for the supplied IETF
        /// </summary>
        /// <param name="inputIETF"></param>
        /// <returns>IETF</returns>
        private static string FindBestIETFMatch(string inputIETF)
        {
            string returnIETF = inputIETF;
            if (string.IsNullOrEmpty(returnIETF))
            {
                returnIETF = DEFAULT_LANGUAGE;
            }
            returnIETF = ReformatIETF(returnIETF);
            if (!languageFiles.ContainsKey(returnIETF))
            {
                LOG.WarnFormat("Unknown language {0}, trying best match!", returnIETF);
                if (returnIETF.Length == 5)
                {
                    returnIETF = returnIETF.Substring(0, 2);
                }
                foreach (string availableIETF in languageFiles.Keys)
                {
                    if (availableIETF.StartsWith(returnIETF))
                    {
                        LOG.InfoFormat("Found language {0}, best match for {1}!", availableIETF, returnIETF);
                        returnIETF = availableIETF;
                        break;
                    }
                }
            }
            return returnIETF;
        }

        /// <summary>
        /// This helper method clears all non alpha characters from the IETF, and does a reformatting.
        /// This prevents problems with multiple formats or typos.
        /// </summary>
        /// <param name="inputIETF"></param>
        /// <returns></returns>
        private static string ReformatIETF(string inputIETF)
        {
            string returnIETF = null;
            if (!string.IsNullOrEmpty(inputIETF))
            {
                returnIETF = inputIETF.ToLower();
                returnIETF = IETF_CLEAN_REGEXP.Replace(returnIETF, "");
                if (returnIETF.Length == 4)
                {
                    returnIETF = returnIETF.Substring(0, 2) + "-" + returnIETF.Substring(2, 2).ToUpper();
                }
            }
            return returnIETF;
        }

        /// <summary>
        /// Return a list of all the supported languages
        /// </summary>
        public static IList<LanguageFile> SupportedLanguages
        {
            get
            {
                IList<LanguageFile> languages = new List<LanguageFile>();
                // Loop over all languages with all the files in there
                foreach (List<LanguageFile> langs in languageFiles.Values)
                {
                    // Loop over all the files for a language
                    foreach (LanguageFile langFile in langs)
                    {
                        // Only take the ones without prefix, these are the "base" language files
                        if (langFile.Prefix == null)
                        {
                            languages.Add(langFile);
                            break;
                        }
                    }
                }
                return languages;
            }
        }

        /// <summary>
        /// Return the path to the help-file
        /// </summary>
        public static string HelpFilePath
        {
            get
            {
                if (helpFiles.ContainsKey(currentLanguage))
                {
                    return helpFiles[currentLanguage];
                }
                return helpFiles[DEFAULT_LANGUAGE];
            }
        }

        /// <summary>
        /// Load the resources from the language file
        /// </summary>
        /// <param name="languageFile">File to load from</param>
        private static void LoadResources(LanguageFile languageFile)
        {
            LOG.InfoFormat("Loading language file {0}", languageFile.Filepath);
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(languageFile.Filepath);
                XmlNodeList resourceNodes = xmlDocument.GetElementsByTagName("resource");
                foreach (XmlNode resourceNode in resourceNodes)
                {
                    string key = resourceNode.Attributes["name"].Value;
                    if (!string.IsNullOrEmpty(languageFile.Prefix))
                    {
                        key = languageFile.Prefix + "." + key;
                    }
                    string text = resourceNode.InnerText;
                    if (!string.IsNullOrEmpty(text))
                    {
                        text = text.Trim();
                    }
                    if (!resources.ContainsKey(key))
                    {
                        resources.Add(key, text);
                    }
                    else
                    {
                        resources[key] = text;
                    }
                }
            }
            catch (Exception e)
            {
                LOG.Error("Could not load language file " + languageFile.Filepath, e);
            }
        }

        /// <summary>
        /// Load the language file information
        /// </summary>
        /// <param name="languageFilePath"></param>
        /// <returns></returns>
        private static LanguageFile LoadFileInfo(string languageFilePath)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(languageFilePath);
                XmlNodeList nodes = xmlDocument.GetElementsByTagName("language");
                if (nodes.Count > 0)
                {
                    LanguageFile languageFile = new LanguageFile();
                    languageFile.Filepath = languageFilePath;
                    XmlNode node = nodes.Item(0);
                    languageFile.Description = node.Attributes["description"].Value;
                    if (node.Attributes["ietf"] != null)
                    {
                        languageFile.Ietf = ReformatIETF(node.Attributes["ietf"].Value);
                    }
                    if (node.Attributes["version"] != null)
                    {
                        languageFile.Version = new Version(node.Attributes["version"].Value);
                    }
                    if (node.Attributes["prefix"] != null)
                    {
                        languageFile.Prefix = node.Attributes["prefix"].Value.ToLower();
                    }
                    if (node.Attributes["languagegroup"] != null)
                    {
                        string languageGroup = node.Attributes["languagegroup"].Value;
                        languageFile.LanguageGroup = languageGroup.ToLower();
                    }
                    return languageFile;
                }
                else
                {
                    throw new XmlException("Root element <language> is missing");
                }
            }
            catch (Exception e)
            {
                LOG.Error("Could not load language file " + languageFilePath, e);
            }
            return null;
        }

        /// <summary>
        /// Scan the files in all directories
        /// </summary>
        private static void ScanFiles()
        {
            languageFiles.Clear();
            helpFiles.Clear();
            foreach (string languagePath in languagePaths)
            {
                if (!Directory.Exists(languagePath))
                {
                    LOG.InfoFormat("Skipping non existing language path {0}", languagePath);
                    continue;
                }
                LOG.InfoFormat("Searching language directory '{0}' for language files with pattern '{1}'", languagePath, LANGUAGE_FILENAME_PATTERN);
                try
                {
                    foreach (string languageFilepath in Directory.GetFiles(languagePath, LANGUAGE_FILENAME_PATTERN, SearchOption.AllDirectories))
                    {
                        //LOG.DebugFormat("Found language file: {0}", languageFilepath);
                        LanguageFile languageFile = LoadFileInfo(languageFilepath);
                        if (languageFile == null)
                        {
                            continue;
                        }
                        if (string.IsNullOrEmpty(languageFile.Ietf))
                        {
                            LOG.WarnFormat("Fixing missing ietf in language-file {0}", languageFilepath);
                            string languageFilename = Path.GetFileName(languageFilepath);
                            if (IETF_REGEXP.IsMatch(languageFilename))
                            {
                                string replacementIETF = IETF_REGEXP.Replace(languageFilename, "$1");
                                languageFile.Ietf = ReformatIETF(replacementIETF);
                                LOG.InfoFormat("Fixed IETF to {0}", languageFile.Ietf);
                            }
                            else
                            {
                                LOG.ErrorFormat("Missing ietf , no recover possible... skipping language-file {0}!", languageFilepath);
                                continue;
                            }
                        }

                        // Check if we can display the file
                        if (!string.IsNullOrEmpty(languageFile.LanguageGroup) && unsupportedLanguageGroups.Contains(languageFile.LanguageGroup))
                        {
                            LOG.InfoFormat("Skipping unsuported (not able to display) language {0} from file {1}", languageFile.Description, languageFilepath);
                            continue;
                        }

                        // build prefix, based on the filename, but only if it's not set in the file itself.
                        if (string.IsNullOrEmpty(languageFile.Prefix))
                        {
                            string languageFilename = Path.GetFileNameWithoutExtension(languageFilepath);
                            if (PREFIX_REGEXP.IsMatch(languageFilename))
                            {
                                languageFile.Prefix = PREFIX_REGEXP.Replace(languageFilename, "$1");
                                if (!string.IsNullOrEmpty(languageFile.Prefix))
                                {
                                    languageFile.Prefix = languageFile.Prefix.Replace("plugin", "").ToLower();
                                }
                            }
                        }
                        List<LanguageFile> currentFiles = null;
                        if (languageFiles.ContainsKey(languageFile.Ietf))
                        {
                            currentFiles = languageFiles[languageFile.Ietf];
                            bool needToAdd = true;
                            List<LanguageFile> deleteList = new List<LanguageFile>();
                            foreach (LanguageFile compareWithLangfile in currentFiles)
                            {
                                if ((languageFile.Prefix == null && compareWithLangfile.Prefix == null) || (languageFile.Prefix != null && languageFile.Prefix.Equals(compareWithLangfile.Prefix)))
                                {
                                    if (compareWithLangfile.Version > languageFile.Version)
                                    {
                                        LOG.WarnFormat("Skipping {0}:{1}:{2} as {3}:{4}:{5} is newer", languageFile.Filepath, languageFile.Prefix, languageFile.Version, compareWithLangfile.Filepath, compareWithLangfile.Prefix, compareWithLangfile.Version);
                                        needToAdd = false;
                                        break;
                                    }
                                    else
                                    {
                                        LOG.WarnFormat("Found {0}:{1}:{2} and deleting {3}:{4}:{5}", languageFile.Filepath, languageFile.Prefix, languageFile.Version, compareWithLangfile.Filepath, compareWithLangfile.Prefix, compareWithLangfile.Version);
                                        deleteList.Add(compareWithLangfile);
                                    }
                                }
                            }
                            if (needToAdd)
                            {
                                foreach (LanguageFile deleteFile in deleteList)
                                {
                                    currentFiles.Remove(deleteFile);
                                }
                                LOG.InfoFormat("Added language {0} from: {1}", languageFile.Description, languageFile.Filepath);
                                currentFiles.Add(languageFile);
                            }
                        }
                        else
                        {
                            currentFiles = new List<LanguageFile>();
                            currentFiles.Add(languageFile);
                            languageFiles.Add(languageFile.Ietf, currentFiles);
                            LOG.InfoFormat("Added language {0} from: {1}", languageFile.Description, languageFile.Filepath);
                        }
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    LOG.InfoFormat("Non existing language directory: {0}", languagePath);
                }
                catch (Exception e)
                {
                    LOG.Error("Error trying for read directory " + languagePath, e);
                }

                // Now find the help files
                LOG.InfoFormat("Searching language directory '{0}' for help files with pattern '{1}'", languagePath, HELP_FILENAME_PATTERN);
                try
                {
                    foreach (string helpFilepath in Directory.GetFiles(languagePath, HELP_FILENAME_PATTERN, SearchOption.AllDirectories))
                    {
                        LOG.DebugFormat("Found help file: {0}", helpFilepath);
                        string helpFilename = Path.GetFileName(helpFilepath);
                        string ietf = ReformatIETF(helpFilename.Replace(".html", "").Replace("help-", ""));
                        if (!helpFiles.ContainsKey(ietf))
                        {
                            helpFiles.Add(ietf, helpFilepath);
                        }
                        else
                        {
                            LOG.WarnFormat("skipping help file {0}, already a file with the same IETF {1} found!", helpFilepath, ietf);
                        }
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    LOG.InfoFormat("Non existing language directory: {0}", languagePath);
                }
                catch (Exception e)
                {
                    LOG.Error("Error trying for read directory " + languagePath, e);
                }
            }
        }

        /// <summary>
        /// Check if a resource with prefix.key exists
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="key"></param>
        /// <returns>true if available</returns>
        public static bool hasKey(string prefix, Enum key)
        {
            if (key == null)
            {
                return false;
            }
            return hasKey(prefix + "." + key.ToString());
        }

        /// <summary>
        /// Check if a resource with key exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if available</returns>
        public static bool hasKey(Enum key)
        {
            if (key == null)
            {
                return false;
            }
            return hasKey(key.ToString());
        }

        /// <summary>
        /// Check if a resource with prefix.key exists
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="key"></param>
        /// <returns>true if available</returns>
        public static bool hasKey(string prefix, string key)
        {
            return hasKey(prefix + "." + key);
        }

        /// <summary>
        /// Check if a resource with key exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if available</returns>
        public static bool hasKey(string key)
        {
            if (key == null)
            {
                return false;
            }
            return resources.ContainsKey(key);
        }

        /// <summary>
        /// TryGet method which combines hasKey & GetString
        /// </summary>
        /// <param name="key"></param>
        /// <param name="languageString">out string</param>
        /// <returns></returns>
        public static bool TryGetString(string key, out string languageString)
        {
            return resources.TryGetValue(key, out languageString);
        }

        /// <summary>
        /// TryGet method which combines hasKey & GetString
        /// </summary>
        /// <param name="prefix">string with prefix</param>
        /// <param name="key">string with key</param>
        /// <param name="languageString">out string</param>
        /// <returns></returns>
        public static bool TryGetString(string prefix, string key, out string languageString)
        {
            return resources.TryGetValue(prefix + "." + key, out languageString);
        }

        public static string Translate(object key)
        {
            string typename = key.GetType().Name;
            string enumKey = typename + "." + key.ToString();
            if (hasKey(enumKey))
            {
                return GetString(enumKey);
            }
            return key.ToString();
        }

        /// <summary>
        /// Get the resource for key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>resource or a "string ###key### not found"</returns>
        public static string GetString(Enum key)
        {
            if (key == null)
            {
                return null;
            }
            return GetString(key.ToString());
        }

        /// <summary>
        /// Get the resource for prefix.key
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="key"></param>
        /// <returns>resource or a "string ###prefix.key### not found"</returns>
        public static string GetString(string prefix, Enum key)
        {
            if (key == null)
            {
                return null;
            }
            return GetString(prefix + "." + key.ToString());
        }

        /// <summary>
        /// Get the resource for prefix.key
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="key"></param>
        /// <returns>resource or a "string ###prefix.key### not found"</returns>
        public static string GetString(string prefix, string key)
        {
            return GetString(prefix + "." + key);
        }

        /// <summary>
        /// Get the resource for key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>resource or a "string ###key### not found"</returns>
        public static string GetString(string key)
        {
            if (key == null)
            {
                return null;
            }
            string returnValue;
            if (!resources.TryGetValue(key, out returnValue))
            {
                return "string ###" + key + "### not found";
            }
            return returnValue;
        }

        /// <summary>
        /// Get the resource for key, format with with string.format an supply the parameters
        /// </summary>
        /// <param name="key"></param>
        /// <returns>formatted resource or a "string ###key### not found"</returns>
        public static string GetFormattedString(Enum key, object param)
        {
            return GetFormattedString(key.ToString(), param);
        }

        /// <summary>
        /// Get the resource for prefix.key, format with with string.format an supply the parameters
        /// </summary>
        /// <param name="key"></param>
        /// <returns>formatted resource or a "string ###prefix.key### not found"</returns>
        public static string GetFormattedString(string prefix, Enum key, object param)
        {
            return GetFormattedString(prefix, key.ToString(), param);
        }

        /// <summary>
        /// Get the resource for prefix.key, format with with string.format an supply the parameters
        /// </summary>
        /// <param name="key"></param>
        /// <returns>formatted resource or a "string ###prefix.key### not found"</returns>
        public static string GetFormattedString(string prefix, string key, object param)
        {
            return GetFormattedString(prefix + "." + key, param);
        }

        /// <summary>
        /// Get the resource for key, format with with string.format an supply the parameters
        /// </summary>
        /// <param name="key"></param>
        /// <returns>formatted resource or a "string ###key### not found"</returns>
        public static string GetFormattedString(string key, object param)
        {
            string returnValue;
            if (!resources.TryGetValue(key, out returnValue))
            {
                return "string ###" + key + "### not found";
            }
            return String.Format(returnValue, param);
        }
    }

    /// <summary>
    /// This class contains the information about a language file
    /// </summary>
    public class LanguageFile : IEquatable<LanguageFile>
    {
        public string Description
        {
            get;
            set;
        }

        public string Ietf
        {
            get;
            set;
        }

        public Version Version
        {
            get;
            set;
        }

        public string LanguageGroup
        {
            get;
            set;
        }

        public string Filepath
        {
            get;
            set;
        }

        public string Prefix
        {
            get;
            set;
        }

        /// <summary>
        /// Overload equals so we can delete a entry from a collection
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(LanguageFile other)
        {
            if (Prefix != null)
            {
                if (!Prefix.Equals(other.Prefix))
                {
                    return false;
                }
            }
            else if (other.Prefix != null)
            {
                return false;
            }
            if (Ietf != null)
            {
                if (!Ietf.Equals(other.Ietf))
                {
                    return false;
                }
            }
            else if (other.Ietf != null)
            {
                return false;
            }
            if (Version != null)
            {
                if (!Version.Equals(other.Version))
                {
                    return false;
                }
            }
            else if (other.Version != null)
            {
                return false;
            }
            if (Filepath != null)
            {
                if (!Filepath.Equals(other.Filepath))
                {
                    return false;
                }
            }
            else if (other.Filepath != null)
            {
                return false;
            }
            return true;
        }
    }
}
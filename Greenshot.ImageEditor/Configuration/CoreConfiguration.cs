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

using Greenshot.IniFile;
using Greenshot.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace GreenshotPlugin.Core
{
    public enum ClipboardFormat
    {
        PNG, DIB, HTML, HTMLDATAURL, BITMAP, DIBV5
    }
    public enum OutputFormat
    {
        bmp, gif, jpg, png, tiff, greenshot
    }
    public enum WindowCaptureMode
    {
        Screen, GDI, Aero, AeroTransparent, Auto
    }

    public enum BuildStates
    {
        UNSTABLE,
        RELEASE_CANDIDATE,
        RELEASE
    }

    public enum ClickActions
    {
        DO_NOTHING,
        OPEN_LAST_IN_EXPLORER,
        OPEN_LAST_IN_EDITOR,
        OPEN_SETTINGS,
        SHOW_CONTEXT_MENU
    }

    /// <summary>
    /// Description of CoreConfiguration.
    /// </summary>
    [IniSection("Core", Description = "Greenshot core configuration")]
    public class CoreConfiguration : IniSection, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [IniProperty("Language", Description = "The language in IETF format (e.g. en-US)")]
        public string Language;

        [IniProperty("RegionHotkey", Description = "Hotkey for starting the region capture", DefaultValue = "PrintScreen")]
        public string RegionHotkey;
        [IniProperty("WindowHotkey", Description = "Hotkey for starting the window capture", DefaultValue = "Alt + PrintScreen")]
        public string WindowHotkey;
        [IniProperty("FullscreenHotkey", Description = "Hotkey for starting the fullscreen capture", DefaultValue = "Ctrl + PrintScreen")]
        public string FullscreenHotkey;
        [IniProperty("LastregionHotkey", Description = "Hotkey for starting the last region capture", DefaultValue = "Shift + PrintScreen")]
        public string LastregionHotkey;
        [IniProperty("IEHotkey", Description = "Hotkey for starting the IE capture", DefaultValue = "Shift + Ctrl + PrintScreen")]
        public string IEHotkey;

        [IniProperty("IsFirstLaunch", Description = "Is this the first time launch?", DefaultValue = "true")]
        public bool IsFirstLaunch;
        [IniProperty("Destinations", Separator = ",", Description = "Which destinations? Possible options (more might be added by plugins) are: Editor, FileDefault, FileWithDialog, Clipboard, Printer, EMail, Picker", DefaultValue = "Picker")]
        public List<string> OutputDestinations = new List<string>();
        [IniProperty("ClipboardFormats", Separator = ",", Description = "Specify which formats we copy on the clipboard? Options are: PNG, HTML, HTMLDATAURL and DIB", DefaultValue = "PNG,DIB")]
        public List<ClipboardFormat> ClipboardFormats = new List<ClipboardFormat>();

        [IniProperty("CaptureMousepointer", Description = "Should the mouse be captured?", DefaultValue = "true")]
        public bool CaptureMousepointer;
        [IniProperty("CaptureWindowsInteractive", Description = "Use interactive window selection to capture? (false=Capture active window)", DefaultValue = "false")]
        public bool CaptureWindowsInteractive;
        [IniProperty("CaptureDelay", Description = "Capture delay in millseconds.", DefaultValue = "100")]
        public int CaptureDelay;
        [IniProperty("ScreenCaptureMode", Description = "The capture mode used to capture a screen. (Auto, FullScreen, Fixed)", DefaultValue = "Auto")]
        public ScreenCaptureMode ScreenCaptureMode;
        [IniProperty("ScreenToCapture", Description = "The screen number to capture when using ScreenCaptureMode Fixed.", DefaultValue = "1")]
        public int ScreenToCapture;
        [IniProperty("WindowCaptureMode", Description = "The capture mode used to capture a Window (Screen, GDI, Aero, AeroTransparent, Auto).", DefaultValue = "Auto")]
        public WindowCaptureMode WindowCaptureMode;
        [IniProperty("WindowCaptureAllChildLocations", Description = "Enable/disable capture all children, very slow but will make it possible to use this information in the editor.", DefaultValue = "False")]
        public bool WindowCaptureAllChildLocations;

        [IniProperty("DWMBackgroundColor", Description = "The background color for a DWM window capture.")]
        public Color DWMBackgroundColor;

        [IniProperty("PlayCameraSound", LanguageKey = "settings_playsound", Description = "Play a camera sound after taking a capture.", DefaultValue = "false")]
        public bool PlayCameraSound = false;
        [IniProperty("ShowTrayNotification", LanguageKey = "settings_shownotify", Description = "Show a notification from the systray when a capture is taken.", DefaultValue = "true")]
        public bool ShowTrayNotification = true;

        [IniProperty("OutputFilePath", Description = "Output file path.")]
        public string OutputFilePath;
        [IniProperty("OutputFileAllowOverwrite", Description = "If the target file already exists True will make Greenshot always overwrite and False will display a 'Save-As' dialog.", DefaultValue = "true")]
        public bool OutputFileAllowOverwrite;
        [IniProperty("OutputFileFilenamePattern", Description = "Filename pattern for screenshot.", DefaultValue = "${capturetime:d\"yyyy-MM-dd HH_mm_ss\"}-${title}")]
        public string OutputFileFilenamePattern;
        [IniProperty("OutputFileFormat", Description = "Default file type for writing screenshots. (bmp, gif, jpg, png, tiff)", DefaultValue = "png")]
        public OutputFormat OutputFileFormat = OutputFormat.png;
        [IniProperty("OutputFileReduceColors", Description = "If set to true, than the colors of the output file are reduced to 256 (8-bit) colors", DefaultValue = "false")]
        public bool OutputFileReduceColors;
        [IniProperty("OutputFileAutoReduceColors", Description = "If set to true the amount of colors is counted and if smaller than 256 the color reduction is automatically used.", DefaultValue = "false")]
        public bool OutputFileAutoReduceColors;
        [IniProperty("OutputFileReduceColorsTo", Description = "Amount of colors to reduce to, when reducing", DefaultValue = "256")]
        public int OutputFileReduceColorsTo;

        [IniProperty("OutputFileCopyPathToClipboard", Description = "When saving a screenshot, copy the path to the clipboard?", DefaultValue = "true")]
        public bool OutputFileCopyPathToClipboard;
        [IniProperty("OutputFileAsFullpath", Description = "SaveAs Full path?")]
        public string OutputFileAsFullpath;

        [IniProperty("OutputFileJpegQuality", Description = "JPEG file save quality in %.", DefaultValue = "80")]
        public int OutputFileJpegQuality;
        [IniProperty("OutputFilePromptQuality", Description = "Ask for the quality before saving?", DefaultValue = "false")]
        public bool OutputFilePromptQuality;
        [IniProperty("OutputFileIncrementingNumber", Description = "The number for the ${NUM} in the filename pattern, is increased automatically after each save.", DefaultValue = "1")]
        public uint OutputFileIncrementingNumber;

        [IniProperty("OutputPrintPromptOptions", LanguageKey = "settings_alwaysshowprintoptionsdialog", Description = "Ask for print options when printing?", DefaultValue = "true")]
        public bool OutputPrintPromptOptions;
        [IniProperty("OutputPrintAllowRotate", LanguageKey = "printoptions_allowrotate", Description = "Allow rotating the picture for fitting on paper?", DefaultValue = "false")]
        public bool OutputPrintAllowRotate;
        [IniProperty("OutputPrintAllowEnlarge", LanguageKey = "printoptions_allowenlarge", Description = "Allow growing the picture for fitting on paper?", DefaultValue = "false")]
        public bool OutputPrintAllowEnlarge;
        [IniProperty("OutputPrintAllowShrink", LanguageKey = "printoptions_allowshrink", Description = "Allow shrinking the picture for fitting on paper?", DefaultValue = "true")]
        public bool OutputPrintAllowShrink;
        [IniProperty("OutputPrintCenter", LanguageKey = "printoptions_allowcenter", Description = "Center image when printing?", DefaultValue = "true")]
        public bool OutputPrintCenter;
        [IniProperty("OutputPrintInverted", LanguageKey = "printoptions_inverted", Description = "Print image inverted (use e.g. for console captures)", DefaultValue = "false")]
        public bool OutputPrintInverted;
        [IniProperty("OutputPrintGrayscale", LanguageKey = "printoptions_printgrayscale", Description = "Force grayscale printing", DefaultValue = "false")]
        public bool OutputPrintGrayscale;
        [IniProperty("OutputPrintMonochrome", LanguageKey = "printoptions_printmonochrome", Description = "Force monorchrome printing", DefaultValue = "false")]
        public bool OutputPrintMonochrome;
        [IniProperty("OutputPrintMonochromeThreshold", Description = "Threshold for monochrome filter (0 - 255), lower value means less black", DefaultValue = "127")]
        public byte OutputPrintMonochromeThreshold;
        [IniProperty("OutputPrintFooter", LanguageKey = "printoptions_timestamp", Description = "Print footer on print?", DefaultValue = "true")]
        public bool OutputPrintFooter;
        [IniProperty("OutputPrintFooterPattern", Description = "Footer pattern", DefaultValue = "${capturetime:d\"D\"} ${capturetime:d\"T\"} - ${title}")]
        public string OutputPrintFooterPattern;
        [IniProperty("NotificationSound", Description = "The wav-file to play when a capture is taken, loaded only once at the Greenshot startup", DefaultValue = "default")]
        public string NotificationSound;
        [IniProperty("UseProxy", Description = "Use your global proxy?", DefaultValue = "True")]
        public bool UseProxy;
        [IniProperty("IECapture", Description = "Enable/disable IE capture", DefaultValue = "True")]
        public bool IECapture;
        [IniProperty("IEFieldCapture", Description = "Enable/disable IE field capture, very slow but will make it possible to annotate the fields of a capture in the editor.", DefaultValue = "False")]
        public bool IEFieldCapture;
        [IniProperty("WindowClassesToCheckForIE", Description = "Comma separated list of Window-Classes which need to be checked for a IE instance!", DefaultValue = "AfxFrameOrView70,IMWindowClass")]
        public List<string> WindowClassesToCheckForIE;
        [IniProperty("AutoCropDifference", Description = "Sets how to compare the colors for the autocrop detection, the higher the more is 'selected'. Possible values are from 0 to 255, where everything above ~150 doesn't make much sense!", DefaultValue = "10")]
        public int AutoCropDifference;

        [IniProperty("IncludePlugins", Description = "Comma separated list of Plugins which are allowed. If something in the list, than every plugin not in the list will not be loaded!")]
        public List<string> IncludePlugins;
        [IniProperty("ExcludePlugins", Description = "Comma separated list of Plugins which are NOT allowed.")]
        public List<string> ExcludePlugins;
        [IniProperty("ExcludeDestinations", Description = "Comma separated list of destinations which should be disabled.")]
        public List<string> ExcludeDestinations;

        [IniProperty("UpdateCheckInterval", Description = "How many days between every update check? (0=no checks)", DefaultValue = "1")]
        public int UpdateCheckInterval;
        [IniProperty("LastUpdateCheck", Description = "Last update check")]
        public DateTime LastUpdateCheck;

        [IniProperty("DisableSettings", Description = "Enable/disable the access to the settings, can only be changed manually in this .ini", DefaultValue = "False")]
        public bool DisableSettings;
        [IniProperty("DisableQuickSettings", Description = "Enable/disable the access to the quick settings, can only be changed manually in this .ini", DefaultValue = "False")]
        public bool DisableQuickSettings;
        [IniProperty("DisableTrayicon", Description = "Disable the trayicon, can only be changed manually in this .ini", DefaultValue = "False")]
        public bool HideTrayicon;
        [IniProperty("HideExpertSettings", Description = "Hide expert tab in the settings, can only be changed manually in this .ini", DefaultValue = "False")]
        public bool HideExpertSettings;

        [IniProperty("ThumnailPreview", Description = "Enable/disable thumbnail previews", DefaultValue = "True")]
        public bool ThumnailPreview;

        [IniProperty("NoGDICaptureForProduct", Description = "List of productnames for which GDI capturing is skipped (using fallback).", DefaultValue = "IntelliJ IDEA")]
        public List<string> NoGDICaptureForProduct;
        [IniProperty("NoDWMCaptureForProduct", Description = "List of productnames for which DWM capturing is skipped (using fallback).", DefaultValue = "Citrix ICA Client")]
        public List<string> NoDWMCaptureForProduct;

        [IniProperty("OptimizeForRDP", Description = "Make some optimizations for usage with remote desktop", DefaultValue = "False")]
        public bool OptimizeForRDP;
        [IniProperty("MinimizeWorkingSetSize", Description = "Optimize memory footprint, but with a performance penalty!", DefaultValue = "False")]
        public bool MinimizeWorkingSetSize;

        [IniProperty("WindowCaptureRemoveCorners", Description = "Remove the corners from a window capture", DefaultValue = "True")]
        public bool WindowCaptureRemoveCorners;

        [IniProperty("CheckForUnstable", Description = "Also check for unstable version updates", DefaultValue = "False")]
        public bool CheckForUnstable;

        [IniProperty("ActiveTitleFixes", Description = "The fixes that are active.")]
        public List<string> ActiveTitleFixes;

        [IniProperty("TitleFixMatcher", Description = "The regular expressions to match the title with.")]
        public Dictionary<string, string> TitleFixMatcher;

        [IniProperty("TitleFixReplacer", Description = "The replacements for the matchers.")]
        public Dictionary<string, string> TitleFixReplacer;

        [IniProperty("ExperimentalFeatures", Description = "A list of experimental features, this allows us to test certain features before releasing them.", ExcludeIfNull = true)]
        public List<string> ExperimentalFeatures;

        [IniProperty("EnableSpecialDIBClipboardReader", Description = "Enable a special DIB clipboard reader", DefaultValue = "True")]
        public bool EnableSpecialDIBClipboardReader;

        [IniProperty("WindowCornerCutShape", Description = "The cutshape which is used to remove the window corners, is mirrorred for all corners", DefaultValue = "5,3,2,1,1")]
        public List<int> WindowCornerCutShape;

        [IniProperty("LeftClickAction", Description = "Specify what action is made if the tray icon is left clicked, if a double-click action is specified this action is initiated after a delay (configurable via the windows double-click speed)", DefaultValue = "SHOW_CONTEXT_MENU")]
        public ClickActions LeftClickAction;

        [IniProperty("DoubleClickAction", Description = "Specify what action is made if the tray icon is double clicked", DefaultValue = "OPEN_LAST_IN_EXPLORER")]
        public ClickActions DoubleClickAction;

        [IniProperty("ZoomerEnabled", Description = "Sets if the zoomer is enabled", DefaultValue = "True")]
        public bool ZoomerEnabled;
        [IniProperty("ZoomerOpacity", Description = "Specify the transparency for the zoomer, from 0-1 (where 1 is no transparency and 0 is complete transparent. An usefull setting would be 0.7)", DefaultValue = "1")]
        public float ZoomerOpacity;

        [IniProperty("MaxMenuItemLength", Description = "Maximum length of submenu items in the context menu, making this longer might cause context menu issues on dual screen systems.", DefaultValue = "25")]
        public int MaxMenuItemLength;

        [IniProperty("MailApiTo", Description = "The 'to' field for the email destination (settings for Outlook can be found under the Office section)", DefaultValue = "")]
        public string MailApiTo;
        [IniProperty("MailApiCC", Description = "The 'CC' field for the email destination (settings for Outlook can be found under the Office section)", DefaultValue = "")]
        public string MailApiCC;
        [IniProperty("MailApiBCC", Description = "The 'BCC' field for the email destination (settings for Outlook can be found under the Office section)", DefaultValue = "")]
        public string MailApiBCC;

        [IniProperty("OptimizePNGCommand", Description = "Optional command to execute on a temporary PNG file, the command should overwrite the file and Greenshot will read it back. Note: this command is also executed when uploading PNG's!", DefaultValue = "")]
        public string OptimizePNGCommand;
        [IniProperty("OptimizePNGCommandArguments", Description = "Arguments for the optional command to execute on a PNG, {0} is replaced by the temp-filename from Greenshot. Note: Temp-file is deleted afterwards by Greenshot.", DefaultValue = "\"{0}\"")]
        public string OptimizePNGCommandArguments;

        [IniProperty("LastSaveWithVersion", Description = "Version of Greenshot which created this .ini")]
        public string LastSaveWithVersion;

        [IniProperty("ProcessEXIFOrientation", Description = "When reading images from files or clipboard, use the EXIF information to correct the orientation", DefaultValue = "True")]
        public bool ProcessEXIFOrientation;

        [IniProperty("LastCapturedRegion", Description = "The last used region, for reuse in the capture last region")]
        public Rectangle LastCapturedRegion;

        private Size _iconSize;
        [IniProperty("IconSize", Description = "Defines the size of the icons (e.g. for the buttons in the editor), default value 16,16 anything bigger will cause scaling", DefaultValue = "16,16")]
        public Size IconSize
        {
            get
            {
                return _iconSize;
            }
            set
            {
                Size newSize = value;
                if (newSize != Size.Empty)
                {
                    if (newSize.Width < 16)
                    {
                        newSize.Width = 16;
                    }
                    else if (newSize.Width > 256)
                    {
                        newSize.Width = 256;
                    }
                    newSize.Width = (newSize.Width / 16) * 16;
                    if (newSize.Height < 16)
                    {
                        newSize.Height = 16;
                    }
                    else if (IconSize.Height > 256)
                    {
                        newSize.Height = 256;
                    }
                    newSize.Height = (newSize.Height / 16) * 16;
                }
                if (_iconSize != newSize)
                {
                    _iconSize = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IconSize"));
                    }
                }
            }
        }

        [IniProperty("WebRequestTimeout", Description = "The connect timeout value for webrequets, these are seconds", DefaultValue = "100")]
        public int WebRequestTimeout;
        [IniProperty("WebRequestReadWriteTimeout", Description = "The read/write timeout value for webrequets, these are seconds", DefaultValue = "100")]
        public int WebRequestReadWriteTimeout;

        /// <summary>
        /// Specifies what THIS build is
        /// </summary>
        public BuildStates BuildState
        {
            get
            {
                string informationalVersion = Application.ProductVersion;
                if (informationalVersion != null)
                {
                    if (informationalVersion.ToLowerInvariant().Contains("-rc"))
                    {
                        return BuildStates.RELEASE_CANDIDATE;
                    }
                    if (informationalVersion.ToLowerInvariant().Contains("-unstable"))
                    {
                        return BuildStates.UNSTABLE;
                    }
                }
                return BuildStates.RELEASE;
            }
        }

        public bool UseLargeIcons
        {
            get
            {
                return IconSize.Width >= 32 || IconSize.Height >= 32;
            }
        }

        /// <summary>
        /// A helper method which returns true if the supplied experimental feature is enabled
        /// </summary>
        /// <param name="experimentalFeature"></param>
        /// <returns></returns>
        public bool isExperimentalFeatureEnabled(string experimentalFeature)
        {
            return (ExperimentalFeatures != null && ExperimentalFeatures.Contains(experimentalFeature));
        }

        /// <summary>
        /// Supply values we can't put as defaults
        /// </summary>
        /// <param name="property">The property to return a default for</param>
        /// <returns>object with the default value for the supplied property</returns>
        public override object GetDefault(string property)
        {
            switch (property)
            {
                case "PluginWhitelist":
                case "PluginBacklist":
                    return new List<string>();
                case "OutputFileAsFullpath":
                    if (IniConfig.IsPortable)
                    {
                        return Path.Combine(Application.StartupPath, @"..\..\Documents\Pictures\Greenshots\dummy.png");
                    }
                    else
                    {
                        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "dummy.png");
                    }
                case "OutputFilePath":
                    if (IniConfig.IsPortable)
                    {
                        string pafOutputFilePath = Path.Combine(Application.StartupPath, @"..\..\Documents\Pictures\Greenshots");
                        if (!Directory.Exists(pafOutputFilePath))
                        {
                            try
                            {
                                Directory.CreateDirectory(pafOutputFilePath);
                                return pafOutputFilePath;
                            }
                            catch (Exception ex)
                            {
                                LOG.Warn(ex);
                                // Problem creating directory, fallback to Desktop
                            }
                        }
                        else
                        {
                            return pafOutputFilePath;
                        }
                    }
                    return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                case "DWMBackgroundColor":
                    return Color.Transparent;
                case "ActiveTitleFixes":
                    List<string> activeDefaults = new List<string>();
                    activeDefaults.Add("Firefox");
                    activeDefaults.Add("IE");
                    activeDefaults.Add("Chrome");
                    return activeDefaults;
                case "TitleFixMatcher":
                    Dictionary<string, string> matcherDefaults = new Dictionary<string, string>();
                    matcherDefaults.Add("Firefox", " - Mozilla Firefox.*");
                    matcherDefaults.Add("IE", " - (Microsoft|Windows) Internet Explorer.*");
                    matcherDefaults.Add("Chrome", " - Google Chrome.*");
                    return matcherDefaults;
                case "TitleFixReplacer":
                    Dictionary<string, string> replacerDefaults = new Dictionary<string, string>();
                    replacerDefaults.Add("Firefox", "");
                    replacerDefaults.Add("IE", "");
                    replacerDefaults.Add("Chrome", "");
                    return replacerDefaults;
            }
            return null;
        }

        /// <summary>
        /// This method will be called before converting the property, making to possible to correct a certain value
        /// Can be used when migration is needed
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="propertyValue">The string value of the property</param>
        /// <returns>string with the propertyValue, modified or not...</returns>
        public override string PreCheckValue(string propertyName, string propertyValue)
        {
            // Changed the separator, now we need to correct this
            if ("Destinations".Equals(propertyName))
            {
                if (propertyValue != null)
                {
                    return propertyValue.Replace('|', ',');
                }
            }
            if ("OutputFilePath".Equals(propertyName))
            {
                if (string.IsNullOrEmpty(propertyValue))
                {
                    return null;
                }
            }
            return base.PreCheckValue(propertyName, propertyValue);
        }

        /// <summary>
        /// This method will be called before writing the configuration
        /// </summary>
        public override void BeforeSave()
        {
            try
            {
                // Store version, this can be used later to fix settings after an update
                LastSaveWithVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
            catch
            {
            }
        }

        /// <summary>
        /// This method will be called after reading the configuration, so eventually some corrections can be made
        /// </summary>
        public override void AfterLoad()
        {
            // Comment with releases
            // CheckForUnstable = true;

            if (string.IsNullOrEmpty(LastSaveWithVersion))
            {
                try
                {
                    // Store version, this can be used later to fix settings after an update
                    LastSaveWithVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
                }
                catch
                {
                }
                // Disable the AutoReduceColors as it causes issues with Mozzila applications and some others
                OutputFileAutoReduceColors = false;
            }

            // Enable OneNote if upgrading from 1.1
            if (ExcludeDestinations != null && ExcludeDestinations.Contains("OneNote"))
            {
                if (LastSaveWithVersion != null && LastSaveWithVersion.StartsWith("1.1"))
                {
                    ExcludeDestinations.Remove("OneNote");
                }
                else
                {
                    // Remove with the release
                    ExcludeDestinations.Remove("OneNote");
                }
            }

            if (OutputDestinations == null)
            {
                OutputDestinations = new List<string>();
            }

            // Make sure there is an output!
            if (OutputDestinations.Count == 0)
            {
                OutputDestinations.Add("Editor");
            }

            // Prevent both settings at once, bug #3435056
            if (OutputDestinations.Contains("Clipboard") && OutputFileCopyPathToClipboard)
            {
                OutputFileCopyPathToClipboard = false;
            }

            // Make sure we have clipboard formats, otherwise a paste doesn't make sense!
            if (ClipboardFormats == null || ClipboardFormats.Count == 0)
            {
                ClipboardFormats = new List<ClipboardFormat>();
                ClipboardFormats.Add(ClipboardFormat.PNG);
                ClipboardFormats.Add(ClipboardFormat.HTML);
                ClipboardFormats.Add(ClipboardFormat.DIB);
            }

            // Make sure the lists are lowercase, to speedup the check
            if (NoGDICaptureForProduct != null)
            {
                // Fix error in configuration
                if (NoGDICaptureForProduct.Count >= 2)
                {
                    if ("intellij".Equals(NoGDICaptureForProduct[0]) && "idea".Equals(NoGDICaptureForProduct[1]))
                    {
                        NoGDICaptureForProduct.RemoveRange(0, 2);
                        NoGDICaptureForProduct.Add("Intellij Idea");
                        IsDirty = true;
                    }
                }
                for (int i = 0; i < NoGDICaptureForProduct.Count; i++)
                {
                    NoGDICaptureForProduct[i] = NoGDICaptureForProduct[i].ToLower();
                }
            }
            if (NoDWMCaptureForProduct != null)
            {
                // Fix error in configuration
                if (NoDWMCaptureForProduct.Count >= 3)
                {
                    if ("citrix".Equals(NoDWMCaptureForProduct[0]) && "ica".Equals(NoDWMCaptureForProduct[1]) && "client".Equals(NoDWMCaptureForProduct[2]))
                    {
                        NoDWMCaptureForProduct.RemoveRange(0, 3);
                        NoDWMCaptureForProduct.Add("Citrix ICA Client");
                        IsDirty = true;
                    }
                }
                for (int i = 0; i < NoDWMCaptureForProduct.Count; i++)
                {
                    NoDWMCaptureForProduct[i] = NoDWMCaptureForProduct[i].ToLower();
                }
            }

            if (AutoCropDifference < 0)
            {
                AutoCropDifference = 0;
            }
            if (AutoCropDifference > 255)
            {
                AutoCropDifference = 255;
            }
            if (OutputFileReduceColorsTo < 2)
            {
                OutputFileReduceColorsTo = 2;
            }
            if (OutputFileReduceColorsTo > 256)
            {
                OutputFileReduceColorsTo = 256;
            }

            if (WebRequestTimeout <= 10)
            {
                WebRequestTimeout = 100;
            }
            if (WebRequestReadWriteTimeout < 1)
            {
                WebRequestReadWriteTimeout = 100;
            }
        }
    }
}
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

using Greenshot.IniFile;
using GreenshotPlugin.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace GreenshotPlugin.Controls
{
    /// <summary>
    /// This form is used for automatically binding the elements of the form to the language
    /// </summary>
    public class GreenshotForm : Form, IGreenshotLanguageBindable
    {
        protected static CoreConfiguration coreConfiguration;
        private static IDictionary<Type, FieldInfo[]> reflectionCache = new Dictionary<Type, FieldInfo[]>();
        private IComponentChangeService m_changeService;
        private bool isDesignModeLanguageSet = false;
        private bool applyLanguageManually = false;
        private bool storeFieldsManually = false;
        private IDictionary<string, Control> designTimeControls;
        private IDictionary<string, ToolStripItem> designTimeToolStripItems;

        static GreenshotForm()
        {
            if (!IsInDesignMode)
            {
                coreConfiguration = IniConfig.GetIniSection<CoreConfiguration>();
            }
        }

        [Category("Greenshot"), DefaultValue(null), Description("Specifies key of the language file to use when displaying the text.")]
        public string LanguageKey
        {
            get;
            set;
        }

        /// <summary>
        /// Used to check the designmode during a constructor
        /// </summary>
        /// <returns></returns>
        protected static bool IsInDesignMode
        {
            get
            {
                return (Application.ExecutablePath.IndexOf("devenv.exe", StringComparison.OrdinalIgnoreCase) > -1) || (Application.ExecutablePath.IndexOf("sharpdevelop.exe", StringComparison.OrdinalIgnoreCase) > -1 || (Application.ExecutablePath.IndexOf("wdexpress.exe", StringComparison.OrdinalIgnoreCase) > -1));
            }
        }

        protected bool ManualLanguageApply
        {
            get
            {
                return applyLanguageManually;
            }
            set
            {
                applyLanguageManually = value;
            }
        }

        protected bool ManualStoreFields
        {
            get
            {
                return storeFieldsManually;
            }
            set
            {
                storeFieldsManually = value;
            }
        }

        /// <summary>
        /// Code to initialize the language etc during design time
        /// </summary>
        protected void InitializeForDesigner()
        {
            if (DesignMode)
            {
                designTimeControls = new Dictionary<string, Control>();
                designTimeToolStripItems = new Dictionary<string, ToolStripItem>();
                try
                {
                    ITypeResolutionService typeResService = GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;

                    // Add a hard-path if you are using SharpDevelop
                    // Language.AddLanguageFilePath(@"C:\Greenshot\Greenshot\Languages");

                    // this "type"
                    Assembly currentAssembly = GetType().Assembly;
                    string assemblyPath = typeResService.GetPathOfAssembly(currentAssembly.GetName());
                    string assemblyDirectory = Path.GetDirectoryName(assemblyPath);
                    if (!Language.AddLanguageFilePath(Path.Combine(assemblyDirectory, @"..\..\Greenshot\Languages\")))
                    {
                        Language.AddLanguageFilePath(Path.Combine(assemblyDirectory, @"..\..\..\Greenshot\Languages\"));
                    }
                    if (!Language.AddLanguageFilePath(Path.Combine(assemblyDirectory, @"..\..\Languages\")))
                    {
                        Language.AddLanguageFilePath(Path.Combine(assemblyDirectory, @"..\..\..\Languages\"));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// This override is only for the design-time of the form
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
            {
                if (!isDesignModeLanguageSet)
                {
                    isDesignModeLanguageSet = true;
                    try
                    {
                        ApplyLanguage();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            base.OnPaint(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!DesignMode)
            {
                if (!applyLanguageManually)
                {
                    ApplyLanguage();
                }
                FillFields();
                base.OnLoad(e);
            }
            else
            {
                LOG.Info("OnLoad called from designer.");
                InitializeForDesigner();
                base.OnLoad(e);
                ApplyLanguage();
            }
        }

        /// <summary>
        /// check if the form was closed with an OK, if so store the values in the GreenshotControls
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            if (!DesignMode && !storeFieldsManually)
            {
                if (DialogResult == DialogResult.OK)
                {
                    LOG.Info("Form was closed with OK: storing field values.");
                    StoreFields();
                }
            }
            base.OnClosed(e);
        }

        /// <summary>
        /// This override allows the control to register event handlers for IComponentChangeService events
        /// at the time the control is sited, which happens only in design mode.
        /// </summary>
        public override ISite Site
        {
            get
            {
                return base.Site;
            }
            set
            {
                // Clear any component change event handlers.
                ClearChangeNotifications();

                // Set the new Site value.
                base.Site = value;

                m_changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

                // Register event handlers for component change events.
                RegisterChangeNotifications();
            }
        }

        private void ClearChangeNotifications()
        {
            // The m_changeService value is null when not in design mode,
            // as the IComponentChangeService is only available at design time.
            m_changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            // Clear our the component change events to prepare for re-siting.
            if (m_changeService != null)
            {
                m_changeService.ComponentChanged -= OnComponentChanged;
                m_changeService.ComponentAdded -= OnComponentAdded;
            }
        }

        private void RegisterChangeNotifications()
        {
            // Register the event handlers for the IComponentChangeService events
            if (m_changeService != null)
            {
                m_changeService.ComponentChanged += OnComponentChanged;
                m_changeService.ComponentAdded += OnComponentAdded;
            }
        }

        /// <summary>
        /// This method handles the OnComponentChanged event to display a notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ce"></param>
        private void OnComponentChanged(object sender, ComponentChangedEventArgs ce)
        {
            if (ce.Component != null && ((IComponent)ce.Component).Site != null && ce.Member != null)
            {
                if ("LanguageKey".Equals(ce.Member.Name))
                {
                    Control control = ce.Component as Control;
                    if (control != null)
                    {
                        LOG.InfoFormat("Changing LanguageKey for {0} to {1}", control.Name, ce.NewValue);
                        ApplyLanguage(control, (string)ce.NewValue);
                    }
                    else
                    {
                        ToolStripItem item = ce.Component as ToolStripItem;
                        if (item != null)
                        {
                            LOG.InfoFormat("Changing LanguageKey for {0} to {1}", item.Name, ce.NewValue);
                            ApplyLanguage(item, (string)ce.NewValue);
                        }
                        else
                        {
                            LOG.InfoFormat("Not possible to changing LanguageKey for {0} to {1}", ce.Component.GetType(), ce.NewValue);
                        }
                    }
                }
            }
        }

        private void OnComponentAdded(object sender, ComponentEventArgs ce)
        {
            if (ce.Component != null && ((IComponent)ce.Component).Site != null)
            {
                Control control = ce.Component as Control;
                if (control != null)
                {
                    if (!designTimeControls.ContainsKey(control.Name))
                    {
                        designTimeControls.Add(control.Name, control);
                    }
                    else
                    {
                        designTimeControls[control.Name] = control;
                    }
                }
                else if (ce.Component is ToolStripItem)
                {
                    ToolStripItem item = ce.Component as ToolStripItem;
                    if (!designTimeControls.ContainsKey(item.Name))
                    {
                        designTimeToolStripItems.Add(item.Name, item);
                    }
                    else
                    {
                        designTimeToolStripItems[item.Name] = item;
                    }
                }
            }
        }

        // Clean up any resources being used.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearChangeNotifications();
            }
            base.Dispose(disposing);
        }

        protected void ApplyLanguage(ToolStripItem applyTo, string languageKey)
        {
            string langString = null;
            if (!string.IsNullOrEmpty(languageKey))
            {
                if (!Language.TryGetString(languageKey, out langString))
                {
                    LOG.WarnFormat("Unknown language key '{0}' configured for control '{1}', this might be okay.", languageKey, applyTo.Name);
                    return;
                }
                applyTo.Text = langString;
            }
            else
            {
                // Fallback to control name!
                if (Language.TryGetString(applyTo.Name, out langString))
                {
                    applyTo.Text = langString;
                    return;
                }
                if (!DesignMode)
                {
                    LOG.DebugFormat("Greenshot control without language key: {0}", applyTo.Name);
                }
            }
        }

        protected void ApplyLanguage(ToolStripItem applyTo)
        {
            IGreenshotLanguageBindable languageBindable = applyTo as IGreenshotLanguageBindable;
            if (languageBindable != null)
            {
                ApplyLanguage(applyTo, languageBindable.LanguageKey);
            }
        }

        protected void ApplyLanguage(Control applyTo)
        {
            IGreenshotLanguageBindable languageBindable = applyTo as IGreenshotLanguageBindable;
            if (languageBindable == null)
            {
                // check if it's a menu!
                ToolStrip toolStrip = applyTo as ToolStrip;
                if (toolStrip != null)
                {
                    foreach (ToolStripItem item in toolStrip.Items)
                    {
                        ApplyLanguage(item);
                    }
                }
                return;
            }

            // Apply language text to the control
            ApplyLanguage(applyTo, languageBindable.LanguageKey);

            // Repopulate the combox boxes
            IGreenshotConfigBindable configBindable = applyTo as IGreenshotConfigBindable;
            GreenshotComboBox comboxBox = applyTo as GreenshotComboBox;
            if (configBindable != null && comboxBox != null)
            {
                if (!string.IsNullOrEmpty(configBindable.SectionName) && !string.IsNullOrEmpty(configBindable.PropertyName))
                {
                    IniSection section = IniConfig.GetIniSection(configBindable.SectionName);
                    if (section != null)
                    {
                        // Only update the language, so get the actual value and than repopulate
                        Enum currentValue = (Enum)comboxBox.GetSelectedEnum();
                        comboxBox.Populate(section.Values[configBindable.PropertyName].ValueType);
                        comboxBox.SetValue(currentValue);
                    }
                }
            }
        }

        /// <summary>
        /// Helper method to cache the fieldinfo values, so we don't need to reflect all the time!
        /// </summary>
        /// <param name="typeToGetFieldsFor"></param>
        /// <returns></returns>
        private static FieldInfo[] GetCachedFields(Type typeToGetFieldsFor)
        {
            FieldInfo[] fields = null;
            if (!reflectionCache.TryGetValue(typeToGetFieldsFor, out fields))
            {
                fields = typeToGetFieldsFor.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                reflectionCache.Add(typeToGetFieldsFor, fields);
            }
            return fields;
        }

        /// <summary>
        /// Apply all the language settings to the "Greenshot" Controls on this form
        /// </summary>
        protected void ApplyLanguage()
        {
            string langString = null;
            SuspendLayout();
            try
            {
                // Set title of the form
                if (!string.IsNullOrEmpty(LanguageKey) && Language.TryGetString(LanguageKey, out langString))
                {
                    Text = langString;
                }

                // Reset the text values for all GreenshotControls
                foreach (FieldInfo field in GetCachedFields(GetType()))
                {
                    Object controlObject = field.GetValue(this);
                    if (controlObject == null)
                    {
                        LOG.DebugFormat("No value: {0}", field.Name);
                        continue;
                    }
                    Control applyToControl = controlObject as Control;
                    if (applyToControl == null)
                    {
                        ToolStripItem applyToItem = controlObject as ToolStripItem;
                        if (applyToItem == null)
                        {
                            LOG.DebugFormat("No Control or ToolStripItem: {0}", field.Name);
                            continue;
                        }
                        ApplyLanguage(applyToItem);
                    }
                    else
                    {
                        ApplyLanguage(applyToControl);
                    }
                }

                if (DesignMode)
                {
                    foreach (Control designControl in designTimeControls.Values)
                    {
                        ApplyLanguage(designControl);
                    }
                    foreach (ToolStripItem designToolStripItem in designTimeToolStripItems.Values)
                    {
                        ApplyLanguage(designToolStripItem);
                    }
                }
            }
            finally
            {
                ResumeLayout();
            }
        }

        /// <summary>
        /// Apply the language text to supplied control
        /// </summary>
        protected void ApplyLanguage(Control applyTo, string languageKey)
        {
            string langString = null;
            if (!string.IsNullOrEmpty(languageKey))
            {
                if (!Language.TryGetString(languageKey, out langString))
                {
                    LOG.WarnFormat("Wrong language key '{0}' configured for control '{1}'", languageKey, applyTo.Name);
                    return;
                }
                applyTo.Text = langString;
            }
            else
            {
                // Fallback to control name!
                if (Language.TryGetString(applyTo.Name, out langString))
                {
                    applyTo.Text = langString;
                    return;
                }
                if (!DesignMode)
                {
                    LOG.DebugFormat("Greenshot control without language key: {0}", applyTo.Name);
                }
            }
        }

        /// <summary>
        /// Fill all GreenshotControls with the values from the configuration
        /// </summary>
        protected void FillFields()
        {
            foreach (FieldInfo field in GetCachedFields(GetType()))
            {
                Object controlObject = field.GetValue(this);
                if (controlObject == null)
                {
                    continue;
                }
                IGreenshotConfigBindable configBindable = controlObject as IGreenshotConfigBindable;
                if (configBindable == null)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(configBindable.SectionName) && !string.IsNullOrEmpty(configBindable.PropertyName))
                {
                    IniSection section = IniConfig.GetIniSection(configBindable.SectionName);
                    if (section != null)
                    {
                        IniValue iniValue = null;
                        if (!section.Values.TryGetValue(configBindable.PropertyName, out iniValue))
                        {
                            LOG.WarnFormat("Wrong property '{0}' configured for field '{1}'", configBindable.PropertyName, field.Name);
                            continue;
                        }

                        CheckBox checkBox = controlObject as CheckBox;
                        if (checkBox != null)
                        {
                            checkBox.Checked = (bool)iniValue.Value;
                            checkBox.Enabled = !iniValue.IsFixed;
                            continue;
                        }
                        RadioButton radíoButton = controlObject as RadioButton;
                        if (radíoButton != null)
                        {
                            radíoButton.Checked = (bool)iniValue.Value;
                            radíoButton.Enabled = !iniValue.IsFixed;
                            continue;
                        }

                        TextBox textBox = controlObject as TextBox;
                        if (textBox != null)
                        {
                            textBox.Text = iniValue.ToString();
                            textBox.Enabled = !iniValue.IsFixed;
                            continue;
                        }

                        GreenshotComboBox comboxBox = controlObject as GreenshotComboBox;
                        if (comboxBox != null)
                        {
                            comboxBox.Populate(iniValue.ValueType);
                            comboxBox.SetValue((Enum)iniValue.Value);
                            comboxBox.Enabled = !iniValue.IsFixed;
                            continue;
                        }
                    }
                }
            }
            OnFieldsFilled();
        }

        protected virtual void OnFieldsFilled()
        {
        }

        /// <summary>
        /// Store all GreenshotControl values to the configuration
        /// </summary>
        protected void StoreFields()
        {
            bool iniDirty = false;
            foreach (FieldInfo field in GetCachedFields(GetType()))
            {
                Object controlObject = field.GetValue(this);
                if (controlObject == null)
                {
                    continue;
                }
                IGreenshotConfigBindable configBindable = controlObject as IGreenshotConfigBindable;
                if (configBindable == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(configBindable.SectionName) && !string.IsNullOrEmpty(configBindable.PropertyName))
                {
                    IniSection section = IniConfig.GetIniSection(configBindable.SectionName);
                    if (section != null)
                    {
                        IniValue iniValue = null;
                        if (!section.Values.TryGetValue(configBindable.PropertyName, out iniValue))
                        {
                            continue;
                        }
                        CheckBox checkBox = controlObject as CheckBox;
                        if (checkBox != null)
                        {
                            iniValue.Value = checkBox.Checked;
                            iniDirty = true;
                            continue;
                        }
                        RadioButton radioButton = controlObject as RadioButton;
                        if (radioButton != null)
                        {
                            iniValue.Value = radioButton.Checked;
                            iniDirty = true;
                            continue;
                        }
                        TextBox textBox = controlObject as TextBox;
                        if (textBox != null)
                        {
                            iniValue.UseValueOrDefault(textBox.Text);
                            iniDirty = true;
                            continue;
                        }
                        GreenshotComboBox comboxBox = controlObject as GreenshotComboBox;
                        if (comboxBox != null)
                        {
                            iniValue.Value = comboxBox.GetSelectedEnum();
                            iniDirty = true;
                            continue;
                        }
                    }
                }
            }
            if (iniDirty)
            {
                //IniConfig.Save();
            }
        }
    }
}
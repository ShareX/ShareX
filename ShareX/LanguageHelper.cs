#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using HelpersLib;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace ShareX
{
    public static class LanguageHelper
    {
        public static bool ChangeLanguage(SupportedLanguage language, params Form[] forms)
        {
            CultureInfo currentCulture;

            if (language == SupportedLanguage.Automatic)
            {
                currentCulture = CultureInfo.InstalledUICulture;
            }
            else
            {
                string cultureName;

                switch (language)
                {
                    default:
                    case SupportedLanguage.English:
                        cultureName = "en-US";
                        break;
                    case SupportedLanguage.Turkish:
                        cultureName = "tr-TR";
                        break;
                }

                currentCulture = CultureInfo.GetCultureInfo(cultureName);
            }

            if (!currentCulture.Equals(Thread.CurrentThread.CurrentUICulture))
            {
                Thread.CurrentThread.CurrentUICulture = currentCulture;
                DebugHelper.WriteLine("Language changed to: " + currentCulture.DisplayName);

                foreach (Form form in forms)
                {
                    ComponentResourceManager resources = new ComponentResourceManager(form.GetType());
                    ApplyResourceToControl(form, resources, currentCulture);
                    resources.ApplyResources(form, "$this", currentCulture);
                }

                return true;
            }

            return false;
        }

        private static void ApplyResourceToControl(Control control, ComponentResourceManager resource, CultureInfo culture)
        {
            if (control is ToolStrip)
            {
                ApplyResourceToToolStripItemCollection(((ToolStrip)control).Items, resource, culture);
            }
            else
            {
                foreach (Control child in control.Controls)
                {
                    ApplyResourceToControl(child, resource, culture);
                }
            }

            resource.ApplyResources(control, control.Name, culture);
        }

        private static void ApplyResourceToToolStripItemCollection(ToolStripItemCollection collection, ComponentResourceManager resource, CultureInfo culture)
        {
            foreach (ToolStripItem item in collection)
            {
                if (item is ToolStripDropDownItem)
                {
                    ApplyResourceToToolStripItemCollection(((ToolStripDropDownItem)item).DropDownItems, resource, culture);
                }

                resource.ApplyResources(item, item.Name, culture);
            }
        }
    }
}
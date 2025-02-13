#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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

using ShareX.HelpersLib;
using ShareX.Properties;
using System.ComponentModel;
using System.Drawing;
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
                string cultureName = GetCultureName(language);
                currentCulture = CultureInfo.GetCultureInfo(cultureName);
            }

            if (!currentCulture.Equals(Thread.CurrentThread.CurrentUICulture))
            {
                Helpers.SetDefaultUICulture(currentCulture);
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

        public static Image GetLanguageIcon(SupportedLanguage language)
        {
            Image icon;

            switch (language)
            {
                default:
                case SupportedLanguage.Automatic:
                    icon = Resources.globe;
                    break;
                case SupportedLanguage.Arabic:
                    icon = Resources.ye;
                    break;
                case SupportedLanguage.Dutch:
                    icon = Resources.nl;
                    break;
                case SupportedLanguage.English:
                    icon = Resources.us;
                    break;
                case SupportedLanguage.French:
                    icon = Resources.fr;
                    break;
                case SupportedLanguage.German:
                    icon = Resources.de;
                    break;
                case SupportedLanguage.Hebrew:
                    icon = Resources.il;
                    break;
                case SupportedLanguage.Hungarian:
                    icon = Resources.hu;
                    break;
                case SupportedLanguage.Indonesian:
                    icon = Resources.id;
                    break;
                case SupportedLanguage.Italian:
                    icon = Resources.it;
                    break;
                case SupportedLanguage.Japanese:
                    icon = Resources.jp;
                    break;
                case SupportedLanguage.Korean:
                    icon = Resources.kr;
                    break;
                case SupportedLanguage.MexicanSpanish:
                    icon = Resources.mx;
                    break;
                case SupportedLanguage.Persian:
                    icon = Resources.ir;
                    break;
                case SupportedLanguage.Polish:
                    icon = Resources.pl;
                    break;
                case SupportedLanguage.Portuguese:
                    icon = Resources.pt;
                    break;
                case SupportedLanguage.PortugueseBrazil:
                    icon = Resources.br;
                    break;
                case SupportedLanguage.Romanian:
                    icon = Resources.ro;
                    break;
                case SupportedLanguage.Russian:
                    icon = Resources.ru;
                    break;
                case SupportedLanguage.SimplifiedChinese:
                    icon = Resources.cn;
                    break;
                case SupportedLanguage.Spanish:
                    icon = Resources.es;
                    break;
                case SupportedLanguage.TraditionalChinese:
                    icon = Resources.tw;
                    break;
                case SupportedLanguage.Turkish:
                    icon = Resources.tr;
                    break;
                case SupportedLanguage.Ukrainian:
                    icon = Resources.ua;
                    break;
                case SupportedLanguage.Vietnamese:
                    icon = Resources.vn;
                    break;
            }

            return icon;
        }

        public static string GetCultureName(SupportedLanguage language)
        {
            string cultureName;

            switch (language)
            {
                case SupportedLanguage.Arabic:
                    cultureName = "ar-YE";
                    break;
                case SupportedLanguage.Dutch:
                    cultureName = "nl-NL";
                    break;
                default:
                case SupportedLanguage.English:
                    cultureName = "en-US";
                    break;
                case SupportedLanguage.French:
                    cultureName = "fr-FR";
                    break;
                case SupportedLanguage.German:
                    cultureName = "de-DE";
                    break;
                case SupportedLanguage.Hebrew:
                    cultureName = "he-IL";
                    break;
                case SupportedLanguage.Hungarian:
                    cultureName = "hu-HU";
                    break;
                case SupportedLanguage.Indonesian:
                    cultureName = "id-ID";
                    break;
                case SupportedLanguage.Italian:
                    cultureName = "it-IT";
                    break;
                case SupportedLanguage.Japanese:
                    cultureName = "ja-JP";
                    break;
                case SupportedLanguage.Korean:
                    cultureName = "ko-KR";
                    break;
                case SupportedLanguage.MexicanSpanish:
                    cultureName = "es-MX";
                    break;
                case SupportedLanguage.Persian:
                    cultureName = "fa-IR";
                    break;
                case SupportedLanguage.Polish:
                    cultureName = "pl-PL";
                    break;
                case SupportedLanguage.Portuguese:
                    cultureName = "pt-PT";
                    break;
                case SupportedLanguage.PortugueseBrazil:
                    cultureName = "pt-BR";
                    break;
                case SupportedLanguage.Romanian:
                    cultureName = "ro-RO";
                    break;
                case SupportedLanguage.Russian:
                    cultureName = "ru-RU";
                    break;
                case SupportedLanguage.SimplifiedChinese:
                    cultureName = "zh-CN";
                    break;
                case SupportedLanguage.Spanish:
                    cultureName = "es-ES";
                    break;
                case SupportedLanguage.TraditionalChinese:
                    cultureName = "zh-TW";
                    break;
                case SupportedLanguage.Turkish:
                    cultureName = "tr-TR";
                    break;
                case SupportedLanguage.Ukrainian:
                    cultureName = "uk-UA";
                    break;
                case SupportedLanguage.Vietnamese:
                    cultureName = "vi-VN";
                    break;
            }

            return cultureName;
        }

        private static void ApplyResourceToControl(Control control, ComponentResourceManager resource, CultureInfo culture)
        {
            if (control is ToolStrip ts)
            {
                ApplyResourceToToolStripItemCollection(ts.Items, resource, culture);
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
                if (item is ToolStripDropDownItem tsddi)
                {
                    ApplyResourceToToolStripItemCollection(tsddi.DropDownItems, resource, culture);
                }

                resource.ApplyResources(item, item.Name, culture);
            }
        }
    }
}
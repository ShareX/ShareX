#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

namespace ShareX.HelpersLib
{
    public class CodeMenuEntryFilename : CodeMenuEntry
    {
        protected override string Prefix { get; } = "%";

        public static readonly CodeMenuEntryFilename t = new CodeMenuEntryFilename("t", Localization.Strings.ReplCodeMenuEntry_t_Title_of_active_window, Localization.Strings.ReplCodeMenuCategory_Window);
        public static readonly CodeMenuEntryFilename pn = new CodeMenuEntryFilename("pn", Localization.Strings.ReplCodeMenuEntry_pn_Process_name_of_active_window, Localization.Strings.ReplCodeMenuCategory_Window);
        public static readonly CodeMenuEntryFilename y = new CodeMenuEntryFilename("y", Localization.Strings.ReplCodeMenuEntry_y_Current_year, Localization.Strings.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename yy = new CodeMenuEntryFilename("yy", Localization.Strings.ReplCodeMenuEntry_yy_Current_year__2_digits_, Localization.Strings.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename mo = new CodeMenuEntryFilename("mo", Localization.Strings.ReplCodeMenuEntry_mo_Current_month, Localization.Strings.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename mon = new CodeMenuEntryFilename("mon", Localization.Strings.ReplCodeMenuEntry_mon_Current_month_name__Local_language_, Localization.Strings.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename mon2 = new CodeMenuEntryFilename("mon2", Localization.Strings.ReplCodeMenuEntry_mon2_Current_month_name__English_, Localization.Strings.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename w = new CodeMenuEntryFilename("w", Localization.Strings.ReplCodeMenuEntry_w_Current_week_name__Local_language_, Localization.Strings.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename w2 = new CodeMenuEntryFilename("w2", Localization.Strings.ReplCodeMenuEntry_w2_Current_week_name__English_, Localization.Strings.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename wy = new CodeMenuEntryFilename("wy", Localization.Strings.ReplCodeMenuEntry_wy_Week_of_year, Localization.Strings.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename d = new CodeMenuEntryFilename("d", Localization.Strings.ReplCodeMenuEntry_d_Current_day, Localization.Strings.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename h = new CodeMenuEntryFilename("h", Localization.Strings.ReplCodeMenuEntry_h_Current_hour, Localization.Strings.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename mi = new CodeMenuEntryFilename("mi", Localization.Strings.ReplCodeMenuEntry_mi_Current_minute, Localization.Strings.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename s = new CodeMenuEntryFilename("s", Localization.Strings.ReplCodeMenuEntry_s_Current_second, Localization.Strings.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename ms = new CodeMenuEntryFilename("ms", Localization.Strings.ReplCodeMenuEntry_ms_Current_millisecond, Localization.Strings.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename pm = new CodeMenuEntryFilename("pm", Localization.Strings.ReplCodeMenuEntry_pm_Gets_AM_PM, Localization.Strings.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename unix = new CodeMenuEntryFilename("unix", Localization.Strings.ReplCodeMenuEntry_unix_Unix_timestamp, Localization.Strings.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename i = new CodeMenuEntryFilename("i", Localization.Strings.ReplCodeMenuEntry_i_Auto_increment_number, Localization.Strings.ReplCodeMenuCategory_Incremental);
        public static readonly CodeMenuEntryFilename ia = new CodeMenuEntryFilename("ia", Localization.Strings.ReplCodeMenuEntry_ia_Auto_increment_alphanumeric, Localization.Strings.ReplCodeMenuCategory_Incremental);
        public static readonly CodeMenuEntryFilename iAa = new CodeMenuEntryFilename("iAa", Localization.Strings.ReplCodeMenuEntry_iAa_Auto_increment_alphanumeric_all, Localization.Strings.ReplCodeMenuCategory_Incremental);
        public static readonly CodeMenuEntryFilename ib = new CodeMenuEntryFilename("ib", Localization.Strings.ReplCodeMenuEntry_ib_Auto_increment_base_alphanumeric, Localization.Strings.ReplCodeMenuCategory_Incremental);
        public static readonly CodeMenuEntryFilename ix = new CodeMenuEntryFilename("ix", Localization.Strings.ReplCodeMenuEntry_ix_Auto_increment_hexadecimal, Localization.Strings.ReplCodeMenuCategory_Incremental);
        public static readonly CodeMenuEntryFilename rn = new CodeMenuEntryFilename("rn", Localization.Strings.ReplCodeMenuEntry_rn_Random_number_0_to_9, Localization.Strings.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename ra = new CodeMenuEntryFilename("ra", Localization.Strings.ReplCodeMenuEntry_ra_Random_alphanumeric_char, Localization.Strings.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename rna = new CodeMenuEntryFilename("rna", Localization.Strings.RandomNonAmbiguousAlphanumericCharRepeatUsingN, Localization.Strings.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename rx = new CodeMenuEntryFilename("rx", Localization.Strings.ReplCodeMenuEntry_rx_Random_hexadecimal, Localization.Strings.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename guid = new CodeMenuEntryFilename("guid", Localization.Strings.ReplCodeMenuEntry_guid_Random_guid, Localization.Strings.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename radjective = new CodeMenuEntryFilename("radjective", Localization.Strings.CodeMenuEntryFilename_RandomAdjective, Localization.Strings.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename ranimal = new CodeMenuEntryFilename("ranimal", Localization.Strings.CodeMenuEntryFilename_RandomAnimal, Localization.Strings.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename remoji = new CodeMenuEntryFilename("remoji", Localization.Strings.RandomEmojiRepeatUsingN, Localization.Strings.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename rf = new CodeMenuEntryFilename("rf", Localization.Strings.ReplCodeMenuEntry_rf_Random_line_from_file, Localization.Strings.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename width = new CodeMenuEntryFilename("width", Localization.Strings.ReplCodeMenuEntry_width_Gets_image_width, Localization.Strings.ReplCodeMenuCategory_Image);
        public static readonly CodeMenuEntryFilename height = new CodeMenuEntryFilename("height", Localization.Strings.ReplCodeMenuEntry_height_Gets_image_height, Localization.Strings.ReplCodeMenuCategory_Image);
        public static readonly CodeMenuEntryFilename un = new CodeMenuEntryFilename("un", Localization.Strings.ReplCodeMenuEntry_un_User_name, Localization.Strings.ReplCodeMenuCategory_Computer);
        public static readonly CodeMenuEntryFilename uln = new CodeMenuEntryFilename("uln", Localization.Strings.ReplCodeMenuEntry_uln_User_login_name, Localization.Strings.ReplCodeMenuCategory_Computer);
        public static readonly CodeMenuEntryFilename cn = new CodeMenuEntryFilename("cn", Localization.Strings.ReplCodeMenuEntry_cn_Computer_name, Localization.Strings.ReplCodeMenuCategory_Computer);
        public static readonly CodeMenuEntryFilename n = new CodeMenuEntryFilename("n", Localization.Strings.ReplCodeMenuEntry_n_New_line);

        public CodeMenuEntryFilename(string value, string description, string category = null) : base(value, description, category)
        {
        }
    }
}

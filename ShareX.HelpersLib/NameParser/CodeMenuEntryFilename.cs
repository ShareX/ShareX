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

using ShareX.HelpersLib.Properties;

namespace ShareX.HelpersLib
{
    public class CodeMenuEntryFilename : CodeMenuEntry
    {
        protected override string Prefix { get; } = "%";

        public static readonly CodeMenuEntryFilename t = new CodeMenuEntryFilename("t", Resources.ReplCodeMenuEntry_t_Title_of_active_window, Resources.ReplCodeMenuCategory_Window);
        public static readonly CodeMenuEntryFilename pn = new CodeMenuEntryFilename("pn", Resources.ReplCodeMenuEntry_pn_Process_name_of_active_window, Resources.ReplCodeMenuCategory_Window);
        public static readonly CodeMenuEntryFilename y = new CodeMenuEntryFilename("y", Resources.ReplCodeMenuEntry_y_Current_year, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename yy = new CodeMenuEntryFilename("yy", Resources.ReplCodeMenuEntry_yy_Current_year__2_digits_, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename mo = new CodeMenuEntryFilename("mo", Resources.ReplCodeMenuEntry_mo_Current_month, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename mon = new CodeMenuEntryFilename("mon", Resources.ReplCodeMenuEntry_mon_Current_month_name__Local_language_, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename mon2 = new CodeMenuEntryFilename("mon2", Resources.ReplCodeMenuEntry_mon2_Current_month_name__English_, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename w = new CodeMenuEntryFilename("w", Resources.ReplCodeMenuEntry_w_Current_week_name__Local_language_, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename w2 = new CodeMenuEntryFilename("w2", Resources.ReplCodeMenuEntry_w2_Current_week_name__English_, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename wy = new CodeMenuEntryFilename("wy", Resources.ReplCodeMenuEntry_wy_Week_of_year, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename d = new CodeMenuEntryFilename("d", Resources.ReplCodeMenuEntry_d_Current_day, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename h = new CodeMenuEntryFilename("h", Resources.ReplCodeMenuEntry_h_Current_hour, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename mi = new CodeMenuEntryFilename("mi", Resources.ReplCodeMenuEntry_mi_Current_minute, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename s = new CodeMenuEntryFilename("s", Resources.ReplCodeMenuEntry_s_Current_second, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename ms = new CodeMenuEntryFilename("ms", Resources.ReplCodeMenuEntry_ms_Current_millisecond, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename pm = new CodeMenuEntryFilename("pm", Resources.ReplCodeMenuEntry_pm_Gets_AM_PM, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename unix = new CodeMenuEntryFilename("unix", Resources.ReplCodeMenuEntry_unix_Unix_timestamp, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly CodeMenuEntryFilename i = new CodeMenuEntryFilename("i", Resources.ReplCodeMenuEntry_i_Auto_increment_number, Resources.ReplCodeMenuCategory_Incremental);
        public static readonly CodeMenuEntryFilename ia = new CodeMenuEntryFilename("ia", Resources.ReplCodeMenuEntry_ia_Auto_increment_alphanumeric, Resources.ReplCodeMenuCategory_Incremental);
        public static readonly CodeMenuEntryFilename iAa = new CodeMenuEntryFilename("iAa", Resources.ReplCodeMenuEntry_iAa_Auto_increment_alphanumeric_all, Resources.ReplCodeMenuCategory_Incremental);
        public static readonly CodeMenuEntryFilename ib = new CodeMenuEntryFilename("ib", Resources.ReplCodeMenuEntry_ib_Auto_increment_base_alphanumeric, Resources.ReplCodeMenuCategory_Incremental);
        public static readonly CodeMenuEntryFilename ix = new CodeMenuEntryFilename("ix", Resources.ReplCodeMenuEntry_ix_Auto_increment_hexadecimal, Resources.ReplCodeMenuCategory_Incremental);
        public static readonly CodeMenuEntryFilename rn = new CodeMenuEntryFilename("rn", Resources.ReplCodeMenuEntry_rn_Random_number_0_to_9, Resources.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename ra = new CodeMenuEntryFilename("ra", Resources.ReplCodeMenuEntry_ra_Random_alphanumeric_char, Resources.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename rna = new CodeMenuEntryFilename("rna", Resources.RandomNonAmbiguousAlphanumericCharRepeatUsingN, Resources.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename rx = new CodeMenuEntryFilename("rx", Resources.ReplCodeMenuEntry_rx_Random_hexadecimal, Resources.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename guid = new CodeMenuEntryFilename("guid", Resources.ReplCodeMenuEntry_guid_Random_guid, Resources.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename radjective = new CodeMenuEntryFilename("radjective", Resources.CodeMenuEntryFilename_RandomAdjective, Resources.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename ranimal = new CodeMenuEntryFilename("ranimal", Resources.CodeMenuEntryFilename_RandomAnimal, Resources.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename remoji = new CodeMenuEntryFilename("remoji", Resources.RandomEmojiRepeatUsingN, Resources.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename rf = new CodeMenuEntryFilename("rf", Resources.ReplCodeMenuEntry_rf_Random_line_from_file, Resources.ReplCodeMenuCategory_Random);
        public static readonly CodeMenuEntryFilename width = new CodeMenuEntryFilename("width", Resources.ReplCodeMenuEntry_width_Gets_image_width, Resources.ReplCodeMenuCategory_Image);
        public static readonly CodeMenuEntryFilename height = new CodeMenuEntryFilename("height", Resources.ReplCodeMenuEntry_height_Gets_image_height, Resources.ReplCodeMenuCategory_Image);
        public static readonly CodeMenuEntryFilename un = new CodeMenuEntryFilename("un", Resources.ReplCodeMenuEntry_un_User_name, Resources.ReplCodeMenuCategory_Computer);
        public static readonly CodeMenuEntryFilename uln = new CodeMenuEntryFilename("uln", Resources.ReplCodeMenuEntry_uln_User_login_name, Resources.ReplCodeMenuCategory_Computer);
        public static readonly CodeMenuEntryFilename cn = new CodeMenuEntryFilename("cn", Resources.ReplCodeMenuEntry_cn_Computer_name, Resources.ReplCodeMenuCategory_Computer);
        public static readonly CodeMenuEntryFilename n = new CodeMenuEntryFilename("n", Resources.ReplCodeMenuEntry_n_New_line);

        public CodeMenuEntryFilename(string value, string description, string category = null) : base(value, description, category)
        {
        }
    }
}
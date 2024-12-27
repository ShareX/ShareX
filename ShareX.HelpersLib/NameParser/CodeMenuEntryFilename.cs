#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2024 ShareX Team

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

namespace ShareX.HelpersLib.NameParser;

public class CodeMenuEntryFilename(string value, string description, string category = null) : CodeMenuEntry(value, description, category)
{
    protected override string Prefix { get; } = "%";


    public static readonly CodeMenuEntryFilename
        t = new ("t", Resources.ReplCodeMenuEntry_t_Title_of_active_window, Resources.ReplCodeMenuCategory_Window),
        pn = new("pn", Resources.ReplCodeMenuEntry_pn_Process_name_of_active_window, Resources.ReplCodeMenuCategory_Window),
        y = new("y", Resources.ReplCodeMenuEntry_y_Current_year, Resources.ReplCodeMenuCategory_Date_and_Time),
        yy = new("yy", Resources.ReplCodeMenuEntry_yy_Current_year__2_digits_, Resources.ReplCodeMenuCategory_Date_and_Time),
        mo = new("mo", Resources.ReplCodeMenuEntry_mo_Current_month, Resources.ReplCodeMenuCategory_Date_and_Time),
        mon = new("mon", Resources.ReplCodeMenuEntry_mon_Current_month_name__Local_language_, Resources.ReplCodeMenuCategory_Date_and_Time),
        mon2 = new("mon2", Resources.ReplCodeMenuEntry_mon2_Current_month_name__English_, Resources.ReplCodeMenuCategory_Date_and_Time),
        w = new("w", Resources.ReplCodeMenuEntry_w_Current_week_name__Local_language_, Resources.ReplCodeMenuCategory_Date_and_Time),
        w2 = new("w2", Resources.ReplCodeMenuEntry_w2_Current_week_name__English_, Resources.ReplCodeMenuCategory_Date_and_Time),
        wy = new("wy", Resources.ReplCodeMenuEntry_wy_Week_of_year, Resources.ReplCodeMenuCategory_Date_and_Time),
        d = new("d", Resources.ReplCodeMenuEntry_d_Current_day, Resources.ReplCodeMenuCategory_Date_and_Time),
        h = new("h", Resources.ReplCodeMenuEntry_h_Current_hour, Resources.ReplCodeMenuCategory_Date_and_Time),
        mi = new("mi", Resources.ReplCodeMenuEntry_mi_Current_minute, Resources.ReplCodeMenuCategory_Date_and_Time),
        s = new("s", Resources.ReplCodeMenuEntry_s_Current_second, Resources.ReplCodeMenuCategory_Date_and_Time),
        ms = new("ms", Resources.ReplCodeMenuEntry_ms_Current_millisecond, Resources.ReplCodeMenuCategory_Date_and_Time),
        pm = new("pm", Resources.ReplCodeMenuEntry_pm_Gets_AM_PM, Resources.ReplCodeMenuCategory_Date_and_Time),
        unix = new("unix", Resources.ReplCodeMenuEntry_unix_Unix_timestamp, Resources.ReplCodeMenuCategory_Date_and_Time),
        i = new("i", Resources.ReplCodeMenuEntry_i_Auto_increment_number, Resources.ReplCodeMenuCategory_Incremental),
        ia = new("ia", Resources.ReplCodeMenuEntry_ia_Auto_increment_alphanumeric, Resources.ReplCodeMenuCategory_Incremental),
        iAa = new("iAa", Resources.ReplCodeMenuEntry_iAa_Auto_increment_alphanumeric_all, Resources.ReplCodeMenuCategory_Incremental),
        ib = new("ib", Resources.ReplCodeMenuEntry_ib_Auto_increment_base_alphanumeric, Resources.ReplCodeMenuCategory_Incremental),
        ix = new("ix", Resources.ReplCodeMenuEntry_ix_Auto_increment_hexadecimal, Resources.ReplCodeMenuCategory_Incremental),
        rn = new("rn", Resources.ReplCodeMenuEntry_rn_Random_number_0_to_9, Resources.ReplCodeMenuCategory_Random),
        ra = new("ra", Resources.ReplCodeMenuEntry_ra_Random_alphanumeric_char, Resources.ReplCodeMenuCategory_Random),
        rna = new("rna", Resources.RandomNonAmbiguousAlphanumericCharRepeatUsingN, Resources.ReplCodeMenuCategory_Random),
        rx = new("rx", Resources.ReplCodeMenuEntry_rx_Random_hexadecimal, Resources.ReplCodeMenuCategory_Random),
        guid = new("guid", Resources.ReplCodeMenuEntry_guid_Random_guid, Resources.ReplCodeMenuCategory_Random),
        radjective = new("radjective", Resources.CodeMenuEntryFilename_RandomAdjective, Resources.ReplCodeMenuCategory_Random),
        ranimal = new("ranimal", Resources.CodeMenuEntryFilename_RandomAnimal, Resources.ReplCodeMenuCategory_Random),
        remoji = new("remoji", Resources.RandomEmojiRepeatUsingN, Resources.ReplCodeMenuCategory_Random),
        rf = new("rf", Resources.ReplCodeMenuEntry_rf_Random_line_from_file, Resources.ReplCodeMenuCategory_Random),
        width = new("width", Resources.ReplCodeMenuEntry_width_Gets_image_width, Resources.ReplCodeMenuCategory_Image),
        height = new("height", Resources.ReplCodeMenuEntry_height_Gets_image_height, Resources.ReplCodeMenuCategory_Image),
        un = new("un", Resources.ReplCodeMenuEntry_un_User_name, Resources.ReplCodeMenuCategory_Computer),
        uln = new("uln", Resources.ReplCodeMenuEntry_uln_User_login_name, Resources.ReplCodeMenuCategory_Computer),
        cn = new("cn", Resources.ReplCodeMenuEntry_cn_Computer_name, Resources.ReplCodeMenuCategory_Computer),
        n = new("n", Resources.ReplCodeMenuEntry_n_New_line);
}
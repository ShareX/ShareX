#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareX.HelpersLib
{
    public class ReplCodeMenuEntry : CodeMenuEntry
    {
        public ReplCodeMenuEntry(string value, string description, string category = null) : base(value, description, category)
        {
        }

        public override string ToPrefixString()
        {
            return '%' + _value;
        }

        public static readonly ReplCodeMenuEntry t = new ReplCodeMenuEntry("t", Resources.ReplCodeMenuEntry_t_Title_of_active_window, Resources.ReplCodeMenuCategory_Target);
        public static readonly ReplCodeMenuEntry pn = new ReplCodeMenuEntry("pn", Resources.ReplCodeMenuEntry_pn_Process_name_of_active_window, Resources.ReplCodeMenuCategory_Target);
        public static readonly ReplCodeMenuEntry y = new ReplCodeMenuEntry("y", Resources.ReplCodeMenuEntry_y_Current_year, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly ReplCodeMenuEntry yy = new ReplCodeMenuEntry("yy", Resources.ReplCodeMenuEntry_yy_Current_year__2_digits_, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly ReplCodeMenuEntry mo = new ReplCodeMenuEntry("mo", Resources.ReplCodeMenuEntry_mo_Current_month, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly ReplCodeMenuEntry mon = new ReplCodeMenuEntry("mon", Resources.ReplCodeMenuEntry_mon_Current_month_name__Local_language_, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly ReplCodeMenuEntry mon2 = new ReplCodeMenuEntry("mon2", Resources.ReplCodeMenuEntry_mon2_Current_month_name__English_, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly ReplCodeMenuEntry d = new ReplCodeMenuEntry("d", Resources.ReplCodeMenuEntry_d_Current_day, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly ReplCodeMenuEntry h = new ReplCodeMenuEntry("h", Resources.ReplCodeMenuEntry_h_Current_hour, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly ReplCodeMenuEntry mi = new ReplCodeMenuEntry("mi", Resources.ReplCodeMenuEntry_mi_Current_minute, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly ReplCodeMenuEntry s = new ReplCodeMenuEntry("s", Resources.ReplCodeMenuEntry_s_Current_second, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly ReplCodeMenuEntry ms = new ReplCodeMenuEntry("ms", Resources.ReplCodeMenuEntry_ms_Current_millisecond, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly ReplCodeMenuEntry pm = new ReplCodeMenuEntry("pm", Resources.ReplCodeMenuEntry_pm_Gets_AM_PM, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly ReplCodeMenuEntry w = new ReplCodeMenuEntry("w", Resources.ReplCodeMenuEntry_w_Current_week_name__Local_language_, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly ReplCodeMenuEntry w2 = new ReplCodeMenuEntry("w2", Resources.ReplCodeMenuEntry_w2_Current_week_name__English_, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly ReplCodeMenuEntry unix = new ReplCodeMenuEntry("unix", Resources.ReplCodeMenuEntry_unix_Unix_timestamp, Resources.ReplCodeMenuCategory_Date_and_Time);
        public static readonly ReplCodeMenuEntry i = new ReplCodeMenuEntry("i", Resources.ReplCodeMenuEntry_i_Auto_increment_number, Resources.ReplCodeMenuCategory_Incremental);
        public static readonly ReplCodeMenuEntry ia = new ReplCodeMenuEntry("ia", Resources.ReplCodeMenuEntry_ia_Auto_increment_alphanumeric, Resources.ReplCodeMenuCategory_Incremental);
        public static readonly ReplCodeMenuEntry iAa = new ReplCodeMenuEntry("iAa", Resources.ReplCodeMenuEntry_iAa_Auto_increment_alphanumeric_all, Resources.ReplCodeMenuCategory_Incremental);
        public static readonly ReplCodeMenuEntry ib = new ReplCodeMenuEntry("ib", Resources.ReplCodeMenuEntry_ib_Auto_increment_base_alphanumeric, Resources.ReplCodeMenuCategory_Incremental);
        public static readonly ReplCodeMenuEntry ix = new ReplCodeMenuEntry("ix", Resources.ReplCodeMenuEntry_ix_Auto_increment_hexadecimal, Resources.ReplCodeMenuCategory_Incremental);
        public static readonly ReplCodeMenuEntry rn = new ReplCodeMenuEntry("rn", Resources.ReplCodeMenuEntry_rn_Random_number_0_to_9, Resources.ReplCodeMenuCategory_Random);
        public static readonly ReplCodeMenuEntry ra = new ReplCodeMenuEntry("ra", Resources.ReplCodeMenuEntry_ra_Random_alphanumeric_char, Resources.ReplCodeMenuCategory_Random);
        public static readonly ReplCodeMenuEntry rx = new ReplCodeMenuEntry("rx", Resources.ReplCodeMenuEntry_rx_Random_hexadecimal, Resources.ReplCodeMenuCategory_Random);
        public static readonly ReplCodeMenuEntry guid = new ReplCodeMenuEntry("guid", Resources.ReplCodeMenuEntry_guid_Random_guid, Resources.ReplCodeMenuCategory_Random);
        public static readonly ReplCodeMenuEntry width = new ReplCodeMenuEntry("width", Resources.ReplCodeMenuEntry_width_Gets_image_width, Resources.ReplCodeMenuCategory_Image);
        public static readonly ReplCodeMenuEntry height = new ReplCodeMenuEntry("height", Resources.ReplCodeMenuEntry_height_Gets_image_height, Resources.ReplCodeMenuCategory_Image);
        public static readonly ReplCodeMenuEntry un = new ReplCodeMenuEntry("un", Resources.ReplCodeMenuEntry_un_User_name, Resources.ReplCodeMenuCategory_Computer);
        public static readonly ReplCodeMenuEntry uln = new ReplCodeMenuEntry("uln", Resources.ReplCodeMenuEntry_uln_User_login_name, Resources.ReplCodeMenuCategory_Computer);
        public static readonly ReplCodeMenuEntry cn = new ReplCodeMenuEntry("cn", Resources.ReplCodeMenuEntry_cn_Computer_name, Resources.ReplCodeMenuCategory_Computer);
        public static readonly ReplCodeMenuEntry n = new ReplCodeMenuEntry("n", Resources.ReplCodeMenuEntry_n_New_line);
    }
}
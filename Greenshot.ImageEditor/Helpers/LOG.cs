/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015  Thomas Braun, Jens Klingen, Robin Krom
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

namespace GreenshotPlugin
{
    // Workaround for removing log4net
    public static class LOG
    {
        public static bool IsDebugEnabled
        {
            get
            {
                return false;
            }
        }

        public static void Warn(string text)
        {
        }

        public static void WarnFormat(string format, params object[] args)
        {
        }

        public static void Warn(Exception e)
        {
        }

        public static void Warn(string text, Exception e)
        {
        }

        public static void Error(string text)
        {
        }

        public static void ErrorFormat(string format, params object[] args)
        {
        }

        public static void Error(Exception e)
        {
        }

        public static void Error(string text, Exception e)
        {
        }

        public static void Info(string text)
        {
        }

        public static void InfoFormat(string format, params object[] args)
        {
        }

        public static void Debug(string text)
        {
        }

        public static void DebugFormat(string text, params object[] args)
        {
        }

        public static void Debug(string text, Exception e)
        {
        }
    }
}
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

using Avalonia.Data.Converters;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Localization;
using System.Globalization;

namespace ShareX.ImageEditor.Presentation.Converters
{
    public class CursorTypeDisplayNameConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            CursorType cursorType = value is CursorType typedCursor ? typedCursor : CursorType.Default;

            return cursorType switch
            {
                CursorType.AppStarting => Strings.CursorTypeDisplayNameConverter_AppStarting,
                CursorType.Arrow => Strings.CursorTypeDisplayNameConverter_Arrow,
                CursorType.Cross => Strings.CursorTypeDisplayNameConverter_Cross,
                CursorType.Default => Strings.CursorTypeDisplayNameConverter_Default,
                CursorType.Hand => Strings.CursorTypeDisplayNameConverter_Hand,
                CursorType.Help => Strings.CursorTypeDisplayNameConverter_Help,
                CursorType.HSplit => Strings.CursorTypeDisplayNameConverter_HSplit,
                CursorType.IBeam => Strings.CursorTypeDisplayNameConverter_IBeam,
                CursorType.No => Strings.CursorTypeDisplayNameConverter_No,
                CursorType.NoMove2D => Strings.CursorTypeDisplayNameConverter_NoMove2D,
                CursorType.NoMoveHoriz => Strings.CursorTypeDisplayNameConverter_NoMoveHoriz,
                CursorType.NoMoveVert => Strings.CursorTypeDisplayNameConverter_NoMoveVert,
                CursorType.PanEast => Strings.CursorTypeDisplayNameConverter_PanEast,
                CursorType.PanNE => Strings.CursorTypeDisplayNameConverter_PanNE,
                CursorType.PanNorth => Strings.CursorTypeDisplayNameConverter_PanNorth,
                CursorType.PanNW => Strings.CursorTypeDisplayNameConverter_PanNW,
                CursorType.PanSE => Strings.CursorTypeDisplayNameConverter_PanSE,
                CursorType.PanSouth => Strings.CursorTypeDisplayNameConverter_PanSouth,
                CursorType.PanSW => Strings.CursorTypeDisplayNameConverter_PanSW,
                CursorType.PanWest => Strings.CursorTypeDisplayNameConverter_PanWest,
                CursorType.SizeAll => Strings.CursorTypeDisplayNameConverter_SizeAll,
                CursorType.SizeNESW => Strings.CursorTypeDisplayNameConverter_SizeNESW,
                CursorType.SizeNS => Strings.CursorTypeDisplayNameConverter_SizeNS,
                CursorType.SizeNWSE => Strings.CursorTypeDisplayNameConverter_SizeNWSE,
                CursorType.SizeWE => Strings.CursorTypeDisplayNameConverter_SizeWE,
                CursorType.UpArrow => Strings.CursorTypeDisplayNameConverter_UpArrow,
                CursorType.VSplit => Strings.CursorTypeDisplayNameConverter_VSplit,
                CursorType.WaitCursor => Strings.CursorTypeDisplayNameConverter_WaitCursor,
                _ => cursorType.ToString()
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return CursorType.Default;
        }
    }
}

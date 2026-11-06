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

using Avalonia;

namespace ShareX.Icons;

/// <summary>
/// Sets the icon pack for a window or control and everything inside it, in place of <see cref="IconCatalog.Pack"/>.
/// A window whose own icons must not change with the app setting, such as the photo editor, sets it to
/// <see cref="IconPack.Modern"/>.
/// </summary>
public sealed class IconPackScope : AvaloniaObject
{
    public static readonly AttachedProperty<IconPack?> PackProperty =
        AvaloniaProperty.RegisterAttached<IconPackScope, AvaloniaObject, IconPack?>("Pack", inherits: true);

    private IconPackScope()
    {
    }

    public static IconPack? GetPack(AvaloniaObject element) => element.GetValue(PackProperty);

    public static void SetPack(AvaloniaObject element, IconPack? value) => element.SetValue(PackProperty, value);
}

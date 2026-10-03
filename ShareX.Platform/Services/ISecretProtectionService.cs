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

namespace ShareX.Platform;

/// <summary>
/// Encrypts small values (upload tokens, passwords) so they can be written to settings files.
/// Windows uses DPAPI. macOS and Linux use <see cref="KeyFileSecretProtectionService"/>.
/// </summary>
/// <remarks>Protected data is tied to the current user. A value protected on one OS cannot be read on another.</remarks>
public interface ISecretProtectionService
{
    byte[] Protect(byte[] data, byte[]? entropy = null);

    /// <exception cref="System.Security.Cryptography.CryptographicException">The data was not protected for this user or was altered.</exception>
    byte[] Unprotect(byte[] protectedData, byte[]? entropy = null);
}

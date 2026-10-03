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

#nullable enable

using ShareX.Platform;
using System;

namespace ShareX.HelpersLib;

/// <summary>Printing availability shared by the image settings and portable preview windows.</summary>
public sealed class PrintWindowViewModel
{
    private readonly Func<FeatureSupport> getSupport;
    private readonly bool previewOnly;

    public PrintWindowViewModel(bool previewOnly = false, Func<FeatureSupport>? getSupport = null)
    {
        this.previewOnly = previewOnly;
        this.getSupport = getSupport ?? (() => PlatformServices.Current.Printing.Support);
    }

    public bool CanPrint => !previewOnly && getSupport().IsSupported;

    // Linux backend policy is audited separately. Do not expose its legacy package-install advice here.
    public string? PrintingUnavailableReason => getSupport().IsSupported ? null : Localization.Strings.PrintWindow_PrintingUnavailable;

    /// <summary>Rechecks availability before invoking a print action, including callbacks from an already open window.</summary>
    public bool TryPrint(Action print)
    {
        ArgumentNullException.ThrowIfNull(print);
        if (!CanPrint) return false;
        print();
        return true;
    }
}

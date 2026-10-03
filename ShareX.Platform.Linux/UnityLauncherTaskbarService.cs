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

using ShareX.Platform.Linux.DBus;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace ShareX.Platform.Linux;

/// <summary>Dock icon progress through the com.canonical.Unity.LauncherEntry signal.</summary>
/// <remarks>
/// Shown by KDE Plasma's task manager, Dash to Dock, Dash to Panel, Plank and Latte. Docks that do not listen ignore the signal, so
/// there is nothing to detect. The signal names ShareX by its desktop entry, which the package installs as sharex.desktop.
/// </remarks>
public sealed class UnityLauncherTaskbarService : ITaskbarService
{
    private const string ObjectPath = "/com/getsharex/ShareX/LauncherEntry";

    private readonly string applicationUri;
    private readonly object syncLock = new object();
    private double progress;
    private bool visible;
    private (double Progress, bool Visible)? lastSent;

    public UnityLauncherTaskbarService(string desktopEntryId = "sharex.desktop")
    {
        applicationUri = "application://" + desktopEntryId;
    }

    public FeatureSupport Support => DBusSession.IsAvailable
        ? FeatureSupport.Supported
        : FeatureSupport.NotSupported("No D-Bus session bus is available.");

    public void SetProgressValue(int value, int maximum)
    {
        if (maximum <= 0)
        {
            return;
        }

        lock (syncLock)
        {
            progress = Math.Clamp((double)value / maximum, 0, 1);
            Send();
        }
    }

    public void SetProgressState(TaskbarProgressState state)
    {
        lock (syncLock)
        {
            visible = state != TaskbarProgressState.None;

            if (!visible)
            {
                progress = 0;
            }

            Send();
        }
    }

    internal static Dictionary<string, VariantValue> CreateProperties(double progress, bool visible) => new Dictionary<string, VariantValue>
    {
        ["progress"] = progress,
        ["progress-visible"] = visible
    };

    private void Send()
    {
        // Progress is reported on every upload chunk; only signal changes.
        (double, bool) current = (Math.Round(progress, 2), visible);

        if (!DBusSession.IsAvailable || lastSent == current)
        {
            return;
        }

        lastSent = current;
        Dictionary<string, VariantValue> properties = CreateProperties(current.Item1, current.Item2);
        _ = EmitAsync(properties);
    }

    private async Task EmitAsync(Dictionary<string, VariantValue> properties)
    {
        try
        {
            DBusConnection bus = await DBusSession.GetConnectionAsync().ConfigureAwait(false);
            MessageWriter writer = bus.GetMessageWriter();

            try
            {
                writer.WriteSignalHeader(destination: null, path: ObjectPath, @interface: "com.canonical.Unity.LauncherEntry",
                    member: "Update", signature: "sa{sv}");
                writer.WriteString(applicationUri);
                writer.WriteDictionary(properties);
                bus.TrySendMessage(writer.CreateMessage());
            }
            finally
            {
                writer.Dispose();
            }
        }
        catch (Exception e) when (DBusSession.IsExpectedFailure(e))
        {
            // Progress is cosmetic. Without a bus there is nobody to show it.
        }
    }
}

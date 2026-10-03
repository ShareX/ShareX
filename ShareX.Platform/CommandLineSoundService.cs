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

using ShareX.Platform.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShareX.Platform;

/// <summary>Plays sounds with the first available command line player: pw-play, paplay or aplay on Linux, afplay on macOS.</summary>
public sealed class CommandLineSoundService : ISoundService
{
    private static readonly TimeSpan PlayTimeout = TimeSpan.FromSeconds(30);

    private readonly ICommandRunner runner;
    private readonly IReadOnlyList<string> players;
    private readonly FeatureSupport missing;
    private readonly string temporaryDirectory;

    /// <param name="players">Players in order of preference. Each takes the file to play as its only argument.</param>
    /// <param name="missing">The reason given when none of them is installed.</param>
    public CommandLineSoundService(ICommandRunner runner, IReadOnlyList<string> players, FeatureSupport missing)
        : this(runner, players, missing, Path.GetTempPath())
    {
    }

    internal CommandLineSoundService(ICommandRunner runner, IReadOnlyList<string> players, FeatureSupport missing, string temporaryDirectory)
    {
        this.runner = runner;
        this.players = players;
        this.missing = missing;
        this.temporaryDirectory = temporaryDirectory;
    }

    public FeatureSupport Support => Player != null ? FeatureSupport.Supported : missing;

    private string? Player => players.FirstOrDefault(runner.Exists);

    public void Play(byte[] wav)
    {
        if (Player == null || wav.Length == 0)
        {
            return;
        }

        // The players read files; they cannot all read standard input.
        string file = Path.Combine(temporaryDirectory, "sharex-sound-" + Guid.NewGuid().ToString("N") + ".wav");

        try
        {
            File.WriteAllBytes(file, wav);
            PlayFile(file);
        }
        finally
        {
            try
            {
                File.Delete(file);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }

    public void PlayFile(string filePath)
    {
        string? player = Player;

        if (player == null || !File.Exists(filePath))
        {
            return;
        }

        try
        {
            Task.Run(() => runner.RunAsync(player, [filePath], timeout: PlayTimeout)).Wait(PlayTimeout + TimeSpan.FromSeconds(5));
        }
        catch (AggregateException e) when (e.InnerException is TimeoutException or System.ComponentModel.Win32Exception or IOException)
        {
            // A sound that does not play is not worth an error.
        }
    }
}

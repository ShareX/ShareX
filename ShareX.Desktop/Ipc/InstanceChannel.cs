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

using ShareX.Desktop.Commands;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Desktop.Ipc;

public sealed record CommandResponse(bool Ok, string Message);

/// <summary>
/// One running instance per user. Later starts send their command to it over a named pipe
/// (a Unix domain socket on Linux and macOS) and exit, which is what a compositor key binding does.
/// </summary>
public static class InstanceChannel
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static string GetPipeName() => "sharex-" + Sanitize(Environment.UserName);

    internal static string Sanitize(string value)
    {
        StringBuilder builder = new StringBuilder(value.Length);

        foreach (char c in value)
        {
            builder.Append(char.IsLetterOrDigit(c) || c is '-' or '_' ? c : '_');
        }

        return builder.ToString();
    }

    internal static string Serialize(DesktopCommand command) => JsonSerializer.Serialize(command, JsonOptions);

    internal static DesktopCommand? DeserializeCommand(string json) => JsonSerializer.Deserialize<DesktopCommand>(json, JsonOptions);

    /// <summary>Sends the command to the running instance. Returns null when none is running.</summary>
    public static async Task<CommandResponse?> TrySendAsync(string pipeName, DesktopCommand command, TimeSpan connectTimeout, TimeSpan responseTimeout, CancellationToken cancellationToken = default)
    {
        using NamedPipeClientStream client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

        try
        {
            using CancellationTokenSource connectCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            connectCts.CancelAfter(connectTimeout);
            await client.ConnectAsync(connectCts.Token).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is OperationCanceledException or IOException or TimeoutException)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return null;
        }

        using StreamWriter writer = new StreamWriter(client, new UTF8Encoding(false), leaveOpen: true) { AutoFlush = true };
        using StreamReader reader = new StreamReader(client, Encoding.UTF8, leaveOpen: true);
        await writer.WriteLineAsync(Serialize(command).AsMemory(), cancellationToken).ConfigureAwait(false);

        using CancellationTokenSource responseCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        responseCts.CancelAfter(responseTimeout);
        string? line = await reader.ReadLineAsync(responseCts.Token).ConfigureAwait(false);

        return line == null ? new CommandResponse(false, "The running instance closed the connection.") : JsonSerializer.Deserialize<CommandResponse>(line, JsonOptions);
    }

    /// <summary>
    /// Starts listening. Returns null when another process already owns the pipe, which means an instance is running.
    /// </summary>
    public static InstanceServer? TryStartServer(string pipeName, Func<DesktopCommand, CancellationToken, Task<CommandResponse>> handler)
    {
        // Named pipes allow many servers with one name, so the lock file is what makes an instance unique.
        // The operating system releases it when the process ends, even after a crash.
        FileStream? instanceLock = TryAcquireLock(pipeName);

        if (instanceLock == null)
        {
            return null;
        }

        NamedPipeServerStream first;

        try
        {
            first = CreateServerStream(pipeName);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            instanceLock.Dispose();
            return null;
        }

        InstanceServer server = new InstanceServer(pipeName, first, instanceLock, handler);
        server.Start();
        return server;
    }

    private static FileStream? TryAcquireLock(string pipeName)
    {
        string path = Path.Combine(Path.GetTempPath(), pipeName + ".lock");

        try
        {
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 1, FileOptions.DeleteOnClose);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    internal static NamedPipeServerStream CreateServerStream(string pipeName) =>
        new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);

    internal static JsonSerializerOptions Options => JsonOptions;
}

public sealed class InstanceServer : IAsyncDisposable
{
    private readonly string pipeName;
    private readonly Func<DesktopCommand, CancellationToken, Task<CommandResponse>> handler;
    private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
    private readonly FileStream instanceLock;
    private NamedPipeServerStream? next;
    private Task? loop;

    internal InstanceServer(string pipeName, NamedPipeServerStream first, FileStream instanceLock, Func<DesktopCommand, CancellationToken, Task<CommandResponse>> handler)
    {
        this.pipeName = pipeName;
        this.handler = handler;
        this.instanceLock = instanceLock;
        next = first;
    }

    internal void Start() => loop = Task.Run(AcceptLoopAsync);

    private async Task AcceptLoopAsync()
    {
        while (!cancellation.IsCancellationRequested)
        {
            NamedPipeServerStream stream = next ?? InstanceChannel.CreateServerStream(pipeName);
            next = null;

            try
            {
                await stream.WaitForConnectionAsync(cancellation.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                await stream.DisposeAsync().ConfigureAwait(false);
                return;
            }
            catch (IOException)
            {
                await stream.DisposeAsync().ConfigureAwait(false);
                continue;
            }

            // Serve each client on its own task so a long upload does not block the next key press.
            _ = Task.Run(() => ServeAsync(stream));
        }
    }

    private async Task ServeAsync(NamedPipeServerStream stream)
    {
        using (stream)
        {
            try
            {
                using StreamReader reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
                using StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true) { AutoFlush = true };
                string? line = await reader.ReadLineAsync(cancellation.Token).ConfigureAwait(false);
                CommandResponse response;

                try
                {
                    DesktopCommand? command = line == null ? null : InstanceChannel.DeserializeCommand(line);
                    response = command == null ? new CommandResponse(false, "Empty command.") : await handler(command, cancellation.Token).ConfigureAwait(false);
                }
                catch (JsonException)
                {
                    response = new CommandResponse(false, "The command could not be read.");
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    response = new CommandResponse(false, ex.Message);
                }

                await writer.WriteLineAsync(JsonSerializer.Serialize(response, InstanceChannel.Options).AsMemory(), cancellation.Token).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is IOException or OperationCanceledException or ObjectDisposedException)
            {
                // The client went away. Nothing to report to.
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        cancellation.Cancel();
        next?.Dispose();

        if (loop != null)
        {
            try
            {
                await loop.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        cancellation.Dispose();
        await instanceLock.DisposeAsync().ConfigureAwait(false);
    }
}

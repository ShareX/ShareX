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

using Microsoft.Data.Sqlite;
using SkiaSharp;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;
using ApplicationFiles = ShareX.ImageEditor.Tests.WindowsApplicationSmokeTests.ApplicationFiles;

namespace ShareX.ImageEditor.Tests;

[Collection("Windows application verification")]
public sealed class WindowsApplicationUploadTests
{
    [WindowsApplicationSmokeFact]
    public Task BinaryUploadCompletesAndPersistsHistoryBeforeAutomaticExit() => VerifyUploadAsync(false);

    [WindowsApplicationSmokeFact]
    public Task ImageUploadCompletesAndPersistsHistoryBeforeAutomaticExit() => VerifyUploadAsync(true);

    private static async Task VerifyUploadAsync(bool image)
    {
        if (!OperatingSystem.IsWindows()) return;
        using ApplicationFiles files = new();
        await using LoopbackUploadServer server = new();
        byte[] payload = CreatePayload(image);
        string path = Path.Combine(files.DirectoryPath, image ? "synthetic image 背景.png" : "synthetic payload 背景.bin");
        File.WriteAllBytes(path, payload);
        ConfigureLocalUpload(files, server.UploadUrl);

        string log = await files.RunAsync(image ? "windows-application-image-upload.log" : "windows-application-binary-upload.log",
            path, "-portable", "-multi", "-silent", "-NoHotkeys", "-AutoClose");
        UploadRequest request = await server.Request.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal("POST " + server.UploadUrl.AbsolutePath + " HTTP/1.1", request.RequestLine);
        Assert.Equal(server.UploadUrl.Authority, request.Headers["Host"]);
        Assert.Equal(image ? "image/png" : "application/octet-stream", request.Headers["Content-Type"]);
        Assert.Equal(payload, request.Body);
        Assert.Equal(payload, File.ReadAllBytes(path));
        Assert.Contains("Task completed. File name: " + Path.GetFileName(path), log);
        Assert.DoesNotContain("Task failed.", log);
        Assert.Contains("ShareX closed.", log);
        VerifyHistory(files, path, server.ResultUrl, image);
        using (FileStream source = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            Assert.Equal(payload.Length, source.Length);
        using (FileStream history = File.Open(Path.Combine(files.SettingsPath, "History.db"), FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            Assert.True(history.Length > 0);
    }

    private static byte[] CreatePayload(bool image)
    {
        if (!image) return Enumerable.Range(0, 65537).Select(index => (byte)(index * 73 + 19)).ToArray();
        using SKBitmap bitmap = new(7, 5, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        bitmap.Erase(new SKColor(17, 83, 211, 190));
        bitmap.SetPixel(0, 0, SKColors.Red);
        bitmap.SetPixel(6, 4, SKColors.Blue);
        using SKImage pixels = SKImage.FromBitmap(bitmap);
        using SKData png = pixels.Encode(SKEncodedImageFormat.Png, 100);
        return png.ToArray();
    }

    private static void ConfigureLocalUpload(ApplicationFiles files, Uri uploadUrl)
    {
        Assert.Equal("http", uploadUrl.Scheme);
        Assert.Equal(IPAddress.Loopback.ToString(), uploadUrl.Host);
        string settingsPath = Path.Combine(files.SettingsPath, "ApplicationConfig.json");
        JsonObject settings = JsonNode.Parse(File.ReadAllText(settingsPath))!.AsObject();
        settings["DisableUpload"] = false;
        settings["MaxUploadFailRetry"] = 0;
        settings["TrayIconProgressEnabled"] = false;
        settings["TaskbarProgressEnabled"] = false;
        settings["HistorySaveTasks"] = true;
        settings["HistoryCheckURL"] = true;
        settings["ProxySettings"] = new JsonObject { ["ProxyMethod"] = "None" };
        settings["DefaultTaskSettings"] = JsonNode.Parse("""
            {
              "AfterCaptureJob": "None", "AfterUploadJob": "None",
              "ImageDestination": "CustomImageUploader", "ImageFileDestination": "CustomFileUploader",
              "TextDestination": "CustomTextUploader", "TextFileDestination": "CustomFileUploader",
              "FileDestination": "CustomFileUploader", "ExternalPrograms": [],
              "GeneralSettings": {
                "PlaySoundAfterCapture": false, "PlaySoundAfterUpload": false, "PlaySoundAfterAction": false,
                "ShowToastNotificationAfterTaskCompleted": false
              },
              "UploadSettings": { "FileUploadUseNamePattern": false, "UploaderFilters": [] },
              "AdvancedSettings": { "ProcessImagesDuringFileUpload": false }
            }
            """);
        File.WriteAllText(settingsPath, settings.ToJsonString());
        File.WriteAllText(Path.Combine(files.SettingsPath, "UploadersConfig.json"), JsonSerializer.Serialize(new
        {
            CustomFileUploaderSelected = 0,
            CustomImageUploaderSelected = 0,
            CustomTextUploaderSelected = 0,
            CustomUploadersList = new[]
            {
                new
                {
                    Name = "Local fixture", DestinationType = "ImageUploader, TextUploader, FileUploader",
                    RequestMethod = "POST", RequestURL = uploadUrl.AbsoluteUri, Body = "Binary", URL = "{json:url}"
                }
            }
        }));
    }

    private static void VerifyHistory(ApplicationFiles files, string source, string resultUrl, bool image)
    {
        SqliteConnectionStringBuilder connectionString = new()
        {
            DataSource = Path.Combine(files.SettingsPath, "History.db"), Mode = SqliteOpenMode.ReadOnly, Pooling = false
        };
        using SqliteConnection connection = new(connectionString.ToString());
        connection.Open();
        using (SqliteCommand integrity = connection.CreateCommand())
        {
            integrity.CommandText = "PRAGMA integrity_check;";
            Assert.Equal("ok", integrity.ExecuteScalar());
        }
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT FileName, FilePath, URL, Type FROM History;";
        using SqliteDataReader reader = command.ExecuteReader();
        Assert.True(reader.Read(), "The completed CLI upload must be saved before automatic shutdown.");
        Assert.Equal(Path.GetFileName(source), reader.GetString(0));
        Assert.Equal(source, reader.GetString(1));
        Assert.Equal(resultUrl, reader.GetString(2));
        Assert.Equal(image ? "Image" : "File", reader.GetString(3));
        Assert.False(reader.Read(), "Each isolated run must save exactly one history row.");
    }

    private sealed record UploadRequest(string RequestLine, Dictionary<string, string> Headers, byte[] Body);

    private sealed class LoopbackUploadServer : IAsyncDisposable
    {
        private readonly TcpListener listener = new(IPAddress.Loopback, 0);
        private readonly CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(35));
        public Uri UploadUrl { get; }
        public string ResultUrl { get; }
        public Task<UploadRequest> Request { get; }

        public LoopbackUploadServer()
        {
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            string token = Guid.NewGuid().ToString("N");
            UploadUrl = new Uri($"http://127.0.0.1:{port}/upload/{token}");
            ResultUrl = $"http://127.0.0.1:{port}/result/{token}";
            Request = ReceiveAsync(cancellation.Token);
        }

        private async Task<UploadRequest> ReceiveAsync(CancellationToken token)
        {
            using TcpClient client = await listener.AcceptTcpClientAsync(token);
            Assert.Equal(IPAddress.Loopback, ((IPEndPoint)client.Client.RemoteEndPoint!).Address);
            using NetworkStream stream = client.GetStream();
            string requestLine = await ReadLineAsync(stream, token);
            Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase);
            for (int count = 0; ; count++)
            {
                if (count >= 64) throw new InvalidDataException("Too many fixture request headers.");
                string line = await ReadLineAsync(stream, token);
                if (line.Length == 0) break;
                int separator = line.IndexOf(':');
                if (separator < 1) throw new InvalidDataException("Invalid fixture request header.");
                headers.Add(line[..separator], line[(separator + 1)..].Trim());
            }
            int length = int.Parse(headers["Content-Length"], CultureInfo.InvariantCulture);
            if (length < 0 || length > 100000) throw new InvalidDataException("Unexpected fixture body length.");
            if (headers.TryGetValue("Expect", out string? expect) && expect.Equals("100-continue", StringComparison.OrdinalIgnoreCase))
                await stream.WriteAsync("HTTP/1.1 100 Continue\r\n\r\n"u8.ToArray(), token);
            byte[] body = new byte[length];
            await stream.ReadExactlyAsync(body, token);
            byte[] response = JsonSerializer.SerializeToUtf8Bytes(new { url = ResultUrl });
            byte[] responseHeaders = Encoding.ASCII.GetBytes($"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: {response.Length}\r\nConnection: close\r\n\r\n");
            await stream.WriteAsync(responseHeaders, token);
            await stream.WriteAsync(response, token);
            return new UploadRequest(requestLine, headers, body);
        }

        private static async Task<string> ReadLineAsync(NetworkStream stream, CancellationToken token)
        {
            using MemoryStream bytes = new();
            byte[] current = new byte[1];
            while (bytes.Length < 8192)
            {
                await stream.ReadExactlyAsync(current, token);
                if (current[0] == '\n') return Encoding.ASCII.GetString(bytes.ToArray()).TrimEnd('\r');
                bytes.WriteByte(current[0]);
            }
            throw new InvalidDataException("Fixture request header is too long.");
        }

        public async ValueTask DisposeAsync()
        {
            cancellation.Cancel();
            listener.Stop();
            try { await Request; }
            catch (OperationCanceledException) { }
            finally { cancellation.Dispose(); }
        }
    }
}

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

using ShareX.Desktop.Settings;
using ShareX.Desktop.Workflows;
using ShareX.Platform;
using ShareX.UploadersLib;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ShareX.Desktop.Tests;

/// <summary>
/// Runs the real UploadersLib custom uploader against a server on localhost, so the whole upload path is exercised
/// without sending anything to a third party.
/// </summary>
public sealed class UploadServiceTests : IDisposable
{
    private static readonly object InitLock = new object();

    private readonly string folder = Path.Combine(Path.GetTempPath(), "sharex-upload-" + Guid.NewGuid().ToString("N"));
    private readonly HttpListener listener = new HttpListener();
    private readonly string baseUrl;
    private readonly CancellationTokenSource stop = new CancellationTokenSource();
    private int status = 200;
    private string? lastBody;

    public UploadServiceTests()
    {
        lock (InitLock)
        {
            // UploadersConfig encrypts secrets through the platform services, like the real application does.
            if (!PlatformServices.IsInitialized)
            {
                PlatformServices.Initialize(new FakePlatform(Path.Combine(Path.GetTempPath(), "sharex-upload-platform-" + Guid.NewGuid().ToString("N"))));
            }
        }

        Directory.CreateDirectory(folder);
        int port = GetFreePort();
        baseUrl = $"http://127.0.0.1:{port}/";
        listener.Prefixes.Add(baseUrl);
        listener.Start();
        _ = Task.Run(ServeAsync);
    }

    public void Dispose()
    {
        stop.Cancel();
        listener.Close();

        if (Directory.Exists(folder))
        {
            Directory.Delete(folder, true);
        }
    }

    private static int GetFreePort()
    {
        System.Net.Sockets.TcpListener probe = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
        probe.Start();
        int port = ((IPEndPoint)probe.LocalEndpoint).Port;
        probe.Stop();
        return port;
    }

    private async Task ServeAsync()
    {
        while (!stop.IsCancellationRequested)
        {
            HttpListenerContext context;

            try
            {
                context = await listener.GetContextAsync();
            }
            catch (Exception ex) when (ex is HttpListenerException or ObjectDisposedException or InvalidOperationException)
            {
                return;
            }

            using StreamReader reader = new StreamReader(context.Request.InputStream, Encoding.Latin1);
            lastBody = await reader.ReadToEndAsync();
            byte[] answer = Encoding.UTF8.GetBytes(status == 200 ? "{\"url\":\"https://files.example.test/uploaded.png\"}" : "server exploded");
            context.Response.StatusCode = status;
            await context.Response.OutputStream.WriteAsync(answer);
            context.Response.Close();
        }
    }

    private UploadersUploadService CreateService(string imageUploader)
    {
        UploadersConfig config = new UploadersConfig();
        CustomUploaderItem item = CustomUploaderItem.Init();
        item.Name = "Local test server";
        item.DestinationType = CustomUploaderDestinationType.ImageUploader;
        item.RequestURL = baseUrl + "upload";
        item.FileFormName = "file";
        item.URL = "{json:url}";
        config.CustomUploadersList.Add(item);
        config.CustomImageUploaderSelected = 0;
        config.Save(Path.Combine(folder, "UploadersConfig.json"));

        return new UploadersUploadService(new DesktopSettings { ImageUploader = imageUploader }, folder);
    }

    [Fact]
    public async Task Upload_SendsTheFileAndReturnsTheUrlFromTheResponse()
    {
        UploadersUploadService service = CreateService("CustomImageUploader");

        Assert.True(service.IsConfigured(isImage: true, out string? reason), reason);
        UploadOutcome outcome = await service.UploadAsync("shot.png", new byte[] { 1, 2, 3, 4, 5 }, isImage: true, CancellationToken.None);

        Assert.True(outcome.Success, outcome.Error + " / body=" + lastBody + " / status=" + status);
        Assert.Equal("https://files.example.test/uploaded.png", outcome.Url);
        Assert.Contains("filename=\"shot.png\"", lastBody);
        Assert.Contains("name=\"file\"", lastBody);
    }

    [Fact]
    public async Task Upload_ServerError_BecomesAReadableFailure()
    {
        status = 500;
        UploadersUploadService service = CreateService("CustomImageUploader");

        UploadOutcome outcome = await service.UploadAsync("shot.png", new byte[] { 1 }, isImage: true, CancellationToken.None);

        Assert.False(outcome.Success);
        Assert.Null(outcome.Url);
        Assert.False(string.IsNullOrWhiteSpace(outcome.Error));
    }

    [Theory]
    [InlineData("", "No upload destination is set")]
    [InlineData("NotARealUploader", "not a known uploader")]
    public void IsConfigured_RequiresAKnownDestination_SoNothingGoesToAThirdPartyByDefault(string uploader, string expectedReason)
    {
        UploadersUploadService service = CreateService(uploader);

        Assert.False(service.IsConfigured(isImage: true, out string? reason));
        Assert.Contains(expectedReason, reason);
    }

    [Fact]
    public void IsConfigured_FileUploads_NeedTheirOwnDestination()
    {
        UploadersUploadService service = CreateService("CustomImageUploader");

        Assert.False(service.IsConfigured(isImage: false, out string? reason));
        Assert.Contains("FileUploader", reason);
    }
}

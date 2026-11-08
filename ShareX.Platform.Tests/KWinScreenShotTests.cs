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

using Microsoft.Win32.SafeHandles;
using ShareX.Platform.Imaging;
using ShareX.Platform.Linux.DBus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using Xunit;

namespace ShareX.Platform.Tests;

public class KWinScreenShotTests
{
    [Fact]
    public void BgraRowsAreCopiedWithoutTheirPadding()
    {
        // 2x2, stride 12: two pixels and four bytes of padding per row. Format 6 is premultiplied ARGB32 (BGRA in memory).
        byte[] data =
        [
            1, 2, 3, 255, 4, 5, 6, 255, 99, 99, 99, 99,
            7, 8, 9, 255, 10, 11, 12, 128, 99, 99, 99, 99
        ];

        PixelBuffer image = KWinScreenShot.DecodeRaw(data, new KWinImage(2, 2, 12, 6));

        Assert.Equal(new byte[] { 1, 2, 3, 255, 4, 5, 6, 255, 7, 8, 9, 255, 10, 11, 12, 255 }, image.Pixels);
    }

    [Fact]
    public void RgbaIsSwappedToBgra()
    {
        PixelBuffer image = KWinScreenShot.DecodeRaw([10, 20, 30, 255], new KWinImage(1, 1, 4, 17));

        Assert.Equal(new byte[] { 30, 20, 10, 255 }, image.Pixels);
    }

    [LinuxPortalFact]
    public async Task ReadsThePixelsKWinWritesAfterItReplies()
    {
        await WithKWin(null, async (kwin, connection) =>
        {
            PixelBuffer image = await KWinScreenShot.CaptureWorkspaceAsync(connection, includeCursor: true, CancellationToken.None);

            Assert.Equal(2, image.Width);
            Assert.Equal(1, image.Height);
            Assert.Equal(new byte[] { 1, 2, 3, 255, 4, 5, 6, 255 }, image.Pixels);
            Assert.True(kwin.IncludeCursor);
            Assert.True(kwin.NativeResolution);
        });
    }

    [LinuxPortalFact]
    public async Task RefusalReachesTheCallerSoThePortalIsUsed()
    {
        await WithKWin("org.kde.KWin.ScreenShot2.Error.NoAuthorized", async (_, connection) =>
        {
            DBusErrorReplyException error = await Assert.ThrowsAsync<DBusErrorReplyException>(() =>
                KWinScreenShot.CaptureWorkspaceAsync(connection, includeCursor: false, CancellationToken.None));
            Assert.Equal("org.kde.KWin.ScreenShot2.Error.NoAuthorized", error.ErrorName);
        });
    }

    [Theory]
    [InlineData(3u, 4u, 4)]   // RGB16 and other formats are not used for screenshots.
    [InlineData(5u, 2u, 4)]   // Stride shorter than a row.
    [InlineData(5u, 4u, 3)]   // Fewer bytes than KWin described.
    public void UnreadableImagesAreRejected(uint format, uint stride, int length)
    {
        Assert.Throws<InvalidDataException>(() => KWinScreenShot.DecodeRaw(new byte[length], new KWinImage(1, 1, stride, format)));
    }

    private static async Task WithKWin(string? refusal, Func<FakeKWin, DBusConnection, Task> verify)
    {
        ProcessStartInfo start = new ProcessStartInfo("/usr/bin/dbus-daemon")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        start.ArgumentList.Add("--session");
        start.ArgumentList.Add("--nofork");
        start.ArgumentList.Add("--print-address=1");
        using Process daemon = Process.Start(start)!;

        try
        {
            string address = (await daemon.StandardOutput.ReadLineAsync().WaitAsync(TimeSpan.FromSeconds(5)))!;
            using DBusConnection server = new DBusConnection(address);
            await server.ConnectAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(5));
            FakeKWin kwin = new FakeKWin(refusal);
            server.AddMethodHandler(kwin);
            await server.RequestNameAsync(KWinScreenShot.BusName).WaitAsync(TimeSpan.FromSeconds(5));
            using DBusConnection client = new DBusConnection(address);
            await client.ConnectAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(5));
            await verify(kwin, client).WaitAsync(TimeSpan.FromSeconds(15));
        }
        finally
        {
            if (!daemon.HasExited)
            {
                daemon.Kill();
            }

            await daemon.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5));
        }
    }

    /// <summary>Answers CaptureWorkspace like KWin 6: the reply first, then a 2x1 premultiplied ARGB32 image on the descriptor.</summary>
    private sealed class FakeKWin(string? refusal) : IPathMethodHandler
    {
        public string Path => KWinScreenShot.ObjectPath;
        public bool HandlesChildPaths => false;
        public bool IncludeCursor { get; private set; }
        public bool NativeResolution { get; private set; }

        public ValueTask HandleMethodAsync(MethodContext context)
        {
            Message request = context.Request;

            if (request.InterfaceAsString != KWinScreenShot.BusName || request.MemberAsString != "CaptureWorkspace")
            {
                context.ReplyUnknownMethodError();
                return ValueTask.CompletedTask;
            }

            if (refusal != null)
            {
                context.ReplyError(refusal, "The process is not authorized to take a screenshot");
                return ValueTask.CompletedTask;
            }

            Reader reader = request.GetBodyReader();
            Dictionary<string, VariantValue> options = reader.ReadDictionaryOfStringToVariantValue();
            IncludeCursor = options["include-cursor"].GetBool();
            NativeResolution = options["native-resolution"].GetBool();
            SafeFileHandle target = reader.ReadHandle<SafeFileHandle>()!;

            using (MessageWriter reply = context.CreateReplyWriter("a{sv}"))
            {
                reply.WriteDictionary(new Dictionary<string, VariantValue>
                {
                    ["type"] = "raw",
                    ["width"] = 2u,
                    ["height"] = 1u,
                    ["stride"] = 8u,
                    ["format"] = 6u
                });
                context.Reply(reply.CreateMessage());
            }

            _ = Task.Run(async () =>
            {
                await Task.Delay(100);
                using FileStream stream = new FileStream(target, FileAccess.Write);
                stream.Write([1, 2, 3, 255, 4, 5, 6, 255]);
            });

            return ValueTask.CompletedTask;
        }
    }
}

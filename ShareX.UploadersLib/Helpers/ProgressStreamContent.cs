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

using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.UploadersLib
{
    internal sealed class ProgressStreamContent : HttpContent
    {
        private readonly Stream source;
        private readonly long startPosition;
        private readonly long contentLength;
        private readonly int bufferSize;
        private readonly Action<int> progressReporter;

        public ProgressStreamContent(Stream source, long startPosition, long contentLength, int bufferSize, Action<int> progressReporter)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.startPosition = startPosition;
            this.contentLength = contentLength;
            this.bufferSize = Math.Max(1, bufferSize);
            this.progressReporter = progressReporter;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return SerializeToStreamAsync(stream, context, CancellationToken.None);
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context, CancellationToken cancellationToken)
        {
            if (source.CanSeek)
            {
                source.Position = startPosition;
            }
            else if (startPosition != 0)
            {
                throw new InvalidOperationException("A non-seekable stream cannot be uploaded from a non-zero position.");
            }

            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

            try
            {
                long remaining = contentLength;

                while (remaining > 0)
                {
                    int count = (int)Math.Min(buffer.Length, remaining);
                    int bytesRead = await source.ReadAsync(buffer.AsMemory(0, count), cancellationToken).ConfigureAwait(false);

                    if (bytesRead == 0)
                    {
                        throw new EndOfStreamException("The upload stream ended before the declared content length was reached.");
                    }

                    await stream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
                    remaining -= bytesRead;
                    progressReporter?.Invoke(bytesRead);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = contentLength;
            return true;
        }
    }
}

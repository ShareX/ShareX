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
using System.IO;

namespace ShareX.UploadersLib
{
    internal sealed class SharedStreamSegment : Stream
    {
        private readonly Stream source;
        private readonly object syncRoot;
        private readonly long offset;
        private readonly long length;
        private readonly Action<int> readReporter;
        private long position;

        public SharedStreamSegment(Stream source, object syncRoot, long offset, long length, Action<int> readReporter = null)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.syncRoot = syncRoot ?? throw new ArgumentNullException(nameof(syncRoot));

            if (!source.CanSeek)
            {
                throw new ArgumentException("The source stream must be seekable.", nameof(source));
            }

            if (offset < 0 || offset > source.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (length < 0 || length > source.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            this.offset = offset;
            this.length = length;
            this.readReporter = readReporter;
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => length;

        public override long Position
        {
            get => position;
            set
            {
                if (value < 0 || value > length)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            ArgumentOutOfRangeException.ThrowIfNegative(offset);
            ArgumentOutOfRangeException.ThrowIfNegative(count);

            if (offset > buffer.Length - count)
            {
                throw new ArgumentException("The buffer is too small for the requested range.");
            }

            return Read(buffer.AsSpan(offset, count));
        }

        public override int Read(Span<byte> buffer)
        {
            long remaining = length - position;

            if (remaining <= 0 || buffer.IsEmpty)
            {
                return 0;
            }

            int count = (int)Math.Min(buffer.Length, remaining);
            int bytesRead;

            lock (syncRoot)
            {
                source.Position = offset + position;
                bytesRead = source.Read(buffer.Slice(0, count));
            }

            position += bytesRead;

            if (bytesRead > 0)
            {
                readReporter?.Invoke(bytesRead);
            }

            return bytesRead;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<int>(cancellationToken);
            }

            try
            {
                return Task.FromResult(Read(buffer, offset, count));
            }
            catch (Exception e)
            {
                return Task.FromException<int>(e);
            }
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ValueTask.FromCanceled<int>(cancellationToken);
            }

            try
            {
                return ValueTask.FromResult(Read(buffer.Span));
            }
            catch (Exception e)
            {
                return ValueTask.FromException<int>(e);
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long target = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => position + offset,
                SeekOrigin.End => length + offset,
                _ => throw new ArgumentOutOfRangeException(nameof(origin))
            };

            Position = target;
            return position;
        }

        public override void Flush()
        {
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}

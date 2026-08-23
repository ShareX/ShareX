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

using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.UploadersLib
{
    public abstract class TextUploader : GenericUploader
    {
        protected sealed override async Task<UploadResult> UploadCoreAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8, true, BufferSize, true);
            string text = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
            return await UploadTextCoreAsync(text, fileName, cancellationToken).ConfigureAwait(false);
        }

        public Task<UploadResult> UploadTextAsync(string text, string fileName, CancellationToken cancellationToken = default)
        {
            return RunOperationAsync(token => UploadTextCoreAsync(text, fileName, token), cancellationToken);
        }

        protected abstract Task<UploadResult> UploadTextCoreAsync(string text, string fileName, CancellationToken cancellationToken);

        public async Task<UploadResult> UploadTextFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (File.Exists(filePath))
            {
                await using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
                    BufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
                return await UploadAsync(stream, Path.GetFileName(filePath), cancellationToken).ConfigureAwait(false);
            }

            return null;
        }
    }
}

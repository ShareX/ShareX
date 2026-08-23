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

using ShareX.HelpersLib;
using System;
using System.IO;

namespace ShareX.UploadersLib.FileUploaders
{
    public class CustomFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.CustomFileUploader;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.CustomUploadersList != null && config.CustomUploadersList.IsValidIndex(config.CustomFileUploaderSelected);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            int index;

            if (taskInfo.OverrideCustomUploader)
            {
                index = taskInfo.CustomUploaderIndex.BetweenOrDefault(0, config.CustomUploadersList.Count - 1);
            }
            else
            {
                index = config.CustomFileUploaderSelected;
            }

            CustomUploaderItem customUploader = config.CustomUploadersList.ReturnIfValidIndex(index);

            if (customUploader != null)
            {
                return new CustomFileUploader(customUploader);
            }

            return null;
        }
    }

    public sealed class CustomFileUploader : FileUploader
    {
        private CustomUploaderItem uploader;

        public CustomFileUploader(CustomUploaderItem customUploaderItem)
        {
            uploader = customUploaderItem;
        }

        protected override async Task<UploadResult> UploadCoreAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            UploadResult result = new UploadResult();
            CustomUploaderInput input = new CustomUploaderInput(fileName, "");

            if (uploader.Body == CustomUploaderBody.MultipartFormData)
            {
                result = await SendRequestFileAsync(uploader.GetRequestURL(input), stream, fileName, uploader.GetFileFormName(), uploader.GetArguments(input),
                    uploader.GetHeaders(input), null, uploader.RequestMethod, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else if (uploader.Body == CustomUploaderBody.Binary)
            {
                result.Response = await SendRequestAsync(uploader.RequestMethod, uploader.GetRequestURL(input), stream, MimeTypes.GetMimeTypeFromFileName(fileName), null,
                    uploader.GetHeaders(input), cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                throw new Exception(string.Format(Localization.Strings.CustomUploader_Unsupported_request_format, uploader.Body));
            }

            uploader.TryParseResponse(result, LastResponseInfo, Errors, input);

            return result;
        }
    }
}

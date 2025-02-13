#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using System.Text;

namespace ShareX.UploadersLib.TextUploaders
{
    public class CustomTextUploaderService : TextUploaderService
    {
        public override TextDestination EnumValue { get; } = TextDestination.CustomTextUploader;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.CustomUploadersList != null && config.CustomUploadersList.IsValidIndex(config.CustomTextUploaderSelected);
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
                index = config.CustomTextUploaderSelected;
            }

            CustomUploaderItem customUploader = config.CustomUploadersList.ReturnIfValidIndex(index);

            if (customUploader != null)
            {
                return new CustomTextUploader(customUploader);
            }

            return null;
        }
    }

    public sealed class CustomTextUploader : TextUploader
    {
        private CustomUploaderItem uploader;

        public CustomTextUploader(CustomUploaderItem customUploaderItem)
        {
            uploader = customUploaderItem;
        }

        public override UploadResult UploadText(string text, string fileName)
        {
            UploadResult result = new UploadResult();
            CustomUploaderInput input = new CustomUploaderInput(fileName, text);

            if (uploader.Body == CustomUploaderBody.None)
            {
                result.Response = SendRequest(uploader.RequestMethod, uploader.GetRequestURL(input), null, uploader.GetHeaders(input));
            }
            else if (uploader.Body == CustomUploaderBody.MultipartFormData)
            {
                if (string.IsNullOrEmpty(uploader.FileFormName))
                {
                    result.Response = SendRequestMultiPart(uploader.GetRequestURL(input), uploader.GetArguments(input), uploader.GetHeaders(input),
                        null, uploader.RequestMethod);
                }
                else
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(text);
                    using (MemoryStream stream = new MemoryStream(bytes))
                    {
                        result = SendRequestFile(uploader.GetRequestURL(input), stream, fileName, uploader.GetFileFormName(), uploader.GetArguments(input),
                            uploader.GetHeaders(input), null, uploader.RequestMethod);
                    }
                }
            }
            else if (uploader.Body == CustomUploaderBody.FormURLEncoded)
            {
                result.Response = SendRequestURLEncoded(uploader.RequestMethod, uploader.GetRequestURL(input), uploader.GetArguments(input), uploader.GetHeaders(input));
            }
            else if (uploader.Body == CustomUploaderBody.JSON || uploader.Body == CustomUploaderBody.XML)
            {
                result.Response = SendRequest(uploader.RequestMethod, uploader.GetRequestURL(input), uploader.GetData(input), uploader.GetContentType(),
                    null, uploader.GetHeaders(input));
            }
            else if (uploader.Body == CustomUploaderBody.Binary)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    result.Response = SendRequest(uploader.RequestMethod, uploader.GetRequestURL(input), stream, MimeTypes.GetMimeTypeFromFileName(fileName),
                        null, uploader.GetHeaders(input));
                }
            }
            else
            {
                throw new Exception("Unsupported request format: " + uploader.Body);
            }

            uploader.TryParseResponse(result, LastResponseInfo, Errors, input);

            return result;
        }
    }
}
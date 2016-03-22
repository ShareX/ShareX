#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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

        public override bool CheckConfig(UploadersConfig uploadersConfig)
        {
            return uploadersConfig.CustomUploadersList != null && uploadersConfig.CustomUploadersList.IsValidIndex(uploadersConfig.CustomFileUploaderSelected);
        }

        public override FileUploader CreateUploader(UploadersConfig uploadersConfig)
        {
            // TODO: Check TaskSettings override index (WorkerTask.GetCustomUploader)
            CustomUploaderItem customUploader = uploadersConfig.CustomUploadersList.ReturnIfValidIndex(uploadersConfig.CustomFileUploaderSelected);

            if (customUploader != null)
            {
                return new CustomFileUploader(customUploader);
            }

            return null;
        }
    }

    public sealed class CustomFileUploader : FileUploader
    {
        private CustomUploaderItem customUploader;

        public CustomFileUploader(CustomUploaderItem customUploaderItem)
        {
            customUploader = customUploaderItem;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (customUploader.RequestType != CustomUploaderRequestType.POST)
            {
                throw new Exception("'Request type' must be 'POST' when using custom file uploader.");
            }

            UploadResult result = UploadData(stream, customUploader.GetRequestURL(), fileName, customUploader.GetFileFormName(), customUploader.GetArguments(),
                customUploader.GetHeaders(), responseType: customUploader.ResponseType);

            if (result.IsSuccess)
            {
                try
                {
                    customUploader.ParseResponse(result);
                }
                catch (Exception e)
                {
                    // TODO: Translate
                    Errors.Add("Response parse failed." + Environment.NewLine + e);
                }
            }

            return result;
        }
    }
}
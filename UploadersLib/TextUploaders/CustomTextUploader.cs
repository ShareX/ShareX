#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UploadersLib.HelperClasses;

namespace UploadersLib.TextUploaders
{
    public sealed class CustomTextUploader : TextUploader
    {
        private CustomUploaderItem customUploader;

        public CustomTextUploader(CustomUploaderItem customUploaderItem)
        {
            customUploader = customUploaderItem;
        }

        public override UploadResult UploadText(string text, string fileName)
        {
            if (string.IsNullOrEmpty(customUploader.RequestURL)) throw new Exception("'Request URL' must be not empty.");

            if ((customUploader.RequestType == CustomUploaderRequestType.GET || string.IsNullOrEmpty(customUploader.FileFormName)) &&
                (customUploader.Arguments == null || !customUploader.Arguments.Any(x => x.Value.Contains("%input") || x.Value.Contains("$input$"))))
                throw new Exception("Atleast one '%input' or '$input$' required for argument value when using GET or non-file POST.");

            UploadResult result = new UploadResult();

            Dictionary<string, string> args = customUploader.ParseArguments(text);

            if (customUploader.RequestType == CustomUploaderRequestType.POST)
            {
                if (string.IsNullOrEmpty(customUploader.FileFormName))
                {
                    result.Response = SendPostRequest(customUploader.RequestURL, args, customUploader.ResponseType);
                }
                else
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(text);
                    using (MemoryStream stream = new MemoryStream(byteArray))
                    {
                        result = UploadData(stream, customUploader.RequestURL, fileName, customUploader.FileFormName, args);
                    }
                }
            }
            else if (customUploader.RequestType == CustomUploaderRequestType.GET)
            {
                result.Response = SendGetRequest(customUploader.RequestURL, args, customUploader.ResponseType);
            }

            customUploader.ParseResponse(result);

            return result;
        }
    }
}
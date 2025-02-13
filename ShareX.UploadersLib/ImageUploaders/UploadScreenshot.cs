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
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace ShareX.UploadersLib.ImageUploaders
{
    public class UploadScreenshot : ImageUploader
    {
        private string APIKey { get; set; }

        public UploadScreenshot(string key)
        {
            APIKey = key;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("apiKey", APIKey);
            arguments.Add("xmlOutput", "1");
            //arguments.Add("testMode", "1");

            UploadResult result = SendRequestFile("http://img1.uploadscreenshot.com/api-upload.php", stream, fileName, "userfile", arguments);

            return ParseResult(result);
        }

        private UploadResult ParseResult(UploadResult result)
        {
            if (result.IsSuccess)
            {
                XDocument xdoc = XDocument.Parse(result.Response);
                XElement xele = xdoc.Root.Element("upload");

                string error = xele.GetElementValue("errorCode");
                if (!string.IsNullOrEmpty(error))
                {
                    string errorMessage;

                    switch (error)
                    {
                        case "1":
                            errorMessage = "The MD5 sum that you provided did not match the MD5 sum that we calculated for the uploaded image file." +
                                           " There may of been a network interruption during upload. Suggest that you try the upload again.";
                            break;
                        case "2":
                            errorMessage = "The apiKey that you provided does not exist or has been banned. Please contact us for more information.";
                            break;
                        case "3":
                            errorMessage = "The file that you provided was not a png or jpg.";
                            break;
                        case "4":
                            errorMessage = "The file that you provided was too large, currently the limit per file is 50MB.";
                            break;
                        case "99":
                        default:
                            errorMessage = "An unkown error occured, please contact the admin and include a copy of the file that you were trying to upload.";
                            break;
                    }

                    Errors.Add(errorMessage);
                }
                else
                {
                    result.URL = xele.GetElementValue("original");
                    result.ThumbnailURL = xele.GetElementValue("small");
                    result.DeletionURL = xele.GetElementValue("deleteurl");
                }
            }

            return result;
        }
    }
}
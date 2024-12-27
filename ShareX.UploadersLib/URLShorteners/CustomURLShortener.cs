#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2024 ShareX Team

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

using ShareX.HelpersLib.Extensions;
using ShareX.UploadersLib.BaseServices;
using ShareX.UploadersLib.BaseUploaders;
using ShareX.UploadersLib.CustomUploader;
using ShareX.UploadersLib.Helpers;

using System;

namespace ShareX.UploadersLib.URLShorteners;

public class CustomURLShortenerService : URLShortenerService
{
    public override UrlShortenerType EnumValue { get; } = UrlShortenerType.CustomURLShortener;

    public override bool CheckConfig(UploadersConfig config)
    {
        return config.CustomUploadersList != null && config.CustomUploadersList.IsValidIndex(config.CustomURLShortenerSelected);
    }

    public override URLShortener CreateShortener(UploadersConfig config, TaskReferenceHelper taskInfo)
    {
        int index = taskInfo.OverrideCustomUploader
            ? taskInfo.CustomUploaderIndex.BetweenOrDefault(0, config.CustomUploadersList.Count - 1)
            : config.CustomURLShortenerSelected;
        CustomUploaderItem customUploader = config.CustomUploadersList.ReturnIfValidIndex(index);

        return customUploader != null ? new CustomURLShortener(customUploader) : (URLShortener)null;
    }
}

public sealed class CustomURLShortener : URLShortener
{
    private CustomUploaderItem uploader;

    public CustomURLShortener(CustomUploaderItem customUploaderItem)
    {
        uploader = customUploaderItem;
    }

    public override UploadResult ShortenURL(string url)
    {
        UploadResult result = new() { URL = url };
        CustomUploaderInput input = new("", url);

        result.Response = uploader.Body == CustomUploaderBody.None
            ? SendRequest(uploader.RequestMethod, uploader.GetRequestURL(input), null, uploader.GetHeaders(input))
            : uploader.Body == CustomUploaderBody.MultipartFormData
                ? SendRequestMultiPart(uploader.GetRequestURL(input), uploader.GetArguments(input), uploader.GetHeaders(input), null, uploader.RequestMethod)
                : uploader.Body == CustomUploaderBody.FormURLEncoded
                            ? SendRequestURLEncoded(uploader.RequestMethod, uploader.GetRequestURL(input), uploader.GetArguments(input), uploader.GetHeaders(input))
                            : uploader.Body == CustomUploaderBody.JSON || uploader.Body == CustomUploaderBody.XML
                                        ? SendRequest(uploader.RequestMethod, uploader.GetRequestURL(input), uploader.GetData(input), uploader.GetContentType(),
                                                        null, uploader.GetHeaders(input))
                                        : throw new Exception("Unsupported request format: " + uploader.Body);
        uploader.TryParseResponse(result, LastResponseInfo, Errors, input, true);

        return result;
    }
}
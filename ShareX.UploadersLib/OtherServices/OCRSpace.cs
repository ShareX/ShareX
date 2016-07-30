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

using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace ShareX.UploadersLib.OtherServices
{
    public enum OCRSpaceLanguages
    {
        [Description("Czech")]
        ce,
        [Description("Danish")]
        dan,
        [Description("Dutch")]
        dut,
        [Description("English")]
        eng,
        [Description("Finnish")]
        fin,
        [Description("French")]
        fre,
        [Description("German")]
        ger,
        [Description("Hungarian")]
        hun,
        [Description("Italian")]
        ita,
        [Description("Norwegian")]
        nor,
        [Description("Polish")]
        pol,
        [Description("Portuguese")]
        por,
        [Description("Spanish")]
        spa,
        [Description("Swedish")]
        swe,
        [Description("Chinese Simplified")]
        chs,
        [Description("Greek")]
        gre,
        [Description("Japanese")]
        jpn,
        [Description("Russian")]
        rus,
        [Description("Turkish")]
        tur,
        [Description("Chinese Traditional")]
        cht,
        [Description("Korean")]
        kor
    }

    public class OCRSpace : Uploader
    {
        private const string APIURLFree = "https://api.ocr.space/parse/image";
        private const string APIURLUSA = "?";
        private const string APIURLEurope = "https://apipro3.ocr.space/parse/image"; // Frankfurt
        private const string APIURLAsia = "https://apipro8.ocr.space/parse/image"; // Tokyo

        public OCRSpaceLanguages Language { get; set; } = OCRSpaceLanguages.eng;
        public bool Overlay { get; set; }

        public OCRSpace(OCRSpaceLanguages language = OCRSpaceLanguages.eng, bool overlay = false)
        {
            Language = language;
            Overlay = overlay;
        }

        public OCRSpaceResponse DoOCR(Stream stream, string fileName)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("apikey", APIKeys.OCRSpaceAPIKey);
            //arguments.Add("url", "");
            arguments.Add("language", Language.ToString());
            arguments.Add("isOverlayRequired", Overlay.ToString());

            UploadResult ur = UploadData(stream, APIURLEurope, fileName, "file", arguments);

            if (ur.IsSuccess)
            {
                return JsonConvert.DeserializeObject<OCRSpaceResponse>(ur.Response);
            }

            return null;
        }
    }

    public class OCRSpaceResponse
    {
        public List<OCRSpaceParsedResult> ParsedResults { get; set; }
        public int OCRExitCode { get; set; }
        public bool IsErroredOnProcessing { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
        public string ProcessingTimeInMilliseconds { get; set; }
    }

    public class OCRSpaceParsedResult
    {
        public OCRSpaceTextOverlay TextOverlay { get; set; }
        public int FileParseExitCode { get; set; }
        public string ParsedText { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
    }

    public class OCRSpaceTextOverlay
    {
        public List<OCRSpaceLine> Lines { get; set; }
        public bool HasOverlay { get; set; }
        public string Message { get; set; }
    }

    public class OCRSpaceLine
    {
        public List<OCRSpaceWord> Words { get; set; }
        public int MaxHeight { get; set; }
        public int MinTop { get; set; }
    }

    public class OCRSpaceWord
    {
        public string WordText { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
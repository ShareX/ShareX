#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2022 ShareX Team

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
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace ShareX.UploadersLib.OtherServices
{
    public enum OCRSpaceLanguages
    {
        [Description("Arabic")]
        ara,
        [Description("Bulgarian")]
        bul,
        [Description("Chinese (Simplified)")]
        chs,
        [Description("Chinese (Traditional)")]
        cht,
        [Description("Croatian")]
        hrv,
        [Description("Czech")]
        cze,
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
        [Description("Greek")]
        gre,
        [Description("Hungarian")]
        hun,
        [Description("Korean")]
        kor,
        [Description("Italian")]
        ita,
        [Description("Japanese")]
        jpn,
        [Description("Norwegian")]
        nor,
        [Description("Polish")]
        pol,
        [Description("Portuguese")]
        por,
        [Description("Russian")]
        rus,
        [Description("Slovenian")]
        slv,
        [Description("Spanish")]
        spa,
        [Description("Swedish")]
        swe,
        [Description("Turkish")]
        tur
    }

    public class OCRSpace : Uploader
    {
        private const string APIURLUSA = "https://apipro1.ocr.space/parse/image";
        private const string APIURLEurope = "https://apipro2.ocr.space/parse/image";
        private const string APIURLAsia = "https://apipro3.ocr.space/parse/image";
        private const string APIURLFree = "https://api.ocr.space/parse/image";

        public string APIKey { get; set; }
        public OCRSpaceLanguages Language { get; set; } = OCRSpaceLanguages.eng;
        public bool Overlay { get; set; }
        public bool UseOCREngine2 { get; set; }

        public OCRSpace(string apiKey, OCRSpaceLanguages language = OCRSpaceLanguages.eng, bool overlay = false, bool useOCREngine2 = false)
        {
            APIKey = apiKey;
            Language = language;
            Overlay = overlay;
            UseOCREngine2 = useOCREngine2;
        }

        public OCRSpaceResponse DoOCR(Stream stream, string fileName)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("apikey", APIKey);

            if (UseOCREngine2)
            {
                arguments.Add("OCREngine", "2");
            }
            else
            {
                arguments.Add("language", Language.ToString());
            }

            arguments.Add("isOverlayRequired", Overlay.ToString());

            UploadResult ur = SendRequestFile(APIURLUSA, stream, fileName, "file", arguments);

            if (!ur.IsSuccess)
            {
                ur = SendRequestFile(APIURLEurope, stream, fileName, "file", arguments);
            }

            if (ur.IsSuccess)
            {
                return JsonConvert.DeserializeObject<OCRSpaceResponse>(ur.Response);
            }

            return null;
        }

        public static string DoOCR(OCRSpaceLanguages language, Stream stream, string fileName)
        {
            string result = null;

            try
            {
                OCRSpace ocr = new OCRSpace(APIKeys.OCRSpaceAPIKey, language, false, language == OCRSpaceLanguages.eng);
                OCRSpaceResponse response = ocr.DoOCR(stream, fileName);

                if (response != null && !response.IsErroredOnProcessing && response.ParsedResults.Count > 0)
                {
                    result = response.ParsedResults[0].ParsedText;

                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.ReplaceNewLines();
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return result;
        }

        public static Task<string> DoOCRAsync(OCRSpaceLanguages language, Stream stream, string fileName)
        {
            return Task.Run(() => DoOCR(language, stream, fileName));
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
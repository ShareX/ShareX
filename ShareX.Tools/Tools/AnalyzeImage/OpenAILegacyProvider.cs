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

#nullable disable

using ShareX.HelpersLib;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShareX.Tools
{
    public class ChatGPTLegacyRequest
    {
        public string model { get; set; }
        public ChatGPTLegacyMessage[] messages { get; set; }
    }

    public class ChatGPTLegacyMessage
    {
        public string role { get; set; }
        public ChatGPTLegacyContent[] content { get; set; }
    }

    public class ChatGPTLegacyContent
    {
        public string type { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string text { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ChatGPTLegacyImageUrl image_url { get; set; }
    }

    public class ChatGPTLegacyImageUrl
    {
        public string url { get; set; }
    }

    public class ChatGPTLegacyResponse
    {
        public string id { get; set; }
        public ChatGPTLegacyResponseChoice[] choices { get; set; }
    }

    public class ChatGPTLegacyResponseChoice
    {
        public ChatGPTLegacyResponseMessage message { get; set; }
    }

    public class ChatGPTLegacyResponseMessage
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class OpenAILegacyProvider : IAIProvider
    {
        public readonly string APIKey;
        public readonly string Model;
        public readonly string CustomURL;

        public OpenAILegacyProvider(string apiKey, string model, string customURL = null)
        {
            APIKey = apiKey;
            Model = model;
            CustomURL = customURL;
        }

        public async Task<string> AnalyzeImage(string filePath, string input = null, string reasoningEffort = null, string textVerbosity = null)
        {
            Image image = ImageHelpers.LoadImage(filePath);

            return await AnalyzeImage(image, input, reasoningEffort, textVerbosity);
        }

        public async Task<string> AnalyzeImage(Image image, string input = null, string reasoningEffort = null, string textVerbosity = null)
        {
            string imageDataUri;

            using (MemoryStream ms = new MemoryStream())
            {
                ImageHelpers.SaveJPEG(image, ms, 90);
                byte[] imageBytes = ms.ToArray();
                string base64Image = Convert.ToBase64String(imageBytes);
                imageDataUri = $"data:image/jpeg;base64,{base64Image}";
            }

            return await AnalyzeImageInternal(imageDataUri, input, reasoningEffort, textVerbosity);
        }

        private async Task<string> AnalyzeImageInternal(string imageDataUri, string input = null, string reasoningEffort = null, string textVerbosity = null)
        {
            HttpClient httpClient = HttpClientFactory.Create();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKey);

            if (string.IsNullOrEmpty(input))
            {
                input = Localization.Strings.AnalyzeImageViewModel_What_is_in_this_image;
            }

            ChatGPTLegacyRequest request = new ChatGPTLegacyRequest()
            {
                model = Model,
                messages = new ChatGPTLegacyMessage[]
                {
                    new ChatGPTLegacyMessage()
                    {
                        role = "user",
                        content = new ChatGPTLegacyContent[]
                        {
                            new ChatGPTLegacyContent()
                            {
                                type = "text",
                                text = input
                            },
                            new ChatGPTLegacyContent()
                            {
                                type = "image_url",
                                image_url = new ChatGPTLegacyImageUrl()
                                {
                                    url = imageDataUri
                                }
                            }
                        }
                    }
                }
            };

            string json = JsonSerializer.Serialize(request);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            string url;

            if (!string.IsNullOrEmpty(CustomURL))
            {
                url = CustomURL;
            }
            else
            {
                url = "https://api.openai.com";
            }

            string path = "/v1/chat/completions";

            if (!url.EndsWith(path))
            {
                url = URLHelpers.CombineURL(url, path);
            }

            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();

            ChatGPTLegacyResponse result = JsonSerializer.Deserialize<ChatGPTLegacyResponse>(responseString);

            if (result.choices != null && result.choices.Length > 0)
            {
                ChatGPTLegacyResponseMessage message = result.choices[0].message;

                if (message != null && message.content != null)
                {
                    return message.content;
                }
            }

            return "";
        }
    }
}

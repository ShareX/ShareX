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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShareX
{
    public class ChatGPTRequest
    {
        public string model { get; set; }
        public ChatGPTInput[] input { get; set; }
        public bool store { get; set; } = false;
    }

    public class ChatGPTInput
    {
        public string role { get; set; } = "user";
        public ChatGPTInputContent[] content { get; set; }
    }

    public class ChatGPTInputContent
    {
        public string type { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string text { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string image_url { get; set; }
    }

    public class ChatGPTResponse
    {
        public string id { get; set; }
        public ChatGPTResponseOutput[] output { get; set; }
    }

    public class ChatGPTResponseOutput
    {
        public string id { get; set; }
        public string type { get; set; }
        public ChatGPTResponseOutputContent[] content { get; set; }
    }

    public class ChatGPTResponseOutputContent
    {
        public string type { get; set; }
        public string text { get; set; }
    }

    public class ChatGPT
    {
        public string APIKey { get; set; }
        public string Model { get; set; }

        public ChatGPT(string apiKey, string model)
        {
            APIKey = apiKey;
            Model = model;
        }

        public async Task<string> AnalyzeImage(string filePath, string input = null)
        {
            HttpClient httpClient = HttpClientFactory.Create();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKey);

            byte[] imageBytes = await File.ReadAllBytesAsync(filePath);
            string base64Image = Convert.ToBase64String(imageBytes);
            string mimeType = MimeTypes.GetMimeTypeFromFileName(filePath);
            string imageDataUri = $"data:{mimeType};base64,{base64Image}";

            if (string.IsNullOrEmpty(input))
            {
                input = "what is in this image?";
            }

            ChatGPTRequest request = new ChatGPTRequest()
            {
                model = Model,
                store = false,
                input = new ChatGPTInput[]
                {
                    new ChatGPTInput()
                    {
                        role = "user",
                        content = new ChatGPTInputContent[]
                        {
                            new ChatGPTInputContent()
                            {
                                type = "input_text",
                                text = input
                            },
                            new ChatGPTInputContent()
                            {
                                type = "input_image",
                                image_url = imageDataUri
                            }
                        }
                    }
                }
            };

            string json = JsonSerializer.Serialize(request);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync("https://api.openai.com/v1/responses", content);
            string responseString = await response.Content.ReadAsStringAsync();

            ChatGPTResponse result = JsonSerializer.Deserialize<ChatGPTResponse>(responseString);

            if (result.output != null && result.output.Length > 0)
            {
                return result.output[0].content?[0].text ?? result.output[1].content?[0].text ?? "";
            }

            return "";
        }
    }
}
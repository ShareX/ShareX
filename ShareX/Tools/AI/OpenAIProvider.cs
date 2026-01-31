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
using System.Drawing;
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
        public ChatGPTReasoning reasoning { get; set; }
        public ChatGPTInput[] input { get; set; }
        public ChatGPTText text { get; set; }
        public bool store { get; set; }
    }

    public class ChatGPTReasoning
    {
        public string effort { get; set; }
    }

    public class ChatGPTInput
    {
        public string role { get; set; }
        public ChatGPTInputContent[] content { get; set; }
    }

    public class ChatGPTText
    {
        public string verbosity { get; set; }
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

    public class OpenAIProvider : IAIProvider
    {
        public string APIKey { get; set; }
        public string Model { get; set; }
        public string CustomURL { get; set; }

        public OpenAIProvider(string apiKey, string model, string customURL = null)
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
                input = "What is in this image?";
            }

            if (Model.Equals("gpt-5.2", StringComparison.OrdinalIgnoreCase) ||
                Model.Equals("gpt-5.1", StringComparison.OrdinalIgnoreCase))
            {
                if (reasoningEffort == null || reasoningEffort.Equals("minimal", StringComparison.OrdinalIgnoreCase))
                {
                    reasoningEffort = "none";
                }
            }
            else
            {
                if (reasoningEffort == null)
                {
                    reasoningEffort = "minimal";
                }
            }

            if (textVerbosity == null)
            {
                textVerbosity = "medium";
            }

            ChatGPTRequest request = new ChatGPTRequest()
            {
                model = Model,
                reasoning = new ChatGPTReasoning()
                {
                    effort = reasoningEffort
                },
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
                },
                text = new ChatGPTText()
                {
                    verbosity = textVerbosity
                },
                store = false
            };

            string json = JsonSerializer.Serialize(request);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync("https://api.openai.com/v1/responses", content);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();

            ChatGPTResponse result = JsonSerializer.Deserialize<ChatGPTResponse>(responseString);

            if (result.output != null && result.output.Length > 0)
            {
                for (int i = 0; i < result.output.Length; i++)
                {
                    if (result.output[i].content != null && result.output[i].content.Length > 0)
                    {
                        string text = result.output[i].content[0].text;

                        if (!string.IsNullOrEmpty(text))
                        {
                            return text;
                        }
                    }
                }
            }

            return "";
        }
    }
}
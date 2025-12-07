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

using Newtonsoft.Json;
using ShareX.HelpersLib;
using System;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ShareX
{
    public class OpenRouterProvider : IAIProvider
    {
        private readonly string apiKey;
        private readonly string model;

        public OpenRouterProvider(string apiKey, string model)
        {
            this.apiKey = apiKey;
            this.model = model;
        }

        public async Task<string> AnalyzeImage(Image image, string prompt, string reasoningEffort, string verbosity)
        {
            string base64Image = ImageHelpers.ImageToBase64(image, System.Drawing.Imaging.ImageFormat.Png);
            return await AnalyzeImageInternal(base64Image, prompt, reasoningEffort, verbosity);
        }

        public async Task<string> AnalyzeImage(string imagePath, string prompt, string reasoningEffort, string verbosity)
        {
            string base64Image = ImageHelpers.ImageFileToBase64(imagePath);
            return await AnalyzeImageInternal(base64Image, prompt, reasoningEffort, verbosity);
        }

        private async Task<string> AnalyzeImageInternal(string base64Image, string prompt, string reasoningEffort, string verbosity)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var payload = new
                {
                    model = this.model,
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = new object[]
                            {
                                new { type = "text", text = prompt },
                                new { type = "image_url", image_url = new { url = $"data:image/png;base64,{base64Image}" } }
                            }
                        }
                    },
                    max_tokens = 1024
                };

                string jsonPayload = JsonConvert.SerializeObject(payload);
                StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                const string url = "https://openrouter.ai/api/v1/chat/completions";

                HttpResponseMessage response = await client.PostAsync(url, content);
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic responseObject = JsonConvert.DeserializeObject(responseString);
                    return responseObject.choices[0].message.content;
                }
                else
                {
                    throw new Exception($"Error from OpenRouter API: {responseString}");
                }
            }
        }
    }
}
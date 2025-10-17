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

namespace ShareX
{
    public enum AIProvider
    {
        OpenAI,
        Gemini,
        OpenRouter,
        Custom
    }

    public class AIOptions
    {
        public AIProvider Provider { get; set; } = AIProvider.OpenAI;

        [JsonEncrypt]
        public string OpenAIAPIKey { get; set; }
        public string OpenAIModel { get; set; } = "gpt-4o-mini";
        public string OpenAICustomURL { get; set; }

        [JsonEncrypt]
        public string GeminiAPIKey { get; set; }
        public string GeminiModel { get; set; } = "gemini-1.5-flash-latest";

        [JsonEncrypt]
        public string OpenRouterAPIKey { get; set; }
        public string OpenRouterModel { get; set; } = "google/gemini-flash-1.5";

        public string ReasoningEffort { get; set; } = "minimal";
        public string Verbosity { get; set; } = "medium";
        public string Input { get; set; } = "What is in this image?";
        public bool AutoStartRegion { get; set; } = true;
        public bool AutoStartAnalyze { get; set; } = true;
        public bool AutoCopyResult { get; set; } = false;
    }
}

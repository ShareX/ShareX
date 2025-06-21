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

using ShareX.UploadersLib.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib.TextUploaders
{
    internal class PastieTextUploaderService : TextUploaderService
    {
        public override TextDestination EnumValue { get; } = TextDestination.Pastie;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override Image ServiceImage => Resources.Pastie;

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Pastie()
            {
                IsPublic = config.PastieIsPublic
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpPastie;
    }

    public sealed class Pastie : TextUploader
    {
        public bool IsPublic { get; set; }

        public override UploadResult UploadText(string text, string fileName)
        {
            UploadResult ur = new UploadResult();

            if (!string.IsNullOrEmpty(text))
            {
                Dictionary<string, string> arguments = new Dictionary<string, string>();
                arguments.Add("paste[body]", text);
                arguments.Add("paste[restricted]", IsPublic ? "0" : "1");
                arguments.Add("paste[authorization]", "burger");

                SendRequestURLEncoded(HttpMethod.POST, "http://pastie.org/pastes", arguments);
                ur.URL = LastResponseInfo.ResponseURL;
            }

            return ur;
        }
    }
}
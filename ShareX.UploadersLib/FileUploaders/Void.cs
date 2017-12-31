#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{

    public class VoidFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Void;

        public override Icon ServiceIcon => null;

        public override bool CheckConfig(UploadersConfig config)
        {
            return true;
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Void();
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => null;
    }

    public class Void : FileUploader
    {
        public override UploadResult Upload(Stream stream, string fileName)
        {
            var res = new UploadResult();

            try
            {
                var req = (HttpWebRequest)WebRequest.Create(string.Format("https://void.cat/src/php/upload.php?filename={0}", fileName));
                req.UserAgent = ShareXResources.UserAgent;
                req.KeepAlive = false;
                req.ContentType = Helpers.GetMimeType(fileName);
                req.ContentLength = stream.Length;
                req.Method = "POST";

                using (var rs = req.GetRequestStream())
                {
                    if (!TransferData(stream, rs)) return null;
                }

                var rsp = (HttpWebResponse)req.GetResponse();

                if (rsp.StatusCode == HttpStatusCode.OK)
                {
                    using (var sr = new StreamReader(rsp.GetResponseStream()))
                    {
                        var rsO = JsonConvert.DeserializeObject<VoidResponse>(sr.ReadToEnd());
                        if (rsO.status == 200)
                        {
                            res.URL = rsO.link;
                            res.IsSuccess = true;
                        }
                        else
                        {
                            res.Response = rsO.msg;
                        }
                    }
                }
                else
                {
                    res.Response = string.Format("Http status code {0}", rsp.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                res.Response = ex.ToString();
            }

            return res;
        }

        public class VoidResponse
        {
            public int status { get; set; }
            public string msg { get; set; }
            public string hash { get; set; }
            public string publichash { get; set; }
            public string link { get; set; }
            public string mime { get; set; }
            public string filename { get; set; }
        }
    }
}

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using ShareX.UploadersLib.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{

    public class VoidFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Void;

        public override Icon ServiceIcon => Resources.Pomf;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.PomfUploader != null && !string.IsNullOrEmpty(config.PomfUploader.UploadURL);
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

                if(rsp.StatusCode == HttpStatusCode.OK)
                {
                    using (var sr = new StreamReader(rsp.GetResponseStream()))
                    {
                        var rsO = JsonConvert.DeserializeObject<VoidResponse>(sr.ReadToEnd());
                        if(rsO.status == 200)
                        {
                            res.URL = rsO.link;
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

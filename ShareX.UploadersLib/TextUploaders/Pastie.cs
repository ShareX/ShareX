using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.UploadersLib.TextUploaders
{
    class PastieTextUploaderService : TextUploaderService
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

                arguments.Add("&paste[body]", text);
                arguments.Add("&paste[restricted]", IsPublic ? "0" : "1");
                arguments.Add("&paste[authorization]", "burger");

                ur.Response = SendRequestURLEncoded("http://pastie.org/pastes", arguments, responseType: ResponseType.RedirectionURL);

                if (!string.IsNullOrEmpty(ur.Response))
                {
                    ur.URL = ur.Response;
                }
            }
            return ur;
        }
    }

}
using System.Windows.Forms;
using ShareX.HelpersLib;
using ShareX.UploadersLib.FileUploaders;

namespace ShareX.UploadersLib.SharingServices
{
    public class EmailSharingService : SharingService
    {
        public override URLSharingServices EnumValue { get; } = URLSharingServices.Email;
        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.EmailSmtpServer)
                   && config.EmailSmtpPort > 0
                   && !string.IsNullOrEmpty(config.EmailFrom)
                   && !string.IsNullOrEmpty(config.EmailPassword);
        }

        public override void ShareURL(string url, UploadersConfig uploadersConfig)
        {
            if (!CheckConfig(uploadersConfig))
            {
                URLHelpers.OpenURL("mailto:?body=" + URLHelpers.URLEncode(url));
                return;
            }

            using (EmailForm emailForm = new EmailForm(uploadersConfig.EmailRememberLastTo ? uploadersConfig.EmailLastTo : string.Empty,
                uploadersConfig.EmailDefaultSubject, url))
            {
                if (emailForm.ShowDialog() == DialogResult.OK)
                {
                    if (uploadersConfig.EmailRememberLastTo)
                    {
                        uploadersConfig.EmailLastTo = emailForm.ToEmail;
                    }

                    Email email = new Email
                    {
                        SmtpServer = uploadersConfig.EmailSmtpServer,
                        SmtpPort = uploadersConfig.EmailSmtpPort,
                        FromEmail = uploadersConfig.EmailFrom,
                        Password = uploadersConfig.EmailPassword
                    };

                    email.Send(emailForm.ToEmail, emailForm.Subject, emailForm.Body);
                }
            }
        }
    }
}
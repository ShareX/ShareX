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
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class EmailFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Email;

        public override Image ServiceImage => Resources.mail;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.EmailSmtpServer) && config.EmailSmtpPort > 0 && !string.IsNullOrEmpty(config.EmailFrom) && !string.IsNullOrEmpty(config.EmailPassword);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            if (config.EmailAutomaticSend && !string.IsNullOrEmpty(config.EmailAutomaticSendTo))
            {
                return new Email()
                {
                    SmtpServer = config.EmailSmtpServer,
                    SmtpPort = config.EmailSmtpPort,
                    FromEmail = config.EmailFrom,
                    Password = config.EmailPassword,
                    ToEmail = config.EmailAutomaticSendTo,
                    Subject = config.EmailDefaultSubject,
                    Body = config.EmailDefaultBody
                };
            }
            else
            {
                using (EmailForm emailForm = new EmailForm(config.EmailRememberLastTo ? config.EmailLastTo : "", config.EmailDefaultSubject, config.EmailDefaultBody))
                {
                    if (emailForm.ShowDialog() == DialogResult.OK)
                    {
                        if (config.EmailRememberLastTo)
                        {
                            config.EmailLastTo = emailForm.ToEmail;
                        }

                        return new Email()
                        {
                            SmtpServer = config.EmailSmtpServer,
                            SmtpPort = config.EmailSmtpPort,
                            FromEmail = config.EmailFrom,
                            Password = config.EmailPassword,
                            ToEmail = emailForm.ToEmail,
                            Subject = emailForm.Subject,
                            Body = emailForm.Body
                        };
                    }
                    else
                    {
                        taskInfo.StopRequested = true;
                    }
                }
            }

            return null;
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpEmail;
    }

    public class Email : FileUploader
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string FromEmail { get; set; }
        public string Password { get; set; }

        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public void Send()
        {
            Send(ToEmail, Subject, Body);
        }

        public void Send(string toEmail, string subject, string body)
        {
            Send(toEmail, subject, body, null, null);
        }

        public void Send(string toEmail, string subject, string body, Stream stream, string fileName)
        {
            using (SmtpClient smtp = new SmtpClient()
            {
                Host = SmtpServer,
                Port = SmtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(FromEmail, Password)
            })
            {
                using (MailMessage message = new MailMessage(FromEmail, toEmail))
                {
                    message.Subject = subject;
                    message.Body = body;

                    if (stream != null)
                    {
                        Attachment attachment = new Attachment(stream, fileName);
                        message.Attachments.Add(attachment);
                    }

                    smtp.Send(message);
                }
            }
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Send(ToEmail, Subject, Body, stream, fileName);
            return new UploadResult { IsURLExpected = false };
        }
    }
}
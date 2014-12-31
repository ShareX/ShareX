#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2015 ShareX Developers

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

using System.IO;
using System.Net;
using System.Net.Mail;

namespace ShareX.UploadersLib.FileUploaders
{
    public class Email : FileUploader
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string FromEmail { get; set; }
        public string Password { get; set; }

        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public void Send(string toEmail, string subject, string body)
        {
            Send(toEmail, subject, body, null, null);
        }

        public void Send(string toEmail, string subject, string body, Stream stream, string fileName)
        {
            SmtpClient smtp = new SmtpClient
            {
                Host = SmtpServer,
                Port = SmtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(FromEmail, Password)
            };

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

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Send(ToEmail, Subject, Body, stream, fileName);
            return new UploadResult { IsURLExpected = false };
        }
    }
}
using Payslips.Data;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Path = System.IO.Path;

namespace Payslips
{
    internal class MessageSender
    {
        public string Smtp { get; set; }//Поле в форме
        public int SmtpPort { get; set; }//Поле в форме
        public string Message { get; set; }//Поле в форме
        public void Send(string login, string pass, List<Person> personal)
        {
            SmtpClient smtpClient = new SmtpClient(Smtp, SmtpPort);
            smtpClient.Credentials = new NetworkCredential(login, pass);
            smtpClient.EnableSsl = true;

            foreach (var person in personal)
            {
                using (MailMessage Mail = new MailMessage())
                {
                    Mail.Attachments.Add(new Attachment(Path.Combine(PdfEditor.PathSave,person.FullName+".pdf")));
                    Mail.Subject = Message;
                    Mail.To.Add(new MailAddress(person.Email));
                    Mail.From = new MailAddress(login);
                    smtpClient.Send(Mail);
                    Thread.Sleep(300);
                }
            }
        }
    }
}

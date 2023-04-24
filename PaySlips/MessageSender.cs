using Payslips.Data;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Windows.Forms;
using Path = System.IO.Path;

namespace Payslips
{
    internal class MessageSender
    {
        
        public bool IsNotCorrect { get;private set; } = false;
        private string _smtp;
        private int _smtpPort;
        private string _message;
        private string _login;
        private string _password;
        public MessageSender(string smtp, string smtpPort, string message, string login, string password)
        {
            Smtp = smtp;
            SmtpPort = smtpPort;
            Message = message;
            Login = login;
            Password = password;
        }
        public string Smtp
        {
            get { return _smtp; }
            private set
            {
                if (!string.IsNullOrEmpty(value)) { _smtp = value; }
                else { IsNotCorrect = true; }
            }
        }
        public string SmtpPort
        {
            get { return _smtpPort.ToString(); }
            private set
            {
                if (int.TryParse(value,out int num)) { _smtpPort = num; }
                else { IsNotCorrect = true; }
            }
        }
        public string Message
        {
            get { return _message; }
            private set
            {
                if (!string.IsNullOrEmpty(value)) { _message = value; }
                else { IsNotCorrect = true; }
            }
        }
        public string Login
        {
            get { return _login; }
            private set
            {
                if (!string.IsNullOrEmpty(value)) { _login = value; }
                else { IsNotCorrect = true; }
            }
        }
        public string Password
        {
            get { return _password; }
            private set
            {
                if (!string.IsNullOrEmpty(value)) { _password = value; }
                else { IsNotCorrect = true; }
            }
        }
        public void Send(IEnumerable<Person> personal,PdfEditor pdfEditor,bool? deleteDirectory)
        {
            try
            {
                pdfEditor.Divide();
                SmtpClient smtpClient = new SmtpClient(Smtp, _smtpPort);
                smtpClient.Credentials = new NetworkCredential(Login, Password);
                smtpClient.EnableSsl = true;
                foreach (Person person in personal)
                {
                    using (MailMessage Mail = new MailMessage())
                    {
                        Mail.Attachments.Add(new Attachment(Path.Combine(PdfEditor.PathSave, person.FullName + ".pdf")));
                        Mail.Subject = Message;
                        Mail.To.Add(new MailAddress(person.Email));
                        Mail.From = new MailAddress(Login);
                        smtpClient.Send(Mail);
                        Thread.Sleep(300);
                    }
                }
                MessageBox.Show("Отправка завершена!");
            }
            catch (System.Exception)
            {
                MessageBox.Show("Ошибка при отправке");
            }
            finally
            {
                pdfEditor.DeleteDirectory(deleteDirectory);
            } 
        }
    }
}

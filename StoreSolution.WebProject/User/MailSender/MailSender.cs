using System.Net.Mail;
using StoreSolution.WebProject.User.MailSender.Contracts;

namespace StoreSolution.WebProject.User.MailSender
{
    public class MailSender : IMailSender
    {
        private string _from;
        private string _to;
        private string _subject;
        private string _body;
        private bool _isBodyHtml;

        public void CreateMail(string @from, string to, string subject, string body, bool isBodyHtml)
        {
            _from = @from;
            _to = to;
            _subject = subject;
            _body = body;
            _isBodyHtml = isBodyHtml;
        }

        public void Send()
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_from);
                mailMessage.To.Add(new MailAddress(_to));
                mailMessage.CC.Add(new MailAddress(_to));
                mailMessage.Subject = _subject;
                mailMessage.Body = _body;
                mailMessage.IsBodyHtml = _isBodyHtml;

                using (var client = new SmtpClient())
                {
                    client.Send(mailMessage);
                }
            }
        }
    }
}
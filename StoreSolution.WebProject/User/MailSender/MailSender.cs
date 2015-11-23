using System.Net.Mail;
using StoreSolution.WebProject.User.MailSender.Contracts;

namespace StoreSolution.WebProject.User.MailSender
{
    public class MailSender : IMailSender
    {
        public void Send(string from, string to, string subject, string body, bool isBodyHtml)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(from);
                mailMessage.To.Add(new MailAddress(to));
                mailMessage.CC.Add(new MailAddress(to));
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = isBodyHtml;

                using (var client = new SmtpClient())
                {
                    client.Send(mailMessage);
                }
            }
        }
    }
}
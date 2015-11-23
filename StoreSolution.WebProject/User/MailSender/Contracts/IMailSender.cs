namespace StoreSolution.WebProject.User.MailSender.Contracts
{
    public interface IMailSender
    {
        void Send(string from, string to, string subject, string body, bool isBodyHtml);
    }
}
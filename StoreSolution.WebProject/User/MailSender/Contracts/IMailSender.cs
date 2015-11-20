namespace StoreSolution.WebProject.User.MailSender.Contracts
{
    public interface IMailSender
    {
        void CreateMail(string @from, string to, string subject, string body, bool isBodyHtml);

        void Send();
    }
}
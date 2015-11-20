using StoreSolution.WebProject.User.MailSender;
using StoreSolution.WebProject.User.MailSender.Contracts;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StoreSolution.WebProject.StructureMap
{
    public class MailRegistry : Registry
    {
        public MailRegistry()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.WithDefaultConventions();
            });

            For<IMailSender>().Use<MailSender>().AlwaysUnique();
        }
    }
}
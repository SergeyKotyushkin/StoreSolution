using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.DatabaseProject.Realizations;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StoreSolution.WebProject.StructureMap
{
    public class RepositoryRegistry : Registry
    {
        public RepositoryRegistry()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.WithDefaultConventions();
            });

            For<IPersonRepository>().Use<EfPersonRepository>().Singleton();
            For<IProductRepository>().Use<EfProductRepository>().Singleton();
            For<IOrderHistoryRepository>().Use<EfOrderHistoryRepository>().Singleton();
        } 
    }
}
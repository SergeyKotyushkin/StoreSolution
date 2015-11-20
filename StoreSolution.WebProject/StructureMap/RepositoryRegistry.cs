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
            this.Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.WithDefaultConventions();
            });

            this.For<IPersonRepository>().Use<EfPersonRepository>().Singleton();
            this.For<IProductRepository>().Use<EfProductRepository>().Singleton();
            this.For<IOrderHistoryRepository>().Use<EfOrderHistoryRepository>().Singleton();
        } 
    }
}
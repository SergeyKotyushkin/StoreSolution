using StructureMap;

namespace StoreSolution.WebProject.StructureMap
{
    public class StructureMapFactory
    {
        private static IContainer _container;

        private StructureMapFactory()
        {
            
        }

        public static void Init()
        {
            _container = new Container();
            _container.Configure(c =>
            {
                c.IncludeRegistry<RepositoryRegistry>();
                c.IncludeRegistry<CurrencyRegistry>();
                c.IncludeRegistry<MailRegistry>();
            });
        }

        public static IContainer GetContainer()
        {
            if (_container != null) return _container;

            _container = new Container();
            _container.Configure(c =>
            {
                c.IncludeRegistry<RepositoryRegistry>();
                c.IncludeRegistry<CurrencyRegistry>();
            });

            return _container;
        }

        public static T Resolve<T>()
        {
            return _container.GetInstance<T>();
        }
    }
}
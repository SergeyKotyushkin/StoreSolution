using System;
using System.Linq;
using Nest;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.Models;

namespace StoreSolution.BusinessLogic.Database.Realizations
{
    public class EsProductsRepository : IDbProductRepository
    {
        private const string EsUri = "http://localhost:9200";
        private const string EsIndex = "database";
        private const string EsType = "products";


        public IQueryable<Product> GetAll()
        {
            var client = GetElasticClient();

            var size = GetDatabaseSize(client);

            var hits =
                client.Search<Product>(s => s
                    .Index(EsIndex)
                    .Type(EsType)
                    .Size(size)
                    .MatchAll());

            return hits.Hits.Select(hit => hit.Source).OrderBy(hit => hit.Id).AsQueryable();
        }
        
        public bool AddOrUpdate(Product product)
        {
            throw new NotImplementedException();
        }

        public bool RemoveById(int id)
        {
            throw new NotImplementedException();
        }

        public Product GetById(int id)
        {
            var client = GetElasticClient();

            return client.Get<Product>(c => c.Index(EsIndex).Type(EsType).Id(id)).Source;
        }

        public IQueryable<Product> SearchByName(IQueryable<Product> products, string searchName)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Product> SearchByCategory(IQueryable<Product> products, string searchCategory)
        {
            throw new NotImplementedException();
        }


        private static ElasticClient GetElasticClient()
        {
            var uri = new Uri(EsUri);
            var settings = new ConnectionSettings(uri);
            return new ElasticClient(settings);
        }

        private static int GetDatabaseSize(IElasticClient client)
        {
            return Convert.ToInt32(client.Count(c => c.Index(EsIndex).Type(EsType)).Count);
        }
    }
}
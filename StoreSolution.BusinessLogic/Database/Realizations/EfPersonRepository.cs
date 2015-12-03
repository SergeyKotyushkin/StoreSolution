using System.Data.Entity.Migrations;
using System.Linq;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.EfContexts;
using StoreSolution.BusinessLogic.Database.Models;

namespace StoreSolution.BusinessLogic.Database.Realizations
{
    public class EfPersonRepository : IDbPersonRepository
    {
        private readonly EfPersonContext _context = new EfPersonContext();
        
        public IQueryable<Person> GetAll()
        {
            return _context.PersonTable;
        }
        
        public bool AddOrUpdate(Person person)
        {
            _context.PersonTable.AddOrUpdate(person);
            return _context.SaveChanges() > 0;
        }
    }
}

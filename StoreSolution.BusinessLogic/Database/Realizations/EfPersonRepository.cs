using System.Data.Entity.Migrations;
using System.Linq;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.EfContexts;
using StoreSolution.BusinessLogic.Database.Models;

namespace StoreSolution.BusinessLogic.Database.Realizations
{
    public class EfPersonRepository : IEfPersonRepository
    {
        private readonly EfPersonContext _context = new EfPersonContext();
        
        public IQueryable<Person> Persons
        {
            get { return _context.PersonTable; }
        }
        
        public bool AddOrUpdate(Person person)
        {
            _context.PersonTable.AddOrUpdate(person);
            return _context.SaveChanges() > 0;
        }
    }
}

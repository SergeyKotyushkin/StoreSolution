 using System.Data.Entity.Migrations;
using System.Linq;
 using StoreSolution.DatabaseProject.Contracts;
 using StoreSolution.DatabaseProject.EfContext;
using StoreSolution.DatabaseProject.Model;

namespace StoreSolution.DatabaseProject.Realizations
{
    public class EfPersonRepository : IPersonRepository
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

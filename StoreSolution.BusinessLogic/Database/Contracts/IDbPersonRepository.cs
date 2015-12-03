using System.Linq;
using StoreSolution.BusinessLogic.Database.Models;

namespace StoreSolution.BusinessLogic.Database.Contracts
{
    public interface IDbPersonRepository
    {
        IQueryable<Person> GetAll();

        bool AddOrUpdate(Person person);
    }
}
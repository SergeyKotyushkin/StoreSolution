using System.Linq;
using StoreSolution.BusinessLogic.Database.Model;

namespace StoreSolution.BusinessLogic.Database.Contracts
{
    public interface IEfPersonRepository
    {
        IQueryable<Person> Persons { get; }

        bool AddOrUpdate(Person person);
    }
}
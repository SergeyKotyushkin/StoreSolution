using System.Linq;
using StoreSolution.DatabaseProject.Model;

namespace StoreSolution.DatabaseProject.Contracts
{
    public interface IPersonRepository
    {
        IQueryable<Person> Persons { get; }

        bool AddOrUpdate(Person person);
    }
}
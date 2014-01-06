using RepositoryExtensions.Core.Models;

namespace RepositoryExtensions.Core.Factories
{
    public class ManagerFactory :IManagerFactory
    {
        public virtual IManager Create(string name)
        {
            return new Employee(name);
        }

        public virtual IManager Create(string name, IEmployee[] employees)
        {
            return new Employee(name, employees);
        }
    }
}

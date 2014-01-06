using RepositoryExtensions.Core.Models;

namespace RepositoryExtensions.Core.Factories
{
    public class EmployeeFactory :IEmployeeFactory
    {
        public virtual IEmployee Create(string name)
        {
            return new Employee(name);
        }
    }
}

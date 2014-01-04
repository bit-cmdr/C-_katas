using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryExtensions.Core.Model;

namespace RepositoryExtensions.Core.Factories
{
    public class EmployeeFactory :IEmployeeFactory
    {
        public virtual IEmployee Create(string name)
        {
            return new Employee(name);
        }

        public virtual IEmployee Create(string name, IManager manager)
        {
            return new Employee(name, manager);
        }
    }
}

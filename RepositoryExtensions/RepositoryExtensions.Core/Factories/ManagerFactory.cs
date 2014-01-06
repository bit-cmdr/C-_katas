using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryExtensions.Core.Model;

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

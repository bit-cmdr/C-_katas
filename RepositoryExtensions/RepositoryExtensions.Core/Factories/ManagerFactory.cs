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

        public virtual IManager Create(string name, IEmployee manager)
        {
            return new Employee(name, manager);
        }

        public virtual IManager Create(string name, IEmployee manager, IEmployee[] employees)
        {
            return new Employee(name, manager, employees);
        }
    }
}

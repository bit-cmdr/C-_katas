using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryExtensions.Core.Model
{
    public class Employee :IEmployee, IManager
    {
        public Employee()
            : this(Guid.Empty, string.Empty, default(Employee), new IEmployee[0])
        {
        }

        public Employee(string name)
            : this(Guid.Empty, name, default(Employee), new IEmployee[0])
        {
        }

        public Employee(string name, IEmployee manager)
            :this(Guid.Empty, name, manager, new IEmployee[0])
        {
        }

        public Employee(string name, IEmployee[] employees)
            : this(Guid.Empty, name, default(Employee), employees)
        {
        }

        public Employee(string name, IEmployee manager, IEmployee[] employees)
            :this(Guid.Empty, name, manager, employees)
        {
        }

        public Employee(Guid id, string name, IEmployee manager)
            :this(id, name, manager, new IEmployee[0])
        {
        }

        public Employee(Guid id, string name, IEmployee manager, IEmployee[] employees)
        {
            Id = id;
            Name = name ?? string.Empty;
            Manager = manager ?? default(Employee);
            _employees = employees ?? new IEmployee[0];
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public IEmployee Manager { get; private set; }
        public IEnumerable<IEmployee> Employees { get { return _employees; } }

        private IEmployee[] _employees;

        public void GainEmployee(IEmployee employee)
        {
            var temp = new IEmployee[_employees.Length + 1];
            _employees.CopyTo(temp, 0);
            temp[temp.Length] = employee;
            _employees = temp;
            temp = null;
        }

        public void RedistributeEmployee(IEmployee employee)
        {
            var temp = new IEmployee[_employees.Length - 1];
            int idx = 0;
            foreach (var emp in _employees.Where(x => x.Id != employee.Id))
            {
                temp[idx] = emp;
                idx++;
            }
            _employees = temp;
            temp = null;
        }
    }
}

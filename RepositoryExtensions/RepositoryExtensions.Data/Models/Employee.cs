using System;
using System.Collections.Generic;

namespace RepositoryExtensions.Data.Models
{
    public class Employee
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Guid? ManagerId { get; set; }
        public virtual Employee Manager { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}

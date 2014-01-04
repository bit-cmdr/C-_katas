using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryExtensions.Data.Models;
using RepositoryExtensions.Data.Models.Mapping;

namespace RepositoryExtensions.Data.Contexts
{
    public class HrContext :DbContext
    {
        public HrContext()
        {
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new EmployeeMap());
        }
    }
}

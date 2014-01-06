using System.Data.Entity;
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

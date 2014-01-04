namespace RepositoryExtensions.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using RepositoryExtensions.Data.Contexts;
    using RepositoryExtensions.Data.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<HrContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(HrContext context)
        {
            var kirk = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Kirk"
            };

            var alex = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Alex",
                ManagerId = kirk.Id
            };

            var mike = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Mike",
                ManagerId = kirk.Id
            };

            var geoff = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Geoff",
                ManagerId = kirk.Id
            };

            var eric = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Eric",
                ManagerId = kirk.Id
            };

            var joe = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Joe",
                ManagerId = kirk.Id
            };

            var ryan = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Ryan",
                ManagerId = kirk.Id
            };

            var marcus = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Marcus",
                ManagerId = kirk.Id
            };

            context.Employees.AddOrUpdate(x => x.Name, new Employee[] {
                kirk,
                alex,
                mike,
                geoff,
                eric,
                joe,
                ryan,
                marcus
            });
        }
    }
}

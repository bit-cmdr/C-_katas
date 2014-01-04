using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryExtensions.Data.Contexts;
using RepositoryExtensions.Data.Models;

namespace RepositoryExtensions.Data.Seed
{
    public class TestSeed :DropCreateDatabaseAlways<HrContext>
    {
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
                Name = "Alex"
            };

            var mike = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Mike"
            };

            var geoff = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Geoff"
            };

            var eric = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Eric"
            };

            var joe = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Joe"
            };

            var ryan = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Ryan"
            };

            var marcus = new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Marcus"
            };

            context.Employees.Add(kirk);
            context.Employees.Add(alex);
            context.Employees.Add(mike);
            context.Employees.Add(geoff);
            context.Employees.Add(eric);
            context.Employees.Add(joe);
            context.Employees.Add(ryan);
            context.Employees.Add(marcus);
        }
    }
}

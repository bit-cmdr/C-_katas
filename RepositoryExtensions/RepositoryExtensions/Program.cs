using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryExtensions.Core;
using RepositoryExtensions.Core.Factories;
using RepositoryExtensions.Core.Model;
using RepositoryExtensions.Data;
using RepositoryExtensions.Data.Seed;

namespace RepositoryExtensions
{
    class Program
    {
        private static DataMapper _mapping;

        static void Main(string[] args)
        {
            Initialize();
            var repo = new Repository<Data.Models.Employee, Employee>(new Data.Contexts.HrContext(), _mapping);
            var all = repo.GetAll();
            var geoff = repo.Get(Guid.Parse("7d2e365a-6749-4ed5-a9f5-0f9db713fd9c"));

            Console.ReadLine();
        }

        private static void Initialize()
        {
            _mapping = new DataMapper();
            _mapping.Configure();
        }
    }
}

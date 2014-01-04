using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryExtensions.Core
{
    public interface IEmployee
    {
        Guid Id { get; }
        string Name { get; }
        IEmployee Manager { get; }
    }

    public interface IManager :IEmployee
    {
        IEnumerable<IEmployee> Employees { get; }

        void GainEmployee(IEmployee employee);
        void RedistributeEmployee(IEmployee employee);
    }

    public interface IEmployeeFactory
    {
        IEmployee Create(string name);
        IEmployee Create(string name, IManager manager);
    }

    public interface IManagerFactory
    {
        IManager Create(string name);
        IManager Create(string name, IManager manager);
        IManager Create(string name, IManager manager, IEmployee[] employees);
    }

    public interface IRepository<T>
        where T :class
    {
        IEnumerable<T> GetAll();
        T Get(Guid id);
        T Add(T item);
        void Update(T item);
        void Remove(Guid id);
        IEnumerable<T> Query(Expression<Func<T, bool>> query);
    }
}

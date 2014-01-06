using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RepositoryExtensions.Core
{
    public interface IEmployee
    {
        Guid Id { get; }
        string Name { get; }
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
    }

    public interface IManagerFactory
    {
        IManager Create(string name);
        IManager Create(string name, IEmployee[] employees);
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

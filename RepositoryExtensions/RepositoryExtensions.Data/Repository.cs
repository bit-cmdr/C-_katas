using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using RepositoryExtensions.Core;
using RepositoryExtensions.Data.Contexts;

namespace RepositoryExtensions.Data
{
    public class Repository<TSource, TDestination> :IRepository<TDestination>
        where TSource :class
        where TDestination :class
    {
        private readonly HrContext _context;
        private readonly DataMapper _mapping;

        public Repository(HrContext context, DataMapper mapping)
        {
            _context = context;
            _mapping = mapping;
        }

        public virtual IEnumerable<TDestination> GetAll()
        {
            var temp = _context.Set<TSource>().ToArray();
            return Mapper.Map<TDestination[]>(temp);
        }

        public virtual TDestination Get(Guid id)
        {
            return Mapper.Map<TDestination>(_context.Set<TSource>().Find(id));
        }

        public virtual TDestination Add(TDestination item)
        {
            var update = Mapper.Map<TSource>(item);
            _context.Set<TSource>().Add(update);
            _context.SaveChanges();

            return Mapper.Map<TDestination>(update);
        }

        public virtual void Update(TDestination item)
        {
            var update = Mapper.Map<TSource>(item);
            _context.Set<TSource>().Add(update);
            _context.SaveChanges();
        }

        public virtual void Remove(Guid id)
        {
            _context.Set<TSource>().Remove(_context.Set<TSource>().Find(id));
        }

        public virtual IEnumerable<TDestination> Query(Expression<Func<TDestination, bool>> query)
        {
            var filter = Mapper.Map<Expression<Func<TSource, bool>>>(query);
            return Mapper.Map<IEnumerable<TDestination>>(
                _context.Set<TSource>()
                .Where(filter));
        }
    }
}

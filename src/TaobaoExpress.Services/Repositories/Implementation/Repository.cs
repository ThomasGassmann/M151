namespace TaobaoExpress.Services.Repositories.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext context;

        public Repository(DbContext context) =>
            this.context = context;

        public void Add(T entity) =>
            this.context.Set<T>().Add(entity);

        public void AddRange(IEnumerable<T> entities) =>
            this.context.Set<T>().AddRange(entities);

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate) =>
            this.context.Set<T>().Where(predicate);

        public T Get(object key) =>
            this.context.Set<T>().Find(key);

        public void Remove(T entity) =>
            this.context.Set<T>().Remove(entity);

        public void RemoveRange(IEnumerable<T> entities) =>
            this.context.Set<T>().RemoveRange(entities);

        public void Update(T entity)
        {
            var local = context.Set<T>()
                            .Local
                            .FirstOrDefault(this.FindExpression(entity));
            if (local != null)
            {
                context.Entry(local).State = EntityState.Detached;
            }

            context.Entry(entity).State = EntityState.Modified;
        }

        protected IQueryable<T> QueryAsNoTracking() =>
            context.Set<T>().AsNoTracking();

        protected abstract Func<T, bool> FindExpression(T entity);
    }
}

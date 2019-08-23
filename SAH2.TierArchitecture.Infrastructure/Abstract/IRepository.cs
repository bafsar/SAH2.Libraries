using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SAH2.TierArchitecture.Infrastructure.Abstract
{
    public interface IRepository<T> : IDisposable where T : class, IEntity
    {
        IQueryable<T> GetAll(Expression<Func<T, bool>> expression = null);

        T GetFirst(Expression<Func<T, bool>> expression);


        T Add(T entity);

        IList<T> Add(IList<T> entities);


        T Update(T entity);

        IList<T> Update(IList<T> entities);


        bool Delete(T entity);

        bool Delete(IList<T> entities);

        bool Delete(Expression<Func<T, bool>> expression);
    }
}
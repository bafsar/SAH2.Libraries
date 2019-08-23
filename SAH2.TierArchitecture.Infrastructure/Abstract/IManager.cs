using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SAH2.TierArchitecture.Infrastructure.Abstract
{
    public interface IManager<T> where T : class, IEntity
    {
        List<T> GetAll(Expression<Func<T, bool>> expression = null);

        T GetFirst(Expression<Func<T, bool>> expression);


        T Add(T entity);
        List<T> Add(IList<T> entities);


        T Update(T entity);

        List<T> Update(IList<T> entities);


        bool Delete(T entity);

        bool Delete(IList<T> entities);


        bool Delete(Expression<Func<T, bool>> expression);
    }
}
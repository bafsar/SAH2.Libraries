using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SAH2.TierArchitecture.Infrastructure.Abstract;

namespace SAH2.TierArchitecture.Infrastructure
{
    public abstract class EfRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        #region Constructors

        protected EfRepository(IDbContext dbContext)
        {
            _dbContext = dbContext as DbContext
                         ?? throw new ArgumentNullException(nameof(dbContext),
                             $"parameter dbContext's type must be {nameof(DbContext)}");

            _dbSet = _dbContext.Set<TEntity>();
        }

        #endregion

        #region Helpers

        private bool DoActionInSafe(Action action, out int affectedRowCount)
        {
            affectedRowCount = 0;

            try
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        action();
                        affectedRowCount = _dbContext.SaveChanges();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        private bool DoActionInSafe(Action action)
        {
            var _ = 0;
            return DoActionInSafe(action, out _);
        }

        #endregion

        #region Fields

        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        #endregion


        #region IRepository Members

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> expression = null)
        {
            return expression == null
                ? _dbSet
                : _dbSet.Where(expression);
        }


        public TEntity GetFirst(Expression<Func<TEntity, bool>> expression)
        {
            return GetAll(expression).First();
        }

        public TEntity Add(TEntity entity)
        {
            var result = DoActionInSafe(() => _dbSet.Add(entity));

            return result ? entity : null;
        }

        public IList<TEntity> Add(IList<TEntity> entities)
        {
            var result = DoActionInSafe(() => _dbSet.AddRange(entities));

            return result ? entities : null;
        }


        public TEntity Update(TEntity entity)
        {
            var result = DoActionInSafe(() => _dbSet.Update(entity));

            return result ? entity : null;
        }

        public IList<TEntity> Update(IList<TEntity> entities)
        {
            var result = DoActionInSafe(() => _dbSet.UpdateRange(entities));

            return result ? entities : null;
        }




        public bool Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "entity can't be null");

            DoActionInSafe(() => _dbSet.Remove(entity), out var affectedRowCount);

            if (affectedRowCount == 0)
                throw new DBConcurrencyException("There is no record affected!");

            return affectedRowCount > 0;
        }

        public bool Delete(IList<TEntity> entities)
        {
            if (entities?.Count == 1)
                return Delete(entities.First());


            if (entities == null || !entities.Any())
                throw new ArgumentNullException(nameof(entities), "entities parameter should have at least one entity");

            DoActionInSafe(() => _dbSet.RemoveRange(entities), out var affectedRowCount);

            if (affectedRowCount == 0)
                throw new DBConcurrencyException("There is no record affected!");

            return affectedRowCount > 0;

        }

        public bool Delete(Expression<Func<TEntity, bool>> expression)
        {
            var entities = GetAll(expression);

            if (entities == null || !entities.Any())
                throw new KeyNotFoundException("it couldn't found any record matches your conditions");

            return entities.Count() == 1
                ? Delete(entities.First())
                : Delete(entities.ToList());
        }

        #endregion


        #region IDisposable Members

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _dbContext.Dispose();

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

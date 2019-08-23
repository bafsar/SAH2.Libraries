using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SAH2.TierArchitecture.Infrastructure.Abstract;

namespace SAH2.TierArchitecture.Infrastructure
{
    public abstract class ManagerBase<TModelEntity> : IManager<TModelEntity> where TModelEntity : class, IEntity
    {
        protected readonly IRepository<TModelEntity> Repository;


        #region Constructor

        protected ManagerBase(IRepository<TModelEntity> repository)
        {
            Repository = repository;
        }

        #endregion


        #region Get

        public virtual List<TModelEntity> GetAll(Expression<Func<TModelEntity, bool>> expression = null)
        {
            return Repository.GetAll(expression).ToList();
        }

        public virtual TModelEntity GetFirst(Expression<Func<TModelEntity, bool>> expression)
        {
            return Repository.GetFirst(expression);
        }

        #endregion


        #region Add

        public virtual TModelEntity Add(TModelEntity entity)
        {
            return Repository.Add(entity);
        }

        public virtual List<TModelEntity> Add(IList<TModelEntity> entities)
        {
            return Repository.Add(entities).ToList();
        }

        #endregion


        #region Update

        public virtual TModelEntity Update(TModelEntity entity)
        {
            return Repository.Update(entity);
        }

        public virtual List<TModelEntity> Update(IList<TModelEntity> entities)
        {
            return Repository.Update(entities).ToList();
        }

        #endregion


        #region Delete

        public virtual bool Delete(TModelEntity entity)
        {
            return Repository.Delete(entity);
        }

        public virtual bool Delete(IList<TModelEntity> entities)
        {
            return Repository.Delete(entities);
        }

        public virtual bool Delete(Expression<Func<TModelEntity, bool>> expression)
        {
            return Repository.Delete(expression);
        }

        #endregion
    }
}
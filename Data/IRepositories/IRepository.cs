using System.Linq.Expressions;

namespace Dextor.API.Data.IRepositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        //int GetMaxId(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> FindAll();
        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);
        TEntity Find(Expression<Func<TEntity, bool>> predicate);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void UpdateSpecificColumn(TEntity entity, params Expression<Func<TEntity, object>>[] updatedProperties);
        void Delete(object id);
        void Delete(TEntity entityToDelete);
        void Delete(Expression<Func<TEntity, bool>> predicate);
        void AddRange(IEnumerable<TEntity> listEntities);
        void RemoveRange(IEnumerable<TEntity> listEntities);
        void Save();
    }
}

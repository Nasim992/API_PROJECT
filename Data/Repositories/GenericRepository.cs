using Dextor.API.Models.BindingModel;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dextor.API.Data.Repositories
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        protected readonly DatabaseContext DbContext;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(DatabaseContext context)
        {
            DbContext = context;
            _dbSet = DbContext.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> FindAll()
        {
            return _dbSet;
        }

        public virtual TEntity Get(int id)
        {
            return _dbSet.Find(id);
        }

        public virtual IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate).FirstOrDefault();
        }

        public virtual void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            _dbSet.Attach(entity);
            DbContext.Entry(entity).State = EntityState.Modified;
        }
        public virtual void UpdateSpecificColumn(TEntity entity, params Expression<Func<TEntity, object>>[] updatedProperties)
        {
            DbContext.Entry(entity).State = EntityState.Unchanged;
            if (updatedProperties.Any())
                foreach (var propertyName in updatedProperties)
                {
                    DbContext.Entry(entity).Property(propertyName).IsModified = true;
                }
        }
        public virtual void Delete(object id)
        {
            var entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }
        public virtual void Delete(TEntity entityToDelete)
        {
            if (DbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            _dbSet.RemoveRange(_dbSet.Where(predicate));
        }

        public virtual void AddRange(IEnumerable<TEntity> listEntities)
        {
            _dbSet.AddRange(listEntities);
        }
        public virtual void RemoveRange(IEnumerable<TEntity> listEntities)
        {
            _dbSet.RemoveRange(listEntities);
        }
        public virtual void Save()
        {
            DbContext.SaveChanges();
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DbContext.Dispose();
            }
        }
        public virtual void Dispose()
        {
            DbContext.Dispose();
        }
    }
}

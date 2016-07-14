using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.OData;

namespace GenericODataWebApi
{
    public class EntityFrameworkODataProvider<TEntity> where TEntity : class
    {
        private DbContext db;

        public EntityFrameworkODataProvider(DbContext dbContext)
        {
            db = dbContext;
        }

        public IQueryable<TEntity> Get()
        {
            return db.Set<TEntity>().AsNoTracking();
        }

        public IQueryable<TEntity> GetByKeyAsQueryable(int key)
        {
            return GetByKeyAsEnumerable(key).AsQueryable();//.ProjectTo<TModel>(MapperConfig);
        }

        private IEnumerable<TEntity> GetByKeyAsEnumerable(int key)
        {
            yield return GetByKey(key);
        }

        /// <summary>
        /// If your key is NOT the primary key, override this!
        /// </summary>
        public virtual TEntity GetByKey(int key)
        {
            return db.Set<TEntity>().Find(key);
        }

        public async Task<bool> Delete(int key)
        {
            var toDelete = GetByKey(key);
            if (toDelete == null)
                return false;

            db.Set<TEntity>().Remove(toDelete);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task Add(TEntity item)
        {
            db.Set<TEntity>().Add(item);
            await db.SaveChangesAsync();
        }

        public async Task<bool> Replace(int key, TEntity item)
        {
            db.Entry(item).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
                //if (!ProductExists(key))
                //{
                //    return NotFound();
                //}
            }
        }

        public async Task<TEntity> Update(int key, Delta<TEntity> deltaEntity)
        {
            var matching = GetByKey(key);

            if (matching == null)
                return null;

            deltaEntity.Patch(matching);

            try
            {
                await db.SaveChangesAsync();
                return matching;
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!ProductExists(key))
                //{
                //    return NotFound();
                //}
                //else
                //{
                    throw;
                //}
            }
        }

        public bool KeyMatchesEntity(int key, TEntity item)
        {
            var efAwareItem = db.Set<TEntity>().Attach(item);
            
            var keyItem = GetByKey(key);
            return efAwareItem == keyItem;
        }
    }
}
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

        public async Task<IQueryable<TEntity>> GetByKeyAsQueryable(int key)
        {
            var wrapper = new[] {await GetByKey(key)};
            return wrapper.AsQueryable();
        }

        /// <summary>
        /// If your key is NOT the primary key, override this!
        /// </summary>
        public virtual async Task<TEntity> GetByKey(int key)
        {
            return await db.Set<TEntity>().FindAsync(key);
        }

        public async Task<bool> Delete(int key)
        {
            var toDelete = await GetByKey(key);
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
            var matching = await GetByKey(key);

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
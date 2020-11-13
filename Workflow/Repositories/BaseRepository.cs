using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Workflow.Interfaces;
using Workflow.Utilities;

namespace Workflow.Repositories
{

    public class BaseRepository<TEntity, TContext> : IBaseRepository<TEntity, TContext> where TEntity : class where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public BaseRepository(TContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }
        #region Async
        public virtual async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(params object[] ids)
        {
            
            return await _dbSet.FindAsync(ids);
        }

        public virtual async Task<IEnumerable<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }

        public virtual async Task AddAsync(TEntity entity, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));
            await _dbSet.AddAsync(entity);
            if (saveNow)
                await _context.SaveChangesAsync();
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            await _dbSet.AddRangeAsync(entities);
            if (saveNow)
                await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TEntity entity, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));
            _dbSet.Update(entity);
            if (saveNow)
                await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            _dbSet.UpdateRange(entities);
            if (saveNow)
                await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(TEntity entity, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));
            _dbSet.Remove(entity);
            if (saveNow)
                await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            _dbSet.RemoveRange(entities);
            if (saveNow)
                await _context.SaveChangesAsync();
        }
        #endregion
    }
}


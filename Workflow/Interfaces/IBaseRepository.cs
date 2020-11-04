using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Workflow.Interfaces
{
    public interface IBaseRepository<TEntity, TContext> where TEntity : class where TContext : DbContext
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(params object[] ids);
        Task<IEnumerable<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes);
        Task AddAsync(TEntity entity, bool saveNow = true);
        Task AddRangeAsync(IEnumerable<TEntity> entities, bool saveNow = true);
        Task UpdateAsync(TEntity entity, bool saveNow = true);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities, bool saveNow = true);
        Task DeleteAsync(TEntity entity, bool saveNow = true);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool saveNow = true);
        Task SaveChangeAsync();
    }
}

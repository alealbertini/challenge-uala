using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TwitterUala.Application.Contracts.Infrastructure;

namespace TwitterUala.Infrastructure.Impl
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly DbSet<T> _entity;
        protected readonly DbContext _context;
        public BaseRepository(DbContext context)
        {
            _entity = context.Set<T>();
        }

        public virtual async Task Add(T entity) => await _entity.AddAsync(entity);

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, bool tracking = true)
            => tracking ? await _entity.FirstOrDefaultAsync(filter) : await _entity.AsNoTracking().FirstOrDefaultAsync(filter);

    }
}

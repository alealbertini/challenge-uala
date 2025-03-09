using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TwitterUala.Application.Contracts.Infrastructure;

namespace TwitterUala.Infrastructure.Impl
{
    public class UnitOfWork(DbContext context, IServiceProvider serviceProvider) : IUnitOfWork
    {
        private readonly DbContext _context = context;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public virtual IBaseRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return _serviceProvider.GetRequiredService<IBaseRepository<TEntity>>();
        }

        public virtual async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}

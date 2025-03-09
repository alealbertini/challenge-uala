using System.Linq.Expressions;

namespace TwitterUala.Application.Contracts.Infrastructure
{
    public interface IBaseRepository<T> where T : class
    {
        Task Add(T entity); Task<T> 
        FirstOrDefaultAsync(Expression<Func<T, bool>> filter, bool tracking = true);
    }
}

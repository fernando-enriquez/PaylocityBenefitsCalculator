using System.Linq.Expressions;

namespace Api.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[]? includes);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? predicate = null);

        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = null);

        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[]? includes);
        Task AddAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveChangesAsync();
    }
}

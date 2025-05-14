using Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Api.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly EmployeePaycheckDbContext _context;
        private readonly DbSet<T> _set;

        public Repository(EmployeePaycheckDbContext context)
        {
            _context = context;
            _set = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _set.AddAsync(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            _set.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[]? includes)
        {
            IQueryable<T> query = _set;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>>? predicate = null)
        {
            IQueryable<T> query = _set;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.ToListAsync();
        }


        public async Task<T> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null)
        {
            IQueryable<T> query = _set;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[]? includes)
        {
            IQueryable<T> query = _set;

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // EF Convention: entity has "Id" as key
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

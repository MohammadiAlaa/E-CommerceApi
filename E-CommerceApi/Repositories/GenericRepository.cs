using E_CommerceApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_CommerceApi.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly StoreDbContext _context;
        private readonly DbSet<T> _table;

        public GenericRepository(StoreDbContext context)
        {
            _context = context;
            _table = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
            => await _table.ToListAsync();


        public async Task<T> GetByIdAsync(int id)
            => await _table.FindAsync(id);


        public async Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _table;
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.SingleOrDefaultAsync(criteria);
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _table;
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.Where(criteria).ToListAsync();
        }

        public async Task AddAsync(T entity) => await _table.AddAsync(entity);

        public void Update(T entity) => _table.Update(entity);

        public void Delete(T entity) => _table.Remove(entity);
    }
}

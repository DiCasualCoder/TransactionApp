using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TransactionApp.DAL.Abstract.EntityFramework.Repositories;
using TransactionApp.DAL.Concrete.EntityFramework.Contexts;

namespace TransactionApp.DAL.Concrete.EntityFramework.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private TransactionManagerDbContext _context;
        protected DbSet<T> _dbSet;

        public GenericRepository(TransactionManagerDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();

        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveById(int id)
        {
            T? entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task<List<T>> GetWhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<List<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            query = query.Where(predicate);
            return await query.ToListAsync();
        }
    }
}

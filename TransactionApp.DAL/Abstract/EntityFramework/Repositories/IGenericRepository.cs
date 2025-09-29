
using System.Linq.Expressions;

namespace TransactionApp.DAL.Abstract.EntityFramework.Repositories
{
    public interface IGenericRepository<T>
    {
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);
        void Remove(T entity);
        void RemoveById(int id);


        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();

        Task<List<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    }
}

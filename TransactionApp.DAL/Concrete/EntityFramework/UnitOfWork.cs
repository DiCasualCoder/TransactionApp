using TransactionApp.DAL.Abstract.EntityFramework;
using TransactionApp.DAL.Concrete.EntityFramework.Contexts;

namespace TransactionApp.DAL.Concrete.EntityFramework
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TransactionManagerDbContext _context;

        public UnitOfWork(TransactionManagerDbContext context)
        {
            _context = context;
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int Commit()
        {
            return _context.SaveChanges();
        }
    }
}

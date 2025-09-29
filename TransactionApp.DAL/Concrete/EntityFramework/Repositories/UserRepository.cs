using TransactionApp.DAL.Abstract.EntityFramework.Repositories;
using TransactionApp.DAL.Concrete.EntityFramework.Contexts;
using TransactionApp.ENTITIES.Concrete.TransactionManager;

namespace TransactionApp.DAL.Concrete.EntityFramework.Repositories
{
    public class UserRepository : GenericRepository<USER>, IUserRepository
    {
        public UserRepository(TransactionManagerDbContext context) : base(context)
        {
            
        }
    }
}

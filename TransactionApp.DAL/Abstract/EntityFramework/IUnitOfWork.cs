namespace TransactionApp.DAL.Abstract.EntityFramework
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
        int Commit();
    }
}

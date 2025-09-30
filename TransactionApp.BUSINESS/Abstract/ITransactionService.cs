using TransactionApp.CORE.Utilities.Result.Abstract;
using TransactionApp.ENTITIES.Dto.TransactionDto;

namespace TransactionApp.BUSINESS.Abstract
{
    public interface ITransactionService
    {
        Task<IDataResult<List<TransactionFetchDto>>> GetAllTransactionsAsync();
        
        Task<IDataResult<int>> AddTransactionAsync(TransactionAddDto transaction);

        Task<IDataResult<Dictionary<int, decimal>>> TotalAmountPerUserAsync();

        Task<IDataResult<Dictionary<string, decimal>>> TotalAmountPerTransactionAsync();

        Task<IDataResult<List<TransactionHighVolumeDto>>> GetHighVolumeTransactionsAsync(decimal highVolumeThreshold);

    }
}

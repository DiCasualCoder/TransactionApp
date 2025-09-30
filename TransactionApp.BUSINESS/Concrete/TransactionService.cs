using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using TransactionApp.BUSINESS.Abstract;
using TransactionApp.CORE.CustomException.TransactionException;
using TransactionApp.CORE.Utilities.Result.Abstract;
using TransactionApp.CORE.Utilities.Result.Concrete;
using TransactionApp.DAL.Abstract.EntityFramework;
using TransactionApp.DAL.Abstract.EntityFramework.Repositories;
using TransactionApp.ENTITIES.Concrete.TransactionManager;
using TransactionApp.ENTITIES.Dto.TransactionDto;

namespace TransactionApp.BUSINESS.Concrete
{
    public class TransactionService : ITransactionService
    {
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserService _userService;

        private const string TotalTransactionPerUserCacheKey = "User_TotalTransactions";
        private const string TotalTransactionPerTransactionTypeCacheKey = "TransactionType_TotalTransactions";

        public TransactionService(ITransactionRepository transactionRepository, IUserService userService, IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
        {
            _mapper = mapper;
            _transactionRepository = transactionRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        /// <summary>
        /// Adds a new transaction and updates the cache for total transactions per user
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>Newly created transaction ID</returns>
        public async Task<IDataResult<int>> AddTransactionAsync(TransactionAddDto transaction)
        {
            if (transaction is null)
                throw new ArgumentNullException(nameof(transaction), "Transaction data cannot be null.");

            var findUserResult = await _userService.GetUserByIdAsync(transaction.UserId);
            if (!findUserResult.Success)
                throw new UserNotFoundException(transaction.UserId.ToString());

            var newTransaction = _mapper.Map<TRANSACTION>(transaction);
            await _transactionRepository.AddAsync(newTransaction);
            var detectedChanges = await _unitOfWork.CommitAsync();

            //In case of successful commit, recalculate related cache entries
            //Or create new cache if data not exists
            if (detectedChanges > 0)
            {
                await UpdateOrCreateCacheAsync<int>(
                    cacheKey: TotalTransactionPerUserCacheKey,
                    key: transaction.UserId,
                    amountToAdd: transaction.Amount,
                    createCacheFunc: CreateTotalTransactionsPerUserCache);

                await UpdateOrCreateCacheAsync<string>(
                    cacheKey: TotalTransactionPerTransactionTypeCacheKey,
                    key: transaction.TransactionType.ToString(),
                    amountToAdd: transaction.Amount,
                    createCacheFunc: CreateTotalTransactionsPerTransactionTypeCache);
            }
            return new SuccessDataResult<int>(newTransaction.Id, "Transaction added successfully");
        }

        /// <summary>
        /// Get all transactions
        /// </summary>
        /// <returns>List of transactions</returns>
        public async Task<IDataResult<List<TransactionFetchDto>>> GetAllTransactionsAsync()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            return new SuccessDataResult<List<TransactionFetchDto>>(_mapper.Map<List<TransactionFetchDto>>(transactions), "Transactions retrieved successfully");
        }

        /// <summary>
        /// Returns total transaction amounts for each user
        /// </summary>
        /// <returns>Per User - Total Amount Dictionary</returns>
        public async Task<IDataResult<Dictionary<int, decimal>>> TotalAmountPerUserAsync()
        {
            //Look for cached data
            if (_cache.TryGetValue(TotalTransactionPerUserCacheKey, out Dictionary<int, decimal> totalTransactionsPerUser))
            {
                return new SuccessDataResult<Dictionary<int, decimal>>(totalTransactionsPerUser, "Total transactions per user successfully retrieved.");
            }

            //Cache miss
            return new SuccessDataResult<Dictionary<int, decimal>>(await CreateTotalTransactionsPerUserCache(), "Total transactions per user calculated");
        }

        /// <summary>
        /// Returns total transaction amounts per transaction type
        /// </summary>
        /// <returns>Per Transaction Type - Total Amount Dictionary</returns>
        public async Task<IDataResult<Dictionary<string, decimal>>> TotalAmountPerTransactionAsync()
        {
            //look for cached data
            if (_cache.TryGetValue(TotalTransactionPerTransactionTypeCacheKey, out Dictionary<string, decimal> totalTransactionsPerTransactionType))
            {
                return new SuccessDataResult<Dictionary<string, decimal>>(totalTransactionsPerTransactionType, "Total transactions per transaction type successfully retrieved");
            }

            //Cache miss
            return new SuccessDataResult<Dictionary<string, decimal>>(await CreateTotalTransactionsPerTransactionTypeCache(), "Total transactions per transaction type calculated");
        }

        public async Task<IDataResult<List<TransactionHighVolumeDto>>> GetHighVolumeTransactionsAsync(decimal highVolumeThreshold)
        {
            var highVolumeTransactions = (await _transactionRepository
                .GetWhereAsync(t => t.Amount > highVolumeThreshold, y => y.User))
                .Select(x => new TransactionHighVolumeDto
                {
                    UserName = string.Join(" ", x.User.Name, x.User.Surname),
                    Amount = x.Amount,
                    TransactionType = x.TransactionType.ToString(),
                    CreatedAt = x.CreatedAt
                }).ToList();

            return new SuccessDataResult<List<TransactionHighVolumeDto>>(highVolumeTransactions, "High volume transactions retrieved successfully");
        }

        private async Task<Dictionary<int, decimal>> CreateTotalTransactionsPerUserCache()
        {
            var transactions = await _transactionRepository.GetAllAsync();

            var totalTransactionPerUser = transactions
                .GroupBy(t => t.UserId)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

            _cache.Set(TotalTransactionPerUserCacheKey, totalTransactionPerUser, new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.NeverRemove
            });

            return totalTransactionPerUser;
        }

        private async Task<Dictionary<string, decimal>> CreateTotalTransactionsPerTransactionTypeCache()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            var totalTransactionPerType = transactions
                .GroupBy(t => t.TransactionType)
                .ToDictionary(g => g.Key.ToString(), g => g.Sum(t => t.Amount));

            _cache.Set(TotalTransactionPerTransactionTypeCacheKey, totalTransactionPerType, new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.NeverRemove
            });

            return totalTransactionPerType;
        }

        private async Task UpdateOrCreateCacheAsync<TKey>(
            string cacheKey,
            TKey key,
            decimal amountToAdd,
            Func<Task> createCacheFunc)
        {
            //If cache exists
            //Update the value or create new key-value pair
            if (_cache.TryGetValue(cacheKey, out Dictionary<TKey, decimal> cacheData))
            {
                if (cacheData.ContainsKey(key))
                {
                    cacheData[key] += amountToAdd;
                }
                else
                {
                    cacheData.Add(key, amountToAdd);
                }
            }
            //If cache does not exist
            //Create cache
            else
            {
                await createCacheFunc();
            }
        }
    }
}

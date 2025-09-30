using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using TransactionApp.BUSINESS.Abstract;
using TransactionApp.BUSINESS.Concrete;
using TransactionApp.CORE.CustomException.TransactionException;
using TransactionApp.CORE.Utilities.Result.Concrete;
using TransactionApp.DAL.Abstract.EntityFramework;
using TransactionApp.DAL.Abstract.EntityFramework.Repositories;
using TransactionApp.ENTITIES.Concrete.TransactionManager;
using TransactionApp.ENTITIES.Dto.TransactionDto;
using TransactionApp.ENTITIES.Dto.UserDto;

namespace TransactionApp.UNIT_TESTS.Services
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepo;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IMemoryCache _cache;
        private readonly TransactionService _transactionService;


        public TransactionServiceTests()
        {
            _mockTransactionRepo = new Mock<ITransactionRepository>();
            _mockUserService = new Mock<IUserService>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();

            _cache = new MemoryCache(new MemoryCacheOptions());

            _transactionService = new TransactionService(
                _mockTransactionRepo.Object,
                _mockUserService.Object,
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _cache
                );
        }

        [Fact]
        public async Task AddTransactionAsync_WithNullTransaction_ThrowsArgumentNullException()
        {
            // No Arrange - Passing Null
            
            //Act
            Func<Task> act = async () => await _transactionService.AddTransactionAsync(null);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("transaction");
        }

        [Fact]
        public async Task AddTransactionAsync_WithNonExistentUser_ThrowsUserNotFoundException()
        {
            // Arrange
            var transactionDto = new TransactionAddDto
            {
                UserId = 999999,
                Amount = 100.50m,
                TransactionType = TransactionTypeEnum.Credit
            };

            var failedUserResult = new ErrorDataResult<UserDto>(null, "User could no be found");
            _mockUserService.Setup(us => us.GetUserByIdAsync(transactionDto.UserId))
                .ReturnsAsync(failedUserResult);

            // Act
            Func<Task> act = async () => await _transactionService.AddTransactionAsync(transactionDto);

            // Assert
            await act.Should().ThrowAsync<UserNotFoundException>()
                .WithMessage($"User with ID {transactionDto.UserId} not found.");
        }

        [Fact]
        public async Task AddTransactionAsync_WithValidTransaction_ReturnsSuccessResult()
        {
            //Arrange
            var transactionDto = new TransactionAddDto
            {
                UserId = 1,
                Amount = 250.75m,
                TransactionType = TransactionTypeEnum.Credit
            };

            var userDto = new UserDto
            {
                Id = 1,
                Name = "John",
                Surname = "Doe"
            };
            var successUserResult = new SuccessDataResult<UserDto>(userDto, "User found");

            var transactionEntity = new TRANSACTION
            {
                Id = 100,
                UserId = 1,
                Amount = 250.75m,
                TransactionType = TransactionTypeEnum.Credit
            };

            _mockUserService
                .Setup(x => x.GetUserByIdAsync(1))
                .ReturnsAsync(successUserResult);

            _mockMapper
                .Setup(x => x.Map<TRANSACTION>(transactionDto))
                .Returns(transactionEntity);

            _mockTransactionRepo
                .Setup(x => x.AddAsync(It.IsAny<TRANSACTION>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.CommitAsync())
                .ReturnsAsync(1);

            //Act
            var result = await _transactionService.AddTransactionAsync(transactionDto);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().Be(100);
            result.Message.Should().Contain("successfully");

            _mockTransactionRepo.Verify(x => x.AddAsync(It.IsAny<TRANSACTION>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task AddTransactionAsync_WhenCacheExists_UpdateCacheCorrectly()
        {
            //Arrange
            var transactionDto = new TRANSACTION
            {
                UserId = 1,
                Amount = 100m,
                TransactionType = TransactionTypeEnum.Debit
            };

            //Setup Cache with mock data
            var existingCache = new Dictionary<int, decimal>
            {
                { 1, 500m }
            };
            _cache.Set("User_TotalTransactions", existingCache);

            var userDto = new UserDto { Id = 1, Name = "John", Surname = "Doe" };
            var successUserResult = new SuccessDataResult<UserDto>(userDto, "User found");

            var transactionEntity = new TransactionAddDto { UserId = 1, Amount = 100m };

            _mockUserService
                .Setup(x => x.GetUserByIdAsync(1))
                .ReturnsAsync(successUserResult);

            _mockMapper
                .Setup(x => x.Map<TransactionAddDto>(transactionDto))
                .Returns(transactionEntity);

            _mockUnitOfWork
                .Setup(x => x.CommitAsync())
                .ReturnsAsync(1);

            //Act
            await _transactionService.AddTransactionAsync(transactionEntity);

            //Assert
            var cachedData = _cache.Get<Dictionary<int, decimal>>("User_TotalTransactions");
            cachedData.Should().ContainKey(1);
            cachedData[1].Should().Be(600m);
        }

        [Fact]
        public async Task TotalAmountPerUser_WhenCacheMissing_CalculatesAndCachesData()
        {
            //Arrange
            var transactions = new List<TRANSACTION>
            {
                new TRANSACTION { UserId = 1, Amount = 100m },
                new TRANSACTION { UserId = 2, Amount = 500m },
                new TRANSACTION { UserId = 1, Amount = 200m }
            };

            _mockTransactionRepo
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(transactions);

            //Act
            var result = await _transactionService.TotalAmountPerUser();

            //Assert
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data[1].Should().Be(300m);
            result.Data[2].Should().Be(500m);

            var cachedData = _cache.Get<Dictionary<int, decimal>>("User_TotalTransactions");
            cachedData.Should().NotBeNull();
            cachedData.Should().BeEquivalentTo(result.Data);
        }

        [Fact]
        public async Task GetHighVolumeTransactions_WithValidThreshold_ReturnsFilteredTransactions()
        {
            //Arrange
            var threshold = 1000m;
            var user = new USER { Id = 1, Name = "Jane", Surname = "Smith" };
            var transactions = new List<TRANSACTION>
            {
                new TRANSACTION { Amount = 500m, UserId = 1, TransactionType = TransactionTypeEnum.Debit },
                new TRANSACTION { Amount = 1500m, UserId = 1, TransactionType = TransactionTypeEnum.Credit },
                new TRANSACTION { Amount = 2000m, UserId = 1, TransactionType = TransactionTypeEnum.Debit }
            };

            _mockTransactionRepo
                .Setup(x => x.GetWhereAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<TRANSACTION, bool>>>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<TRANSACTION, object>>>()))
                .ReturnsAsync(transactions.Where(t => t.Amount > threshold).ToList());

            //Act
            var result = await _transactionService.GetHighVolumeTransactions(threshold);

            //Assert
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data.Should().OnlyContain(t => t.Amount > threshold);
            result.Data.First().UserName.Should().Be("Jane Smith");
        }
    }
}

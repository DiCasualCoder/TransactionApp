using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TransactionApp.BUSINESS.Concrete;
using TransactionApp.BUSINESS.MapperConfiguration.AutoMapper;
using TransactionApp.DAL.Concrete.EntityFramework;
using TransactionApp.DAL.Concrete.EntityFramework.Contexts;
using TransactionApp.DAL.Concrete.EntityFramework.Repositories;
using TransactionApp.ENTITIES.Concrete.TransactionManager;
using TransactionApp.ENTITIES.Dto.TransactionDto;

namespace TransactionApp.INTEGRATION_TESTS.Services
{
    public class TransactionServiceIntegrationTests
    {
        private readonly DbContextOptions<TransactionManagerDbContext> _dbOptions;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public TransactionServiceIntegrationTests()
        {
            _dbOptions = new DbContextOptionsBuilder<TransactionManagerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var config = new MapperConfiguration((cfg =>
            {
                cfg.AddProfile<MapProfile>();
                cfg.LicenseKey = "<eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzkwNjQwMDAwIiwiaWF0IjoiMTc1OTE0Mzg1MiIsImFjY291bnRfaWQiOiIwMTk5OTUxZWMzOWM3ZDZjOWI4ZDc0N2E2YmExMzAzMCIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazZhajl5MWhxa3pqdjhiZ2YyMWFyamE0Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.zt33jqAZfGYCH7BgaEIovJcMe33Q2fzXDi4XpzoaBZon-RAhdzL-rNkKKpO1iZOh8twT7Ee-X8tL4UKDfGxaxXUTbORK1UO0-geugXxwJUjML51dnZfvv8B4bwIegFqIkeVELdDOoQluF_sZcF5BsjMTmqoHcLWxA7oOW748YWqAMF13zcrJU6z-GlxCbaw5vpi17pNByM7V-VhfFybMld6mMcocb14uot8wZm1KaWc-o8O3K5LcmUuqghMAQrybUE2tAv5jMUfttrbw9NmUJ-ZSOyKRgIc50RTfcghiGMLCkO_ffdvyKKWkjfJ2kijDZl4ZKSw2msZBclRHeQU-5w>";
            }),null);
            _mapper = config.CreateMapper();

            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        [Fact]
        public async Task AddTransaction_FullFlow_SavesTransactionAndUpdatesCache()
        {
            //Arrange
            using var context = new TransactionManagerDbContext(_dbOptions);

            var user = new USER
            {
                Id = 1,
                Name = "TestName",
                Surname = "TestSurname",
                Email = "test@example.com"
            };

            context.USERS.Add(user);
            await context.SaveChangesAsync();

            var transactionRepo = new TransactionRepository(context);
            var userRepo = new UserRepository(context);
            var unitOfWork = new UnitOfWork(context);
            var userService = new UserService(userRepo,unitOfWork, _mapper);

            var transactionService = new TransactionService(
                transactionRepo,
                userService,
                unitOfWork,
                _mapper, 
                _cache);

            var transactionDto = new TransactionAddDto
            {
                UserId = 1,
                Amount = 500m,
                TransactionType = TransactionTypeEnum.Debit
            };

            //Act
            var result = await transactionService.AddTransactionAsync(transactionDto);

            //Assert
            result.Success.Should().BeTrue();
            result.Data.Should().BeGreaterThan(0);

            var savedTransaction = await context.TRANSACTIONS.FindAsync(result.Data);
            savedTransaction.Should().NotBeNull();
            savedTransaction.Amount.Should().Be(500m);
            savedTransaction.UserId.Should().Be(1);

            var cachedTotalTransactionData = _cache.Get<Dictionary<int, decimal>>("User_TotalTransactions");
            cachedTotalTransactionData.Should().NotBeNull();
            cachedTotalTransactionData.Should().ContainKey(1);
            cachedTotalTransactionData[1].Should().Be(500m);

            var cachedTransactionTypeData = _cache.Get<Dictionary<string, decimal>>("TransactionType_TotalTransactions");
            cachedTransactionTypeData.Should().NotBeNull();
            cachedTransactionTypeData.Should().ContainKey("Debit");
            cachedTransactionTypeData["Debit"].Should().Be(500m);
        }

        [Fact]
        public async Task TotalAmountPerUser_WithMultipleTransactions_CalculatesCorrectly()
        {
            // Arrange
            using var context = new TransactionManagerDbContext(_dbOptions);

            var users = new[]
            {
                new USER { Id = 1, Name = "User", Surname = "One", Email = "user1@test.com" },
                new USER { Id = 2, Name = "User", Surname = "Two", Email = "user2@test.com" }
            };
            context.USERS.AddRange(users);

            var transactions = new[]
            {
                new TRANSACTION { UserId = 1, Amount = 100m, TransactionType = TransactionTypeEnum.Debit },
                new TRANSACTION { UserId = 1, Amount = 200m, TransactionType = TransactionTypeEnum.Credit },
                new TRANSACTION { UserId = 1, Amount = 150m, TransactionType = TransactionTypeEnum.Debit },
                new TRANSACTION { UserId = 2, Amount = 500m, TransactionType = TransactionTypeEnum.Credit },
                new TRANSACTION { UserId = 2, Amount = 250m, TransactionType = TransactionTypeEnum.Debit }
            };
            context.TRANSACTIONS.AddRange(transactions);
            await context.SaveChangesAsync();

            var transactionRepo = new TransactionRepository(context);
            var userRepo = new UserRepository(context);
            var unitOfWork = new UnitOfWork(context);
            var userService = new UserService(userRepo, unitOfWork, _mapper);

            var transactionService = new TransactionService(
                transactionRepo,
                userService,
                unitOfWork,
                _mapper,
                _cache
            );

            // Act
            var result = await transactionService.TotalAmountPerUser();

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data[1].Should().Be(450m); // 100 + 200 + 150
            result.Data[2].Should().Be(750m); // 500 + 250

            // Verify second call uses cache
            var result2 = await transactionService.TotalAmountPerUser();
            result2.Message.Should().Contain("retrieved"); // Cache hit message
        }

        [Fact]
        public async Task GetHighVolumeTransactions_WithRealDatabase_FiltersCorrectly()
        {
            // Arrange
            using var context = new TransactionManagerDbContext(_dbOptions);

            var user = new USER
            {
                Id = 1,
                Name = "High",
                Surname = "Roller",
                Email = "highroller@test.com"
            };
            context.USERS.Add(user);

            var transactions = new[]
            {
                new TRANSACTION { UserId = 1, Amount = 500m, TransactionType = TransactionTypeEnum.Debit, CreatedAt = DateTime.UtcNow },
                new TRANSACTION { UserId = 1, Amount = 5000m, TransactionType = TransactionTypeEnum.Credit, CreatedAt = DateTime.UtcNow },
                new TRANSACTION { UserId = 1, Amount = 10000m, TransactionType = TransactionTypeEnum.Debit, CreatedAt = DateTime.UtcNow },
                new TRANSACTION { UserId = 1, Amount = 750m, TransactionType = TransactionTypeEnum.Credit, CreatedAt = DateTime.UtcNow }
            };
            context.TRANSACTIONS.AddRange(transactions);
            await context.SaveChangesAsync();

            var transactionRepo = new TransactionRepository(context);
            var userRepo = new UserRepository(context);
            var unitOfWork = new UnitOfWork(context);
            var userService = new UserService(userRepo,unitOfWork, _mapper);

            var transactionService = new TransactionService(
                transactionRepo,
                userService,
                unitOfWork,
                _mapper,
                _cache
            );

            // Act
            var result = await transactionService.GetHighVolumeTransactions(1000m);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2); // Only 5000 and 10000
            result.Data.Should().OnlyContain(t => t.Amount > 1000m);
            result.Data.Should().Contain(t => t.UserName == "High Roller");
        }

        [Fact]
        public async Task CompleteWorkflow_AddMultipleTransactions_AllOperationsWork()
        {
            // Arrange - This test simulates a real-world scenario
            using var context = new TransactionManagerDbContext(_dbOptions);

            // Create users
            var users = new[]
            {
                new USER { Id = 1, Name = "Alice", Surname = "Smith", Email = "alice@test.com" },
                new USER { Id = 2, Name = "Bob", Surname = "Jones", Email = "bob@test.com" }
            };
            context.USERS.AddRange(users);
            await context.SaveChangesAsync();

            var transactionRepo = new TransactionRepository(context);
            var userRepo = new UserRepository(context);
            var unitOfWork = new UnitOfWork(context);
            var userService = new UserService(userRepo,unitOfWork, _mapper);

            var transactionService = new TransactionService(
                transactionRepo,
                userService,
                unitOfWork,
                _mapper,
                _cache
            );

            // Act - Add multiple transactions
            await transactionService.AddTransactionAsync(new TransactionAddDto
            {
                UserId = 1,
                Amount = 1000m,
                TransactionType = TransactionTypeEnum.Debit
            });

            await transactionService.AddTransactionAsync(new TransactionAddDto
            {
                UserId = 1,
                Amount = 500m,
                TransactionType = TransactionTypeEnum.Credit
            });

            await transactionService.AddTransactionAsync(new TransactionAddDto
            {
                UserId = 2,
                Amount = 2000m,
                TransactionType = TransactionTypeEnum.Debit
            });

            // Get all transactions
            var allTransactions = await transactionService.GetAllTransactionsAsync();

            // Get totals per user
            var totalsPerUser = await transactionService.TotalAmountPerUser();

            // Get totals per type
            var totalsPerType = await transactionService.TotalAmountPerTransaction();

            // Get high volume
            var highVolume = await transactionService.GetHighVolumeTransactions(1500m);

            // Assert
            allTransactions.Data.Should().HaveCount(3);

            totalsPerUser.Data[1].Should().Be(1500m); // 1000 + 500
            totalsPerUser.Data[2].Should().Be(2000m);

            totalsPerType.Data["Debit"].Should().Be(3000m); // 1000 + 2000
            totalsPerType.Data["Credit"].Should().Be(500m);

            highVolume.Data.Should().HaveCount(1); // Only 2000
            highVolume.Data[0].Amount.Should().Be(2000m);
            highVolume.Data[0].UserName.Should().Be("Bob Jones");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using TransactionApp.BUSINESS.Abstract;
using TransactionApp.BUSINESS.Concrete;
using TransactionApp.ENTITIES.Dto.TransactionDto;

namespace TransactionApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Create a new transaction
        /// </summary>
        /// <param name="transactionAddDto"></param>
        /// <returns>Created transaction ID</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TransactionAddDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddTransaction([FromBody] TransactionAddDto transactionAddDto)
        {
            var result = await _transactionService.AddTransactionAsync(transactionAddDto);
            return CreatedAtAction(nameof(GetTransactionById), new { id = result.Data }, transactionAddDto);
        }

        /// <summary>
        /// Get transaction by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Transaction information</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TransactionFetchDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var result = await _transactionService.GetTransactionByIdAsync(id);
            return Ok(result.Data);
        }

        /// <summary>
        /// Get all transactions
        /// </summary>
        /// <returns>List of transactions</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<TransactionFetchDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTransactions()
        {
            var result = await _transactionService.GetAllTransactionsAsync();
            return Ok(result.Data);
        }

        /// <summary>
        /// Gets the total transaction amount per user
        /// </summary>
        /// <returns>Per User - Total Amount Dictionary</returns>
        [HttpGet("totals/by-user")]
        [ProducesResponseType(typeof(Dictionary<int, decimal>), StatusCodes.Status200OK)]
        public async Task<IActionResult> TotalTransactionAmountPerUser()
        {
            var result = await _transactionService.TotalAmountPerUserAsync();
            return Ok(result.Data);
        }

        /// <summary>
        /// Gets the total transaction amount per transaction type
        /// </summary>
        /// <returns>Per Transaction Type - Total Amount Dictionary</returns>
        [HttpGet("totals/by-type")]
        [ProducesResponseType(typeof(Dictionary<string, decimal>), StatusCodes.Status200OK)]
        public async Task<IActionResult> TotalTransactionAmountPerTransactionType()
        {
            var result = await _transactionService.TotalAmountPerTransactionAsync();
            return Ok(result.Data);
        }

        [HttpGet("high-volume")]
        [ProducesResponseType(typeof(List<TransactionHighVolumeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHighVolumeTransactions([FromQuery] decimal highVolumeThreshold)
        {
            var result = await _transactionService.GetHighVolumeTransactionsAsync(highVolumeThreshold);
            return Ok(result.Data);
        }
    }
}

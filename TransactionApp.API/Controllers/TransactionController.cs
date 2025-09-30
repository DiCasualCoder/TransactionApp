using Microsoft.AspNetCore.Mvc;
using TransactionApp.BUSINESS.Abstract;
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
        public async Task<IActionResult> AddTransaction([FromBody] TransactionAddDto transactionAddDto)
        {
            var result = await _transactionService.AddTransactionAsync(transactionAddDto);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// Get all transactions
        /// </summary>
        /// <returns>List of transactions</returns>
        [HttpGet("GetAllTransactions")]
        [ProducesResponseType(typeof(List<TransactionFetchDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTransactions()
        {
            var result = await _transactionService.GetAllTransactionsAsync();
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// Gets the total transaction amount per user
        /// </summary>
        /// <returns>Per User - Total Amount Dictionary</returns>
        [HttpGet("TotalTransactionAmountPerUser")]
        public async Task<IActionResult> TotalTransactionAmountPerUser()
        {
            var result = await _transactionService.TotalAmountPerUserAsync();
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// Gets the total transaction amount per transaction type
        /// </summary>
        /// <returns>Per Transaction Type - Total Amount Dictionary</returns>
        [HttpGet("TotalTransactionAmountPerTransactionType")]
        public async Task<IActionResult> TotalTransactionAmountPerTransactionType()
        {
            var result = await _transactionService.TotalAmountPerTransactionAsync();
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result);
        }

        [HttpGet("GetHighVolumeTransactions")]
        public async Task<IActionResult> GetHighVolumeTransactions(decimal highVolumeThreshold)
        {
            var result = await _transactionService.GetHighVolumeTransactionsAsync(highVolumeThreshold);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result);
        }
    }
}

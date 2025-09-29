using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionApp.ENTITIES.Concrete.TransactionManager;

namespace TransactionApp.ENTITIES.Dto.TransactionDto
{
    public class TransactionAddDto
    {
        [Required(ErrorMessage = "User ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be greater than zero")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Transaction amount is required")]
        [Range(0.01, int.MaxValue, ErrorMessage = "Transaction amount must be greater than zero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Transaction type is required")]
        [EnumDataType(typeof(TransactionTypeEnum), ErrorMessage = "Invalid transaction type")]
        public TransactionTypeEnum TransactionType { get; set; }
    }
}

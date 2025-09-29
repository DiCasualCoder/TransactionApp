using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionApp.ENTITIES.Concrete.TransactionManager;

namespace TransactionApp.ENTITIES.Dto.TransactionDto
{
    public class TransactionFetchDto
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public TransactionTypeEnum TransactionType { get; set; }
    }
}

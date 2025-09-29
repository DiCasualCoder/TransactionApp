using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionApp.ENTITIES.Concrete.TransactionManager;

namespace TransactionApp.ENTITIES.Dto.TransactionDto
{
    public class TransactionHighVolumeDto
    {
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

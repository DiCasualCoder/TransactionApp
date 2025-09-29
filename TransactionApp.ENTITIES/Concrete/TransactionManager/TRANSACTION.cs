
namespace TransactionApp.ENTITIES.Concrete.TransactionManager
{
    public class TRANSACTION
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public TransactionTypeEnum TransactionType { get; set; }
        public DateTime CreatedAt { get; set; }

        //1 to 1 relationship
        public virtual USER User { get; set; } = null!;
    }

    public enum TransactionTypeEnum
    {
        Debit,
        Credit
    }
}

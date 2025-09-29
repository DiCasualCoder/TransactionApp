
namespace TransactionApp.ENTITIES.Concrete.TransactionManager
{
    public class USER
    {
        public int Id { get; set; }
        public required string Name { get; set; } 
        public required string Surname { get; set; } 
        public string? Email { get; set; }

        // 1 to many relationship
        public virtual ICollection<TRANSACTION> Transactions { get; set; } = new List<TRANSACTION>();
    }
}

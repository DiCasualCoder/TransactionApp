namespace TransactionApp.CORE.CustomException.TransactionException
{
    public abstract class DomainException : Exception
    {
        public int StatusCode { get; }

        protected DomainException(string message, int statusCode = 400) : base(message)
        {
            StatusCode = statusCode;
        } 
        
    }

    public class UserNotFoundException : DomainException
    {
        public UserNotFoundException(string userId) : base($"User with ID {userId} not found", 404)
        {
        }
    }
}

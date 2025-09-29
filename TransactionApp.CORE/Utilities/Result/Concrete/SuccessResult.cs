using TransactionApp.CORE.Utilities.Result.Abstract;

namespace TransactionApp.CORE.Utilities.Result.Concrete
{
    public class SuccessResult : Result
    {
        public SuccessResult() : base(true)
        {
        } 
        
        public SuccessResult(string message) : base(true, message)
        {
        }
    }
}

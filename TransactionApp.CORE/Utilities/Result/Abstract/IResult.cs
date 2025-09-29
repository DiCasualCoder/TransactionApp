namespace TransactionApp.CORE.Utilities.Result.Abstract
{
    public interface IResult
    {
        bool Success { get; }
        string Message { get; }
    }
}

using TransactionApp.CORE.Utilities.Result.Abstract;
using TransactionApp.ENTITIES.Concrete.TransactionManager;
using TransactionApp.ENTITIES.Dto.UserDto;

namespace TransactionApp.BUSINESS.Abstract
{
    public interface IUserService
    {
        Task<IDataResult<List<UserListDto>>> GetAllUsersAsync();
        Task<IDataResult<UserDto>> GetUserByIdAsync(int id);
        Task<IDataResult<int>> AddUserAsync(UserCreateDto user);
        Task<IResult> UpdateUserAsync(UserUpdateDto user);
        Task<IResult> DeleteUserAsync(int id);
    }
}

using AutoMapper;
using TransactionApp.BUSINESS.Abstract;
using TransactionApp.CORE.Utilities.Result.Abstract;
using TransactionApp.CORE.Utilities.Result.Concrete;
using TransactionApp.DAL.Abstract.EntityFramework;
using TransactionApp.DAL.Abstract.EntityFramework.Repositories;
using TransactionApp.ENTITIES.Concrete.TransactionManager;
using TransactionApp.ENTITIES.Dto.UserDto;

namespace TransactionApp.BUSINESS.Concrete
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>The id of the newly created user</returns>
        public async Task<IDataResult<int>> AddUserAsync(UserCreateDto user)
        {
            try
            {
                if (user is null)
                {
                    return new ErrorDataResult<int>(nameof(user) + " cannot be null");
                }

                var newUser = _mapper.Map<USER>(user);

                await _userRepository.AddAsync(newUser);
                await _unitOfWork.CommitAsync();

                return new SuccessDataResult<int>(newUser.Id, "User added successfully");
            }
            catch (Exception ex)
            {
                //Logging mechanism can be added here
                return new ErrorDataResult<int>($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<IResult> DeleteUserAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new ErrorResult($"Invalid User ID: {id}");
                }

                _userRepository.RemoveById(id);
                await _unitOfWork.CommitAsync();

                return new SuccessResult($"User with ID {id} deleted successfully");
            }
            catch (Exception ex)
            {
                //Logging mechanism can be added here
                return new ErrorResult($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<IDataResult<List<UserListDto>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                return new SuccessDataResult<List<UserListDto>>(_mapper.Map<List<UserListDto>>(users), "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                //Logging mechanism can be added here
                return new ErrorDataResult<List<UserListDto>>($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<IDataResult<USER>> GetUserByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new ErrorDataResult<USER>($"Invalid User ID: {id}");
                }

                var user = await _userRepository.GetByIdAsync(id);
                if (user is null)
                {
                    return new ErrorDataResult<USER>($"User with ID {id} could not be found");
                }

                return new SuccessDataResult<USER>(user, "User retrieved successfully");
            }
            catch (Exception ex)
            {
                //Logging mechanism can be added here
                return new ErrorDataResult<USER>($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<IResult> UpdateUserAsync(UserUpdateDto userUpdateDto)
        {
            try
            {
                if (userUpdateDto is null)
                {
                    return new ErrorResult(nameof(userUpdateDto) + " cannot be null");
                }

                var existingUser = await _userRepository.GetByIdAsync(userUpdateDto.Id);
                if (existingUser is null)
                {
                    return new ErrorResult($"User with ID {userUpdateDto.Id} could not be found");
                }

                _mapper.Map(userUpdateDto, existingUser);
                _userRepository.Update(existingUser);
                await _unitOfWork.CommitAsync();

                return new SuccessResult("User updated successfully");
            }
            catch (Exception ex)
            {
                //Logging mechanism can be added here
                return new ErrorResult($"Unexpected error: {ex.Message}");
            }
        }
    }
}

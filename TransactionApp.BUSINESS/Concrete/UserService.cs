using AutoMapper;
using TransactionApp.BUSINESS.Abstract;
using TransactionApp.CORE.CustomException.TransactionException;
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
            if (user is null)
                throw new ArgumentNullException(nameof(user), "Transaction data cannot be null.");

            var newUser = _mapper.Map<USER>(user);

            await _userRepository.AddAsync(newUser);
            await _unitOfWork.CommitAsync();

            return new SuccessDataResult<int>(newUser.Id, "User added successfully");
        }

        /// <summary>
        /// Deletes a user by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Operation result object</returns>
        public async Task<IResult> DeleteUserAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be greater than zero", nameof(id));


            _userRepository.RemoveById(id);
            await _unitOfWork.CommitAsync();

            return new SuccessResult($"User with ID {id} deleted successfully");
        }

        public async Task<IDataResult<List<UserListDto>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return new SuccessDataResult<List<UserListDto>>(_mapper.Map<List<UserListDto>>(users), "Users retrieved successfully");
        }

        public async Task<IDataResult<UserDto>> GetUserByIdAsync(int id)
        {   
            if (id <= 0)
                throw new ArgumentException("ID must be greater than zero", nameof(id));

            var user = await _userRepository.GetByIdAsync(id);
            if (user is null)
                throw new UserNotFoundException(id.ToString());

            return new SuccessDataResult<UserDto>(_mapper.Map<UserDto>(user), "User retrieved successfully");
        }

        public async Task<IResult> UpdateUserAsync(UserUpdateDto userUpdateDto)
        {
            if (userUpdateDto is null)
                throw new ArgumentNullException(nameof(userUpdateDto), "Transaction data cannot be null.");


            var existingUser = await _userRepository.GetByIdAsync(userUpdateDto.Id);
            if (existingUser is null)
                throw new UserNotFoundException(userUpdateDto.Id.ToString());


            _mapper.Map(userUpdateDto, existingUser);
            _userRepository.Update(existingUser);
            await _unitOfWork.CommitAsync();

            return new SuccessResult("User updated successfully");
        }
    }
}

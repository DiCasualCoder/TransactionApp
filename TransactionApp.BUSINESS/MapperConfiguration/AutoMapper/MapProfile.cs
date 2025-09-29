using AutoMapper;
using TransactionApp.ENTITIES.Concrete.TransactionManager;
using TransactionApp.ENTITIES.Dto.TransactionDto;
using TransactionApp.ENTITIES.Dto.UserDto;

namespace TransactionApp.BUSINESS.MapperConfiguration.AutoMapper
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            //User Profiles
            CreateMap<USER, UserListDto>();

            CreateMap<UserCreateDto, USER>();
            CreateMap<UserUpdateDto, USER>();

            //Transaction Profiles
            CreateMap<TRANSACTION, TransactionFetchDto>();

            CreateMap<TransactionAddDto, TRANSACTION>();

        }
    }
}

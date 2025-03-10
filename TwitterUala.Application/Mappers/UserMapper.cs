using TwitterUala.Application.Dtos;
using TwitterUala.Domain.Entities;

namespace TwitterUala.Application.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToDto(User user)
        {
            UserDto UserDto = new UserDto
            {
                IdUser = user.IdUser,
                Username = user.Username
            };
            return UserDto;
        }
    }
}

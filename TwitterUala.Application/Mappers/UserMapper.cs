using TwitterUala.Application.Dtos.Out;
using TwitterUala.Domain.Entities;

namespace TwitterUala.Application.Mappers
{
    public static class UserMapper
    {
        public static UserOutDto ToDto(User user)
        {
            UserOutDto UserDto = new UserOutDto
            {
                IdUser = user.IdUser,
                Username = user.Username
            };
            return UserDto;
        }
    }
}

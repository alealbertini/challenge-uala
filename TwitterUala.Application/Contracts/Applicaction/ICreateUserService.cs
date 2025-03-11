using TwitterUala.Application.Dtos.Out;

namespace TwitterUala.Application.Contracts.Applicaction
{
    public interface ICreateUserService
    {
        public Task<UserOutDto> CreateUserAsync(UserInDto userInDto);
    }
}
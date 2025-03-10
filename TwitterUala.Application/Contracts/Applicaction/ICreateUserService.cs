using TwitterUala.Application.Dtos;

namespace TwitterUala.Application.Contracts.Applicaction
{
    public interface ICreateUserService
    {
        public Task<UserDto> CreateUserAsync(string username);
    }
}
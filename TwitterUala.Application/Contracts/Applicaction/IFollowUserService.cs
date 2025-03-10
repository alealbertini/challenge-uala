using TwitterUala.Application.Dtos;

namespace TwitterUala.Application.Contracts.Applicaction
{
    public interface IFollowUserService
    {
        Task<FollowingDto> FollowUserAsync(long userId, long userToFollowId);
    }
}
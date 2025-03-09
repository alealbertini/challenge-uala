namespace TwitterUala.Application.Contracts.Applicaction
{
    public interface IFollowUserService
    {
        Task FollowUserAsync(long userId, long userToFollowId);
    }
}
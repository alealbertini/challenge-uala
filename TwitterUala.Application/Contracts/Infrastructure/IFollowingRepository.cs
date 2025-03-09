using TwitterUala.Domain.Entities;

namespace TwitterUala.Application.Contracts.Infrastructure
{
    public interface IFollowingRepository
    {
        IEnumerable<Tweet> TweetsFromFollowingByUserId(long userId);
    }
}
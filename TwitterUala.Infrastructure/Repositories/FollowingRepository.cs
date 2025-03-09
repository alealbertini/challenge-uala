using Microsoft.EntityFrameworkCore;
using TwitterUala.Application.Contracts.Infrastructure;
using TwitterUala.Domain.Entities;

namespace TwitterUala.Infrastructure.Repositories
{
    public class FollowingRepository(TwitterDbContext dbContext) : IFollowingRepository
    {
        private readonly TwitterDbContext _dbContext = dbContext;

        public IEnumerable<Tweet> TweetsFromFollowingByUserId(long userId)
        {
            var tweetsByUser = from following in _dbContext.Following
                               join tweet in _dbContext.Tweet on following.UsersToFollowId equals tweet.UserId
                               where following.UserId == userId
                               orderby tweet.TweetPosted ascending
                               select tweet;
            return tweetsByUser;
        }
    }
}

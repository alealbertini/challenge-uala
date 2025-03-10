using TwitterUala.Application.Dtos;

namespace TwitterUala.Application.Contracts.Applicaction
{
    public interface IPublishTweetService
    {
        Task<TweetDto> PublishTweetAsync(long userId, string tweetMessage);
    }
}
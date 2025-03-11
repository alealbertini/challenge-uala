using TwitterUala.Application.Dtos.Out;

namespace TwitterUala.Application.Contracts.Applicaction
{
    public interface IPublishTweetService
    {
        Task<TweetOutDto> PublishTweetAsync(TweetInDto tweetInDto);
    }
}
namespace TwitterUala.Application.Contracts.Applicaction
{
    public interface IPublishTweetService
    {
        Task PublishTweetAsync(long userId, string tweetMessage);
    }
}
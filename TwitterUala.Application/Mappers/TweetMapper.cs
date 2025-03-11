using TwitterUala.Application.Dtos.Out;
using TwitterUala.Domain.Entities;

namespace TwitterUala.Application.Mappers
{
    public static class TweetMapper
    {
        public static TweetOutDto ToDto(Tweet tweet)
        {
            TweetOutDto tweetDto = new TweetOutDto
            {
                UserId = tweet.UserId,
                TweetMessage = tweet.TweetMessage,
                TweetPosted = tweet.TweetPosted
            };
            return tweetDto;
        }
    }
}

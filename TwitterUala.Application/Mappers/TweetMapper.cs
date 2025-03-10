using TwitterUala.Application.Dtos;
using TwitterUala.Domain.Entities;

namespace TwitterUala.Application.Mappers
{
    public static class TweetMapper
    {
        public static TweetDto ToDto(Tweet tweet)
        {
            TweetDto tweetDto = new TweetDto
            {
                UserId = tweet.UserId,
                TweetMessage = tweet.TweetMessage,
                TweetPosted = tweet.TweetPosted
            };
            return tweetDto;
        }
    }
}

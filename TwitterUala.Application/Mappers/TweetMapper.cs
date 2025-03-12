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
                UserTweet = tweet.UserTweet,
                TweetMessage = tweet.TweetMessage,
                TweetPosted = tweet.TweetPosted
            };
            return tweetDto;
        }

        public static List<TweetOutDto> ToDtoList(List<Tweet> tweets)
        {
            List<TweetOutDto> tweetDto = new List<TweetOutDto>();
            foreach(Tweet tweet in tweets)
            {
                tweetDto.Add(new TweetOutDto{
                    UserTweet = tweet.UserTweet,
                    TweetMessage = tweet.TweetMessage,
                    TweetPosted = tweet.TweetPosted
                });
            };
            return tweetDto;
        }
    }
}

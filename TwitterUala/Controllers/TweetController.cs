using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TwitterUala.Application.Contracts.Applicaction;
using TwitterUala.Application.Dtos.Out;
using TwitterUala.Domain.Entities;

namespace TwitterUala.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TweetController(ILogger<TweetController> logger, 
        IPublishTweetService publishTweetService,
        ITimelineService tweetsFromFollowingByUserService) : ControllerBase
    {
        private readonly ILogger<TweetController> _logger = logger;
        private readonly ITimelineService _tweetsFromFollowingByUserService = tweetsFromFollowingByUserService;
        private readonly IPublishTweetService _publishTweetService = publishTweetService;

        [HttpPost(Name = "PublishTweet")]
        public async Task<TweetOutDto> PublishTweetAsync(TweetInDto tweet)
        {
            _logger.LogInformation("Se publicará el tweet {0} ", JsonConvert.SerializeObject(tweet));
            TweetOutDto tweetDto = await _publishTweetService.PublishTweetAsync(tweet);
            return tweetDto;
        }

        [HttpGet(Name = "TimelineByUserId")]
        public async Task<List<TweetOutDto>> TimelineByUserId(long userId)
        {
            _logger.LogInformation("Obtener tweets para el usuario: {0}", userId);
            var tweets = await _tweetsFromFollowingByUserService.TimelineByUserIdAsync(userId);
            return tweets;
        }
    }
}

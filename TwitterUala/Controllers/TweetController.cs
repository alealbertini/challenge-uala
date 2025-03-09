using Microsoft.AspNetCore.Mvc;
using TwitterUala.Application.Contracts.Applicaction;
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
        public async Task PublishTweetAsync(long userId, string tweetMessage)
        {
            _logger.LogInformation("Se publicará el tweet {0} para el usuario: {1}", tweetMessage, userId);
            await _publishTweetService.PublishTweetAsync(userId, tweetMessage);
        }

        [HttpGet(Name = "TimelineByUserId")]
        public async Task<List<Tweet>> TimelineByUserId(long userId)
        {
            _logger.LogInformation("Obtener tweets para el usuario: {0}", userId);
            return await _tweetsFromFollowingByUserService.TimelineByUserIdAsync(userId);
        }
    }
}

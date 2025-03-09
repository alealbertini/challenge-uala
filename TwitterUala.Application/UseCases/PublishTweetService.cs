using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitterUala.Application.Contracts.Applicaction;
using TwitterUala.Application.Contracts.Infrastructure;
using TwitterUala.Domain.Entities;

namespace TwitterUala.Application.UseCases
{
    public class PublishTweetService(IUnitOfWork unitOfWork, ILogger<PublishTweetService> logger) : IPublishTweetService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<PublishTweetService> _logger = logger;

        public async Task PublishTweetAsync(long userId, string tweetMessage)
        {
            var validUser = await _unitOfWork.GetRepository<User>().FirstOrDefaultAsync(u => u.IdUser == userId);
            if (validUser == null)
            {
                throw new InvalidDataException("El usuario actual no es valido");
            }

            if (tweetMessage.Length <= 0)
            {
                throw new InvalidDataException("El mensaje no puede ser vacío");
            }

            if (tweetMessage.Length > 280)
            {
                throw new InvalidDataException("El mensaje no puede ser mayor a 280 caracteres");
            }

            Tweet tweet = new Tweet();
            tweet.UserId = userId;
            tweet.TweetMessage = tweetMessage;
            tweet.TweetPosted = DateTime.UtcNow;

            await _unitOfWork.GetRepository<Tweet>().Add(tweet);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Se publicó el tweet para el usuario: {0}", JsonConvert.SerializeObject(tweet));
        }
    }
}

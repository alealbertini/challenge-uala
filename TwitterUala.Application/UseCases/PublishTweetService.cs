using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitterUala.Application.Contracts.Applicaction;
using TwitterUala.Application.Contracts.Infrastructure;
using TwitterUala.Application.Dtos.Out;
using TwitterUala.Application.Mappers;
using TwitterUala.Domain.Entities;

namespace TwitterUala.Application.UseCases
{
    public class PublishTweetService(IUnitOfWork unitOfWork, ILogger<PublishTweetService> logger) : IPublishTweetService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<PublishTweetService> _logger = logger;

        public async Task<TweetOutDto> PublishTweetAsync(TweetInDto tweetInDto)
        {
            await ExecValidationsAsync(tweetInDto);

            Tweet tweet = new Tweet();
            tweet.UserId = tweetInDto.UserId;
            tweet.TweetMessage = tweetInDto.TweetMessage;
            tweet.TweetPosted = DateTime.UtcNow;

            await _unitOfWork.GetRepository<Tweet>().Add(tweet);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Se publicó el tweet para el usuario: {0}", JsonConvert.SerializeObject(tweet));

            TweetOutDto tweetDto = TweetMapper.ToDto(tweet);
            return tweetDto;
        }

        private async Task ExecValidationsAsync(TweetInDto tweetInDto)
        {
            var validUser = await _unitOfWork.GetRepository<User>().FirstOrDefaultAsync(u => u.IdUser == tweetInDto.UserId);
            if (validUser == null)
            {
                throw new InvalidDataException("El usuario actual no es valido");
            }

            if (tweetInDto.TweetMessage.Length <= 0)
            {
                throw new InvalidDataException("El mensaje no puede ser vacío");
            }

            if (tweetInDto.TweetMessage.Length > 280)
            {
                throw new InvalidDataException("El mensaje no puede ser mayor a 280 caracteres");
            }
        }
    }
}

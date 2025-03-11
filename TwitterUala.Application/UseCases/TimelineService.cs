using Microsoft.Extensions.Logging;
using TwitterUala.Application.Contracts.Applicaction;
using TwitterUala.Application.Contracts.Infrastructure;
using TwitterUala.Application.Dtos.Out;
using TwitterUala.Application.Mappers;
using TwitterUala.Domain.Entities;

namespace TwitterUala.Application.UseCases
{
    public class TimelineService(IFollowingRepository followingRepository, IUnitOfWork unitOfWork, ILogger<TimelineService> logger) : ITimelineService
    {
        private readonly IFollowingRepository _followingRepository = followingRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<TimelineService> _logger = logger;

        public async Task<List<TweetOutDto>> TimelineByUserIdAsync(long userId)
        {
            await ExecValidationsAsync(userId);

            var tweets = _followingRepository.TweetsFromFollowingByUserId(userId).ToList();
            _logger.LogInformation("Se obtuvieron los tweets para el usuario: {0}", userId);
            List<TweetOutDto> tweetsDto = TweetMapper.ToDtoList(tweets);
            return tweetsDto;
        }

        private async Task ExecValidationsAsync(long userId)
        {
            var validUser = await _unitOfWork.GetRepository<User>().FirstOrDefaultAsync(u => u.IdUser == userId);
            if (validUser == null)
            {
                throw new InvalidDataException("El usuario actual no es válido");
            }
        }
    }
}

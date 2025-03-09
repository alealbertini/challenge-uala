using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitterUala.Application.Contracts.Applicaction;
using TwitterUala.Application.Contracts.Infrastructure;
using TwitterUala.Domain.Entities;

namespace TwitterUala.Application.UseCases
{
    public class FollowUserService(IUnitOfWork unitOfWork, ILogger<FollowUserService> logger) : IFollowUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<FollowUserService> _logger = logger;

        public async Task FollowUserAsync(long userId, long userToFollowId)
        {
            var validUser = await _unitOfWork.GetRepository<User>().FirstOrDefaultAsync(u => u.IdUser == userId);
            if (validUser == null)
            {
                throw new InvalidDataException("El usuario actual no es válido");
            }

            var validUserToFollow = await _unitOfWork.GetRepository<User>().FirstOrDefaultAsync(u => u.IdUser == userToFollowId);
            if (validUserToFollow == null)
            {
                throw new InvalidDataException("El usuario a seguir no es válido");
            }

            if (userId == userToFollowId)
            {
                throw new InvalidDataException("El usuario actual no puede seguirse a si mismo");
            }

            Following following = new Following();
            following.UserId = userId;
            following.UsersToFollowId = userToFollowId;

            await _unitOfWork.GetRepository<Following>().Add(following);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Usuario seguido: {0}", JsonConvert.SerializeObject(following));
        }
    }
}

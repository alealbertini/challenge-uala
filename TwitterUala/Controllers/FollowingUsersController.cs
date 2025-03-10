using Microsoft.AspNetCore.Mvc;
using TwitterUala.Application.Contracts.Applicaction;
using TwitterUala.Application.Dtos;

namespace TwitterUala.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FollowingUsersController(ILogger<FollowingUsersController> logger, IFollowUserService followUserService) : ControllerBase
    {
        private readonly ILogger<FollowingUsersController> _logger = logger;
        private readonly IFollowUserService _followUserService = followUserService;

        [HttpPost(Name = "FollowUser")]
        public async Task<FollowingDto> FollowUserAsync(long userId, long userToFollowId)
        {
            _logger.LogInformation("Usuario a insertar: Usuario: {0} Usuario a seguir: {1}", userId, userToFollowId);
            FollowingDto followingDto = await _followUserService.FollowUserAsync(userId, userToFollowId);
            return followingDto;
        }
    }
}

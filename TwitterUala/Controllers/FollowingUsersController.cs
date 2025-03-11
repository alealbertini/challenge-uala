using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        public async Task<FollowingDto> FollowUserAsync(FollowingDto followingInDto)
        {
            _logger.LogInformation("Usuario a insertar: {0}", JsonConvert.SerializeObject(followingInDto));
            FollowingDto followingDto = await _followUserService.FollowUserAsync(followingInDto);
            return followingDto;
        }
    }
}

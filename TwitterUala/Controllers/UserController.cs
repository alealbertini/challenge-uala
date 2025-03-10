using Microsoft.AspNetCore.Mvc;
using TwitterUala.Application.Contracts.Applicaction;
using TwitterUala.Application.Dtos;

namespace TwitterUala.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(ICreateUserService createUserService, ILogger<UserController> logger) : ControllerBase
    {
        private readonly ICreateUserService _createUserService = createUserService;
        private readonly ILogger<UserController> _logger = logger;

        [HttpPost(Name = "User")]
        public async Task<UserDto> CreateUserAsync(string username)
        {
            _logger.LogInformation("Usuario a insertar: {0}", username);
            UserDto userDto = await _createUserService.CreateUserAsync(username);
            return userDto;
        }
    }
}

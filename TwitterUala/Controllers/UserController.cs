using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TwitterUala.Application.Contracts.Applicaction;
using TwitterUala.Application.Dtos.Out;

namespace TwitterUala.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(ICreateUserService createUserService, ILogger<UserController> logger) : ControllerBase
    {
        private readonly ICreateUserService _createUserService = createUserService;
        private readonly ILogger<UserController> _logger = logger;

        [HttpPost(Name = "User")]
        public async Task<UserOutDto> CreateUserAsync(UserInDto user)
        {
            _logger.LogInformation("Usuario a insertar: {0}", JsonConvert.SerializeObject(user));
            UserOutDto userDto = await _createUserService.CreateUserAsync(user);
            return userDto;
        }
    }
}

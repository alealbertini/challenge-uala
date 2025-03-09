using Microsoft.AspNetCore.Mvc;
using TwitterUala.Application.Contracts.Applicaction;

namespace TwitterUala.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(ICreateUserService createUserService, ILogger<UserController> logger) : ControllerBase
    {
        private readonly ICreateUserService _createUserService = createUserService;
        private readonly ILogger<UserController> _logger = logger;

        [HttpPost(Name = "User")]
        public async Task CreateUserAsync(string username)
        {
            _logger.LogInformation("Usuario a insertar: {0}", username);
            await _createUserService.CreateUserAsync(username);
        }
    }
}

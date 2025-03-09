using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitterUala.Application.Contracts.Applicaction;
using TwitterUala.Application.Contracts.Infrastructure;
using TwitterUala.Domain.Entities;

namespace TwitterUala.Application.UseCases
{
    public class CreateUserService(IUnitOfWork unitOfWork, ILogger<CreateUserService> logger) : ICreateUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CreateUserService> _logger = logger;

        public async Task CreateUserAsync(string username)
        {
            if (username.Length <= 0)
            {
                throw new InvalidDataException("El nombre del usuario no puede ser vacío");
            }

            if (username.Length > 50)
            {
                throw new InvalidDataException("El nombre del usuario no puede ser mayor a 50 caracteres");
            }

            var existUsername = await _unitOfWork.GetRepository<User>().FirstOrDefaultAsync(x => x.Username == username) != null;
            if (existUsername)
            {
                throw new InvalidDataException("Ya existe un usuario con el mismo nombre de usuario");
            }

            User user = new User();
            user.Username = username.Trim();

            await _unitOfWork.GetRepository<User>().Add(user);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Usuario insertado: {0}", JsonConvert.SerializeObject(user));
        }
    }
}

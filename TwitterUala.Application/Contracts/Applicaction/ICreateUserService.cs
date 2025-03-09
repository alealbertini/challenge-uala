namespace TwitterUala.Application.Contracts.Applicaction
{
    public interface ICreateUserService
    {
        public Task CreateUserAsync(string username);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TwitterUala.Application.Contracts.Applicaction;
using TwitterUala.Application.Contracts.Infrastructure;
using TwitterUala.Application.UseCases;
using TwitterUala.Infrastructure;
using TwitterUala.Infrastructure.Database;
using TwitterUala.Infrastructure.Handlers;
using TwitterUala.Infrastructure.Repositories;

namespace TwitterUalaTest
{
    [TestClass]
    public class CreateUserTestServices
    {
        public static ServiceProvider _serviceProvider;

        [TestInitialize]
        public void Setup()
        {
            var services = new ServiceCollection();

            services.AddDbContext<TwitterDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<DbContext, TwitterDbContext>();

            services.AddLogging();

            services.AddScoped<IFollowUserService, FollowUserService>();
            services.AddScoped<IPublishTweetService, PublishTweetService>();
            services.AddScoped<ITimelineService, TimelineService>();
            services.AddScoped<ICreateUserService, CreateUserService>();
            services.AddScoped<IFollowingRepository, FollowingRepository>();

            services.AddExceptionHandler<ExceptionHandler>();

            _serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public void CreateUser_ShouldInsertUserOk()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<ICreateUserService>();
                var dbContext = scopedServices.GetRequiredService<TwitterDbContext>();

                string username = "Benedicto";
                manager.CreateUserAsync(username);

                var addedItem = dbContext.User.Find((long)1);
                Assert.IsNotNull(addedItem);
                Assert.AreEqual(username, addedItem.Username);
            }
        }

        [TestMethod]
        public async Task CreateUserAsync_ThrowsInvalidDataException_WhenUsernameIsEmpty()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<ICreateUserService>();

                string username = "";
                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.CreateUserAsync(username));
                Assert.AreEqual("El nombre del usuario no puede ser vac�o", exception.Message);
            }
        }

        [TestMethod]
        public async Task CreateUserAsync_ThrowsInvalidDataException_WhenUsernameIsTooLongAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<ICreateUserService>();

                string longUsername = new string('a', 51);
                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.CreateUserAsync(longUsername));
                Assert.AreEqual("El nombre del usuario no puede ser mayor a 50 caracteres", exception.Message);
            }
        }

        [TestMethod]
        public async Task CreateUserAsync_ThrowsInvalidDataException_WhenUsernameAlreadyExists()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<ICreateUserService>();

                string username = "Benedicto";
                await manager.CreateUserAsync(username);

                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.CreateUserAsync(username));
                Assert.AreEqual("Ya existe un usuario con el mismo nombre de usuario", exception.Message);
            }
        }
    }
}
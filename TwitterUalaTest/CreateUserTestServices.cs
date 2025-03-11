using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TwitterUala.Application.Contracts.Applicaction;
using TwitterUala.Application.Contracts.Infrastructure;
using TwitterUala.Application.Dtos.Out;
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

            services.AddSingleton<IUnitOfWork, UnitOfWork>();
            services.AddSingleton(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddSingleton<DbContext, TwitterDbContext>();

            services.AddLogging();

            services.AddSingleton<IFollowUserService, FollowUserService>();
            services.AddSingleton<IPublishTweetService, PublishTweetService>();
            services.AddSingleton<ITimelineService, TimelineService>();
            services.AddSingleton<ICreateUserService, CreateUserService>();
            services.AddSingleton<IFollowingRepository, FollowingRepository>();

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

                UserInDto userInDto = new UserInDto();
                userInDto.Username = "Benedicto";
                manager.CreateUserAsync(userInDto);

                var addedItem = dbContext.User.Find((long)1);
                Assert.IsNotNull(addedItem);
                Assert.AreEqual(userInDto.Username, addedItem.Username);
            }
        }

        [TestMethod]
        public async Task CreateUserAsync_ThrowsInvalidDataException_WhenUsernameIsEmpty()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<ICreateUserService>();

                UserInDto userInDto = new UserInDto();
                userInDto.Username = "";
                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.CreateUserAsync(userInDto));
                Assert.AreEqual("El nombre del usuario no puede ser vacío", exception.Message);
            }
        }

        [TestMethod]
        public async Task CreateUserAsync_ThrowsInvalidDataException_WhenUsernameIsTooLongAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<ICreateUserService>();

                UserInDto userInDto = new UserInDto();
                userInDto.Username = new string('a', 51);
                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.CreateUserAsync(userInDto));
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

                UserInDto userInDto = new UserInDto();
                userInDto.Username = "Benedicto";
                await manager.CreateUserAsync(userInDto);

                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.CreateUserAsync(userInDto));
                Assert.AreEqual("Ya existe un usuario con el mismo nombre de usuario", exception.Message);
            }
        }
    }
}
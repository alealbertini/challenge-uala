using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TwitterUala.Application.Contracts.Applicaction;
using TwitterUala.Application.Contracts.Infrastructure;
using TwitterUala.Application.Dtos;
using TwitterUala.Application.Dtos.Out;
using TwitterUala.Application.UseCases;
using TwitterUala.Infrastructure;
using TwitterUala.Infrastructure.Database;
using TwitterUala.Infrastructure.Handlers;
using TwitterUala.Infrastructure.Repositories;

namespace TwitterUalaTest
{
    [TestClass]
    public class FollowUserTestServices
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
        public void FollowUserAsync_InsertOK()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<IFollowUserService>();
                var managerCreateUser = scopedServices.GetRequiredService<ICreateUserService>();
                var dbContext = scopedServices.GetRequiredService<TwitterDbContext>();

                UserInDto userInDto = new UserInDto();
                userInDto.Username = "Benedicto";
                managerCreateUser.CreateUserAsync(userInDto);

                UserInDto userInDtoAmanda = new UserInDto();
                userInDtoAmanda.Username = "Amanda";
                managerCreateUser.CreateUserAsync(userInDtoAmanda);

                FollowingDto followingDto = new FollowingDto();
                followingDto.UserId = 1;
                followingDto.UserToFollowId = 2;
                manager.FollowUserAsync(followingDto);

                var addedItem = dbContext.Following.FirstOrDefault(x => x.UserId == followingDto.UserId && x.UserToFollowId == followingDto.UserToFollowId);
                Assert.IsNotNull(addedItem);
            }
        }

        [TestMethod]
        public async Task FollowUserAsync_ThrowsInvalidDataException_WhenUserDoesNotExist()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<IFollowUserService>();

                FollowingDto followingDto = new FollowingDto();
                followingDto.UserId = 11111;
                followingDto.UserToFollowId = 2;
                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.FollowUserAsync(followingDto));
                Assert.AreEqual("El usuario actual no es válido", exception.Message);
            }
        }

        [TestMethod]
        public async Task CreateUserAsync_ThrowsInvalidDataException_WhenUserToFollowDoesNotExist()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<IFollowUserService>();
                var managerCreateUser = scopedServices.GetRequiredService<ICreateUserService>();

                UserInDto userInDto = new UserInDto();
                userInDto.Username = "Raul";
                await managerCreateUser.CreateUserAsync(userInDto);

                FollowingDto followingDto = new FollowingDto();
                followingDto.UserId = 1;
                followingDto.UserToFollowId = 222222;
                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.FollowUserAsync(followingDto));
                Assert.AreEqual("El usuario a seguir no es válido", exception.Message);
            }
        }

        [TestMethod]
        public async Task CreateUserAsync_ThrowsInvalidDataException_WhenUserAndUserToFollowAreTheSame()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<IFollowUserService>();
                var managerCreateUser = scopedServices.GetRequiredService<ICreateUserService>();

                UserInDto userInDto = new UserInDto();
                userInDto.Username = "Sebastian";
                await managerCreateUser.CreateUserAsync(userInDto);

                FollowingDto followingDto = new FollowingDto();
                followingDto.UserId = 1;
                followingDto.UserToFollowId = 1;
                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.FollowUserAsync(followingDto));
                Assert.AreEqual("El usuario actual no puede seguirse a si mismo", exception.Message);
            }
        }

        [TestMethod]
        public async Task FollowUserAsync_ThrowsInvalidDataException_WhenUserTryToFollowTheSameUserAgainAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<IFollowUserService>();
                var managerCreateUser = scopedServices.GetRequiredService<ICreateUserService>();
                var dbContext = scopedServices.GetRequiredService<TwitterDbContext>();

                UserInDto userInDto = new UserInDto();
                userInDto.Username = "Benedicto";
                managerCreateUser.CreateUserAsync(userInDto);

                UserInDto userInDtoAmanda = new UserInDto();
                userInDtoAmanda.Username = "Amanda";
                managerCreateUser.CreateUserAsync(userInDtoAmanda);

                FollowingDto followingDto = new FollowingDto();
                followingDto.UserId = 1;
                followingDto.UserToFollowId = 2;
                manager.FollowUserAsync(followingDto);

                manager.FollowUserAsync(followingDto);

                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.FollowUserAsync(followingDto));
                Assert.AreEqual("El usuario actual ya esta siguiendo al usuario a seguir", exception.Message);
            }
        }
    }
}
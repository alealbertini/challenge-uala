using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TwitterUala.Application.Contracts.Applicaction;
using TwitterUala.Application.Contracts.Infrastructure;
using TwitterUala.Application.UseCases;
using TwitterUala.Infrastructure;
using TwitterUala.Infrastructure.Impl;
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

            // Using In-Memory database for testing
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
        public void FollowUserAsync_InsertOK()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<IFollowUserService>();
                var managerCreateUser = scopedServices.GetRequiredService<ICreateUserService>();
                var dbContext = scopedServices.GetRequiredService<TwitterDbContext>();

                string usernameBenedicto = "Benedicto";
                managerCreateUser.CreateUserAsync(usernameBenedicto);

                string usernameAmanda = "Amanda";
                managerCreateUser.CreateUserAsync(usernameAmanda);

                long user = 1;
                long userToFollow = 2;
                manager.FollowUserAsync(user, userToFollow);

                // Assert
                var addedItem = dbContext.Following.FirstOrDefault(x => x.UserId == user && x.UsersToFollowId == userToFollow);
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
                var dbContext = scopedServices.GetRequiredService<TwitterDbContext>();

                long user = 11111;
                long userToFollow = 2;
                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.FollowUserAsync(user, userToFollow));
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
                var dbContext = scopedServices.GetRequiredService<TwitterDbContext>();

                string usernameBenedicto = "Benedicto";
                managerCreateUser.CreateUserAsync(usernameBenedicto);

                long user = 1;
                long userToFollow = 222222;
                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.FollowUserAsync(user, userToFollow));
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
                var dbContext = scopedServices.GetRequiredService<TwitterDbContext>();

                string usernameBenedicto = "Benedicto";
                managerCreateUser.CreateUserAsync(usernameBenedicto);

                long user = 1;
                long userToFollow = 1;
                manager.FollowUserAsync(user, userToFollow);

                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.FollowUserAsync(user, userToFollow));
                Assert.AreEqual("El usuario actual no puede seguirse a si mismo", exception.Message);
            }
        }
    }
}
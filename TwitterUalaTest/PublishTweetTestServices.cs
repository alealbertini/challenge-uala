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
    public class PublishTweetTestServices
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
        public void PublishTweetAsync_InsertOK()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<IPublishTweetService>();
                var managerCreateUser = scopedServices.GetRequiredService<ICreateUserService>();
                var managerFollowUser = scopedServices.GetRequiredService<IFollowUserService>();
                var managerTimeline = scopedServices.GetRequiredService<ITimelineService>();

                string usernameBenedicto = "Benedicto";
                managerCreateUser.CreateUserAsync(usernameBenedicto);

                string usernameAmanda = "Amanda";
                managerCreateUser.CreateUserAsync(usernameAmanda);

                long user = 1;
                long userToFollow = 2;
                managerFollowUser.FollowUserAsync(user, userToFollow);

                string message = "First Tweet";
                manager.PublishTweetAsync(userToFollow, message);

                var tweetsFromUser = managerTimeline.TimelineByUserIdAsync(user).GetAwaiter().GetResult();
                Assert.IsNotNull(tweetsFromUser);
            }
        }

        [TestMethod]
        public async Task PublishTweetAsync_ThrowsInvalidDataException_WhenUserDoesNotExist()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<IPublishTweetService>();

                long user = 1111111;
                string message = "First Tweet";
                
                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.PublishTweetAsync(user, message));
                Assert.AreEqual("El usuario actual no es valido", exception.Message);
            }
        }

        [TestMethod]
        public async Task PublishTweetAsync_ThrowsInvalidDataException_WhenMessageIsEmpty()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<IPublishTweetService>();
                var managerCreateUser = scopedServices.GetRequiredService<ICreateUserService>();

                string usernameNicolas = "Nicolas";
                await managerCreateUser.CreateUserAsync(usernameNicolas);

                long user = 1;
                string message = "";

                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.PublishTweetAsync(user, message));
                Assert.AreEqual("El mensaje no puede ser vacío", exception.Message);
            }
        }

        [TestMethod]
        public async Task PublishTweetAsync_ThrowsInvalidDataException_WhenMessageLengthIsMoreThan280()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<IPublishTweetService>();
                var managerCreateUser = scopedServices.GetRequiredService<ICreateUserService>();

                string usernameRicardo = "Ricardo";
                await managerCreateUser.CreateUserAsync(usernameRicardo);

                long user = 1;
                string message = new string('a', 281);

                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.PublishTweetAsync(user, message));
                Assert.AreEqual("El mensaje no puede ser mayor a 280 caracteres", exception.Message);
            }
        }
    }
}
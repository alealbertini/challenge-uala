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
    public class TimelineTestServices
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
            //services.AddScoped<DbContext, DbInMemoryContext>();
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
        public async Task TimelineAsync_ThrowsInvalidDataException_WhenUserDoesNotExist()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<ITimelineService>();
                var dbContext = scopedServices.GetRequiredService<TwitterDbContext>();

                long user = 1;
                await manager.TimelineByUserIdAsync(user);

                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.TimelineByUserIdAsync(user));
                Assert.AreEqual("El usuario actual no es válido", exception.Message);
            }
        }

        [TestMethod]
        public void TimelineAsync_InsertOK()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var managerPublishTweet = scopedServices.GetRequiredService<IPublishTweetService>();
                var managerCreateUser = scopedServices.GetRequiredService<ICreateUserService>();
                var managerFollowUser = scopedServices.GetRequiredService<IFollowUserService>();
                var manager = scopedServices.GetRequiredService<ITimelineService>();
                var dbContext = scopedServices.GetRequiredService<TwitterDbContext>();

                string usernameBenedicto = "Benedicto";
                managerCreateUser.CreateUserAsync(usernameBenedicto);

                string usernameAmanda = "Amanda";
                managerCreateUser.CreateUserAsync(usernameAmanda);

                long user = 1;
                long userToFollow = 2;
                managerFollowUser.FollowUserAsync(user, userToFollow);

                string message = "First Tweet";
                managerPublishTweet.PublishTweetAsync(userToFollow, message);

                // Assert
                var tweetsFromUserToFollow = manager.TimelineByUserIdAsync(user).GetAwaiter().GetResult();
                Assert.IsNotNull(tweetsFromUserToFollow);
                Assert.AreEqual(tweetsFromUserToFollow.Count, 1);
                Assert.AreEqual(tweetsFromUserToFollow.First().UserId, userToFollow);
                Assert.AreEqual(tweetsFromUserToFollow.First().TweetMessage, message);

                string secondMessage = "Second Tweet";
                managerPublishTweet.PublishTweetAsync(userToFollow, secondMessage);

                tweetsFromUserToFollow = manager.TimelineByUserIdAsync(user).GetAwaiter().GetResult();
                Assert.AreEqual(tweetsFromUserToFollow.Count, 2);
                var existSecondMessage = tweetsFromUserToFollow.FirstOrDefault(t => t.UserId == userToFollow && t.TweetMessage == secondMessage);
                Assert.IsNotNull(existSecondMessage);

                string messageUser1 = "First Tweet from user 1";
                managerPublishTweet.PublishTweetAsync(user, messageUser1);

                var tweetsFromUser = manager.TimelineByUserIdAsync(user).GetAwaiter().GetResult();
                tweetsFromUserToFollow = manager.TimelineByUserIdAsync(userToFollow).GetAwaiter().GetResult();
                Assert.AreEqual(tweetsFromUser.Count, 2);
                Assert.AreEqual(tweetsFromUserToFollow.Count, 0);

                managerFollowUser.FollowUserAsync(userToFollow, user);
                tweetsFromUserToFollow = manager.TimelineByUserIdAsync(userToFollow).GetAwaiter().GetResult();
                Assert.AreEqual(tweetsFromUserToFollow.Count, 1);
            }
        }
    }
}
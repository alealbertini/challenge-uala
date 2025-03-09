using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TwitterUala.Application.Contracts.Applicaction;
using TwitterUala.Application.Contracts.Infrastructure;
using TwitterUala.Application.UseCases;
using TwitterUala.Infrastructure;
using TwitterUala.Infrastructure.Database;
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
        public async Task TimelineAsync_ThrowsInvalidDataException_WhenUserDoesNotExist()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<ITimelineService>();
                var dbContext = scopedServices.GetRequiredService<TwitterDbContext>();

                long user = 111111;
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

                string usernameBenedicto = "Ernesto";
                managerCreateUser.CreateUserAsync(usernameBenedicto);

                string usernameAmanda = "Olivia";
                managerCreateUser.CreateUserAsync(usernameAmanda);

                long user = 3;
                long userToFollow = 4;
                managerFollowUser.FollowUserAsync(user, userToFollow);

                string message = "First Tweet";
                managerPublishTweet.PublishTweetAsync(userToFollow, message).GetAwaiter().GetResult();

                // Assert
                var tweetsFromUser = manager.TimelineByUserIdAsync(user).GetAwaiter().GetResult();
                Assert.AreEqual(1, tweetsFromUser.Count);
                Assert.AreEqual(tweetsFromUser.First().UserId, userToFollow);
                Assert.AreEqual(tweetsFromUser.First().TweetMessage, message);

                string secondMessage = "Second Tweet";
                managerPublishTweet.PublishTweetAsync(userToFollow, secondMessage);

                tweetsFromUser = manager.TimelineByUserIdAsync(user).GetAwaiter().GetResult();
                Assert.AreEqual(tweetsFromUser.Count, 2);
                var existSecondMessage = tweetsFromUser.FirstOrDefault(t => t.UserId == userToFollow && t.TweetMessage == secondMessage);
                Assert.IsNotNull(existSecondMessage);

                string messageUser1 = "First Tweet from user 1";
                managerPublishTweet.PublishTweetAsync(user, messageUser1);

                tweetsFromUser = manager.TimelineByUserIdAsync(user).GetAwaiter().GetResult();
                var tweetsFromUserToFollow = manager.TimelineByUserIdAsync(userToFollow).GetAwaiter().GetResult();
                Assert.AreEqual(tweetsFromUser.Count, 2);
                Assert.AreEqual(tweetsFromUserToFollow.Count, 0);

                managerFollowUser.FollowUserAsync(userToFollow, user);
                tweetsFromUserToFollow = manager.TimelineByUserIdAsync(userToFollow).GetAwaiter().GetResult();
                Assert.AreEqual(tweetsFromUserToFollow.Count, 1);
            }
        }
    }
}
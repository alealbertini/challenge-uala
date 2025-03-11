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

                long user = 111111;
                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.TimelineByUserIdAsync(user));
                Assert.AreEqual("El usuario actual no es válido", exception.Message);
            }
        }

        [TestMethod]
        public async Task TimelineAsync_InsertOKAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var managerPublishTweet = scopedServices.GetRequiredService<IPublishTweetService>();
                var managerCreateUser = scopedServices.GetRequiredService<ICreateUserService>();
                var managerFollowUser = scopedServices.GetRequiredService<IFollowUserService>();
                var manager = scopedServices.GetRequiredService<ITimelineService>();

                UserInDto userInDto = new UserInDto();
                userInDto.Username = "Ernesto";
                managerCreateUser.CreateUserAsync(userInDto);

                UserInDto userInDtoOlivia = new UserInDto();
                userInDtoOlivia.Username = "Olivia";
                managerCreateUser.CreateUserAsync(userInDtoOlivia);

                FollowingDto followingDto = new FollowingDto();
                followingDto.UserId = 1;
                followingDto.UserToFollowId = 2;
                managerFollowUser.FollowUserAsync(followingDto);

                TweetInDto tweetInDto = new TweetInDto();
                tweetInDto.UserId = 2;
                tweetInDto.TweetMessage = "First Tweet";
                await managerPublishTweet.PublishTweetAsync(tweetInDto);

                var tweetsFromUser = await manager.TimelineByUserIdAsync(followingDto.UserId);
                Assert.AreEqual(1, tweetsFromUser.Count);
                Assert.AreEqual(tweetsFromUser[0].UserId, tweetInDto.UserId);
                Assert.AreEqual(tweetsFromUser[0].TweetMessage, tweetInDto.TweetMessage);

                TweetInDto tweetInDto2 = new TweetInDto();
                tweetInDto2.UserId = 2;
                tweetInDto2.TweetMessage = "Second Tweet";
                managerPublishTweet.PublishTweetAsync(tweetInDto2);

                tweetsFromUser = await manager.TimelineByUserIdAsync(followingDto.UserId);
                Assert.AreEqual(2, tweetsFromUser.Count);
                var existSecondMessage = tweetsFromUser.FirstOrDefault(t => t.UserId == tweetInDto2.UserId && t.TweetMessage == tweetInDto2.TweetMessage);
                Assert.IsNotNull(existSecondMessage);

                TweetInDto tweetInDto3 = new TweetInDto();
                tweetInDto3.UserId = 2;
                tweetInDto3.TweetMessage = "Third Tweet from user 2";
                managerPublishTweet.PublishTweetAsync(tweetInDto3);

                tweetsFromUser = await manager.TimelineByUserIdAsync(followingDto.UserId);
                var tweetsFromUserToFollow = await manager.TimelineByUserIdAsync(followingDto.UserToFollowId);
                Assert.AreEqual(3, tweetsFromUser.Count);
                Assert.AreEqual(0, tweetsFromUserToFollow.Count);

                FollowingDto followingDto1 = new FollowingDto();
                followingDto1.UserId = 2;
                followingDto1.UserToFollowId = 1;
                managerFollowUser.FollowUserAsync(followingDto1);

                TweetInDto tweetInDto4 = new TweetInDto();
                tweetInDto4.UserId = 1;
                tweetInDto4.TweetMessage = "First Tweet from user 1";
                managerPublishTweet.PublishTweetAsync(tweetInDto4);

                tweetsFromUserToFollow = await manager.TimelineByUserIdAsync(followingDto.UserToFollowId);
                Assert.AreEqual(1, tweetsFromUserToFollow.Count);
            }
        }
    }
}
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
    public class PublishTweetTestServices
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
        public void PublishTweetAsync_InsertOK()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var manager = scopedServices.GetRequiredService<IPublishTweetService>();
                var managerCreateUser = scopedServices.GetRequiredService<ICreateUserService>();
                var managerFollowUser = scopedServices.GetRequiredService<IFollowUserService>();
                var managerTimeline = scopedServices.GetRequiredService<ITimelineService>();

                UserInDto userInDto = new UserInDto();
                userInDto.Username = "Benedicto";
                managerCreateUser.CreateUserAsync(userInDto);

                UserInDto userInDtoAmanda = new UserInDto();
                userInDto.Username = "Amanda";
                managerCreateUser.CreateUserAsync(userInDtoAmanda);

                FollowingDto followingDto = new FollowingDto();
                followingDto.UserId = 1;
                followingDto.UserToFollowId = 2;
                managerFollowUser.FollowUserAsync(followingDto);

                TweetInDto tweetInDto = new TweetInDto();
                tweetInDto.UserId = 2;
                tweetInDto.TweetMessage = "First Tweet";
                manager.PublishTweetAsync(tweetInDto);

                long user = 1;
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

                TweetInDto tweetInDto = new TweetInDto();
                tweetInDto.UserId = 1111111;
                tweetInDto.TweetMessage = "First Tweet";

                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.PublishTweetAsync(tweetInDto));
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

                UserInDto userInDto = new UserInDto();
                userInDto.Username = "Nicolas";
                await managerCreateUser.CreateUserAsync(userInDto);

                TweetInDto tweetInDto = new TweetInDto();
                tweetInDto.UserId = 1;
                tweetInDto.TweetMessage = "";

                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.PublishTweetAsync(tweetInDto));
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

                UserInDto userInDto = new UserInDto();
                userInDto.Username = "Ricardo";
                await managerCreateUser.CreateUserAsync(userInDto);

                TweetInDto tweetInDto = new TweetInDto();
                tweetInDto.UserId = 1;
                tweetInDto.TweetMessage = new string('a', 281);

                var exception = await Assert.ThrowsExceptionAsync<InvalidDataException>(() => manager.PublishTweetAsync(tweetInDto));
                Assert.AreEqual("El mensaje no puede ser mayor a 280 caracteres", exception.Message);
            }
        }
    }
}
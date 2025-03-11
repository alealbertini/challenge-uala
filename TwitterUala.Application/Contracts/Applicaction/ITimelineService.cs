using TwitterUala.Application.Dtos.Out;
using TwitterUala.Domain.Entities;

namespace TwitterUala.Application.Contracts.Applicaction
{
    public interface ITimelineService
    {
        Task<List<TweetOutDto>> TimelineByUserIdAsync(long userId);
    }
}
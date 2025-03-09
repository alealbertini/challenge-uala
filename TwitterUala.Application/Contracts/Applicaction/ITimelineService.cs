using TwitterUala.Domain.Entities;

namespace TwitterUala.Application.Contracts.Applicaction
{
    public interface ITimelineService
    {
        Task<List<Tweet>> TimelineByUserIdAsync(long userId);
    }
}
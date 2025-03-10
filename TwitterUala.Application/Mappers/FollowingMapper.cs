using TwitterUala.Application.Dtos;
using TwitterUala.Domain.Entities;

namespace TwitterUala.Application.Mappers
{
    public static class FollowingMapper
    {
        public static FollowingDto ToDto(Following following)
        {
            FollowingDto followingDto = new FollowingDto
            {
                UserId = following.UserId,
                UserToFollowId = following.UserId
            };
            return followingDto;
        }
    }
}

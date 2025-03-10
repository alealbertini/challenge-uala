namespace TwitterUala.Application.Dtos
{
    public class TweetDto
    {
        public long UserId { get; set; }
        public string TweetMessage { get; set; }
        public DateTime TweetPosted { get; set; }
    }
}

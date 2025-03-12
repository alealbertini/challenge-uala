namespace TwitterUala.Domain.Entities
{
    public partial class Tweet
    {
        public long IdTweet { get; set; }
        public long UserTweet { get; set; }
        public string TweetMessage { get; set; }
        public DateTime TweetPosted { get; set; }
    }
}

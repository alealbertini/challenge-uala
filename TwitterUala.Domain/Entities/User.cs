namespace TwitterUala.Domain.Entities
{
    public partial class User
    {
        public long IdUser { get; set; }
        public string Username { get; set; }
        public virtual ICollection<Following> Followings { get; set; } = new List<Following>();
    }
}

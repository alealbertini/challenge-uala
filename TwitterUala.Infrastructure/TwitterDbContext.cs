using Microsoft.EntityFrameworkCore;
using TwitterUala.Domain.Entities;

namespace TwitterUala.Infrastructure
{
    public class TwitterDbContext : DbContext
    {
        public TwitterDbContext(DbContextOptions<TwitterDbContext> options)
        : base(options)
        {
        }

        public virtual DbSet<Following> Following { get; set; }
        public virtual DbSet<Tweet> Tweet { get; set; }
        public virtual DbSet<User> User { get; set; }


        public static string ToUnderscoreLowerCase(string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var tables = new List<Type> { typeof(Tweet), typeof(Following), typeof(User) };

            foreach (var table in tables)
            {
                foreach (var column in table.GetProperties())
                {
                    modelBuilder.Entity(table).Property(column.Name).HasColumnName(ToUnderscoreLowerCase(column.Name));
                }
                modelBuilder.Entity(table).ToTable(ToUnderscoreLowerCase(table.Name));
            }

            modelBuilder.Entity<User>().Ignore(f => f.Followings);

            modelBuilder.Entity<Following>(entity =>
            {
                entity.Ignore(f => f.User);

                entity.HasKey(f => new { f.UserId, f.UserToFollowId });

                entity
                    .HasOne(f => f.User)
                    .WithMany(u => u.Followings)
                    .HasForeignKey(f => f.UserId);
            });

            modelBuilder.Entity<Tweet>(entity =>
            {
                entity.HasKey(e => e.IdTweet);

                entity.HasIndex(t => t.UserTweet, "IDX_Tweet_UserToFollowId");

                entity.Property(e => e.IdTweet)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.TweetMessage)
                    .HasMaxLength(280);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.IdUser);

                entity.Property(e => e.IdUser)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Username)
                    .HasMaxLength(50);
            });
        }
    }
}
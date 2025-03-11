using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TwitterUala.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "following",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    user_to_follow_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_following", x => new { x.user_id, x.user_to_follow_id });
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id_user = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id_user);
                });

            migrationBuilder.CreateTable(
                name: "tweet",
                columns: table => new
                {
                    id_tweet = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    tweet_message = table.Column<string>(type: "character varying(280)", maxLength: 280, nullable: false),
                    tweet_posted = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FollowingUserId = table.Column<long>(type: "bigint", nullable: true),
                    FollowingUserToFollowId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tweet", x => x.id_tweet);
                    table.ForeignKey(
                        name: "FK_tweet_following_FollowingUserId_FollowingUserToFollowId",
                        columns: x => new { x.FollowingUserId, x.FollowingUserToFollowId },
                        principalTable: "following",
                        principalColumns: new[] { "user_id", "user_to_follow_id" });
                });

            migrationBuilder.CreateIndex(
                name: "IDX_Following_UserId_UsersToFollowId",
                table: "following",
                columns: new[] { "user_id", "user_to_follow_id" });

            migrationBuilder.CreateIndex(
                name: "IDX_Following_UsersToFollowId",
                table: "following",
                column: "user_to_follow_id");

            migrationBuilder.CreateIndex(
                name: "IDX_Tweet_UserId",
                table: "tweet",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tweet_FollowingUserId_FollowingUserToFollowId",
                table: "tweet",
                columns: new[] { "FollowingUserId", "FollowingUserToFollowId" });

            migrationBuilder.CreateIndex(
                name: "IDX_User_IdUser",
                table: "user",
                column: "id_user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tweet");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "following");
        }
    }
}

using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets for related entities
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<GameListing> GameListings { get; set; }
        public DbSet<UserRating> UserRatings { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<GenreGame> GenreGames { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // UserPreference: muitos-para-muitos entre usuário e gênero
            builder.Entity<UserPreference>()
                .HasKey(up => new { up.UserId, up.GenreId });

            builder.Entity<UserPreference>()
                .HasOne(up => up.User)
                .WithMany(u => u.Preferences)
                .HasForeignKey(up => up.UserId);

            builder.Entity<UserPreference>()
                .HasOne(up => up.Genre)
                .WithMany(g => g.UserPreferences)
                .HasForeignKey(up => up.GenreId);

            // GameListing: relação com usuário dono
            builder.Entity<GameListing>()
                .HasOne(gl => gl.OwnerUser)
                .WithMany(u => u.GameListings)
                .HasForeignKey(gl => gl.OwnerUserId);

            // UserRating: relação com quem avaliou e quem foi avaliado
            builder.Entity<UserRating>()
                .HasOne(ur => ur.RaterUser)
                .WithMany(u => u.RatingsGiven)
                .HasForeignKey(ur => ur.RaterUserId);

            builder.Entity<UserRating>()
                .HasOne(ur => ur.TargetUser)
                .WithMany(u => u.RatingsReceived)
                .HasForeignKey(ur => ur.TargetUserId);

            // GenreGame: muitos-para-muitos entre jogo e gênero
            builder.Entity<GenreGame>()
                .HasKey(gg => new { gg.GameId, gg.GenreId });

            builder.Entity<GenreGame>()
                .HasOne(gg => gg.Game)
                .WithMany(g => g.GenreGames)
                .HasForeignKey(gg => gg.GameId);

            builder.Entity<GenreGame>()
                .HasOne(gg => gg.Genre)
                .WithMany(g => g.GenreGames)
                .HasForeignKey(gg => gg.GenreId);

            // GameListing: relação com o jogo
            builder.Entity<GameListing>()
                .HasOne(gl => gl.Game)
                .WithMany(g => g.GameListings)
                .HasForeignKey(gl => gl.GameId);
        }
    }
}

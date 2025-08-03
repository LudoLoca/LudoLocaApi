using Microsoft.EntityFrameworkCore;
using Backend.Domain;

namespace Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    // Conjuntos de entidades (tabelas do banco)
    public DbSet<Game> Games { get; set; }
    public DbSet<GameListing> GameListings { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<UserRating> UserRatings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Garante que um usuário só possa avaliar outro uma única vez por aluguel
        modelBuilder.Entity<UserRating>()
            .HasIndex(r => new { r.RentalId, r.RaterUserId })
            .IsUnique();

        // Relacionamento: quem avaliou (Rater) → várias avaliações dadas
        modelBuilder.Entity<UserRating>()
            .HasOne(r => r.Rater)
            .WithMany(u => u.RatingsGiven)
            .HasForeignKey(r => r.RaterUserId)
            .OnDelete(DeleteBehavior.Restrict); // impede apagar usuário se houver avaliações

        // Relacionamento: quem foi avaliado (Target) → várias avaliações recebidas
        modelBuilder.Entity<UserRating>()
            .HasOne(r => r.Target)
            .WithMany(u => u.RatingsReceived)
            .HasForeignKey(r => r.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento opcional: avaliação vinculada a um aluguel (pode ser null)
        modelBuilder.Entity<UserRating>()
            .HasOne(r => r.Rental)
            .WithMany()
            .HasForeignKey(r => r.RentalId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relacionamento: aluguel pertence a um anúncio específico
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.GameListing)
            .WithMany(gl => gl.Rentals)
            .HasForeignKey(r => r.GameListingId)
            .OnDelete(DeleteBehavior.Cascade); // se o anúncio for apagado, os aluguéis também

        // Relacionamento: aluguel feito por um usuário (locatário)
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Renter)
            .WithMany(u => u.Rentals)
            .HasForeignKey(r => r.RenterUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento: anúncio está vinculado a um jogo do catálogo
        modelBuilder.Entity<GameListing>()
            .HasOne(gl => gl.Game)
            .WithMany(g => g.Listings)
            .HasForeignKey(gl => gl.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento: anúncio pertence a um usuário (dono da cópia)
        modelBuilder.Entity<GameListing>()
            .HasOne(gl => gl.Owner)
            .WithMany(u => u.Listings)
            .HasForeignKey(gl => gl.OwnerUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Domain;

public class Rental
{
    [Key]
    public Guid Id { get; set; }

    // FK pra GameListing (o item sendo alugado)
    [Required]
    public Guid GameListingId { get; set; }

    public GameListing GameListing { get; set; } = null!;

    // FK pra User
    [Required]
    public Guid RenterUserId { get; set; }

    public User Renter { get; set; } = null!;

    // EF Core guarda como int por padrao
    public enum RentalStatus
    {
        [Display(Name = "Ativo")]
        Active,

        [Display(Name = "Devolvido")]
        Returned,

        [Display(Name = "Cancelado")]
        Cancelled,

        [Display(Name = "Atrasado")]
        Overdue
    }

    [Required]
    public DateTime RentalStart { get; set; }

    public DateTime? RentalEnd { get; set; }           // Data de retorno esperada
    public DateTime? ReturnedAt { get; set; }          // Data de retorno efetiva (null até efetivação)

    [Range(0, 10000)]
    public decimal DailyRate { get; set; }

    [Range(0, 365)]
    public int DaysRented { get; set; }

    [MaxLength(500)]
    public string? CancellationReason { get; set; }

    [Range(0, 1000)]
    public decimal? OverdueFee { get; set; }

    public bool IsRatedByOwner { get; set; } = false;
    public bool IsRatedByRenter { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Derivadas - não mapeadas
    [NotMapped]
    public decimal TotalCost => DailyRate * DaysRented + (OverdueFee ?? 0);
}

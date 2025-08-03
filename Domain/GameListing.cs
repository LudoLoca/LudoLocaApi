using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // pra [NotMapped]

namespace Backend.Domain;

public class GameListing
{
    [Key]
    public Guid Id { get; set; }

    // FK e navegação pra Game
    [Required]
    public Guid GameId { get; set; }

    public Game Game { get; set; } = null!;
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>(); // todas as transacoes pra essa entrada

    // FK e navegação pra User
    [Required]
    public Guid OwnerUserId { get; set; }

    public User Owner { get; set; } = null!;

    [MaxLength(500)]
    public string? ConditionNotes { get; set; }

    [Range(0, 1000)]
    public decimal PricePerDay { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Não mapeado para o DB, propriedade da classe
    [NotMapped]
    public bool IsRentable => IsAvailable && PricePerDay > 0;
}

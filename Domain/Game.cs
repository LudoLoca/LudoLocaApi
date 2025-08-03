using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // pra [NotMapped]

namespace Backend.Domain;

// A ENTRADA DE CATALOGO. não a entrada para aluguel
// representa o jogo em abstrato. não cadastro individual de disponibilidade
public class Game
{
    [Key]
    public Guid Id { get; set; }  // Populado pelo EF

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Publisher { get; set; }

    [Range(1900, 2100)]
    public int YearPublished { get; set; }

    [Range(1, 100)]
    public int MinPlayers { get; set; }

    [Range(1, 100)]
    public int MaxPlayers { get; set; }

    [Range(1, 600)]
    public int PlayTimeMinutes { get; set; }

    [MaxLength(100)]
    public string? Genre { get; set; }

    [MaxLength(100)]
    public string? Designer { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navegação: todas as listagens de usuários pra esse jogo
    public ICollection<GameListing> Listings { get; set; } = new List<GameListing>();
}
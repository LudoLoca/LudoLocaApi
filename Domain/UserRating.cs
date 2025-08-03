using System.ComponentModel.DataAnnotations;

namespace Backend.Domain;

public class UserRating
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid RaterUserId { get; set; }
    public User Rater { get; set; } = null!;

    [Required]
    public Guid TargetUserId { get; set; }
    public User Target { get; set; } = null!;

    [Required]
    public bool IsPositive { get; set; }  // V = thumbs up, F = thumbs down

    [MaxLength(500)]
    public string? ContextNote { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Opcional FK pra Rental — atrelar rating a transação específica
    public Guid? RentalId { get; set; }

    public Rental? Rental { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // pra [NotMapped]


namespace Backend.Domain;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Email { get; set; }

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    // Navegacao
    public ICollection<GameListing> Listings { get; set; } = new List<GameListing>();
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>(); // como locatário
    public ICollection<UserRating> RatingsReceived { get; set; } = new List<UserRating>();
    public ICollection<UserRating> RatingsGiven { get; set; } = new List<UserRating>();

    // Propriedades derivadas, não mapeadas pro DB
    [NotMapped]
    public int ListingsCount => Listings.Count;

    [NotMapped]
    public int RentalsCount => Rentals.Count;

    [NotMapped]
    public int RatingsCount => RatingsReceived.Count;

    [NotMapped]
    public int ThumbsUpCount => RatingsReceived.Count(r => r.IsPositive);

    [NotMapped]
    public int ThumbsDownCount => RatingsReceived.Count(r => !r.IsPositive);

    [NotMapped]
    public double ReputationScore => RatingsCount == 0 
        ? 0 
        : Math.Round((double)ThumbsUpCount / RatingsCount * 5, 2); // normalized to 5 stars
}

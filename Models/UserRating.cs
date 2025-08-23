using System;

namespace API.Models
{
    public class UserRating
    {
        public Guid Id { get; set; }
        public Guid GameListingId { get; set; }
        public Guid RaterUserId { get; set; }
        public Guid TargetUserId { get; set; }
        public bool IsPositive { get; set; }
        public string? ContextNote { get; set; }
        public DateTime CreatedAt { get; set; }

        
        public AppUser RaterUser { get; set; } = null!;
        public AppUser TargetUser { get; set; } = null!;
        public GameListing GameListing { get; set; } = null!;
    }
}
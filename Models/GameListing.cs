using System;

namespace API.Models
{
    public class GameListing
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public Guid OwnerUserId { get; set; }
        public string? ConditionNotes { get; set; }
        public string PricePerDay { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        
        public AppUser OwnerUser { get; set; } = null!;
        public Game Game { get; set; } = null!;
    }
}
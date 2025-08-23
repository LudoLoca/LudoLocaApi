using System;
using System.Collections.Generic;

namespace API.Models
{
    public class Game
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int YearPublished { get; set; }
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public int PlayTimeMinutes { get; set; }
        public string? Designer { get; set; }
        public DateTime CreatedAt { get; set; }

        
        public ICollection<GenreGame> GenreGames { get; set; } = new List<GenreGame>();
        public ICollection<GameListing> GameListings { get; set; } = new List<GameListing>();
    }
}
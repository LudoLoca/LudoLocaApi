using System;
using System.Collections.Generic;

namespace API.Models
{
    public class Genre
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        
        public ICollection<GenreGame> GenreGames { get; set; } = new List<GenreGame>();
        public ICollection<UserPreference> UserPreferences { get; set; } = new List<UserPreference>();
    }
}
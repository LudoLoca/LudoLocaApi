using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace API.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        
        public string Username { get; set; } = string.Empty; 
        public DateTime RegisteredAt { get; set; } 
        public bool IsActive { get; set; } = true;

        
        public virtual ICollection<UserPreference> Preferences { get; set; } = new List<UserPreference>();
        public virtual ICollection<GameListing> GameListings { get; set; } = new List<GameListing>();
        public virtual ICollection<UserRating> RatingsGiven { get; set; } = new List<UserRating>();
        public virtual ICollection<UserRating> RatingsReceived { get; set; } = new List<UserRating>();
    }
}
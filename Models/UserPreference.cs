using System;

namespace API.Models
{
    public class UserPreference
    {
        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;

        public Guid GenreId { get; set; }
        public Genre Genre { get; set; } = null!;
    }
}
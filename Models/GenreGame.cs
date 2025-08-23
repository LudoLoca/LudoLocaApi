using System;

namespace API.Models
{
    public class GenreGame
    {
        public Guid GameId { get; set; }
        public Game Game { get; set; } = null!;

        public Guid GenreId { get; set; }
        public Genre Genre { get; set; } = null!;
    }
}
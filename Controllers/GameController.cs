using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        // TODO: Add DbContext or service injection for game management
        
        // Game creation record
        public record CreateGameRequest(string Title, string Description);
        public record UpdateGameRequest(string? Title, string? Description);

        /// <summary>
        /// Creates a new game
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Title))
                return BadRequest(new { error = "Title is required." });

            // TODO: Implement game creation logic
            var game = new Game
            {
                Id = Guid.NewGuid(),
                Title = req.Title,
                Description = req.Description,
                CreatedAt = DateTime.UtcNow
            };

            // Simulate successful creation
            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
        }

        /// <summary>
        /// Gets all games
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetGames()
        {
            // TODO: Implement games listing logic
            var games = new List<Game>(); // Replace with actual data retrieval
            return Ok(games);
        }

        /// <summary>
        /// Gets a specific game by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(Guid id)
        {
            // TODO: Implement single game retrieval logic
            var game = new Game(); // Replace with actual data retrieval
            
            if (game == null)
                return NotFound(new { error = "Game not found." });

            return Ok(game);
        }

        /// <summary>
        /// Updates a game
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateGame(Guid id, [FromBody] UpdateGameRequest req)
        {
            // TODO: Implement game update logic
            var game = new Game(); // Replace with actual data retrieval

            if (game == null)
                return NotFound(new { error = "Game not found." });

            // Update only provided fields
            if (req.Title != null) game.Title = req.Title;
            if (req.Description != null) game.Description = req.Description;

            return Ok(game);
        }

        /// <summary>
        /// Deletes a game
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(Guid id)
        {
            // TODO: Implement game deletion logic
            var game = new Game(); // Replace with actual data retrieval

            if (game == null)
                return NotFound(new { error = "Game not found." });

            // Delete the game
            return NoContent();
        }
    }

    // Temporary Game model (should be moved to Models folder)
    public class Game
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

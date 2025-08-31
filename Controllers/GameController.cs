using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using API.Models;
using API.Data;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public GameController(AppDbContext context)
        {
            _dbContext = context;
        }

        // Contratos de requisição para criação e atualização de jogos
        public record CreateGameRequest(
            string Title, 
            string Description, 
            string Publisher, 
            int YearPublished, 
            int MinPlayers, 
            int MaxPlayers, 
            int PlayTimeMinutes,
            string? Designer
        );

        public record UpdateGameRequest(
            string? Title, 
            string? Description, 
            string? Publisher, 
            int? YearPublished, 
            int? MinPlayers, 
            int? MaxPlayers, 
            int? PlayTimeMinutes,
            string? Designer
        );

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Title))
                return BadRequest(new { error = "Title is required." });

            var game = new Game
            {
                Id = Guid.NewGuid(),
                Title = req.Title,
                Description = req.Description,
                Publisher = req.Publisher,
                YearPublished = req.YearPublished,
                MinPlayers = req.MinPlayers,
                MaxPlayers = req.MaxPlayers,
                PlayTimeMinutes = req.PlayTimeMinutes,
                Designer = req.Designer,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Games.Add(game);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
        }

        [HttpGet]
        public async Task<IActionResult> GetGames()
        {
            var games = await _dbContext.Games
                .Include(g => g.GenreGames)
                    .ThenInclude(gg => gg.Genre)
                .Select(g => new
                {
                    g.Id,
                    g.Title,
                    g.Description,
                    g.Publisher,
                    g.YearPublished,
                    g.MinPlayers,
                    g.MaxPlayers,
                    g.PlayTimeMinutes,
                    g.Designer,
                    g.CreatedAt,
                    Genres = g.GenreGames.Select(gg => new
                    {
                        gg.GenreId,
                        gg.Genre.Name
                    })
                })
                .ToListAsync();
            return Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(Guid id)
        {
            var game = await _dbContext.Games
                .Include(g => g.GenreGames)
                    .ThenInclude(gg => gg.Genre)
                .Where(g => g.Id == id)
                .Select(g => new
                {
                    g.Id,
                    g.Title,
                    g.Description,
                    g.Publisher,
                    g.YearPublished,
                    g.MinPlayers,
                    g.MaxPlayers,
                    g.PlayTimeMinutes,
                    g.Designer,
                    g.CreatedAt,
                    Genres = g.GenreGames.Select(gg => new
                    {
                        gg.GenreId,
                        gg.Genre.Name
                    })
                })
                .FirstOrDefaultAsync();

            if (game == null)
                return NotFound(new { error = "Game not found." });

            return Ok(game);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateGame(Guid id, [FromBody] UpdateGameRequest req)
        {
            var game = await _dbContext.Games.FindAsync(id);

            if (game == null)
                return NotFound(new { error = "Game not found." });

            // Atualiza apenas os campos fornecidos na requisição
            if (req.Title != null) game.Title = req.Title;
            if (req.Description != null) game.Description = req.Description;
            if (req.Publisher != null) game.Publisher = req.Publisher;
            if (req.YearPublished.HasValue) game.YearPublished = req.YearPublished.Value;
            if (req.MinPlayers.HasValue) game.MinPlayers = req.MinPlayers.Value;
            if (req.MaxPlayers.HasValue) game.MaxPlayers = req.MaxPlayers.Value;
            if (req.PlayTimeMinutes.HasValue) game.PlayTimeMinutes = req.PlayTimeMinutes.Value;
            if (req.Designer != null) game.Designer = req.Designer;

            await _dbContext.SaveChangesAsync();

            return Ok(game);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(Guid id)
        {
            var game = await _dbContext.Games.FindAsync(id);

            if (game == null)
                return NotFound(new { error = "Game not found." });

            _dbContext.Games.Remove(game);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}

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
    //[Authorize(Roles = "Admin")]

    public class GenreGameController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public GenreGameController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public record CreateGenreGameRequest(
            Guid GameId,
            Guid GenreId
        );

        [HttpPost]
        public async Task<IActionResult> AddGenreToGame([FromBody] CreateGenreGameRequest req)
        {
            if (req.GameId == Guid.Empty || req.GenreId == Guid.Empty)
                return BadRequest(new { error = "GameId and GenreId are required." });

            var gameExists = await _dbContext.Games.AnyAsync(g => g.Id == req.GameId);
            var genreExists = await _dbContext.Genres.AnyAsync(g => g.Id == req.GenreId);

            if (!gameExists || !genreExists)
                return NotFound(new { error = "Game or Genre not found." });

            var exists = await _dbContext.GenreGames
                .AnyAsync(gg => gg.GameId == req.GameId && gg.GenreId == req.GenreId);

            if (exists)
                return Conflict(new { error = "This genre is already associated with the game." });

            var genreGame = new GenreGame
            {
                GameId = req.GameId,
                GenreId = req.GenreId
            };

            _dbContext.GenreGames.Add(genreGame);
            await _dbContext.SaveChangesAsync();

            return Ok(req);
        }

        [HttpGet("{gameId}")]
        public async Task<IActionResult> GetGenresByGameId(Guid gameId)
        {
            // Validação do parâmetro
            if (gameId == Guid.Empty)
                return BadRequest(new { error = "GameId is required." });

            // Verifica se o jogo existe
            var gameExists = await _dbContext.Games.AnyAsync(g => g.Id == gameId);
            if (!gameExists)
                return NotFound(new { error = "Game not found." });

            // Busca os gêneros associados ao jogo específico
            var genreGames = await _dbContext.GenreGames
                .AsNoTracking()
                .Where(gg => gg.GameId == gameId)
                .Include(gg => gg.Game)
                .Include(gg => gg.Genre)
                .Select(gg => new
                {
                    gg.GameId,
                    gg.Game.Title,
                    gg.GenreId,
                    gg.Genre.Name
                })
                .ToListAsync();

            return Ok(genreGames);
        }


        //[HttpGet] // APENAS PARA TESTES
        //public async Task<IActionResult> GetAllGenreGames()
        //{
        //    var genreGames = await _dbContext.GenreGames
        //        .AsNoTracking()
        //        .Include(gg => gg.Game)
        //        .Include(gg => gg.Genre)
        //        .Select(gg => new 
        //        {
        //            gg.GameId,
        //            gg.Game.Title,
        //            gg.GenreId,
        //            gg.Genre.Name
        //        })
        //        .ToListAsync();
        //    return Ok(genreGames);
        //}

        [HttpDelete("genre/{genreId}/game/{gameId}")]
        public async Task<IActionResult> DeleteGenreFromGame(Guid genreId, Guid gameId)
        {
            if (gameId == Guid.Empty || genreId == Guid.Empty)
                return BadRequest(new { error = "GameId and GenreId are required." });

            var genreGame = await _dbContext.GenreGames
                .FirstOrDefaultAsync(gg => gg.GameId == gameId && gg.GenreId == genreId);

            if (genreGame == null)
                return NotFound(new { error = "Association not found." });

            _dbContext.GenreGames.Remove(genreGame);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Association deleted successfully." });
        }
    }
}

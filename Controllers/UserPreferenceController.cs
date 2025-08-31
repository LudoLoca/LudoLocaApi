using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;
using API.Data;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPreferencesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public UserPreferencesController(AppDbContext context)
        {
            _dbContext = context;
        }

        public class CreateUserPreferenceDto
        {
            public Guid UserId { get; set; }
            public Guid GenreId { get; set; }
        }


        [HttpPost]
        public async Task<ActionResult> Create(CreateUserPreferenceDto dto)
        {
            var userPreference = new UserPreference
            {
                UserId = dto.UserId,
                GenreId = dto.GenreId
            };

            _dbContext.UserPreferences.Add(userPreference);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserGenres), new { userId = dto.UserId }, userPreference);
        }

        [HttpGet("user/{userId}/genres")]
        public async Task<ActionResult<IEnumerable<UserPreference>>> GetUserGenres(Guid userId)
        {
            var genres = await _dbContext.UserPreferences
                .Where(up => up.UserId == userId)
                .ToListAsync();

            if (genres == null || !genres.Any())
                return NotFound();

            return Ok(genres);
        }


        [HttpDelete("user/{userId}/genre/{genreId}")]
        public async Task<ActionResult> Delete(Guid userId, Guid genreId)
        {
            var userPreference = await _dbContext.UserPreferences
                .FirstOrDefaultAsync(up => up.UserId == userId && up.GenreId == genreId);

            if (userPreference == null)
                return NotFound();

            _dbContext.UserPreferences.Remove(userPreference);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}

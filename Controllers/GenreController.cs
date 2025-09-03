using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class GenreController : ControllerBase
    {
        private readonly AppDbContext _dB;
        public GenreController(AppDbContext dB)
        {
            _dB = dB;
        }

        public record CreateGenre(string Name);

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGenre genre)
        {
            if (string.IsNullOrEmpty(genre.Name))
                return BadRequest(new { error = "Este campo é obrigatório." });

            var exists = await _dB.Genres.AnyAsync(g => g.Name == genre.Name);
            if (exists)
                return Conflict(new { error = "Já existe um gênero com esse nome." });

            var genero = new Genre
            {
                Id = Guid.NewGuid(),
                Name = genre.Name.Trim(),
            };
            _dB.Genres.Add(genero);
            await _dB.SaveChangesAsync();
            return Ok(genero);
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> ListAll(Guid? id)
        {
            if (id.HasValue)
            {
                var genre = await _dB.Genres
                    .AsNoTracking()
                    .Where(g => g.Id == id.Value)
                    .Select(g => new
                    {
                        g.Id,
                        g.Name
                    })
                    .FirstOrDefaultAsync();

                if (genre == null)
                    return NotFound();

                return Ok(genre);
            }

            var itens = await _dB.Genres
                .AsNoTracking()
                .OrderBy(g => g.Name)
                .Select(g => new
                {
                    g.Id,
                    g.Name
                })
                .ToListAsync();

            return Ok(itens);
        }


        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] CreateGenre genre)
        {
            if (string.IsNullOrWhiteSpace(genre.Name))
                return BadRequest(new { error = "O campo 'Name' é obrigatório." });

            var genero = await _dB.Genres.FindAsync(id);
            if (genero is null)
                return NotFound(new { error = "Gênero não encontrado." });

            var exists = await _dB.Genres.AnyAsync(g => g.Name.ToLower() == genre.Name.ToLower() && g.Id != id);
            if (exists)
                return Conflict(new { error = "Já existe um gênero com esse nome." });

            genero.Name = genre.Name.Trim();
            await _dB.SaveChangesAsync();

            return Ok(genero);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var genero = await _dB.Genres.FindAsync(id);
            if (genero is null)
                return NotFound(new { error = "Gênero não encontrado." });

            _dB.Genres.Remove(genero);
            await _dB.SaveChangesAsync();

            return NoContent();
        }
    }
}
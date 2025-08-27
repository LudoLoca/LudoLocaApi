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
    [Authorize(Roles = "Admin")]

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


        [HttpGet]
        public async Task<IActionResult> ListAll()
        {
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

        [HttpPatch("{name}")]
        public async Task<IActionResult> Patch(string name, [FromBody] CreateGenre genre)
        {
            if (string.IsNullOrWhiteSpace(genre.Name))
                return BadRequest(new { error = "O campo 'Name' é obrigatório." });


            var genero = await _dB.Genres
                .FirstOrDefaultAsync(g => g.Name.ToLower() == name.ToLower());

            if (genero is null)
                return NotFound(new { error = "Gênero não encontrado." });


            var exists = await _dB.Genres.AnyAsync(g => g.Name.ToLower() == genre.Name.ToLower() && g.Id != genero.Id);
            if (exists)
                return Conflict(new { error = "Já existe um gênero com esse nome." });

            genero.Name = genre.Name.Trim();
            await _dB.SaveChangesAsync();

            return Ok(genero);
        }


        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { error = "O nome é obrigatório." });


            var genero = await _dB.Genres
                .FirstOrDefaultAsync(g => g.Name.ToLower() == name.ToLower());

            if (genero is null)
                return NotFound(new { error = "Gênero não encontrado." });

            _dB.Genres.Remove(genero);
            await _dB.SaveChangesAsync();

            return NoContent();
        }



    }




}
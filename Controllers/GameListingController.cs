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
	[Authorize]
	public class GameListingController : ControllerBase
	{
		private readonly AppDbContext _db;
		private readonly UserManager<AppUser> _userManager;

		public GameListingController(AppDbContext db, UserManager<AppUser> userManager)
		{
			_db = db;
			_userManager = userManager;
		}

		public record CreateGameListingRequest(Guid GameId, string? ConditionNotes, string PricePerDay,	bool? IsAvailable);
		public record PatchGameListingRequest(string? PricePerDay, bool? IsAvailable, string? ConditionNotes);

		//Cria��o de jogo ([POST] /GameListing)
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateGameListingRequest req)
		{
			//Valida se o GameId e PricePerDay foram fornecidos
			if (req.GameId == Guid.Empty)
				return BadRequest(new { error = "GameId � obrigat�rio." });

			if (string.IsNullOrWhiteSpace(req.PricePerDay))
				return BadRequest(new { error = "PricePerDay � obrigat�rio." });

			//Verifica se o jogo existe
			var gameExists = await _db.Games.AnyAsync(g => g.Id == req.GameId);
			if (!gameExists)
				return NotFound(new { error = "Jogo n�o encontrado." });

			//Pega o usu�rio autenticado
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return Unauthorized(new { error = "Usu�rio n�o autenticado." });

			//Cria o an�ncio
			var gameListing = new GameListing
			{
				Id = Guid.NewGuid(),
				GameId = req.GameId,
				OwnerUserId = user.Id,
				ConditionNotes = req.ConditionNotes,
				PricePerDay = req.PricePerDay,
				IsAvailable = req.IsAvailable ?? true,
				CreatedAt = DateTime.UtcNow
			};

			_db.GameListings.Add(gameListing);
			await _db.SaveChangesAsync();

			//retorna informa��es b�sicas do an�ncio criado
			return CreatedAtAction(nameof(GetById), new { id = gameListing.Id }, new
			{
                gameListing.Id,
                gameListing.GameId,
                gameListing.OwnerUserId,
                gameListing.PricePerDay,
                gameListing.IsAvailable,
                gameListing.ConditionNotes,
                gameListing.CreatedAt
			});
		}
		
		//Listagem de jogos ([GET] /GameListing)
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ListAll()
		{
			var items = await _db.GameListings
				.AsNoTracking()
				.Include(gl => gl.Game)
				.Include(gl => gl.OwnerUser)
				.OrderByDescending(gl => gl.CreatedAt)
				.Select(gl => new
				{
					gl.Id,
					gl.PricePerDay,
					gl.IsAvailable,
					gl.ConditionNotes,
					gl.CreatedAt,
					Game = new
					{
						gl.Game.Id,
						gl.Game.Title
					},
					OwnerUser = new
					{
						gl.OwnerUser.Id,
						gl.OwnerUser.Email
					}
				})
				.ToListAsync();

			return Ok(items);
		}

		//Listar um jogo pelo ID ([GET] /GameListing/{id})
		[HttpGet("{id:guid}")]
		[AllowAnonymous]
		public async Task<IActionResult> GetById(Guid id)
		{
			var listing = await _db.GameListings
				.AsNoTracking()
				.Include(gl => gl.Game)
				.Include(gl => gl.OwnerUser)
				.FirstOrDefaultAsync(gl => gl.Id == id);

			if (listing == null) 
				return NotFound(new { error = "An�ncio n�o encontrado." });


			return Ok(new
			{
				listing.Id,
				listing.PricePerDay,
				listing.IsAvailable,
				listing.ConditionNotes,
				listing.CreatedAt,
				Game = new { listing.Game.Id, listing.Game.Title },
				Owner = new { listing.OwnerUser.Id, listing.OwnerUser.Email }
			});
		}

		//Editar um jogo ([PATCH] /GameListing/{id})
		[HttpPatch("{id:guid}")]
		[Authorize]
		public async Task<IActionResult> Patch(Guid id, [FromBody] PatchGameListingRequest req)
		{
			if (id == Guid.Empty)
				return BadRequest(new { error = "Id inv�lido." });

			var listing = await _db.GameListings.FindAsync(id);
			if (listing == null)
				return NotFound(new { error = "An�ncio n�o encontrado." });

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return Unauthorized();

			if (listing.OwnerUserId != user.Id)
				return Forbid();

			if (req.PricePerDay is not null)
				listing.PricePerDay = req.PricePerDay;

			if (req.IsAvailable.HasValue)
				listing.IsAvailable = req.IsAvailable.Value;

			if (req.ConditionNotes is not null)
				listing.ConditionNotes = req.ConditionNotes;

			await _db.SaveChangesAsync();

			return Ok(new
			{
				listing.Id,
				listing.GameId,
				listing.OwnerUserId,
				listing.PricePerDay,
				listing.IsAvailable,
				listing.ConditionNotes,
				listing.CreatedAt
			});
		}

		//Deletar um jogo ([DELETE] /GameListing/{id})
		[HttpDelete("{id:guid}")]
		[Authorize]
		public async Task<IActionResult> Delete(Guid id)
		{
			if (id == Guid.Empty)
				return BadRequest(new { error = "Id inv�lido." });

			var listing = await _db.GameListings.FindAsync(id);
			if (listing == null)
				return NotFound(new { error = "An�ncio n�o encontrado." });

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return Unauthorized();

			if (listing.OwnerUserId != user.Id)
				return Forbid();

			_db.GameListings.Remove(listing);
			await _db.SaveChangesAsync();

			return NoContent();
		}

	}
}
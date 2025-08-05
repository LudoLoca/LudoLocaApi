using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Backend.Domain;
using Backend.Helpers;

namespace Backend.Controllers;

[Authorize(Roles = "Admin")] // Requer autenticação com a role "Admin"
[ApiController]
[Route("api/[controller]")]
public class MetaController : ControllerBase
{

    /// Retorna todos os status de aluguel disponíveis com seus nomes localizados.

    [HttpGet("rental-statuses")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Lista de status de aluguel",
        Description = "Retorna os valores do enum RentalStatus com nomes traduzidos (Display).")]
    public IActionResult GetRentalStatuses()
    {
        // var values = Enum.GetValues<RentalStatus>()
        //     .Cast<RentalStatus>()
        //     .Select(status => new
        //     {
        //         Id = (int)status,
        //         Name = status.GetDisplayName()
        //     });

        // return Ok(values);
        return Ok();
    }
}

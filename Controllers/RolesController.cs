using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Apenas administradores podem gerenciar papéis
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RolesController(RoleManager<IdentityRole<Guid>> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // Cria um novo papel (role) se não existir
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("Role name is required.");

            if (await _roleManager.RoleExistsAsync(roleName))
                return Conflict("Role already exists.");

            var result = await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }

        // Atribui um papel (role) a um usuário pelo e-mail
        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest req)
        {
            var user = await _userManager.FindByEmailAsync(req.Email);
            if (user == null)
                return NotFound("User not found.");

            var result = await _userManager.AddToRoleAsync(user, req.Role);
            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }

       
        /// Contrato de requisição para atribuição de papel a um usuário
        public record AssignRoleRequest(string Email, string Role);
    }
}
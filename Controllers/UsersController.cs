using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Apenas administradores podem acessar estes endpoints
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        // Retorna a lista de usuários com status de admin
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<object>();

            foreach (var user in users)
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                userList.Add(new
                {
                    user.Id,
                    user.Email,
                    user.UserName,
                    IsAdmin = isAdmin
                });
            }

            return Ok(userList);
        }

        // Remove um usuário pelo id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded ? NoContent() : BadRequest(result.Errors);
        }

        // Adiciona ou remove o papel de admin de um usuário
        [HttpPost("{id}/set-admin")]
        public async Task<IActionResult> SetAdmin(Guid id, [FromBody] bool isAdmin)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var isInRole = await _userManager.IsInRoleAsync(user, "Admin");
            IdentityResult result;
            if (isAdmin && !isInRole)
                result = await _userManager.AddToRoleAsync(user, "Admin");
            else if (!isAdmin && isInRole)  
                result = await _userManager.RemoveFromRoleAsync(user, "Admin");
            else
                return Ok(); 

            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Ok(new { authenticated = false });

            var user = await _userManager.GetUserAsync(User);
            var roles = user != null ? await _userManager.GetRolesAsync(user) : Array.Empty<string>();

            return Ok(new
            {
                authenticated = true,
                name = User.Identity!.Name,
                roles = roles,
                claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }
    }
}

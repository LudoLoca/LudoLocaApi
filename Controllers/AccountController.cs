using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Contratos de requisição para registro e login
        public record RegisterRequest(string Email, string Password);
        public record LoginRequest(string Email, string Password, bool RememberMe = false);

        /// <summary>
        /// Registra um novo usuário local. Não há confirmação de e-mail ou atribuição de papéis.
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            // Valida se o e-mail e a senha foram fornecidos
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { error = "Email and Password are required." });

            // Verifica se já existe um usuário com o e-mail informado
            var existing = await _userManager.FindByEmailAsync(req.Email);
            if (existing != null)
                return Conflict(new { error = "Email already registered." });

            // Cria uma nova instância de usuário
            var user = new AppUser
            {
                UserName = req.Email,
                Email = req.Email,
                EmailConfirmed = true,
                RegisteredAt = DateTime.UtcNow
            };

            // Tenta criar o usuário no banco de dados
            var result = await _userManager.CreateAsync(user, req.Password);
            if (!result.Succeeded)
                return BadRequest(new
                {
                    error = "Registration failed.",
                    details = result.Errors.Select(e => e.Description)
                });

            // Realiza login automático após o registro (cookie de autenticação da API)
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Retorna os dados básicos do usuário criado
            return CreatedAtAction(nameof(Me), new { }, new { user.Id, user.Email });
        }

        /// <summary>
        /// Realiza o login do usuário utilizando Identity. Retorna informações básicas e papéis do usuário.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            // Valida se o e-mail e a senha foram fornecidos
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { error = "Email and Password are required." });

            // Busca o usuário pelo e-mail
            var user = await _userManager.FindByEmailAsync(req.Email);
            if (user == null)
                return Unauthorized(new { error = "Invalid credentials." });

            // Realiza o login utilizando o SignInManager, respeitando políticas de bloqueio
            var result = await _signInManager.PasswordSignInAsync(
                userName: user.UserName!,
                password: req.Password,
                isPersistent: req.RememberMe,
                lockoutOnFailure: true);

            if (!result.Succeeded)
                return Unauthorized(new { error = "Invalid credentials." });

            // Obtém os papéis do usuário autenticado
            var roles = await _userManager.GetRolesAsync(user);

            // Retorna informações de sucesso, incluindo papéis
            return Ok(new
            {
                success = true,
                userId = user.Id,
                email = user.Email ?? user.UserName,
                roles = roles.ToArray()
            });
        }

        /// <summary>
        /// Realiza o logout do usuário, removendo o cookie de autenticação da API.
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }

        /// <summary>
        /// Retorna informações sobre o usuário autenticado. Útil para testar o fluxo de autenticação via cookie da API.
        /// </summary>
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

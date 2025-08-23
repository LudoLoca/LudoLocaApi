using API.Data;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados (PostgreSQL via Entity Framework Core)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do Identity para autenticação baseada em cookie (sem JWT)
// Define regras de senha e exige e-mail único
builder.Services
    .AddIdentity<AppUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Configuração do cookie de autenticação para uso em API (sem redirecionamentos)
// Permite uso cross-site (ex: cliente e API em portas diferentes)
builder.Services.ConfigureApplicationCookie(options =>
{
    // Retorna status HTTP ao invés de redirecionar em casos de não autenticado ou acesso negado
    options.Events.OnRedirectToLogin = ctx => { ctx.Response.StatusCode = StatusCodes.Status401Unauthorized; return Task.CompletedTask; };
    options.Events.OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = StatusCodes.Status403Forbidden; return Task.CompletedTask; };

    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Apenas HTTPS
    options.Cookie.SameSite = SameSiteMode.None;             // Necessário para cookies cross-site
});

// Adiciona suporte a controllers e documentação Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração de CORS para permitir requisições do cliente (origem diferente)
// ATENÇÃO: ajuste a origem conforme necessário para produção
const string ClientCors = "ClientCors";
builder.Services.AddCors(o =>
{
    o.AddPolicy(ClientCors, p =>
        p.WithOrigins("https://localhost:7156")   // Origem do cliente (ajuste para produção)
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());                   // Permite envio de cookies
});

var app = builder.Build();

// Pipeline de execução da aplicação
if (app.Environment.IsDevelopment())
{
    // Ativa Swagger apenas em desenvolvimento
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS deve vir antes de autenticação para permitir preflight e cookies
app.UseCors(ClientCors);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Criação de escopo para seed de dados iniciais (roles e admin)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    // Garante que os papéis "Admin" e "User" existam
    string[] roles = { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
    }

    // Cria o primeiro usuário admin caso não existam usuários
    // TODO: Trocar a senha padrão após o primeiro login em produção
    string adminEmail = "admin@admin.com";
    string adminPassword = "Admin123!"; 

    if (!userManager.Users.Any())
    {
        var adminUser = new AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            RegisteredAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        // TODO: Adicionar logs ou tratamento de erro caso a criação falhe
    }
}

app.Run();

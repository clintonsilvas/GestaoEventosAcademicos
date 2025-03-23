using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GestaoEventosAcademicos.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration["Data:BancoGestaoEventosAcademicos:ConnectionString"]));

// Configuração do Identity
builder.Services.AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<Context>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login/Index"; // Redireciona para a tela de login
    options.AccessDeniedPath = "/Home/AcessoNegado"; // Tela de erro de acesso negado
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await CriarRolesAsync(services);
    await SeedDataAsync(services); // <- Adicionando chamada do Seed Data
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();

async Task CriarRolesAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();

    string[] roles = { "Administrador", "Participante" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
async Task SeedDataAsync(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();
    var context = serviceProvider.GetRequiredService<Context>();

    context.Database.EnsureCreated();

    if (!context.Cursos.Any())
    {
        var cursos = new List<Curso>
        {
            new Curso { Nome = "Ciência da Computação" },
            new Curso { Nome = "Engenharia de Software" },
            new Curso { Nome = "Administração" },
            new Curso { Nome = "Direito" },
            new Curso { Nome = "Medicina" }
        };

        context.Cursos.AddRange(cursos);
        await context.SaveChangesAsync();
    }

    if (!userManager.Users.Any(u => u.TipoUsuario == "Administrador"))
    {
        var admin1 = new Administrador
        {
            UserName = "admin1@evento.com",
            Email = "admin1@evento.com",
            EmailConfirmed = true,
            Nome = "Admin 1",
            Telefone = "35999999999",
            TipoUsuario = "Administrador",
            DataNascimento = new DateTime(1990, 1, 1)
        };
        await userManager.CreateAsync(admin1, "@Adm123");
        await userManager.AddToRoleAsync(admin1, "Administrador");

        var admin2 = new Administrador
        {
            UserName = "admin2@evento.com",
            Email = "admin2@evento.com",
            EmailConfirmed = true,
            Nome = "Admin 2",
            Telefone = "35988888888",
            TipoUsuario = "Administrador",
            DataNascimento = new DateTime(1985, 5, 20)
        };
        await userManager.CreateAsync(admin2, "@Adm123");
        await userManager.AddToRoleAsync(admin2, "Administrador");
    }

    if (!userManager.Users.Any(u => u.TipoUsuario == "Participante"))
    {
        var curso = context.Cursos.First(); // Pegando um curso existente

        var participante1 = new Participante
        {
            UserName = "user1@evento.com",
            Email = "user1@evento.com",
            EmailConfirmed = true,
            Nome = "Participante 1",
            Telefone = "35977777777",
            TipoUsuario = "Participante",
            DataNascimento = new DateTime(2002, 10, 15),
            CursoID = curso.CursoID
        };
        await userManager.CreateAsync(participante1, "@User123");

        var participante2 = new Participante
        {
            UserName = "user2@evento.com",
            Email = "user2@evento.com",
            EmailConfirmed = true,
            Nome = "Participante 2",
            Telefone = "35966666666",
            TipoUsuario = "Participante",
            DataNascimento = new DateTime(2001, 3, 22),
            CursoID = curso.CursoID
        };
        await userManager.CreateAsync(participante2, "@User123");
    }

    if (!context.Eventos.Any())
    {
        var administrador = userManager.Users.OfType<Administrador>().FirstOrDefault();
        if (administrador != null)
        {
            var eventos = new List<Evento>
            {
                new Evento { Nome = "Palestra sobre IA", Local = "Auditório 1", Data = DateTime.Now.AddDays(10), Duração = "2h", CodigoParticipacao = "AI2025", Capacidade = 100, AdministradorID = administrador.Id },
                new Evento { Nome = "Workshop de Programação", Local = "Sala 101", Data = DateTime.Now.AddDays(15), Duração = "3h", CodigoParticipacao = "DEV2025", Capacidade = 50, AdministradorID = administrador.Id }
            };

            context.Eventos.AddRange(eventos);
            await context.SaveChangesAsync();
        }
    }
}
